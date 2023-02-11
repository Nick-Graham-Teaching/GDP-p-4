using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private Transform cSpawnTransform;
    private Transform notCspawnTransform;
    private Transform mazePos;
    public GameObject mazeFirst;
    public GameObject mazeMiddle;
    public GameObject trap;
    public GameObject mark;
    public Material activate;
    

    public static GameManager instance;
    GameObject camera;
   
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        cSpawnTransform = transform.Find("Cube");
        notCspawnTransform = transform.Find("Overview");
        mazePos = GameObject.Find("MazePosition").transform;
        camera = transform.Find("Main Camera").gameObject;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        // CreateMaze();
    }

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
        GameObject first = Instantiate(mazeFirst, mazePos.position, default);
        GameObject middle = Instantiate(mazeMiddle, first.transform.Find("EndPoint").position, default);
        return first;
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

    public void CreateMark(Vector3 pos)
    {
        Instantiate(mark, pos, default);
    }

    public void EraseMarks()
    {
        GameObject[] marks = GameObject.FindGameObjectsWithTag("Mark");
        foreach (GameObject m in marks)
        {
            Destroy(m);
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

