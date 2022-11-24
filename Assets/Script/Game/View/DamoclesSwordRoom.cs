using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamoclesSwordRoom : MonoBehaviourPun
{

    public GameManager gameManager;
    public GameObject currentPlayer = null;
    public GameObject sword;
    public bool canChangePlayer = true;
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
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(false);
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            GameObject player = ChoosePlayerRandomly();
            Debug.Log(player.GetComponent<PhotonView>().ViewID);
            SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
            CounterLaunch(10);
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
            if (player.GetComponent<PlayerGO>().isDeadBySwordDamocles)
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
        yield return new WaitForSeconds(0.5f);
        SetPlayerColor(this.currentPlayer);
        GameObject player = ChoosePlayerRandomly();
        SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
        canChangePlayer = true;
        if (TestLastPlayer())
        {
            GiveAwardToPlayer(GetLastPlayer());
            SendResetColor();
            photonView.RPC("DesactivateDamoclesSwordRoom", RpcTarget.All);
            
        }
        else
        {
            StartCoroutine(TimerCouroutine(10f));
        }
    }

    [PunRPC]
    public void KillCurrentPlayer()
    {
        sword.transform.localPosition = new Vector3(0, 0);
        this.currentPlayer.GetComponent<PlayerGO>().isDeadBySwordDamocles = true;
    }

    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchBydDamoclesSword(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isDeadBySwordDamocles || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
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
            if (!player.GetComponent<PlayerGO>().isDeadBySwordDamocles)
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
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().isDeadBySwordDamocles = false;
        }
    }

    [PunRPC]
    public void DesactivateDamoclesSwordRoom()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        this.sword.transform.parent = this.transform;
        this.sword.transform.position = new Vector3(0, 0);
        this.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManager.CloseDoorWhenVote(false);
        gameManager.damoclesIsLaunch = true;
        gameManager.speciallyIsLaunch = false;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        this.gameObject.SetActive(false);
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        gameManager.game.nbTorch++;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }
}
