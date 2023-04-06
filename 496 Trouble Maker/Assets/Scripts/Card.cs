using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card : MonoBehaviour
{
    // Start is called before the first frame update
    public string cardName;
    public Sprite thisImage;
    public string effect;

    // Update is called once per frame
    public Card(string name, Sprite image, string function)
    {
        cardName = name;
        thisImage = image;
        effect = function;
    }
}
