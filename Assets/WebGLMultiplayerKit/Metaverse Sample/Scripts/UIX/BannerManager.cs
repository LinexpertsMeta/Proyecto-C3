using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MetaverseSample{
public class BannerManager : MonoBehaviour
{
	
	public bool isOpen;

	public string description;

	public float price;

	public GameObject nft_sprite;

	public string nft_link;
	
	[Header("Max Player Distance")]
	public float maxDistance = 1f;
	
	public bool OnPlayer;

	public enum BannerMode {NFT,INFO}; 

	public BannerMode bannerMode;



	
    // Start is called before the first frame update
    void Start()
    {
        
    }
public void Update()
	{
		OnCustomCollider ();
	}

	/// <summary>
	/// method for detecting the proximity of the player to the Banner
	/// </summary>
	void OnCustomCollider()
	{
	
		if(NetworkManager.instance.localPlayer!=null )
		{
			Vector3 meToPlayer = transform.position - NetworkManager.instance.localPlayer.transform.position;

			//Debug.Log("meToPlayer.sqrMagnitude: "+meToPlayer.sqrMagnitude);

			//check if player is near
			if (meToPlayer.sqrMagnitude < maxDistance) 
			{ 
				if (!OnPlayer) {
					OnPlayer = true;

					if(!isOpen)
					{
						if(bannerMode.Equals(BannerMode.NFT))
						{
							// shows a small tolltip when the player is close
						    TooltipView.instance.ShowTooltip(gameObject,"Open", "NFT", new Vector2(transform.position.x,transform.position.z));
						

						}
						else
						{
							// shows a small tolltip when the player is close
						    TooltipView.instance.ShowTooltip(gameObject,"Open", "INFO", new Vector2(transform.position.x,transform.position.z));
							
						}
					    
			            if (Input.GetKey (KeyCode.LeftAlt)||Input.GetMouseButton(0))
		                {
							isOpen = true;
							TooltipView.instance.HideTooltip();

							if(bannerMode.Equals(BannerMode.NFT))
						    {
								CanvasManager.instance.OpenPurchasePanel(description, nft_sprite,price.ToString(),CanvasManager.instance.defaultPublicAdrrTo,nft_link,this);
						
						    }
						    else
						    {
								CanvasManager.instance.OpenInfoPanel(description, nft_sprite,nft_link,this);
							
						    }
						
		                }
			
					}

				}
				
			    if (Input.GetKeyUp (KeyCode.LeftAlt) || Input.GetMouseButton(0))
		        {	
			        //open the panel
					isOpen = true;
					TooltipView.instance.HideTooltip();
					if(bannerMode.Equals(BannerMode.NFT))
					{
						CanvasManager.instance.OpenPurchasePanel(description, nft_sprite,price.ToString(),CanvasManager.instance.defaultPublicAdrrTo,nft_link,this);
						
					}
					else
					{
						CanvasManager.instance.OpenInfoPanel(description, nft_sprite,nft_link,this);
							
					}
					

		        }

			}

			else
			{
				if(OnPlayer)
				{
					OnPlayer = false;
					isOpen = false;

					TooltipView.instance.HideTooltip();
				
				}
			}

		}
	}

}
}
