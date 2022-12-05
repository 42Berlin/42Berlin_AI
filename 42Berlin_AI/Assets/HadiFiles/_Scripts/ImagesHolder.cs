using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagesHolder : MonoBehaviour
{
    public static ImagesHolder instance;
    public Texture2D[] image;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
