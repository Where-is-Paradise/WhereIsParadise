using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerValue : MonoBehaviour
{

    //public float timer = 0;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = Mathf.FloorToInt(gameManager.timer.timeLeft).ToString();
    }
}
