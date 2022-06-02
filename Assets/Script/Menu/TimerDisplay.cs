using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{

    public float timeLeft = 15;
    public bool timerLaunch = false;
    // Start is called before the first frame update
    void Start()
    {
        timerLaunch = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (timerLaunch)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timerLaunch = false;

            }
        }

        this.GetComponent<Text>().text = Mathf.FloorToInt(timeLeft).ToString();
    }
}
