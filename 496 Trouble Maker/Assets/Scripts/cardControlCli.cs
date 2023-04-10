using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardControlCli : MonoBehaviour
{

    public GameObject card1;
    public GameObject card2;
    public GameObject card3;
    public GameObject card4;
    public GameObject card5;

    public GameObject hint;
    public Text hintNameText;
    public Text hintEffectText;

    public bool canUse;

    private GameObject client;

    private GameObject card1Comp;
    private GameObject card2Comp;
    private GameObject card3Comp;
    private GameObject card4Comp;
    private GameObject card5Comp;
    //private GameObject card6Comp;

    private bool isMagnifiedC1;
    private bool isMagnifiedC2;
    private bool isMagnifiedC3;
    private bool isMagnifiedC4;
    private bool isMagnifiedC5;
    //private bool isMagnifiedC6;

    private mouseControll card1MouseControll;
    private mouseControll card2MouseControll;
    private mouseControll card3MouseControll;
    private mouseControll card4MouseControll;
    private mouseControll card5MouseControll;

    public List<Card> cardList = new List<Card>();
    private List<int> activeList = new List<int>();


    public AudioSource getCardSound;
    public AudioSource usingCardSound;
    // Start is called before the first frame update
    void Awake()
    {
        activeList.Add(1);
        activeList.Add(1);
        activeList.Add(1);
        activeList.Add(0);
        activeList.Add(0);
        //activeList.Add(0);

        card1MouseControll = card1.GetComponent<mouseControll>();
        card2MouseControll = card2.GetComponent<mouseControll>();
        card3MouseControll = card3.GetComponent<mouseControll>();
        card4MouseControll = card4.GetComponent<mouseControll>();
        card5MouseControll = card5.GetComponent<mouseControll>();

        client = GameObject.Find("Client").transform.Find("Player").gameObject;


        //isMagnifiedC6 = false;
        card1Comp = card1.transform.Find("cardClient").gameObject;
        card2Comp = card2.transform.Find("cardClient").gameObject;
        card3Comp = card3.transform.Find("cardClient").gameObject;
        card4Comp = card4.transform.Find("cardClient").gameObject;
        card5Comp = card5.transform.Find("cardClient").gameObject;
        //card6Comp = card6.transform.Find("cardClient").gameObject;

        card4Comp.SetActive(false);
        card5Comp.SetActive(false);
        //card6Comp.SetActive(false);
        hint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isMagnifiedC1 = card1MouseControll.magnified;
        isMagnifiedC2 = card2MouseControll.magnified;
        isMagnifiedC3 = card3MouseControll.magnified;
        isMagnifiedC4 = card4MouseControll.magnified;
        isMagnifiedC5 = card5MouseControll.magnified;
        string name;
        string function;

        if (!isMagnifiedC1 && !isMagnifiedC2 && !isMagnifiedC3 && !isMagnifiedC4 && !isMagnifiedC5)
        {
            hint.SetActive(false);
        }
        else
        {
            hint.SetActive(true);
            if (isMagnifiedC1)
            {

                name = card1Comp.GetComponent<thisCardClient>().cardName;
                function = card1Comp.GetComponent<thisCardClient>().cardEffect;
                Debug.Log("Card 1 Name: " + name);
                hintNameText.text = "Card Name: " + name;
                hintEffectText.text = "Effect: " + function + "--Right-click to use";
            }
            if (isMagnifiedC2)
            {
                name = card2Comp.GetComponent<thisCardClient>().cardName;
                function = card2Comp.GetComponent<thisCardClient>().cardEffect;
                hintNameText.text = "Card Name: " + name;
                hintEffectText.text = "Effect: " + function + "--Right-click to use";
            }
            if (isMagnifiedC3)
            {
                name = card3Comp.GetComponent<thisCardClient>().cardName;
                function = card3Comp.GetComponent<thisCardClient>().cardEffect;
                hintNameText.text = "Card Name: " + name;
                hintEffectText.text = "Effect: " + function + "--Right-click to use";
            }
            if (isMagnifiedC4)
            {
                name = card4Comp.GetComponent<thisCardClient>().cardName;
                function = card4Comp.GetComponent<thisCardClient>().cardEffect;
                hintNameText.text = "Card Name: " + name;
                hintEffectText.text = "Effect: " + function + "--Right-click to use";
            }
            if (isMagnifiedC5)
            {
                name = card5Comp.GetComponent<thisCardClient>().cardName;
                function = card5Comp.GetComponent<thisCardClient>().cardEffect;
                Debug.Log("Card 5 Name: " + name);
                hintNameText.text = "Card Name: " + name;
                hintEffectText.text = "Effect: " + function + "--Right-click to use";
            }
        }
        //CARD1
        


        if (Input.GetMouseButtonDown(1) && isMagnifiedC1 == true && canUse)
        {

            castCard(card1Comp);
            activeList[0] = 0;
            card1Comp.SetActive(false);
        }


        //CARD2


        if (Input.GetMouseButtonDown(1) && isMagnifiedC2 == true && canUse)
        {

            castCard(card2Comp);
            activeList[1] = 0;

            card2Comp.SetActive(false);

        }


        //CARD3


        if (Input.GetMouseButtonDown(1) && isMagnifiedC3 == true && canUse)
        {

            castCard(card3Comp);
            activeList[2] = 0;


            card3Comp.SetActive(false);

        }

        //CARD4


        if (Input.GetMouseButtonDown(1) && isMagnifiedC4 == true && canUse)
        {

            castCard(card4Comp);
            activeList[3] = 0;


            card4Comp.SetActive(false);

        }

        //CARD5


        if (Input.GetMouseButtonDown(1) && isMagnifiedC5 == true && canUse)
        {

            castCard(card5Comp);
            activeList[4] = 0;


            card5Comp.SetActive(false);

        }

    }


    public void draw()

    {
        int count = 0;
        {
            for (int i = 0; i < 5; i++)
            {
                //Debug.Log(activeList.Count);
                if (activeList[i] == 0 && count < 2)
                {
                    count += 1;
                    if (count == 2)
                    {
                        break;
                    }
                    if (i == 0)
                    {
                        card1Comp.SetActive(true);
                        card1Comp.GetComponent<thisCardClient>().index += 1;

                    }
                    if (i == 1)
                    {
                        card2Comp.SetActive(true);
                        card2Comp.GetComponent<thisCardClient>().index += 1;
                    }
                    if (i == 2)
                    {
                        card3Comp.SetActive(true);
                        card3Comp.GetComponent<thisCardClient>().index += 1;
                    }
                    if (i == 3)
                    {
                        card4Comp.SetActive(true);
                        card4Comp.GetComponent<thisCardClient>().index += 1;
                    }
                    if (i == 4)
                    {
                        card5Comp.SetActive(true);
                        card5Comp.GetComponent<thisCardClient>().index += 1;
                    }
                    //if (i == 5)
                    //{
                    //  card6Comp.SetActive(true);
                    //card6Comp.GetComponent<thisCard>().index += 1;
                    //}
                    activeList[i] = 1;
                    getCardSound.Play();

                }
            }
        }
    }

    public void castCard(GameObject card)
    {
        usingCardSound.Play();
        string name = card.GetComponent<thisCardClient>().cardName;
        if (name == "Slow")
        {
            client.GetComponent<Movement>().Slow();
        }
        if (name == "Confusion")
        {
            client.GetComponent<Movement>().Chaos();
        }
        if (name == "Blind")
        {
            client.GetComponent<Movement>().Blind();
        }
        if (name == "Obstacle")
        {
            client.GetComponent<Movement>().Obstacle();
        }
        if (name == "Erase")
        {
            client.GetComponent<Movement>().Erase();
        }

        if (name == "Teleport")
        {
            client.GetComponent<Movement>().Teleport();
        }
    }

  
}
