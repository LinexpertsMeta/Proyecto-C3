using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Cinemachine;
using TMPro;



/// <summary>
/// Network Manager class.
/// </summary>
///
namespace MetaverseSample{
public class NetworkManager : MonoBehaviour {


    public static NetworkManager instance; //useful for any gameObject to access this class without the need of instances her or you declare it

	static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator
    
	[HideInInspector]
	public bool onLogged = false; //flag which is determined the player is logged in the game arena

	  
	[HideInInspector]
	public bool onLoggedWithMetamask = false; //flag which is determined the player is logged in the game arena

	[HideInInspector]
	public GameObject localPlayer; //store localPlayer

    [HideInInspector]
	public string local_player_id;

	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();
	
	[Header("Local Player Prefab")]
	public GameObject[] playerPref; //store the local player prefabs

	[Header("Local Player Prefab")]
	public GameObject[] remotePlayerPref; //store the local player prefabs

	
	[Header("Spawn Points")]
	public Transform[] spawnPoints; //stores the spawn points


	[Header("Field Of View Variables")] 
   public float defaultFOV;

	[HideInInspector]
	public bool isGameOver; // game over flag

	[HideInInspector]
   public string _inputAxisNameX;

   [HideInInspector]
   public string _inputAxisNameY;

   public string account;

    [Header("Cinemachine Camera Brain")]
	public GameObject camBrain;

	[Header("FreeLookCamera")]
    public CinemachineFreeLook cinemachineFreeLook;


	[DllImport("__Internal")] private static extern void DetectDevice();

	[DllImport("__Internal")] private static extern void MetamaskSignIn();

	[DllImport("__Internal")] private static extern void  ConfirmTransaction(string _amount);



	
	void Awake()
	{
		Application.ExternalEval("socket.isReady = true;");

		_inputAxisNameX = cinemachineFreeLook.m_XAxis.m_InputAxisName;
        _inputAxisNameY = cinemachineFreeLook.m_YAxis.m_InputAxisName;

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


    
	// <summary>
	/// manages and switches user login.
	/// </summary>
	public void SignIn(string _method)
    {

		switch(_method)
		{
			case "metamask":
			 MetamaskSignIn();
			break;
			case "guest":
			CanvasManager.instance.OpenScreen(2);
			break;
		}  
        
    }






	//call be  OnClickJoinBtn() method from CanvasManager class
	/// <summary>
	/// Emits the player's information to the server.
	/// </summary>
	/// <param name="_login">Login.</param>
	public void EmitJoin()
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();


		//makes the draw of a point for the player to be spawn
		int index = Random.Range (0, spawnPoints.Length);

		//send the position point to server
		string msg = string.Empty;


		data["name"] = CanvasManager.instance.inputLogin.text;

		data["publicAddress"] = "none";

		if(onLoggedWithMetamask)
		{
			data["publicAddress"] = CanvasManager.instance.myPublicAdrr;
		}

		//store player's skin
		data["model"] = CharacterChoiceManager.instance.current_model.ToString();
		data["posX"] = spawnPoints[index].position.x.ToString();
		data["posY"] = spawnPoints[index].position.y.ToString();
		data["posZ"] = spawnPoints[index].position.z.ToString();

		//sends to the nodejs server through socket the json package
	   	Application.ExternalCall("socket.emit", "JOIN",new JSONObject(data));


		Debug.Log("join sended");

		//obs: take a look in server script.
	}

	/// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		

		/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2] = position.x (local player position x)
		 * pack[3] = position.y (local player position ...)
		 * pack[4] = model

		*/

		Debug.Log("Login successful, joining game");
		
		var pack = data.Split (Delimiter);
		
		// the local player now is logged
		onLogged = true;
		

		// take a look in NetworkPlayer.cs script
		PlayerManager newPlayer;

		// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
		newPlayer = GameObject.Instantiate (playerPref[int.Parse(pack[5])],
				new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
					UtilsClass.StringToFloat(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


		Debug.Log("player instantiated");

		account = pack [0];

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

		//hide the lobby menu (the input field and join buton)
		CanvasManager.instance.OpenScreen(1);
		CharacterChoiceManager.instance.HideModels();
		DetectDevice();
		Debug.Log("player in game");
		
	}

	
	public void SetCinemachineFreeLookTarget(Transform _target)
	{

		cinemachineFreeLook.LookAt = _target;
		    
		cinemachineFreeLook.Follow = _target;



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
		    newPlayer = GameObject.Instantiate (remotePlayerPref[int.Parse(pack[5])],
					new Vector3(UtilsClass.StringToFloat(pack[2]), UtilsClass.StringToFloat(pack[3]),
					UtilsClass.StringToFloat(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


            Debug.Log("player spawned");

		    newPlayer.id = pack [0];

			newPlayer.name = pack[1];

		    newPlayer.isLocalPlayer = false; //it is not the local player
				
		    newPlayer.isOnline = true; //set network player online in the arena

		    newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

		    newPlayer.gameObject.name = pack [0];
				
		    networkPlayers [pack [0]] = newPlayer; //puts the network player on the list
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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////// USERS LIST UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public void EmitGetUsersList()
{

	CanvasManager.instance.ClearUsersList();
			
    //sends to the nodejs server through socket the json package
	Application.ExternalCall("socket.emit", "GET_USERS_LIST");
		
	
}

void OnClearUsersList()
{
	CanvasManager.instance.ClearUsersList();
}
	
void OnUpdateUsersList(string data)
{
	    
	    /*
		 * pack[0] = id
		 * pack[1] = name
		 * pack[2] = public address
		*/
			
		var pack = data.Split (Delimiter);

	
		//Debug.Log("received best players from server ...");
		//Debug.Log("id: "+pack[0]);
		//Debug.Log("name: "+pack[1]);
		//Debug.Log("public address: "+pack[2]);
		
		CanvasManager.instance.SetUpUser(pack[0], pack[1],  pack[2]);

}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////MESSAGE FUNCTIONS////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

     /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitMessage()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "MESSAGE";
		
		data ["id"] = local_player_id;
		
		data ["message"] = CanvasManager.instance.inputFieldMessage.text;
		
		CanvasManager.instance.inputFieldMessage.text = string.Empty;
			
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}
	
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceiveMessage(string data)
	{
	
	/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1]= message
		*/
		
    
		  
		var pack = data.Split (Delimiter);

	
		if (local_player_id.Equals(pack[0])) {
			  
			 CanvasManager.instance.SpawnMyMessage(pack[1]);
			
		}
		else
		{
			CanvasManager.instance.SpawnNetworkMessage(networkPlayers[pack[0]].name,pack[1]);
		}
	
			 
	}



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////PRIVATE CHAT FUNCTIONS////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


 /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitOpenChatBox(string _player_id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "SEND_OPEN_CHAT_BOX";
		
		data ["player_id"] = _player_id;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}


/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceiveOpenChatBox(string data)
	{
	
	/*
		 * data.pack[0] = host id 
		 * data.pack[1]= guest id
		*/
		
    
		  
		var pack = data.Split (Delimiter);

		if(local_player_id.Equals(pack[0]))
		{
			//spawn new chatbox
		    CanvasManager.instance.SpawnChatBox( pack[0],pack[0],pack[1], networkPlayers[pack[1]].name);

		}
		else
		{
			CanvasManager.instance.SpawnChatBox( pack[0],pack[1],pack[0], networkPlayers[pack[0]].name);


		}

			 
	}

	
     /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitPrivateMessage(string _message,string _chat_box_id, string _gest_id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "PRIVATE_MESSAGE";

		data["chat_box_id"] = _chat_box_id;
		
		data ["guest_id"] = _gest_id;
		
		data ["message"] = _message;
		
		CanvasManager.instance.inputFieldPrivateMessage.text = string.Empty;
			
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}
	
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceivePrivateMessage(string data)
	{
	
	/*
		 * data.pack[0] = guest (network player id)
		 * data.pack[1]= message
		*/
		
    
		  
		var pack = data.Split (Delimiter);

		Debug.Log("pack[0]: "+pack [0]);
		Debug.Log("pack[1]: "+pack [1]);
		Debug.Log("pack[2]: "+pack [2]);

	    if( CanvasManager.instance.chatBoxes.ContainsKey(pack [0]))
		{
		  if (local_player_id.Equals(pack[1])) {
			  
			 
			CanvasManager.instance.chatBoxes[pack[0]].SpawnMyMessage(pack[2]);
			
		   }
		   else
		   {
			 CanvasManager.instance.chatBoxes[pack[0]].SpawnNetworkMessage(pack[2]);
		   }
		}
	
			 
	}

	   /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitConfirmTransaction(string _id_to,string _amount)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "CONFIRM_TRANSACTION";

		data["idTo"] = _id_to;
		
		data ["amount"] = _amount;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}

	void OnConfirmTransaction(string data)
	{
	
	    /*
		 * data.pack[0] = amount
		
		*/
		  
		var pack = data.Split (Delimiter);

		Debug.Log("amount: "+pack [0]);

		ConfirmTransaction(pack [0]);
		
			 
	}

	
	void OnUpdateUserVoiceInfo(string data)
	{
	
	    /*
		 * data.pack[0] = id
		
		*/
		  
		var pack = data.Split (Delimiter);

		//Debug.Log("name: "+networkPlayers[pack[0]].name);

		CanvasManager.instance.if_CurrentUserNameVoice.text = networkPlayers[pack[0]].name+" is talking";

		StartCoroutine("ClearCurrentVoiceText");

	
		
			 
	}

		
	IEnumerator ClearCurrentVoiceText()
	{
		
		yield return new WaitForSeconds(3f); // wait for set reload time

		CanvasManager.instance.if_CurrentUserNameVoice.text = string.Empty;

	    
	}

	/// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitMuteAllUsers()
	{

		Dictionary<string, string> data = new Dictionary<string, string>();
		
		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "MUTE_ALL_USERS";

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"]);
		
	
	}

	/// Emits the local player mute request  to Server.js.
	/// </summary>
	public void EmitAudioMute()
	{
		
		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit", "AUDIO_MUTE");


	}

	
	
	

	/// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitRemoveAllUsersMute()
	{

		Dictionary<string, string> data = new Dictionary<string, string>();
		
		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "REMOVE_MUTE_ALL_USERS";

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"]);
		
	
	}


	/// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitMuteUser(string _id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "ADD_MUTE_USER";

		data["id"] = _id;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}

	/// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitRemoveMuteUser(string _id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "REMOVE_MUTE_USER";

		data["id"] = _id;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
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


}
}