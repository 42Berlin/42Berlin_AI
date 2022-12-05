using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SimpleJSON;

using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

public class TCPClient : MonoBehaviour
{
  private TcpClient socketConnection;
  private Thread clientReceiveThread;
  public GameObject connectionProblemIcon;

  public UnityEvent<JSONNode> OnMessageReceived = new UnityEvent<JSONNode>();
  // public UnityEvent<JSONNode> OnImgReceived = new UnityEvent<JSONNode>(); // used for image generation
  public UnityEvent<JSONNode> OnVideoBackgroundReceived = new UnityEvent<JSONNode>();
  public UnityEvent<JSONNode> OnVideoOnlyReceived = new UnityEvent<JSONNode>();
  public UnityEvent<JSONNode> OnPhotoBackgroundReceived = new UnityEvent<JSONNode>();

  public UnityEvent OnConnected = new UnityEvent();

  JSONNode lastMessageSent;
  JSONNode lastMessageReceived;

  public Text clientText;
  public Text serverText;

  public InputField UIInputField;

  public JSONNode initial_weather_report;

  AIConfig config;
  void Start()
  {
    Debug.Log("[TCP CLIENT] Start");      
    config = new AIConfig();
    UIInputField.onEndEdit.AddListener(delegate
    {
      JSONNode n = new JSONObject();
      n["type"] = "message";
      n["author"] = "UnityClient";
      n["string"] = UIInputField.text;
      SendJSONMessage(n);
    });
    ConnectToTcpServer();
  }

  void printLastMessage()
  {
    Debug.Log(
      "type: " + lastMessageReceived["type"] + " " +
      "author: " + lastMessageReceived["author"] + " " +
      "string: " + lastMessageReceived["string"] + " " +
      "videoId: " + lastMessageReceived["videoId"] + " " +
      "photo1Id: " + lastMessageReceived["photo1Id"] + " " +
      "photo2Id: " + lastMessageReceived["photo2Id"] + " " +
      "payload: " + lastMessageReceived["payload"] + " "
    );
  }

  void Update()
  {
    if (lastMessageSent != null) clientText.text = lastMessageSent["author"] + ": " + lastMessageSent["string"];
    if (lastMessageReceived != null)
    {
      // if (lastMessageReceived["type"] == "image") OnImgReceived.Invoke(lastMessageReceived); else if // used for image generation
      if (lastMessageReceived["type"] == "message")
      {
        serverText.text = lastMessageReceived["author"] + ": " + lastMessageReceived["string"];
        OnMessageReceived.Invoke(lastMessageReceived);
      }
      if (lastMessageReceived["type"] == "video_only")
      {
        // serverText.text = lastMessageReceived["author"] + ": " + lastMessageReceived["string"];
        OnVideoOnlyReceived.Invoke(lastMessageReceived);
      }
      if (lastMessageReceived["type"] == "video_background")
      {
        // serverText.text = lastMessageReceived["author"] + ": " + lastMessageReceived["string"];
        OnVideoBackgroundReceived.Invoke(lastMessageReceived);
      }
      if (lastMessageReceived["type"] == "photo_background")
      {
        serverText.text = lastMessageReceived["author"] + ": " + lastMessageReceived["string"];
        OnPhotoBackgroundReceived.Invoke(lastMessageReceived);
      }
      if (lastMessageReceived["type"] == "weather_init")
			{
				// Transform initial weather report into a JSON object
				initial_weather_report = JSON.Parse(lastMessageReceived["string"]);
				// JSONArray weather = initial_weather_report.AsArray;
				Debug.Log("WEATHER_INIT : " + initial_weather_report);
				// serverText.text = lastMessageReceived["author"] + ": " + lastMessageReceived["string"];
				// OnMessageReceived.Invoke(lastMessageReceived);
			}
      lastMessageReceived = null;
    }

  }

  private void ConnectToTcpServer()
  {
    try
    {
      clientReceiveThread = new Thread(new ThreadStart(ListenForData));
      clientReceiveThread.IsBackground = true;
      clientReceiveThread.Start();
    }
    catch (Exception e)
    {
      Debug.Log("On client connect exception " + e);
    }
  }

  public static byte[] Combine(byte[] first, int firstLength, byte[] second, int secondLength)
  {
      byte[] ret = new byte[firstLength + secondLength];
      Buffer.BlockCopy(first, 0, ret, 0, firstLength);
      Buffer.BlockCopy(second, 0, ret, firstLength, secondLength);
      return ret;
  }
  
  private void ListenForData()
  {
    try
    {
      Debug.Log("Connecting to server " + config.serverIP + ":" + config.serverPort);
      socketConnection = new TcpClient(config.serverIP, config.serverPort);
      OnConnected.Invoke();
      Byte[] bytes = new Byte[1024];
      var incommingData = new byte[0];
      while (true)
      {
        // Get a stream object for reading 				
        using (NetworkStream stream = socketConnection.GetStream())
        {
          int length;
          // Read incomming stream into byte arrary. 					
          while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
          {
            // var incommingData = new byte[length];
            // Array.Copy(bytes, 0, incommingData, 0, length);
            incommingData = Combine(incommingData, incommingData.Length, bytes, length);
            // Convert byte array to string message.
            if (length != 1024)
            {
              string serverMessage = Encoding.ASCII.GetString(incommingData);
              lastMessageReceived = JSON.Parse(serverMessage);
              Debug.Log("server message received as: " + serverMessage);
              incommingData = new byte[0];
            }
          }
        }
      }
    }
    catch (SocketException socketException)
    {
      Debug.Log("Socket exception: " + socketException);
    }
  }


  public void ReceiveMessage()
  {
  }

  public void SendJSONMessage(JSONNode n)
  {
    while (socketConnection == null)
    {
      Debug.Log("tcpclient - SendJSONMessage - socketConnection == null");
      // return;
    }
    try
    {
      // Get a stream object for writing. 			
      NetworkStream stream = socketConnection.GetStream();
      if (stream.CanWrite)
      {
        // Convert string message to byte array.                 
        byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(n.ToString());
        // Write byte array to socketConnection stream.                 
        stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
        lastMessageSent = n;
        Debug.Log("Client sent a message - should be received by server");
      }
    }
    catch (SocketException socketException)
    {
      Debug.Log("Socket exception: " + socketException);
    }
  }

  public void SendClientMessage(string message)
  {
    if (socketConnection == null)
    {
      return;
    }
    try
    {
      // Get a stream object for writing. 			
      NetworkStream stream = socketConnection.GetStream();
      if (stream.CanWrite)
      {
        JSONNode n = new JSONObject();
        n["type"] = "message";
        n["author"] = "Client";
        n["string"] = message;

        // Convert string message to byte array.                 
        byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(n.ToString());
        // Write byte array to socketConnection stream.                 
        stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
        lastMessageSent = n;
        Debug.Log("Client sent a message - should be received by server");
      }
    }
    catch (SocketException socketException)
    {
      Debug.Log("Socket exception: " + socketException);
    }
  }

  void OnApplicationQuit()
    {
      if (socketConnection != null)
      {
        socketConnection.Close();
        Debug.Log("Aborting Threads.");
        clientReceiveThread.Abort();
        Debug.Log("Thread active:"+clientReceiveThread.IsAlive);
      }
    }
}