using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Lobby : MonoBehaviourPunCallbacks
{


    public int nbPlayer = 1;
    public int indexPlayer = 1;
    public int maxPlayer = 1;
    public GameObject playerInstantiate;

    public List<GameObject> players;

    public string code;
    public string code2;

    public UI_Managment ui_management;

    public bool matchmaking;

    public bool isBackToWaitingRoom = false;
    private string oldCode = "0";
    public string oldPlayerName = "";

    // Use this for initialization
    void Start()
    {
        matchmaking = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ConnectToMaster();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConnectToMaster()
    {

        setPhotonAppVersion();
        
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }

    private void setPhotonAppVersion() {
        PhotonNetwork.NetworkingClient.AppVersion = Application.version;
        PhotonNetwork.GameVersion = Application.version;
    }

    public override void OnConnectedToMaster()
    {
        print("Connected");
        base.OnConnectedToMaster();
        if (GameObject.Find("Setting_backWaitingRoom"))
        {
            if (GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().isBackToWaitingRoom)
            {
                isBackToWaitingRoom = true;
                oldCode = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().codeRoom;
                oldPlayerName = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().playerName;
                CreateRoomBack();

            }
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print("Disconnected : " + cause.ToString());
        matchmaking = false;
        ui_management.DisplayErrorPanel(cause.ToString());
        ui_management.canChange = false;

        ConnectToMaster();
    }

    public void Matchmaking()
    {
        matchmaking = true;
        PhotonNetwork.JoinRandomRoom();
      
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom(20 , true);
    }

    public void CreateRoom(int maxPlayerParam, bool isVisible)
    {
        code = GenerateCodeRoom(7);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        maxPlayer = maxPlayerParam;
        PhotonNetwork.CreateRoom(code, new RoomOptions { MaxPlayers = (byte) maxPlayer , PublishUserId = true, IsVisible = isVisible });
        ui_management.SetNbPlayerUI(1, maxPlayer);
        code2 = GenerateCodeRoom(7);
        setting.codeRoom = code2;
    }

    public void CreateRoomBack()
    {
        maxPlayer = 8;
        PhotonNetwork.CreateRoom(oldCode, new RoomOptions { MaxPlayers = (byte)maxPlayer, PublishUserId = true, IsVisible = false });
        ui_management.SetNbPlayerUI(1, maxPlayer);
        ui_management.LauchWaitingRoom();
        Destroy(GameObject.Find("Setting_backWaitingRoom").gameObject);
        code2 = GenerateCodeRoom(7);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        setting.codeRoom = code2;
        code = oldCode;
        ui_management.SetCodeText();
    }

    public void SendCode(string code , string code2)
    {
        photonView.RPC("SetCode", RpcTarget.Others, code, code2);
    }
    [PunRPC]
    public void SetCode(string code , string code2)
    {
        this.code2 = code2;
        this.code = code;
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        setting.codeRoom = code2;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Error creation room  RIP ... " + message);
        Debug.Log(returnCode);
        if(returnCode == 32766 && isBackToWaitingRoom)
        {
            ConnectToRoom(oldCode);
            if(GameObject.Find("Setting_backWaitingRoom"))
                Destroy(GameObject.Find("Setting_backWaitingRoom").gameObject);
        }
        else
        {
            ui_management.HideLoadingConnectionPanel();
            ui_management.DisplayErrorPanel(message);
            matchmaking = false;
            
        }
       
    }

    public void ConnectToRoom(string code)
    {
        print("Connecting to room...");
        if(code.Length == 0)
        {
            code = "error";
        }
        PhotonNetwork.JoinRoom(code);
        ui_management.LauchWaitingRoom();
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        nbPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        indexPlayer = PhotonNetwork.CurrentRoom.PlayerCount;

        print("join room  nbPlayer "+nbPlayer+ " indexPlayer : " + indexPlayer);
        GameObject newPlayer = SpawnPlayer(indexPlayer);


        int index_skin = Random.Range(0,7);

        newPlayer.GetComponent<PlayerNetwork>().SendindexSkin(index_skin);

        if (matchmaking)
        {
            ui_management.SetPlayerNameMatchmaking(newPlayer);
        }
        else
        {
            ui_management.SetPlayerName(newPlayer);
        }
        

        ui_management.SetNbPlayerUI(nbPlayer, maxPlayer);
        //ui_management.SetSkin(newPlayer);
        newPlayer.GetComponent<PlayerGO>().SetPlayerNameServer();
        

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        
        ui_management.DisplayErroCodeRoom();
        ui_management.HideLoadingConnectionPanel();
        matchmaking = false;
    }

    public GameObject SpawnPlayer( int indexPlayer)
    {
        GameObject newPlayer = PhotonNetwork.Instantiate("Player_GO", new Vector3(playerInstantiate.transform.position.x, (playerInstantiate.transform.position.y), -1), Quaternion.identity);
        newPlayer.name = "Player_GO_" + indexPlayer;
        return newPlayer;
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
        base.OnPlayerEnteredRoom(newPlayer);
        nbPlayer++;
        GameObject[] listplayer = GameObject.FindGameObjectsWithTag("Player");
        for (int i =0; i < listplayer.Length; i++)
        {
            listplayer[i].GetComponent<PlayerGO>().SetPlayerNameServer();
            
        }
        SendMaxPlayer(maxPlayer);
        ui_management.SetNbPlayerUI(nbPlayer, maxPlayer);
        SendCode(code,code2);
        if (PlayerIsMine())
            PlayerIsMine().GetComponent<PlayerNetwork>().SendindexSkin(PlayerIsMine().GetComponent<PlayerGO>().indexSkin);

        //ui_management.SetGameSettingFirstTime();
        ui_management.SetSettingInWaitingRoom();
    }



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        nbPlayer--;
        ui_management.SetNbPlayerUI(nbPlayer, maxPlayer);
    }

    public void SendMaxPlayer(int maxPlayer)
    {
        photonView.RPC("SetMaxPlayer", RpcTarget.Others, maxPlayer);
    }

    [PunRPC]
    public void SetMaxPlayer(int maxPlayer)
    {
        this.maxPlayer = maxPlayer;
        ui_management.SetNbPlayerUI(nbPlayer, maxPlayer);
    }

    public void OnclikStartGame()
    {
        ui_management.DisplayLoadingPage();
        photonView.RPC("SetLoadingPage", RpcTarget.Others);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    [PunRPC]
    public void SetLoadingPage()
    {
        ui_management.DisplayLoadingPage();
    }

    private string GenerateCodeRoom(int sizeCode)
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        string code = "";
        for (int i = 0; i < sizeCode; i++)
        {
            char chara_code = glyphs[Random.Range(0, glyphs.Length)];
            code += chara_code;
        }
        return code;
    }

    public GameObject PlayerIsMine()
    {
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                return player;
            }
        }
        return null;
    }

}
