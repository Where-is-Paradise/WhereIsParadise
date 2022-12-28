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
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.MaximumTransferUnit = 576;
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
        Door doorExplorate = gameManager.GetDoorExplorator(player.GetComponent<PhotonView>().ViewID);
        if(doorExplorate)
            gameManager.CancelDoorExplorationWhenDisconnection(doorExplorate.index);


        if (dataGame.GetPlayerByIndex(player.GetComponent<PhotonView>().ViewID).hasWinFireBallRoom)
            gameManager.ResetLeverDisconnect();
        if (gameManager.timer.timerLaunch)
            StartCoroutine(HideZoneCouroutine());
        //gameManager.game.NumberExpeditionAvailable(gameManager.setting.LIMITED_TORCH, 0);
        if (player.GetComponent<PlayerGO>().isBoss)
        {
            gameManager.ChangeBoss();
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
                Time.timeScale = 1;
                Debug.Log("Successful reconnected!", this);
            }
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Successful reconnected and joined!", this);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SendDataAllPlayer();
        SendDataRoom();
        SendGlobalData();
        gameManager.HidePlayerNotInSameRoom();
        StartCoroutine(CouroutineHidePlayerNoteSameRoom());
        StartCoroutine(CouroutineHidePlayerNoteSameRoomN2());
        StartCoroutine(AddPlayerInListCoroutine());
    }

    public IEnumerator AddPlayerInListCoroutine()
    {
        yield return new WaitForSeconds(2f);
        gameManager.SetTABToList(GameObject.FindGameObjectsWithTag("Player"), gameManager.listPlayer);
    }

    public IEnumerator CouroutineHidePlayerNoteSameRoom()
    {
        yield return new WaitForSeconds(1);
        gameManager.HidePlayerNotInSameRoom();
    }
    public IEnumerator CouroutineHidePlayerNoteSameRoomN2()
    {
        yield return new WaitForSeconds(2);
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
/*            PowerImpostor playerPowerImpostorTrap = player.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
            ObjectImpostor playerObjectImpostor = player.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();*/
            dataGame.SendDataOtherPlayers(id, playerGo.transform.position.x,
                playerGo.transform.position.y, playerGo.position_X, playerGo.position_Y,
                playerGo.isImpostor, playerGo.isBoss, playerGo.isSacrifice, playerGo.isInJail,
                playerGo.isInvisible, playerGo.indexSkin, playerGo.playerName, playerGo.hasWinFireBallRoom, 
                playerGo.GetComponent<PlayerNetwork>().userId);
        }
       
    }

    public void SendDataRoom()
    {
        foreach(Room room in gameManager.game.dungeon.rooms)
        {
            dataGame.SendRoomData(room.Index, room.speciallyPowerIsUsed);
        }
    }

    public void SendGlobalData()
    {
        dataGame.SendGlobalData(gameManager.labyrinthIsUsed,
            gameManager.NPCIsUsed, gameManager.PrayIsUsed,
            gameManager.ResurectionIsUsed, gameManager.PurificationIsUsed, gameManager.game.key_counter, gameManager.game.nbTorch);
    }


}
