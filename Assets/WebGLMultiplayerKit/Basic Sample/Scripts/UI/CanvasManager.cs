using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	public GameObject gameCanvas;

	public GameObject pLobby;

	public GameObject alertGameOverDialog;

	public InputField inputLogin;

	public TextMesh txt_name;

	public Text alertGameOverText;

	public Text alertDialogText;

	public GameObject lobbyCamera;

	public int currentMenu;

	public GameObject joystick;

	public GameObject uiCanvasControllerInput;

	public bool isMobile;
	

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);
			instance = this;
			alertGameOverDialog.SetActive(false);
			alertDialogText.enabled = false;
			OpenScreen(0);

		}
		else
		{
			Destroy(this.gameObject);
		}

		CloseAlertDialog ();

	}

	void Update()
	{
		txt_name.text = inputLogin.text;
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
			gameCanvas.SetActive(false);
			lobbyCamera.GetComponent<Camera> ().enabled = true;
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
			pLobby.SetActive(false);
			gameCanvas.SetActive(true);
			lobbyCamera.GetComponent<Camera> ().enabled = false;
			break;

		}

	}

	public void SetUpCanvas(string _device)
	{

		
		if(_device.Equals("mobile"))
		{
			isMobile = true;

			
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


	public void ShowGameOverDialog()
	{
		
		alertGameOverText.text = "YOU LOSE!";
		alertGameOverDialog.SetActive(true);
	}

	public void CloseGameOverDialog()
	{
		alertGameOverDialog.SetActive(false);
       
		OpenScreen (0);
	}

	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowAlertDialog(string _message)
	{
		alertDialogText.text = _message;
		alertDialogText.enabled = true;
		StartCoroutine (CloseAlertDialog() );
	}

	/// <summary>
	/// Closes the alert dialog.
	/// </summary>

	IEnumerator CloseAlertDialog() 
	{

		yield return new WaitForSeconds(4);
		alertDialogText.text = "";
		alertDialogText.enabled = false;
	} 

	
}
