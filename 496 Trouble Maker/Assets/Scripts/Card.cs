using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card : MonoBehaviour
{
    public int id;
    public string cardName;
    public string cardDescription;

    public Card()
    {

    }

    public Card(int id, string name, string description)
    {
        id = id;
        cardName = name;
        cardDescription = description;
    }
}
