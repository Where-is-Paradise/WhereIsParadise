using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallRoom : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLeverToRelauch()
    {
        if (!GameObject.FindGameObjectWithTag("FireBall") && gameManager.fireBallIsLaunch)
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 2);
            gameManager.fireBallIsLaunch = false;
        }
    }
}
