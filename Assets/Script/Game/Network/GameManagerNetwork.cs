using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManagerNetwork : MonoBehaviourPun
{

    private GameManager gameManager;

    

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
       
    }




    public void SendSetting(int numberExpedtionMax , bool miniMap , bool obstacleMap ,
        bool keyMap , bool randomRoomKey, bool limitedTorch , int additionalTorch)
    {
        photonView.RPC("SetSetting", RpcTarget.All, numberExpedtionMax, miniMap, obstacleMap,
            keyMap, randomRoomKey, limitedTorch, additionalTorch) ;
    }
    [PunRPC]
    public void SetSetting(int numberExpedtionMax , bool miniMap , bool obstacleMap, bool keyMap,
        bool randomRoomKey, bool limitedTorch, int additionalTorch)
    {
        gameManager.game.setting.NUMBER_EXPEDTION_MAX = numberExpedtionMax;
        gameManager.game.setting.DISPLAY_MINI_MAP = miniMap;
        gameManager.game.setting.DISPLAY_OBSTACLE_MAP = obstacleMap;
        gameManager.game.setting.DISPLAY_KEY_MAP = keyMap;
        gameManager.game.setting.RANDOM_ROOM_ADDKEYS = randomRoomKey;
        gameManager.game.setting.LIMITED_TORCH = limitedTorch;
        gameManager.game.setting.TORCH_ADDITIONAL = additionalTorch;

        gameManager.setting.NUMBER_EXPEDTION_MAX = numberExpedtionMax;
        gameManager.setting.DISPLAY_MINI_MAP = miniMap;
        gameManager.setting.DISPLAY_OBSTACLE_MAP = obstacleMap;
        gameManager.setting.DISPLAY_KEY_MAP = keyMap;
        gameManager.setting.RANDOM_ROOM_ADDKEYS = randomRoomKey;
        gameManager.setting.LIMITED_TORCH = limitedTorch;
        gameManager.setting.TORCH_ADDITIONAL = additionalTorch;

        //gameManager.ui_Manager.SetDescriptionLoadPage("Parameter synchronization..", 0.1f);
    }

    public void SendBoss( int indexNewBoss)
    {
        photonView.RPC("SetBoss", RpcTarget.All ,indexNewBoss);
    }

    [PunRPC]
    public void SetBoss( int indexNewBoss)
    {
        if (gameManager.game.GetBoss())
        {
            if (gameManager.GetPlayer(gameManager.game.GetBoss().GetId()).GetComponent<PhotonView>().IsMine)
                gameManager.ui_Manager.DisplayAllDoorLightExploration(false);
            gameManager.game.GetBoss().SetIsBoss(false);
            gameManager.GetBoss().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
            gameManager.GetBoss().GetComponent<PlayerGO>().isBoss = false; 
        }
       
        gameManager.game.SetBoss(indexNewBoss);
        gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerGO>().isBoss = true;
        gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerNetwork>().SendDisplayCrown(true);
        gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerNetwork>().SendDisplayBlueTorch(false);

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss && !gameManager.isEndGame)
        {
            gameManager.ui_Manager.DisplayPanelBossInformation(true);
        }
        StartCoroutine(gameManager.CanChangeBossCoroutine());
    }

    public void SendIndexBoss(int indexBoss)
    {
        photonView.RPC("SetIndexBoss", RpcTarget.All, indexBoss);
    }

    [PunRPC]
    public void SetIndexBoss(int indexBoss)
    {
        gameManager.indexBoss = indexBoss;
    }


    public void SendRole(int indexPlayer , bool isImpostor , bool isLast)
    {
        photonView.RPC("SetRole", RpcTarget.All, indexPlayer, isImpostor , isLast);
    }

    [PunRPC]
    public void SetRole(int indexPlayer, bool isImpostor, bool isLast)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isImpostor = isImpostor;
        gameManager.game.GetPlayerById(indexPlayer).SetIsImpostor(isImpostor);
        
        //gameManager.ui_Manager.SetDescriptionLoadPage("Roles creation.." , 0.1f);
    }

    public void SendWidthHeightMap(int width , int height)
    {
        photonView.RPC("SetWidhtHeightMap", RpcTarget.All, width, height);
    }

    [PunRPC]
    public void SetWidhtHeightMap(int width, int height)
    {
        gameManager.game.Launch(width, height);
        SendIsReceiveMap();
    }
    public void SendIsReceiveMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        photonView.RPC("SetIsReceiveMap", RpcTarget.All);
    }

    [PunRPC]
    public void SetIsReceiveMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameManager.nbReceiveWidthAndHeightMap++;
            if(gameManager.nbReceiveWidthAndHeightMap == gameManager.listPlayerTab.Length - 1)
            {
                gameManager.MasterClientCreateMap();
            }
        }
    }


    public void SendMap(int indexRoom, bool isExit, bool isObstacle, bool isTooFar, 
        bool isInitial, int distance_exit, int distance_pathFinding,int distance_pathFinding_IR,bool isLast, 
        bool isFoggy , bool isVirus, bool hasKey, bool chest , bool isHide, bool isTrial, bool isSpecial, bool isTeamTrial)
    {
        photonView.RPC("SetRooms", RpcTarget.Others, indexRoom,isExit,isObstacle, isTooFar, isInitial,
            distance_exit,distance_pathFinding, distance_pathFinding_IR, isLast, isFoggy, isVirus, hasKey, chest , isHide, isTrial, isSpecial, isTeamTrial);
    }

    [PunRPC]
    public void SetRooms(int indexRoom, bool isExit, bool isObstacle, bool isTooFar,
        bool isInitial, int distance_exit, int distance_pathFinding,int  distance_pathFinding_IR, 
        bool isLast, bool isFoggy, bool isVirus, bool hasKey, bool chest, bool isHide, bool isTrial, bool isSpecial, bool isTeamTrial)
    {
        gameManager.game.dungeon.rooms[indexRoom].IsExit = isExit;
        gameManager.game.dungeon.rooms[indexRoom].IsObstacle = isObstacle;
        gameManager.game.dungeon.rooms[indexRoom].isTooFar = isTooFar;
        gameManager.game.dungeon.rooms[indexRoom].DistanceExit = distance_exit;
        gameManager.game.dungeon.rooms[indexRoom].DistancePathFinding = distance_pathFinding;
        gameManager.game.dungeon.rooms[indexRoom].IsInitiale = isInitial;
        gameManager.game.dungeon.rooms[indexRoom].distance_pathFinding_initialRoom = distance_pathFinding_IR;

        gameManager.game.dungeon.rooms[indexRoom].IsFoggy = isFoggy;
        gameManager.game.dungeon.rooms[indexRoom].HasKey = hasKey;
        gameManager.game.dungeon.rooms[indexRoom].chest = chest;
        gameManager.game.dungeon.rooms[indexRoom].IsVirus = isVirus;
        gameManager.game.dungeon.rooms[indexRoom].isHide = isHide;
        gameManager.game.dungeon.rooms[indexRoom].isTrial = isTrial;
        gameManager.game.dungeon.rooms[indexRoom].isSpecial = isSpecial;
        gameManager.game.dungeon.rooms[indexRoom].isTeamTrial = isTeamTrial;

        if (isExit)
        {
            gameManager.game.dungeon.SetExit(gameManager.game.dungeon.rooms[indexRoom]);
            
        }
        if (isInitial)
        {
            gameManager.game.dungeon.initialRoom = gameManager.game.dungeon.rooms[indexRoom];
            gameManager.game.currentRoom = gameManager.game.dungeon.initialRoom;
            gameManager.ui_Manager.SetDistanceRoom(gameManager.game.currentRoom.DistancePathFinding, gameManager.game.currentRoom); 
        }

        if (isLast)
        {
            gameManager.GenerateHexagone(-7, 3.5f);
            Hexagone initialHexa = gameManager.GenerateObstacle();
            gameManager.SetPositionHexagone(initialHexa);
            gameManager.SetDoorObstacle(gameManager.game.currentRoom);
            gameManager.game.SetKeyCounter();
            gameManager.distanceInitial = gameManager.game.currentRoom.DistancePathFinding;
            gameManager.SetInitialPositionPlayers();
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().SetRoomCursed();
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().
               SendDistanceCursed(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().distanceCursed,
               gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().roomUsedWhenCursed.Index);
        }
        
        //gameManager.ui_Manager.SetDescriptionLoadPage("Création des Salles " + indexRoom + ".." , 0.1f + (indexRoom / 100));

    }

    public void SendExpedition(int playerIndex, int roomIndex)
    {
        photonView.RPC("CreateExpedition", RpcTarget.All, playerIndex,roomIndex);
    }


    [PunRPC]
    public void CreateExpedition(int playerIndex, int roomIndex)
    {
        gameManager.game.CreateExpedition(playerIndex, roomIndex);
        if (gameManager.SamePositionAtBoss())
            gameManager.DesactivateColliderDoorToExplorater(roomIndex, playerIndex);
    }

    public void SendExpeditionHadPropose(bool hadPropose)
    {
        photonView.RPC("SetExpeditionHadPropose", RpcTarget.All, hadPropose);
    }

    [PunRPC]
    public void SetExpeditionHadPropose(bool hadPropose)
    {
        gameManager.expeditionHasproposed = hadPropose;
    }

    public void LaunchTimerExpedition()
    {
        photonView.RPC("SetlaunchTimerExpedition", RpcTarget.All);
    }

    [PunRPC]
    public void SetlaunchTimerExpedition()
    {
        gameManager.timer.LaunchTimer(5, false);
        StartCoroutine(gameManager.LaunchExploration());
        if (gameManager.SamePositionAtBoss())
        {
            gameManager.ui_Manager.DisplayZoneVote();
            gameManager.ui_Manager.DisplayAllGost(true);
        }

        gameManager.expeditionHasproposed = true;
        gameManager.ui_Manager.HideDistanceRoom();
        gameManager.ui_Manager.DisplayKeyAndTorch(false);
        StartCoroutine(gameManager.CloseDoorWhenVoteCoroutine(true));
        

        if (gameManager.ui_Manager.map.activeSelf)
        {
            gameManager.ui_Manager.DisplayMap();
           
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);


    }

    public void SendDisplayAllGost(bool display)
    {
        photonView.RPC("SetDisplayAllGost", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayAllGost(bool display)
    {
        gameManager.ui_Manager.DisplayAllGost(display);
    }

    public void SendVoteYesToExploration(bool isAwardTrial)
    {
        photonView.RPC("SetVoteYesToExploration", RpcTarget.All , isAwardTrial);
    }

    [PunRPC]
    public void SetVoteYesToExploration(bool isAwardTrial)
    {
        if(!isAwardTrial)
            gameManager.SetNbTorch(gameManager.game.current_expedition.Count);   
        if (gameManager.SamePositionAtBoss())
            gameManager.OpenDoorsToExpedition();
        gameManager.alreaydyExpeditionHadPropose = true;
        gameManager.SetPlayersHaveTogoToExpeditionBool();
        gameManager.ResetVoteExploration();
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.explorationIsUsed = true;
        gameManager.waitForEndVote = false;
    }

    public void SendVoteNoToExploration()
    {
        photonView.RPC("SetVoteNoToExploration", RpcTarget.All);
    }

    
    public void SendExplorationIsUsed(int indexRoom, bool isUsed)
    {
        photonView.RPC("SetExplorationIsUsed", RpcTarget.All, indexRoom, isUsed);
    }

    [PunRPC]
    public void SetExplorationIsUsed(int indexRoom, bool isUsed)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).explorationIsUsed = isUsed;
        gameManager.CloseDoorWhenVote(false);
    }

    [PunRPC]
    public void SetVoteNoToExploration()
    {
        gameManager.ui_Manager.DisplayAllGost(false);
        gameManager.ChangeBoss();
        gameManager.ClearExpedition();
        gameManager.ClearDoor();
        gameManager.expeditionHasproposed = false;
        gameManager.ui_Manager.DisplayMainLevers(true);
        gameManager.ResetVoteExploration();
        gameManager.waitForEndVote = false;
    }

    public void SendVoteCP(int playerIndex, int vote)
    {
        
        photonView.RPC("SetVoteCP", RpcTarget.All, playerIndex, vote);
    }

    [PunRPC]
    public void SetVoteCP(int playerIndex, int vote)
    {
        PlayerDun player = gameManager.game.GetPlayerById(playerIndex);
        player.SetVote_CP(vote);
        player.SetHasVoted_CP(true);   
    }

    public void SendIsInExpedition(int playerId , bool isInExpedition)
    {
        photonView.RPC("SetIsInExpedition", RpcTarget.Others, playerId, isInExpedition);
    }

    [PunRPC]
    public void SetIsInExpedition(int playerId, bool isInExpedition)
    {
        gameManager.game.GetPlayerById(playerId).SetIsInExpedition(isInExpedition);
    }


    public void SendvoteDoor(int indexPlayer, int indexDoor)
    {
        photonView.RPC("SetVoteDoor", RpcTarget.All, indexPlayer, indexDoor);
    }

    [PunRPC]
    public void SetVoteDoor(int indexPlayer, int indexDoor)
    {
        gameManager.game.GetPlayerById(indexPlayer).SetVote_VD(indexDoor);
        gameManager.game.GetPlayerById(indexPlayer).SetHasVoted_VD(true);

        gameManager.ui_Manager.SetTextNbVote(indexDoor);

    }



    public void SendKey( int nbKey)
    {
        photonView.RPC("Setkey", RpcTarget.All, nbKey);
    }

    [PunRPC]
    public void Setkey(int nbKey)
    {
        gameManager.game.key_counter = nbKey;
        gameManager.ui_Manager.SetNBKey();
        //gameManager.ui_Manager.SetDescriptionLoadPage("Objects Initialization..", 0.1f);
    }


    public void SendAnimationAddTorch()
    {
        photonView.RPC("SetAnimationAddTorch", RpcTarget.Others);
    }

    [PunRPC]
    public void SetAnimationAddTorch()
    {
        gameManager.ui_Manager.LaunchAnimationAddTorch();
        gameManager.ui_Manager.SetTorchNumber();
    }

    public void SendAnimationAddKey()
    {
        photonView.RPC("SetAnimationAddKey", RpcTarget.Others);
    }

    [PunRPC]
    public void SetAnimationAddKey()
    {
        gameManager.ui_Manager.LaunchAnimationAddKey();
        gameManager.ui_Manager.SetNBKey();
    }

    public void SendAnimationBrokenKey()
    {
        photonView.RPC("SetAnimationBrokenKey", RpcTarget.Others);
    }

    [PunRPC]
    public void SetAnimationBrokenKey()
    {
        gameManager.ui_Manager.LaunchAnimationBrokenKey();
        gameManager.ui_Manager.SetNBKey();
    }

    public void SendAnimationBrokenTorch()
    {
        photonView.RPC("SetAnimationBrokenTorch", RpcTarget.Others);
    }

    [PunRPC]
    public void SetAnimationBrokenTorch()
    {
        gameManager.ui_Manager.LaunchAnimationBrokenTorch();
        gameManager.ui_Manager.SetTorchNumber();
    }

    public void SendBackToExpe(int indexPlayer)
    {
        photonView.RPC("BackToExpe", RpcTarget.Others, indexPlayer);
    }

    [PunRPC]
    public void BackToExpe(int indexPlayer)
    {
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (player )
        {

            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
        }

    }

    public void SendDoorToClose(int indexDoor)
    {
        photonView.RPC("SetDoorToClose", RpcTarget.Others, indexDoor);
    }

    [PunRPC]
    public void SetDoorToClose(int indexDoor)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition)
        {
            GameObject door = gameManager.GetDoorGo(indexDoor);
            gameManager.ui_Manager.DisplayGostPlayer(indexDoor, false,0);
            door.GetComponent<Door>().old_player = null;
            door.GetComponent<Door>().player = null;
            door.GetComponent<Door>().isOpen = false;
            //door.GetComponent<Animator>().SetBool("open", false);
            door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
            door.GetComponent<Door>().counterPlayerInDoorZone = 0;
            door.GetComponent<Door>().letter_displayed = false;
        }
    }

    public IEnumerator SendActiveZoneDoor()
    {
        yield return new WaitForSeconds(0.05f);
        gameManager.ui_Manager.ResetExplorationGost();
        photonView.RPC("ActiveZoneDoor", RpcTarget.All);
    }

    [PunRPC]
    public void ActiveZoneDoor()
    {
       
        gameManager.ui_Manager.HideDistanceRoom();
        gameManager.ui_Manager.DisplayKeyAndTorch(false);
        DisplayLightAllAvailableDoorN2(false);
        gameManager.canVoteDoor = true;
        gameManager.timer.LaunchTimer(5, false);
        StartCoroutine(ResultAllDoorVoteSinceBoss());
        StartCoroutine(gameManager.LauchVoteDoorCoroutine());
        gameManager.voteDoorHasProposed = true;
        gameManager.CloseDoorWhenVote(true);
        if (gameManager.ui_Manager.map.activeSelf || gameManager.ui_Manager.mapLostSoul.activeSelf)
        {
            gameManager.ui_Manager.map.SetActive(false);
            gameManager.ui_Manager.mapLostSoul.SetActive(false);
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayBlueTorch(false);
        StartCoroutine(CoroutineActiveZoneDoor());


    }

    public IEnumerator ResultAllDoorVoteSinceBoss()
    {
        yield return new WaitForSeconds(5f);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject door in listDoor)
            {
                photonView.RPC("SendResultOneDoorVote", RpcTarget.All, door.GetComponent<Door>().index, door.GetComponent<Door>().nbVote, gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote);
            }
        }
    }
    [PunRPC]
    public void SendResultOneDoorVote(int indexDoor, int resultVote, int xVote)
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.GetDoorGo(indexDoor).GetComponent<Door>().nbVote = resultVote;
        gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = xVote;
        gameManager.canVoteDoor = false;
    }

    public IEnumerator CoroutineActiveZoneDoor()
    {
        yield return new WaitForSeconds(0.15f);
        if (!gameManager.game.currentRoom.IsVirus)
            gameManager.ui_Manager.ActiveZoneDoor(gameManager.SamePositionAtBoss());
        else
            gameManager.ui_Manager.ActiveXzone(true);
    }



    public void SendCloseDoorWhenVoteCoroutine()
    {
        photonView.RPC("SetCloseDoorWhenVoteCouroutine", RpcTarget.All);
    }

    [PunRPC]
    public void SetCloseDoorWhenVoteCouroutine()
    {
        StartCoroutine(gameManager.CloseDoorWhenVoteCoroutine(true));
        
    }

    public void SendCloseDoorWhenVote(bool close)
    {
        photonView.RPC("SetCloseDoorWhenVote", RpcTarget.All, close);
    }

    [PunRPC]
    public void SetCloseDoorWhenVote(bool close)
    {
        gameManager.CloseDoorWhenVote(close);
    }


    public void SendHidePlayerTakeDoor()
    {
        photonView.RPC("HidePlayerTakeDoor", RpcTarget.All);
    }

    [PunRPC]
    public void HidePlayerTakeDoor()
    {
        gameManager.HidePlayerNotInSameRoom();
    }

    public void SendPositionPlayer(int indexPlayer, int x, int y)
    {
        photonView.RPC("PositionPlayer", RpcTarget.Others, indexPlayer, x, y);
    }
    [PunRPC]
    public void PositionPlayer(int indexPlayer, int x, int y)
    {
        PlayerGO player = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();

        if (player.position_X == playerMine.position_X && player.position_Y == playerMine.position_Y)
        {

            player.position_X = x;
            player.position_Y = y;

            // SendHidePlayerTakeDoor();
            gameManager.HidePlayerNotInSameRoom();

        }
        else
        {
            player.position_X = x;
            player.position_Y = y;
            StartCoroutine(PauseWhenPlayerTakeDoor(0.3f));
        }
      
    }

    /*
     * set the reel position, this method permit to skip the bug when player can instant appear in opposite door 
    */
    public void SendPositionReel(int indexPlayer, float x, float y)
    {
        photonView.RPC("SetPositionReel", RpcTarget.Others, indexPlayer, x, y);
    }

    [PunRPC]
    public void SetPositionReel(int indexPlayer, float x, float y)
    {
        GameObject player = gameManager.GetPlayer(indexPlayer);
        player.transform.position = new Vector2(x, y);
    }


    public void SendHavetoGoToExpedition(bool haveToGo, int indexPlayer)
    {
        photonView.RPC("SetHaveToGoToExpedition", RpcTarget.Others, haveToGo, indexPlayer);
    }

    [PunRPC]
    public void SetHaveToGoToExpedition(bool haveToGo, int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().haveToGoToExpedition = haveToGo;
    }

    public void SendRandomIndexDoor(int indexDoor)
    {
        photonView.RPC("SetRandomIndexDoor", RpcTarget.All, indexDoor);
    }

    [PunRPC]
    public void SetRandomIndexDoor(int indexDoor)
    {
        GameObject door = gameManager.GetDoorGo(indexDoor);
        gameManager.OpenDoor(door, false);
        gameManager.expeditionHasproposed = false;
        gameManager.game.key_counter--;
        gameManager.nbKeyBroken++;
        gameManager.ui_Manager.SetNBKey();
        gameManager.alreaydyExpeditionHadPropose = false;
        if (gameManager.game.currentRoom.IsVirus)
        {
            gameManager.ui_Manager.ResetLetterDoor();
        }

    }


    public void SendHell(int indexHell)
    {
        photonView.RPC("SetHell", RpcTarget.All, indexHell);
    }

    [PunRPC]
    public void SetHell(int indexHell)
    {
        gameManager.hell = gameManager.game.dungeon.GetRoomByIndex(indexHell);
        gameManager.hell.IsHell = true;
        gameManager.SetCurrentRoomColor();
    }

    public void SendMixDoor(int indexDoor, int newIndex)
    {
        photonView.RPC("SetDoorMix", RpcTarget.Others, indexDoor, newIndex);
    }

    [PunRPC]
    public void SetDoorMix(int indexDoor, int newIndex)
    {
        gameManager.GetDoorGo(indexDoor).GetComponent<Door>().index = newIndex;
    }

    public void AddKey(int indexRoom)
    {
        photonView.RPC("SetAddKey", RpcTarget.All, indexRoom);
    }

    [PunRPC]
    public void SetAddKey(int indexRoom)
    {
        StartCoroutine(CoroutineAddKey(indexRoom));
    }


    public IEnumerator CoroutineAddKey(int indexRoom)
    {
        yield return new WaitForSeconds(0.3f);
        if (gameManager.game.dungeon.GetRoomByIndex(indexRoom).availableKey)
        {
            gameManager.game.key_counter++;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).availableKey = false;
        }
    }

    public IEnumerator PauseWhenPlayerTakeDoor( float secondePause)
    {
        yield return new WaitForSeconds(secondePause);
        gameManager.HidePlayerNotInSameRoom();
    }



    public void  SendLoadingFinish()
    {
        photonView.RPC("SetLoadingFinish", RpcTarget.All);
    }

    [PunRPC]
    public void SetLoadingFinish()
    {
        gameManager.nbPlayerFinishLoading++;
    }

    public void SendComeToParadise(int indexPlayer)
    {
        photonView.RPC("SetComeToParadise", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetComeToParadise(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).transform.GetChild(0).gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.GetChild(1).gameObject.SetActive(false);

        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().comeToParadise = true;

        if (gameManager.AllPlayerGoneToParadise())
        {
            gameManager.ui_Manager.DisplayBlackScreenToDemonWhenAllGone();
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise = true;
        }
        
    }

    public void SendComeToHell()
    {
        photonView.RPC("SetComeToHell", RpcTarget.Others);
    }

    [PunRPC]
    public void SetComeToHell()
    {
        if (gameManager.AllPlayerGoneToHell() && (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isSacrifice))
        {
            Debug.Log(" sa passe frero ");
            gameManager.ui_Manager.DisplayBlackScreenToDemonWhenAllGone();
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell = true;
        }
    }




    public void SendCollisionZoneVoteDoor(int indexPlayer, int indexDoor, bool enter, bool stay)
    {
        photonView.RPC("SetCollisionZoneVoteDoor", RpcTarget.All, indexPlayer, indexDoor, enter, stay);
    }
    [PunRPC]
    public void SetCollisionZoneVoteDoor(int indexPlayer, int indexDoor, bool enter, bool stay)
    {

        GameObject door = gameManager.GetDoorGo(indexDoor);
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (!gameManager.SamePositionAtBoss())
        {
            return;
        }
        if (stay)
        {
            if(gameManager.voteDoorHasProposed)
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(true);
            return;
        }
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            if (enter)
            {
                door.GetComponent<Door>().nbVote++;
            }
            else
            {
                if (gameManager.voteDoorHasProposed)
                {
                    door.GetComponent<Door>().nbVote--;
                    if (door.GetComponent<Door>().nbVote < 0)
                    {
                        door.GetComponent<Door>().nbVote = 0;
                    }
                }
            }
        }
        player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(enter);
        player.GetComponent<PlayerGO>().hasVoteVD = enter;

    }


    public void SendCollisionExpeditionLetter(int indexPlayer, int indexDoor , bool enter, int newCounter)
    {
        photonView.RPC("SetCollitionExpeditionLetter", RpcTarget.All, indexPlayer, indexDoor, enter, newCounter);
    }

    [PunRPC]
    public void SetCollitionExpeditionLetter(int indexPlayer, int indexDoor, bool enter, int newCounter)
    {
        GameObject door = gameManager.GetDoorGo(indexDoor);
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (enter ) 
        {

            door.GetComponent<Door>().counterPlayerInDoorZone++;
            door.GetComponent<Door>().player = player;
        }
        else
        {
            door.GetComponent<Door>().counterPlayerInDoorZone--;
            player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().enabled = false;
            door.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().gameObject.SetActive(true);

            if(!gameManager.expeditionHasproposed)
                door.GetComponent<Door>().player = null;
            door.GetComponent<Door>().letter_displayed = false;
            door.transform.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.25f);
        }
    }


    public void SendCollisionExpeditionLetterStay(int indexPlayer, int indexDoor)
    {
        photonView.RPC("SetCollitionExpeditionLetterStay", RpcTarget.All, indexPlayer, indexDoor);
    }

    [PunRPC]
    public void SetCollitionExpeditionLetterStay(int indexPlayer, int indexDoor)
    {
        GameObject door = gameManager.GetDoorGo(indexDoor);
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (!gameManager.SamePositionAtBoss())
        {
            return;
        }
        if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
        {
            if (player.GetComponent<PlayerGO>().position_X == gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().position_X)
            {
                if (player.GetComponent<PlayerGO>().position_Y == gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().position_Y)
                {

                    door.GetComponent<Door>().player = player.gameObject;
                    player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().enabled = true;
                    player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = door.GetComponent<Door>().name;
                    door.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
                    door.transform.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
                    door.GetComponent<Door>().letter_displayed = true;

                }
            }
            
        }
       
    }

    public void SendCollisionZoneVoteDoorX(int indexPlayer, bool enter, bool stay)
    {
        photonView.RPC("SetCollisionZoneVoteDoorX", RpcTarget.All, indexPlayer, enter, stay);
    }
    [PunRPC]
    public void SetCollisionZoneVoteDoorX(int indexPlayer, bool enter, bool stay)
    {
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (stay)
        {
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(true);
            player.transform.Find("ActivityCanvas").Find("X_vote").gameObject.SetActive(true);
            return;
        }
        else
        {

            if (enter)
            {
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                {
                    gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote++;
                }
               
            }
            else
            {
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                {
                    if (gameManager.voteDoorHasProposed)
                    {
                        gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote--;
                        if (gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote < 0)
                            gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = 0;
                    }
                }                   
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(false);
                player.transform.Find("ActivityCanvas").Find("X_vote").gameObject.SetActive(false);

            }
        }
    }

    public void SendCollisionChestVote(int indexPlayer, int indexChest, bool enter, bool stay)
    {
        photonView.RPC("SetCollisionChestVote", RpcTarget.All, indexPlayer, indexChest, enter, stay);
    }
    [PunRPC]
    public void SetCollisionChestVote(int indexPlayer,int indexChest,  bool enter, bool stay)
    {
        if (!gameManager.voteChestHasProposed)
        {
            return;
        }
        if (!gameManager.SamePositionAtBoss())
        {
            return;
        }
        GameObject chest = null;
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Chest").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Chest")[i].transform.Find("VoteZone").GetComponent<ChestZoneVote>().indexChest == indexChest)
            {
                chest = GameObject.FindGameObjectsWithTag("Chest")[i];
            }
        }

        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (stay)
        {
           
            if (indexChest == 1)
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_redDark").gameObject.SetActive(true);
            else
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_blue").gameObject.SetActive(true);
        }
        else
        {
            if (enter)
            {
                
                chest.transform.Find("VoteZone").GetComponent<ChestZoneVote>().nbVote++;
                
                if (indexChest == 1)
                    player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_redDark").gameObject.SetActive(true);
                else
                    player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_blue").gameObject.SetActive(true);
            }
            else
            {
                chest.transform.Find("VoteZone").GetComponent<ChestZoneVote>().nbVote--;
                
                if (indexChest == 1)
                    player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_redDark").gameObject.SetActive(false);
                else
                    player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_blue").gameObject.SetActive(false);

            }
        }
    }




    public void SendTorchNumber(int number)
    {
        photonView.RPC("SetTorchNumber", RpcTarget.All, number);
    }

    [PunRPC]
    public void SetTorchNumber(int number)
    {
        gameManager.game.nbTorch = number;
        gameManager.ui_Manager.SetTorchNumber();
    }

    public void SendCollisionHeadParadise(int indexPlayer , bool enter)
    {
       photonView.RPC("SetCollisionHeadParadise", RpcTarget.All, indexPlayer, enter);
    }

    [PunRPC]
    public void SetCollisionHeadParadise(int indexPlayer , bool enter)
    {
        GameObject player = gameManager.GetPlayer(indexPlayer);
        if (enter)
        {
            //player.transform.GetChild(1).GetChild(7).gameObject.SetActive(true);
            gameManager.headParadise.nbPlayer++;
        }
        else
        {
            //player.transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
            gameManager.headParadise.nbPlayer--;
        }
        
    }


    public void SendOpenDoor(int indexDoor, int x_room, int y_room, bool isExpedition, int indexRoomTeam , int indexRoomBehind)
    {
        photonView.RPC("SetOpenDoor", RpcTarget.All, indexDoor, x_room, y_room, isExpedition, indexRoomTeam, indexRoomBehind);
    }

    [PunRPC]
    public void SetOpenDoor(int indexDoor, int x_room, int y_room, bool isExpedition, int indexRoomTeam , int indexRoomBehind)
    {
        if (!isExpedition)
        {
            // c la pour update le door_isOpen[indexDoor] pour la porte inverse de la prochaine salle
            Room roomInPlayer = gameManager.game.dungeon.GetRoomByPosition(x_room, y_room);
            roomInPlayer.door_isOpen[indexDoor] = true;
            int indexNeWDoor3 = gameManager.GetIndexDoorAfterCrosse(indexDoor);
            gameManager.game.dungeon.GetRoomByIndex(indexRoomBehind).door_isOpen[indexNeWDoor3] = true;
            Room roomTeam2 = gameManager.game.dungeon.GetRoomByIndex(indexRoomTeam);

            gameManager.InsertSpeciallyRoom(roomTeam2);
            if (roomTeam2.isJail)
            {
                roomTeam2.speciallyPowerIsUsed = true;
            }
     
        }

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
        {
           

            Room roomTeam = gameManager.game.dungeon.GetRoomByIndex(indexRoomTeam);
            if (roomTeam.X == gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().position_X && roomTeam.Y == gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().position_Y)
            {
                int indexDoorInJail = gameManager.GetIndexDoorAfterCrosse(indexDoor);
                gameManager.GetDoorGo(indexDoorInJail).transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                gameManager.GetDoorGo(indexDoorInJail).GetComponent<Door>().isOpenForAll = true;
                gameManager.game.currentRoom.door_isOpen[gameManager.GetDoorGo(indexDoorInJail).GetComponent<Door>().index] = true;
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail = false;
                SendIsInJail(false, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID , roomTeam.Index);
                gameManager.game.dungeon.GetRoomByIndex(roomTeam.Index).speciallyPowerIsUsed = true;
                gameManager.UpdateSpecialsRooms(roomTeam);
                return;
            }

            if ( (gameManager.nbKeyBroken - gameManager.nbKeyWhenJail) >= 2)
            {
                Door doorInJail = gameManager.GetDoorGo(gameManager.indexDoorExplorationInJail).GetComponent<Door>();
                //gameManager.game.currentRoom.door_isOpen[doorInJail.index] = true;
                doorInJail.gameObject.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                gameManager.game.dungeon.GetRoomByIndex(gameManager.game.currentRoom.Index).speciallyPowerIsUsed = true;
                doorInJail.isOpenForAll = true;
                gameManager.UpdateSpecialsRooms(gameManager.game.currentRoom);
            }       
        }
       

    }
    public void SendIsInJail(bool isInJail, int indexPlayer , int indexRoom)
    {
        photonView.RPC("SetIsInJail", RpcTarget.All, isInJail, indexPlayer, indexRoom);
    }

    [PunRPC]
    public void SetIsInJail(bool isInJail, int indexPlayer , int indexRoom)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isInJail = isInJail;
        if (!isInJail)
        {
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).speciallyPowerIsUsed = !isInJail;
            gameManager.GetPlayerMineGO().transform.Find("collisionTriger").gameObject.SetActive(true);
        }
          
    }

    public void SendKeyNumber()
    {
        photonView.RPC("SetKeyNumber", RpcTarget.All);
    }

    [PunRPC]
    public void SetKeyNumber()
    {
        gameManager.expeditionHasproposed = false;  
        gameManager.game.key_counter--;
        gameManager.ui_Manager.SetNBKey();
        gameManager.ui_Manager.LaunchAnimationBrokenKey();
        gameManager.alreaydyExpeditionHadPropose = false;
        gameManager.nbKeyBroken++;
/*        if(gameManager.game.key_counter == 0 && !gameManager.HaveMoreKeyInTraversedRoom())
        {
            StartCoroutine(gameManager.CouroutineSacrificeAllPlayer());
        }*/
    }


    public void SendIsChooseForExpedition(int indexPlayer)
    {
        photonView.RPC("SetisChooseForExpedition", RpcTarget.All,indexPlayer);
    }
    public void SetisChooseForExpedition(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isChooseForExpedition = false;
        this.transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);
    }

    


    public void SendAskReset(int indexPlayer, int newPosX, int newPosY)
    {
        photonView.RPC("SetAskReset", RpcTarget.Others, indexPlayer,  newPosX,  newPosY);
    }

    [PunRPC]
    public void SetAskReset(int indexPlayer, int newPosX, int newPosY)
    {
        int nbKey = gameManager.game.key_counter;
        int indexCurrentRoom = gameManager.game.currentRoom.GetIndex();
        SendRespondReset(indexPlayer, nbKey, indexCurrentRoom);
    }

    public void SendRespondReset(int indexPlayer, int  nbKey, int indexCurrentRoom)
    {
        photonView.RPC("SetRespondReset", RpcTarget.Others, indexPlayer, nbKey, indexCurrentRoom);
    }

    [PunRPC]
    public void SetRespondReset(int indexPlayer , int nbKey, int indexCurrentRoom)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID == indexPlayer)
        {
            gameManager.game.key_counter = nbKey;
            gameManager.CloseAllDoor(gameManager.game.currentRoom, false);
            gameManager.OpenDoorMustBeOpen();
            gameManager.HidePlayerNotInSameRoom();
            gameManager.ClearDoor();
            gameManager.ClearExpedition();
            gameManager.ui_Manager.ResetNbVote();
            gameManager.ui_Manager.DesactiveZoneDoor();
            gameManager.voteDoorHasProposed = false;
            gameManager.expeditionHasproposed = false;
            gameManager.timer.ResetTimer();
            gameManager.ClearDoor();
            gameManager.ClearExpedition();
            gameManager.ui_Manager.DisplayKeyAndTorch(true);
            gameManager.alreadyPass = false;
            gameManager.SetAlreadyHideForAllPlayers();
            gameManager.ui_Manager.HideZoneVote();
            gameManager.ResetDoor();
            gameManager.ui_Manager.blackWallPaper.SetActive(false);
            gameManager.ui_Manager.LoadPage.SetActive(false);
            gameManager.ui_Manager.x_zone_red.SetActive(false);
            gameManager.ui_Manager.zones_X.SetActive(false);
            gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
            gameManager.ui_Manager.ResetExplorationGost();
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().haveToGoToExpedition = false;
            gameManager.gameManagerNetwork.SendHavetoGoToExpedition(false, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition = false;


        }
    }


    public void SendHellIsFind(bool hellIsFind  , int indexPlayer)
    {
        photonView.RPC("SetHellIsFind", RpcTarget.All, hellIsFind, indexPlayer);
    }

    [PunRPC]
    public void SetHellIsFind(bool hellIsFind,  int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().hellIsFind = hellIsFind;
    }

    public void SendParadiseIsFind(int indexPlayer)
    {
        photonView.RPC("SetParadiseIsFind", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetParadiseIsFind(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().paradiseIsFind = true;

        if (gameManager.AllLostSoulFindParadise())
        {
            StartCoroutine(gameManager.CouroutineOpenDoorParadise());
        }
    }


    public void SendParadiseOrHellFind(bool paradise, bool hell)
    {
        photonView.RPC("SetParadiseOrHellFind", RpcTarget.All, paradise, hell);
    }

    [PunRPC]
    public void SetParadiseOrHellFind(bool paradise, bool hell)
    {
        gameManager.DoorParadiseOrHellisOpen = true;
        gameManager.OnePlayerFindParadise = paradise;
        gameManager.OnePlayerFindHell = hell;
    }

    public void SendOpenDoorParadiseForAll()
    {
        photonView.RPC("SetOpenDoorParadiseForAll", RpcTarget.All);
    }

    [PunRPC]
    public void SetOpenDoorParadiseForAll()
    {
        gameManager.ui_Manager.OpenParadise();  
    }

    public void SendActiveZoneVoteChest()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetActiveZoneVoteChest", RpcTarget.All);
    }

    [PunRPC]
    public void SetActiveZoneVoteChest()
    {
        gameManager.voteChestHasProposed = true;
        gameManager.ui_Manager.ActiveZoneVoteChest(true);
        gameManager.CloseDoorWhenVote(true);
        StartCoroutine(gameManager.LaunchTimerChest());
    }

    public void SendChestData(int indexRoom , int index, bool isAward, int indexAward, int indexTrap)
    {
        photonView.RPC("SetChestData", RpcTarget.Others, indexRoom,index, isAward, indexAward, indexTrap);
    }

    [PunRPC]
    public void SetChestData(int indexRoom, int index, bool isAward, int indexAward, int indexTrap)
    {
        gameManager.game.dungeon.rooms[indexRoom].chestList.Add(Chest.CreateInstance(index, isAward, indexAward, indexTrap));
    }

    public void SendFireBallData(int indexRoom,bool isFireBall)
    {
        photonView.RPC("SetFireBallData", RpcTarget.All, indexRoom, isFireBall);
    }

    [PunRPC]
    public void SetFireBallData(int indexRoom, bool isFireBall)
    {
        gameManager.game.dungeon.rooms[indexRoom].fireBall = isFireBall;
    }

    public void SendSacrificeData(int indexRoom, bool isSacrifice)
    {
        photonView.RPC("SetSacrificeData", RpcTarget.All, indexRoom, isSacrifice);
    }

    [PunRPC]
    public void SetSacrificeData(int indexRoom, bool isSacrifice)
    {
        gameManager.game.dungeon.rooms[indexRoom].isSacrifice = isSacrifice;
    }


    public void SendAxData(int indexRoom, bool isAx)
    {
        photonView.RPC("SetAxData", RpcTarget.All, indexRoom, isAx);
    }

    [PunRPC]
    public void SetAxData(int indexRoom, bool isAx)
    {
        gameManager.game.dungeon.rooms[indexRoom].isAx = isAx;
    }

    public void SendSwordData(int indexRoom, bool isSword)
    {
        photonView.RPC("SetSwordData", RpcTarget.All, indexRoom, isSword);
    }

    [PunRPC]
    public void SetSwordData(int indexRoom, bool isSword)
    {
        gameManager.game.dungeon.rooms[indexRoom].isSword = isSword;
    }

    public void SendDamoclesData(int indexRoom, bool isDamocles)
    {
        photonView.RPC("SetDamoclesData", RpcTarget.All, indexRoom, isDamocles);
    }

    [PunRPC]
    public void SetDamoclesData(int indexRoom, bool isDamocles)
    {
        gameManager.game.dungeon.rooms[indexRoom].isSwordDamocles = isDamocles;
    }

    public void SendDeahtNPCData(int indexRoom, bool isDeath)
    {
        photonView.RPC("SetDeahtNPCData", RpcTarget.All, indexRoom, isDeath);
    }

    [PunRPC]
    public void SetDeahtNPCData(int indexRoom, bool isDeath)
    {
        gameManager.game.dungeon.rooms[indexRoom].isDeathNPC = isDeath;
    }

    public void SendLostTorchData(int indexRoom, bool isLostTorch)
    {
        photonView.RPC("SetLostTorchData", RpcTarget.All, indexRoom, isLostTorch);
    }

    [PunRPC]
    public void SetLostTorchData(int indexRoom, bool isLostTorch)
    {
        gameManager.game.dungeon.rooms[indexRoom].isLostTorch = isLostTorch;
    }
    public void SendMonstersData(int indexRoom, bool isMonster)
    {
        photonView.RPC("SetMonstersData", RpcTarget.All, indexRoom, isMonster);
    }

    [PunRPC]
    public void SetMonstersData(int indexRoom, bool isMonster)
    {
        gameManager.game.dungeon.rooms[indexRoom].isMonsters = isMonster;
    }
    public void SendPurificationData(int indexRoom, bool isPurification)
    {
        photonView.RPC("SetPurificationData", RpcTarget.All, indexRoom, isPurification);
    }

    [PunRPC]
    public void SetPurificationData(int indexRoom, bool isPurification)
    {
        gameManager.game.dungeon.rooms[indexRoom].isPurification = isPurification;

    }
    public void SendResurectionData(int indexRoom, bool isResurection)
    {
        photonView.RPC("SetResurectionData", RpcTarget.All, indexRoom, isResurection);
    }

    [PunRPC]
    public void SetResurectionData(int indexRoom, bool isResurection)
    {
        gameManager.game.dungeon.rooms[indexRoom].isResurection = isResurection;
    }
    public void SendPrayData(int indexRoom, bool isPray)
    {
        photonView.RPC("SetPrayData", RpcTarget.All, indexRoom, isPray);
    }

    [PunRPC]
    public void SetPrayData(int indexRoom, bool isPray)
    {
        gameManager.game.dungeon.rooms[indexRoom].isPray = isPray;
    }

    public void SendNPCData(int indexRoom, bool isNpc, int indexEvil, int indexEvil_2)
    {
        photonView.RPC("SetNPCData", RpcTarget.All, indexRoom, isNpc, indexEvil, indexEvil_2);
    }

    [PunRPC]
    public void SetNPCData(int indexRoom, bool isNpc, int indexEvil, int indexEvil_2)
    {
        gameManager.game.dungeon.rooms[indexRoom].isNPC = isNpc;
        gameManager.game.dungeon.rooms[indexRoom].indexEvilNPC = indexEvil;
        gameManager.game.dungeon.rooms[indexRoom].indexEvilNPC_2 = indexEvil_2;
    }

    public void SendLabyrinthData(int indexRoom, bool isLabyrintheHide)
    {
        photonView.RPC("SetLabyrinthData", RpcTarget.All, indexRoom, isLabyrintheHide);
    }

    [PunRPC]
    public void SetLabyrinthData(int indexRoom, bool isLabyrintheHide)
    {
        gameManager.game.dungeon.rooms[indexRoom].isLabyrintheHide = isLabyrintheHide;
    }

    public void SendIsImpostorRoomData(int indexRoom, bool isImpostorRoom)
    {
        photonView.RPC("SetIsImpostorRoomData", RpcTarget.All, indexRoom, isImpostorRoom);
    }

    [PunRPC]
    public void SetIsImpostorRoomData(int indexRoom, bool isImpostorRoom)
    {
        gameManager.game.dungeon.rooms[indexRoom].isImpostorRoom = isImpostorRoom;
    }

    public void SendJailRoom(int indexRoom , bool isJail)
    {
        photonView.RPC("SetJailRoom", RpcTarget.All, indexRoom, isJail);
    }

    [PunRPC]
    public void SetJailRoom(int indexRoom , bool isJail)
    {
        gameManager.game.dungeon.rooms[indexRoom].isJail = isJail;
    }


    public void SendNewParadise(int index)
    {
        photonView.RPC("SetNewParadise", RpcTarget.Others, index);
    }

    [PunRPC]
    public void SetNewParadise(int index)
    {
        gameManager.game.dungeon.exit.isOldParadise = true;
        gameManager.game.dungeon.exit.IsExit = false;
        Room newParadise = gameManager.game.dungeon.GetRoomByIndex(index);
        gameManager.game.dungeon.exit = newParadise;
        newParadise.isNewParadise = true;
        gameManager.ResetSpeciallyRoomState(newParadise);
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            gameManager.GetHexagone(newParadise.Index).DiplayInformationSpeciallyRoom(false);
        newParadise.IsExit = true;
        gameManager.SetCurrentRoomColor();
        gameManager.game.dungeon.SetPathFindingDistanceAllRoom();
        
        
    }


    public void SendDisplayFireBallRoom(bool display)
    {
        photonView.RPC("SetDisplayFireBallRoom", RpcTarget.All, display);
    }
    [PunRPC]
    public void SetDisplayFireBallRoom(bool display)
    {
        if (!display)
        {
            gameManager.ResetFireBallRoom();
            gameManager.speciallyIsLaunch = false;
            gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        }
        //gameManager.game.currentRoom.speciallyPowerIsUsed = !display;
        gameManager.CloseAllDoor(gameManager.game.currentRoom, false);
        gameManager.fireBallIsLaunch = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = !display;
        gameManager.ActivateCollisionTPOfAllDoor(true);
    }

    public void SendLaunchFireBallRoom()
    {
        int categorieFireball = Random.Range(0, 2);
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchFireBallRoom", RpcTarget.All, categorieFireball);
    }

    [PunRPC]
    public void SetLaunchFireBallRoom(int categorieFireball)
    {
        gameManager.fireBallIsLaunch = true;
        GameObject.Find("FireBallRoom").GetComponent<FireBallRoom>().LanchFireBallRoom();
    }

    public void SendDisplayMainLevers(bool display)
    {
        photonView.RPC("SetDisplayMainLevers", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayMainLevers(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
            return;
        gameManager.ui_Manager.DisplayMainLevers(display);
    }

    public void SendDisplayDoorLever(bool display)
    {
        photonView.RPC("SetDisplayDoorLever", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayDoorLever(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
            return;
        gameManager.ui_Manager.DisplayLeverVoteDoor(display);
        gameManager.expeditionHasproposed = false;
        gameManager.CloseDoorWhenVote(false);
    }

    public void SendDisplaySpeciallyLevers(bool display , int indexSpecially)
    {
        photonView.RPC("SetDisplaySpeciallyLevers", RpcTarget.All, display, indexSpecially);
    }

    [PunRPC]
    public void SetDisplaySpeciallyLevers(bool display, int indexSpecially)
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.ui_Manager.DisplaySpeciallyLevers(display , indexSpecially);
    }



    public void SendDisplayNuVoteSacrificeForAllPlayer()
    {
        photonView.RPC("SetDisplayNuVoteSacrificeForAllPlayer", RpcTarget.All) ;
    }

    [PunRPC]
    public void SetDisplayNuVoteSacrificeForAllPlayer()
    {
        gameManager.ui_Manager.DisplayNuVoteSacrificeForAllPlayer();
    }


    public void  SendSacrificeVoteIsLaunch(bool isLaunch)
    {
        photonView.RPC("SetSacrificeVoteIsLaunch", RpcTarget.All, isLaunch);
    }

    [PunRPC]
    public void SetSacrificeVoteIsLaunch(bool isLaunch)
    {
        if(GameObject.Find("SacrificeRoom"))
            GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().sacrificeVoteIsLaunch = isLaunch;
        gameManager.speciallyIsLaunch = isLaunch;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(isLaunch);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendResetClickToExpedition();
        gameManager.ui_Manager.DisplayMainLevers(!isLaunch);
    }
    public void SendUpdateNeighbourSpeciality(int indexRoom , int indexSpeciality)
    {
        photonView.RPC("SetUpdateNeighbourSpeciality", RpcTarget.All, indexRoom, indexSpeciality);
    }


    [PunRPC]
    public void SetUpdateNeighbourSpeciality(int indexRoom, int indexSpeciality)
    {
        Room room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
        switch (indexSpeciality)
        {
            case 0:
                room.fireBall = true;
                break;
            case 1:
                room.isSword = true;
                break;
            case 2:
                room.isAx = true;
                break;
            case 3:
                room.isSwordDamocles = true;
                break;
            case 4:
                room.isLostTorch = true;
                break;
            case 5:
                room.isDeathNPC = true;
                break;
            case 6:
                room.isMonsters = true;
                break;
            case 7:
                room.isLabyrintheHide = true;
                break;
        }
        room.speciallyIsInsert = true;
    }

    public void SendUpdateListSpecialityProbality(float newValue, int indexSpeciality)
    {
        photonView.RPC("SetUpdateListSpecialityProbality", RpcTarget.All, newValue, indexSpeciality);
    }

    [PunRPC]
    public void SetUpdateListSpecialityProbality(float newValue, int indexSpeciality)
    {
        gameManager.listProbabilitySpecialityRoom[indexSpeciality] = newValue;
    }

    public void SendCatchInJailRoom(int indexDoorExpedition)
    {
        photonView.RPC("SetCatchInJailRoom", RpcTarget.Others, indexDoorExpedition);
    }

    [PunRPC]
    public void SetCatchInJailRoom(int indexDoorExpedition)
    {

        int indexDoor = gameManager.GetIndexDoorAfterCrosse(indexDoorExpedition);
        GameObject door = gameManager.GetDoorGo(indexDoor);
        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
        door.GetComponent<Door>().old_player = null;
        door.GetComponent<Door>().player = null;
        door.GetComponent<Door>().isOpen = false;
        door.GetComponent<Door>().counterPlayerInDoorZone = 0;
        door.GetComponent<Door>().letter_displayed = false;
    }


    public void SendCancelDoorExploration(int indexDoor)
    {
        photonView.RPC("SetCancelDoorExploration", RpcTarget.All, indexDoor);
    }

    [PunRPC]
    public void SetCancelDoorExploration(int indexDoor)
    {
        GameObject door = gameManager.GetDoorGo(indexDoor);
        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
        door.GetComponent<Door>().old_player = null;
        door.GetComponent<Door>().player = null;
        door.GetComponent<Door>().isOpen = false;
        door.GetComponent<Door>().counterPlayerInDoorZone = 0;
        door.GetComponent<Door>().letter_displayed = false;
        gameManager.ui_Manager.DisplayGostPlayer(indexDoor, false, 0);
    }

    public void SendHexagoneNewPower(int indexHexagone, int indexPower)
    {
        photonView.RPC("SetHexagoneNewPower", RpcTarget.All, indexHexagone, indexPower);
    }

    [PunRPC]
    public void SetHexagoneNewPower(int indexHexagone, int indexPower)
    {
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            gameManager.GetHexagone(indexHexagone).gameObject.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.SetActive(true);
        if (indexPower == 0)
            gameManager.GetHexagone(indexHexagone).Room.IsFoggy = true;
        if (indexPower == 1)
            gameManager.GetHexagone(indexHexagone).Room.IsVirus = true;
        if (indexPower == 2)
        {
            gameManager.GetHexagone(indexHexagone).Room.isTraped = true;
            gameManager.GetHexagone(indexHexagone).Room.isPray = true;
        }   
        if (indexPower == 3)
        {
            gameManager.GetHexagone(indexHexagone).Room.chest = true;
            gameManager.GetHexagone(indexHexagone).Room.isTraped = true;
        }

        gameManager.GetHexagone(indexHexagone).Room.isSpecial = true;
    }

    public void SendLightHexagone(int indexHexagone, bool activeLight)
    {
        photonView.RPC("SetLightHexagone", RpcTarget.Others, indexHexagone, activeLight);
    }

    [PunRPC]
    public void SetLightHexagone(int indexHexagone, bool activeLight)
    {
        gameManager.GetHexagone(indexHexagone).SetLight(activeLight);
    }

    public void SendDistanceAwardChest(int indexChest)
    {
        photonView.RPC("SetDistanceAwardChest", RpcTarget.All, indexChest);
    }

    [PunRPC]
    public void SetDistanceAwardChest(int indexChest)
    {
        gameManager.ui_Manager.SetDistanceTextAwardChest(indexChest);
    }

    public void SendChangeLeverDeathNPC()
    {
        photonView.RPC("SetChangeLeverDeathNPC", RpcTarget.All);
    }

    [PunRPC]
    public void SetChangeLeverDeathNPC()
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManager.UpdateSpecialsRooms(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room);
        gameManager.ui_Manager.DisplayMainLevers(true);
    }

    public void SendLaunchDeathNPC()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchDeathNPC", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchDeathNPC()
    {
        StartCoroutine(GameObject.Find("DeathNPCRoom").GetComponent<DeathNpcRoom>().StartDeathNPCRoomAfterTeleportation());
    }
    public void SendLaunchDamoclesRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchDamoclesRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchDamoclesRoom()
    {
        GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>().LaunchDamoclesSwordRoom();
        
    }
    public void SendLaunchAxRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchAxRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchAxRoom()
    {
        GameObject.Find("AxRoom").GetComponent<AxRoom>().LaunchAxRoom();

    }

    public void SendNpcRoom()
    {
        //GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetNPCRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetNPCRoom()
    {
        GameObject.Find("NPCRoom").GetComponent<NPCRoom>().LaunchNPCRoom();

    }

    public void SendLaunchSwordRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchSwordRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchSwordRoom()
    {
        if (GameObject.Find("SwordRoom"))
            GameObject.Find("SwordRoom").GetComponent<SwordRoom>().LaunchSwordRoom(); ;

    }
    public void SendLaunchLostTorchRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchLostTorchRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchLostTorchRoom()
    {
        if(GameObject.Find("LostTorchRoom"))
            GameObject.Find("LostTorchRoom").GetComponent<LostTorchRoom>().StartLostTorchRoom();

    }
    public void SendLaunchMonsterRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchMonsterRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchMonsterRoom()
    {
        if (GameObject.Find("MonstersRoom"))
        {
            GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>().StartMonstersRoom();
            //gameManager.ui_Manager.LaunchFightMusic();
        }
            
    }



    public void SendLaunchPurificationRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchPurificationRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchPurificationRoom()
    {
        GameObject.Find("PurificationRoom").GetComponent<PurificationRoom>().LaunchPurificationRoom();
    }

    public void SendLaunchResurectionRoom()
    {
        //GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchResurectionRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchResurectionRoom()
    {
        if (GameObject.Find("ResurectionRoom"))
            GameObject.Find("ResurectionRoom").GetComponent<ResurectionRoom>().LaunchResurectionRoom();
    }

    public void SendLaunchPrayRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchPrayRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchPrayRoom()
    {
        GameObject.Find("PrayRoom").GetComponent<PrayRoom>().LaunchPrayRoom();
    }

    public void SendLaunchLabyrinthRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        photonView.RPC("SetLaunchLabyrinthRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchLabyrinthRoom()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBossEvenSameRoom();
        GameObject.Find("LabyrinthHideRoom").GetComponent<LabyrinthRoom>().StartRoom();
    }

    public void SendDisplayLightAllAvailableDoor(bool display)
    {
        photonView.RPC("DisplayLightAllAvailableDoor", RpcTarget.All, display);
    }

    [PunRPC]
    public void DisplayLightAllAvailableDoor(bool display)
    {
        return;
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower == -1)
            return;
        if (gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed && display)
            return;
        if (!gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed && !display)
            return;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().barricade)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().isOpenForAll)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (!door.GetComponent<Door>().GetRoomBehind().isHide &&
                   !(door.GetComponent<Door>().GetRoomBehind().chest && 
                   gameManager.GetPlayerMineGO().transform.Find("PowerImpostor")
                   .GetComponent<PowerImpostor>().indexPower == 3))
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().iscurrentlyOpen)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.speciallyIsLaunch)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().GetRoomBehind().isTraped)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (!gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().canUsed)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.OnePlayerHaveToGoToExpedition())
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().GetRoomBehind().isSpecial)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            door.transform.Find("RedLight").gameObject.SetActive(display);
        }
    }

    public void SendDisplayLightAllAvailableDoorN2(bool display)
    {
        photonView.RPC("DisplayLightAllAvailableDoorN2", RpcTarget.All, display);
    }

    [PunRPC]
    public void DisplayLightAllAvailableDoorN2(bool display)
    {
        return;   
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower == -1)
            return;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().barricade)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().isOpenForAll)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (door.GetComponent<Door>().iscurrentlyOpen)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (!door.GetComponent<Door>().GetRoomBehind().isHide &&  !(door.GetComponent<Door>().GetRoomBehind().chest &&
                   gameManager.GetPlayerMineGO().transform.Find("PowerImpostor")
                   .GetComponent<PowerImpostor>().indexPower == 3))
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.speciallyIsLaunch)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (!gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().canUsed) 
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if (gameManager.OnePlayerHaveToGoToExpedition())
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            if(door.GetComponent<Door>().GetRoomBehind().isSpecial)
            {
                door.transform.Find("RedLight").gameObject.SetActive(false);
                continue;
            }
            door.transform.Find("RedLight").gameObject.SetActive(display);
            if (gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed)
                door.transform.Find("RedLight").gameObject.SetActive(false);
        }
        

    }



    public void SendDisplayPowerImpostorInGame()
    {
        photonView.RPC("SetDisplayPowerImpostorInGame", RpcTarget.All);

    }

    [PunRPC]
    public void SetDisplayPowerImpostorInGame()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        gameManager.GiveImpostorObject();
    }

    public void SendDisplayObjectPowerImpostor()
    {
        photonView.RPC("SetDisplayObjectPowerImpostor", RpcTarget.All);
    }

    [PunRPC]
    public void SetDisplayObjectPowerImpostor()
    {
        gameManager.ui_Manager.DisplayObjectPowerImpostorInGame();
    }

    public void SendIsDiscorved(bool isDiscovered, int indexRoom)
    {
        photonView.RPC("SetIsDiscovered", RpcTarget.All, isDiscovered , indexRoom);
    }
    [PunRPC]
    public void SetIsDiscovered( bool isDiscovered , int indexRoom)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).IsDiscovered = isDiscovered;
    }

    public void SendUpdateHidePlayer()
    {
        photonView.RPC("UpdateHidePlayer", RpcTarget.Others);
    }
    [PunRPC]
    public void UpdateHidePlayer()
    {
        gameManager.HidePlayerNotInSameRoom();
    }

    public void SendRandomSacrificePlayer(int indexPlayer)
    {
        photonView.RPC("SetRandomSacrificedPlayer", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetRandomSacrificedPlayer(int indexPlayer)
    {
        GameObject playerRevive = gameManager.GetPlayer(indexPlayer);
        playerRevive.transform.position = new Vector3(0, -0.3f);
       
        if (playerRevive.GetComponent<PhotonView>().IsMine)
        {
            if (!gameManager.SamePositionAtBoss())
            {
                
                gameManager.game.currentRoom = gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room;
                Debug.LogError(gameManager.game.currentRoom.Index);
                gameManager.CloseAllDoor(gameManager.game.currentRoom, false);
                gameManager.SetDoorNoneObstacle(gameManager.game.currentRoom);
                gameManager.SetDoorObstacle(gameManager.game.currentRoom);
                gameManager.SetCurrentRoomColor();
                gameManager.ui_Manager.HideDistanceRoom();
                
                gameManager.UpdateSpecialsRooms(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room);
            }
        }
        
        playerRevive.GetComponent<PlayerGO>().position_X = gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.X;
        playerRevive.GetComponent<PlayerGO>().position_Y = gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Y;

        if(playerRevive.GetComponent<PhotonView>().IsMine)
            gameManager.HidePlayerNotInSameRoom();
        StartCoroutine(gameManager.CloseDoorWhenVoteCoroutine(false));
        RevivePlayer(playerRevive);
        ResetRoom();
    }

    public void SendRelaunchRoom()
    {
        photonView.RPC("RelaunchRoom", RpcTarget.All);
    }
    [PunRPC]
    public void RelaunchRoom()
    {
        gameManager.ResurectionIsUsed = true;
        //gameManager.UpdateSpecialsRooms(this.gameManager.game.currentRoom);
        gameManager.ui_Manager.DisplaySpeciallyLevers(true, 11);
    }

    public void RevivePlayer(GameObject player)
    {
        player.GetComponent<PlayerNetwork>().SendResetSacrifice();
    }

    [PunRPC]
    public void ResetRoom()
    {
        this.gameManager.game.currentRoom.speciallyPowerIsUsed = true;
    }


    public void SendLoose()
    {
        photonView.RPC("SetLoose", RpcTarget.All);
    }

    [PunRPC]
    public void SetLoose()
    {
        StartCoroutine(this.gameManager.SacrificeAllLostSoul());
    }

    public void SendUpdateDataPlayer(int indexPlayer)
    {
        photonView.RPC("SetUpdateDataPlayer", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetUpdateDataPlayer(int indexPlayer)
    {
        gameManager.UpdataDataPlayer(indexPlayer);
    }

    public void SendChangeBoss()
    {

        photonView.RPC("SetChangeBoss", RpcTarget.All);
    }

    [PunRPC]
    public void SetChangeBoss()
    {
        gameManager.ChangeBoss();
    }

    public void SendChangementParadiseBool(bool paradiseHadChange)
    {
        photonView.RPC("SetChangementParadiseBool", RpcTarget.All , paradiseHadChange);
    }

    [PunRPC]
    public void SetChangementParadiseBool(bool paradiseHadChange)
    {
        gameManager.paradiseHasChange = paradiseHadChange;
    }

    public void SendOldSpecialityIndex(int indexRoom , int indexOldSpeciality)
    {
        photonView.RPC("SetOldSpecialityIndex", RpcTarget.All, indexRoom, indexOldSpeciality);
    }

    [PunRPC]
    public void SetOldSpecialityIndex(int indexRoom, int indexOldSpeciality)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).isOldSpeciality = indexOldSpeciality;
    }

    public void SendActivateAllObstacles(bool display, string nameObject)
    {
        photonView.RPC("SetActivateAllObstacles", RpcTarget.All, display, nameObject);
    }

    [PunRPC]
    public void SetActivateAllObstacles(bool display, string nameObject)
    {
        gameManager.ui_Manager.DesactivateAllobstacles(nameObject, display);
    }


    public void SendDisplayLightExplorationTransparency(int doorId)
    {
        photonView.RPC("SetDisplayLightExplorationTransparency", RpcTarget.Others, doorId);
    }

    [PunRPC]
    public void SetDisplayLightExplorationTransparency(int doorId)
    {
        gameManager.GetDoorGo(doorId).GetComponent<Door>().DisplayTransparencyLightExploration();
        gameManager.ui_Manager.doorTorched_black.Play();
    }

    public void SendDisplayTrappedDoor(int indexDoor)
    {
        photonView.RPC("DisplayTrappedDoor", RpcTarget.All, indexDoor);
    }

    [PunRPC]
    public void DisplayTrappedDoor(int indexDoor)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;

        Door door = gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        door.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        gameManager.ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(door.index, true);
    }

    public void SendNewSpeciallyRoom(int indexRoom, int indexSpeciallity)
    {
        photonView.RPC("SetNewSpeciallyRoom", RpcTarget.All, indexRoom, indexSpeciallity);
    }


    [PunRPC]
    public void SetNewSpeciallyRoom(int indexRoom, int indexSpeciallity)
    {
        gameManager.ResetSpeciallyRoomState(gameManager.game.dungeon.GetRoomByIndex(indexRoom));

        switch (indexSpeciallity)
        {
            case 0:
                Room room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
                room.chest = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    gameManager.game.dungeon.InsertChestRoom(indexRoom);
                    for (int i = 0; i < 2; i++)
                    {
                        gameManager.gameManagerNetwork.SendChestData(indexRoom, room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward, room.chestList[i].indexTrap);
                    }
                }

                break;
            case 1:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isSacrifice = true;
                break;
            case 2:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isPray = true;
                break;
            case 3:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isResurection = true;
                break;
            case 4:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isPurification = true;
                break;
        }
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).isSpecial = true;

    }

    public void SendNewTrapedRoom(int indexRoom, int indexSpeciallity)
    {
        photonView.RPC("SetNewTrapedRoom", RpcTarget.All, indexRoom, indexSpeciallity);
    }


    [PunRPC]
    public void SetNewTrapedRoom(int indexRoom, int indexSpeciallity)
    {
       

        switch (indexSpeciallity)
        {
            case 0:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).IsFoggy = true;
                break;
            case 1:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).IsVirus = true;
                break;
            case 2:
                gameManager.ResetSpeciallyRoomState(gameManager.game.dungeon.GetRoomByIndex(indexRoom));
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isPray = true;
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isTraped = true;
                break;
            case 3:
                gameManager.ResetSpeciallyRoomState(gameManager.game.dungeon.GetRoomByIndex(indexRoom));
                Room room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
                room.chest = true;
                room.isTraped = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                gameManager.game.dungeon.InsertChestRoom(indexRoom);
                for (int i = 0; i < 2; i++)
                {
                    gameManager.gameManagerNetwork.SendChestData(indexRoom, room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward, room.chestList[i].indexTrap);
                }
                break;
            case 4:
                gameManager.game.dungeon.GetRoomByIndex(indexRoom).isIllustion = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    gameManager.MixRoomNeigbour(gameManager.game.dungeon.GetRoomByIndex(indexRoom));
                }
                break;
        }
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).isTraped = true;

    }



    public void SendOrangeDoor(int indexDoor)
    {
        photonView.RPC("SetOrangeDoor", RpcTarget.All, indexDoor);
    }

    [PunRPC]
    public void SetOrangeDoor(int indexDoor)
    {
        Door door = gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        door.GetComponent<SpriteRenderer>().color = new Color(255, (195f/255f), 0);
        gameManager.ui_Manager.magical_key.Play();
    }

    public void SendDoorInNPCRoom(int indexRoom, string doorName)
    {
        photonView.RPC("SetDoorInNPCRoom", RpcTarget.All, indexRoom, doorName);
    }
    [PunRPC]
    public void SetDoorInNPCRoom(int indexRoom, string doorName)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorInNpc = doorName;
    }

    public void SendEvilInNPCRoom(int indexRoom, bool evilIsLeft)
    {
        photonView.RPC("SetEvilInNPCRoom", RpcTarget.All, indexRoom, evilIsLeft);
    }
    [PunRPC]
    public void SetEvilInNPCRoom(int indexRoom, bool evilIsLeft)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).evilIsLeft = evilIsLeft;
    }

    public void SendNpcChooseLeft(int indexRoom, int indexNpc)
    {
        photonView.RPC("SetNpcChooseLeft", RpcTarget.All, indexRoom, indexNpc);
    }

    [PunRPC]
    public void SetNpcChooseLeft(int indexRoom, int indexNpc)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).npcChooseIndex = indexNpc;
    }

    public void SendNpcDoorShorterAndLonger(int indexRoom, string shorter, string longer)
    {
        photonView.RPC("SetNpcDoorShorterAndLonger", RpcTarget.All, indexRoom, shorter, longer) ;
    }

    [PunRPC]
    public void SetNpcDoorShorterAndLonger(int indexRoom, string shorter, string longer)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameLongerNPC = longer;
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameShorterNPC = shorter;
    }

    public void SendPowerIsUsed(int indexRoom, bool isUsed)
    {
        photonView.RPC("SetPowerIsUsed", RpcTarget.All, indexRoom, isUsed);
    }

    [PunRPC]
    public void SetPowerIsUsed(int indexRoom, bool isUsed)
    {
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).speciallyPowerIsUsed = isUsed;
    }

    public void SendListIndexDoor(int indexRoom, int value, int i)
    {
        photonView.RPC("SetListIndexDoor", RpcTarget.Others, indexRoom, value, i);
    }

    [PunRPC]
    public void SetListIndexDoor(int indexRoom, int value, int i)
    {
        Room room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
        room.listIndexDoor[i] = value;
    }

    public void SendResetNoneValueInListIndexDoor(int indexRoom)
    {
        photonView.RPC("SetResetNoneValueInListIndexDoor", RpcTarget.Others, indexRoom);
    }
    [PunRPC]
    public void SetResetNoneValueInListIndexDoor(int indexRoom)
    {
        Room room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
        for (int j = 0; j < room.listIndexDoor.Count; j++)
        {
            if (room.listIndexDoor[j] == -1)
            {
                room.listIndexDoor.RemoveAt(j);
                j--;
            }
        }
    }

    public void SendTemporyCloseDoor()
    {
        photonView.RPC("SetTemporyCloseDoor", RpcTarget.All);
    }

    [PunRPC]
    public void SetTemporyCloseDoor()
    {
        gameManager.CloseDoorWhenVote(false);
        gameManager.ActivateCollisionTPOfAllDoor(true);
    }

    public void SendIndexPreviousExplorater(int indexPlayer)
    {
        photonView.RPC("SetIndexPreviousExplorater", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetIndexPreviousExplorater(int indexPlayer)
    {
        if (gameManager.indexPlayerPreviousExploration != -1)
        {
            gameManager.GetPlayer(gameManager.indexPlayerPreviousExploration).transform.Find("TorchBarre").gameObject.SetActive(false);
        }
        gameManager.indexPlayerPreviousExploration = indexPlayer;
        gameManager.GetPlayer(indexPlayer).transform.Find("TorchBarre").gameObject.SetActive(true);
    }

    public void SendChangeBoss2()
    {
        photonView.RPC("SetChangeBoss2", RpcTarget.All);
    }
    
    [PunRPC]
    public void SetChangeBoss2()
    {
        gameManager.ChangeBoss();
    }

    public void SendImpostorRoom()
    {
        int counter = 0;
        int distance = 4;
        bool isNearOfInitial = false;
        int scale = 1;
        foreach (GameObject player in gameManager.GetAllImpostor())
        {
            if(gameManager.GetAllImpostor().Count == 1)
            {
                // permet faire en sorte que lorsque l'impostor room est au milieu lui mettre un random quand la distance/2 et impair ( ex : 5 => 2 ou 3) au lieu de 2 tjr

                Debug.Log(gameManager.game.dungeon.initialRoom.DistancePathFinding / 2);
                Debug.Log((gameManager.game.dungeon.initialRoom.DistancePathFinding / 2) % 2);
                if (gameManager.game.dungeon.initialRoom.DistancePathFinding%2 != 0)
                {
                    int randomInt = Random.Range(0, 2);
                    if(randomInt == 0)
                    {
                        distance = gameManager.game.dungeon.initialRoom.DistancePathFinding / 2;
                    }
                    else if ( randomInt == 1)
                    {
                        distance = (gameManager.game.dungeon.initialRoom.DistancePathFinding / 2) + 1;
                    }
                }
                else
                {
                    distance = (gameManager.game.dungeon.initialRoom.DistancePathFinding / 2);
                }
                isNearOfInitial = false;
            }
            else
            {
                if(gameManager.GetAllImpostor().Count == 2)
                {
                    if (counter == 0)
                    {
                        distance = Random.Range(1, 3);
                        isNearOfInitial = true;
                        scale = -1;
                    }
                    else if (counter == 1)
                    {
                        distance = Random.Range(2, 4);
                        isNearOfInitial = false;
                    }
                }
                else
                {
                    if (counter == 0)
                    {
                        distance = Random.Range(1, 3);
                        isNearOfInitial = true;
                        scale = -1;
                    }
                    else if (counter == 1)
                    {
                        distance = Random.Range(2, 4);
                        isNearOfInitial = false;
                        scale = -1;
                    }
                    else
                    {
                        distance = (gameManager.game.dungeon.initialRoom.DistancePathFinding / 2);
                        isNearOfInitial = true;
                        scale = -2;
                    }
                }
            }

            photonView.RPC("SetImpostorRoom", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, distance, isNearOfInitial, scale);
            counter++;
        }
    }

    [PunRPC]
    public void SetImpostorRoom(int indexPlayer, int distance, bool isNearOfInitial, int scale)
    {
        if (indexPlayer != gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
            return;

        int indexRoom = gameManager.game.dungeon.InsertImpostorRoom(distance, isNearOfInitial, scale);
        SendResetSpeciallyRoomToImpostor(indexRoom);
        gameManager.GetHexagone(indexRoom).DiplayInformationSpeciallyRoom(false);
 
    }

    public void SendResetSpeciallyRoomToImpostor(int indexRoom)
    {
        photonView.RPC("SetResetSpeciallyRoomToImpostor", RpcTarget.Others, indexRoom);
    }

    [PunRPC]
    public void SetResetSpeciallyRoomToImpostor(int indexRoom)
    {
        gameManager.ResetSpeciallyRoomState(gameManager.game.dungeon.GetRoomByIndex(indexRoom));
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).isHide = true;
        gameManager.GetHexagone(indexRoom).DiplayInformationSpeciallyRoom(false);
        
        //gameManager.game.dungeon.GetRoomByIndex(indexRoom).isImpostorRoom = true;
    }

    public void SendIsEndGame(bool isEndGame)
    {
        photonView.RPC("SetIsEndGame", RpcTarget.All, isEndGame);
    }

    [PunRPC]
    public void SetIsEndGame(bool value)
    {
        gameManager.isEndGame = value;
    }

    public void SendPlayerList(int indexList, int  indexPlayer)
    {
        photonView.RPC("SetPlayerList", RpcTarget.Others, indexList , indexPlayer);
    }

    [PunRPC]
    public void SetPlayerList(int indexList, int indexPlayer)
    {
        gameManager.listPlayerFinal.Add(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>());
    }

    public void SendClearPlayerList()
    {
        photonView.RPC("SetClearPlayerList", RpcTarget.Others);
    }

    [PunRPC]
    public void SetClearPlayerList()
    {
        gameManager.listPlayerFinal.Clear();
    }

    public void SendEndVoteDoorCoroutine(int indexDoor)
    {
        photonView.RPC("SetEndVoteDoorCoroutine", RpcTarget.All, indexDoor);
    }

    [PunRPC]
    public void SetEndVoteDoorCoroutine(int indexDoor)
    {
        GameObject door = gameManager.GetDoorGo(indexDoor);

        if (gameManager.SamePositionAtBoss() && gameManager.VerifyVoteVD(door.GetComponent<Door>().nbVote))
        {
            if (door.GetComponent<Door>().nbVote == 0 || gameManager.game.currentRoom.IsVirus)
            {
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                {
                    List<GameObject> listDoorAvailable = gameManager.GetDoorAvailable();
                    int indexDoorCurrent = Random.Range(0, listDoorAvailable.Count);
                    SendRandomIndexDoor(listDoorAvailable[indexDoorCurrent].GetComponent<Door>().index);
                }
            }
            else
            {
                gameManager.ui_Manager.SetNBKey();
                gameManager.ui_Manager.LaunchAnimationBrokenKey();
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                    gameManager.gameManagerNetwork.SendKeyNumber();
                if (gameManager.SamePositionAtBoss())
                    gameManager.OpenDoor(door, false);
                gameManager.expeditionHasproposed = false;
                gameManager.alreaydyExpeditionHadPropose = false;

            }
            StartCoroutine(gameManager.ChangeBossCoroutine(0.1f));
        }

        gameManager.ui_Manager.ResetNbVote();
        gameManager.ui_Manager.DesactiveZoneDoor();
        gameManager.voteDoorHasProposed = false;
        gameManager.timer.ResetTimer();
        gameManager.ClearDoor();
        gameManager.ClearExpedition();
        gameManager.ui_Manager.DisplayKeyAndTorch(true);
        gameManager.alreadyPass = false;
        gameManager.SetAlreadyHideForAllPlayers();
        gameManager.UpdateSpecialsRooms(gameManager.game.currentRoom);
        if (gameManager.game.currentRoom.IsVirus)
            gameManager.ui_Manager.ResetLetterDoor();
        if (gameManager.game.dungeon.initialRoom.HasSameLocation(gameManager.game.currentRoom))
        {
            gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.DistancePathFinding, null);

        }
        gameManager.CloseDoorWhenVote(false);
        gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = 0;
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(false);
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivateTime(true, 3);

        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(false);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 3);
    }


}
