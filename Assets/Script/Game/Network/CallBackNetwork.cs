using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CallBackNetwork : MonoBehaviourPunCallbacks
{

    public GameManager gameManager;
    public bool quit = false;

    public int maxReconnectionAttempts = 10;

    public SaveDataNetwork dataGame;

    // Le temps à attendre entre chaque tentative de reconnexion (en secondes)
    public float timeBetweenReconnectionAttempts = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        Photon.Realtime.Room room = PhotonNetwork.CurrentRoom;
        PhotonNetwork.CurrentRoom.EmptyRoomTtl = 600;
        //room.IsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        
        Debug.LogError(cause);
        dataGame.isDisconnect = true;
        if (cause.ToString() != "DisconnectByClientLogic")
        {
            gameManager.ui_Manager.DisplayReconnexionPanel(true);
            StartCoroutine(Reconnect());
          
        }
        else
        {
           
            //StartCoroutine(MainReconnect());
            BackToMenuScene();
            //StartCoroutine(MainReconnect());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GameObject player = gameManager.GetPlayerUserId(otherPlayer.UserId);
        SettingGameAfterPlayerDisconnect(player);
    }
    
    public void SettingGameAfterPlayerDisconnect(GameObject player)
    {
        if (!player)
        {
            return;
        }
        player.SetActive(false);
        gameManager.UpdateListPlayerGO();
        gameManager.SetTABToList(gameManager.listPlayerTab, gameManager.listPlayer);
        if (dataGame.GetPlayerByIndex(player.GetComponent<PhotonView>().ViewID).hasWinFireBallRoom)
            gameManager.ResetLeverDisconnect();
        if (gameManager.timer.timerLaunch)
            StartCoroutine(HideZoneCouroutine());

        if(PhotonNetwork.IsMasterClient)
            gameManager.RemovePlayerOfList(player);
        if (player.GetComponent<PlayerGO>().isBoss)
        {
            gameManager.canChangeBoss = true;
            gameManager.ChangeBossWithMasterClient();
        }

        if (PhotonNetwork.IsMasterClient && (gameManager.speciallyIsLaunch || gameManager.fireBallIsLaunch))
        {
            gameManager.TestLastPlayerSpeciallayRoom(player);
        }
    }

    public IEnumerator HideZoneCouroutine()
    {
        yield return new WaitForSeconds(7);
        gameManager.ui_Manager.HideZoneVote();

    }

    public void QuitLobby()
    {
        PhotonNetwork.Disconnect();
        //SceneManager.LoadScene("Menu");
    }


    public override void OnLeftRoom()
    {

        

        base.OnLeftRoom();
    }

    public void BackToMenuScene()
    {
        foreach (GameObject objectDonDesroy in this.gameObject.scene.GetRootGameObjects())
        {
            Destroy(objectDonDesroy);
        }
        //Destroy(GameObject.Find("Setting"));
        Destroy(GameObject.Find("Input Manager"));

        SceneManager.LoadScene("Menu");
    }



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log(" join room");
        dataGame.InstantiatePlayerMine();
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
        if (gameManager.ui_Manager.blackWallPaper.activeSelf)
            gameManager.ui_Manager.blackWallPaper.SetActive(false);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        Debug.Log(" Failed join room " + returnCode  + " " + gameManager.setting.oldCodeRoom + " " + message);
        if(returnCode == 32746)
        {
            StartCoroutine(JoinRoomCouroutine());
        }
    }

    public IEnumerator JoinRoomCouroutine()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.JoinRoom(gameManager.setting.oldCodeRoom);
    }

    public override void OnConnectedToMaster()
    {
        print("Reconnected");
        Debug.Log(PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinRoom(gameManager.setting.oldCodeRoom);
       
    }

    public IEnumerator Reconnect()
    {
        int reconnectionAttempts = 0;

        while (!PhotonNetwork.IsConnected && reconnectionAttempts < maxReconnectionAttempts)
        {
            // Afficher un message au joueur indiquant que la connexion a été perdue et que le jeu essaie de se reconnecter
            Debug.Log("Connexion perdue. Tentative de reconnexion en cours...");

            // Essayer de se connecter au serveur en utilisant la version du jeu et le code de région
            bool connectionSuccessful = PhotonNetwork.ConnectToRegion(gameManager.setting.region);
            
            // Si la connexion n'a pas réussi, attendre la durée spécifiée avant de réessayer
            if (!connectionSuccessful)
            {
                yield return new WaitForSeconds(timeBetweenReconnectionAttempts);
                reconnectionAttempts++;
            }
           
        }

        if (PhotonNetwork.IsConnected)
        {
            //Debug.Log(" is reconnected");
            yield break;
        }

        // Si le nombre maximum de tentatives a été atteint et que la connexion n'a pas réussi, afficher un message d'erreur
        Debug.LogError("Impossible de se reconnecter au serveur. Nombre maximum de tentatives atteint.");
    }

    private IEnumerator MainReconnect()
    {
        Debug.LogError(PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
        while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            Debug.Log("Waiting for client to be fully disconnected..", this);

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Client is disconnected!", this);

        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            if (PhotonNetwork.Reconnect())
            {
                Debug.Log("Successful reconnected!", this);
            }
        }
        else
        {
            Debug.Log("Successful reconnected and joined!", this);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SendDataAllPlayer();
        StartCoroutine(SendDataRoomCouroutine());
        StartCoroutine(SendGlobalDataCoroutine());
        StartCoroutine(SendDataSpeciallyRoom());
        gameManager.HidePlayerNotInSameRoom();
        StartCoroutine(AddPlayerInListCoroutine());
    }

    public IEnumerator AddPlayerInListCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        gameManager.SetTABToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayer);
        gameManager.SetTAB2ToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayerFinal);
        gameManager.TreePlayerList(gameManager.listPlayerFinal);
    }

    public IEnumerator CouroutineHidePlayerNoteSameRoom()
    {
        yield return new WaitForSeconds(2);
        gameManager.HidePlayerNotInSameRoom();
    }
    public IEnumerator CouroutineHidePlayerNoteSameRoomN2()
    {
        yield return new WaitForSeconds(6);
        gameManager.HidePlayerNotInSameRoom();
    }


    public void SendDataAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if (!player)
                continue;
            int id = player.GetComponent<PhotonView>().ViewID;
            PlayerGO playerGo = player.GetComponent<PlayerGO>();
            dataGame.SendDataOtherPlayers(id, playerGo.transform.position.x,
                playerGo.transform.position.y, playerGo.position_X, playerGo.position_Y,
                playerGo.isImpostor, playerGo.isBoss, playerGo.isSacrifice, playerGo.isInJail,
                playerGo.isInvisible, playerGo.indexSkin, playerGo.playerName, playerGo.hasWinFireBallRoom, 
                playerGo.GetComponent<PlayerNetwork>().userId);
        }
       
    }

    public IEnumerator SendDataRoomCouroutine()
    {
        yield return new WaitForSeconds(0.25f);
        int counter = 1;
        foreach (Room room in gameManager.game.dungeon.rooms)
        {
            dataGame.SendRoomData(room.Index, room.speciallyPowerIsUsed, counter == gameManager.game.dungeon.rooms.Count) ;
            dataGame.SendSpecialityRoom(room.Index, room.chest, room.isSacrifice, room.isJail, room.IsVirus, room.fireBall,
                room.IsFoggy, room.isDeathNPC, room.isSwordDamocles, room.isAx, room.isSword, room.isLostTorch, room.isMonsters, 
                room.isPurification, room.isResurection, room.isPray, room.isNPC, room.isLabyrintheHide, room.isCursedTrap, room.isTraped);
            dataGame.SendGlobalSpecialityRoom(room.Index, room.isTrial, room.isTeamTrial, room.isSpecial);
            

            // door open
            for (int i = 0; i < 6; i++)
            {
                dataGame.SendDoorOpenAllRoom(room.Index, room.door_isOpen[i], i);
            }
            counter++;
        }
    }

    public IEnumerator SendGlobalDataCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        dataGame.SendGlobalData(gameManager.labyrinthIsUsed,
            gameManager.NPCIsUsed, gameManager.PrayIsUsed,
            gameManager.ResurectionIsUsed, gameManager.PurificationIsUsed, gameManager.game.key_counter, gameManager.game.nbTorch, gameManager.speciallyIsLaunch, gameManager.indexPlayerPreviousExploration);
    }

    public IEnumerator SendDataSpeciallyRoom()
    {
        yield return new WaitForSeconds(0.25f);

        if(gameManager.game.currentRoom.isSwordDamocles && gameManager.speciallyIsLaunch)
        {
            DamoclesSwordRoom damoclesRoom = GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>();
            if (damoclesRoom.currentPlayer)
                dataGame.SendDataSpecialityRoom(damoclesRoom.currentPlayer.GetComponent<PhotonView>().ViewID, -1);
            else
                dataGame.SendDataSpecialityRoom(-1, -1);
        }
        else
        {
            if (gameManager.game.currentRoom.isLostTorch && gameManager.speciallyIsLaunch)
            {
                LostTorchRoom lostTorchRoom = GameObject.Find("LostTorchRoom").GetComponent<LostTorchRoom>();
                if (lostTorchRoom.lostTorch.currentPlayer)
                    dataGame.SendDataSpecialityRoom(-1, lostTorchRoom.lostTorch.currentPlayer.GetComponent<PhotonView>().ViewID);
                else
                    dataGame.SendDataSpecialityRoom(-1, -1);
            }
            else
            {
                dataGame.SendDataSpecialityRoom(-1, -1);
            }
        }
        
       
    }

}
