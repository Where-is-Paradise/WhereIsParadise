using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsRoom : MonoBehaviourPun
{
    public GameManager gameManagerParent;
    public bool roomIsLaunchParent = false;
    public int indexObject = -1;
    public PlayerGO playerwinner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerParent = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
    }
    
    public void GetAward(int playerWinnerindex)
    {
        int randomInt = Random.Range(0, 6);
        Debug.Log(playerWinnerindex);
        playerwinner = gameManagerParent.GetPlayer(playerWinnerindex).GetComponent<PlayerGO>();
        int indexPlayerMine = playerwinner.GetComponent<PhotonView>().ViewID;
        photonView.RPC("SendRandomInt", RpcTarget.All, randomInt, indexPlayerMine) ;
    }

    [PunRPC]
    public void SendRandomInt(int randomInt, int indexPlayer)
    {
        GetRandomAward(randomInt, indexPlayer);
    }

    public void GetRandomAward(int randomInt, int indexPlayer)
    {
        randomInt = 5;
        int randomtrwo = Random.Range(0, 2);
        if (randomtrwo == 0)
            randomInt = 2;
        else
            randomInt = 3;
        switch (randomInt)
        {
            case 0:
                // bluetorch
                DisplayAwardObject(0);
                indexObject = 0;
                break;
            case 1:
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasMap)
                {
                    if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                        return;
                    Debug.Log("Has map restart");
                    GetAward(indexPlayer);
                    return;
                }
                
                DisplayAwardObject(1);
                indexObject = 1;
                break;
            case 2:
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasProtection)
                {
                    if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                        return;
                    Debug.Log("Has protection restart");
                    GetAward(indexPlayer);
                    return;
                }
                DisplayAwardObject(2);
                indexObject = 2;
                break;
            case 3:
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasTrueEyes)
                {
                    if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                        return;
                    Debug.Log("Has true EYES restart");
                    GetAward(indexPlayer);
                    return;
                }
                DisplayAwardObject(3);
                indexObject = 3;
                break;
            case 4:
                // magical key
                if (gameManagerParent.GetNumberDoorCanBeModifaiteBySpeciallity() == 0)
                {
                    if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                        return;
                    Debug.Log("Magical Key restart");
                    GetAward(indexPlayer);
                    return;
                }
                DisplayAwardObject(4);
                indexObject = 4;
                break;
            case 5:
                // Black torch
                DisplayAwardObject(5);
                indexObject = 5;
                break;
        }
        gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasWinFireBallRoom = true;
        playerwinner = gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
    }

    public void DisplayAwardObject(int index)
    {
        this.transform.parent.Find("AwardObject").gameObject.SetActive(true);
        this.transform.parent.Find("AwardObject").GetChild(index).gameObject.SetActive(true);

    }

    public void DesactivateRoom()
    {
        photonView.RPC("SendDesactivateRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SendDesactivateRoom()
    {
        if (!gameManagerParent.SamePositionAtBoss())
            return;

        ResetHeartForAllPlayer();
        
    }

    public void ReactivateCurrentRoom()
    {
        if (!gameManagerParent.SamePositionAtBoss())
            return;

        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        ResetColorAllPlayer();
        gameManagerParent.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManagerParent.speciallyIsLaunch = false;
        gameManagerParent.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManagerParent.CloseDoorWhenVote(false);
        gameManagerParent.ActivateCollisionTPOfAllDoor(true);
        StartCoroutine(CanMoveActiveCoroutine());
        gameManagerParent.gameManagerNetwork.SendActivateAllObstacles(false, this.gameObject.name);
        roomIsLaunchParent = false;
        if (gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManagerParent.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(true);
        }
    }

    public void ActivateObjectPower(int indexPlayer)
    {
        switch (indexObject)
        {
            case 0:
                if (!gameManagerParent.SamePositionAtBoss())
                    return;
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
                break;
            case 1:
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasMap = true;
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;
            case 2:
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasProtection = true;
                //gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isCursed = false;
                //gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isBlind = false;
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().lifeTrialRoom = 3;
                //
 /*               gameManagerParent.GetPlayer(indexPlayer).transform.Find("Skins")
                    .GetChild(gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin)
                    .Find("Light_Cursed").gameObject.SetActive(false);*/
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.GetPlayer(indexPlayer).transform.Find("TrialObject").Find("AuraProtection").gameObject.SetActive(true);
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(0);
                }
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;
            case 3:
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasTrueEyes = true;
              
                gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().DisplayCursedPlayers();
                // display all information et  changé chaque situation daffichage aussi
                StartCoroutine(CouroutineActivateDoorLever(2));
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.GetPlayer(indexPlayer).transform.Find("TrialObject").Find("TrueEye").gameObject.SetActive(true);
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(1);
                }
                break;
            case 4:
                gameManagerParent.GetPlayer(indexPlayer).transform.Find("TrialObject").Find("MagicalKey").gameObject.SetActive(true); 
                if (!gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                    return;
                gameManagerParent.ui_Manager.DisplayAllDoorLightOther(true);                
                gameManagerParent.ui_Manager.DisplayMagicalKeyButton(); 
                break;
            case 5:
                // a modifier biensur
                if (!gameManagerParent.SamePositionAtBoss())
                    return;
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().hasBlackTorch = true;
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendBlackTorch(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
                
                //StartCoroutine(CouroutineActivateDoorLever(2));
                break;
        }
        //gameManagerParent.GetBoss().GetComponent<PlayerGO>().canLaunchDoorVoteLever = true;
    }

    public void ResetHeartForAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().ResetHeart();
            if (player.GetComponent<PlayerGO>().hasProtection)
                player.GetComponent<PlayerGO>().lifeTrialRoom = 3;
            else
                player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
        }
    }

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
                if (gameManagerParent.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().ResetHeart();
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
        }
    }

    public IEnumerator CanMoveActiveCoroutine()
    {
        yield return new WaitForSeconds(2);
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
    }

    public IEnumerator CouroutineActivateDoorLever(int secondes)
    {
        yield return new WaitForSeconds(secondes);
        gameManagerParent.ui_Manager.DisplayLeverVoteDoor(true);
        ResetHasWinFireBallRoom();
    }

    public void ResetHasWinFireBallRoom()
    {
        playerwinner.GetComponent<PlayerGO>().hasWinFireBallRoom = false;
    }

   
}
