using System.IO;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition;
using FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition.Examples;
using SimpleJSON;
using System.Threading.Tasks;
public class AIManager : MonoBehaviour
{
  public GC_TextToSpeech_TutorialExample TextToSpeech;
  public GCSSR_Example sstAPI;
  public TCPClient tcpClient;
  public GCStreamingSpeechRecognition SpeechRecognition;
  public FaceManager faceTracking;
  public BearTarget BearTarget;
  public GameObject ttsUI;
  public GameObject sttUI;
  public GameObject aifaceUI;
  public GameObject faceTrackerUI;
  public GameObject tcpclientUI;
  private AIConfig ConfigData;
  string ConfigFile = "42config.json";

  public string _currentResponseType;

  // variables for chunked response
  public JSONArray  _payload;
  public int        _currentChunk;

  public enum CamState
  {
    IDLE,
    HUMAN,
    HUMAN_IN_POSITION
  }

  public enum VoiceType
  {
    MALE,
    FEMALE
  }

  public CamState camstate;
  public StatesManager.BearState bearState;
  public Text aistateText;
  public Text camStateText;
  public Text promptText;
  string EOS_ideal_ts;
  string EOS_final_ts;
  public GameObject microPhoneImage;
  public GameObject cutedMicrophoneImage;
  public StatesManager statesManager;
  public VoiceType _voiceType;
  System.Random random;

  void randomlyUpdateVoiceType()
  {
    Array values = Enum.GetValues(typeof(VoiceType));
    _voiceType = (VoiceType)values.GetValue(random.Next(values.Length));
    // Debug.Log("randomlyUpdateVoiceType to : " + _voiceType.ToString());
    if (TextToSpeech)
      TextToSpeech.updateCurrentVoiceConfig(_voiceType.ToString());
    else
      Debug.Log("----- VoiceConfig update failed : TextToSpeech is null -----");
  }

  void Start()
  {
    Debug.Log("________ [AI MANAGER] START ________");
    random = new System.Random();
    statesManager = StatesManager.instance;
    updateBearState(StatesManager.BearState.SLEEPING);
   // AudioManager.instance.AudioClipPlayer(10);
    randomlyUpdateVoiceType();
    camstate = CamState.IDLE;
    cutedMicrophoneImage = GameObject.Find("CutedMicrophoneIcon");
    cutedMicrophoneImage.SetActive(true);
    ConfigData = new AIConfig();

    readFile();
    writeFile(); // Always write file too, to write new Config variables into file. Delete for default.
    BlockMicrophone();

  // ################################################### UTILS ###################################################

    void updateBearState(StatesManager.BearState newState)
    {
      statesManager.updateState(newState);
      bearState = newState;
    }

    void BlockMicrophone()
    {
        SpeechRecognition.mic_blocked=true;
        microPhoneImage.SetActive(false);
        cutedMicrophoneImage.SetActive(true);
    }

    void printMessageReceived(JSONNode n)
    {
      Debug.Log(
        "type: " + n["type"] + " " +
        "author: " + n["author"] + " " +
        "string: " + n["string"] + " " +
        "videoId: " + n["videoId"] + " " +
        "photo1Id: " + n["photo1Id"] + " " +
        "photo2Id: " + n["photo2Id"] + " "
        // "payload: " + n["payload"]
      );
    }

    void change_to_listening_or_sleeping()
    {
      statesManager.displayVideo(false, 0, true, false);
      statesManager.displayPhotos(false, 0, 0);
      if (faceTracking.getIsHuman() == true)
      {
        StartCoroutine(WaitBeforeActivatingMic());
        updateBearState(StatesManager.BearState.LISTENING);
        AudioManager.instance.AudioClipPlayer(9);
      } 
      else
      {
        updateBearState(StatesManager.BearState.SLEEPING);
       AudioManager.instance.AudioClipPlayer(10);
      }

      promptText.text="";
    }

    // ################################################### SPEECH RECOGNITION LISTENERS ###################################################

    SpeechRecognition.FinalResultDetectedEvent += (string s) => // Speech recognition done, 
      {
        EOS_final_ts = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff");
        updateBearState(StatesManager.BearState.PROCESSING);
        AudioManager.instance.AudioClipPlayer(7);
        BlockMicrophone();
        promptText.text=s;
        JSONNode n = new JSONObject();
        n["type"] = "message";
        n["author"] = "UnityClient";
        n["string"] = s;
        n["EOS_final_ts"] = EOS_final_ts;
        n["EOS_ideal_ts"] = EOS_ideal_ts;
        tcpClient.SendJSONMessage(n);
      };

    // ################################################### STATES MANAGER LISTENERS ###################################################

    statesManager.OnVideoFinished.AddListener(delegate (bool videoHideBear)
    {
      change_to_listening_or_sleeping();
    });

    // ################################################### TCP CLIENT LISTENERS ###################################################

    tcpClient.OnConnected.AddListener(delegate
    {
      Debug.Log("TCP Connected.");
    });

    tcpClient.OnMessageReceived.AddListener(delegate (JSONNode n)
    {
      _currentResponseType = n["type"];
      updateBearState(StatesManager.BearState.PROCESSING);
      BlockMicrophone();
      TextToSpeech.SynthesizeString(n["string"], _voiceType.ToString());
    });

    tcpClient.OnPhotoBackgroundReceived.AddListener(delegate (JSONNode n)
    {
      _currentResponseType = n["type"];
      printMessageReceived(n);
      BlockMicrophone();
      Debug.Log(n); // JSONObject

      _payload = n["payload"].AsArray;
      if (_payload == null)
      {
        TextToSpeech.SynthesizeString(n["string"], _voiceType.ToString());
        statesManager.displayPhotos(true, n["photo1Id"], n["photo2Id"]);
      }
      else
      {
        _currentChunk = 0;
        TextToSpeech._chunkInProgress = true;
        statesManager.displayPhotos(true, _payload[_currentChunk]["photos"][0], _payload[_currentChunk]["photos"][1]);
        TextToSpeech.SynthesizeString(_payload[_currentChunk]["text"], _voiceType.ToString());
      }
    });

   tcpClient.OnVideoBackgroundReceived.AddListener(delegate (JSONNode n)
    {
      _currentResponseType = n["type"];
      printMessageReceived(n);
      BlockMicrophone();
      Debug.Log(n);

      _payload = n["payload"].AsArray;
      if (_payload == null)
      {
        TextToSpeech.SynthesizeString(n["string"], _voiceType.ToString());
        statesManager.displayVideo(true, n["videoId"], false, false);
      }
      else
      {
        _currentChunk = 0;
        TextToSpeech._chunkInProgress = true;
        statesManager.displayVideo(true, _payload[_currentChunk]["video"], false, false);
        TextToSpeech.SynthesizeString(_payload[_currentChunk]["text"], _voiceType.ToString());
      }
    });

    tcpClient.OnVideoOnlyReceived.AddListener(delegate (JSONNode n)
    {
      _currentResponseType = n["type"];
      BlockMicrophone();
      _payload = n["payload"].AsArray;
      statesManager.displayVideo(true, _payload[0]["video"], true, true);
      Debug.Log(n);
    });

    // ################################################### TEXT TO SPEECH LISTENERS ###################################################

    if (TextToSpeech._gcTextToSpeech != null)
    {
      TextToSpeech._gcTextToSpeech.SynthesizeSuccessEvent += delegate
      {
        updateBearState(StatesManager.BearState.SPEAKING);
      };
    }

    TextToSpeech.DonePlayingSynthesisEvent += () =>
    {
      Debug.Log("[AI MANAGER] done playing normal audio clip");
      change_to_listening_or_sleeping();
    };

    TextToSpeech.DonePlayingChunkEvent += () =>
    {
      Debug.Log("[AI MANAGER] done playing chunk audio clip");
      _currentChunk++;
      if (_currentChunk < _payload.Count)
      {
        if (_currentResponseType == "photo_background")
          statesManager.displayPhotos(true, _payload[_currentChunk]["photos"][0], _payload[_currentChunk]["photos"][1]);
        else if (_currentResponseType == "video_background")
          statesManager.displayVideo(true, _payload[_currentChunk]["video"], false, false);
        TextToSpeech.SynthesizeString(_payload[_currentChunk]["text"], _voiceType.ToString());
      }
      else
      {
        TextToSpeech._chunkInProgress = false;
        change_to_listening_or_sleeping();
      }
    };

    // ################################################### FACE TRACKING LISTENERS ###################################################

    faceTracking.BearMovement += (float xPosition, float yPosition) =>
    {
      camstate = CamState.HUMAN;
      BearTarget.rotateAt(xPosition, yPosition);
    };

    faceTracking.OnLostHumans += () =>
    {
      camstate = CamState.IDLE;
      updateBearState(StatesManager.BearState.SLEEPING);
     AudioManager.instance.AudioClipPlayer(10);
      BlockMicrophone();
    };

    faceTracking.OnNewHumanDetected += (List<byte[]> faces_bytes) =>
    {
      // Send connect request to server, which return hello msg -> Speech recognition actived after T2S -> Human can chat
      updateBearState(StatesManager.BearState.GREETING);
      AudioManager.instance.AudioClipPlayer(11);
      randomlyUpdateVoiceType();
      JSONNode n = new JSONObject();
      n["type"] = "connect";
      n["author"] = "Client";
      var intArr = n["string"].AsArray;
      foreach(var val in faces_bytes)
          intArr.Add(val);
      tcpClient.SendJSONMessage(n);
      Debug.Log("sent 'connect' message to server - new face detected (" + n + ")");
    };
    SwitchAllUIOff();
    aifaceUI.SetActive(true);
  }


  IEnumerator WaitBeforeActivatingMic(){
    yield return new WaitForSeconds(1.0f);
    Debug.Log("Activating microphone");
    cutedMicrophoneImage.SetActive(false);
    microPhoneImage.SetActive(true);
    SpeechRecognition.mic_blocked=false;
  }

  public void SwitchAllUIOff()
  {
    ttsUI.SetActive(false);
    sttUI.SetActive(false);
    faceTrackerUI.SetActive(false);
    tcpclientUI.SetActive(false);
  }

  void Update()
  {
    aistateText.text = bearState.ToString();
    camStateText.text=camstate.ToString();
    if (Input.GetKeyDown("m"))
      EOS_ideal_ts = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff");
    if (Input.GetKeyDown(KeyCode.N))
      aifaceUI.GetComponent<AIFaceModes>().NextMode();
    if (Input.GetKeyDown(KeyCode.F1))
    {
      SwitchAllUIOff();
      aifaceUI.SetActive(true);
    }
    if (Input.GetKeyDown(KeyCode.F2))
    {
      SwitchAllUIOff();
      ttsUI.SetActive(true);
    }
    if (Input.GetKeyDown(KeyCode.F3))
    {
      SwitchAllUIOff();
      sttUI.SetActive(true);
    }
    if (Input.GetKeyDown(KeyCode.F4))
    {
      SwitchAllUIOff();
      faceTrackerUI.SetActive(true);
    }
    if (Input.GetKeyDown(KeyCode.F5))
    {
      SwitchAllUIOff();
      tcpclientUI.SetActive(true);
    }
  }

  public void readFile()
  {
    if (File.Exists(ConfigFile))
    {
      Debug.Log("Found Config File " + ConfigFile);
      string fileContents = File.ReadAllText(ConfigFile);
      ConfigData = JsonUtility.FromJson<AIConfig>(fileContents);
    }
    else
      writeFile();
  }

  public void writeFile()
  {
    string jsonString = JsonUtility.ToJson(ConfigData);
    jsonString = JsonUtility.ToJson(JsonUtility.FromJson<AIConfig>(jsonString), true);
    Debug.Log("Saving config file json " + jsonString);
    File.WriteAllText(ConfigFile, jsonString);
  }
}
