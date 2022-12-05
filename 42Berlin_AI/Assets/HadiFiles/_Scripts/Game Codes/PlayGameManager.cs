using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] obstacles;
    [SerializeField] float currentTime;
    [SerializeField] float instanceDelay;
    [SerializeField] int instanceDelay2;
    [SerializeField] float maxTime;
    

    // Start is called before the first frame update
    void Start()
    {
        //instanceDelay = Mathf.RoundToInt(Random.Range(0, maxTime));

    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime> instanceDelay2)
        {
            InstantiateObstacles();
        }

    }


    public void InstantiateObstacles()
    {
        Instantiate(obstacles[Random.Range(0, 3)], transform.position, Quaternion.identity);
        instanceDelay2 = Random.Range(4, 6);
        currentTime = 0;
    }

}
