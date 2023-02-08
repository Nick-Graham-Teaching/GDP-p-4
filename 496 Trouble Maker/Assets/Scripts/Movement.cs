using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.PlayerLoop;

public class Movement : NetworkBehaviour
{
	private NetworkVariable<float> syncSpeed = new NetworkVariable<float>();
	private NetworkVariable<float> syncDir = new NetworkVariable<float>();
	private NetworkVariable<bool> syncIsSp = new NetworkVariable<bool>();
	private NetworkVariable<Vector3> syncVec = new NetworkVariable<Vector3>();
	private NetworkVariable<Quaternion> syncRot = new NetworkVariable<Quaternion>();

	public bool useCharacterForward = false;
    public bool lockToCameraForward = false;
    public float turnSpeed = 10f;
    public KeyCode sprintJoystick = KeyCode.JoystickButton2;
    public KeyCode sprintKeyboard = KeyCode.Space;

    private bool isC;
    private bool hostCanMove = true;
    private bool begin = false;
    
    private float turnSpeedMultiplier;
    private float speed = 0f;
    private float direction = 0f;
    private bool isSprinting = false;
    private Animator anim;
    private Vector3 targetDirection;
    private Vector2 input;
    private Quaternion freeRotation;
    private Camera mainCamera;
    private float velocity;
    private bool slow = false;
    private bool chaos = false;
    private bool blind = false;
    private bool stun = false;
    
    private float timer = 0;
    private float delayTime = 10f;
    
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    mainCamera = transform.parent.Find("Camera").GetComponent<Camera>();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		
	}

	public void setIsCTrue()
	{
		isC = true;
	}

	public void setIsCFalse()
	{
		isC = false;
	}

	private void Update()
	{
		if (IsLocalPlayer)
		{
			LocalInput();
		}
		if (!IsLocalPlayer)
		{
			SyncInput();
		}
	}

	[ServerRpc]
	void UpdateInputServerRpc(float speed, float dir, bool isSp, Vector3 vec, Quaternion rot)
	{
		syncDir.Value = dir;
		syncSpeed.Value = speed;
		syncIsSp.Value = isSp;
		syncVec.Value = vec;
		syncRot.Value = rot;
	}

	/// <summary>
	/// Game end
	/// </summary>
	[ServerRpc]
	void UpdateGameStatusServerRpc()
	{
		GameStatusClientRpc();
	}

	[ClientRpc]
	void GameStatusClientRpc()
	{
		hostCanMove = false;
		GameObject.Find("Host").transform.Find("Player").transform.GetComponent<Movement>().begin = false;
		GameObject.Find("Client").transform.Find("Player").transform.GetComponent<Movement>().begin = false;
		GameObject.Find("Canvas").transform.Find("Timer").gameObject.SetActive(false);
		Debug.Log("Game over, Challenger win!");
	}
	/// <summary>
	///  Game Start
	/// </summary>
	[ServerRpc]
	void UpdateBeginServerRpc()
	{
		BeginClientRpc();
	}

	[ClientRpc]
	void BeginClientRpc()
	{
		begin = true;
		GameObject.Find("Canvas").transform.Find("Timer").gameObject.SetActive(true);
		GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().begin = true;
		Debug.Log("Game Start");
		GameObject maze = GameManager.instance.CreateMaze();
		transform.position = maze.transform.Find("StartPos").transform.position;
		GameObject.Find("Client").transform.position = maze.transform.Find("Overview").transform.position;
	}
	
	/// <summary>
	/// Change Turn
	/// </summary>
	[ServerRpc]
	void UpdateTurnServerRpc()
	{
		TurnChangeClientRpc();
	}

	[ClientRpc]
	void TurnChangeClientRpc()
	{
		if(hostCanMove) hostCanMove = false;
		else if(!hostCanMove) hostCanMove = true;
		bool canMove = GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().hostCanMove;
		if (canMove) GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().hostCanMove = false;
		else if (!canMove) GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().hostCanMove = true;
		if (!hostCanMove)
		{ 
			if (isSprinting) isSprinting = false;
			if (slow) slow = false;
			if (chaos) chaos = false;
			if (blind) blind = false;
		}
	}
	
	
	/// <summary>
	/// Slow down
	/// </summary>
	[ServerRpc]
	void UpdateSlowDownServerRpc()
	{
		SlowDownClientRpc();
	}

	[ClientRpc]
	public void SlowDownClientRpc()
	{
		if(IsOwner) return;
		GameObject.Find("Host").transform.Find("Player").transform.GetComponent<Movement>()
			.slow = true;
	}

	/// <summary>
	/// Purify Slow down
	/// </summary>
	[ServerRpc]
	void UpdatePurifySlowServerRpc()
	{
		PurifySlowClientRpc();
	}

	[ClientRpc]
	void PurifySlowClientRpc()
	{
		slow = false;
	}

	/// <summary>
	/// Chaos
	/// </summary>
	[ServerRpc]
	void UpdateChaosStatusServerRpc()
	{
		ChaosStatusClientRpc();
	}

	[ClientRpc]
	void ChaosStatusClientRpc()
	{
		if(IsOwner) return;
		GameObject.Find("Host").transform.Find("Player").transform.GetComponent<Movement>()
			.chaos = true;
	}
	
	/// <summary>
	/// Acceleration
	/// </summary>
	[ServerRpc]
	void UpdateIsSprintingServerRpc()
	{
		IsSprintingClientRpc();
	}

	[ClientRpc]
	void IsSprintingClientRpc()
	{
		isSprinting = true;
	}

	/// <summary>
	/// Blind
	/// </summary>
	[ServerRpc]
	void UpdateBlindServerRpc()
	{
		BlindClientRpc();
	}

	[ClientRpc]
	void BlindClientRpc()
	{
		GameObject.Find("Host").transform.Find("Camera").transform.GetComponent<Camera>().fieldOfView = 10f;
	}

	/// <summary>
	///  Stun 
	/// </summary>
	[ServerRpc]
	void UpdateBeTrappedServerRpc(string str)
	{
		BeTrappedClientRpc(str);
	}

	[ClientRpc]
	void BeTrappedClientRpc(string str)
	{
		stun = true;
		speed = 0f;
		isSprinting = false;
		Destroy(GameObject.Find(str).gameObject);
	}

	/// <summary>
	/// Stun over
	/// </summary>
	[ServerRpc]
	void UpdateNotStunServerRpc()
	{
		NotStunClientRpc();
	}

	[ClientRpc]
	void NotStunClientRpc()
	{
		stun = false;
	}

	[ServerRpc]
	void UpdatePlaceTrapServerRpc(Vector3 point)
	{
		PlaceTrapClientRpc(point);
	}

	[ClientRpc]
	void PlaceTrapClientRpc(Vector3 point)
	{
		GameManager.instance.CreateTrap(point);
	}
	
	/// <summary>
	/// Update movement status
	/// </summary>
	void SyncInput()
	{
		anim.SetFloat("Speed", syncSpeed.Value);
		anim.SetFloat("Direction", syncDir.Value);
		anim.SetBool("isSprinting", syncIsSp.Value);
		transform.position = syncVec.Value;
		transform.rotation = syncRot.Value;
	}

	/// <summary>
	/// Basic Movement/Abilities
	/// </summary>
	void LocalInput()
	{
		if (isC) // Challenger
		{
			input.x = Input.GetAxis("Horizontal");
			input.y = Input.GetAxis("Vertical");

			// set speed to both vertical and horizontal inputs
			if (useCharacterForward)
				speed = Mathf.Abs(input.x) + input.y;
			else
				speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);

			speed = Mathf.Clamp(speed, 0f, 1f);
			speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
			anim.SetFloat("Speed", speed);

			if (input.y < 0f && useCharacterForward)
				direction = input.y;
			else
				direction = 0f;

			anim.SetFloat("Direction", direction);

			// set sprinting
			//isSprinting = ((Input.GetKey(sprintJoystick) || Input.GetKey(sprintKeyboard)) && input != Vector2.zero && direction >= 0f);
			anim.SetBool("isSprinting", isSprinting);

			// Update target direction relative to the camera view (or not if the Keep Direction option is checked)
			UpdateTargetDirection();
			if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
			{
				Vector3 lookDirection = targetDirection.normalized;
				freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
				var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
				var eulerY = transform.eulerAngles.y;

				if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
				var euler = new Vector3(0, eulerY, 0);

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
			}
			
			// Game start
			if (IsHost && Input.GetKeyDown(KeyCode.Alpha1))
			{
				UpdateBeginServerRpc();
			}

			// Change turn
			if (begin)
			{
				timer += Time.deltaTime;
				if (timer > delayTime)
				{
					UpdateTurnServerRpc();
					timer = 0;
				}
			}

			if (stun)
			{
				timer += Time.deltaTime;
				if (timer > 4f)
				{
					UpdateNotStunServerRpc();
				}
			}

			if (!hostCanMove)
			{
				transform.position += transform.forward * 0f;
			}
			else if (hostCanMove && slow)
			{
				transform.position += transform.forward * speed * Time.deltaTime * 2.5f;
			}

			else if (hostCanMove && chaos)
			{
				transform.position += transform.forward * speed * Time.deltaTime * -3.5f;
			}
			
			else if (hostCanMove && stun)
			{
				transform.position += transform.forward * 0f;
			}

			else
			{
				transform.position += transform.forward * speed * Time.deltaTime * 5;
			}

			if (hostCanMove) // Abilites
			{
				// Purify slow down
				if (Input.GetKeyDown(KeyCode.R)&&slow)
				{
					UpdatePurifySlowServerRpc();
					Debug.Log("Purify slow");
				}
				
				// Accelerate
				if (Input.GetKeyDown(KeyCode.Space))
				{
					UpdateIsSprintingServerRpc();
					Debug.Log("Sprinting");
				}
			}

		}

		if (!isC) // Obstructionists
		{
			// Camera Movement
			input.x = Input.GetAxis("Horizontal");
			input.y = Input.GetAxis("Vertical");
			if (useCharacterForward)
				speed = Mathf.Abs(input.x) + input.y;
			else
				speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);
			UpdateTargetDirection();
			if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
			{
				Vector3 lookDirection = targetDirection.normalized;
				freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
				var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
				var eulerY = transform.eulerAngles.y;

				if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
				var euler = new Vector3(0, eulerY, 0);

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler),
					turnSpeed * turnSpeedMultiplier * Time.deltaTime);
			}

			Vector3 moveDirection = new Vector3(input.x, 0.0f, input.y);
			transform.position += transform.forward * speed * Time.deltaTime * 5;

			if (!hostCanMove) // Abilities
			{
				// slow ability
				if (Input.GetKeyDown(KeyCode.R))
				{
					Debug.Log("slow pressed!");
					UpdateSlowDownServerRpc();
				}

				// chaos ability
				if (Input.GetKeyDown(KeyCode.T))
				{
					Debug.Log("Chaos pressed");
					UpdateChaosStatusServerRpc();
				}

				// blind
				if (Input.GetKeyDown(KeyCode.Y))
				{
					Debug.Log("Blind pressed");
				}

				if (Input.GetMouseButtonUp(0)) 
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit))
					{
						// Debug.Log(hit.transform.parent.name);
						Vector3 point = hit.point;
						UpdatePlaceTrapServerRpc(point);
					}
					
					Debug.Log("Placed trap");
				}
			}
		}
		
		UpdateInputServerRpc(speed, direction, isSprinting, transform.position, transform.rotation);
	}
	
    public virtual void UpdateTargetDirection()
    {
        if (!useCharacterForward)
        {
            turnSpeedMultiplier = 1f;
            var forward = mainCamera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = mainCamera.transform.TransformDirection(Vector3.right);

            // determine the direction the player will face based on input and the referenceTransform's right and forward directions
            targetDirection = input.x * right + input.y * forward;
        }
        else
        {
            turnSpeedMultiplier = 0.2f;
            var forward = transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = transform.TransformDirection(Vector3.right);
            targetDirection = input.x * right + Mathf.Abs(input.y) * forward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
	    if (other.tag == "Finish")
	    {
		    UpdateGameStatusServerRpc();
	    }

	    if (other.tag == "Trap")
	    {
		    UpdateBeTrappedServerRpc(other.name);
		    Debug.Log("Stun");
	    }
    }
    
}
