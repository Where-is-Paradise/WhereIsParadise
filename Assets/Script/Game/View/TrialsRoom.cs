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
        //randomFloat = 99;
        if(randomFloat < 50)
        {
            // bluetorch
            if (gameManagerParent.game.currentRoom.HaveOneNeighbour())
            {
                GetAward(indexPlayer);
                return;
            }

            DisplayAwardObject(0);
            indexObject = 0; 
        }
        else if (randomFloat < 70 && gameManagerParent.setting.listObject[0])
        {
            if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasMap)
            {
                if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                    return;
                Debug.Log("Has map restart");
                GetAward(indexPlayer);
                return;
            }

            DisplayAwardObject(0);
            indexObject = 1;
        }
        else if(randomFloat < 90 && gameManagerParent.setting.listObject[1])
        {
            if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasProtection)
            {
                if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                    return;
                Debug.Log("Has protection restart");
                GetAward(indexPlayer);
                return;
            }
            DisplayAwardObject(0);
            indexObject = 2;
        }
        else if(randomFloat < 100 && gameManagerParent.setting.listObject[2])
        {
            DisplayAwardObject(0);
            indexObject = 3;
        }
        else
        {
            if (gameManagerParent.game.currentRoom.HaveOneNeighbour())
            {
                return;
            }
            DisplayAwardObject(0);
            indexObject = 0;
        }
        gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasWinFireBallRoom = true;
        playerwinner = gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        StartCoroutine(ActiveCollisionChestCoroutine());
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
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        ResetColorAllPlayer();
        if(gameManagerParent.GetBoss())
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
        gameManagerParent.ui_Manager.DisplayTrapPowerButtonDesactivateTime(true, 6);
        gameManagerParent.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 6);

        gameManagerParent.DisplayTorchBarre(true);


        gameManagerParent.ui_Manager.DisplayInteractionObject(true);
        gameManagerParent.ui_Manager.HideFightMusic();

        if(indexObject != 0)
            gameManagerParent.PauseTimerFroce(false);

        if (gameManagerParent.game.currentRoom.IsVirus)
        {
            gameManagerParent.ui_Manager.DisplayFloorVirusTransparency();
        }
    }

    public void ActivateObjectPower(int indexPlayer)
    {

        if (!gameManagerParent.GetPlayer(indexPlayer)){
            List<GameObject> listPlayerTree = gameManagerParent.TreePlayerByID();
            GameObject lastPlayer = listPlayerTree[listPlayerTree.Count - 1];
            indexPlayer = lastPlayer.GetComponent<PhotonView>().ViewID;
        }


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
                if(!gameManagerParent.game.currentRoom.isLabyrintheHide)
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
                if (!gameManagerParent.game.currentRoom.isLabyrintheHide)
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

        this.transform.parent.transform.Find("AwardObject").Find("Chest").Find("collisionChest").gameObject.SetActive(true);

    }

    public void ActivateImpostorObject(int indexPlayer)
    {
        if (!gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
            return;
        if (gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasOneTrapPower)
            return;
        if (!gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isImpostor)
            return;

        float randomfloat = Random.Range(0, 100);

        randomfloat = 20;

        if (randomfloat < 25 &&  gameManagerParent.setting.listTrapRoom[0])
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexPower(gameManagerParent.listIndexImpostorPower[0]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(9);
        }
        else if ( randomfloat < 50 && gameManagerParent.setting.listTrapRoom[1])
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexPower(gameManagerParent.listIndexImpostorPower[1]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(10);
        }
        else if (randomfloat < 75 && gameManagerParent.setting.listTrapRoom[2])
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexPower(gameManagerParent.listIndexImpostorPower[2]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(11);

        }
        else if(randomfloat < 100 && gameManagerParent.setting.listTrapRoom[3])
        {
            gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIndexPower(gameManagerParent.listIndexImpostorPower[3]);
            gameManagerParent.ui_Manager.DisplayInformationObjectWon(12);
        }


        gameManagerParent.ui_Manager.DisplayPowerImpostorInGame();
        gameManagerParent.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hasOneTrapPower = true;
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
                    if (player.GetComponent<PlayerGO>().hasProtection)
                        player.transform.Find("TrialObject").Find("AuraProtection").gameObject.SetActive(true);
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
        randomInt = 3;
        switch (randomInt)
        {
            case 0:
                if (gameManagerParent.allPlayerHaveMap)
                {
                    if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                        return;
                    Debug.Log("Has map restart");
                    DisplayGloballyAward(Random.Range(1,3));
                    return;
                }
                DisplayAwardObject(1);
                indexObject = 0;
                break;
            case 1:
                if (gameManagerParent.GetNumberDoorCanBeModifaiteBySpeciallity() == 0)
                {
                    Debug.Log("Has map restart");
                    DisplayGloballyAward(Random.Range(0, 3));
                    return;
                }

                DisplayAwardObject(4);
                indexObject = 1;
                break;
            case 2:
                DisplayAwardObject(6);
                indexObject = 2;
                break;
            case 3:
                /*DisplayAwardObject(7);*/
                this.transform.parent.Find("AwardObject").gameObject.SetActive(true);
                this.transform.parent.transform.Find("AwardObject").Find("ChestTeam").gameObject.SetActive(true);
                StartCoroutine(ActiveCollisionChestTeamCoroutine());
                indexObject = 3;
                break;

        }
        
    }
    public void ApplyGlobalAward(int indexPlayer)
    {
        switch (indexObject)
        {
            case 0:
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<PlayerGO>().hasMap = true;
                }
                gameManagerParent.allPlayerHaveMap = true;
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;

            case 1:
                //gameManagerParent.GetBoss().transform.Find("TrialObject").Find("MagicalKey").gameObject.SetActive(true);
                if (!gameManagerParent.GetBoss().GetComponent<PhotonView>().IsMine)
                    return;
               
                gameManagerParent.ui_Manager.DisplayAllDoorLightOther(true);
                gameManagerParent.ui_Manager.DisplayMagicalKeyButton();
                gameManagerParent.CloseDoorWhenVote(true);
                gameManagerParent.ActivateCollisionTPOfAllDoor(false);
                break;
            case 2:
                gameManagerParent.game.key_counter++;
                gameManagerParent.game.nbTorch++;
                gameManagerParent.ui_Manager.LaunchAnimationAddKey();
                StartCoroutine(CouroutineActivateDoorLever(2));
                break;
            case 3:
                if (!gameManagerParent.GetPlayer(indexPlayer).GetComponent<PhotonView>().IsMine)
                    return;
                gameManagerParent.ui_Manager.panelChooseAwardTeamTrial.SetActive(true);
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


    public void OnClickButtonChooseAwardTeamTriam(int index)
    {
        Debug.Log(this.gameObject.activeSelf);
        if (!this.gameObject.activeSelf)
            return;

        switch (index)
        {
            case 0:
                gameManagerParent.game.key_counter++;
                gameManagerParent.ui_Manager.LaunchAnimationAddKey();
                gameManagerParent.gameManagerNetwork.SendAnimationAddKey();
                gameManagerParent.gameManagerNetwork.SendKey(gameManagerParent.game.key_counter);
                break;
            case 1:
                gameManagerParent.game.nbTorch++;
                gameManagerParent.ui_Manager.LaunchAnimationAddTorch();
                gameManagerParent.gameManagerNetwork.SendAnimationAddTorch();
                gameManagerParent.gameManagerNetwork.SendTorchNumber(gameManagerParent.game.nbTorch);
                gameManagerParent.ui_Manager.SetTorchNumber();
                break;
            case 2:
                gameManagerParent.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayMagicalKey(true);
                gameManagerParent.ui_Manager.DisplayAllDoorLightOther(true);
                gameManagerParent.ui_Manager.DisplayMagicalKeyButton();
                gameManagerParent.CloseDoorWhenVote(true);
                gameManagerParent.ActivateCollisionTPOfAllDoor(false);
                break;
        }
        photonView.RPC("SendReactivateCurrentRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SendReactivateCurrentRoom()
    {
        //ReactivateCurrentRoom();
        StartCoroutine(CouroutineActivateDoorLever(2));
    }
    public IEnumerator ActiveCollisionChestCoroutine()
    {
        yield return new WaitForSeconds(1f);
        this.transform.parent.transform.Find("AwardObject").Find("Chest").Find("collisionChest").gameObject.SetActive(false);
        this.transform.parent.transform.Find("AwardObject").Find("Chest").GetComponent<BoxCollider2D>().enabled = true;
    }

    public IEnumerator ActiveCollisionChestTeamCoroutine()
    {
        yield return new WaitForSeconds(1f);
        this.transform.parent.transform.Find("AwardObject").Find("ChestTeam").Find("collisionChest").gameObject.SetActive(false);
        this.transform.parent.transform.Find("AwardObject").Find("ChestTeam").GetComponent<BoxCollider2D>().enabled = true;
    }

}
