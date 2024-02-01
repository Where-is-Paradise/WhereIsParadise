using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataNetwork : MonoBehaviourPunCallbacks
{
    public GameManager gameManager;

    public PlayerGO playerMine;
    public PlayerData playerDataMine;
    public List<PlayerData> listOtherPlayer;


    public int nbKey;
    public int nbTorch;
    public List<RoomData> listRoom;

    public bool labyrinthIsUsed;
    public bool npcIsUsed;
    public bool prayIsUsed;
    public bool resurectionIsUsed;
    public bool purificationIsUsed;
    public bool specialityIsLaunch;
    public int indexPreviousExploration;

    public bool isDisconnect = false;
    public bool hasRecuperateData = false;
    public bool hasRecuperateRoomData = false;
    public bool hasRecuperateRoomDataN2 = false;
    public bool hasRecuperateRoomDataN3 = false;
    public bool hasRecuperateRoomDataN4 = false;

    public SpeciallyRoomData speciallyRoomData;

    // Start is called before the first frame update
    void Start()
    {
        SetCreateDataPlayerMine(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        PlayerGO playerMineN2 = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
        PowerImpostor playerPowerImpostorTrap = playerMineN2.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
        ObjectImpostor playerObjectImpostor = playerMineN2.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
        SetDataPlayerMine(playerDataMine.viewId, playerMineN2.transform.position.x, playerMineN2.transform.position.y,
            playerMineN2.position_X, playerMineN2.position_Y, playerMineN2.isImpostor, playerMineN2.isBoss, playerMineN2.isSacrifice,
            playerMineN2.isInJail, playerMineN2.isInvisible, playerMineN2.indexSkin, playerMineN2.playerName, playerMineN2.hasWinFireBallRoom,
            playerMineN2.GetComponent<PlayerNetwork>().userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition, playerMineN2.indexSkinColor, playerMineN2.explorationPowerIsAvailable);

        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PhotonView>().IsMine)
                SetCreateDataOtherPlayers(player.GetComponent<PhotonView>().ViewID);
        }

        StartCoroutine(SetDataPlayerMineCouroutine());
      

        speciallyRoomData = SpeciallyRoomData.CreateInstance();
    }


    public IEnumerator SetDataPlayerMineCouroutine()
    {
        yield return new WaitForSeconds(3);
        if (!isDisconnect)
        {
            if (gameManager.GetPlayerMineGO())
            {
                PlayerGO playerMineN2 = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
                PowerImpostor playerPowerImpostorTrap = playerMineN2.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
                ObjectImpostor playerObjectImpostor = playerMineN2.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
                SetDataPlayerMine(playerDataMine.viewId, playerMineN2.transform.position.x, playerMineN2.transform.position.y,
                    playerMineN2.position_X, playerMineN2.position_Y, playerMineN2.isImpostor, playerMineN2.isBoss, playerMineN2.isSacrifice,
                    playerMineN2.isInJail, playerMineN2.isInvisible, playerMineN2.indexSkin, playerMineN2.playerName, playerMineN2.hasWinFireBallRoom,
                    playerMineN2.GetComponent<PlayerNetwork>().userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
                playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition, playerMineN2.indexSkinColor, playerMineN2.explorationPowerIsAvailable)  ;
            }
            GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in listPlayer)
            {

                PlayerGO otherplayer = player.GetComponent<PlayerGO>();
                if (!player.GetComponent<PhotonView>().IsMine)
                {
                    SetDataOtherPlayers(otherplayer.GetComponent<PhotonView>().ViewID, otherplayer.transform.position.x, otherplayer.transform.position.y,
                   otherplayer.position_X, otherplayer.position_Y, otherplayer.isImpostor, otherplayer.isBoss, otherplayer.isSacrifice,
                   otherplayer.isInJail, otherplayer.isInvisible, otherplayer.indexSkin, otherplayer.playerName, otherplayer.hasWinFireBallRoom,
                   otherplayer.GetComponent<PlayerNetwork>().userId, otherplayer.explorationPowerIsAvailable);
                }
            }
        }
       
        StartCoroutine(SetDataPlayerMineCouroutine());
    }

    public void SetDataPlayerById(int indexPlayer)
    {
        GameObject currentPlayer = gameManager.GetPlayer(indexPlayer);
        if(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID == currentPlayer.GetComponent<PhotonView>().ViewID)
        {
            PlayerGO playerMineN2 = currentPlayer.GetComponent<PlayerGO>();
            PowerImpostor playerPowerImpostorTrap = playerMineN2.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
            ObjectImpostor playerObjectImpostor = playerMineN2.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
            SetDataPlayerMine(playerDataMine.viewId, playerMineN2.transform.position.x, playerMineN2.transform.position.y,
                playerMineN2.position_X, playerMineN2.position_Y, playerMineN2.isImpostor, playerMineN2.isBoss, playerMineN2.isSacrifice,
                playerMineN2.isInJail, playerMineN2.isInvisible, playerMineN2.indexSkin, playerMineN2.playerName, playerMineN2.hasWinFireBallRoom,
                playerMineN2.GetComponent<PlayerNetwork>().userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition, playerMineN2.indexSkinColor, playerMineN2.explorationPowerIsAvailable );
        }
        else
        {
            PlayerGO otherplayer = currentPlayer.GetComponent<PlayerGO>();
            SetDataOtherPlayers(otherplayer.GetComponent<PhotonView>().ViewID, otherplayer.transform.position.x, otherplayer.transform.position.y,
             otherplayer.position_X, otherplayer.position_Y, otherplayer.isImpostor, otherplayer.isBoss, otherplayer.isSacrifice,
             otherplayer.isInJail, otherplayer.isInvisible, otherplayer.indexSkin, otherplayer.playerName, otherplayer.hasWinFireBallRoom,
             otherplayer.GetComponent<PlayerNetwork>().userId, otherplayer.explorationPowerIsAvailable);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( gameManager.GetPlayerMineGO() && isDisconnect && hasRecuperateData && hasRecuperateRoomData && hasRecuperateRoomDataN2 && hasRecuperateRoomDataN3 && hasRecuperateRoomDataN4)
        {

            SetMine();
            RecuperateDataReconnexion();
            gameManager.SetTABToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayer);
            gameManager.SetTAB2ToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayerFinal);
            gameManager.TreePlayerList(gameManager.listPlayerFinal);
            SetBoss();
            StartCoroutine(UpdateView());
            gameManager.HidePlayerNotInSameRoom();
            gameManager.CloseAllDoor(gameManager.game.currentRoom, false) ;
            isDisconnect = false;
            hasRecuperateData = false;
            hasRecuperateRoomData = false;
            hasRecuperateRoomDataN2 = false;
            hasRecuperateRoomDataN3 = false;
            hasRecuperateRoomDataN4 = false;
            gameManager.ui_Manager.DisplayReconnexionPanel(false);
            gameManager.gameManagerNetwork.SendHidePlayerNotSameRoom();
            StartCoroutine(SendPlayerMineGo());
            gameManager.gameManagerNetwork.SendActiveCollision(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
            StartCoroutine(VerificationSameRoomOfBoss());
            StartCoroutine(DesactivateSpeciality());
        }
    }

    public void SetBoss()
    {
        if (!gameManager.GetBoss())
        {
            gameManager.gameManagerNetwork.SendChangeBoss();
        }
    }

    public IEnumerator UpdateView()
    {
        yield return new WaitForSeconds(1);
        gameManager.GetBoss().GetComponent<PlayerNetwork>().SetDisplayCrown(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SetIndexSkin(playerDataMine.indexSkin);
        foreach(GameObject player in gameManager.GetAllImpostor())
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)    
                player.GetComponent<PlayerNetwork>().SendDisplayHorn(true);
        }
        gameManager.ui_Manager.SetNBKey();
        gameManager.ui_Manager.SetTorchNumber();

        foreach (PlayerData otherPlayerData in listOtherPlayer)
        {
            if (!otherPlayerData)
                continue;
            GameObject otherPlayerGO = gameManager.GetPlayer(otherPlayerData.viewId);
            if (otherPlayerGO)
                continue;
            PlayerGO otherPlayer = otherPlayerGO.GetComponent<PlayerGO>();
            otherPlayer.GetComponent<PlayerNetwork>().SendindexSkin(otherPlayerData.indexSkin);
        }

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition)
        {
            gameManager.UpdateSpecialsRooms(gameManager.game.currentRoom);
           
            gameManager.SetDoorNoneObstacle(gameManager.game.currentRoom);
            gameManager.SetDoorObstacle(gameManager.game.currentRoom);
            gameManager.CloseAllDoor(gameManager.game.currentRoom, false);
            gameManager.GetPlayerMine().SetIsInExpedition(false);
        }
    } 

    public void SetMine()
    {
        playerDataMine.Old_viewId = playerDataMine.viewId;
        playerDataMine.viewId = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
        photonView.RPC("SetViewId", RpcTarget.Others, playerDataMine.Old_viewId, playerDataMine.viewId);

    }

    public void InstantiatePlayerMine()
    {
        GameObject playerMineGO = PhotonNetwork.Instantiate("Player_GO", new Vector3(playerDataMine.positionMineX, playerDataMine.positionMineY, -1), Quaternion.identity);
        playerMine = playerMineGO.GetComponent<PlayerGO>();
        InitiateDataSpeciality();
    }

    public void RecuperatePlayerMine()
    {
        playerMine.position_X = playerDataMine.positionMineX_dungeon;
        playerMine.position_Y = playerDataMine.positionMineY_dungeon;      
        playerMine.canMove = true;
        playerMine.isImpostor = playerDataMine.isImpostor;
        playerMine.isBoss = playerDataMine.isBoss;
        playerMine.isSacrifice = playerDataMine.isSacrifice;
        playerMine.isInvisible = playerDataMine.isInvisible;
        playerMine.isInJail = playerDataMine.isInJail;
        playerMine.indexSkin = playerDataMine.indexSkin;
        playerMine.indexSkinColor = playerDataMine.indexSkinColor;
        playerMine.playerName = playerDataMine.namePlayer;
        playerMine.hasWinFireBallRoom = false;
        playerMine.explorationPowerIsAvailable = playerDataMine.explorationPowerIsAvailble;

        if (playerMine.isImpostor)
        {
            playerMine.transform.Find("PowerImpostor").gameObject.SetActive(true);
            playerMine.transform.Find("ImpostorObject").gameObject.SetActive(true);

            playerMine.transform.Find("PowerImpostor").GetComponent<PowerImpostor>().indexPower = playerDataMine.indexPowerTrap;
            playerMine.transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed = playerDataMine.powerIsUsed;
            playerMine.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().indexPower = playerDataMine.indexObject;
            playerMine.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().powerIsUsed = playerDataMine.objectIsUsed;

            playerMine.GetComponent<PlayerGO>().indexPower = playerDataMine.indexPowerTrap;
            playerMine.GetComponent<PlayerGO>().indexObjectPower = playerDataMine.indexObject;
        }
        playerMine.GetComponent<CapsuleCollider2D>().enabled = true;
        DontDestroyOnLoad(playerMine);
    }
    public IEnumerator SendPlayerMineGo()
    {
        yield return new WaitForSeconds(0.25f);
        playerMine.GetComponent<PlayerNetwork>().SendNamePlayer(playerDataMine.namePlayer);
        playerMine.GetComponent<PlayerNetwork>().SendindexSkin(playerDataMine.indexSkin);
        playerMine.GetComponent<PlayerNetwork>().SendindexSkinColor(playerDataMine.indexSkinColor, false);
        playerMine.GetComponent<PlayerNetwork>().SendGlobalVariabel(playerDataMine.isImpostor, playerDataMine.isSacrifice, playerDataMine.isInJail, playerDataMine.isInvisible, playerDataMine.explorationPowerIsAvailble);
        playerMine.GetComponent<PlayerNetwork>().SendDungeonPosition(playerDataMine.positionMineX_dungeon, playerDataMine.positionMineY_dungeon);
        playerMine.GetComponent<PlayerNetwork>().SendUserId(PhotonNetwork.LocalPlayer.UserId);
    }

    public void RecuperateOthersPlayer()
    {
        foreach (PlayerData otherPlayerData in listOtherPlayer)
        {
            // GameObject otherPlayerGO = PhotonNetwork.Instantiate("Player_GO", new Vector3(otherPlayerData.positionMineX, otherPlayerData.positionMineY, -1), Quaternion.identity);
            if (!otherPlayerData)
                return;
            GameObject otherPlayerGO  = gameManager.GetPlayer(otherPlayerData.viewId);
            if (!otherPlayerGO)
                return;
            PlayerGO otherPlayer = otherPlayerGO.GetComponent<PlayerGO>();
            otherPlayer.position_X = otherPlayerData.positionMineX_dungeon;
            otherPlayer.position_Y = otherPlayerData.positionMineY_dungeon;
            otherPlayer.canMove = true;
            otherPlayer.isImpostor = otherPlayerData.isImpostor;
            otherPlayer.isBoss = otherPlayerData.isBoss;
            otherPlayer.isSacrifice = otherPlayerData.isSacrifice;
            otherPlayer.isInvisible = otherPlayerData.isInvisible;
            otherPlayer.isInJail = otherPlayerData.isInJail;
            otherPlayer.indexSkin = otherPlayerData.indexSkin;
            otherPlayer.playerName = otherPlayerData.namePlayer;
            otherPlayer.hasWinFireBallRoom = false;
            otherPlayer.GetComponent<PlayerNetwork>().userId = otherPlayerData.userId;
            otherPlayer.explorationPowerIsAvailable = otherPlayerData.explorationPowerIsAvailble;
            DontDestroyOnLoad(otherPlayer);
        }

    }

    public void RecuperationRooms()
    {
        foreach(RoomData room in listRoom)
        {
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).speciallyPowerIsUsed = room.speciallyPowerIsUsed;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).chest = room.isChest;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isSacrifice = room.isSacrifice;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isJail = room.isJail;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).IsVirus = room.isVirus;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).fireBall = room.isFireball;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).IsFoggy = room.isFoggy;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isDeathNPC = room.isDeathNPC;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isSwordDamocles = room.isSwordDamocles;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isAx = room.isAx;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isSword = room.isSword;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isLostTorch = room.isLostTorch;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isMonsters = room.isMonsters;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isPurification = room.isPurification;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isResurection = room.isResurection;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isPray = room.isPray;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isNPC = room.isNPC;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isLabyrintheHide = room.isLabyrinthHide;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isCursedTrap = room.isCursedTrap;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isTraped = room.isTrap;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isTrial = room.isTrial;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isTeamTrial = room.isTrialTeam;
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).isSpecial = room.isSpecial;
            
        }
        RecuperationDoorOpenAllRoom();
        gameManager.labyrinthIsUsed = labyrinthIsUsed;
        gameManager.NPCIsUsed = npcIsUsed;
        gameManager.PrayIsUsed = prayIsUsed;
        gameManager.ResurectionIsUsed = resurectionIsUsed;
        gameManager.PurificationIsUsed = purificationIsUsed;
        gameManager.speciallyIsLaunch = specialityIsLaunch;
        gameManager.indexPlayerPreviousExploration = indexPreviousExploration;

    }

    public void RecuperationDoorOpenAllRoom()
    {
        foreach (RoomData room in listRoom)
        {
            for(int i =0; i < room.door_isOpen.Count; i++)
            {
                gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).door_isOpen[i] = room.door_isOpen[i];
            }
        }
    }

    public void RecuperationGlobalData()
    {
        gameManager.game.nbTorch = nbTorch;
        gameManager.game.key_counter = nbKey;
        gameManager.speciallyIsLaunch = specialityIsLaunch;
    }


    public void RecuperateDataReconnexion()
    {
        RecuperatePlayerMine();
        RecuperateOthersPlayer();
        RecuperationRooms();
        RecuperationGlobalData();
    }


    public void SetCreateDataPlayerMine(int viewId)
    {
        playerDataMine = PlayerData.CreateInstance(viewId);
    }
    public void SetCreateDataOtherPlayers(int viewId)
    {
        PlayerData playerDataOther = PlayerData.CreateInstance(viewId);
        listOtherPlayer.Add(playerDataOther);
    }

    public void SendDataPlayerMine(int viewId, float positionX, float positionY, int positionDungeonX,
    int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail, bool isInvisible, 
    int indexSkin, string namePlayer, bool hasWinFireBallRoom, string userId, int indexPowerTrap, bool powerIsUsed,
        int indexObject, bool objectIsUsed)
    {
        photonView.RPC("SetDataPlayerMine", RpcTarget.All, viewId, positionX, positionY, positionDungeonX,
            positionDungeonY, isImpostor, isBoss, isSacrifice, isInJail, isInvisible, indexSkin, namePlayer, hasWinFireBallRoom, userId, 
            indexPowerTrap, powerIsUsed, indexObject, objectIsUsed );
    }

    [PunRPC]
    public void SetDataPlayerMine(int viewId, float positionX, float positionY, int positionDungeonX,
        int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail,
        bool isInvisible, int indexSkin, string namePlayer, bool hasWinFireBallRoom, string userId, int indexPowerTrap, bool powerIsUsed,
        int indexObject, bool objectIsUsed, bool isInExpedition, int indexSkinColor, bool explorationPowerIsAvailable)
    {
        playerDataMine.positionMineX = positionX;
        playerDataMine.positionMineY = positionY;
        playerDataMine.positionMineX_dungeon = positionDungeonX;
        playerDataMine.positionMineY_dungeon = positionDungeonY;
        playerDataMine.isImpostor = isImpostor;
        playerDataMine.isBoss = false;
        playerDataMine.isSacrifice = isSacrifice;
        playerDataMine.isInJail = isInJail;
        playerDataMine.isInvisible = isInvisible;
        playerDataMine.indexSkin = indexSkin;
        playerDataMine.indexSkinColor = indexSkinColor;
        playerDataMine.namePlayer = namePlayer;
        playerDataMine.hasWinFireBallRoom = hasWinFireBallRoom;
        playerDataMine.userId = userId;
        playerDataMine.indexPowerTrap = indexPowerTrap;
        playerDataMine.powerIsUsed = powerIsUsed;
        playerDataMine.indexObject = indexObject;
        playerDataMine.objectIsUsed = objectIsUsed;
        playerDataMine.isInExpedition = isInExpedition;
        playerDataMine.explorationPowerIsAvailble = explorationPowerIsAvailable;

    }



    public void SendDataOtherPlayers(int viewId, float positionX, float positionY, int positionDungeonX,
    int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail, bool isInvisible, int indexSkin, 
    string namePlayer, bool hasWinFireBallRoom, string userId, bool explorationIsAvalaible)
    {
        photonView.RPC("SetDataOtherPlayers", RpcTarget.All, viewId, positionX, positionY, positionDungeonX,
            positionDungeonY, isImpostor, isBoss, isSacrifice, isInJail, isInvisible, indexSkin, namePlayer, hasWinFireBallRoom, userId, explorationIsAvalaible) ;
    }

    [PunRPC]
    public void SetDataOtherPlayers(int viewId, float positionX, float positionY, int positionDungeonX,
        int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail, bool isInvisible, int indexSkin, 
        string namePlayer, bool hasWinFireBallRoom, string userId, bool explorationIsAvalaible)
    {
        PlayerData otherPlayer = GetPlayerByIndex(viewId);
        if (!otherPlayer)
            return;
        otherPlayer.positionMineX = positionX;
        otherPlayer.positionMineY = positionY;
        otherPlayer.positionMineX_dungeon = positionDungeonX;
        otherPlayer.positionMineY_dungeon = positionDungeonY;
        otherPlayer.isImpostor = isImpostor;
        otherPlayer.isBoss = isBoss;
        otherPlayer.isSacrifice = isSacrifice;
        otherPlayer.isInJail = isInJail;
        otherPlayer.isInvisible = isInvisible;
        otherPlayer.indexSkin = indexSkin;
        otherPlayer.namePlayer = namePlayer;
        otherPlayer.hasWinFireBallRoom = hasWinFireBallRoom;
        otherPlayer.userId = userId;
        otherPlayer.explorationPowerIsAvailble = explorationIsAvalaible;
    }

    public void SendRoomData(int indexRoom, bool speciallyPowerIsUsed, bool isEnd)
    {
        photonView.RPC("SetRoomData", RpcTarget.All, indexRoom, speciallyPowerIsUsed, isEnd);
    }

    [PunRPC]
    public void SetRoomData(int indexRoom, bool speciallyPowerIsUsed, bool isEnd)
    {
        if(!GetRoomByIndex(indexRoom))
            listRoom.Add(RoomData.CreateInstance(indexRoom, speciallyPowerIsUsed));
        GetRoomByIndex(indexRoom).speciallyPowerIsUsed = speciallyPowerIsUsed;
        if (isEnd)
        {
            hasRecuperateRoomDataN2 = true;
        }
            
    }

    public void SendGlobalData(bool labyrinthIsUsed, bool npcIsUsed,
         bool prayIsUsed, bool resurectionIsUsed, bool purificationUsed , int nbKey , int nbTorch, bool specialityIsLaunch, int indexPreviousExploration)
    {
        photonView.RPC("SetGlobalData", RpcTarget.All, labyrinthIsUsed, npcIsUsed,
           prayIsUsed, resurectionIsUsed, purificationUsed, nbKey , nbTorch, specialityIsLaunch, indexPreviousExploration);
    }

    [PunRPC]
    public void SetGlobalData(bool labyrinthIsUsed, bool npcIsUsed,
         bool prayIsUsed, bool resurectionIsUsed, bool purificationIsUsed, int nbKey , int nbTorch, bool specialityIsLaunch, int indexPreviousExploration)
    {
        this.labyrinthIsUsed = labyrinthIsUsed;
        this.npcIsUsed = npcIsUsed;
        this.prayIsUsed = prayIsUsed;
        this.resurectionIsUsed = resurectionIsUsed;
        this.purificationIsUsed = purificationIsUsed;
        this.nbTorch = nbTorch;
        this.nbKey = nbKey;
        this.specialityIsLaunch = specialityIsLaunch;
        this.indexPreviousExploration = indexPreviousExploration;

        hasRecuperateData = true;
    }

    public void SendSpecialityRoom(int roomID, bool isChest, bool isSacrifice, bool isJail, bool isVirus, bool isFireball, bool isFoggy,
        bool isDeathNPC, bool isSwordDamocles, bool isAx, bool isSword, bool isLostTorch, bool isMonsters, bool isPurification,
        bool isResurection, bool isPray, bool isNPC, bool isLabyrinthHide, bool isCursedTrap, bool isTrap)   
    {
        photonView.RPC("SetSpecialityRoom", RpcTarget.All, roomID, isChest, isSacrifice,
          isJail, isVirus, isFireball, isFoggy, isDeathNPC, isSwordDamocles, isAx, isSword, isLostTorch, isMonsters, 
          isPurification, isResurection, isPray, isNPC, isLabyrinthHide, isCursedTrap, isTrap);
    }

    [PunRPC]
    public void SetSpecialityRoom(int roomID, bool isChest, bool isSacrifice, bool isJail, bool isVirus, bool isFireball, bool isFoggy,
    bool isDeathNPC, bool isSwordDamocles, bool isAx, bool isSword, bool isLostTorch, bool isMonsters, bool isPurification,
    bool isResurection, bool isPray, bool isNPC, bool isLabyrinthHide, bool isCursedTrap, bool isTrap)

    {
        RoomData room = GetRoomByIndex(roomID);
        room.isChest = isChest;
        room.isSacrifice = isSacrifice;
        room.isJail = isJail;
        room.isVirus = isVirus;
        room.isFireball = isFireball;
        room.isFoggy = isFoggy;
        room.isDeathNPC = isDeathNPC;
        room.isSwordDamocles = isSwordDamocles;
        room.isAx = isAx;
        room.isSword = isSword;
        room.isLostTorch = isLostTorch;
        room.isMonsters = isMonsters;
        room.isPurification = isPurification;
        room.isResurection = isResurection;
        room.isPray = isPray;
        room.isNPC = isNPC;
        room.isLabyrinthHide = isLabyrinthHide;
        room.isCursedTrap = isCursedTrap;
        room.isTrap = isTrap;

        hasRecuperateRoomData = true;

    }

    public void SendGlobalSpecialityRoom(int roomID, bool isTrial, bool isTrialTeam, bool isSpecial)
    {
        photonView.RPC("SetGlobalSpecialityRoom", RpcTarget.All, roomID, isTrial, isTrialTeam, isSpecial) ;
    }

    [PunRPC]
    public void SetGlobalSpecialityRoom(int roomID, bool isTrial, bool isTrialTeam, bool isSpecial)

    {
        RoomData room = GetRoomByIndex(roomID);
        room.isTrial = isTrial;
        room.isTrialTeam = isTrialTeam;
        room.isSpecial = isSpecial;
        hasRecuperateRoomDataN3 = true;
    }

    public void SendDataSpecialityRoom(int currentPlayer_damolcesSword, int currentPlayer_lostTorch)
    {
        photonView.RPC("SetDataSpecialityRoom", RpcTarget.All, currentPlayer_damolcesSword, currentPlayer_lostTorch);
    }

    [PunRPC]
    public void SetDataSpecialityRoom(int currentPlayer_damolcesSword, int currentPlayer_lostTorch)

    {
        speciallyRoomData.currentPlayer_damolcesSword = currentPlayer_damolcesSword;
        speciallyRoomData.currentPlayer_lostTorch = currentPlayer_lostTorch;
        hasRecuperateRoomDataN4 = true;

    }

    public void SendDoorOpenAllRoom(int roomID, bool value, int indexDoor)
    {
        photonView.RPC("SetDoorOpenAllRoom", RpcTarget.All, roomID, value, indexDoor);
    }

    [PunRPC]
    public void SetDoorOpenAllRoom(int roomId, bool value, int indexDoor)
    {
        RoomData room = GetRoomByIndex(roomId);
        room.door_isOpen[indexDoor] = value;
    }

    public RoomData GetRoomByIndex(int indexRoom)
    {
        foreach(RoomData room in listRoom)
        {
            if (room.indexRoom == indexRoom)
                return room;
        }
        return null;
    }
    public PlayerData GetPlayerByIndex(int indexPlayer)
    {
        foreach (PlayerData player in listOtherPlayer)
        {
            if (player.viewId == indexPlayer)
                return player;
        }
        return null;
    }

    [PunRPC]
    public void SetViewId(int indexPlayer, int newViewId)
    {
        GetPlayerByIndex(indexPlayer).viewId = newViewId;
    }

    public IEnumerator VerificationSameRoomOfBoss()
    {
        yield return new WaitForSeconds(0.3f);
        if (gameManager.speciallyIsLaunch)
        {
            if (!gameManager.SamePositionAtBoss())
            {
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendTeleportPlayerToSameRoomOfBoss();
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
            }
        }
    }

    public IEnumerator DesactivateSpeciality()
    {
        yield return new WaitForSeconds(0.4f);
        if (!gameManager.speciallyIsLaunch)
        {
            if (GameObject.Find("FireBallRoom"))
            {
                GameObject.Find("FireBallRoom").GetComponent<FireBallRoom>().DesactivateFireBallRoom();
                GameObject.Find("FireBallRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }    
            if (GameObject.Find("SwordRoom"))
            {
                GameObject.Find("SwordRoom").GetComponent<SwordRoom>().SetDesactivateRoom();
                GameObject.Find("SwordRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }
              
            if (GameObject.Find("DamoclesSwordRoom"))
            {
                GameObject.Find("DamolcesRoom").GetComponent<DamoclesSwordRoom>().DesactivateDamoclesSwordRoom();
                GameObject.Find("DamoclesSwordRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }
               
            if (GameObject.Find("DeathNPCRoom"))
            {
                GameObject.Find("DeathNPCRoom").GetComponent<DeathNpcRoom>().DesactivateRoom();
                GameObject.Find("DeathNPCRoom").GetComponent<DeathNpcRoom>().DesactivateNPC();
            }
            if (GameObject.Find("AxRoom"))
            {
                GameObject.Find("AxRoom").GetComponent<AxRoom>().DesactivateRoomChild();
                GameObject.Find("AxRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }
            if (GameObject.Find("MonstersRoom"))
            {
                GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>().DesactivateRoom();
                GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>().DesactivateRoomChild();
                GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>().ReactivateCurrentRoom();
                gameManager.ui_Manager.DisplayLeverVoteDoor(true);
            }
            if (GameObject.Find("LostTorchRoom"))
            {
                GameObject.Find("LostTorchRoom").GetComponent<LostTorchRoom>().DesactivateLostTorchRoom();
                GameObject.Find("LostTorchRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }
            if (GameObject.Find("LabyrinthHideRoom"))
            {
                GameObject.Find("LabyrinthHideRoom").GetComponent<LabyrinthRoom>().DesactivateRoomChild();
                GameObject.Find("LabyrinthHideRoom").GetComponent<TrialsRoom>().ReactivateCurrentRoom();
            }
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIstouchInTrial(false);
            gameManager.ui_Manager.HideAllLever();
            gameManager.game.currentRoom.speciallyPowerIsUsed = true;
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = true;
        }
        else
        {
            if (GameObject.Find("LabyrinthHideRoom"))
            {
                GameObject.Find("LabyrinthHideRoom").GetComponent<LabyrinthRoom>().SendChangeScalePlayer();
            }
            else
            {
                if (GameObject.Find("LostTorchRoom"))
                {
                    if (GameObject.Find("LostTorch"))
                    {
                        LostTorchRoom lostTorchRoom = GameObject.Find("LostTorchRoom").GetComponent<LostTorchRoom>();
                        lostTorchRoom.lostTorch.currentPlayer = gameManager.GetPlayer(speciallyRoomData.currentPlayer_lostTorch).GetComponent<PlayerGO>();
                        GameObject.Find("LostTorch").transform.parent = lostTorchRoom.lostTorch.currentPlayer.transform;
                        StartCoroutine(GameObject.Find("LostTorch").GetComponent<LostTorch>().CanChangePlayerCouroutineOnlyMine());
                    }
                }
                else
                {
                    if (GameObject.Find("DamoclesSwordRoom"))
                    {
                        DamoclesSwordRoom damoclesSwordRoom = GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>();
                        damoclesSwordRoom.currentPlayer = gameManager.GetPlayer(speciallyRoomData.currentPlayer_damolcesSword);
                        GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>().DesactivateDamoclesSwordRoom();
                    }

                    gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
                    gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
                    
                }
            }
            gameManager.game.currentRoom.speciallyPowerIsUsed = true;
            gameManager.ui_Manager.HideAllLever();
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canDisplayMap = true;
        }
    }

    public void InitiateDataSpeciality()
    {
        if (GameObject.Find("DamoclesSwordRoom"))
        {
            DamoclesSwordRoom damoclesSwordRoom = GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>();
            damoclesSwordRoom.canChangePlayer = false;
        }
        if (GameObject.Find("LostTorchRoom"))
        {
            LostTorchRoom lostTorchRoom = GameObject.Find("LostTorchRoom").GetComponent<LostTorchRoom>();
            lostTorchRoom.lostTorch.canChangePlayer = false;
        }
    }





}
