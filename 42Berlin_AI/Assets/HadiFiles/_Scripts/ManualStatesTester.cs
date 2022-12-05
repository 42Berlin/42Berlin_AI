using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualStatesTester : MonoBehaviour
{
    public static ManualStatesTester instance;

    public string testBodyAnimationName;
    public string testFaceAnimationName;
    public int lifetime;
    public bool listen;
    public bool sceneState;
    public bool tvOn;
    public int currentAnimation;
    public bool imageViewerActive;
    public int currentImage_A;
    public int currentImage_B;
    public int currentVideo;

    public bool directinHuman;


    private void Awake()
    {
        if (instance!=null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
        if (StatesManager.instance)
        {
            StatesManager.instance.StartListening(listen);
            StatesManager.instance.Processing(lifetime);
            StatesManager.instance.Initializing(sceneState);
            StatesManager.instance.VideoViewer(tvOn, currentVideo);
            StatesManager.instance.ImageViewer(imageViewerActive, currentImage_A,currentImage_B);
            StatesManager.instance.CurrentAnimation(currentAnimation);
        }
        if (DirectingHuman.instance)
            DirectingHuman.instance.DirectingHumanState(directinHuman); 
    }

    
}
