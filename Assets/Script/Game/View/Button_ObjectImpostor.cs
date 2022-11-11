using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ObjectImpostor : MonoBehaviour
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


    public void OnClickObject()
    {
        gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().UsePower();
        gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().isClickedInButtonPower = true;
    }
    public void OnClickPowerTrap()
    {
        gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().UsePowerTrapInDoor();
    }
}
