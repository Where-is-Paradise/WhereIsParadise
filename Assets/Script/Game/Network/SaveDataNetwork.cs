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

    public bool isDisconnect = false;
    public bool hasRecuperateData = false;
    public bool hasRecuperateRoomData = false;
    public bool hasRecuperateRoomDataN2 = false;

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
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition);

        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PhotonView>().IsMine)
                SetCreateDataOtherPlayers(player.GetComponent<PhotonView>().ViewID);
        }

        StartCoroutine(SetDataPlayerMineCouroutine());
    }


    public IEnumerator SetDataPlayerMineCouroutine()
    {
        yield return new WaitForSeconds(7);
      
        if (gameManager.GetPlayerMineGO())
        {
            PlayerGO playerMineN2 = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
            PowerImpostor playerPowerImpostorTrap = playerMineN2.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
            ObjectImpostor playerObjectImpostor = playerMineN2.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
            SetDataPlayerMine(playerDataMine.viewId, playerMineN2.transform.position.x, playerMineN2.transform.position.y,
                playerMineN2.position_X, playerMineN2.position_Y, playerMineN2.isImpostor, playerMineN2.isBoss, playerMineN2.isSacrifice,
                playerMineN2.isInJail, playerMineN2.isInvisible, playerMineN2.indexSkin, playerMineN2.playerName, playerMineN2.hasWinFireBallRoom,
                playerMineN2.GetComponent<PlayerNetwork>().userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition);
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
               otherplayer.GetComponent<PlayerNetwork>().userId);
            }
               
        }
        //StartCoroutine(SetDataPlayerMineCouroutine());
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
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition);
        }
        else
        {
            PlayerGO otherplayer = currentPlayer.GetComponent<PlayerGO>();
            SetDataOtherPlayers(otherplayer.GetComponent<PhotonView>().ViewID, otherplayer.transform.position.x, otherplayer.transform.position.y,
             otherplayer.position_X, otherplayer.position_Y, otherplayer.isImpostor, otherplayer.isBoss, otherplayer.isSacrifice,
             otherplayer.isInJail, otherplayer.isInvisible, otherplayer.indexSkin, otherplayer.playerName, otherplayer.hasWinFireBallRoom,
             otherplayer.GetComponent<PlayerNetwork>().userId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( gameManager.GetPlayerMineGO() && isDisconnect && hasRecuperateData && hasRecuperateRoomData && hasRecuperateRoomDataN2)
        {
            SetMine();
            RecuperateDataReconnexion();
            gameManager.SetTABToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayer);
            SetBoss();
            StartCoroutine(UpdateView());
            SendPlayerMineGo();
            gameManager.HidePlayerNotInSameRoom();
            gameManager.CloseAllDoor(gameManager.game.currentRoom, false) ;
            isDisconnect = false;
            hasRecuperateData = false;
            hasRecuperateRoomData = false;
            hasRecuperateRoomDataN2 = false;
            gameManager.ui_Manager.DisplayReconnexionPanel(false);
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
            GameObject otherPlayerGO = gameManager.GetPlayer(otherPlayerData.viewId);
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
        playerMine.playerName = playerDataMine.namePlayer;
        playerMine.hasWinFireBallRoom = false;

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

        DontDestroyOnLoad(playerMine);
    }
    public void SendPlayerMineGo()
    {
        playerMine.GetComponent<PlayerNetwork>().SendNamePlayer(playerDataMine.namePlayer);
        playerMine.GetComponent<PlayerNetwork>().SendindexSkin(playerDataMine.indexSkin);
        playerMine.GetComponent<PlayerNetwork>().SendGlobalVariabel(playerDataMine.isImpostor, playerDataMine.isSacrifice, playerDataMine.isInJail, playerDataMine.isInvisible);
        playerMine.GetComponent<PlayerNetwork>().SendDungeonPosition(playerDataMine.positionMineX_dungeon, playerDataMine.positionMineY_dungeon);
        playerMine.GetComponent<PlayerNetwork>().SendUserId(PhotonNetwork.LocalPlayer.UserId);
    }

    public void RecuperateOthersPlayer()
    {
        foreach (PlayerData otherPlayerData in listOtherPlayer)
        {
            // GameObject otherPlayerGO = PhotonNetwork.Instantiate("Player_GO", new Vector3(otherPlayerData.positionMineX, otherPlayerData.positionMineY, -1), Quaternion.identity);
            GameObject otherPlayerGO  = gameManager.GetPlayer(otherPlayerData.viewId);
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
        }
        gameManager.labyrinthIsUsed = labyrinthIsUsed;
        gameManager.NPCIsUsed = npcIsUsed;
        gameManager.PrayIsUsed = prayIsUsed;
        gameManager.ResurectionIsUsed = resurectionIsUsed;
        gameManager.PurificationIsUsed = purificationIsUsed;
    }

    public void RecuperationGlobalData()
    {
        gameManager.game.nbTorch = nbTorch;
        gameManager.game.key_counter = nbKey;
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
        int indexObject, bool objectIsUsed, bool isInExpedition)
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
        playerDataMine.namePlayer = namePlayer;
        playerDataMine.hasWinFireBallRoom = hasWinFireBallRoom;
        playerDataMine.userId = userId;
        playerDataMine.indexPowerTrap = indexPowerTrap;
        playerDataMine.powerIsUsed = powerIsUsed;
        playerDataMine.indexObject = indexObject;
        playerDataMine.objectIsUsed = objectIsUsed;
        playerDataMine.isInExpedition = isInExpedition;

    }



    public void SendDataOtherPlayers(int viewId, float positionX, float positionY, int positionDungeonX,
    int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail, bool isInvisible, int indexSkin, 
    string namePlayer, bool hasWinFireBallRoom, string userId)
    {
        photonView.RPC("SetDataOtherPlayers", RpcTarget.All, viewId, positionX, positionY, positionDungeonX,
            positionDungeonY, isImpostor, isBoss, isSacrifice, isInJail, isInvisible, indexSkin, namePlayer, hasWinFireBallRoom, userId) ;
    }

    [PunRPC]
    public void SetDataOtherPlayers(int viewId, float positionX, float positionY, int positionDungeonX,
        int positionDungeonY, bool isImpostor, bool isBoss, bool isSacrifice, bool isInJail, bool isInvisible, int indexSkin, 
        string namePlayer, bool hasWinFireBallRoom, string userId)
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
         bool prayIsUsed, bool resurectionIsUsed, bool purificationUsed , int nbKey , int nbTorch)
    {
        photonView.RPC("SetGlobalData", RpcTarget.All, labyrinthIsUsed, npcIsUsed,
           prayIsUsed, resurectionIsUsed, purificationUsed, nbKey , nbTorch);
    }

    [PunRPC]
    public void SetGlobalData(bool labyrinthIsUsed, bool npcIsUsed,
         bool prayIsUsed, bool resurectionIsUsed, bool purificationIsUsed, int nbKey , int nbTorch)
    {
        this.labyrinthIsUsed = labyrinthIsUsed;
        this.npcIsUsed = npcIsUsed;
        this.prayIsUsed = prayIsUsed;
        this.resurectionIsUsed = resurectionIsUsed;
        this.purificationIsUsed = purificationIsUsed;
        this.nbTorch = nbTorch;
        this.nbKey = nbKey;

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
}
