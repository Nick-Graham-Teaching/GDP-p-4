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
    public Material activate;

    private float timer = 0;
    private float delayTime = 5.0f;

    public static GameManager instance;
    GameObject camera;
    

    /// <summary>
    /// Position Challenger Spawn
    /// </summary>
    public Vector3 ChallengerSpawnPos
    {
        get { return cSpawnTransform.position; }
    }

    /// <summary>
    /// Position Obstructionist Spawn
    /// </summary>
    public Vector3 ObsSpawnPos
    {
        get { return notCspawnTransform.position; }
    }

    /// <summary>
    ///  Instantiate specific maze
    /// </summary>
    /// <returns></returns>
    public GameObject CreateMaze()
    {
        return Instantiate(maze, mazePos.position, default);
    }

    /// <summary>
    /// Instantiate trap 
    /// </summary>
    /// <param name="pos"></param>
    public void CreateTrap(Vector3 pos)
    {
        if (GameObject.Find("Trap") == null)
        {
            Instantiate(trap, pos, default);
            Debug.Log("Placed trap");
        }
        else
        {
            Debug.Log("Trap already be generated");
        }
    }

    /// <summary>
    /// Activate specific obstacle
    /// </summary>
    public bool CreateObstacle(string n)
    {
        if (GameObject.Find(n).transform.GetComponent<MeshRenderer>().material != activate)
        {
            GameObject.Find(n).transform.GetComponent<MeshRenderer>().material = activate;
            GameObject.Find(n).transform.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("Placed obstacle");
            return true;
        }
        else return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        cSpawnTransform = transform.Find("Cube");
        notCspawnTransform = transform.Find("Overview");
        mazePos = GameObject.Find("Ground").transform;
        camera = transform.Find("Main Camera").gameObject;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {

            NetworkManager.Singleton.StartClient();
        }
    }

    public void CloseCamera()
    {
        camera.SetActive(false);
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            Debug.Log("Too much players");
            response.Approved = false;
        }
        else
        {
            var clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            var connectionData = request.Payload;
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
            response.Pending = false;
        }
    }
}

