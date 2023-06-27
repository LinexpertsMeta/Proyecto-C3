using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;

/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
namespace MetaverseSample{
public class PlayerManager : MonoBehaviour {

	public string	id;

	public string name;

	public TMP_Text txtName;

	public bool isOnline;

	public bool isLocalPlayer;

	public bool move;

	public bool mute;

	public Rigidbody myRigidbody;

	//distances low to arrive close to the player
	[Range(1f, 200f)][SerializeField] float minDistanceToPlayer = 10f ;


	public int current_model;


	public Animator _animator;

	float h ;
	
	float v;

	public ThirdPersonController thirdPersonController;

	StarterAssetsInputs _input;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDMotionSpeed; 
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
	private float _animationBlend;

	private bool _hasAnimator;

	// timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;






	// Use this for initialization
	void Awake () {

		if (GetComponent<StarterAssetsInputs>())
		{
			_input = GetComponent<StarterAssetsInputs>();
		}
		
	    _animIDSpeed = Animator.StringToHash("Speed");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		_animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");

		 // reset our timeouts on start
        _jumpTimeoutDelta = thirdPersonController.JumpTimeout;
         _fallTimeoutDelta =  thirdPersonController.FallTimeout;
	
		
	}
	

	// Use this for initialization
	public void Set3DName(string name)
	{
		if(!isLocalPlayer)
		{
			txtName.text = name;

		}
		else
		{
			txtName.text = string.Empty;

		}
		

	}


	void Update()
	{
		if (isLocalPlayer)
		{

			_hasAnimator = TryGetComponent(out _animator);

			 // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? thirdPersonController.SprintSpeed : thirdPersonController.MoveSpeed;

			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * thirdPersonController.SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

			  // update animator if using character
            if (_hasAnimator)
            {
                
                 
                NetworkManager.instance.EmitAnimation (_animIDSpeed, _animationBlend.ToString(),"float");  
                NetworkManager.instance.EmitAnimation (_animIDMotionSpeed,inputMagnitude.ToString(),"float");  

 
                
            }

			EmitJumpToServer();

			UpdateStatusToServer ();

		}

	}	


	void UpdateStatusToServer ()
	{


		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["posX"] = transform.position.x.ToString();
		data["posY"] = transform.position.y.ToString();
		data["posZ"] = transform.position.z.ToString();

		data["rotation"] = transform.rotation.x+";"+transform.rotation.y+";"+transform.rotation.z+";"+transform.rotation.w;


		NetworkManager.instance.EmitMoveAndRotate(data);



	}

	void EmitJumpToServer()
	{
		if(thirdPersonController.Grounded)
		{
			// reset the fall timeout timer
            _fallTimeoutDelta = thirdPersonController.FallTimeout;

			// update animator if using character
            if (_hasAnimator)
            {
				NetworkManager.instance.EmitAnimation (_animIDJump, "false","bool");
                NetworkManager.instance.EmitAnimation (_animIDFreeFall, "false","bool");
			}

		
			// Jump
            if (_input.jump &&  _jumpTimeoutDelta <= 0.0f)
            {
                
                // update animator if using character
                if (_hasAnimator)
                {
                    
					NetworkManager.instance.EmitAnimation (_animIDJump, "true","bool");
                }
            }
			// jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

		}//END_IF
				
		else
		{
			// reset the jump timeout timer
            _jumpTimeoutDelta = thirdPersonController.JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
					NetworkManager.instance.EmitAnimation (_animIDFreeFall, "true","bool");
                   
                }
            }

					
		}//END_ELSE

		// update animator if using character
        if (_hasAnimator)
        {
			NetworkManager.instance.EmitAnimation (_animIDGrounded, thirdPersonController.Grounded.ToString(),"bool");
        }
	}	




	public void UpdatePosition(Vector3 position)
	{
	
		transform.position = new Vector3 (position.x, position.y, position.z);
		
	}

	public void UpdateRotation(Quaternion _rotation)
	{
		transform.rotation = _rotation;

	}

	

}//END_CLASS
}//END_NAMESPACE