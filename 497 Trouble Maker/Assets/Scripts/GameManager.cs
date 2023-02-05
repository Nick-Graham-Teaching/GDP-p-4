using System.Collections;
using System.Collections.Generic;
using Den.Tools;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    private Transform spawnTransform;

    public static GameManager instance;
    GameObject camera;

    public Vector3 SpawnPos
    {
        get { return spawnTransform.position; }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawnTransform = transform.Find("Cube");
        camera = transform.Find("Main Camera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsHost&&!NetworkManager.Singleton.IsClient)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("There is already a server running");
                    return;
                }

                NetworkManager.Singleton.StartHost();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                NetworkManager.Singleton.StartClient();
            }
        }
        else
        {
            
        }
    }

    public void CloseCamera()
    {
        camera.SetActive(false);
    }
}
