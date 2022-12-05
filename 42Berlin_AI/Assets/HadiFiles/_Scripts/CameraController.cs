using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public CinemachineTargetGroup cinemachineTargetGroup;

    void Awake()
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
    
    // void Start()
    // {
    //     BearFocus(1,1,1);
    // }

    void Update()
    {
        //Manual Temp. Tester
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ImageViewer(Random.Range(0.5f,1f),3,5);
            Processing(1,2,4);
            BearFocus(1,3,5);
            Listenning(1,3,5);
        }

        
    }


    //My suggestion: min and max = 3 & 5
    public void ImageViewer(float weight,float minRadius, float maxRadius)
    {
        float radiusOfTarget = Random.Range(minRadius, maxRadius);
        cinemachineTargetGroup.m_Targets[0].weight = weight;
        cinemachineTargetGroup.m_Targets[0].radius = radiusOfTarget;

        for (int i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            if (i !=0)
            {
                cinemachineTargetGroup.m_Targets[i].weight = 0;
            }
        }
    }

    //My suggestion: min and max = 2 & 4
    public void Processing(float weight, float minRadius, float maxRadius)
    {
        float radiusOfTarget = Random.Range(minRadius, maxRadius);
        cinemachineTargetGroup.m_Targets[1].weight = weight;
        cinemachineTargetGroup.m_Targets[1].radius = radiusOfTarget;

        for (int i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            if (i != 1)
            {
                cinemachineTargetGroup.m_Targets[i].weight = 0;
            }
        }
    }

    // My suggestion: min and max = 3 & 5
    public void BearFocus(float weight, float minRadius, float maxRadius)
    {
        float radiusOfTarget = Random.Range(minRadius, maxRadius);
        cinemachineTargetGroup.m_Targets[2].weight = weight;    
        cinemachineTargetGroup.m_Targets[3].weight = weight;    
        cinemachineTargetGroup.m_Targets[2].radius = radiusOfTarget;
        cinemachineTargetGroup.m_Targets[2].radius = radiusOfTarget;

        for (int i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            if (i != 2 && i != 3)
            {
                cinemachineTargetGroup.m_Targets[i].weight = 0;
            }
        }
    }

    // My suggestion: min and max = 3 & 5
    public void Listenning(float weight, float minRadius, float maxRadius)
    {
        float radiusOfTarget = Random.Range(minRadius, maxRadius);
        cinemachineTargetGroup.m_Targets[3].weight = weight;
        cinemachineTargetGroup.m_Targets[3].radius = radiusOfTarget;

        for (int i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            if (i != 3)
            {
                cinemachineTargetGroup.m_Targets[i].weight = 0;
            }
        }
    }




}
