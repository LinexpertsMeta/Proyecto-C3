using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MetaverseSample{
public class CharacterChoiceManager : MonoBehaviour
{
  
	public static  CharacterChoiceManager  instance;

	 [Header("Character Models")]
	public GameObject[] models;
	
	public int maxCharacters = 2;

	public Button[] selected_model_images;
	
	public int current_model = 0;


	// Use this for initialization
	void Start () {
	
		// if don't exist an instance of this class
		if (instance == null) {


			// define the class as a static variable
			instance = this;
			
			current_model = 0;
			
			HideModels();
           
            ChooseCharacter(current_model);


		}
	}
	



	//method called by the BtnNext button that selects the next avatar
	public void ChooseCharacter(int _current_model)
	{

		int last_character_index = current_model;
		current_model = _current_model;
		selected_model_images[_current_model].interactable = false;
		selected_model_images[last_character_index].interactable = true;
		SetModel(current_model);

		
		
	}

	void SetModel(int _index)
	{
	  for(int i =0;i<models.Length;i++)
	  {
	    if(i.Equals(_index))
		{
		 
		  models[_index].SetActive(true); 
		}
		else
		{
			
		 models[i].SetActive(false);  //hides the othersmodels
		}
	  }
	}

	public void HideModels()
	{
	  for(int i =0;i<models.Length;i++)
	  {
	   
		models[i].SetActive(false);  //hides the others characters
	
	  }
	}

	public void ReSetModels()
	{

		SetModel(0);
	
		 		
	}
}//END_CLASS
}//END_NAMESPACE
