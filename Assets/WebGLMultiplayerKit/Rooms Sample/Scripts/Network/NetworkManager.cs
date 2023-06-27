using System.Collections;
using System.Collections.Generic;
//using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;


namespace RoomsSample
{

public class NetworkManager : MonoBehaviour
{
   
	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

	//local player id
	public string myId = string.Empty;
	
	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {':'};
	
	//local player id
	public string local_player_id;
	
	//store localPlayer
	public GameObject localPlayer;
	
	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();
	
	ArrayList playersNames;
	
	//store the local players' sprites
	public GameObject localPlayerPrefab;
	

	//store the networkplayers' sprites
	public GameObject networkPlayerPrefab;
	
	//stores the spawn points 
	public Transform[] spawnPoints;
	
	[Header("Camera Rig Prefab")]
	public GameObject camRigPref;

    [HideInInspector]
	public GameObject camRig;
	
	public bool isGameOver;
	
	int index;

	public GameObject[] mapsPref;
	
	
	void Awake()
	{
		Application.ExternalEval("socket.isReady = true;");
		
	}

	
	
	// Use this for initialization
	void Start () {
	
	 // if don't exist an instance of this class
	 if (instance == null) {

		//it doesn't destroy the object, if other scene be loaded
		DontDestroyOnLoad (this.gameObject);

		instance = this;// define the class as a static variable
		
		
		playersNames = new ArrayList();
		
	
		
	 }
	 else
	 {
		//it destroys the class if already other class exists
		Destroy(this.gameObject);
	 }
		
	}
	

	/// <summary>
	///  receives an answer of the server.
	/// from  void OnReceivePing(string [] pack,IPEndPoint anyIP ) in server
	/// </summary>
	public void OnPrintPongMsg()
	{

		/*
		 * data.pack[0]= CALLBACK_NAME: "PONG"
		 * data.pack[1]= "pong!!!!"
		*/

		Debug.Log("receive pong");
		
	}

	// <summary>
	/// sends ping message to UDPServer.
	///     case "PING":
	///     OnReceivePing(pack,anyIP);
	///     break;
	/// take a look in TicTacttoeServer.cs script
	/// </summary>
	public void EmitPing() {

			//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string>();

		//store "ping!!!" message in msg field
	    data["msg"] = "ping!!!!";

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "PING",new JSONObject(data));
	
	}
	

	/// <summary>
	/// Emits the CREATE_ROOM event to Server.
	/// method called by the Create button present in Create room panel
	/// take a look in Server script
	/// </summary>
	public void EmitCreateServer()
	{
	 
	  //hash table <key, value>	
	  Dictionary<string, string> data = new Dictionary<string, string>();

	  data["map"] = CanvasManager.instance.currentMap;

	   
      data["max_players"] = CanvasManager.instance.maxPlayers.ToString();

	
      data["isPrivateRoom"] = CanvasManager.instance. isPrivateRoom.ToString();

	  //player's name	
	  data["name"] = CanvasManager.instance.inputLogin.text;

	  //makes the draw of a point for the player to be spawn
	  int index = UnityEngine.Random.Range (0, spawnPoints.Length);

	  data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;
 
			  
	 //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "CREATE_ROOM",new JSONObject(data));

	  Debug.Log("create server sended");

	
	}

	/// <summary>
	/// Raises the create room event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	void OnCreateRoom(string data)
	{
	
	    /*
		 * pack[0] = id (local player id)
		 * pack[1]= current_players 
		 * pack[2]= max_players 
		 * pack[3]= map
		
		*/
		try
		{
		  Debug.Log ("\n joining ...\n");
		

		  Debug.Log("Room created, joining room");


			
		 var pack = data.Split (Delimiter);

		
		 CanvasManager.instance.OpenScreen("lobby_room");

		
		  CanvasManager.instance.SetUpRoom( pack[0],pack[1],pack[2],int.Parse(pack[3]));
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
		

		
	

	}


	/// <summary>
	/// Raises the join game event from Server.
	/// only the first player to connect gets this feedback from the server
	/// </summary>
	/// <param name="data">Data.</param>
	void OnOpenLobbyRoom(string data)
	{
	
	   /*
		 * pack[0] = id (local player id)
		 * pack[1]= current_players 
		 * pack[2]= max_players 
		 * pack[3]= map
		
		*/
		try
		{
		Debug.Log ("\n joining ...\n");
	
		var pack = data.Split (Delimiter);

		CanvasManager.instance.OpenScreen("lobby_room");

		CanvasManager.instance.SetUpRoom( pack[0],pack[1],pack[2],int.Parse(pack[3]));

		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}

	}

	/// <summary>
	/// Raises the  event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	void OnUpdateCurrentPlayers(string data)
	{
	
	 /*
	  * pack[0] = current_players	 	
	*/
	 try
	  {
		Debug.Log ("\n updating current players ...\n");

		var pack = data.Split (Delimiter);

		CanvasManager.instance.UpdateCurrentPlayers(pack[0]);

		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}

	}


     /// <summary>
	/// method called by BtnStart game.
	/// Emits the START_GAME event to Server.
	/// </summary>
	/// <param name="data">Data.</param>
	public void EmitStartGame()
	{
	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "START_GAME");
	  
	}

    /// <summary>
	/// Raises the can start game event from Server
	/// </summary>
	void OnCanStartGame()
	{
		try{
	 	 
		  CanvasManager.instance.btnStartGame.SetActive(true);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	

	/// <summary>
	/// Emits the join game to Server.
	/// case "JOIN_GAME":
	///   OnReceiveJoinGame(pack,anyIP);
	///  break;
	/// take a look in Server.cs script
	/// </summary>
	public void EmitJoinGame(string roomID)
	{
	

		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();


		//player's name
		data["name"] = CanvasManager.instance.inputLogin.text;

		data["roomID"] = roomID;
		
		//makes the draw of a point for the player to be spawn
		int index = UnityEngine.Random.Range (0, spawnPoints.Length);

		//send the position point to server
		string msg = string.Empty;

		data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;

		
		
		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit", "JOIN_ROOM",new JSONObject(data));

	}

	public void SpawnMap(int _map)
	{

		GameObject.Instantiate (mapsPref[_map],
				new Vector3(0,0,0),Quaternion.identity);

	}


	/// <summary>
	/// Raises the start game event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnStartGame(string data)
	{
	
	   
		/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2] = position

		*/

		var pack = data.Split (Delimiter);
		

		Debug.Log("Login successful, joining game");

		Debug.Log("pack[1]: "+pack[1]);
		Debug.Log("pack[2]: "+pack[2]);
		Debug.Log("pack[3]: "+pack[3]);
		Debug.Log("pack[4]: "+pack[4]);
		Debug.Log("pack[5]: "+pack[5]);
	
	
		// the local player now is logged
		onLogged = true;

		SpawnMap(int.Parse(pack[5]));

		Debug.Log("try to spawn player");

		

		// take a look in NetworkPlayer.cs script
		PlayerManager newPlayer;

		// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
		newPlayer = GameObject.Instantiate (localPlayerPrefab,
				new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
					UtilsClass.StringToFloat(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


		Debug.Log("player instantiated");

		newPlayer.id = pack [0];
		//this is local player
		newPlayer.isLocalPlayer = true;

		//now local player online in the arena
		newPlayer.isOnline = true;

		//set local player's 3D text with his name
		newPlayer.Set3DName(pack[1]);

		//puts the local player on the list
		networkPlayers [pack [0]] = newPlayer;

		localPlayer = networkPlayers [pack[0]].gameObject;

		local_player_id =  pack [0];

		Debug.Log("camera instantiated");

		//spawn camRigPref from Standard Assets\Cameras\Prefabs\MultipurposeCameraRig.prefab
		camRig = GameObject.Instantiate (camRigPref, new Vector3 (0f, 0f, 0f), Quaternion.identity);

			

		//set local player how  being MultipurposeCameraRig target to follow him
		camRig.GetComponent<CameraFollow> ().SetTarget (localPlayer.transform, newPlayer.cameraToTarget);

		//hide the lobby menu (the input field and join buton)
		CanvasManager.instance.OpenScreen("game");
		Debug.Log("player in game");
	
	}
	
	/// <summary>
	/// Raises the spawn network players event from server.
	/// </summary>
	void OnSpawnPlayer(string data)
	{
	
	  /*
		 * pack[0] = id (network player id)
		 * pack[1]= name
		 * pack[3] = position.x
		 * pack[4] = position.y
		 * pack[5] = position.z
		*/

        var pack = data.Split (Delimiter);

		bool alreadyExist = false;

		//verify all players to avoid duplicates 
		if(networkPlayers.ContainsKey(pack [0]))
		{
			alreadyExist = true;
		}
		if (!alreadyExist)
		{
			Debug.Log("received spawn network player");

		  

	
		    PlayerManager newPlayer;

		    // newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
		    newPlayer = GameObject.Instantiate (networkPlayerPrefab,
					new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
						UtilsClass.StringToFloat(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


            Debug.Log("player spawned");

		    newPlayer.id = pack [0];

				
		    newPlayer.isLocalPlayer = false; //it is not the local player
				
		    newPlayer.isOnline = true; //set network player online in the arena

				
		     newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

		     newPlayer.gameObject.name = pack [0];
				
		     networkPlayers [pack [0]] = newPlayer; //puts the network player on the list
		}

	}

	
	

	/// <summary>
	/// Gets the rooms from server
	/// </summary>
	public void GetRooms(string _map)
	{

		CanvasManager.instance.OpenScreen("roomList");

		CanvasManager.instance.ClearRooms();

		//hash table <key, value>	
	    Dictionary<string, string> data = new Dictionary<string, string>();

	    data["map"] = _map;

	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit","GET_ROOMS",new JSONObject(data));

	}

	

	public void OnReceiveRooms(string data)
	{
	
	    /*
		 * pack[0] = id (room id)
		 * pack[1]= name
		 * pack[2]= current_players 
		 * pack[3]= max_players 
		
		*/
	  try{
	   Debug.Log("roons received");
	   
	 var pack = data.Split (Delimiter);

	  Debug.Log("pack[0]: "+pack[0]);
	  Debug.Log("pack[1]: "+pack[1]);
	  Debug.Log("pack[2]: "+pack[2]);
	  Debug.Log("pack[3]: "+pack[3]);


	   CanvasManager.instance.SpawnRoom(pack[0],pack[1],pack[2],pack[3]);
    }
	catch(Exception e)
	{
		Debug.Log(e.ToString());
		
	}


  }






////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////PLAYER POSITION AND ROTATION UPDATES///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// send player's position and rotation to the server
    /// </summary>
    /// <param name="data"> package with player's position and rotation</param>
	public void EmitMoveAndRotate( Dictionary<string, string> data)
	{

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "POS_AND_ROT",new JSONObject(data));
	
	}



	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePosAndRot(string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = position.x
		 * data.pack[2] = position.y
		 * data.pack[3] = position.z
		 * data.pack[4] = "rotation"
		*/

		Debug.Log("received pos and rot");
		
		var pack = data.Split (Delimiter);
		
		if (networkPlayers.ContainsKey(pack [0]))
		{
		    
			PlayerManager netPlayer = networkPlayers[pack[0]];
			
			//update with the new position
			netPlayer.UpdatePosition(new Vector3(
				UtilsClass.StringToFloat(pack[1]), UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3])));
				
			//update new player rotation
			netPlayer.UpdateRotation(new Quaternion (netPlayer.transform.rotation.x,UtilsClass.StringToFloat(pack[4]),
			netPlayer.transform.rotation.z,netPlayer.transform.rotation.w));
		
		}
		

	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
		
}
}//EN_NAMESPACE