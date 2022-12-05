using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject[] groundObjects;
    [SerializeField] private int currentObj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GroundChanger();
    }

    private void GroundChanger()
    {
        for (int i = 0; i <= groundObjects.Length-1; i++)
        {
            if (i == currentObj)
            {
                groundObjects[i].SetActive(true);
            }
            else
            {
                groundObjects[i].SetActive(false);

            }
        }
        
    }
}
