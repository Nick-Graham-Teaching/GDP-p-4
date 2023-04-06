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
    public GameObject mazeStart1;
    public GameObject mazeStart2;
    public GameObject mazeStart3;
    public GameObject mazeMiddle11;
    public GameObject mazeMiddle12;
    public GameObject mazeMiddle13;
    public GameObject mazeMiddle21;
    public GameObject mazeMiddle22;
    public GameObject mazeMiddle23;
    public GameObject mazeEnd1;
    public GameObject mazeEnd2;
    public GameObject mazeEnd3;
    public GameObject trap;
    public GameObject mark;
    public Material activate;
    public GameObject GameReadyUI;
    public GameObject instruction;
    public GameObject controllerUI;
    public GameObject challengerUI;
    
    string ipAddress;
    [SerializeField] TextMeshProUGUI ipAddressText;
    [SerializeField] TMP_InputField ip;
    
    public static GameManager instance;
    
    GameObject camera;
    private List<GameObject> mazeStarts = new List<GameObject>();
    private List<GameObject> mazeMiddleList1 = new List<GameObject>();
    private List<GameObject> mazeMiddleList2 = new List<GameObject>();
    private List<GameObject> mazeEnds = new List<GameObject>();

    public GameObject challengerCards;
    public GameObject controllerCards;
    public bool server;


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
        mazeStarts.Add(mazeStart1);
        mazeStarts.Add(mazeStart2);
        mazeStarts.Add(mazeStart3);
        mazeMiddleList1.Add(mazeMiddle11);
        mazeMiddleList1.Add(mazeMiddle12);
        mazeMiddleList1.Add(mazeMiddle13);
        mazeMiddleList2.Add(mazeMiddle21);
        mazeMiddleList2.Add(mazeMiddle22);
        mazeMiddleList2.Add(mazeMiddle23);
        mazeEnds.Add(mazeEnd1);
        mazeEnds.Add(mazeEnd2);
        mazeEnds.Add(mazeEnd3);
        
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
        
        i = Random.Range(0, mazeMiddleList1.Count);
        m = mazeMiddleList1[i];
        GameObject middle1 = Instantiate(m, first.transform.Find("EndPoint").position, default);
        
        i = Random.Range(0, mazeMiddleList2.Count);
        m = mazeMiddleList2[i];
        GameObject middle2 = Instantiate(m, middle1.transform.Find("EndPoint").position, default);
        
        i = Random.Range(0, mazeEnds.Count);
        m = mazeEnds[i];
        Instantiate(m, middle2.transform.Find("EndPoint").position, default);
        return first;
    }

    /// <summary>
    /// Instantiate trap 
    /// </summary>
    /// <param name="pos"></param>
    public void CreateTrap(Vector3 pos)
    {
        if (GameObject.FindGameObjectsWithTag("Trap").Length <= 10)
        {
            GameObject g = Instantiate(trap, pos, default);
            g.transform.eulerAngles = new Vector3(90, 0, 0);
            Debug.Log("Placed trap");
        }
        else
        {
            Debug.Log("Maximum number of traps reach");
        }
    }

    public void CreateMark(Vector3 pos, Vector3 v)
    {
        GameObject g = Instantiate(mark, pos, default);
        g.transform.eulerAngles = v;
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
    public String CreateObstacle(string n)
    {
        if (!GameObject.Find(n).transform.GetComponent<MeshRenderer>().enabled)
        {
            GameObject.Find(n).transform.GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find(n).transform.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("Placed obstacle");
            return n;
        }
        else return null;
    }

    public void DestroyObstacle(string n)
    {
        if (GameObject.Find(n).transform.GetComponent<MeshRenderer>().enabled)
        {
            GameObject.Find(n).transform.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find(n).transform.GetComponent<BoxCollider>().isTrigger = true;
            Debug.Log("Destroy obstacle");
        }
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
        ipAddressText.gameObject.SetActive(true);
        ipAddressText.text = ipAddress;
        GameReadyUI.SetActive(true);
        CloseCamera();
        instruction.SetActive(true);
        challengerUI.SetActive(true);
    }

    public void OnClientButtonClick()
    {
        
        ipAddress = ip.text;
        SetIpAddress();
        if (NetworkManager.Singleton.StartClient())
        {
            GameObject.Find("ConnectMenuUI").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Wait").gameObject.SetActive(true);
            instruction.SetActive(true);
            controllerUI.SetActive(true);
        }
        else Debug.Log("Client connect failed");
        CloseCamera();
    }
    
    public void Draw()
    {
        if (server)
        {
            challengerCards.GetComponent<cardControl>().draw();
        }
        else
        {
            controllerCards.GetComponent<cardControlCli>().draw();
        }
    }

    public void SetServerCanUse(bool b)
    {
        GameObject.Find("Canvas").transform.Find("ServerCards").GetComponent<cardControl>().canUse = b;
    }
    
    public void SetClientCanUse(bool b)
    {
        GameObject.Find("Canvas").transform.Find("ClientCards").GetComponent<cardControlCli>().canUse = b;
    }
    
}

