using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviourPun
{

    public SwordRoom swordRoom;
    // Start is called before the first frame update
    void Start()
    {
        swordRoom = GameObject.Find("SwordRoom").GetComponent<SwordRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        IsTouchPlayer(collision);
    }

    public void IsTouchPlayer(Collider2D collision)
    {
        if(!this.transform.parent.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (!swordRoom.gameManager.SamePositionAtBossWithIndex(collision.transform.parent.gameObject.GetComponent<PhotonView>().ViewID))
                return;
            if (collision.transform.parent.gameObject.GetComponent<PlayerGO>().isInvincible)
                return;
            collision.transform.parent.gameObject.GetComponent<PlayerGO>().lifeTrialRoom--;
            collision.transform.parent.gameObject.GetComponent<PlayerNetwork>()
                .SendLifeTrialRoom(collision.transform.parent.gameObject.GetComponent<PlayerGO>().lifeTrialRoom);
            if (collision.transform.parent.gameObject.GetComponent<PlayerGO>().lifeTrialRoom == 0)
            {
                SetPlayerColor(collision.transform.parent.gameObject);
                if (TestLastPlayer())
                {
                    GiveAwardToPlayer(GetLastPlayer());
                    SendResetColor();
                    DesactivateSwordRoom();
                }
            }
        }
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            swordRoom.gameManager.RandomWinFireball();
        }
        if (TestLastPlayer())
        {
            GiveAwardToPlayer(GetLastPlayer());
            SendResetColor();
            DesactivateSwordRoom();
        }
    }

    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchByWord(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchBySword || !swordRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice )
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchBySword || !swordRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == listPlayer.Length)
            return true;
        return false;
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        //swordRoom.gameManager.game.nbTorch++;
        swordRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        swordRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        //swordRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
        //swordRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!swordRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchBySword)
                return player;
        }
        return null;
    }

    public void SendResetColor()
    {
        photonView.RPC("ResetColorAllPlayer", RpcTarget.All);
    }

    public void DesactivateSwordRoom()
    {
        this.swordRoom.SendDesactivateRoom();
    }

    [PunRPC]
    public void ResetColorAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (swordRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().isTouchByAx = false;
            player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
        }
    }

}
