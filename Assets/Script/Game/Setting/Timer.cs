using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public float timeLeft = 10;
    public bool timerLaunch = false;
    public bool timerFinish = false;
    public bool displayTimer = false;

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
            if (timeLeft < 0)
            {
                timerFinish = true;
                timerLaunch = false;
              
            }
            if (displayTimer)
            {
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).GetComponent<Text>().text = timeLeft.ToString();
            }
           
        }
        if (timerFinish)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            
        }
    }

    public void LaunchTimer(float time , bool displayTimer)
    {
        timerLaunch = true;
        timeLeft = time;
        this.displayTimer = displayTimer;
    }
    public void ResetTimer()
    {
        timeLeft = 10;
        timerLaunch = false;
        timerFinish = false;
        displayTimer = false;
    }
}
