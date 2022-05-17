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

        gameManager.ui_Manager.SetDescriptionLoadPage("Syncronisation des paramètres..", 0.1f);
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
            gameManager.game.GetBoss().SetIsBoss(false);
            gameManager.GetBoss().GetComponent<PlayerGO>().isBoss = false;
        }
       
        gameManager.game.SetBoss(indexNewBoss);
        gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerGO>().isBoss = true;
        //gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerGO>().isBoss = true ;
        //gameManager.GetPlayer(indexNewBoss).GetComponent<PlayerGO>().LaunchTimerBoss();
        gameManager.ui_Manager.SetDescriptionLoadPage("Choix du chef..", 0.1f);

    }


    public void SendRole(int indexPlayer , bool isImpostor , bool isLast)
    {
        photonView.RPC("SetRole", RpcTarget.All, indexPlayer, isImpostor , isLast);
    }

    [PunRPC]
    public void SetRole(int indexPlayer, bool isImpostor, bool isLast)
    {
        gameManager.game.GetPlayerById(indexPlayer).SetIsImpostor(isImpostor);
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isImpostor = isImpostor;
        gameManager.ui_Manager.SetDescriptionLoadPage("Création des rôles.." , 0.1f);
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


    public void SendMap(int indexRoom, bool isExit, bool isObstacle, bool isInitial, int distance_exit, int distance_pathFinding,int distance_pathFinding_IR,bool isLast, bool isFoggy , bool isVirus, bool hasKey, bool chest)
    {
        photonView.RPC("SetRooms", RpcTarget.Others, indexRoom,isExit,isObstacle, isInitial, distance_exit,distance_pathFinding, distance_pathFinding_IR, isLast, isFoggy, isVirus, hasKey, chest);
    }

    [PunRPC]
    public void SetRooms(int indexRoom, bool isExit, bool isObstacle,bool isInitial, int distance_exit, int distance_pathFinding,int  distance_pathFinding_IR, bool isLast, bool isFoggy, bool isVirus, bool hasKey, bool chest)
    {
        gameManager.game.dungeon.rooms[indexRoom].IsExit = isExit;
        gameManager.game.dungeon.rooms[indexRoom].IsObstacle = isObstacle;
        gameManager.game.dungeon.rooms[indexRoom].DistanceExit = distance_exit;
        gameManager.game.dungeon.rooms[indexRoom].DistancePathFinding = distance_pathFinding;
        gameManager.game.dungeon.rooms[indexRoom].IsInitiale = isInitial;
        gameManager.game.dungeon.rooms[indexRoom].distance_pathFinding_initialRoom = distance_pathFinding_IR;

        gameManager.game.dungeon.rooms[indexRoom].IsFoggy = isFoggy;
        gameManager.game.dungeon.rooms[indexRoom].HasKey = hasKey;
        gameManager.game.dungeon.rooms[indexRoom].chest = chest;
        gameManager.game.dungeon.rooms[indexRoom].IsVirus = isVirus;
        
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
        gameManager.CloseDoorWhenVote(true);

        if (gameManager.ui_Manager.map.activeSelf)
        {
            gameManager.ui_Manager.DisplayMap();
           
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);


    }

    public void SendVoteYesToExploration()
    {
        photonView.RPC("SetVoteYesToExploration", RpcTarget.All);
    }

    [PunRPC]
    public void SetVoteYesToExploration()
    {
        if (gameManager.setting.LIMITED_TORCH && !gameManager.game.currentRoom.fireBall)
            gameManager.SetNbTorch(gameManager.game.current_expedition.Count);
        if (gameManager.SamePositionAtBoss())
            gameManager.OpenDoorsToExpedition();
        gameManager.alreaydyExpeditionHadPropose = true;
        gameManager.SetPlayersHaveTogoToExpeditionBool();
        gameManager.ResetVoteExploration();
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        //gameManager.game.currentRoom
    }

    public void SendVoteNoToExploration()
    {
        photonView.RPC("SetVoteNoToExploration", RpcTarget.All);
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
        gameManager.ui_Manager.SetDescriptionLoadPage("Initialisation des objets..", 0.1f);
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
        gameManager.timer.LaunchTimer(5, false);
        StartCoroutine(gameManager.LauchVoteDoorCoroutine());
        gameManager.voteDoorHasProposed = true;

        if (gameManager.ui_Manager.map.activeSelf)
        {
            gameManager.ui_Manager.DisplayMap();
           
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);

        StartCoroutine(CoroutineActiveZoneDoor());



    }

    public IEnumerator CoroutineActiveZoneDoor()
    {
        yield return new WaitForSeconds(0.15f);
        if (!gameManager.game.currentRoom.IsVirus)
            gameManager.ui_Manager.ActiveZoneDoor(gameManager.SamePositionAtBoss());
        else
            gameManager.ui_Manager.ActiveXzone(true);
    }



    public void SendCloseDoorWhenVote()
    {
        photonView.RPC("SetCloseDoorWhenVote", RpcTarget.All);
    }

    [PunRPC]
    public void SetCloseDoorWhenVote()
    {
        gameManager.CloseDoorWhenVote(true);
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
        if (gameManager.AllPlayerGoneToHell() && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
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
                player.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
            return;
        }
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
        //Debug.LogError(door.GetComponent<Door>().nbVote + " " + door.GetComponent<Door>().doorName);
        player.transform.GetChild(1).GetChild(4).gameObject.SetActive(enter);
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
            player.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
        }
        else
        {
            if (enter)
            {
                gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote++;
                //player.GetComponent<PlayerGO>().hasVoteVD = true;
                //player.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
            }
            else
            { 
                gameManager.ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote--;
                //player.GetComponent<PlayerGO>().hasVoteVD = false;
                player.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);

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
            player.transform.GetChild(1).GetChild(9).gameObject.SetActive(true);
            if (indexChest == 1)
                player.transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
        }
        else
        {
            if (enter)
            {
                
                chest.transform.Find("VoteZone").GetComponent<ChestZoneVote>().nbVote++;
                player.transform.GetChild(1).GetChild(9).gameObject.SetActive(true);
                if (indexChest == 1)
                    player.transform.GetChild(1).GetChild(6).gameObject.SetActive(true);
            }
            else
            {
                chest.transform.Find("VoteZone").GetComponent<ChestZoneVote>().nbVote--;
                player.transform.GetChild(1).GetChild(9).gameObject.SetActive(false);
                if (indexChest == 1)
                    player.transform.GetChild(1).GetChild(6).gameObject.SetActive(false);

            }
        }
    }





    public void SendTorchNumber(int number)
    {
        photonView.RPC("SetTorchNumber", RpcTarget.Others, number);
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
            player.transform.GetChild(1).GetChild(7).gameObject.SetActive(true);
            gameManager.headParadise.nbPlayer++;
        }
        else
        {
            player.transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
            gameManager.headParadise.nbPlayer--;
        }
        
    }


    public void SendOpenDoor(int indexDoor, int x_room, int y_room, bool isExpedition, int indexRoomTeam)
    {
        photonView.RPC("SetOpenDoor", RpcTarget.All, indexDoor, x_room, y_room, isExpedition, indexRoomTeam);
    }

    [PunRPC]
    public void SetOpenDoor(int indexDoor, int x_room, int y_room, bool isExpedition, int indexRoomTeam)
    {
        if (!isExpedition)
        {
            Room roomInPlayer = gameManager.game.dungeon.GetRoomByPosition(x_room, y_room);
            roomInPlayer.door_isOpen[indexDoor] = true;

            Room roomTeam2 = gameManager.game.dungeon.GetRoomByIndex(indexRoomTeam);
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
                SendIsInJail(false, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
            }
        }
    }
    public void SendIsInJail(bool isInJail, int indexPlayer)
    {
        photonView.RPC("SetIsInJail", RpcTarget.All, isInJail, indexPlayer);
    }

    [PunRPC]
    public void SetIsInJail(bool isInJail, int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isInJail = isInJail;
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
    }


    public void SendIsChooseForExpedition(int indexPlayer)
    {
        photonView.RPC("SetisChooseForExpedition", RpcTarget.All,indexPlayer);
    }
    public void SetisChooseForExpedition(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isChooseForExpedition = false;
        this.transform.GetChild(1).GetChild(4).gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);
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
            //gameManager.game.currentRoom = gameManager.game.GetRoomById(indexCurrentRoom);
            //Debug.Log(gameManager.game.currentRoom.door_isOpen);
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
            //gameManager.GetPlayerMineGO().GetComponent<PlayerGO>(). = false;
            /*
                        Room roomInPlayer = gameManager.game.dungeon.GetRoomByPosition(gameManager.game.currentRoom.GetPos_X(), gameManager.game.currentRoom.GetPos_Y());
                        roomInPlayer.door_isOpen[indexDoor] = true;*/


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
        photonView.RPC("SetActiveZoneVoteChest", RpcTarget.All);
    }

    [PunRPC]
    public void SetActiveZoneVoteChest()
    {
        gameManager.voteChestHasProposed = true;
        gameManager.ui_Manager.ActiveZoneVoteChest(true);
        StartCoroutine(gameManager.LaunchTimerChest());
    }

    public void SendChestData(int indexRoom , int index, bool isAward, int indexAward)
    {
        photonView.RPC("SetChestData", RpcTarget.All, indexRoom,index, isAward, indexAward);
    }

    [PunRPC]
    public void SetChestData(int indexRoom, int index, bool isAward, int indexAward)
    {
        gameManager.game.dungeon.rooms[indexRoom].chestList.Add(Chest.CreateInstance(index, isAward, indexAward));
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
        }
        gameManager.ui_Manager.DisplayFireBallRoom(display);
        gameManager.game.currentRoom.speciallyPowerIsUsed = !display;
        gameManager.CloseAllDoor(gameManager.game.currentRoom, false);

       
    }

    public void SendLaunchFireBallRoom()
    {
        photonView.RPC("SetLaunchFireBallRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetLaunchFireBallRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach(GameObject turret in turrets)
        {
            turret.GetComponent<Turret>().LaunchTurret();
        }
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

    public void SendDisplayExplorationLever(bool display)
    {
        photonView.RPC("SetDisplayDoorLever", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayDoorLever(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
            return;
        gameManager.ui_Manager.DisplayLeverVoteDoor(display);
    }

    public void SendDisplayNuVoteSacrificeForAllPlayer()
    {
        photonView.RPC("SetDisplayNuVoteSacrificeForAllPlayer", RpcTarget.All);
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
        GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().sacrificeVoteIsLaunch = isLaunch;
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
                room.chest = true;
                for (int i = 0; i < 2; i++)
                {
                    if(room.chestList.Count > 0)
                        SendChestData(indexRoom, room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward);
                }
                break;
            case 1:
                room.fireBall = true;
                break;
            case 2:
                room.isSacrifice = true;
                break;
        }
        gameManager.game.dungeon.GetRoomByIndex(indexRoom).isSpecial = true;

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
            gameManager.GetHexagone(indexHexagone).Room.isJail = true;

        gameManager.GetHexagone(indexHexagone).Room.isSpecial = true;
    }

}
