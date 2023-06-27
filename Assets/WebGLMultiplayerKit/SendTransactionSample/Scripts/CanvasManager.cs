using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System.Numerics;

namespace SendTransaction{
public class CanvasManager : MonoBehaviour
{

    public static  CanvasManager instance;

    static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator

    public GameObject pLobby;

    public GameObject dashboard;

    public TMP_InputField if_myPublicAdrr;

    public TMP_InputField if_balance;

    public TMP_InputField if_amount;

    public TMP_InputField if_publicAdrrTo;

    public TMP_InputField if_transHash;

    public int currentMenu;

    public bool onLogged;

    public decimal amount = 0.0m;





    [DllImport("__Internal")] private static extern void   MetamaskSignIn();

	[DllImport("__Internal")] private static extern void  MetamaskTransferTo(string _myPublicAdrr,string _to_public_address,string _amount);



    // Start is called before the first frame update
    void Start()
    {
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
            dashboard.SetActive(false);
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
            dashboard.SetActive(true);
			pLobby.SetActive(false);      
			break;

		}

	}

    public void SignIn()
    {

        MetamaskSignIn();
        
    }

    /// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnMetamaskSignIn(string data)
	{
		Debug.Log("Login successful");
		
		var pack = data.Split (Delimiter);
		
	
		// the local player now is logged
		onLogged = true;

		/*
		 * pack[0] = my public address
		 * pack[1]= balance (MATIC)

		*/

        if_myPublicAdrr.text = pack[0];

        if_balance.text = pack[1];

        if_publicAdrrTo.text = "0xb22186477a77C9EFFF37FD2A199fC293e6d47cE6";


		
        OpenScreen(1);
		
		
	}

    public void TransferTo()
    {
      //  amount = System.Decimal.Parse(if_amount.text);
        MetamaskTransferTo(if_myPublicAdrr.text,if_publicAdrrTo.text, if_amount.text);
	

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

       if_transHash.text = pack[0];

       if_balance.text = pack[1];
		
		
	}


}//END_CLASS
}//END_NAMESPACE
