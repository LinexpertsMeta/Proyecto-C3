using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MetaverseSample{
public class VoiceUser : MonoBehaviour
{

    public string id;

	public string name;

	public Image headSetImg;

    public GameObject[] headSetSprites;

    bool isMute;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MuteUser()
    {

        if(isMute)
        {
            headSetImg.sprite = headSetSprites[1].GetComponent<SpriteRenderer>().sprite;
            NetworkManager.instance.EmitRemoveMuteUser(id);

        }
        else
        {
            headSetImg.sprite = headSetSprites[0].GetComponent<SpriteRenderer>().sprite;
            NetworkManager.instance.EmitMuteUser(id);	

        }
        isMute = !isMute;
       

    }

 

}//END_CLASS
}//END_NAMESPACE
