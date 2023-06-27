using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MetaverseSample{
public class TooltipView : MonoBehaviour {

	public bool IsActive {
		get {
			return gameObject.activeSelf;
		}
	}
	
	public UnityEngine.UI.Text tooltipTitle;
	public UnityEngine.UI.Text tooltipText;
	public bool onHover;


	public GameObject currentGameObject;

	void Awake() {
		instance = this;
		HideTooltip();
	}

	public void ShowTooltip(GameObject _currentGameObject, string title, string text, Vector2 _world_point) {


		onHover = true;

		currentGameObject = _currentGameObject;

		 var rect_transform = GetComponent<RectTransform>();
 
        rect_transform.anchoredPosition = WorldToCanvas(_world_point);

		
		if (tooltipTitle.text != title)
				tooltipTitle.text = title;

		if (tooltipText.text != text)
				tooltipText.text = text;

	

			gameObject.SetActive (true);
	
	}

	public void HideTooltip() {
		onHover = false;
		currentGameObject = null;
		gameObject.SetActive(false);
	}

	// Standard Singleton Access 
	public  static TooltipView instance;


	 public Vector2 WorldToCanvas( Vector3 world_position)
     {
         
         if(Camera.main!=null)
		 {
			var viewport_position = Camera.main.WorldToViewportPoint(world_position);
            var canvas_rect = GetComponent<RectTransform>();
 
            return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                            (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));

		 }
		 return Vector2.zero;
      
     }

	
}
}
