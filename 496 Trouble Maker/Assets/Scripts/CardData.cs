using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    // Start is called before the first frame update
    void Awake()
    {
        cardList.Add(new Card("AddTime", Resources.Load<Sprite>("AddTime"), "Add some time to this term"));
        cardList.Add(new Card("Speed", Resources.Load<Sprite>("Speed"),"Run faster"));
        cardList.Add(new Card("RaiseUp", Resources.Load<Sprite>("RaiseUp"),"Look down the maze from a high position"));
        cardList.Add(new Card("Invisible", Resources.Load<Sprite>("Invisible"), "Make ur self invisible"));
        cardList.Add(new Card("Cleanse", Resources.Load<Sprite>("Cleanse"), "remove the debuff on u"));


        cardList.Add(new Card("Blind", Resources.Load<Sprite>("Blind"), "Make challenger uanble to see"));
        cardList.Add(new Card("Confusion", Resources.Load<Sprite>("Confusion"), "Lead challenger to wrong directions"));
        cardList.Add(new Card("Obstacle", Resources.Load<Sprite>("Obstacle"), "Place it any where in the maze to block the way"));
        cardList.Add(new Card("Slow", Resources.Load<Sprite>("Slow"), "Slow the challenger"));
        cardList.Add(new Card("Erase", Resources.Load<Sprite>("Erase"), "Clean all the sign challenger created"));
        cardList.Add(new Card("Teleport", Resources.Load<Sprite>("Teleport"), "teleport the challenger to a random place"));

    }

}
