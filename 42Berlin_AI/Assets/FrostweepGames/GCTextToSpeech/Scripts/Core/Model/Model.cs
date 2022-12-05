﻿namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    #region requests

    [UnityEngine.Scripting.Preserve]
    public class GetVoicesRequest
    {
        public string languageCode;
    }

    [UnityEngine.Scripting.Preserve]
    public class PostSynthesizeRequest
    {
        public SynthesisInput input;
        public VoiceSelectionParams voice;
        public AudioConfig audioConfig;

        /// <summary>
        /// BETA API
        /// </summary>
        public Enumerators.TimepointType[] enableTimePointing;
    }

    #endregion

    #region models

    [UnityEngine.Scripting.Preserve]
    public class SynthesisInput
    {
    }

    [UnityEngine.Scripting.Preserve]
    public class SynthesisInputText : SynthesisInput
    {
        public string text;
    }

    [UnityEngine.Scripting.Preserve]
    public class SynthesisInputSSML : SynthesisInput
    {
        public string ssml;
    }

    [UnityEngine.Scripting.Preserve]
    public class VoiceSelectionParams
    {
        public string languageCode;
        public string name;
        public Enumerators.SsmlVoiceGender ssmlGender;

        /// <summary>
        /// BETA API
        /// </summary>
        public CustomVoiceParams customVoice;
    }

    [UnityEngine.Scripting.Preserve]
    public class AudioConfig
    {

        public Enumerators.AudioEncoding audioEncoding;
        public double speakingRate;
        public double pitch;
        public double volumeGainDb;
        public double sampleRateHertz;

        /// <summary> 
        /// wearable-class-device -	Smart watches and other wearables, like Apple Watch, Wear OS watch <br />
        /// handset-class-device - Smartphones, like Google Pixel, Samsung Galaxy, Apple iPhone<br />
        /// headphone-class-device - Earbuds or headphones for audio playback, like Sennheiser headphones<br />
        /// small-bluetooth-speaker-class-device - Small home speakers, like Google Home Mini<br />
        /// medium-bluetooth-speaker-class-device - mart home speakers, like Google Home<br />
        /// large-home-entertainment-class-device  - Home entertainment systems or smart TVs, like Google Home Max, LG TV<br />
        /// large-automotive-class-device - ar speakers<br />
        /// telephony-class-application - Interactive Voice Response(IVR) systems
        /// </summary>
        public string[] effectsProfileId;
    }

    [UnityEngine.Scripting.Preserve]
    public class Voice
    {
        public string[] languageCodes;
        public string name;
        public Enumerators.SsmlVoiceGender ssmlGender;
        public double naturalSampleRateHertz;
    }

    #endregion

    #region responses

    [UnityEngine.Scripting.Preserve]
    public class GetVoicesResponse
    {
        public Voice[] voices;
    }

    [UnityEngine.Scripting.Preserve]
    public class PostSynthesizeResponse
    {
        public string audioContent; // base64 string

        /// <summary>
        /// BETA API
        /// </summary>
        public Timepoint[] timepoints;

        /// <summary>
        /// BETA API
        /// </summary>
        public AudioConfig audioConfig;
    }

    #endregion

    [UnityEngine.Scripting.Preserve]
    public class VoiceConfig
    {
        public string languageCode;
        public string name;
        public Enumerators.SsmlVoiceGender gender;
    }

    [UnityEngine.Scripting.Preserve]
    public class Timepoint
    {
        public string markName;
        public double timeSeconds;
    }

    [UnityEngine.Scripting.Preserve]
    public class CustomVoiceParams
    {
        public string model;
        public Enumerators.ReportedUsage reportedUsage;
    }
}