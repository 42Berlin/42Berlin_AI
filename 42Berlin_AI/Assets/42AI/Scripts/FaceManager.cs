using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FaceManager : MonoBehaviour
{
  // Start is called before the first frame update
  bool is_human = false;
  public Text NumFaces;
  int NumFramesNoFaces = 0;
  int NumFramesWithFaces = 0;
  FaceTrackerExample.FaceTrackerARExample facetracker;
  public event UnityAction<List<byte[]> > OnNewHumanDetected;
  public event UnityAction OnLostHumans;
  public event UnityAction<float, float> BearMovement;


  void Start()
  {
    Debug.Log("[FACE MANAGER] Start");      
    facetracker = GetComponent<FaceTrackerExample.FaceTrackerARExample>();
  }

  // Update is called once per frame
  void Update()
  {
    if (facetracker.FaceRects.Count == 0)
    {
      NumFramesWithFaces = 0;
      NumFramesNoFaces++;

      if (NumFramesNoFaces > 20) NumFaces.text = "No human, still believing there is one.";
      // Debug.Log("NumFramesNoFaces : " + NumFramesNoFaces);
      if (NumFramesNoFaces > 400)
      {
        NumFaces.text = "I don't see any humans anymore ?";
        if (is_human == true) 
        {
          is_human = false;
          OnLostHumans.Invoke();
        }
      }
      if (NumFramesNoFaces > 2000) NumFaces.text = "Guess all Humans are gone now. !";
    }
    else
    {
      NumFramesNoFaces = 0;
      NumFramesWithFaces++;
      // Debug.Log("Reset NumFramesNoFaces");

      if (NumFramesWithFaces > 20 && is_human == false) // if nobody was here ans then we detect a face during X frames => new user
      {
        is_human = true;
        List<byte[]> faces_bytes = facetracker.FacesToBytes();
        if (OnNewHumanDetected != null) // TESS ADDITION
          OnNewHumanDetected.Invoke(faces_bytes);
      }
      NumFaces.text = "faces" + facetracker.FaceRects.Count + " " + facetracker.FaceRects[0].height + " " + ((facetracker.FaceRects[0].height > 100) ? "YES" : "NO") + "numframesnoface" + NumFramesNoFaces;
      float xPosition = facetracker.imageWidth / 2 - (facetracker.FaceRects[0].x + facetracker.FaceRects[0].width / 2);
      float yPosition = facetracker.imageHeight / 2 - (facetracker.FaceRects[0].y + facetracker.FaceRects[0].height / 2);
      if (BearMovement != null) // TESS ADDITION
        BearMovement.Invoke(xPosition, yPosition);

    }
  }

  public bool getIsHuman()
  {
    return is_human;
  }
}
