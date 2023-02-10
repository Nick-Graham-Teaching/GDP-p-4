using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

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
	private bool hostCanMove = false;
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
	private bool growUp = false;
	private bool invisiable = false;
	private Vector3 lastPos = new Vector3();
	
	private float timer = 0;
	private float stunTimer = 0;
	public float delayTime = 10f;
	private float stunSpeed = 1f;
	private float chaosSpeed = 1f;
	private float slowSpeed = 1f;
	
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		mainCamera = transform.parent.Find("Camera").GetComponent<Camera>();
	}


	public void SetIsCTrue() { isC = true; }
	public void SetIsCFalse() { isC = false; }
	
	// Update is called once per frame
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
	void UpdateGameStatusServerRpc() { GameStatusClientRpc(); }

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
	void UpdateBeginServerRpc() { BeginClientRpc(); }

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
	void UpdateTurnServerRpc() { TurnChangeClientRpc(); }

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
			if (growUp) growUp = false;
		}
		else if (hostCanMove)
		{
			lastPos = GameObject.Find("Host").transform.Find("Player").transform.localPosition;
			GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().lastPos = GameObject.Find("Host").
				transform.Find("Player").transform.localPosition;
		}
		
	}
	
	
	/// <summary>
	/// Slow down
	/// </summary>
	[ServerRpc]
	void UpdateSlowDownServerRpc() { SlowDownClientRpc(); }

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
	void UpdatePurifyServerRpc() { PurifyClientRpc(); }

	[ClientRpc]
	void PurifyClientRpc()
	{
		if (IsOwner)
		{ 
			List<string> l = new List<string>();
			if (stun) l.Add("stun");
			if (slow) l.Add("slow");
			if (blind) l.Add("blind");
			if (chaos) l.Add("chaos");
			if (l.Count == 0)
			{
				Debug.Log("No debuff found");
				return;
			}
			int i = Random.Range(0, l.Count);
			string debuff = l[i];
			if (debuff == "stun") stun = false;
			else if (debuff == "slow") slow = false;
			else if (debuff == "blind") blind = false;
			else if (debuff == "chaos")  chaos = false;
			Debug.Log("Purify " + debuff);
		}
	}

	/// <summary>
	/// Chaos
	/// </summary>
	[ServerRpc]
	void UpdateChaosStatusServerRpc() { ChaosStatusClientRpc(); }

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
	void UpdateIsSprintingServerRpc() { IsSprintingClientRpc(); }

	[ClientRpc]
	void IsSprintingClientRpc() { isSprinting = true; }

	/// <summary>
	/// Blind
	/// </summary>
	[ServerRpc]
	void UpdateBlindServerRpc() { BlindClientRpc(); }

	[ClientRpc]
	void BlindClientRpc() { GameObject.Find("Host").transform.Find("Camera").transform.GetComponent<Camera>().fieldOfView = 10f; }

	/// <summary>
	/// Stun 
	/// </summary>
	[ServerRpc]
	void UpdateBeTrappedServerRpc(string str) { BeTrappedClientRpc(str); }

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
	void UpdateNotStunServerRpc() { NotStunClientRpc(); }

	[ClientRpc]
	void NotStunClientRpc() { stun = false; }

	/// <summary>
	/// Place trap
	/// </summary>
	/// <param name="point"></param>
	[ServerRpc]
	void UpdatePlaceTrapServerRpc(Vector3 point) { PlaceTrapClientRpc(point); }
	

	[ClientRpc]
	void PlaceTrapClientRpc(Vector3 point) { GameManager.instance.CreateTrap(point); }

	/// <summary>
	/// Growing up
	/// </summary>
	[ServerRpc]
	void UpdateGrowUpServerRpc()
	{
		GrowUpClientRpc();
	}

	[ClientRpc]
	void GrowUpClientRpc()
	{
		growUp = true;
	}

	/// Invisible
	[ServerRpc]
	void UpdateInvisibleServerRpc() { InvisibleClientRpc(); }

	[ClientRpc]
	void InvisibleClientRpc() 
	{ 
		transform.Find("Body").gameObject.SetActive(false);
		invisiable = true;
		GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().invisiable = true;
	}
	
	/// Cancel Invisible
	[ServerRpc]
	void UpdateCancelInvisibleServerRpc(){ CancleInvisibleClientRpc(); }

	[ClientRpc]
	void CancleInvisibleClientRpc()
	{
		invisiable = false;
		transform.Find("Body").gameObject.SetActive(true);
	}
	
	
	/// <summary>
	/// Teleport
	/// </summary>
	/// <param name="pos"></param>
	[ServerRpc] 
	void UpdateTeleportServerRpc(Vector3 pos) { TeleportClientRpc(pos); }

	[ClientRpc]
	void TeleportClientRpc(Vector3 pos)
	{
		GameObject.Find("Host").transform.Find("Player").transform.localPosition = pos;
	}

	/// <summary>
	/// Increase time left this turn
	/// </summary>
	[ServerRpc]
	void UpdateTimeServerRpc()
	{
		TimeClientRpc();
	}

	[ClientRpc]
	void TimeClientRpc()
	{
		timer -= 5;
		GameObject.Find("Client").transform.Find("Player").GetComponent<Movement>().timer -= 5;
		GameObject.Find("Canvas").transform.Find("Timer").GetComponent<TimeCounter>().TimeIncreased();
	}

	void SyncInput()
	{
		anim.SetFloat("Speed", syncSpeed.Value);
		anim.SetFloat("Direction", syncDir.Value);
		anim.SetBool("isSprinting", syncIsSp.Value);
		transform.position = syncVec.Value;
		transform.rotation = syncRot.Value;
	}


	void LocalInput()
	{
		if (isC) // Challenger
		{
			input.x = Input.GetAxis("Horizontal");
			input.y = Input.GetAxis("Vertical");

			// set speed to both vertical and horizontal inputs
			if (useCharacterForward) speed = Mathf.Abs(input.x) + input.y;
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

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler),
					turnSpeed * turnSpeedMultiplier * Time.deltaTime);
			}

			// Game start
			if (IsHost && Input.GetKeyDown(KeyCode.Alpha1))
			{
				UpdateBeginServerRpc();
				UpdateTurnServerRpc();
			}

			// Change turn
			if (begin)
			{
				timer += Time.deltaTime;
				if (timer > delayTime)
				{
					UpdateTurnServerRpc();
					Debug.Log(invisiable);
					if(invisiable) UpdateCancelInvisibleServerRpc();
					timer = 0;
				}
			}

			// Be trapped
			if (stun)
			{
				stunTimer += Time.deltaTime;
				if (stunTimer > 4f)
				{
					UpdateNotStunServerRpc();
					stunTimer = 0;
				}
				stunSpeed = 0f;
			}

			else if (!stun) stunSpeed = 1f;
			if (slow) slowSpeed = 0.5f;
			else if (!slow) slowSpeed = 1f;
			if (chaos) chaosSpeed = -1f;
			else if (!chaos) chaosSpeed = 1f;
			
			// Grow up
			if (growUp)
			{
				transform.parent.Find("PlayerCameraControl").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 80f;
				transform.parent.Find("PlayerCameraControl").GetComponentInChildren<CinemachineFramingTransposer>().m_TrackedObjectOffset.y = 5;
				transform.parent.Find("PlayerCameraControl").GetComponent<CinemachineVirtualCamera>()
					.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 6;
			}
			else
			{
				transform.parent.Find("PlayerCameraControl").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 40f;
				transform.parent.Find("PlayerCameraControl").GetComponentInChildren<CinemachineFramingTransposer>().m_TrackedObjectOffset.y = 1.6f;
				transform.parent.Find("PlayerCameraControl").GetComponent<CinemachineVirtualCamera>()
					.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 2.22f;
			}

			if (!hostCanMove)
			{
				transform.position += transform.forward * 0f;
			}

			else
			{
				transform.position += transform.forward * speed * Time.deltaTime * 5 * slowSpeed * chaosSpeed * stunSpeed;
			}

			if (hostCanMove) // Abilites
			{
				// Purify
				if (Input.GetKeyDown(KeyCode.R))
				{
					UpdatePurifyServerRpc();
				}

				// Accelerate
				if (Input.GetKeyDown(KeyCode.Space))
				{
					UpdateIsSprintingServerRpc();
					Debug.Log("Sprinting");
				}
				
				// Growing up
				if (Input.GetKeyDown(KeyCode.G))
				{
					UpdateGrowUpServerRpc();
					Debug.Log("Grow up");
				}
				
				// Increase time
				if (Input.GetKeyDown(KeyCode.T))
				{
					UpdateTimeServerRpc();
					Debug.Log("Time increaed");
				}
				
				// Invisable
				if (Input.GetKeyDown(KeyCode.I))
				{
					UpdateInvisibleServerRpc();
					Debug.Log("Invisible");
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
	
			// always false
			slow = false;
			stun = false;
			chaos = false;

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

				// Trap
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

				// Teleport
				if (Input.GetKeyDown(KeyCode.E))
				{
					UpdateTeleportServerRpc(lastPos);
					Debug.Log("Teleport pressed");
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
