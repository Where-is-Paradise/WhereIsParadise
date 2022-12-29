using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathNpcRoom : MonoBehaviour
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
        if (!GameObject.FindGameObjectWithTag("GodDeath"))
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 4);
        }
    }
}
