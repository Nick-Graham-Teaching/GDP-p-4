using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    // Start is called before the first frame update
    void Awake()
    {
        cardList.Add(new Card("AddTime", Resources.Load<Sprite>("AddTime")));
        cardList.Add(new Card("Speed", Resources.Load<Sprite>("Speed")));
        cardList.Add(new Card("RaiseUp", Resources.Load<Sprite>("RaiseUp")));
        cardList.Add(new Card("Invisible", Resources.Load<Sprite>("Invisible")));
        cardList.Add(new Card("Blind", Resources.Load<Sprite>("Blind")));
        cardList.Add(new Card("Confusion", Resources.Load<Sprite>("Confusion")));
        cardList.Add(new Card("Obstacle", Resources.Load<Sprite>("Obstacle")));
        cardList.Add(new Card("Slow", Resources.Load<Sprite>("Slow")));
        cardList.Add(new Card("Erase", Resources.Load<Sprite>("Erase")));
    }

}
