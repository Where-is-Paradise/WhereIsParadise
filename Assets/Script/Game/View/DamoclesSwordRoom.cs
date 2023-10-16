using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamoclesSwordRoom : TrialsRoom
{

    public GameManager gameManager;
    public GameObject currentPlayer = null;
    public GameObject sword;
    public bool canChangePlayer = true;
    public bool isAnimation = false;
    public bool speciallyLaunched = false;
    // Start is called before the first frame update
    void Start()
    {
        sword = this.transform.Find("Sword").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchDamoclesSwordRoom()
    {
        StartCoroutine(LaunchDamoclesRoomAfterTeleportation());
    }
    public IEnumerator LaunchDamoclesRoomAfterTeleportation()
    {
        yield return new WaitForSeconds(2);
        gameManager.damoclesIsLaunch = true;
        gameManager.CloseDoorWhenVote(true);
        //gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(false);
        gameManager.speciallyIsLaunch = true;
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        speciallyLaunched = true;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.ui_Manager.SetRandomObstacles(this.gameObject);
            gameManager.gameManagerNetwork.SendActivateAllObstacles(true, this.name);
            GameObject player = ChoosePlayerRandomly();
            SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
            CounterLaunch(15);
        }  
    }

    public GameObject ChoosePlayerRandomly()
    {
        GameObject[] listePlayer = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listPlayerPossible = new List<GameObject>();
        foreach(GameObject player in listePlayer)
        {
            if(player.GetComponent<PlayerGO>().isSacrifice)
            {
                continue;
            }
            if (player.GetComponent<PlayerGO>().isInJail)
            {
                continue;
            }           
            if (player.GetComponent<PlayerGO>().isTouchInTrial)
            {
                continue;
            }
            listPlayerPossible.Add(player);
        }
        int randomInt = Random.Range(0, listPlayerPossible.Count);
        return listPlayerPossible[randomInt];
    }

    public void SendCurrentPlayer(int indexPlayer)
    {
        photonView.RPC("SetCurrentPlayer", RpcTarget.All , indexPlayer);
    }

    [PunRPC]
    public void SetCurrentPlayer(int indexPlayer)
    {
        if (currentPlayer)
            currentPlayer.GetComponent<PlayerGO>().damoclesSwordIsAbove = false;

        this.currentPlayer = gameManager.GetPlayer(indexPlayer);
        currentPlayer.GetComponent<PlayerGO>().damoclesSwordIsAbove = true;
        ChangePositionAtPlayer(indexPlayer);
    }

    public void SendChangePositionAtPlayer(int indexPlayer)
    {
        photonView.RPC("ChangePositionAtPlayer", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void ChangePositionAtPlayer(int indexPlayer)
    {
        GameObject player = gameManager.GetPlayer(indexPlayer);
        sword.transform.position = player.transform.position;
        sword.transform.position += new Vector3(0.1f, 1.3f);
        sword.transform.parent = player.transform;
    }

    public void CounterLaunch(float seconde)
    {
        StartCoroutine(TimerCouroutine(seconde));
    }

    public IEnumerator TimerCouroutine(float seconde)
    {
        yield return new WaitForSeconds(seconde);
        photonView.RPC("KillCurrentPlayer", RpcTarget.All);
        StartCoroutine(CouroutineAnimationDeath());
    }

    public IEnumerator CouroutineAnimationDeath()
    {
        canChangePlayer = false;
        yield return new WaitForSeconds(0.4f);
        if (this.currentPlayer)
        {
            SetPlayerColor(this.currentPlayer);
            GameObject player = ChoosePlayerRandomly();
            SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
            photonView.RPC("SendCanChangePlayer", RpcTarget.All, true);
            canChangePlayer = true;

        } 
        if (TestLastPlayer())
        {
            GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
            DesactivateRoom();
            photonView.RPC("DesactivateDamoclesSwordRoom", RpcTarget.All);
        }
        else
        {
            StartCoroutine(TimerCouroutine(15f));
        }
    }

    [PunRPC]
    public void SendCanChangePlayer(bool newCanChangePlayer)
    {
        canChangePlayer = newCanChangePlayer;
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            gameManager.RandomWinFireball();
        }
        if (TestLastPlayer())
        {
            GiveAwardToPlayer(GetLastPlayer());
            SendResetColor();
            photonView.RPC("DesactivateDamoclesSwordRoom", RpcTarget.All);
        }
    }



    [PunRPC]
    public void KillCurrentPlayer()
    {
        sword.transform.localPosition = new Vector3(0, 0);
        if (!this.currentPlayer)
        {
            //SendCurrentPlayer(gameManager.GetRandomPlayerID());
            return;
        }      
        this.currentPlayer.GetComponent<PlayerGO>().isTouchInTrial = true;
        //canChangePlayer = false;
    }

    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
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
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == listPlayer.Length)
            return true;
        return false;
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchInTrial)
                return player;  
        }
        return null;
    }

    public void SendResetColor()
    {
        photonView.RPC("ResetColorAllPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void ResetColorAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
        }
    }

    [PunRPC]
    public void DesactivateDamoclesSwordRoom()
    {
        this.sword.transform.parent = this.transform;
        this.sword.transform.localPosition = new Vector3(-35.55f, 5.23f);
        gameManager.damoclesIsLaunch = false;
        speciallyLaunched = false;
        //this.gameObject.SetActive(false);
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        //gameManager.game.nbTorch++;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }
}
