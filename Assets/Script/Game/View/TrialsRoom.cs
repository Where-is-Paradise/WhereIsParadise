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
        float randomFloat = Random.Range(0, 100);
        Debug.Log(playerWinnerindex);
        playerwinner = gameManagerParent.GetPlayer(playerWinnerindex).GetComponent<PlayerGO>();
        int indexPlayerMine = playerwinner.GetComponent<PhotonView>().ViewID;
        photonView.RPC("SendRandomInt", RpcTarget.All, randomFloat, indexPlayerMine) ;
    }

    [PunRPC]
    public void SendRandomInt(float randomFloat, int indexPlayer)
    {
        GetRandomAward(randomFloat, indexPlayer);
    }

    public void GetRandomAward(float randomFloat, int indexPlayer)
    {
        //randomInt = 5;
        //int randomtrwo = Random.Range(0, 2);
        /*        if (randomtrwo == 0)
                    randomInt = 2;
                else
                    randomInt = 3;*/
        //randomInt = 4;

        Debug.Log(randomFloat);

        if(randomFloat < 50)
        {
            // bluetorch
            DisplayAwardObject(0);
            indexObject = 0; 
        }
        else if (randomFloat < 70)
        {
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
        }
        else if(randomFloat < 90)
        {
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
        }
        else
        {
            DisplayAwardObject(5);
            indexObject = 3;
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
                gameManagerParent.gameManagerNetwork.SendCloseDoorWhenVote(true);
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(0);
                }
                break;
            case 1:
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasMap = true;
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(1);
                }
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;
            case 2:
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasProtection = true;
                gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().lifeTrialRoom = 3;
                gameManagerParent.GetPlayer(indexPlayer).transform.Find("TrialObject").Find("AuraProtection").gameObject.SetActive(true);
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(2);
                }
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;
            case 3:
                if (!gameManagerParent.SamePositionAtBoss())
                    return;
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().hasBlackTorch = true;
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendBlackTorch(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
                gameManagerParent.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
                gameManagerParent.gameManagerNetwork.SendCloseDoorWhenVote(true);
                if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                {
                    gameManagerParent.ui_Manager.DisplayInformationObjectWon(3);
                }
                break;
        }
    }

    public void ActivateImpostorObject(int indexPlayer)
    {
        if (!gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
            return;

        float randomfloat = Random.Range(0, 100);

        if(randomfloat < 40)
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManagerParent.listIndexImpostorObject[0]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(5);
        }
        else if ( randomfloat < 80)
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManagerParent.listIndexImpostorObject[2]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(6);
        }
        else
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManagerParent.listIndexImpostorObject[1]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(7);
        }
        gameManagerParent.ui_Manager.DisplayObjectPowerImpostorInGame();
        gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasImpostorObject = true;
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
        if(playerwinner)
            playerwinner.GetComponent<PlayerGO>().hasWinFireBallRoom = false;
    }

    public void DisplayGloballyAward(int randomInt)
    {
        gameManagerParent.ui_Manager.DisplayKeyAndTorch(false);
        switch (randomInt)
        {
            case 0:
                DisplayAwardObject(1);
                indexObject = 0;
                break;
            case 1:
                Debug.Log("Magical Key restart");
                DisplayAwardObject(4);
                indexObject = 4;
                break;
        }
    }
    public void ApplyGlobalAward(int indexObject)
    {
        switch (indexObject)
        {
            case 0:
                gameManagerParent.game.key_counter++;
                gameManagerParent.game.nbTorch++;
                gameManagerParent.ui_Manager.LaunchAnimationAddKey();
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;

            case 1:
                gameManagerParent.GetBoss().transform.Find("TrialObject").Find("MagicalKey").gameObject.SetActive(true);
                if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                    return;
               
                gameManagerParent.ui_Manager.DisplayAllDoorLightOther(true);
                gameManagerParent.ui_Manager.DisplayMagicalKeyButton();

                break;
        }
        gameManagerParent.teamHasWinTrialRoom = false;
        gameManagerParent.ui_Manager.DisplayKeyAndTorch(true);
    }

    public void SendObstalceGroup()
    {
        this.transform.Find("Obstacles").gameObject.SetActive(true);
        int childCount = this.transform.Find("Obstacles").childCount;
        int randomGroupObstacle = Random.Range(0, this.transform.Find("Obstacles").childCount);
        photonView.RPC("SetObstalceGroup", RpcTarget.All, randomGroupObstacle);

    }

    [PunRPC]
    public void SetObstalceGroup(int randomGroupObstacle)
    {
        this.transform.Find("Obstacles").gameObject.SetActive(true);
        this.transform.Find("Obstacles").GetChild(randomGroupObstacle).gameObject.SetActive(true);
    }

    public void SendResetObstacle()
    {
        photonView.RPC("ResetObstacleGroup", RpcTarget.All);
    }

    [PunRPC]
    public void ResetObstacleGroup()
    {
        this.transform.Find("Obstacles").gameObject.SetActive(false);
        int childCount = this.transform.Find("Obstacles").childCount;
        for (int i =0; i < childCount; i++)
        {
            this.transform.Find("Obstacles").GetChild(i).gameObject.SetActive(false);
        }
       
    }

}
