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
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed);

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
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed);
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
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed);
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
        if ( gameManager.GetPlayerMineGO() && isDisconnect && hasRecuperateData)
        {
            SetMine();
            RecuperateDataReconnexion();
            gameManager.SetTABToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayer);
            UpdateView();
            SendPlayerMineGo();
            gameManager.HidePlayerNotInSameRoom();
            gameManager.CloseAllDoor(gameManager.game.currentRoom, false) ;
            isDisconnect = false;
            hasRecuperateData = false;
            gameManager.ui_Manager.DisplayReconnexionPanel(false);
        }
    }

    public void UpdateView()
    {
        gameManager.GetBoss().GetComponent<PlayerNetwork>().SetDisplayCrown(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SetIndexSkin(playerDataMine.indexSkin);
        foreach(GameObject player in gameManager.GetAllImpostor())
        {
            if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                return;
            player.GetComponent<PlayerNetwork>().SendDisplayHorn(true);
        }
    } 

    public void SetMine()
    {
        playerDataMine.viewId = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
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
            DontDestroyOnLoad(otherPlayer);
        }

    }

    public void RecuperationRooms()
    {
        foreach(RoomData room in listRoom)
        {
            gameManager.game.dungeon.GetRoomByIndex(room.indexRoom).speciallyPowerIsUsed = room.speciallyPowerIsUsed; 
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
        int indexObject, bool objectIsUsed)
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

    public void SendRoomData(int indexRoom, bool speciallyPowerIsUsed)
    {
        photonView.RPC("SetRoomData", RpcTarget.All, indexRoom, speciallyPowerIsUsed);
    }

    [PunRPC]
    public void SetRoomData(int indexRoom, bool speciallyPowerIsUsed)
    {
        if(!GetRoomByIndex(indexRoom))
            listRoom.Add(RoomData.CreateInstance(indexRoom, speciallyPowerIsUsed));
        GetRoomByIndex(indexRoom).speciallyPowerIsUsed = speciallyPowerIsUsed;
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
}
