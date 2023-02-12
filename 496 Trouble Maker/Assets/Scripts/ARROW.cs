using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARROW : MonoBehaviour
{
    // Start is called before the first frame update



    public Transform playerObject;
    public Transform destination;

    void Update()
    {
        // Keep the arrow in front of the player object
        transform.position = playerObject.position + playerObject.forward * 2;
        transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        // Point the arrow towards the destination
        transform.LookAt(destination);
    }
}
