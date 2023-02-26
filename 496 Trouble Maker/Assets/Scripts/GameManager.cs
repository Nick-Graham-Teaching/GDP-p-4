using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UIElements;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    UnityTransport transport;
    private Transform cSpawnTransform;
    private Transform notCspawnTransform;
    private Transform mazePos;
    public GameObject mazeStart;
    public GameObject mazeMiddle;
    public GameObject mazeMiddle1;
    public GameObject mazeEnd;
    public GameObject trap;
    public GameObject mark;
    public Material activate;
    public GameObject GameStartUI;
    
    string ipAddress;
    [SerializeField] TextMeshProUGUI ipAddressText;
    [SerializeField] TMP_InputField ip;
    
    public static GameManager instance;
    
    GameObject camera;
    private List<GameObject> mazeStarts = new List<GameObject>();
    private List<GameObject> mazeMiddles = new List<GameObject>();
    private List<GameObject> mazeEnds = new List<GameObject>();


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
        ipAddress = "0.0.0.0";
        mazeStarts. Add(mazeStart);
        mazeMiddles.Add(mazeMiddle);
        mazeMiddles.Add(mazeMiddle1);
        mazeEnds.Add(mazeEnd);
        
        // SetIpAddress();
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
        int i = Random.Range(0, mazeStarts.Count);
        GameObject m = mazeStarts[i];
        GameObject first = Instantiate(m, mazePos.position, default);
        i = Random.Range(0, mazeMiddles.Count);
        m = mazeMiddles[i];
        GameObject middle = Instantiate(m, first.transform.Find("EndPoint").position, default);
        i = Random.Range(0, mazeEnds.Count);
        m = mazeEnds[i];
        Instantiate(m, middle.transform.Find("EndPoint").position, default);
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

    
    void SetIpAddress()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipAddress;
    }

    void GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log(ip);
                ipAddress = ip.ToString();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip.ToString(), default, ip.ToString());
            }
        }
    }

    public void OnHostButtonClick()
    {
        NetworkManager.Singleton.StartHost();
        GetLocalIPAddress();
        GameObject.Find("ConnectMenuUI").gameObject.SetActive(false);
        GameStartUI.SetActive(true);
    }

    public void OnClientButtonClick()
    {
        
        ipAddress = ip.text;
        SetIpAddress();
        if (NetworkManager.Singleton.StartClient())
        {
            GameObject.Find("ConnectMenuUI").gameObject.SetActive(false);
        }
        else Debug.Log("Client connect failed");
    }
    
}

