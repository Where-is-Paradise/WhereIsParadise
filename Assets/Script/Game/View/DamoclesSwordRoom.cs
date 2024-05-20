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
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.CloseDoorWhenVote(true);
        gameManagerParent.ui_Manager.DisplayInteractionObject(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendWantToChangeBossFalse();
        gameManager.PauseTimerFroce(true);
        yield return new WaitForSeconds(2);
        gameManager.ui_Manager.LaunchFightMusic();
        gameManager.damoclesIsLaunch = true;
        gameManager.speciallyIsLaunch = true;
        gameManagerParent.DisplayTorchBarre(false);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        speciallyLaunched = true;
        DisplayHeartsFoAllPlayer(true);

        //CounterLaunch(15);
       

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            int randomTimer = Random.Range(8, 17);
            photonView.RPC("SendCouterLaunch", RpcTarget.All, randomTimer);
            SendObstalceGroup();
            GameObject player = ChoosePlayerRandomly();
            SetCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
            SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
            SendChangePositionAtPlayer(player.GetComponent<PhotonView>().ViewID);
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
            sword.GetComponent<DamoclesSword>().SendCanChangePlayer(false);
            StartCoroutine(sword.GetComponent<DamoclesSword>().CanChangePlayerCoroutine());
           
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
            if (player.GetComponent<PlayerGO>().damoclesSwordIsAbove)
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
        photonView.RPC("SetCurrentPlayer", RpcTarget.Others , indexPlayer);
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
        sword.transform.parent = player.transform;
        sword.transform.localPosition = new Vector3(0f, 2.20f);
         
    }

    public void CounterLaunch(int seconde)
    {
        StartCoroutine(TimerCouroutine(seconde));
    }

    [PunRPC]
    public void SendCouterLaunch(int seconde)
    {
        CounterLaunch(seconde);
    }

    public IEnumerator TimerCouroutine(int seconde)
    {
        yield return new WaitForSeconds(seconde);
        StartCoroutine(CouroutineAnimationDeath());
    }

    public IEnumerator CouroutineAnimationDeath()
    {
        canChangePlayer = false;
        yield return new WaitForSeconds(0.4f);

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            if (this.currentPlayer)
            {
                Debug.LogError("sa passe  CouroutineAnimationDeath");
                currentPlayer.GetComponent<PlayerGO>().lifeTrialRoom--;
                currentPlayer.GetComponent<PlayerNetwork>()
                    .SendLifeTrialRoom(currentPlayer.GetComponent<PlayerGO>().lifeTrialRoom);
                photonView.RPC("KillCurrentPlayer", RpcTarget.All, currentPlayer.GetComponent<PlayerGO>().lifeTrialRoom);

                if (currentPlayer.GetComponent<PlayerGO>().lifeTrialRoom == 0)
                {
                    currentPlayer.GetComponent<PlayerGO>().isTouchInTrial = true;
                    SetPlayerColor(this.currentPlayer);
                    GameObject player = ChoosePlayerRandomly();
                    SetCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
                    SendCurrentPlayer(player.GetComponent<PhotonView>().ViewID);
                    photonView.RPC("SendCanChangePlayer", RpcTarget.All, true);

                }
            }

        }

        if (TestLastPlayer())
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            {
                if (gameManager.damoclesIsLaunch)
                {
                    GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
                    DesactivateRoom();
                    photonView.RPC("DesactivateDamoclesSwordRoom", RpcTarget.All);
                }
            }
        }
        else
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            {
                int randomTimer = Random.Range(9, 16);
                photonView.RPC("SendCouterLaunch", RpcTarget.All, randomTimer);
            }
        }
    }

    [PunRPC]
    public void SendCanChangePlayer(bool newCanChangePlayer)
    {
        canChangePlayer = newCanChangePlayer;
    }

    public bool Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            gameManager.RandomWinFireball("DamoclesSwordRoom");
            DesactivateDamoclesSwordRoom();
            return true;
        }
        if (TestLastPlayer())
        {
            DesactivateDamoclesSwordRoom();
            photonView.RPC("DesactivateDamoclesSwordRoom", RpcTarget.All);
            GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
            DesactivateRoom();
            return true;
        }
        return false;
    }



    [PunRPC]
    public void KillCurrentPlayer(int nbLife)
    {
        Debug.LogError("sa  passe KillCurrentPlayer ");
        sword.transform.Find("Animation").GetChild(0).gameObject.SetActive(true);
        sword.transform.localPosition = new Vector3(0, 0);
        gameManager.ui_Manager.damoclesExplosion.Play();
        if(nbLife > 0)
            StartCoroutine(ReturnSwordPositionAfterAtack());
    }

    public IEnumerator ReturnSwordPositionAfterAtack()
    {
        yield return new WaitForSeconds(0.5f);
        sword.transform.position += new Vector3(0f, 1.5f);
        canChangePlayer = true;
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
    public void ResetColorAllPlayerChild()
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
            player.GetComponent<PlayerGO>().damoclesSwordIsAbove = false;
        }
    }


    [PunRPC]
    public void DesactivateDamoclesSwordRoom()
    {
        this.sword.transform.parent = this.transform;
        this.sword.transform.localPosition = new Vector3(-35.55f, 5.23f);
        gameManager.damoclesIsLaunch = false;
        speciallyLaunched = false;
        SendResetObstacle();
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

    public void DisplayHeartsFoAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
                continue;

            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }
}
