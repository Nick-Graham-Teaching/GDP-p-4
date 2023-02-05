using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        GameManager.instance.CloseCamera();
        transform.Find("Player").position = GameManager.instance.SpawnPos;
        if (IsLocalPlayer)
        {
            transform.Find("PlayerCameraControl").gameObject.SetActive(true);
            transform.Find("Camera").gameObject.SetActive(true);
        }

        if (IsHost)
        {
            transform.Find("Player").GetComponent<Movement>().enabled = true;
        }
        else
        {
            transform.Find("Player").GetComponent<overview>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
