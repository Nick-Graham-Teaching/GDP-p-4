using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{

    private Text txtTimer;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        txtTimer = GetComponent<Text>();
        timer = GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().delayTime;
    }

    // Update is called once per frame
    void Update()
    {
        Timer1();
    }

    void Timer1()
    {
        timer -= Time.deltaTime;
        int time = (int)timer;
        txtTimer.text = string.Format(time.ToString());
        if (timer <= 0)
        {
            timer = GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().delayTime;;
        }
    }

    public void TimeIncreased()
    {
        timer += 5;
    }
}
