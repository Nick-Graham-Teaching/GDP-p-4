using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class thisCard : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Card> this_Card = new List<Card>();
    public string cardName;

    public int index;
    public bool used;
    public Sprite thisSprite;
    public Image thatImage;

    private bool isMagnified = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        //Debug.Log(CardData.cardList.Count);
        //Debug.Log(CardData.cardList[0].cardName);
        index = 0;
        for(int i = 0; i < 15; i++)
        {
            int randomInt = Random.Range(0, 5);
            this_Card.Add(CardData.cardList[randomInt]);
        }
       


    }

    // Update is called once per frame
    void Update()
    {

        cardName = this_Card[index].cardName;
        thisSprite = this_Card[index].thisImage;
        thatImage.sprite = thisSprite;

        // if (Input.GetKeyDown(KeyCode.Alpha6))
        //  {
        //   if (isMagnified)
        //   {
        //        transform.localScale = originalScale;
        //       isMagnified = false;
        //     }
        //   else
        //   {
        //         transform.localScale *= 1.5f; // increase scale by 50%
        //       isMagnified = true;
        //  }
        //  }
        // if (isMagnified && Input.GetKeyDown(KeyCode.Return))
        // {
        //    GameObject myGameObject = GameObject.Find("Card6");
        //    myGameObject.SetActive(false);
        // }

    }
}