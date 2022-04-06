using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_mobile : MonoBehaviour
{

    private float timeStayTouch = 0;

    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseOver()
    {
        if(this.name == "Exploration_button")
        {
            InputExplorationAnimation(Input.GetMouseButtonDown(0), Input.GetMouseButton(0), Input.GetMouseButtonUp(0));
        }
        else
        {
            InputVoteDoorAnimation(Input.GetMouseButtonDown(0), Input.GetMouseButton(0), Input.GetMouseButtonUp(0));
        }
    }




    public void InputExplorationAnimation(bool down , bool stay, bool up)
    {
        if (down)
        {
            timeStayTouch = 0;
        }
        if (stay)
        {
            timeStayTouch += Time.deltaTime;

            if (timeStayTouch > 0.5f)
            {
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = false;
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(true);
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().Play("animation_zone_circle_tomobile");
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().speed = 6f;
                gameManager.ui_Manager.Display_identificationExpedition(true);
                gameManager.ui_Manager.DisplayKeyAndTorch(false);
                gameManager.ui_Manager.HideDistanceRoom();
            }
            if (timeStayTouch > 1.5f)
            {
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchExpeditionWithAnimation = true;
                timeStayTouch = 0;
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
                gameManager.ui_Manager.Display_identificationExpedition(false);
                gameManager.ui_Manager.DisplayKeyAndTorch(true);
                if (gameManager.SamePositionAtInitialRoom())
                    gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.GetDistance_pathfinding(), null);
            }
        }
        if (up)
        {
            gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
            timeStayTouch = 0;
            //isFirstTouch = false;
            //canMove = true;
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
            gameManager.ui_Manager.Display_identificationExpedition(false);
            gameManager.ui_Manager.DisplayKeyAndTorch(true);
            if (gameManager.SamePositionAtInitialRoom())
                gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.GetDistance_pathfinding(), null);
        }


    }

    public void InputVoteDoorAnimation(bool down, bool stay, bool up)
    {
        if (down)
        {
            timeStayTouch = 0;
        }
        if (stay)
        {
            timeStayTouch += Time.deltaTime;

            if (timeStayTouch > 0.5f)
            {
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = false;
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(true);
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().Play("animation_zone_circle_tomobile");
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().speed = 6f;
                gameManager.ui_Manager.Display_identificationVoteDoor(true);
                gameManager.ui_Manager.DisplayKeyAndTorch(false);
                gameManager.ui_Manager.HideDistanceRoom();
            }
            if (timeStayTouch > 1.5f)
            {
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchVoteDoorMobile = true;
                timeStayTouch = 0;
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
                gameManager.ui_Manager.Display_identificationVoteDoor(false);
                //gameManager.ui_Manager.DisplayKeyAndTorch(true);
/*                if (gameManager.SamePositionAtInitialRoom())
                    gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.GetDistance_pathfinding(), null);*/
            }
        }
        if (up)
        {
            gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
            timeStayTouch = 0;
            //isFirstTouch = false;
            //canMove = true;
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
            gameManager.ui_Manager.Display_identificationVoteDoor(false);
/*            gameManager.ui_Manager.DisplayKeyAndTorch(true);
            if (gameManager.SamePositionAtInitialRoom())
                gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.GetDistance_pathfinding(), null);*/
        }
    }
}
