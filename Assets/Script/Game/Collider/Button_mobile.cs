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
        if(this.name == "Map_panel")
        {
            if (gameManager.ui_Manager.map.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

/*    public void OnMouseOver()
    {

    }*/



    public void OnMouseDown()
    {
        if (this.name == "Exploration_button")
        {
            //InputExplorationAnimation(Input.GetMouseButtonDown(0), Input.GetMouseButton(0), Input.GetMouseButtonUp(0));
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchExpeditionWithAnimation = true;
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
               this.gameObject.SetActive(false);
            }
        }
        if (this.name == "Door_panel")
        {
            //InputVoteDoorAnimation(Input.GetMouseButtonDown(0), Input.GetMouseButton(0), Input.GetMouseButtonUp(0));
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchVoteDoorMobile = true;
        }
        if (this.name == "Map_panel")
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().displayMap = true;
        }
        if(this.tag == "SpecialRoom_UI_Mobile")
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
            gameManager.gameManagerNetwork.SendCloseDoorWhenVote();

            gameManager.ui_Manager.DisplaySpeciallyLevers(false, 0);
        }
        if (this.name == "Tuto_panel")
        {
            PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
            playerMine.displayTutorial = true;
        }
        if (this.name == "Change_Boss")
        {
            PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
            playerMine.changeBoss = true;
        }

    }


}
