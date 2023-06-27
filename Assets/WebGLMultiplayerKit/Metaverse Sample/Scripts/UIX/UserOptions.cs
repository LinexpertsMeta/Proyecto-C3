using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace MetaverseSample{
public class UserOptions : MonoBehaviour
{

    public Text userName;

    public string id;

    public string userPublicAddress;

    public TMP_InputField if_myPublicAdrr;

    public TMP_InputField if_balance;

	public TMP_InputField if_amount;
    
    public TMP_InputField if_publicAdrrTo;

    public TMP_InputField if_transHash;

    public void OpenPrivateChat()
    {

        NetworkManager.instance.EmitOpenChatBox(id);

    }

    public void OpenTransactionPanel()
    {


        CanvasManager.instance.transactionPanel.SetActive(true);

        if_myPublicAdrr.text = CanvasManager.instance.myPublicAdrr;
        if_publicAdrrTo.text = userPublicAddress;
        if_balance.text =  CanvasManager.instance.balance;
         




    }

    public void CloseTransactionPanel()
    {

        CanvasManager.instance.transactionPanel.SetActive(false);

    }
}
}
