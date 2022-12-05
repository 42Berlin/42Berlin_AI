using FrostweepGames.Plugins.Core;
using System;
using System.IO;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
#if CT_RTV
    [ExecuteInEditMode]
#endif
  public class GCTextToSpeech : MonoBehaviour
  {
    public event Action<GetVoicesResponse, long> GetVoicesSuccessEvent;
    public event Action<PostSynthesizeResponse, long> SynthesizeSuccessEvent;

    public event Action<string, long> GetVoicesFailedEvent;
    public event Action<string, long> SynthesizeFailedEvent;

    private string newApiKey = string.Empty;

    private static GCTextToSpeech _Instance;
    public static GCTextToSpeech Instance
    {
      get
      {
        if (_Instance == null)
        {
          var obj = Resources.Load<GameObject>("Prefabs/GCTextToSpeech");

          if (obj != null)
          {
            obj.name = "[Singleton]GCTextToSpeech";
            _Instance = obj.GetComponent<GCTextToSpeech>();
          }
          else
            _Instance = new GameObject("[Singleton]GCTextToSpeech").AddComponent<GCTextToSpeech>();
        }

        return _Instance;
      }
    }

    private ITextToSpeechManager _textToSpeechManager;
    private IMediaManager _mediaManager;

    public ServiceLocator ServiceLocator { get { return ServiceLocator.Instance; } }

    [Header("Prefab Object Settings")]
    public bool isDontDestroyOnLoad = false;
    public bool isFullDebugLogIfError = false;

    // [Header("Prefab Fields")]
    // [PasswordField]
    // public string apiKey = string.Empty;

    public static class DotEnv
      {
          public static void Load(string filePath)
          {
              if (!File.Exists(filePath))
                  return;

              foreach (var line in File.ReadAllLines(filePath))
              {
                  var parts = line.Split(
                      '=',
                      StringSplitOptions.RemoveEmptyEntries);

                  if (parts.Length != 2)
                      continue;

                  Environment.SetEnvironmentVariable(parts[0], parts[1]);
              }
          }
      }
  
    private void Awake()
    {
      var dotenv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
      DotEnv.Load(dotenv);
      newApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
      if (
#if CT_RTV
                Application.isPlaying &&
#endif
          _Instance != null)
      {
        Destroy(gameObject);
        return;
      }

      if (isDontDestroyOnLoad)
        DontDestroyOnLoad(gameObject);

      _Instance = this;

      ServiceLocator.Register<ITextToSpeechManager>(new TextToSpeechManager());
      ServiceLocator.Register<IMediaManager>(new MediaManager());
      ServiceLocator.InitServices();

      _textToSpeechManager = ServiceLocator.Get<ITextToSpeechManager>();
      _mediaManager = ServiceLocator.Get<IMediaManager>();

      _textToSpeechManager.GetVoicesSuccessEvent += GetVoicesSuccessEventHandler;
      _textToSpeechManager.SynthesizeSuccessEvent += SynthesizeSuccessEventHandler;

      _textToSpeechManager.GetVoicesFailedEvent += GetVoicesFailedEventHandler;
      _textToSpeechManager.SynthesizeFailedEvent += SynthesizeFailedEventHandler;
    }

    private void Update()
    {
      if (_Instance == this)
      {
        ServiceLocator.Instance.Update();
      }
    }

    private void OnDestroy()
    {
      if (_Instance == this)
      {
        _textToSpeechManager.GetVoicesSuccessEvent -= GetVoicesSuccessEventHandler;
        _textToSpeechManager.SynthesizeSuccessEvent -= SynthesizeSuccessEventHandler;

        _textToSpeechManager.GetVoicesFailedEvent -= GetVoicesFailedEventHandler;
        _textToSpeechManager.SynthesizeFailedEvent -= SynthesizeFailedEventHandler;

        _Instance = null;
        ServiceLocator.Instance.Dispose();
      }
    }

    public string getKey()
    {
      return newApiKey;
    }  

    public string PrepareLanguage(Enumerators.LanguageCode lang)
    {
      return _textToSpeechManager.PrepareLanguage(lang);
    }

    public AudioClip GetAudioClipFromBase64(string value, Enumerators.AudioEncoding audioEncoding)
    {
      return _mediaManager.GetAudioClipFromBase64String(value, audioEncoding);
    }

    public long GetVoices(GetVoicesRequest getVoicesRequest)
    {
      return _textToSpeechManager.GetVoices(getVoicesRequest);
    }

    public long Synthesize(string content, VoiceConfig voiceConfig, bool ssml = false, double pitch = 1.0, double speakingRate = 1.0, double sampleRateHertz = Constants.DEFAULT_SAMPLE_RATE, string[] effectsProfileId = null, Enumerators.TimepointType[] timepoints = null)
    {
      // Debug.Log("VOICE : " + voiceConfig.gender + "-" + voiceConfig.languageCode + "-" + voiceConfig.name);
      // voiceConfig.name = "en-GB-Neural2-C";
      // voiceConfig.languageCode = "en-GB";
      // voiceConfig.gender = Enumerators.SsmlVoiceGender.FEMALE;
      speakingRate = 1;
      
      SynthesisInput synthesisInput = null;

      if (ssml)
        synthesisInput = new SynthesisInputSSML() { ssml = content };
      else
        synthesisInput = new SynthesisInputText() { text = content };

      var request = new PostSynthesizeRequest()
      {
        audioConfig = new AudioConfig()
        {
          audioEncoding = Constants.DEFAULT_AUDIO_ENCODING,
          pitch = pitch,
          sampleRateHertz = sampleRateHertz,
          speakingRate = speakingRate,
          volumeGainDb = Constants.DEFAULT_VOLUME_GAIN_DB,
          effectsProfileId = effectsProfileId == null ? new string[0] : effectsProfileId
        },
        input = synthesisInput,
        voice = new VoiceSelectionParams()
        {
          languageCode = voiceConfig.languageCode,
          name = voiceConfig.name,
          ssmlGender = voiceConfig.gender
        }
      };

      if (GeneralConfig.Config.betaAPI)
      {
        request.enableTimePointing = timepoints;
      }
      // Debug.Log("[GCTTS] Synthesize will actually start");
      return _textToSpeechManager.Synthesize(request);
    }

    public void CancelRequest(long requestId)
    {
      _textToSpeechManager.CancelRequest(requestId);
    }

    public void CancelAllRequests()
    {
      _textToSpeechManager.CancelAllRequests();
    }

    private void GetVoicesFailedEventHandler(string data, long requestId)
    {
      if (GetVoicesFailedEvent != null)
        GetVoicesFailedEvent(data, requestId);
    }

    private void SynthesizeFailedEventHandler(string data, long requestId)
    {
      if (SynthesizeFailedEvent != null)
        SynthesizeFailedEvent(data, requestId);
    }

    private void GetVoicesSuccessEventHandler(GetVoicesResponse data, long requestId)
    {
      if (GetVoicesSuccessEvent != null)
        GetVoicesSuccessEvent(data, requestId);
    }


    private void SynthesizeSuccessEventHandler(PostSynthesizeResponse data, long requestId)
    {
      // Debug.Log("[GCTTS] synthesize success event received (triggering playing the audio clip ?)");
      if (SynthesizeSuccessEvent != null)
        SynthesizeSuccessEvent(data, requestId);
    }
  }
}