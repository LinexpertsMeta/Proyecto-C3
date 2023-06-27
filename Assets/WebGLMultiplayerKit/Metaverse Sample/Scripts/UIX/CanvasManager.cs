using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using StarterAssets;


namespace MetaverseSample{
public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator

	public GameObject pLobby;

	public GameObject pGame;

	public GameObject hudUsersList;

	public GameObject hudUserOptions;

	public GameObject hudVoiceUsersList;

	public GameObject chatRoom;

	public GameObject transactionPanel;

	public GameObject purchasePanel;

	public GameObject infoPanel;

	public GameObject metamaskPanel;

	public GameObject guestPanel;

	public GameObject avatarPanel;

	public GameObject lobbyCamera;

	public TMP_InputField inputLogin;

	public TMP_InputField if_myPublicAdrr;

    public TMP_InputField if_balance;

    public Text txt_amount;

    public TMP_InputField if_publicAdrrTo;

    public TMP_InputField if_transHash;

	public TMP_InputField if_CurrentUserNameVoice;

	public Button muteAllBtn;

	public string balance;

	public string myPublicAdrr;

	public string defaultPublicAdrrTo;

	public int currentMenu;

	BannerManager tempBannerManager;

	[Header("Content Users")]
	public GameObject contentUsers;

	[Header("Content Voice Users")]
	public GameObject contentVoiceUsers;

	public GameObject userPrefab;

	public GameObject voiceUserPrefab;

	public ArrayList usersList = new ArrayList ();

	public ArrayList voiceUsersList = new ArrayList ();

	public GameObject chatBox;

	public GameObject contentChatBox;

	
	public InputField inputFieldMessage;	

	
	public GameObject myMessagePrefab; // set in inspector. stores the user Prefab message game object
	
	public GameObject networkMessagePrefab;  // set in inspector. stores the network user message game object

    [Header("Content Message :")]
	public GameObject contentMessages; // set in inspector. stores the content messages game object
	
	
    [HideInInspector]
	public int countMessages; //variable for controlling the number of messages on the screen
	
	[HideInInspector]
	public int maxDeleteMessage; //variable for controlling the number of messages on the screen
	
	ArrayList messages = new ArrayList(); // list to store all messages

	bool allMute;

	
    [Header("Default Color")]
	public Color defaultColour = new Color(1f, 0f, 0f, 0.1f);  // set in inspector.

	[Header("Green Color")]
	public Color greenColour = new Color(1f, 0f, 0f, 0.1f);  // set in inspector.

	//store all players in game
	public Dictionary<string, ChatBox> chatBoxes = new Dictionary<string, ChatBox>();

	public InputField inputFieldPrivateMessage;	

	bool openMic;

	public Image micImg;

    public GameObject[] micSprites;

	public GameObject joystick;

	public GameObject uiCanvasControllerInput;

	[DllImport("__Internal")] private static extern void  MetamaskTransferTo(string _myPublicAdrr,string _to_public_address,string _amount);

   

	

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);
			instance = this;
			OpenScreen(0);

		}
		else
		{
			Destroy(this.gameObject);
		}


	}



	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(int _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case 0:
			currentMenu = _current;
			pLobby.SetActive(true);
			pGame.SetActive(false);
	        //metamaskPanel.SetActive(true);
            guestPanel.SetActive(true);
            avatarPanel.SetActive(false);
			lobbyCamera.GetComponent<Camera> ().enabled = true;
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
			pGame.SetActive(true);
			pLobby.SetActive(false);
		    avatarPanel.SetActive(false);
			lobbyCamera.GetComponent<Camera> ().enabled = false;
			break;
			case 2:
			pLobby.SetActive(true);
			pGame.SetActive(false);
	        //metamaskPanel.SetActive(false);
            guestPanel.SetActive(false);
            avatarPanel.SetActive(true);
			break;

		}

	}

	public void SetUpCanvas(string _device)
	{

		
		if(_device.Equals("mobile"))
		{
			
			
			joystick.SetActive(true);
			uiCanvasControllerInput.GetComponent<UICanvasControllerInput>().starterAssetsInputs = NetworkManager.instance.localPlayer.GetComponent<StarterAssetsInputs>();

			
			Debug.Log("change device to mobile");
			

		}
		else
		{
			
		    joystick.SetActive(false);
			Debug.Log("change device to desktop");
			
		}
		

	}

	 /// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnMetamaskSignIn(string data)
	{
		
		/*
		 * pack[0] = my public address
		 * pack[1]= balance (MATIC)

		*/

		Debug.Log("Login successful");
		
		var pack = data.Split (Delimiter);
		
		// the local player now is logged
		NetworkManager.instance.onLoggedWithMetamask = true;

        if_myPublicAdrr.text = pack[0];

		myPublicAdrr = pack[0];

        balance = pack[1];

		if_balance.text = pack[1];

	    defaultPublicAdrrTo = "0x2953399124F0cBB46d2CbACD8A89cF0599974963";

        OpenScreen(2);
		
		
	}

	public void OpenPurchasePanel(string _description, GameObject _nft_sprite,string price,string publicAdrrTo,string _nft_link,BannerManager _bannerManager)
	{
		tempBannerManager = _bannerManager;
		purchasePanel.GetComponent<PurchasePanelManager>().nftImg.sprite = _nft_sprite.GetComponent<SpriteRenderer>().sprite;
		purchasePanel.GetComponent<PurchasePanelManager>().txt_description.text = _description;
		purchasePanel.GetComponent<PurchasePanelManager>().if_publicAdrrTo.text = publicAdrrTo;
		purchasePanel.GetComponent<PurchasePanelManager>().txt_price.text = price;
		purchasePanel.GetComponent<PurchasePanelManager>().if_balance.text= balance;
		purchasePanel.GetComponent<PurchasePanelManager>().nft_link= _nft_link;
		purchasePanel.SetActive(true);
	}

	public void ClosePurchasePanel()
	{
		tempBannerManager.isOpen = false;
		tempBannerManager.OnPlayer = false;
		purchasePanel.SetActive(false);
	}


	public void OpenInfoPanel(string _description, GameObject _info_sprite,string _info_link,BannerManager _bannerManager)
	{
		tempBannerManager = _bannerManager;
		infoPanel.GetComponent<InfoPanelManager>().infoImg.sprite = _info_sprite.GetComponent<SpriteRenderer>().sprite;
		infoPanel.GetComponent<InfoPanelManager>().txt_description.text = _description;
		infoPanel.GetComponent<InfoPanelManager>().info_link= _info_link;
		infoPanel.SetActive(true);
	}

	public void CloseInfoPanel()
	{
		tempBannerManager.isOpen = false;
		tempBannerManager.OnPlayer = false;
		infoPanel.SetActive(false);
	}

	public void TransferTo()
    {
     
        MetamaskTransferTo(if_myPublicAdrr.text,
		        hudUserOptions.GetComponent<UserOptions>().if_publicAdrrTo.text, 
				         hudUserOptions.GetComponent<UserOptions>().if_amount.text);
	

    }

	    /// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnEndTransaction(string data)
	{
		Debug.Log("Transaction successful");
		
		var pack = data.Split (Delimiter);

		/*
		 * pack[0] = txhash
		 * pack[1]= balance (MATIC)

		*/

		Debug.Log("hash: "+pack[0]);

		Debug.Log("balance: "+pack[1]);

        hudUserOptions.GetComponent<UserOptions>().if_transHash.text = pack[0];

        hudUserOptions.GetComponent<UserOptions>().if_balance.text = pack[1];

	    balance = pack[1];

		NetworkManager.instance.EmitConfirmTransaction(hudUserOptions.GetComponent<UserOptions>().id,hudUserOptions.GetComponent<UserOptions>().if_amount.text);
		
		
	}

	
	public void SetUpUser(string _id, string _name, string _publicAddress)
	{
	    	  
	  	GameObject newUser = Instantiate (userPrefab) as GameObject;

		newUser.GetComponent<User>().txtName.text = _name;
		newUser.GetComponent<User>().name= _name;
		newUser.GetComponent<User>().id = _id;
		newUser.GetComponent<User>().publicAddress = _publicAddress;
		newUser.transform.parent = contentUsers.transform;
		newUser.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		usersList.Add (newUser);


		GameObject newVoiceUser = Instantiate (voiceUserPrefab) as GameObject;

		newVoiceUser.GetComponent<VoiceUser>().name= _name;
		newVoiceUser.GetComponent<VoiceUser>().id = _id;
		newVoiceUser.transform.parent = contentVoiceUsers.transform;
		newVoiceUser.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		voiceUsersList.Add (newVoiceUser);

	}
	




	
	/// <summary>
	/// Clears the leader board.
	/// </summary>
	public void ClearUsersList()
	{
		foreach (GameObject user in usersList)
		{

			Destroy (user.gameObject);
		}

		usersList.Clear ();
	}
	
	public void OpenUsersList()
	{
		if (!hudUsersList.active)
		{
			NetworkManager.instance.EmitGetUsersList();
	        hudUsersList.SetActive(true);

		}
		else
		{
			CloseUsersList();

		}
	 
	}

	public void OpenChatRoom()
	{
		chatRoom.SetActive(true);
	}

	public void CloseChatRoom()
	{
		chatRoom.SetActive(false);
	}

	public void SpawnNetworkMessage(string _user_name, string _message)
	{
	  countMessages +=1;

	  Debug.Log("user name: "+_user_name);
	  
	  GameObject newMessage = Instantiate (networkMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	  newMessage.GetComponent<Message>().id = countMessages;
	  newMessage.GetComponent<Message>().txtUserName.text = _user_name;
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
	  newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  
	  messages.Add (newMessage);
	  
	  if (messages.Count > 7)
		{
			int j = 0;

			foreach(Message msg in messages )
			{
				if (j == 0) 
				{

					Destroy (GameObject.Find(msg.id.ToString()));
					messages.Remove (msg);

					break;
				}
				j += 1;

			}
		}  
	}
	
	public void SpawnMyMessage( string _message)
	{
	
	 
	  countMessages +=1;
	  
	  GameObject newMessage = Instantiate (myMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
      newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  	  
	  messages.Add (newMessage);
	  Debug.Log("messages.Count : "+messages.Count);
	   if (messages.Count > 7)
		{
		     ArrayList deleteMessages = new ArrayList();

			int j = 0;

			foreach(GameObject msg in messages )
			{
				if (j <= maxDeleteMessage) 
				{
                    deleteMessages.Add(msg);
				}
				j += 1;

			}
			
			foreach(GameObject msg in deleteMessages)
            {
			  Destroy (msg);
              messages.Remove(msg);
             }

		}
    
	}

	public void CloseUsersList()
	{
	  hudUsersList.SetActive(false);
	  CloseUserOptions();
	}

	public void OpenVoiceUsersList()
	{
		if (!hudVoiceUsersList.active)
		{
			NetworkManager.instance.EmitGetUsersList();
	        hudVoiceUsersList.SetActive(true);

		}
		else
		{
			CloseVoiceUsersList();

		}
	 
	}

	public void CloseVoiceUsersList()
	{
	  hudVoiceUsersList.SetActive(false);
	 
	}

	public void OpenUserOptions(string _id, string _name , string _publicAddress)
	{
		if (!hudUserOptions.active)
		{
		hudUserOptions.GetComponent<UserOptions>().id = _id;
		hudUserOptions.GetComponent<UserOptions>().userName.text = _name;
		hudUserOptions.GetComponent<UserOptions>().userPublicAddress = _publicAddress;
		hudUserOptions.SetActive(true);
		}
		else
		{
			CloseUserOptions();

		}
	}

	public void CloseUserOptions()
	{
		hudUserOptions.SetActive(false);
	}

	public void SpawnChatBox( string _id,string _host_id,string _guest_id, string _profileName)
	{
	  
	  
	  GameObject newChatBox = Instantiate (chatBox) as GameObject;

	  newChatBox.GetComponent<ChatBox>().id =  _id; 
	  newChatBox.GetComponent<ChatBox>().host_id =  _host_id; 
	  newChatBox.GetComponent<ChatBox>().guest_id=  _guest_id; 
	  newChatBox.GetComponent<ChatBox>().profileName.text = _profileName; 
      newChatBox.transform.parent = contentChatBox.transform;
	  newChatBox.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	
	  chatBoxes [_id]  = newChatBox.GetComponent<ChatBox>();

	}
	

	public void CloseChatBox(ChatBox _chat_box)
	{
		Destroy (_chat_box.gameObject);
        chatBoxes.Remove(_chat_box.id);
	}

	public void MuteAllUsers()
	{
		if(!allMute)
		{
			muteAllBtn.GetComponent<Image>().color = greenColour;
			NetworkManager.instance.EmitMuteAllUsers();

		}
		else
		{
			muteAllBtn.GetComponent<Image>().color = defaultColour;
			NetworkManager.instance.EmitRemoveAllUsersMute();

		}
		allMute =!allMute;

	}

	public void SetMic()
	{
		
		openMic = !openMic; 	//allow or block mic

		NetworkManager.instance.EmitAudioMute();

		if(openMic)
		{
			micImg.sprite = micSprites[1].GetComponent<SpriteRenderer>().sprite;
		

		}
		else
		{
			micImg.sprite = micSprites[0].GetComponent<SpriteRenderer>().sprite;
		  
		}
	}




}//END_CLASS
}//END_NAMESPACE