using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class ImageChanger : MonoBehaviour
{
    public static ImageChanger instance;
    [SerializeField]
    private VisualEffect visual;
    public Texture2D[] image;
    int current = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
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
            if (current >= image.Length)
            {
                current = 0;
            }
            else
            {
                visual.SetTexture("img", image[current]);
                current++;
            }
            
        }
        
    }
}
