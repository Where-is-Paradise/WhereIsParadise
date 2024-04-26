using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMenu : MonoBehaviour
{

    public float timeLeft = 10;
    public bool timerLaunch = false;
    public float initial_timer = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerLaunch)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timerLaunch = false;
                timeLeft = initial_timer;
            }
        }

        this.GetComponent<Text>().text = Mathf.FloorToInt(timeLeft).ToString();
    }
}
