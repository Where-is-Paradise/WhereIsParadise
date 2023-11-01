using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_mobile : MonoBehaviour
{

    private float timeStayTouch = 0;

    public GameManager gameManager;
    void Start()
    {
        
    }

    void Update()
    {
/*        if(this.name == "Map_panel")
        {
            if (gameManager.ui_Manager.map.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }*/
    }

    public void OnClickExplorationButton()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchExpeditionWithAnimation = true;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            GameObject.Find("Exploration_button").SetActive(false);
        }
    }

    public void OnClickDoorPanel()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchVoteDoorMobile = true;
    }


    public void OnClickMapPanel()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().displayMap = true;
    }

    public void OnClickSpecialityRoomMobile()
    {
        if (gameManager.game.currentRoom.chest)
        {
            gameManager.gameManagerNetwork.SendActiveZoneVoteChest();

        }

        if (gameManager.game.currentRoom.fireBall)
        {
            gameManager.gameManagerNetwork.SendLaunchFireBallRoom();

        }

        if (gameManager.game.currentRoom.isSacrifice)
        {
            gameManager.gameManagerNetwork.SendDisplayNuVoteSacrificeForAllPlayer();
            GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().LaunchTimerVote();
        }
        if (gameManager.game.currentRoom.IsVirus)
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchVoteDoorMobile = true;
        }

        gameManager.gameManagerNetwork.SendCloseDoorWhenVoteCoroutine();

        gameManager.ui_Manager.DisplaySpeciallyLevers(false, 0);
    }

    public void OnClickTutoPanel()
    {
        PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
        playerMine.displayTutorial = true;
    }

    public void OnClickChangeBoss()
    {
        PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
        playerMine.changeBoss = true;
    }

}
