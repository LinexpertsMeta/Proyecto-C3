using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RoomsSample
{
public class CanvasManager : MonoBehaviour
{
    
	public static CanvasManager  instance;

	public GameObject  navBar;

	public GameObject  mainPanel;

	public GameObject  lobbyRoom;

	public GameObject  createRoomPanel;

	public GameObject  roomListPanel;
	
	public GameObject btnStartGame;
	
	public GameObject alertgameDialog;

	public Text alertDialogText;

	public Text messageText;
	
	public Text txtLog;
	
	public string currentMenu;

	public  AudioClip buttonSound;
	
	public InputField inputLogin;

	public InputField inputPrivateRoomID;

	[Header("Input Max Players")]
	public InputField inputMaxPlayers;
	
	public GameObject[] spriteFacesPref;
	
	public Image localPlayerImg;
	
	public Text txtLocalPlayerName;
	
	public Text txtLocalPlayerHealth;

	public Text txtRoomID;

	public Button roomType;

	public Text txtCurrentRoomPlayers;

	public Text txtMaxRoomPlayers;

	public bool CanStartGame;

	public GameObject contentRooms;

	public GameObject roomPrefab;

	public GameObject btnPrivateRoom;

	public GameObject activeBtnSprite;

	public GameObject desabledBtnSprite;

	public string currentMap;

	public int maxPlayers;

	public bool isPrivateRoom;
	
	public bool enabledMobileBtns;

	
	
	
	
	public float delay = 0f;

	ArrayList rooms;


	
	[Header("Map Slider Buttons")]
	[SerializeField] private GameObject nextButton, prevButton,nextButton2, prevButton2;
	
	[Header("Image Animator")]
	public Animator mapAnim,mapAnim2;
	
	[Header("Max Maps")]
	public int maxMaps = 3;
	
	public int chooseMap,chooseMap2 = 0;

    [Header("Avaliable Maps")]
	public GameObject[] avaliableMaps;

	[Header("Current Location Text")]
	public Text txtCurrentLocation,txtCurrentLocation2;


	public GameObject lobbyCamera;
	
	
	

	
	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;

			rooms = new ArrayList ();
			
			OpenScreen("main_menu");

			CloseAlertDialog ();

			chooseMap = -1;
			NextMap();
		
	
			
		}
		else
		{
			Destroy(this.gameObject);
		}



	}

	void Update()
	{
		delay += Time.deltaTime;

		if (Input.GetKey ("escape") && delay > 1f) {

		  switch (currentMenu) {

			case "main_menu":
			 delay = 0f;
			 Application.Quit ();
			break;

		

		 }//END_SWITCH

	 }//END_IF
}
	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(string _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case "main_menu":
			currentMenu = _current;
			mainPanel.SetActive(true);
			lobbyRoom.SetActive(false);
			createRoomPanel.SetActive(false);
			roomListPanel.SetActive(false);
	
			break;

			case "lobby_room":
			currentMenu = _current;
			navBar.SetActive(false);
			mainPanel.SetActive(false);
			lobbyRoom.SetActive(true);
			createRoomPanel.SetActive(false);
			roomListPanel.SetActive(false);
	
			break;


		    case "roomList":
			currentMenu = _current;
			mainPanel.SetActive(false);
			lobbyRoom.SetActive(false);
			createRoomPanel.SetActive(false);
			roomListPanel.SetActive(true);
		
		
			break;
			 case "create_room":
			currentMenu = _current;
			mainPanel.SetActive(false);
			lobbyRoom.SetActive(false);
			createRoomPanel.SetActive(true);
			roomListPanel.SetActive(false);
	
			break;
			case "game":
			currentMenu = _current;
			navBar.SetActive(false);
			mainPanel.SetActive(false);
			lobbyRoom.SetActive(false);
			createRoomPanel.SetActive(false);
			roomListPanel.SetActive(false);
			lobbyCamera.GetComponent<Camera> ().enabled = false;
		
		
			break;

	
		}

	}


	public void SetMaxPlayers()
	{
	   maxPlayers = int.Parse(inputMaxPlayers.text);
	

	}

	public void SetPrivateRoom()
	{
		isPrivateRoom =!isPrivateRoom;

		if(isPrivateRoom)
		{
          btnPrivateRoom.GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
		  btnPrivateRoom.GetComponent<Image> ().color = activeBtnSprite.GetComponent<SpriteRenderer> ().color;
		
		}
		else
		{
          btnPrivateRoom.GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
		  btnPrivateRoom.GetComponent<Image> ().color = desabledBtnSprite.GetComponent<SpriteRenderer> ().color;
		}

		

	}



	/// <summary>
	/// method called automatically after the player has joined the room.
	/// </summary>
	public void SetMap(string _map)
	{
		Debug.Log("current map: "+_map);
		currentMap = _map;
		mapAnim.SetTrigger (currentMap);
		CheckButtonStatus();
	}

	public void ChooseAvaliableMaps(string _map)
	{
		currentMap = _map;
		mapAnim2.SetTrigger (currentMap);
		NetworkManager.instance.GetRooms(_map);
		
		
	}


	/// <summary>
	/// method for controlling the avatars choice buttons
	/// </summary>
	private void CheckButtonStatus()
	{
	
		if (nextButton == null || prevButton == null)
			return;
		
		if (chooseMap == 0) 
		{
			prevButton.SetActive(false);
			nextButton.SetActive(true);
		} else if (chooseMap >= maxMaps-1) 
		{
			prevButton.SetActive(true);
			nextButton.SetActive(false);
		} else 
		{
			prevButton.SetActive(true);
			nextButton.SetActive(false);
		}
		
	}
	
	
	
	/// <summary>
	/// method called by the BtnNext button that selects the next avatar
	/// </summary>
	public void NextMap()
	{
	  if(chooseMap+1< maxMaps)
	  {	   
		chooseMap++;

		switch (chooseMap) { 

		  case 0:
		
			SetMap(chooseMap.ToString());
			txtCurrentLocation.text = "MAP1";
		  break;
		  case 1:
			SetMap(chooseMap.ToString());
			txtCurrentLocation.text = "MAP2";
		  break;
		 
		}
		
		if(chooseMap>=maxMaps)
		{
			chooseMap = maxMaps - 1;
		}

		CheckButtonStatus();
	
	  }
	}
	
	/// <summary>
	/// method called by the BtnPrev button that selects the previous avatar
	/// </summary>
	public void PrevMap()
	{
	
	
	  if(chooseMap-1 >= 0)
	   {
				    
		  chooseMap--;

         
		 switch (chooseMap) { 

		  case 0:
		  	SetMap(chooseMap.ToString());
			txtCurrentLocation.text = "MAP1";
		  break;
		  case 1:
			SetMap(chooseMap.ToString());
			txtCurrentLocation.text = "MAP2";
		  break;
		
		}
		 		
		   if(chooseMap<0)
		   {
			  chooseMap =0;
		   }

		   CheckButtonStatus();
		  
		}
	}
	
	/// <summary>
	/// method for controlling the avatars choice buttons
	/// </summary>
	private void CheckButtonStatus2()
	{
	
		if (nextButton2 == null || prevButton2 == null)
			return;
		
		if (chooseMap2 == 0) 
		{
			prevButton2.SetActive(false);
			nextButton2.SetActive(true);
		} else if (chooseMap2 >= maxMaps-1) 
		{
			prevButton2.SetActive(true);
			nextButton2.SetActive(false);
		} else 
		{
			prevButton2.SetActive(true);
			nextButton2.SetActive(true);
		}
		
	}
	
	/// <summary>
	/// method called by the BtnNext button that selects the next avatar
	/// </summary>
	public void NextMap2()
	{
	  if(chooseMap2+1< maxMaps)
	  {	   
		chooseMap2++;

		switch (chooseMap2) { 

		  case 0:
		
			ChooseAvaliableMaps("0");
			txtCurrentLocation2.text = "MAP 1";
		  break;
		  case 1:
			ChooseAvaliableMaps("1");
			txtCurrentLocation2.text = "MAP 2";
		  break;
		  
		 
		}
		
		if(chooseMap2>=maxMaps)
		{
			chooseMap2 = maxMaps - 1;
		}

		CheckButtonStatus2();
	
	  }
	}
	
	/// <summary>
	/// method called by the BtnPrev button that selects the previous avatar
	/// </summary>
	public void PrevMap2()
	{
	
	
	  if(chooseMap2-1 >= 0)
	   {
				    
		  chooseMap2--;

         
		 switch (chooseMap2) { 

		  case 0:
		  
			ChooseAvaliableMaps("0");
			txtCurrentLocation2.text = "MAP 1";
		  break;
		  case 1:
		  
			ChooseAvaliableMaps("1");
			txtCurrentLocation2.text = "MAP 2";
		  break;
		 
		}
		 		
		   if(chooseMap2<0)
		   {
			  chooseMap2 =0;
		   }

		   CheckButtonStatus2();
		  
		}
	}

	


	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowAlertDialog(string _message)
	{
		alertDialogText.text = _message;
		alertgameDialog.SetActive(true);
	}

	
	/// <summary>
	/// Closes the alert dialog.
	/// </summary>
	public void CloseAlertDialog()
	{
		alertgameDialog.SetActive(false);
	}
	
		/// <summary>
	/// Shows the alert dialog.Debug.Log
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowMessage(string _message)
	{
		messageText.text = _message;
		messageText.enabled = true;
		StartCoroutine (CloseMessage() );//chama corrotina para esperar o player colocar o outro pé no chão
	}
	
	/// <summary>
	/// Closes the alert dialog.
	/// </summary>

	IEnumerator CloseMessage() 
	{

		yield return new WaitForSeconds(4);
		messageText.text = "";
		messageText.enabled = false;
	} 




	public void PlayAudio(AudioClip _audioclip)
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);

	}

	public void SetUpRoom(string room_id, string current_players,string max_players,int map)
	{
	
	  txtRoomID.text = room_id;
      
	  txtCurrentRoomPlayers.text = current_players;

	  txtMaxRoomPlayers.text = max_players;

	}


	public void UpdateCurrentPlayers( string current_players)
	{
	
	  txtCurrentRoomPlayers.text = current_players;


	}

	/// <summary>
	/// Clears rooms.
	/// </summary>
	public void ClearRooms()
	{
		foreach (GameObject room in rooms)
		{

			Destroy (room.gameObject);
		}

		rooms.Clear ();
	}

	public void SpawnRoom(string id,string name, string current_players, string max_players)
	{
		
       
		GameObject newRoom = Instantiate (roomPrefab) as GameObject;

		newRoom.GetComponent<Room>().id = id;
		newRoom.GetComponent<Room>().txtRoomName.text = name;
		newRoom.GetComponent<Room>().txtPlayers.text = current_players+" / "+max_players;
	
		newRoom.transform.parent = contentRooms.transform;
		newRoom.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);

		rooms.Add (newRoom);
				

	}

	public void JoinToPrivateRoom()
	{
		 NetworkManager.instance.EmitJoinGame(inputPrivateRoomID.text);
	}
	
	
	
	
}

}
