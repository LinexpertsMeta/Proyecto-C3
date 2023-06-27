using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Cinemachine;

/// <summary>
/// Network Manager class.
/// </summary>
///

public class NetworkManager : MonoBehaviour {


    public static NetworkManager instance; //useful for any gameObject to access this class without the need of instances her or you declare it

	static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator
    
	[HideInInspector]
	public bool onLogged = false; //flag which is determined the player is logged in the game arena

	[HideInInspector]
	public GameObject localPlayer; //store localPlayer

    [HideInInspector]
	public string local_player_id;

	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();
	
	[Header("Local Player Prefab")]
	public GameObject localPlayerPrefab; //store the local player prefabs

	[Header("Network Player Prefab")]
	public GameObject networkPlayerPrefab; //store the remote player prefabs


	public GameObject playerArmature;

	[Header("Spawn Points")]
	public Transform[] spawnPoints; //stores the spawn points


    [Header("Cinemachine Camera Brain")]
	public GameObject camBrain;

	[Header("FreeLookCamera")]
    public CinemachineFreeLook cinemachineFreeLook;

	[HideInInspector]
	public bool isGameOver; // game over flag

	[DllImport("__Internal")] private static extern void DetectDevice();


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
			
			Debug.Log("start mmo game");



		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

	}

	public void OnDetectDevice(string _device)
	{
		
		/*
		 * _data =  desktop or mobile
		 * 

		*/

		Debug.Log("data: "+_device);

	
		CanvasManager.instance.SetUpCanvas(_device);
	
	}



	/// <summary>
	/// Prints the pong message which arrived from server.
	/// </summary>
	/// <param name="_msg">Message.</param>
	public void OnPrintPongMsg(string data)
	{

		/*
		 * data.pack[0]= msg
		*/

		var pack = data.Split (Delimiter);
		Debug.Log ("received message: "+pack[0] +" from server by callbackID: PONG");
		CanvasManager.instance.ShowAlertDialog ("received message: "+pack[0] +" from server by callbackID: PONG");
	}

	// <summary>
	/// sends ping message to server.
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





	//call be  OnClickJoinBtn() method from CanvasManager class
	/// <summary>
	/// Emits the player's name to server.
	/// </summary>
	/// <param name="_login">Login.</param>
	public void EmitJoin()
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();


		//player's name
		data["name"] = CanvasManager.instance.inputLogin.text;
		
		//makes the draw of a point for the player to be spawn
		int index = Random.Range (0, spawnPoints.Length);

		//send the position point to server
		string msg = string.Empty;

	

		data["name"] = CanvasManager.instance.inputLogin.text;
			
		if(CanvasManager.instance.inputLogin.text.Equals(string.Empty))
		{
			int rand = Random.Range (0, 999);
			data["name"] = "guess"+rand;
		}

		data["posX"] = spawnPoints[index].position.x.ToString();
		data["posY"] = spawnPoints[index].position.y.ToString();
		data["posZ"] = spawnPoints[index].position.z.ToString();
			
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "LOGIN",new JSONObject(data));



		//obs: take a look in server script.
	}

	/// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		Debug.Log("Login successful, joining game");
		var pack = data.Split (Delimiter);
		
	
		// the local player now is logged
		onLogged = true;

		/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2] = avatar
		 * pack[3] = position.x (local player position x)
		 * pack[4] = position.y (local player position ...)

		*/


		if (!localPlayer) {

			// take a look in NetworkPlayer.cs script
			PlayerManager newPlayer;

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayerPrefab,
				new Vector3(float.Parse(pack[2]), float.Parse(pack[3]),
					float.Parse(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


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

			camBrain.GetComponent<Camera>().enabled = true;

		    SetCinemachineFreeLookTarget(newPlayer.transform);

			playerArmature.SetActive(false);

			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen(1);
			DetectDevice();
			Debug.Log("player in game");
		}
	}

	/// <summary>
	/// Raises the spawn player event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * pack[0] = id (network player id)
		 * pack[1]= name
		 * pack[2]= avatar
		 * pack[3] = position.x
		 * pack[4] = position.y
		 * pack[5] = position.z
		*/

		Debug.Log("received spawn network player");

		var pack = data.Split (Delimiter);

		if (onLogged ) {

			bool alreadyExist = false;

			//verify all players to avoid duplicates 
			if(networkPlayers.ContainsKey(pack [0]))
			{
			  alreadyExist = true;
			}
			if (!alreadyExist) {


				PlayerManager newPlayer;

				// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
			    newPlayer = GameObject.Instantiate (networkPlayerPrefab,
					new Vector3(float.Parse(pack[2]), float.Parse(pack[3]),
						float.Parse(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


                Debug.Log("player spawned");

				newPlayer.id = pack [0];

				
				newPlayer.isLocalPlayer = false; //it is not the local player
				
				newPlayer.isOnline = true; //set network player online in the arena

				
				newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

				newPlayer.gameObject.name = pack [0];
				
				networkPlayers [pack [0]] = newPlayer; //puts the network player on the list

				//int avatar_index = int.Parse(pack[2]);

			   //setup network player skin
			  //  CharacterChooseManager.instance.SetUpNetworkCharacter(newPlayer,avatar_index);
				
				Debug.Log("player configured");

			}

		}

	}


    /// <summary>
    /// send player's position and rotation to the server
    /// </summary>
    /// <param name="data"> package with player's position and rotation</param>
	public void EmitMoveAndRotate( Dictionary<string, string> data)
	{

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "MOVE_AND_ROTATE",new JSONObject(data));


	}



	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateMoveAndRotate(string data)
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
			Vector4 _rot = 	UtilsClass.StringToVector4(pack[4]);
			//update new player rotation
			netPlayer.UpdateRotation(new Quaternion (_rot.x,_rot.y,_rot.z,_rot.w));
		
		}
		

	}

	

	/// <summary>
	/// Emits the local player animation to Server.js.
	/// </summary>
	/// <param name="_animation">animation's name.</param>
	public void EmitAnimation(int _key, string _value,string _type)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

		data ["key"] = _key.ToString();

		data ["value"] = _value;

		data ["type"] = _type;

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit", "ANIMATION",new JSONObject(data));


	}


	/// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="data">package received from server with player id and  animation's name</param>
	void OnUpdateAnim(string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = key
		 * data.pack[2] = value
		*/

		var pack = data.Split (Delimiter);

	//	Debug.Log("pack[1]: "+pack[1] );
	//	Debug.Log("pack[2]: "+pack[2] );

		if(onLogged)
		{
		   //find network player by your id
		   PlayerManager netPlayer = networkPlayers[pack[0]];
		   //updates current animation

		   switch(pack[3])
		   {
			case "float":
			 netPlayer._animator.SetFloat(int.Parse(pack[1]), UtilsClass.StringToFloat(pack[2]));
			break;
			case "bool":
			 netPlayer._animator.SetBool(int.Parse(pack[1]), bool.Parse(pack[2]));
			break;
		   }
		  
		//   Debug.Log("animation updated");

		}
				

	}


	/// <summary>
	/// inform the local player to destroy offline network player
	/// </summary>
	/// <param name="_msg">Message.</param>
	//desconnect network player
	void OnUserDisconnected(string data )
	{

		/*
		 * data.pack[0] = id (network player id)
		*/

		var pack = data.Split (Delimiter);

		if (networkPlayers.ContainsKey(pack [0]))
		{


			//destroy network player by your id
			Destroy( networkPlayers[pack[0]].gameObject);


			//remove from the dictionary
			networkPlayers.Remove(pack[0]);

		}

	}


	public void SetCinemachineFreeLookTarget(Transform _target)
	{

		cinemachineFreeLook.LookAt = _target;
		    
		cinemachineFreeLook.Follow = _target;



	}


}
