using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay2 : MonoBehaviour
{
    public float timeLeft = 15;
    public bool timerLaunch = false;
    public bool pause = false;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerLaunch)
        {
            if(!pause)
                timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timerLaunch = false;
                this.transform.parent.gameObject.SetActive(false);
            }
        }

        this.GetComponent<Text>().text = Mathf.FloorToInt(timeLeft).ToString();
    }
}
