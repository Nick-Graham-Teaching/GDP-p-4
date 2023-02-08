using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Player : NetworkBehaviour
{
    private Transform player;
    private float timer = 0;
    private float delayTime = 5.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        GameManager.instance.CloseCamera();
        player = transform.Find("Player");
        if (IsLocalPlayer)
        {
            transform.Find("PlayerCameraControl").gameObject.SetActive(true);
            transform.Find("Camera").gameObject.SetActive(true);
        }

        if (IsHost)
        {
            name = "Host";
            player.position = GameManager.instance.ChallengerSpawnPos;
            player.GetComponent<Movement>().SetIsCTrue();
            player.Find("Body").gameObject.SetActive(true);
            player.Find("Skeleton").gameObject.SetActive(true);
            player.transform.GetComponent<Collider>().enabled = true;
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(false);
                player.Find("Skeleton").gameObject.SetActive(false);
                player.transform.GetComponent<CapsuleCollider>().enabled = false;
                name = "Client";
            }
            
        }
        else
        {
            name = "Client";
            player.position = GameManager.instance.ObsSpawnPos;
            player.GetComponent<Movement>().SetIsCFalse();
            Destroy(player.GetComponent<Rigidbody>());
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(true);
                player.Find("Skeleton").gameObject.SetActive(true);
                player.transform.GetComponent<CapsuleCollider>().enabled = true;
                name = "Host";
            }
            
        }

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeTurn()
    {
        
    }

    
    
}
