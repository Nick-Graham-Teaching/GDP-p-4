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

    //private bool full = true;
    
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
            transform.Find("Camera").GetComponent<CinemachineBrain>().enabled = false;
            if (!IsLocalPlayer)
            {
                player.Find("Body").gameObject.SetActive(true);
                player.Find("Skeleton").gameObject.SetActive(true);
                player.transform.GetComponent<CapsuleCollider>().enabled = true;
                name = "Host";
                
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GetLocalIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) 
        {
            Debug.Log(ip.ToString()); 
        }
    }

    public void OnStartButtonClick()
    {
        if (GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().StarGame())
        {
            GameObject.Find("GameStart").gameObject.SetActive(false);
        }
        else return;
    }
}
