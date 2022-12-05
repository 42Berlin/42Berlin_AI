using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Google.Protobuf;
using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using FrostweepGames.Plugins.Native;
using System.Collections;
using UnityEngine.Assertions;
using Grpc.Core;

namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition
{
  public class GCStreamingSpeechRecognition : MonoBehaviour
  {
    public static GCStreamingSpeechRecognition Instance { get; private set; } // allow to instantiate this class
    public Config config; // cf. Scripts/Settings/Config.cs

    private SpeechClient _speechClient;
    private bool _initialized;

    // CustomMicrophone (cf. _Generic/Tools/CustomMicrophone.cs)
    private Coroutine _checkOnMicAndRunStreamRoutine;

    [ReadOnly]
    public bool isRecording;

    private const bool LogExceptions = false;
    private const int SampleRate = 16000; // LINEAR 16 frequency
    private const int StreamingRecognitionTimeLimit = 110; // around 2 minutes. ~ 1.83333334
    private const int AudioChunkSize = SampleRate; // amount of samples

    public event Action StreamingRecognitionStartedEvent;
    public event Action<string> StreamingRecognitionFailedEvent;
    public event Action StreamingRecognitionEndedEvent;
    public event Action<string> InterimResultDetectedEvent;
    public event Action<string> FinalResultDetectedEvent;

    private SpeechClient.StreamingRecognizeStream _streamingRecognizeStream;

    private AudioClip _workingClip;

    private CancellationTokenSource _cancellationToken;
    private float _recordingTime;
    private int _currentSamplePosition;
    private int _previousSamplePosition;
    private float[] _currentAudioSamples;
    private List<byte> _currentRecordedSamples;

    
    private Enumerators.LanguageCode _currentLanguageCode;
    private List<List<string>> _currentRecogntionContext;

    private bool _isRecognizing;
    private float _maxVoiceFrame;

	// Flag to block samples from the microphone
    public bool mic_blocked=false;

    [ReadOnly]
    public string microphoneDevice;

    public System.Diagnostics.Stopwatch watch;
    ////////////////////////////////////////////// AWAKE - CLIENT INITIALISATION //////////////////////////////////////////////

    public string prevWatchMsg = "";
    public string currWatchMsg = "";

    public void printTimeWatch(string msg)
    {
        prevWatchMsg = currWatchMsg;
        currWatchMsg = msg;
        // watch.Stop();
        // long time = watch.ElapsedMilliseconds;
        // if (time > 0)
        // {
        //   Debug.Log($"{time, 10} ms - {currWatchMsg} ---------- (since {prevWatchMsg})");
        // }
        // watch.Restart();
    }

    private void Awake() // should be used to initialize variables or states before the game starts
    { // currently : instanciate new SpeechReco class that will be check at every update
      if (Instance != null)
      {
        Destroy(gameObject);
        return;
      }
      Instance = this;
      Assert.IsNotNull(config, "Config is required to be added."); // where is config defined ?

      watch = new System.Diagnostics.Stopwatch();
      watch.Start();

      printTimeWatch("[I] Awake");
      Initialize();
    }

    private string getGoogleSpeechCredentials() {
      string credentialJson;
      if (config.googleCredentialLoadFromResources)
      {
        Debug.Log("googleCredentialLoadFromResources FOUND");
        if (string.IsNullOrEmpty(config.googleCredentialFilePath) || string.IsNullOrWhiteSpace(config.googleCredentialFilePath))
        {
          Debug.LogException(new Exception("The googleCredentialFilePath is empty. Please fill path to file."));
          return ("");
        }
        TextAsset textAsset = Resources.Load<TextAsset>(config.googleCredentialFilePath);
        if (textAsset == null)
        {
          Debug.LogException(new Exception($"Couldn't load file: {config.googleCredentialFilePath} ."));
          return ("");
        }
        credentialJson = textAsset.text;
      }
      else
      {
        Debug.Log("googleCredentialLoadFromResources NOT FOUND");
        credentialJson = config.googleCredentialJson;
      }
      return (credentialJson);
    }

    private void Initialize() // Initializes speech client for future requests to service
    {
      string credentialJson;

      credentialJson = getGoogleSpeechCredentials();
      if (string.IsNullOrEmpty(credentialJson) || string.IsNullOrWhiteSpace(credentialJson))
      {
        Debug.LogException(new Exception("The Google service account credential is empty."));
        return;
      }
      try
      {
#pragma warning disable CS1701
        _speechClient = new SpeechClientBuilder
        {
          ChannelCredentials = GoogleCredential.FromJson(credentialJson).ToChannelCredentials()
        }.Build();
#pragma warning restore CS1701
        _initialized = true; // new SpeechClient created (with credentials)
        printTimeWatch("[I] Initialize [END]");
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }

    ////////////////////////////////////////////// CLIENT DESTRUCTION //////////////////////////////////////////////

    private async void OnDestroy()
    {
      if (Instance != this || !_initialized) // check initialisation
        return;

      await StopStreamingRecognition(); // wait for all data to be sent successfully
      Instance = null;
    }

    ////////////////////////////////////////////// MICROPHONE SETTINGS //////////////////////////////////////////////

    /// Configures current Microphone device for recording
    public void SetMicrophoneDevice(string deviceName)
    {
      if (isRecording)
        return;
      microphoneDevice = deviceName;
    }

    /// Returns array of connected microphone devices
    public string[] GetMicrophoneDevices()
    {
      return CustomMicrophone.devices;
    }

    /// Requests permission for Microphone device if it not granted
    public void RequestMicrophonePermission()
    {
      printTimeWatch("[I] RequestMicrophonePermission [START]");
      if (!CustomMicrophone.HasMicrophonePermission())
      {
        CustomMicrophone.RequestMicrophonePermission();
      }
      printTimeWatch("[I] RequestMicrophonePermission [END]");
    }

    /// Returns true if at least 1 microphone device is connected
    public bool HasConnectedMicrophoneDevices()
    {
      return CustomMicrophone.HasConnectedMicrophoneDevices();
    }

    /// Requests microphone permission in a coroutine and then runs recognition stream
    private IEnumerator CheckOnMicrophoneAndRunStream()
    {
      while (!HasConnectedMicrophoneDevices())
      {
        RequestMicrophonePermission();
        yield return null;
      }
      RunStreamingRecognition();
      _checkOnMicAndRunStreamRoutine = null;
    }


    ////////////////////////////////////////////// STREAMING RECOGNITION # START //////////////////////////////////////////////

    public void StartStreamingRecognition(Enumerators.LanguageCode languageCode, List<List<string>> context)
    {
      printTimeWatch("[I] StartStreamingRecognition [START]");
      if (!_initialized)
      {
        StreamingRecognitionFailedEvent?.Invoke("Failed to start recogntion due to: 'Not initialized'");
        return;
      }
      _currentRecogntionContext = context;
      _currentLanguageCode = languageCode;
      printTimeWatch("[I] StartCoroutine [START]");
      _checkOnMicAndRunStreamRoutine = StartCoroutine(CheckOnMicrophoneAndRunStream());
      printTimeWatch("[I] StartCoroutine [END]");
      printTimeWatch("[I] StartStreamingRecognition [END]");
    }

    ////////////////////////////////////////////// STREAMING RECOGNITION # RUN //////////////////////////////////////////////

    /// Starts speech recognition stream
    private async void RunStreamingRecognition()
    {
      printTimeWatch("[I] RunStreamingRecognition [START]");
      if (isRecording)
      {
        StreamingRecognitionFailedEvent?.Invoke("Already recording");
        return;
      }

      if (!StartRecording())// todo handle
      {
        StreamingRecognitionFailedEvent?.Invoke("Cannot start recording");
        return;
      }

      _streamingRecognizeStream = _speechClient.StreamingRecognize();

      var recognitionConfig = new RecognitionConfig()
      {
        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
        SampleRateHertz = SampleRate,
        LanguageCode = _currentLanguageCode.Parse(),
        MaxAlternatives = config.maxAlternatives,
        // Model = "latest_short", // TESS ADDITION
        // Model = "command_and_search",
        // Model = "phone_call",
        // UseEnhanced = true,
      };
      if (_currentRecogntionContext != null)
      {
        SpeechContext speechContext;
        foreach (var phrases in _currentRecogntionContext)
        {
          if (phrases != null)
          {
            speechContext = new SpeechContext();
            foreach (var phrase in phrases)
            {
              speechContext.Phrases.Add(phrase);
            }
            recognitionConfig.SpeechContexts.Add(speechContext);
            // Debug.Log("speechContext : " + speechContext);
          }
        }
      }
      printTimeWatch("[I] RunStreamingRecognition [Context set]");

      StreamingRecognitionConfig streamingConfig = new StreamingRecognitionConfig()
      {
        Config = recognitionConfig,
        InterimResults = config.interimResults,
        // SingleUtterance = true, // TESS ADDITION
      };

      try
      {
        await _streamingRecognizeStream.WriteAsync(new StreamingRecognizeRequest()
        {
          StreamingConfig = streamingConfig
        });
      }
      catch (RpcException ex)
      {
        StopRecording();
        _streamingRecognizeStream = null;
        StreamingRecognitionFailedEvent?.Invoke($"Cannot start recognition due to: {ex.Message}");
        return;
      }

      printTimeWatch("[I] RunStreamingRecognition [_isRecognizing = true / startevent]");
      _isRecognizing = true;
      StreamingRecognitionStartedEvent.Invoke();
      _cancellationToken = new CancellationTokenSource();

      HandleStreamingRecognitionResponsesTask();
      printTimeWatch("[I] RunStreamingRecognition [END]");
    }

    /// Handles all speech recognition response asynchronously
    private async void HandleStreamingRecognitionResponsesTask()
    {
      printTimeWatch("[I] HandleResponse [START]");
      try
      {
        while (await _streamingRecognizeStream.GetResponseStream().MoveNextAsync(_cancellationToken.Token))
        {
          printTimeWatch("[I] HandleResponse [in await]"); // VERY LONG
          var current = _streamingRecognizeStream.GetResponseStream().Current;
          if (current == null)
            return;

          var results = _streamingRecognizeStream.GetResponseStream().Current.Results;

          if (results.Count <= 0)
            continue;

          StreamingRecognitionResult result = results[0];
          if (result.Alternatives.Count <= 0)
            continue;
          if (result.IsFinal)
          {
            printTimeWatch("[I] HandleResponse [result is final]");
            FinalResultDetectedEvent.Invoke(result.Alternatives[0].Transcript.Trim());
          }
          else
          {
            if (config.interimResults)
            {
              for (int i = 0; i < config.maxAlternatives; i++)
              {
                if (i >= result.Alternatives.Count)
                  break;
                printTimeWatch("[I] HandleResponse [result is interim]");
                InterimResultDetectedEvent.Invoke(result.Alternatives[i].Transcript.Trim());
              }
            }
          }
        }
        printTimeWatch("[I] HandleResponse [END]");
      }
      catch (Exception ex)
      {
        Debug.Log(ex);
        // if (LogExceptions)
        // {
        //   Debug.LogException(ex);
        // }
      }
    }



    ////////////////////////////////////////////// UPDATE //////////////////////////////////////////////

    public void TriggerFinalResultDetectedEvent(string s)
    {
      FinalResultDetectedEvent.Invoke(s);
    }

    private async void Update()
    {
      if (Instance != this || !_initialized)
        return;

      if (!isRecording)
        return;

      _recordingTime += Time.unscaledDeltaTime;

      if (_recordingTime >= StreamingRecognitionTimeLimit)
      {
        await RestartStreamingRecognitionAfterLimit(); // restart speech recogntion when time limit is reached
        _recordingTime = 0;
      }

      HandleRecordingData(); // handle data from microphone each frame
    }

    private void FixedUpdate()
    {
      if (Instance != this || !_initialized)
        return;

      WriteDataToStream(); // write data to stream each physics frame
    }

    public float GetLastFrame()
    {
      int minValue = SampleRate / 8;

      if (_currentAudioSamples == null)
        return 0;

      int position = Mathf.Clamp(_currentSamplePosition - (minValue + 1), 0, _currentAudioSamples.Length - 1);

      float sum = 0f;
      for (int i = position; i < _currentAudioSamples.Length; i++)
      {
        sum += Mathf.Abs(_currentAudioSamples[i]);
      }

      sum /= minValue;

      return sum;
    }

    public float GetMaxFrame()
    {
      return _maxVoiceFrame;
    }


    /// <summary>
    /// Stops streaming recognition if started
    /// </summary>
    /// <returns></returns>
    public async Task StopStreamingRecognition()
    {
      printTimeWatch("[I] StopStreamingRecognition [START]");
      if (!isRecording || !_isRecognizing)
        return;

      _isRecognizing = false;
      printTimeWatch("[I] StopStreamingRecognition [_isRecognizing = false]");

      StopRecording();

      if (_streamingRecognizeStream != null)
      {
        await _streamingRecognizeStream.WriteCompleteAsync();
      }

      _streamingRecognizeStream = null;
      _currentRecordedSamples = null;

      if (_checkOnMicAndRunStreamRoutine != null)
      {
        StopCoroutine(_checkOnMicAndRunStreamRoutine);
        _checkOnMicAndRunStreamRoutine = null;
      }

      if (_cancellationToken != null)
      {
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = null;
      }

      StreamingRecognitionEndedEvent?.Invoke();
      printTimeWatch("[I] StopStreamingRecognition [END]");
    }

    /// <summary>
    /// Restats automatically streaming recognition when time limit is reached
    /// </summary>
    /// <returns></returns>
    private async Task RestartStreamingRecognitionAfterLimit()
    {
      printTimeWatch("[I] RestartStreamingRecognitionAfterLimit [START]");
      await StopStreamingRecognition();
      printTimeWatch("[I] (re)StartCoroutine [START]");
      _checkOnMicAndRunStreamRoutine = StartCoroutine(CheckOnMicrophoneAndRunStream());
      printTimeWatch("[I] (re)StartCoroutine [END]");
      printTimeWatch("[I] RestartStreamingRecognitionAfterLimit [END]");
    }

    /// <summary>
    /// Starts microphone device recording
    /// </summary>
    /// <returns></returns>
    private bool StartRecording()
    {
      printTimeWatch("[I] StartRecording [START]");
      if (string.IsNullOrEmpty(microphoneDevice))
        return false;

      _workingClip = CustomMicrophone.Start(microphoneDevice, true, 3, SampleRate);

      _currentAudioSamples = new float[_workingClip.samples];

      _currentRecordedSamples = new List<byte>();

      isRecording = true;
      _maxVoiceFrame = 0;

      printTimeWatch("[I] StartRecording [END]");
      return true;
    }

    /// <summary>
    /// stops microphone device recording and lceaning recording data
    /// </summary>
    public void StopRecording()
    {
      printTimeWatch("[I] StopRecording [START]");
      if (!isRecording)
        return;

      if (string.IsNullOrEmpty(microphoneDevice))
        return;

      CustomMicrophone.End(microphoneDevice);
      printTimeWatch("[I] StopRecording [CustomMicrophone.End(microphoneDevice)]");

      MonoBehaviour.Destroy(_workingClip);

      _currentRecordedSamples.Clear();

      isRecording = false;
      printTimeWatch("[I] StoptRecording [END / isRecording = false]");
    }

    /// <summary>
    /// Writes recordign data from Microphone device to buffer for sending to service
    /// </summary>
    private void HandleRecordingData()
    {
      if (!isRecording)
        return;

      // printTimeWatch("[I] HandleRecordingData [START]");
      _currentSamplePosition = CustomMicrophone.GetPosition(microphoneDevice);

      if (CustomMicrophone.GetRawData(ref _currentAudioSamples, _workingClip))
      {
        if (_previousSamplePosition > _currentSamplePosition)
        {
          for (int i = _previousSamplePosition; i < _currentAudioSamples.Length; i++)
          {
            if (_currentAudioSamples[i] > _maxVoiceFrame)
              _maxVoiceFrame = _currentAudioSamples[i];

            _currentRecordedSamples.AddRange(FloatToBytes(_currentAudioSamples[i]));
          }

          _previousSamplePosition = 0;
        }

        for (int i = _previousSamplePosition; i < _currentSamplePosition; i++)
        {
          if (_currentAudioSamples[i] > _maxVoiceFrame)
            _maxVoiceFrame = _currentAudioSamples[i];

          _currentRecordedSamples.AddRange(FloatToBytes(_currentAudioSamples[i]));
        }

        _previousSamplePosition = _currentSamplePosition;
      }
      // printTimeWatch("[I] HandleRecordingData [END]");
    }

    /// <summary>
    /// Sends samples asynchronously in stream
    /// </summary>
    private async void WriteDataToStream()
    {
      if (_streamingRecognizeStream == null)
        return;

      ByteString chunk;
      List<byte> samplesChunk = null;

      if (isRecording || (_currentRecordedSamples != null && _currentRecordedSamples.Count > 0))
      {
        if (_currentRecordedSamples.Count >= AudioChunkSize * 2)
        {
          samplesChunk = _currentRecordedSamples.GetRange(0, AudioChunkSize * 2);
          _currentRecordedSamples.RemoveRange(0, AudioChunkSize * 2);
        }
        else if (!isRecording)
        {
          samplesChunk = _currentRecordedSamples.GetRange(0, _currentRecordedSamples.Count);
          _currentRecordedSamples.Clear();
        }

        if (samplesChunk != null && samplesChunk.Count > 0)
        {
          chunk = ByteString.CopyFrom(samplesChunk.ToArray(), 0, samplesChunk.Count);

          try
          {
            await _streamingRecognizeStream.WriteAsync(new StreamingRecognizeRequest() { AudioContent = chunk });
          }
          catch (RpcException ex)
          {
            StreamingRecognitionFailedEvent?.Invoke($"Cannot proceed recognition due to: {ex.Message}");

            _streamingRecognizeStream = null;

            await StopStreamingRecognition();
          }
        }
      }
    }
    private byte[] FloatToBytes(float sample)
    {
      if (mic_blocked) return System.BitConverter.GetBytes((short)(0.0f));
      
      return System.BitConverter.GetBytes((short)(sample * 32767));
    }
  }
}