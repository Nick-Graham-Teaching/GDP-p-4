using System.Collections;
using System.Collections.Generic;
using Den.Tools;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool begin = false;
    private Transform cSpawnTransform;
    private Transform notCspawnTransform;
    
    private float timer = 0;
    private float delayTime = 5.0f;

    public static GameManager instance;
    GameObject camera;

    public Vector3 ChallengerSpawnPos
    {
        get { return cSpawnTransform.position; }
    }

    public Vector3 ObsSpawnPos
    {
        get{return notCspawnTransform.position;}
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        cSpawnTransform = transform.Find("Cube");
        notCspawnTransform = transform.Find("Overview");
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
                    //Debug.Log("There is already a server running");
                    return;
                }

                NetworkManager.Singleton.StartHost();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Begin");
            begin = true;
        }

        if (begin)
        {
            timer += Time.deltaTime;
            if (timer > delayTime)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var g in players)
                {
                    g.GetComponent<Player>().ChangeTurn();
                }
                timer = 0;
                //Debug.Log("Change");
            }
        }
    }
    
    public void CloseCamera()
    {
        camera.SetActive(false);
    }
    
}
