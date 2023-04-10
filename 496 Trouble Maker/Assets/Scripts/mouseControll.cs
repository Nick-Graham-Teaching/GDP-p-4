using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseControll : MonoBehaviour
{
    public bool magnified;

    // Start is called before the first frame update
    void Start()
    {
        magnified = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Enter()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        magnified = true;

    }
    public void Exit()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        magnified = false;

    }
}
