using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{

    private Text txtTimer;
    private string n;

    private float lastTimer;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        txtTimer = GetComponent<Text>();
        timer = GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().delayTime;
        lastTimer = GameObject.Find("Host").transform.Find("Player").GetComponent<Movement>().delayTime;
        n = "Challenger Turn: ";
    }

    // Update is called once per frame
    void Update()
    {
        CountTime();
    }

    void CountTime()
    {
        timer -= Time.deltaTime;
        int time = (int)timer;
        string str = n + time.ToString();
        txtTimer.text = str;
        if (timer <= 0)
        {
            if (lastTimer == 30f)
            {
                timer = 10f;
                lastTimer = 10f;
            }
            else if (lastTimer == 10f)
            {
                lastTimer = 30f;
                timer = 30;
            }
            if (n == "Controller Turn: ")
            {
                    txtTimer.color = Color.green;
                    n = "Challenger Turn: ";
            }
            else if (n == "Challenger Turn: ")
            {
                    txtTimer.color = Color.red;
                    n = "Controller Turn: ";
            }
        }
    }

    public void TimeIncreased()
    {
        timer += 5;
    }
}
