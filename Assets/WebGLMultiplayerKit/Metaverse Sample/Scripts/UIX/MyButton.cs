using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace MetaverseSample{
public class MyButton : MonoBehaviour {
 

 public Image micImg;

public GameObject[] micSprites;
public bool buttonPressed;

bool sendedPress;
bool sendedRelease;
 
 



	public delegate void OnActionPress( GameObject unit, bool state );
	public event OnActionPress onPress;
	EventTrigger eventTrigger;



	void Start () {

		eventTrigger = this.gameObject.GetComponent<EventTrigger>();
		AddEventTrgger( OnPointDown, EventTriggerType.PointerDown);
		AddEventTrgger(OnPointUp, EventTriggerType.PointerUp);

	}


	void AddEventTrgger( UnityAction action, EventTriggerType triggerType ){

		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener( (eventData) => action());

		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		eventTrigger.triggers.Add(entry);

	}


	void OnPointDown(){

	 
        buttonPressed = true;
        sendedRelease = false;
       NetworkManager.instance.EmitAudioMute();
		if( onPress != null  ){

			onPress(this.gameObject, true);

		}else{
			Debug.Log("Event null");
		}

	}

	void OnPointUp(){

		
        buttonPressed = false;
        sendedPress = false;

         NetworkManager.instance.EmitAudioMute();

		if( onPress != null  ){
			Debug.Log("OnPointUp");
            
			onPress(this.gameObject, false);
			
		}
	}

    
void Update()
{
    if(buttonPressed)
    {
        if(!sendedPress)
        {
            sendedPress = true;
            micImg.sprite = micSprites[1].GetComponent<SpriteRenderer>().sprite;
            Debug.Log("button press");
        }
        
    }

    else
    {
        if(!sendedRelease)
        {
            sendedRelease = true;
            micImg.sprite = micSprites[0].GetComponent<SpriteRenderer>().sprite;
            Debug.Log("button released");
        }
      
    }
}



}
}
