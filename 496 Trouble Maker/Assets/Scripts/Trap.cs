using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int i = GameObject.FindGameObjectsWithTag("Trap").Length;
        name += i.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
