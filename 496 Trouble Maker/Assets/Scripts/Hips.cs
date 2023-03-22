using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hips : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = transform.parent.localPosition;
        Debug.Log(transform.parent.position);
        Debug.Log(transform.localPosition);
        Debug.Log(transform.position);
    }
}
