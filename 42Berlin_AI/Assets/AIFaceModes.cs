using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFaceModes : MonoBehaviour
{
  public GameObject Mouth;
  public GameObject Cloud;

  public GameObject Particles;
  public GameObject FirePlane;

  int mode = 0;

  void Awake()
  {
    mode = -1;
    NextMode();
  }


  public void NextMode()
  {
    mode++;
    if (mode == 1)
    {
      Cloud.SetActive(true);
      Mouth.SetActive(true);
      Particles.SetActive(true);
      FirePlane.SetActive(false);

    }
    else if (mode == 2)
    {
      Cloud.SetActive(false);
      Mouth.SetActive(false);
      Particles.SetActive(true);
      FirePlane.SetActive(true);

    }
    else
    {
      mode = 0;
      Cloud.SetActive(true);
      Mouth.SetActive(false);
      Particles.SetActive(true);
      FirePlane.SetActive(false);
    }

  }


}
