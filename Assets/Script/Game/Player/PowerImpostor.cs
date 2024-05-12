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
    public float timerToUsing = 5;
    public bool timerLaunch = false;
    public bool canUsed = false;
    public PlayerGO player;
    public bool powerIsStart = false;

    public Collider2D collision;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetTimerToUsingByIndex(indexPower);
        DisplayButtonDesactivateTimer(true, timerToUsing);
        StartCoroutine(CanUsedTimerCoroutine());
        player = this.transform.parent.GetComponent<PlayerGO>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        indexPower = this.transform.parent.GetComponent<PlayerGO>().indexPower;
        player = this.transform.parent.GetComponent<PlayerGO>();
        /*       indexPower = 1;*/
        if (!powerIsStart)
            return;
        GetAllSituationToCanUsed();
    }

    public IEnumerator CanUsedTimerCoroutine()
    {
        yield return new WaitForSeconds(timerToUsing);
        powerIsStart = true;
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
            SetRedLightColorPlayer(false, collision.transform.parent.GetComponent<Door>().index);
            if (!canUsed)
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
            if (player.isSacrifice)
                return;
            if (indexPower == 3 && (collision.transform.parent.GetComponent<Door>().GetRoomBehind().IsExit || collision.transform.parent.GetComponent<Door>().GetRoomBehind().IsHell))
                return;

            SetRedLightColorPlayer(true, collision.transform.parent.GetComponent<Door>().index);
            isNearOfDoor = true;
            DisplayButtonCanUsed(isNearOfDoor);
            this.collision = collision;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "PowerImpostor" || indexPower == -1)
            return;
        if (collision.transform.parent && collision.transform.parent.tag == "Door")
        {
            SetRedLightColorPlayer(false , collision.transform.parent.GetComponent<Door>().index);
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
        if (room.isTraversed)
            return;
        if (room.IsExit || room.IsHell)
            return;
        if(indexPower == 1 || indexPower == 3 )
            gameManager.ResetSpeciallyRoomState(room);
        this.transform.parent.GetComponent<PlayerNetwork>().SendInsertPowerToDoor(room.Index, indexPower);
        this.transform.parent.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.DisplayLightAllAvailableDoor(false);
        SetRedLightColorPlayer(false, door.index);
        photonView.RPC("SetRedColorDoor", RpcTarget.All, collision.transform.parent.GetComponent<Door>().index, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        DisplayButtonCanUsed(false);
        gameManager.ui_Manager.HidePowerButtonImpostor();
        gameManager.ui_Manager.traped_door.Play();
        this.transform.parent.GetComponent<PlayerGO>().hasOneTrapPower = false;
    }

    public void UsePowerTrapInDoor2()
    {
        if (!isNearOfDoor || indexPower == -1)
            return;
        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(collision.transform.parent.GetComponent<Door>().index).GetComponent<Door>();
        Room room = door.GetRoomBehind();
        if (room.IsObstacle)
            return;
        if (room.IsExit || room.IsHell)
            return;
        photonView.RPC("SetRedColorDoor", RpcTarget.All, collision.transform.parent.GetComponent<Door>().index, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        DisplayButtonCanUsed(false);
    }


    [PunRPC]
    public void SetRedColorDoor(int indexDoor, int indexPlayer)
    {
        if ((!this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes) || indexPower == -1)
            return;
        if (!gameManager || !gameManager.SamePositionAtMine(indexPlayer))
            return;
        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        door.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(door.index, true);
        gameManager.gameManagerNetwork.SendDisplayTrappedDoor(indexDoor);
    }

    public void SetRedLightColorPlayer(bool display, int indexDoor)
    {
        gameManager.GetDoorGo(indexDoor).transform.Find("RedLight").gameObject.SetActive(display);
    }



    public void DisplayButtonCanUsed(bool display)
    {
        if (!canUsed)
        {
            this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(false);
            return;
        }
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(display);
       
    }

    public void DisplayButtonCanUsed2(bool display)
    {
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(display);
    }

    public void GetTimerToUsingByIndex(int index)
    {
        int initalTimerPlus = 0;
        switch (index)
        {
            case 0:
                timerToUsing = 0;
                break;
            case 1:
                timerToUsing = 0;
                break;
            case 2:
                timerToUsing = 0;
                break;
        }
        //timerToUsing += initalTimerPlus;
    }

    public void GetAllSituationToCanUsed()
    {
        if (!gameManager)
            return;
        if (gameManager.speciallyIsLaunch)
        {
            canUsed = false;
            DisplayButtonCanUsed2(true);
            return;
        };
        //canUsed = true;
    }
    bool couroutineIsUsed = false;
    public IEnumerator CouroutineCanUsed(float secondes)
    {
        couroutineIsUsed = true;
        yield return new WaitForSeconds(secondes);
        if (!couroutineIsUsed)
            canUsed = true;
        else
            canUsed = false;
    }

    public void DisplayButtonDesactivateTimer(bool display, float timer)
    {
        if (!gameManager)
            return;
        this.transform.parent.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerButtonDesactivateTime(display, timer);
    }
}
