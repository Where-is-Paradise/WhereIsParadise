using Luminosity.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerImpostor : MonoBehaviourPun
{
    public int indexPower;
    public bool powerIsUsed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        indexPower = this.transform.parent.GetComponent<PlayerGO>().indexPower;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "PowerImpostor")
            return;
        if(collision.transform.parent &&  collision.transform.parent.tag == "Door")
        {
            SetRedColorPlayer(false);
            if (powerIsUsed)
                return;
            if (!this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                return;
            if (collision.transform.parent.GetComponent<Door>().GetRoomBehind().isTraped)
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
            SetRedColorPlayer(true);
            if (!InputManager.GetButton("Exploration"))
                return;
            photonView.RPC("InsertPowerToDoor", RpcTarget.All, collision.transform.parent.GetComponent<Door>().index);
            this.transform.parent.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.DisplayLightAllAvailableDoor(false);
            SetRedColorPlayer(false);
            photonView.RPC("SetRedColorDoor", RpcTarget.All, collision.transform.parent.GetComponent<Door>().index);

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "PowerImpostor")
            return;
        if (collision.transform.parent && collision.transform.parent.tag == "Door")
            SetRedColorPlayer(false);
    }

    [PunRPC]
    public void InsertPowerToDoor(int  indexDoor)
    {

        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        Room room = door.GetRoomBehind();
        if (room.IsObstacle)
            return;
        if (room.isSpecial)
            return;
        if (room.isDeathNPC || room.isAx || room.isSword)
            return;
        if (room.isSwordDamocles || room.fireBall)
            return;
        if (room.IsExit || room.IsHell)
            return;

        AssignRoomToIndexPower(room, indexPower);

    }

    [PunRPC]
    public void SetRedColorDoor(int indexDoor)
    {
        if (!this.transform.parent.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        Door door = this.transform.parent.GetComponent<PlayerGO>().gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        door.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
    }

    public void SetRedColorPlayer(bool display)
    {

        this.transform.parent.GetComponent<PlayerNetwork>().SendRedColor(display);
    }

    public void AssignRoomToIndexPower(Room room , int indexPower)
    {
        switch (indexPower)
        {
            case 0: room.IsFoggy = true;
                break;
            case 1:
                room.IsVirus= true;
                break;
            case 2:
                room.isJail = true;
                break;
            case 3:
                room.chest = true;
                room.isTraped = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    this.transform.parent.GetComponent<PlayerGO>().gameManager.game.dungeon.InsertChestRoom(room.Index);
                    this.transform.parent.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.SendUpdateNeighbourSpeciality(room.Index, 0);
                }
                
                break;
        }
        room.isTraped = true;
        powerIsUsed = true;
    }


    [PunRPC]
    public void SendChestInformation()
    {

    }

}
