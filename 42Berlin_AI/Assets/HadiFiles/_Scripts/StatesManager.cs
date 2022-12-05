    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;
using UnityEngine.Video;
using UnityEngine.Events;

public class StatesManager : MonoBehaviour
{
    public static StatesManager instance;

    [Header("Elements")]
    [SerializeField] private Animator imgViewerAnimator;
    [SerializeField] private Animator listening_AnimatorController;
    [SerializeField] private VisualEffect imgViewrParticleSystem;
    [SerializeField] private VisualEffect bearParticleSystem;
    [SerializeField] private VisualEffect processingParticleSystem;
    [SerializeField] private GameObject listeningParticleSystem;
    [SerializeField] private MeshRenderer tvObject;
    [SerializeField] private Texture2D[] images;
    public VideoPlayer videoPlayer;
    public GameObject[] imageViewer;
    [Header("Settings")]
    [SerializeField] private int minParticle = 0;
    [SerializeField] private int maxParticle = 100000;
    [SerializeField] private int rateChangeSpeed = 1;
    [SerializeField] private int minLifetme = 1;
    [SerializeField] private int maxLifeTime = 3;
    [SerializeField] private float minTurbulenceIntensity = 1;
    [SerializeField] private float maxTurbulenceIntensity = 1;
    [SerializeField] private float turbulenceChangerSpeed = 1;
    [SerializeField] [Range(0f, 3f)] float lerpTime;
    [SerializeField] private bool colorOnBool;
    [SerializeField] private Color colorOn;
    [SerializeField] private Color colorOff;

    // bear scene settings
    public bool     _sceneIdleState = true;
    public int      _particleLifeTime = 0;
    public bool     _listenParm = false;
    public int      _currentAnimation = 0;

    // photos settings
    public bool     _imageViewerActive = false;
    public int      _currentImage_A = 0;
    public int      _currentImage_B = 0;

    // videos settings
    public bool     _tvOn = false;
    public int      _currentVideo = 0;
    public bool     _videoAudioOn = true;
    public bool     _videoHideBear = false;

    public double   videoLength;
    public double   lastVideoPlayerTime;
    public  UnityEvent<bool> OnVideoFinished = new UnityEvent<bool>();

    public bool     _greeting = false;

    public enum BearState
    {
        SLEEPING,
        GREETING,
        START_LISTENING,
        LISTENING,
        PROCESSING,
        SPEAKING,
        SHOWING,
        GOODBYE
    }
    private BearState _bearState;
    // public CameraController cameraController;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    void Start()
    {
        // cameraController = CameraController.instance;
    }

    void Update()
    {
        if (_tvOn)
        {   
            if (videoLength > 0)
            {
                if (_videoHideBear == true)
                {
                    if ((videoLength - videoPlayer.time <= 0.06))
                    {
                        displayVideo(false, 0, true, true);
                        OnVideoFinished.Invoke(_videoHideBear);
                    }
                    lastVideoPlayerTime = videoPlayer.time;
                }
            }
        }
     }

    public void printVideosVariables()
    {
        Debug.Log(
        "[_tvOn : " + _tvOn + "] " +
        "[_currentVideo : " + _currentVideo + "] " +
        "[_videoAudioOn : " + _videoAudioOn + "] " +
        "[_videoHideBear : " + _videoHideBear + "] "
        );
    }

    public void displayVideo(bool tvOn, int currentVideo, bool videoAudioOn, bool videoHideBear)
    {
        _tvOn = tvOn;
        _currentVideo = currentVideo;
        _videoAudioOn = videoAudioOn;
        _videoHideBear = videoHideBear;
    }

    public void displayPhotos(bool imageViewerActive, int currentImage_A=0, int currentImage_B=0)
    {
        _imageViewerActive = imageViewerActive;
        _currentImage_A = currentImage_A;
        _currentImage_B = currentImage_B;
    }

    public void updateStatesVariables()
    {
        bool    tmp_scene_idle_state = false;
        int     tmp_particleLifeTime = 0;
        bool    tmp_listenParm = false;
        int     tmp_currentAnimation = 0;
        bool    tmp_greeting = false;

        if (_bearState == BearState.SLEEPING)
        {
            tmp_scene_idle_state = true;
            // if (cameraController != null)
            //     cameraController.BearFocus(1,1,1);
        }
        else if (_bearState == BearState.GREETING)
        {
            tmp_currentAnimation = 2;
            tmp_greeting = true;
            // if (cameraController != null)
            //     cameraController.BearFocus(1,1,1);
        }
        else if (_bearState == BearState.START_LISTENING)
        {
            tmp_listenParm = true;
        }
        else if (_bearState == BearState.LISTENING)
        {
            tmp_listenParm = true;
            // if (cameraController != null)
            //     cameraController.Processing(1,2,4);
        }
        else if (_bearState == BearState.PROCESSING)
        {
            tmp_particleLifeTime = 2;
            // if (cameraController != null)
            //     cameraController.Processing(1,2,4);
        }
        else if (_bearState == BearState.SPEAKING)
        {
            tmp_currentAnimation = 1;
            // if (cameraController != null)
            //     cameraController.BearFocus(1,1,1);
        }
        else if (_bearState == BearState.GOODBYE)
        {
            tmp_scene_idle_state = true;
            tmp_currentAnimation = 3;
            // if (cameraController != null)
            //     cameraController.BearFocus(1,1,1);
        }
        _sceneIdleState = tmp_scene_idle_state;
        _particleLifeTime = tmp_particleLifeTime;
        _listenParm = tmp_listenParm;
        _currentAnimation = tmp_currentAnimation;
        _greeting = tmp_greeting;
    }

    public void printStatesVariables()
    {
        Debug.Log(
        "[_bearState : " + _bearState + "] " +
        "[_sceneIdleState : " + _sceneIdleState + "] " +
        "[_particleLifeTime : " + _particleLifeTime + "] " +
        "[_listenParm : " + _listenParm + "] " +
        "[_currentAnimation : " + _currentAnimation + "] " +
        "[_tvOn : " + _tvOn + "] " +
        "[_currentVideo : " + _currentVideo + "] " +
        "[_videoAudioOn : " + _videoAudioOn + "] " +
        "[_videoHideBear : " + _videoHideBear + "] " +
        "[_imageViewerActive : " + _imageViewerActive + "] " +
        "[_currentImage_A : " + _currentImage_A + "] " +
        "[_currentImage_B : " + _currentImage_B + "] "
        );
    }

    public void updateState(BearState newState)
    {
        Debug.Log("________ updateState ________ from " + _bearState + " to " + newState);
        _bearState = newState;
        updateStatesVariables();
        printStatesVariables();
    }
    public void SceneIdleState(bool sceneIdleState)
    {
        if (_greeting == true)
        {
            // Debug.Log("GREETING (talking)");
            imgViewerAnimator.SetBool("idle", false);
            imgViewerAnimator.SetBool("standing", false);
            imgViewerAnimator.SetBool("talking", true);
            imgViewrParticleSystem.SetTexture("current_Image", images[0]);
        }
        else if (!_sceneIdleState) // NOT SLEEPING
        {
            // Debug.Log("NOT SLEEPING (idle)");
            imgViewerAnimator.SetBool("idle", true);
            imgViewerAnimator.SetBool("standing", false);
            imgViewerAnimator.SetBool("talking", false);
            imgViewrParticleSystem.SetFloat("HeightReference", 1.0f);
            imgViewrParticleSystem.SetFloat("Color_Intensity", 1.0f);
            imgViewrParticleSystem.SetTexture("current_Image", images[0]);
            bearParticleSystem.SetFloat("Min Lifetime", 1);
            bearParticleSystem.SetFloat("Max Lifetime", 3);
            //bearParticleSystem.SetFloat("Blend _Velocity", 0.2f);
            //bearParticleSystem.SetFloat("Rate", Mathf.RoundToInt(Mathf.SmoothStep(minParticle, maxParticle, rateChangeSpeed)));
            //bearParticleSystem.SetFloat("Turbulence Intensity", Mathf.RoundToInt(Mathf.SmoothStep(maxTurbulenceIntensity, minTurbulenceIntensity, turbulenceChangerSpeed)));
 
        }
        else // SLEEPING / IDLE
        {
            // Debug.Log("SLEEPING (standing)");
            imgViewerAnimator.SetBool("idle", false);
            imgViewerAnimator.SetBool("standing", true);
            imgViewerAnimator.SetBool("talking", false);
            imgViewrParticleSystem.SetFloat("HeightReference", Mathf.SmoothStep(1.0f, 0.8f, 2.0f));
            imgViewrParticleSystem.SetFloat("Color_Intensity", 1.0f);
            imgViewrParticleSystem.SetTexture("current_Image", images[1]);
            bearParticleSystem.SetFloat("Min Lifetime", 0);
            bearParticleSystem.SetFloat("Max Lifetime", 0);
            //bearParticleSystem.SetFloat("Blend _Velocity", 0.5f);
            //bearParticleSystem.SetFloat("Turbulence Intensity", Mathf.RoundToInt(Mathf.SmoothStep(minTurbulenceIntensity, maxTurbulenceIntensity, turbulenceChangerSpeed)));
            //bearParticleSystem.SetFloat("Rate", Mathf.RoundToInt(Mathf.SmoothStep(maxParticle, minParticle, rateChangeSpeed)));
        }
    }

    void TVController(bool boolColor)
    {
        if (boolColor)
            tvObject.material.color = Color.Lerp(tvObject.material.color, colorOn, lerpTime * Time.deltaTime);
        else
            tvObject.material.color = Color.Lerp(tvObject.material.color, colorOff, lerpTime * Time.deltaTime);
    }

    //If nobody is there it must be true if not false
    public void Initializing(bool sceneIdleState)
    {
        SceneIdleState(sceneIdleState);
    }

    //While searching for an answer enter the number 2 and after that 0
    public void Processing(int particleLifeTime)
    {
        processingParticleSystem.SetFloat("Lifetime", Mathf.RoundToInt(_particleLifeTime));
    }

    //Set with a bool if the bear is hearing or not
    public void StartListening(bool listenParm)
    {
        listening_AnimatorController.SetBool("on", _listenParm);
    }

    //To show users exactly where to stand
    public void DirectingTheHuman(bool idle)
    {
        // DirectingHuman.instance.DirectingHumanState(idle);
    }

    //Give the correct name of animations for both of Animator controller
    public void CurrentAnimation(int currentAnimation)
    {
        BearController.instance.CurrentAnimation(_currentAnimation);
    }

    //To enable and disable the TV for showing images and videos
    public void VideoViewer(bool tvOn, int currentVideo)
    {
        TVController(_tvOn);
        if (_tvOn || _imageViewerActive)
        {
            processingParticleSystem.gameObject.SetActive(false);
            listeningParticleSystem.gameObject.SetActive(false);
        }
        else
        {
            processingParticleSystem.gameObject.SetActive(true);
            listeningParticleSystem.gameObject.SetActive(true);
        }
        videoPlayer.clip = VideosHolder.instance.video[_currentVideo];
        videoLength = videoPlayer.clip.length;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        if (_videoHideBear)
            bearParticleSystem.gameObject.SetActive(false);
        else
            bearParticleSystem.gameObject.SetActive(true);
    }

    public void ImageViewer(bool imageViewerActive, int currentImage_A, int currentImage_B)
    {
        if (_imageViewerActive)
        {
            foreach (var viewer in imageViewer)
                viewer.SetActive(_imageViewerActive);
            imageViewer[0].GetComponent<MeshRenderer>().material.mainTexture = ImagesHolder.instance.image[_currentImage_A];
            imageViewer[1].GetComponent<MeshRenderer>().material.mainTexture = ImagesHolder.instance.image[_currentImage_B];
        }
        else
        {
            foreach (var viewer in imageViewer)
                viewer.SetActive(_imageViewerActive);
        }
    }
}
