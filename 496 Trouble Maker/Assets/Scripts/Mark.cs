using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        Color color = new Color(r,g,b);
        Debug.Log(transform.GetComponent<MeshRenderer>().material.color);
        transform.GetComponent<MeshRenderer>().material.color = color;
        Debug.Log(transform.GetComponent<MeshRenderer>().material.color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
