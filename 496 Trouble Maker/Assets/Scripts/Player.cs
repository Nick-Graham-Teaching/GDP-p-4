using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;


public class Player : NetworkBehaviour
{
    private Transform player;
    
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
            //player.Find("Skeleton").gameObject.SetActive(true);
            player.transform.GetComponent<Collider>().enabled = true;
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(false);
                // player.Find("Skeleton").gameObject.SetActive(false);
                player.transform.GetComponent<CapsuleCollider>().enabled = false;
                name = "Client";
            }
        }
        else
        {
            name = "Client";
            player.position = GameManager.instance.ObsSpawnPos;
            player.GetComponent<Movement>().SetIsCFalse();
            // Destroy(player.GetComponent<Rigidbody>());
            transform.Find("Camera").GetComponent<CinemachineBrain>().enabled = false;
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(true);
                // player.Find("Skeleton").gameObject.SetActive(true);
                player.transform.GetComponent<CapsuleCollider>().enabled = true;
                name = "Host";
            }
        }
    }
    /*
    public override void OnNetworkDespawn()
    {
        Application.Quit();
    }*/
    
    public void OnReadyButtonClick()
    {
        if (GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().StarGame())
        {
            GameObject.Find("Ready").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GameStart").gameObject.SetActive(true);
            GameObject.Find("IP Address").gameObject.SetActive(false);
        }
        else return;
    }

    public void onBeginButtonClick()
    {
        GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().Begin();
        GameObject.Find("GameStart").gameObject.SetActive(false);
    }
}
