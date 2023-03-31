using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        int i = GameObject.FindGameObjectsWithTag("Obstacle").Length;
        name += i.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
