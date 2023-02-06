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
    
    private float timer = 0;
    private float delayTime = 5.0f;
    
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

	public bool getMove()
	{
		return hostCanMove;
	}
	public void setHostCanMove(bool tf)
	{
		hostCanMove = tf;
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

	[ServerRpc]
	void UpdateSlowStatusServerRpc()
	{
		SlowDownClientRpc();
	}

	[ClientRpc]
	public void SlowDownClientRpc()
	{
		if(IsOwner) return;
		GameObject.Find("Host").transform.Find("Player").transform.GetComponent<Movement>()
			.slow = true;
		Debug.Log(transform.parent.name);
		Debug.Log(slow);
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
			isSprinting = ((Input.GetKey(sprintJoystick) || Input.GetKey(sprintKeyboard)) && input != Vector2.zero && direction >= 0f);
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
			if (hostCanMove&&slow)
			{ transform.position += transform.forward * speed * Time.deltaTime * 0.5f; }

			else if (hostCanMove&&!slow)
			{ transform.position += transform.forward * speed * Time.deltaTime * 5;}

			if (Input.GetKeyDown(KeyCode.K))
			{
				Debug.Log(slow);
				//slow = false;
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

			if (!hostCanMove)
			{
				// slow ability
				if (Input.GetKeyDown(KeyCode.R)) 
				{
					Debug.Log("slow preesed!");
					if (slow) { slow = false; }
					else { slow = true; }
				}
			}
			if (Input.GetKeyDown(KeyCode.R)) 
			{
				Debug.Log("slow preesed!");
				UpdateSlowStatusServerRpc();
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
}
