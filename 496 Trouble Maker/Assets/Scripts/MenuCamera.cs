using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour {

    public float speed;
    public Camera C;

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        
        float mouseX = Input.GetAxis ("Mouse X") * speed;
        float mouseY = Input.GetAxis ("Mouse Y") * speed;
        C.transform.localRotation = C.transform.localRotation * Quaternion.Euler ( -mouseY, 0, 0);
        transform.localRotation = transform.localRotation * Quaternion.Euler ( 0, mouseX, 0);

    }
}