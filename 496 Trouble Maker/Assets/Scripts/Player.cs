using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Player : NetworkBehaviour
{
    private Transform player;
    
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
            this.name = "Host";
            player.position = GameManager.instance.ChallengerSpawnPos;
            player.GetComponent<Movement>().setIsCTrue();
            player.Find("Body").gameObject.SetActive(true);
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(false);
                this.name = "Client";
            }
            
        }
        else
        {
            this.name = "Client";
            player.position = GameManager.instance.ObsSpawnPos;
            player.GetComponent<Movement>().setIsCFalse();
            Destroy(player.GetComponent<Rigidbody>());
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(true);
                this.name = "Host";
            }
            
        }

        
    }
    // Update is called once per frame
    void Update()
    {

    }
    
    public void ChangeTurn()
    {
        bool move = player.GetComponent<Movement>().getMove();
        if (!move)
        {
            player.GetComponent<Movement>().setHostCanMove(true);
            //Debug.Log(player.GetComponent<Movement>().slow);
        }
        else if(move)
        {
            player.GetComponent<Movement>().setHostCanMove(false);
            //Debug.Log(player.GetComponent<Movement>().slow);
        }
        move = player.GetComponent<Movement>().getMove();
        Debug.Log(this.name);
        Debug.Log(move);
    }
    
}
