using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectingHuman : MonoBehaviour
{
    public static DirectingHuman instance;
    [SerializeField]
    Animator[] anim;


    void Start()
    {
        if (instance !=null)
        {
            Destroy(instance);
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

    public void DirectingHumanState(bool idle)
    {
        foreach (var item in anim)
        {
            item.SetBool("idle",idle);
        }
    }

  
}
