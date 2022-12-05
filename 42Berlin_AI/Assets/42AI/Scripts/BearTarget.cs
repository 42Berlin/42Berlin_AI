using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTarget : MonoBehaviour
{
    Quaternion originalRotationValue;

    void Start() // Start is called before the first frame update
    {
        Debug.Log("[BEAR TARGET] Start");      
        originalRotationValue = transform.rotation; // save the initial rotation
    }

    public void rotateAt(float xPosition, float yPosition)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, xPosition * 0.08f, 0f), 0.3f);
    }

    public void resetPosition()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, 1); 
    }
}
