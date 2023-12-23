using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Steamworks;
using System;

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

    public bool openRoom = false;

    public bool versionIsCorrect = false;

    int index_skin = 0;

    public Setting setting;

    public bool isConnected= false;

    public string nearServer = "";

    public Settin_management settingManagement;
    // Use this  initialization
    void Start()
    {
        matchmaking = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.MaximumTransferUnit = 400;
        //ConnectToMaster();
        //index_skin = Random.Range(0, 7);
        index_skin = 0;
        setting = GameObject.Find("Setting").GetComponent<Setting>();
        setting.INDEX_SKIN = index_skin;
        versionIsCorrect = true;
        StartCoroutine(GetText());
        ConnectToMaster();
        //StartCoroutine(GetUserInfoRquest());
        //StartCoroutine(waittotes());

        //Debug.Log(SteamApps.GetCurrentGameLanguage());
        //StartCoroutine(TestRequestSkin());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogError(PhotonNetwork.CloudRegion);
    }



    public void ConnectToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //PhotonNetwork.LocalCleanPhotonView()
        //PhotonNetwork.MaxResendsBeforeDisconnect = 10;
        
        PhotonNetwork.OfflineMode = false;

        //PhotonNetwork.ConnectUsingSettings();
        //ConnectToMasterInSpecificRegion();
        if(setting.region != "")
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = setting.region;
        PhotonNetwork.ConnectUsingSettings();
       
        //ConnectToMasterInSpecificRegion();
    }


    public void ConnectToMasterInSpecificRegion()
    {
        PhotonNetwork.Disconnect();
        //PhotonNetwork.BestRegionSummaryInPreferences
        Debug.Log(setting.region);
        PhotonNetwork.ConnectToRegion(setting.region);
    }

    


    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://ec2-35-180-178-202.eu-west-3.compute.amazonaws.com/version");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            StartCoroutine(GetText());
        }
        else
        {
            // Show results as tex

            Debug.Log(www.downloadHandler.text);
            try
            {
                ListVersion newVersion = JsonUtility.FromJson<ListVersion>(www.downloadHandler.text);

                if (newVersion.response[0].major == setting.major && newVersion.response[0].minor == setting.minor && newVersion.response[0].revision == setting.revision)
                {
                    versionIsCorrect = true;
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e);
            }
           

          
            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }

   
        
    public override void OnConnectedToMaster()
    {
        print("Connected");
        Debug.LogError(PhotonNetwork.CloudRegion);
        if (setting.region == "")
        {
            settingManagement.SaveServerRegion(PhotonNetwork.CloudRegion);
        }
        isConnected = true;
        base.OnConnectedToMaster();
        
        if (GameObject.Find("Setting_backWaitingRoom"))
        {
            if (GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().isBackToWaitingRoom)
            {
                isBackToWaitingRoom = true;
                oldCode = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().codeRoom;
                setting.oldCodeRoom = oldCode;
                oldPlayerName = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().playerName;
                matchmaking = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().isMatchmaking;
                index_skin = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().indexSkin;
                setting.INDEX_SKIN_COLOR = GameObject.Find("Setting_backWaitingRoom").GetComponent<BackWaitingRoom>().indexSkinColor;
                setting.INDEX_SKIN = index_skin;
                CreateRoomBack();

            }
        }
        //Debug.LogError(PhotonNetwork.CloudRegion);
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError("Disconnected : " + cause.ToString());
        isConnected = false;
        if (cause.ToString().Equals("DnsExceptionOnConnect"))
        {
            //StartCoroutine(reconnect());
            ui_management.DisplayErrorPanel("Disconnection..");
        }
        else
        {
            matchmaking = false;
            // 
            ui_management.canChange = false;
            ui_management.DisplayErrorPanel("Disconnection..");
            //ConnectToMaster();
            StartCoroutine(Reconnect());
        }
        ui_management.OnClickBackInWaitingRoom(true);
    }

    public IEnumerator Reconnect()
    {
        Debug.LogError("Reconnexion..");
        ConnectToMaster();
        yield return new WaitForSeconds(2f);
        if (!PhotonNetwork.IsConnected)
            StartCoroutine(Reconnect());
    }

    public IEnumerator ReconnectCreateRoom(int maxPlayer, bool isVisible)
    {
        StartCoroutine(Reconnect());
        Debug.LogError("recreate room..");
        yield return new WaitForSeconds(3f);
        if (!PhotonNetwork.IsConnected)
            StartCoroutine(ReconnectCreateRoom(maxPlayer,isVisible));
        else
            if(!PhotonNetwork.InRoom)
                CreateRoom(maxPlayer, isVisible);
    }



    private IEnumerator MainReconnect()
    {
        while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            Debug.Log("Waiting for client to be fully disconnected..", this);

            yield return new WaitForSeconds(5f);
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


    public void Matchmaking()
    {
        matchmaking = true;
        PhotonNetwork.JoinRandomRoom();   
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom(8 , true);
    }

    public void CreateRoom(int maxPlayerParam, bool isVisible)
    {
        code = GenerateCodeRoom(5);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        maxPlayer = maxPlayerParam;
        if (matchmaking)
            maxPlayer = 6;

        bool createRoomSucces = PhotonNetwork.CreateRoom(code, new RoomOptions { MaxPlayers = (byte)maxPlayer, PublishUserId = true, IsVisible = isVisible });
        if(!createRoomSucces)
        {
            Debug.LogError("excpetion creation room ");
            StartCoroutine(ReconnectCreateRoom(maxPlayerParam , isVisible));
        }
       
        ui_management.SetNbPlayerUI(1, maxPlayer);
        code2 = GenerateCodeRoom(5);
        setting.codeRoom = code2;
        setting.oldCodeRoom = code;
    }

    public void CreateRoomBack()
    {
        maxPlayer = 8;
        if (matchmaking)
        {
            Matchmaking();
            ui_management.LauchWaitingRoom();
            ui_management.SetLabelSearchPlayer(matchmaking);
            Destroy(GameObject.Find("Setting_backWaitingRoom").gameObject);
            return;
        }
            
        PhotonNetwork.CreateRoom(oldCode, new RoomOptions { MaxPlayers = (byte)maxPlayer, PublishUserId = true, IsVisible = true });
        ui_management.SetNbPlayerUI(1, maxPlayer);
        ui_management.LauchWaitingRoom();
        //matchmaking = true;
        ui_management.SetLabelSearchPlayer(matchmaking);
        Destroy(GameObject.Find("Setting_backWaitingRoom").gameObject);
        code2 = GenerateCodeRoom(5);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        setting.codeRoom = code2;
        code = oldCode;
        ui_management.SetCodeText();
        //GetPlayerMineGO().GetComponent<PlayerNetwork>().SendNamePlayer(oldPlayerName);
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
        setting.oldCodeRoom = code;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("Error creation room  RIP ... " + message);
        Debug.Log(returnCode);
        if( returnCode == 32766 && isBackToWaitingRoom)
        {
            //matchmaking = true;
            ConnectToRoom(oldCode);
            if(GameObject.Find("Setting_backWaitingRoom"))
                Destroy(GameObject.Find("Setting_backWaitingRoom").gameObject);
        }
        else
        {
            Debug.Log(returnCode);
            ui_management.HideLoadingConnectionPanel();
            ui_management.DisplayErrorPanel(message);
            matchmaking = false;
            
        }
        //GetPlayerMineGO().GetComponent<PlayerNetwork>().SendNamePlayer(oldPlayerName);
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
        ui_management.SetLabelSearchPlayer(matchmaking);
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        nbPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        indexPlayer = PhotonNetwork.CurrentRoom.PlayerCount;

        print("join room  nbPlayer "+nbPlayer+ " indexPlayer : " + indexPlayer);
        GameObject newPlayer = SpawnPlayer(indexPlayer);


        

        newPlayer.GetComponent<PlayerNetwork>().SendindexSkin(index_skin);
        newPlayer.GetComponent<PlayerNetwork>().SendindexSkinColor(setting.INDEX_SKIN_COLOR, true);
        //
        GameObject.Find("Setting").GetComponent<Setting>().isMatchmaking = matchmaking;
        if (matchmaking)
        {
            ui_management.SetPlayerNameMatchmaking(newPlayer);
            ui_management.SetDifficultyValue(0);
        }
        else
        {
            ui_management.SetPlayerName(newPlayer);
            if (PhotonNetwork.IsMasterClient)
            {
                ui_management.DisplayStartButton(true);
            }
        }


        if (isBackToWaitingRoom)
            newPlayer.GetComponent<PlayerNetwork>().SendNamePlayer(oldPlayerName);


        ui_management.SetLabelSearchPlayer(matchmaking);
        ui_management.SetNbPlayerUI(nbPlayer, maxPlayer);
        //ui_management.SetSkin(newPlayer);
        newPlayer.GetComponent<PlayerGO>().SetPlayerNameServer();

        isBackToWaitingRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode);
        matchmaking = false;
        Debug.Log(isBackToWaitingRoom + " " + code);
        if (returnCode == 32764 && isBackToWaitingRoom)
        {
            ui_management.OnClickBackInWaitingRoom(false);
            return;
        }
        //ui_management.
        ui_management.DisplayErroCodeRoom();
        ui_management.HideLoadingConnectionPanel();
       
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
        {
            PlayerIsMine().GetComponent<PlayerNetwork>().SendindexSkin(PlayerIsMine().GetComponent<PlayerGO>().indexSkin);
            PlayerIsMine().GetComponent<PlayerNetwork>().SendindexSkinColor(PlayerIsMine().GetComponent<PlayerGO>().indexSkinColor, true);
        }
            

        //ui_management.SetGameSettingFirstTime();
        //ui_management.SetSettingInWaitingRoom();

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length+1 == 6 && matchmaking)
            {
                ui_management.SendDisplayReadyButton(false);
                ui_management.DisabledBackButton();
                StartCoroutine(CouroutineStartGame());
            }
        }
        GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIsReady(GetPlayerMineGO().GetComponent<PlayerGO>().isReady);

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
        StartCoroutine(CoroutineLoadLevel());
    }

    public IEnumerator CoroutineLoadLevel()
    {
        yield return new WaitForSeconds(3);
        GetPlayerMineGO().GetComponent<PlayerGO>().displayChatInput = false;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    [PunRPC]
    public void SetLoadingPage()
    {
        ResetAllPlayerReady();
        ui_management.DisplayLoadingPage();
    }

    private string GenerateCodeRoom(int sizeCode)
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        string code = "";
        for (int i = 0; i < sizeCode; i++)
        {
            char chara_code = glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
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
    public GameObject GetPlayerMineGO()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            //Debug.Log(player.GetComponent<PhotonView>().ViewID);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                //Debug.Log(player.GetComponent<PhotonView>().ViewID);
                return player;
            }
        }
        return null;
    }

    public GameObject GetPlayer(int indexPlayer)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().ViewID == indexPlayer)
            {
                return player;
            }
        }
        return null;
    }


    public void SetVisibleRoom()
    {
        openRoom = !openRoom;


        if(openRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            //button.transform.GetChild(0).GetComponent<Text>().text = "Close Room";
            ui_management.SetLabelSearchPlayer(true);
            return;
        }
        else
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            //button.transform.GetChild(0).GetComponent<Text>().text = "Open Room";
            ui_management.SetLabelSearchPlayer(false);
        }
        
    }

    public void OnClickPlayerReady()
    {
        bool mineIsReady = GetPlayerMineGO().GetComponent<PlayerGO>().isReady;
        photonView.RPC("SetIsReady", RpcTarget.All, !mineIsReady , GetPlayerMineGO().GetComponent<PhotonView>().ViewID);

    }

    public bool CheckAllPlayerAreReady()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().isReady)
            {
                return false;
            }
        }
        if (players.Length < 4)
        {
            return false;
        }
        return true;
    }

    [PunRPC]
    public void SetIsReady(bool isReady , int indexPlayer)
    {
        GetPlayer(indexPlayer).GetComponent<PlayerGO>().isReady = isReady;
        GetPlayer(indexPlayer).transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(isReady);
        if (CheckAllPlayerAreReady())
                StartCoroutine(CouroutineStartGame());
    }

    public IEnumerator CouroutineStartGame()
    {
        ui_management.DisplayReadyButton(false);
        ui_management.DisabledBackButton();
        ResetAllPlayerReady();
        //ui_management.launchChrono.Play();
        PhotonNetwork.CurrentRoom.IsVisible = false;
        

       yield return new WaitForSeconds(15);
        ResetAllPlayerReady();
        OnclikStartGame();
    }

    public void ResetAllPlayerReady()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerGO>().isReady = false;
            player.transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(false);
        }
    }


    


    public IEnumerator GetUserInfoRquest()
    {
        string apikey = "110CECAF8B4523084D352599DD2EFFA2";
        string steamId = ""+SteamUser.GetSteamID();
        UnityWebRequest userInfo = UnityWebRequest.Get("https://partner.steam-api.com/ISteamMicroTxnSandbox/GetUserInfo/v2/?key=" + apikey + "&appid=1746620&steamid=" + steamId + "&format=json");
        
        yield return userInfo.SendWebRequest();

        if (userInfo.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(userInfo.result);
            StartCoroutine(GetUserInfoRquest());
        }
        else
        {
            // Show results as tex
          
            //Debug.Log(userInfo.downloadHandler.text);

            //JsonUtility.FromJson(userInfo.downloadHandler.text);

            //{"response":{"result":"OK","params":{"state":"","country":"FR","currency":"EUR","status":"Trusted"}}}
            try
            {
                UserInfoResponse userInfoObject = JsonUtility.FromJson<UserInfoResponse>(userInfo.downloadHandler.text);

               
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

            //Debug.Log(ParserJson.ParseStringToJson(userInfo.downloadHandler.text, "status"));

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }


}
