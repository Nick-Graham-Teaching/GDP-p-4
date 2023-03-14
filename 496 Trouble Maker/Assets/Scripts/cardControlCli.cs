using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardControlCli : MonoBehaviour
{

    public GameObject card1;
    public GameObject card2;
    public GameObject card3;
    public GameObject card4;
    public GameObject card5;

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


    public List<Card> cardList = new List<Card>();
    private List<int> activeList = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        activeList.Add(1);
        activeList.Add(1);
        activeList.Add(1);
        activeList.Add(0);
        activeList.Add(0);
        //activeList.Add(0);

        client = GameObject.Find("Client").transform.Find("Player").gameObject;

        isMagnifiedC1 = false;
        isMagnifiedC2 = false;
        isMagnifiedC3 = false;
        isMagnifiedC4 = false;
        isMagnifiedC5 = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        //CARD1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isMagnifiedC1)
            {
                card1Comp.transform.localScale /= 1.5f;
                isMagnifiedC1 = false;
                
            }
            else
            {
                card1Comp.transform.localScale *= 1.5f;
                isMagnifiedC1 = true;
                
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMagnifiedC1 == true && canUse)
        {
            castCard(card1Comp);
            activeList[0] = 0;
            card1Comp.transform.localScale /= 1.5f;
            isMagnifiedC1 = false;
            card1Comp.SetActive(false);
        }


        //CARD2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isMagnifiedC2)
            {
                card2Comp.transform.localScale /= 1.5f;
                isMagnifiedC2 = false;
                //Debug.Log(isMagnifiedC2);
            }
            else
            {
                card2Comp.transform.localScale *= 1.5f;
                isMagnifiedC2 = true;
                //Debug.Log(isMagnifiedC2);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMagnifiedC2 == true && canUse)
        {
            castCard(card2Comp);
            activeList[1] = 0;
            card2Comp.transform.localScale /= 1.5f;
            isMagnifiedC2 = false;
            card2Comp.SetActive(false);
        }


        //CARD3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isMagnifiedC3)
            {
                card3Comp.transform.localScale /= 1.5f;
                isMagnifiedC3 = false;
                //Debug.Log(isMagnifiedC3);
            }
            else
            {
                card3Comp.transform.localScale *= 1.5f;
                isMagnifiedC3 = true;
                //Debug.Log(isMagnifiedC3);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMagnifiedC3 == true && canUse)
        {
            castCard(card3Comp);
            activeList[2] = 0;
            card3Comp.transform.localScale /= 1.5f;
            isMagnifiedC3 = false;
            card3Comp.SetActive(false);
        }

        //CARD4
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (isMagnifiedC4)
            {
                card4Comp.transform.localScale /= 1.5f;
                isMagnifiedC4 = false;
                //Debug.Log(isMagnifiedC4);
            }
            else
            {
                card4Comp.transform.localScale *= 1.5f;
                isMagnifiedC4 = true;
                //Debug.Log(isMagnifiedC4);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMagnifiedC4 == true && canUse)
        {
            castCard(card4Comp);
            activeList[3] = 0;
            card4Comp.transform.localScale /= 1.5f;
            isMagnifiedC4 = false;
            card4Comp.SetActive(false);
        }

        //CARD5
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (isMagnifiedC5)
            {
                card5Comp.transform.localScale /= 1.5f;
                isMagnifiedC5 = false;
                //Debug.Log(isMagnifiedC5);
            }
            else
            {
                card5Comp.transform.localScale *= 1.5f;
                isMagnifiedC5 = true;
                //Debug.Log(isMagnifiedC5);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isMagnifiedC5 == true && canUse)
        {
            castCard(card5Comp);
            activeList[4] = 0;
            card5Comp.transform.localScale /= 1.5f;
            isMagnifiedC5 = false;
            card5Comp.SetActive(false);
        }

    }


    public void draw()

    {
        int count = 0;
        {
            for (int i = 0; i < 5; i++)
            {
                
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

                }
            }
        }
    }

    public void castCard(GameObject card)
    {
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
    }

}
