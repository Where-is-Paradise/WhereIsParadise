using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{

    public float timeLeft = 15;
    public bool timerLaunch = false;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        //timerLaunch = true;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
                this.transform.parent.gameObject.SetActive(false);
                if (this.transform.parent.parent.parent.name == "Power")
                    gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().canUsed = true;

                if (this.transform.parent.parent.parent.name == "Object")
                    gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().canUsed = true;
            }
            else
            {
                if (this.transform.parent.parent.parent.name == "Power")
                    gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().canUsed = false;

                if (this.transform.parent.parent.parent.name == "Object")
                    gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().canUsed = false;
            }
        }

        this.GetComponent<Text>().text = Mathf.FloorToInt(timeLeft).ToString();
    }
}
