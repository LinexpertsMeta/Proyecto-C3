using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
namespace MetaverseSample{
public class PurchasePanelManager : MonoBehaviour
{

    public Text txt_description;

    public Image nftImg;

    public TMP_InputField if_balance;

    public Text txt_price;

    public TMP_InputField if_publicAdrrTo;

    public TMP_InputField if_transHash;

    public string nft_link;

    [DllImport("__Internal")] private static extern void  OpenWindow(string link);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Purshase()
    {
     
       OpenWindow(nft_link);
	
    }
}
}
