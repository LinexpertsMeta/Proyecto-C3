using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
namespace MetaverseSample{
public class InfoPanelManager : MonoBehaviour
{
     public Text txt_description;

    public Image infoImg;

    public string info_link;

    [DllImport("__Internal")] private static extern void  OpenWindow(string link);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInfoLink()
    {
     
       OpenWindow(info_link);
	
    }
}
}
