using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardDataBase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {
        cardList.Add(new Card(0,"Accelerate","increase ur move speed"));
        cardList.Add(new Card(1, "Hide", "hide ur position from enemies"));
        cardList.Add(new Card(2, "Break", "None"));
        
       
    }
}
