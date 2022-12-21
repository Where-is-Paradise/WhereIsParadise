using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour
{

    public GameManager gameManager;
    public UI_Manager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager)
            return;
        if (!gameManager.game)
            return;
        if (!gameManager.game.currentRoom)
            return;
/*        if (gameManager.GetDoorExpedition(gameManager.GetPlayerMine().GetId()).isJail)
        {
            uiManager.DisplayAutelTutorialSpeciallyRoom(true);
            return;
        }*/
        if (gameManager.speciallyIsLaunch || gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            uiManager.DisplayAutelTutorialSpeciallyRoom(false);
        }
    }
}
