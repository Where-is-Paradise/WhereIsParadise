using Luminosity.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerImpostor : MonoBehaviourPun
{
    public GameManager gameManager;
    public int indexPower;
    public bool powerIsUsed = false;
    public bool isNearOfDoor = false;
    public float timerToUsing;
    public bool canUsed = false;
    

    public Collider2D collision;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        indexPower = this.transform.parent.GetComponent<PlayerGO>().indexPower;
    }
    public IEnumerator CanUsedTimerCoroutine()
    {
        yield return new WaitForSeconds(30);
        DisplayButtonDesactivateTimer(false, 0);
        canUsed = true;
        if (!this.transform.parent.GetComponent<PlayerGO>().gameManager.timer.timerLaunch &&
            !this.transform.parent.GetComponent<PlayerGO>().gameManager.speciallyIsLaunch)
            this.transform.parent.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.DisplayLightAllAvailableDoor(true);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "PowerImpostor" || indexPower == -1)
            return;
        if(collision.transform.parent &&  collision.transform.parent.tag == "Door")
        {
            SetRedColorPlayer(false);
            if (!canUsed)
                return;
            if (powerIsUsed)
                return;
            if (!this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                return;
            if (!this.transform.parent.GetComponent<PhotonView>().IsMine)
                return;
            if (collision.transform.parent.GetComponent<Door>().GetRoomBehind().isTraped)
                return;
            if (collision.transform.parent.GetComponent<Door>().barricade)
                return;
            if (collision.transform.parent.GetComponent<Door>().GetRoomBehind().IsHell || collision.transform.parent.GetComponent<Door>().GetRoomBehind().IsExit)
                return;
            if (!collision.transform.parent.GetComponent<Door>().GetRoomBehind().isHide && 
                   !(collision.transform.parent.GetComponent<Door>().GetRoomBehind().chest && indexPower == 3))
                return;
            if (collision.transform.parent.GetComponent<Door>().GetRoomBehind().isSpecial)
                return;
            if (collision.transform.parent.GetComponent<Door>().isOpenForAll)
                return;
            if (collision.transform.parent.GetComponent<Door>().IsCloseNotPermantly)
                return;
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.timer.timerLaunch)
                return;      
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.speciallyIsLaunch)
                return;
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
                return;
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().haveToGoToExpedition)
                return;
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.voteDoorHasProposed)
                return;
            if (this.transform.parent.GetComponent<PlayerGO>().gameManager.OnePlayerHaveToGoToExpedition())
                return;

            SetRedColorPlayer(true);
            isNearOfDoor = true;
            DisplayButtonCanUsed(isNearOfDoor);
            this.collision = collision;
/*            if (!InputManager.GetButton("Exploration"))
                return;           
            UsePowerTrapInDoor();*/
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "PowerImpostor" || indexPower == -1)
            return;
        if (collision.transform.parent && collision.transform.parent.tag == "Door")
        {
            SetRedColorPlayer(false);
            isNearOfDoor = false;
            DisplayButtonCanUsed(isNearOfDoor);
        }

    }
    public void UsePowerTrapInDoor()
    {
        if (!isNearOfDoor || indexPower == -1)
            return;
        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(collision.transform.parent.GetComponent<Door>().index).GetComponent<Door>();
        Room room = door.GetRoomBehind();
        if (room.IsObstacle)
            return;
        if (room.isDeathNPC || room.isAx || room.isSword)
            return;
        if (room.isSwordDamocles || room.fireBall)
            return;
        if (room.IsExit || room.IsHell)
            return;
        this.transform.parent.GetComponent<PlayerNetwork>().SendInsertPowerToDoor(room.Index, indexPower);
        this.transform.parent.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.DisplayLightAllAvailableDoor(false);
        SetRedColorPlayer(false);
        photonView.RPC("SetRedColorDoor", RpcTarget.All, collision.transform.parent.GetComponent<Door>().index);
        DisplayButtonCanUsed(false);
    }


    [PunRPC]
    public void SetRedColorDoor(int indexDoor)
    {
        if (!this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || indexPower == -1)
            return;
        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        door.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(door.index, true);
    }

    public void SetRedColorPlayer(bool display)
    {

        //this.transform.parent.GetComponent<PlayerNetwork>().SendRedColor(display);
        this.transform.parent.Find("Perso").Find("Light_red").gameObject.SetActive(display);
    }



    public void DisplayButtonCanUsed(bool display)
    {
        if (powerIsUsed)
        {
            this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate();
            this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(false);
            return;
        }
        if (!canUsed)
        {
            this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(false);
            return;
        }
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(display);
    }

    public void GetTimerToUsingByIndex(int index)
    {
        int initalTimerPlus = 13;
        switch (index)
        {
            case 0:
                timerToUsing = 30;
                break;
            case 1:
                timerToUsing = 30;
                break;
            case 2:
                timerToUsing = 30;
                break;
        }
        timerToUsing += initalTimerPlus;
    }

    public void DisplayButtonDesactivateTimer(bool display, float timer)
    {
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerButtonDesactivateTime(display, timer);
    }
}
