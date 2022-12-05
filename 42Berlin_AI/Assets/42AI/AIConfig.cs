using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIConfig
{
  public string description = "Bear AI Config file. Delete to reset to defaults.";
  public string serverIP;
  public int serverPort;
  public AIConfig()
  {
    serverIP = "localhost";
    serverPort = 50007;
  }
}

