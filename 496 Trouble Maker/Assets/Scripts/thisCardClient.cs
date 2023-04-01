using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class thisCardClient : MonoBehaviour
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
        index = 0;
        for (int i = 0; i < 15; i++)
        {
            int randomInt = Random.Range(5, 11);
            this_Card.Add(CardData.cardList[randomInt]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        cardName = this_Card[index].cardName;
        thisSprite = this_Card[index].thisImage;
        thatImage.sprite = thisSprite;
    }
}
