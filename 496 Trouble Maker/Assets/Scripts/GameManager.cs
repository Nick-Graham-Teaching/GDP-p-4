using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool begin = false;
    private Transform cSpawnTransform;
    private Transform notCspawnTransform;
    private Transform mazePos;
    public GameObject maze;
    public GameObject trap;

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

    public GameObject CreateMaze()
    {
        return Instantiate(maze, mazePos.position, default);
    }

    public void CreateTrap(Vector3 pos)
    {
        Instantiate(trap,pos, default);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        cSpawnTransform = transform.Find("Cube");
        notCspawnTransform = transform.Find("Overview");
        mazePos = GameObject.Find("Ground").transform;
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
        
    }
    
    public void CloseCamera()
    {
        camera.SetActive(false);
    }
    
}
