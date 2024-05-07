using CI.QuickSave;
using Photon.Pun;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_Managment : MonoBehaviourPun
{

    public GameObject mainMenu_lobby;
    public GameObject mainMenuPlay;
    public GameObject createLobby_panel;
    public GameObject joinLobby_panel;
    public GameObject matchmakingPanel;

    public Lobby lobby;
    public Text code_in_input;

    public Text playerNuberMax_input;
    public Text paradiseDistanceMax_input;
    public Text playerName_input;
    public Text numberExpeditionMax_input;
    public Text numberOfAdditionnalKey_input;
    public Text numberOfAdditionnalTorch_input;

    public Toggle foggyRoom;
    public Toggle virusRoom;
    public Toggle randomRoomKeys;
    public Toggle hellRoom;
    public Toggle miniMap;
    public Toggle distanceT1;
    public Toggle displayObstacle;
    public Toggle displayKeyMap;
    public Toggle limitedTorch;

    public Toggle hellRoom2;


    public Text code_Text;
    public GameObject label_text;
    public InputField playerNameJoin_input;
    public InputField playerName_matchmaking;
    public GameObject labelSearch;

    public GameObject panelDoorMatchmaking;
    public GameObject panelDoorLocalGame;

    public Setting setting;
    public GameObject waitingMap;
    public GameObject buttonStartGame;
    public GameObject panelErrorCode;
    public GameObject panelErrorForm;

    public Text textNbPlayer;
    public Text textNbPlayerMatchmaking;

    public GameObject loading_page;

    public int current_indexButton_menu1 = 0;
    public List<GameObject> listButton_menu1;
    public GameObject firsInputFiled_menu2;
    public List<GameObject> listButton_menu2;
    private bool vertical_zero = false;
    
    public GameObject backgroundImage;
    public GameObject Canvas;
    public GameObject pannel_loadingConnection;

    public GameObject difficulty;
    public GameObject difficulty2;


    public VideoPlayer trailer;
    public GameObject presentation;
    public Text textIntro;

    private int index_menu;

    public bool canChange = false;
    public bool isLoadingConnection = false;
    public bool loadingCancel = false;


    public GameObject soundButton;

    public AudioSource menuAudio;

    public bool clickToLaunch = false;

    private bool backToMenu = false;

    public Text textTrailerSkip;

    public GameObject chatPanelInputParent;

    public int indexSkinUI = 0;

    public GameObject PanelAllSkin;
    public GameObject ArrowLeftSkin;
    public GameObject ArrowRightSkin;

    public GameObject readyButton;
    public GameObject launcGameInfoText;

    public GameObject openRoomButton;
    public GameObject settingButton;

    public GameObject backButtonInDoor;
    public GameObject backButtonInDoormatchmaking;

    public GameObject CanvasChatInput;
    public GameObject ButtonChatInputMobile;


    public GameObject video_settingButton;
    public GameObject controle_settingButton;

    public AudioSource launchChrono;

    public GameObject panelLanguageReset;
    public GameObject panelFirstConnexion;
    public GameObject panelWelcome;
    public GameObject panelTestSKIN_IP;

    public Version_http version;

    public List<ToogleSpeciallyRoom> list_toogle_speciallyRoom;
    public List<ToggleTrialRoom> list_toogle_trialRoom;
    public List<ToggleTeamTrial> list_toogle_teamTrialRoom;
    public List<ToggleTrapRoom> list_toogle_trapRoom;
    public List<ToogleObject> list_toogle_object;
    public List<ToggleObjectImpostor> list_toogle_objectImpostor;

    public GameObject timerMatchMaking;

    public GameObject timerMatchmaking_global;

    public GameObject error_version_panel;
    public GameObject error_connexion_panel;

    public GameObject publicPrivate_panel;
    public GameObject private_panel;

    float k = -1;
    // Start is called before the first frame update
    void Start()
    {

        //Debug.Log(SteamFriends.GetPersonaName());

   

        index_menu = 1;
        //ChangeColorButton(listButton_menu1, index_menu);
        if (GameObject.Find("BackToMenu"))
            backToMenu = true;
        StartCoroutine(TranslateDropdown());
        StartCoroutine(HideSkipTextTrailer());
        StartCoroutine(LaunchMenuMusic());
        //StartCoroutine(DisplayCoroutineWelcomePanel());
        StartCoroutine(DisplayFirstConnexionPanel());

        Debug.Log(SteamUser.GetSteamID());

    }

    // Update is called once per frame
    void Update()
    {

        // limité le nb de joueur

/*        if (PhotonNetwork.IsMasterClient && !lobby.matchmaking && buttonStartGame)
        {
            if ((lobby.nbPlayer > 3 && lobby.nbPlayer < 9) || GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerGO>().playerName == "Homertimes   ")
            {
                buttonStartGame.SetActive(true);
            }
            else
            {
                buttonStartGame.SetActive(false);
            }

            //buttonStartGame.SetActive(true);

        }*/

        // display StartButton when masterclient


        if (PhotonNetwork.IsMasterClient)
            DisplayStartButton(true);
        else
            DisplayStartButton(false);

        if (isLoadingConnection)
        {
            GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
            if(listPlayer.Length > 0)
            {
                //StartCoroutine(HideLoadingConnection());
                isLoadingConnection = false;
            }
        }

        if (trailer.isPaused || backToMenu)
        {
            HideTrailer();
           
        }

        if (clickToLaunch || backToMenu)
        {
            presentation.SetActive(false);
            backToMenu = false;
           
        }
        else
        {
            //presentation.SetActive(true);
        }

        k += (Time.deltaTime / 4f);
        AnimationClickIntro(k);
        if(k > 1)
        {
            k = 0;
        }

        CheckIndexSkinOutOfBounds();
        HideButtonForNoMobile();

        if (GameObject.Find("Setting"))
            setting = GameObject.Find("Setting").GetComponent<Setting>();
    }


    public void ZoomInMenuForMobile()
    {
#if UNITY_IOS || UNITY_ANDROID
        Camera.main.orthographicSize = 2.6f;
        Camera.main.transform.position = new Vector3(-0.22f, -1.71f , -10);
#endif
    }
    public void ResetZoomeMenuMobile()
    {
#if UNITY_IOS || UNITY_ANDROID
        Camera.main.orthographicSize = 5;
        Camera.main.transform.position = new Vector3(0, 0 , -10);
#endif
    }


    public void CursorBehaviorMenu( List<GameObject> listButton , int indexMenu)
    {
        if (Input.GetAxis("Vertical") < 0 && !vertical_zero  )
        {
            current_indexButton_menu1++;
            if (current_indexButton_menu1 == listButton.Count)
            {
                current_indexButton_menu1 = listButton.Count - 1;
            }
            ChangeColorButton(listButton, indexMenu);
            vertical_zero = true;   
        }
        if (Input.GetAxis("Vertical") > 0 && !vertical_zero )
        {
            current_indexButton_menu1--;
            if (current_indexButton_menu1 < 0)
            {
                current_indexButton_menu1 = 0;
            }
            ChangeColorButton(listButton, indexMenu);
            vertical_zero = true;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            vertical_zero = false;
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) )
        {
            PressButton(listButton, current_indexButton_menu1);
        }
    }

    public void  OnclickIntro()
    {
        clickToLaunch = true;
        if (setting.firstTimePanel)
        {
            panelFirstConnexion.gameObject.SetActive(true);
            setting.firstTimePanel = true;
        }
          
    }

    public void OnClickFirstConnexionPanel()
    {
        StartCoroutine(DisplayCoroutineIpPanel());
    }

    public IEnumerator DisplayCoroutineWelcomePanel()
    {
        yield return new WaitForSeconds(2);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        if (setting.welcome)
        {
            panelWelcome.SetActive(true);
        }
    }
    public IEnumerator DisplayCoroutineIpPanel()
    {
        yield return new WaitForSeconds(1f);
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        Debug.Log(setting.ip + " "  + (setting.ip == ""));
        if (setting.MODE_TEST_SKIN_IP && setting.ip == "")
        {
            panelTestSKIN_IP.SetActive(true);
        }
    }

    public void HideTrailer()
    {
        trailer.gameObject.SetActive(false);
        //presentation.SetActive(true);
        //menuAudio.Play();
        backgroundImage.SetActive(true);
        presentation.SetActive(false);
        Canvas.SetActive(true);
       
    }


    public void OnClickCreateLobbySettings()
    {
        mainMenu_lobby.SetActive(!mainMenu_lobby.activeSelf);
        createLobby_panel.SetActive(!createLobby_panel.activeSelf);
        firsInputFiled_menu2.GetComponent<InputField>().ActivateInputField();
        current_indexButton_menu1 = 0;
        index_menu = 2;
    }

    public void OnClickJoinLobbySetting()
    {
        mainMenu_lobby.SetActive(!mainMenu_lobby.activeSelf);
        joinLobby_panel.SetActive(!joinLobby_panel.activeSelf);
    }


    public void OnClickCreateLobby()
    {
        
        SetDifficulty(difficulty);
        lobby.CreateRoom(setting.NB_PLAYER_MAX , false);
        //setting.NUMBER_EXPEDTION_MAX = numberExpeditionMax;
/*        setting.FOGGY_ROOM = foggyRoom.isOn;
        setting.VIRUS_ROOM = virusRoom.isOn;
        setting.RANDOM_ROOM_ADDKEYS = randomRoomKeys.isOn;
        setting.HELL_ROOM = hellRoom.isOn;
        setting.DISPLAY_MINI_MAP = miniMap.isOn;
        setting.DISPLAY_DISTANCE_T1 = true;
        setting.DISPLAY_OBSTACLE_MAP = true;
        setting.DISPLAY_KEY_MAP = displayKeyMap.isOn;
        setting.LIMITED_TORCH = true;*/

        SetCodeText();
        DisplayLoadingConnection();
        //SendGameSettingSpciallyRoom();
        canChange = true;
    }

    public void SetSettingInWaitingRoom()
    {
        if (canChange)
        {
            SetDifficulty(difficulty2);
/*            setting.FOGGY_ROOM = foggyRoom2.isOn;
            setting.VIRUS_ROOM = virusRoom2.isOn;
            setting.RANDOM_ROOM_ADDKEYS = randomRoomKeys2.isOn;
            setting.HELL_ROOM = hellRoom2.isOn;
            setting.DISPLAY_MINI_MAP = miniMap2.isOn;
            setting.DISPLAY_DISTANCE_T1 = true;
            setting.DISPLAY_OBSTACLE_MAP = true;
            setting.DISPLAY_KEY_MAP = displayKeyMap2.isOn;
            setting.LIMITED_TORCH = true;*/
            //photonView.RPC("SendGameSetting", RpcTarget.Others, difficulty2.GetComponent<Dropdown>().value, foggyRoom2.isOn, virusRoom2.isOn, hellRoom2.isOn, randomRoomKeys2.isOn, miniMap2.isOn, displayKeyMap2.isOn);
        }
    }

    public void SetCanChange()
    {
        canChange = true;
    }

    public void SendGameSettingSpciallyRoom()
    {   
        for(int i =0; i< setting.listSpeciallyRoom.Count; i++)
        {
            photonView.RPC("SendGameSettingSpeciallyRoom",
           RpcTarget.Others, setting.listSpeciallyRoom[i], i);
        }
    }

    public void SendOnChangeGameSettingSpeciallyRoom(bool active , int index)
    {
        photonView.RPC("SendGameSettingSpeciallyRoom",
        RpcTarget.Others, active, index);
    }
    public void SendOnChangeGameSettingTrialsRoom(bool active, int index)
    {
        photonView.RPC("SendGameSettingTrialsRoom",
        RpcTarget.Others, active, index);
    }
    public void SendOnChangeGameSettingTeamTrialsRoom(bool active, int index)
    {
        photonView.RPC("SendGameSettingTeamTrialsRoom",
        RpcTarget.Others, active, index);
    }
    public void SendOnChangeGameSettingTrapRoom(bool active, int index)
    {
        photonView.RPC("SendGameSettingTrapRoom",
        RpcTarget.Others, active, index);
    }
    public void SendOnChangeGameSettingObject(bool active, int index)
    {
        photonView.RPC("SendGameSettingObject",
        RpcTarget.Others, active, index);
    }
    public void SendOnChangeGameSettingObjectImpostor(bool active, int index)
    {
        photonView.RPC("SendGameSettingObjectImpostor",
        RpcTarget.Others, active, index);
    }

    public void DisplayLoadingConnection()
    {
        pannel_loadingConnection.SetActive(true);
        isLoadingConnection = true;
    }

    public IEnumerator HideLoadingConnection()
    {
        float randomSecond = Random.Range(0.5f, 2.5f);
        yield return new WaitForSeconds(randomSecond);
        if (!lobby.disconnectByCancel)
        {
            pannel_loadingConnection.SetActive(false);
            /*        if(!loadingCancel)*/
            LauchWaitingRoom();
            SetCanChange();
        }
    

        loadingCancel = false;
    }

    public void OnClickLoadingCancel()
    {
        loadingCancel = true;
        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
/*        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(player);

        }*/
        
    }

    [PunRPC]
    public void SendGameSettingSpeciallyRoom(bool isActive , int index)
    {
        canChange = false;
        setting.listSpeciallyRoom[index] = isActive;
    }
    [PunRPC]
    public void SendGameSettingTrialsRoom(bool isActive, int index)
    {
        canChange = false;
        Debug.Log("sa passe " + isActive);
        setting.listTrialRoom[index] = isActive;
    }
    [PunRPC]
    public void SendGameSettingTeamTrialsRoom(bool isActive, int index)
    {
        canChange = false;
        setting.listTeamTrialRoom[index] = isActive;
    }

    [PunRPC]
    public void SendGameSettingTrapRoom(bool isActive, int index)
    {
        canChange = false;
        Debug.Log("sa passe " + index + " " + isActive);
        setting.listTrapRoom[index] = isActive;
    }
    [PunRPC]
    public void SendGameSettingObject(bool isActive, int index)
    {
        canChange = false;
        setting.listObject[index] = isActive;
    }
    [PunRPC]
    public void SendGameSettingObjectImpostor(bool isActive, int index)
    {
        canChange = false;
        setting.listObjectImpostor[index] = isActive;
    }

    
    public void ActivateAllFormSetting( bool activate)
    {
        difficulty2.GetComponent<Dropdown>().interactable = activate;
    }

    public void LauchWaitingRoom()
    {
        SetCodeText();
        createLobby_panel.SetActive(!createLobby_panel.activeSelf);
        backgroundImage.SetActive(false);
        Canvas.SetActive(false);
        waitingMap.SetActive(true);
        StartCoroutine(CoroutineDisplayChatInputMobile(true));
        if (lobby.matchmaking)
            DisplayMenuDoorMatchmaking(true);
        else
            DisplayMenuDoorLocalGame(true);
    }

   
    public void SetDifficulty(GameObject difficulty_var)
    {
        Dropdown difficulty_dp = difficulty_var.GetComponent<Dropdown>();
        SetDifficultyValue(difficulty_dp.value);
    }

    public void SetDifficultyValue(int value)
    {
        int distanceParadise = 0;
        if (value == 0)
        {
            distanceParadise = UnityEngine.Random.Range(5, 8);
        }
        if (value == 1)
        {
            distanceParadise = UnityEngine.Random.Range(7, 12);
        }

        setting.DISTANCE_EXIT_DOOR_MAX = distanceParadise;
    }

    public void OnClickJoinLobby()
    {
        string code = code_in_input.text;
        Debug.Log(code);
        lobby.ConnectToRoom(code);
        isLoadingConnection = true;
        pannel_loadingConnection.SetActive(true);
    }

    public void SetPlayerName(GameObject player)
    {
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        if (setting.MODE_TEST_SKIN_IP)
            player.GetComponent<PlayerGO>().SetPlayerName(setting.ip);
        else
            player.GetComponent<PlayerGO>().SetPlayerName(SteamFriends.GetPersonaName());
 
    }

    public void SetPlayerNameMatchmaking(GameObject player)
    {
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        if (setting.MODE_TEST_SKIN_IP)
            player.GetComponent<PlayerGO>().SetPlayerName(setting.ip);
        else
            player.GetComponent<PlayerGO>().SetPlayerName(SteamFriends.GetPersonaName());
    }

    public void SetSkin(GameObject player)
    {
        int indexSkin = UnityEngine.Random.Range(0, 9);
        player.GetComponent<PlayerGO>().SetSkin(indexSkin);
        player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).gameObject.SetActive(true);

    }

    public void SetCodeText()
    {
        code_Text.text = lobby.code;
    }

    public void OnClickStartGame()
    {
        photonView.RPC("SendDestroyBackWaitingRoom", RpcTarget.All);
        DisplayLoadingPage();
        SceneManager.LoadSceneAsync("Game");
    }


    public void CleanInputName(GameObject input)
    {
        input.GetComponent<InputField>().text = "";
    }

    public void DisplayErroCodeRoom()
    {
        panelErrorCode.SetActive(true);
        waitingMap.SetActive(false);
        backgroundImage.SetActive(true);
        Canvas.SetActive(true);
        joinLobby_panel.SetActive(true);
        createLobby_panel.SetActive(false);
        DisplayChatInputMobile(false);
    }
    public void OnClickHidePanelError()
    {
        panelErrorForm.SetActive(false);
    }

    public void OnClickBack(GameObject gameObject)
    {
        gameObject.SetActive(false);
        mainMenu_lobby.SetActive(true);
        index_menu = 1;
        canChange = false;

    }

    public void HidePanel(GameObject gameObject)
    {
        soundButton.GetComponent<AudioSource>().Play();
        gameObject.SetActive(false);
    }
    public void DisplayPanel(GameObject gameObject)
    {

        gameObject.SetActive(true);
    }

    public void InverseStateActive(GameObject gameObjectParam)
    {
        gameObjectParam.SetActive(!gameObjectParam.activeSelf);
    }

    public void OnClickOKpanelErrorForm(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public void SetNbPlayerUI(int nbPlayer , int nbPlayerMax)
    {
        textNbPlayer.text = nbPlayer + " / " + nbPlayerMax;
        textNbPlayerMatchmaking.text = textNbPlayer.text;
        if (nbPlayer < 3)
        {
            textNbPlayer.color = new Color(255, 0, 0);
            textNbPlayerMatchmaking.color = textNbPlayer.color;
        }
        else
        {
            textNbPlayer.color = new Color(255, 255, 255);
            textNbPlayerMatchmaking.color = textNbPlayer.color;
        }
    }

    public void DisplayLoadingPage()
    {
        loading_page.SetActive(true);
    }
    
    public void DisplayErrorPanel(string message)
    {
        if (!panelErrorForm.activeSelf)
        {
            panelErrorForm.SetActive(true);
            panelErrorForm.transform.GetChild(0).GetComponent<Text>().text = message;
        }
       
    }

    public void ChangeColorButton(List<GameObject> listButton, int indexMenu )
    {
        foreach (GameObject button in listButton)
        {
            if (indexMenu == 1 )
            {
                if (button.GetComponent<Button_controller>().index == current_indexButton_menu1)
                {
                    button.transform.GetChild(0).gameObject.SetActive(true);
                    button.transform.GetChild(1).gameObject.SetActive(true);
                    button.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    button.transform.GetChild(0).gameObject.SetActive(false);
                    button.transform.GetChild(1).gameObject.SetActive(false);
                    button.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
            else
            {
                if (button.GetComponent<Button_controller>().index == current_indexButton_menu1)
                {
                    button.transform.GetChild(0).gameObject.SetActive(true);
                    if (button.name.Equals("Strat_Button"))
                    {
                        button.transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
                else
                {
                    button.transform.GetChild(0).gameObject.SetActive(false);
                    if (button.name.Equals("Strat_Button"))
                    {
                        button.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
           
        }
    }

    public void PressButton( List<GameObject> listButton ,  int indexButton)
    {
        foreach (GameObject button in listButton)
        {
            if (button.GetComponent<Button_controller>().index == indexButton)
            {
                if (button.GetComponent<Button>())
                {
                    button.GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    if (button.GetComponent<Toggle>()){
                        button.GetComponent<Toggle>().SetIsOnWithoutNotify(!button.GetComponent<Toggle>().isOn);
                    }
                }
              
            }

        }
    }

    public void HideLoadingConnectionPanel()
    {
        pannel_loadingConnection.SetActive(false);
    }


    public void OnClickBackInWaitingRoom(bool deco)
    {
        //createLobby_panel.SetActive(true);
        mainMenu_lobby.SetActive(true);
        backgroundImage.SetActive(true);
        Canvas.SetActive(true);
        waitingMap.SetActive(false);
        mainMenuPlay.SetActive(false);
        createLobby_panel.SetActive(false);
        joinLobby_panel.SetActive(false);
        matchmakingPanel.SetActive(false);
        DisplayChatInputMobile(false);
        panelErrorCode.SetActive(false);
        lobby.matchmaking = false;
        canChange = false;
        if(!deco)
            PhotonNetwork.LeaveRoom();


/*        foreach (GameObject objectDonDesroy in this.gameObject.scene.GetRootGameObjects())
        {
            Destroy(objectDonDesroy);
        }
        Destroy(GameObject.Find("Setting"));
        Destroy(GameObject.Find("Input Manager"));
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");*/

    }


    public IEnumerator TranslateDropdown()
    {
        yield return new WaitForSeconds(2);
        setting = GameObject.Find("Setting").GetComponent<Setting>();
/*        string easyResult = "Easy";
        QuickSaveReader.Create(setting.langage)
                .Read<string>("menu_difficulty_easy", (r) => { easyResult = r; });*/
        string meadiumResult = "Medium";
        QuickSaveReader.Create(setting.langage)
                .Read<string>("menu_difficulty_medium", (r) => { meadiumResult = r; });
        string hardResult = "Hard";
        QuickSaveReader.Create(setting.langage)
                .Read<string>("menu_difficulty_hard", (r) => { hardResult = r; });


/*        Dropdown.OptionData easy = new Dropdown.OptionData();
        easy.text = easyResult;*/
        Dropdown.OptionData meadium = new Dropdown.OptionData();
        meadium.text = meadiumResult;
        Dropdown.OptionData hard = new Dropdown.OptionData();
        hard.text = hardResult;

        //difficulty.GetComponent<Dropdown>().options[0] = easy;
        difficulty.GetComponent<Dropdown>().options[0] = meadium;
        difficulty.GetComponent<Dropdown>().options[1] = hard;

        //difficulty2.GetComponent<Dropdown>().options[0] = easy;
        difficulty2.GetComponent<Dropdown>().options[0] = meadium;
        difficulty2.GetComponent<Dropdown>().options[1] = hard;


    }

    public IEnumerator HideSkipTextTrailer()
    {
        yield return new WaitForSeconds(4);
        textTrailerSkip.gameObject.SetActive(false);
    }


    public void AnimationClickIntro(float k)
    {
        textIntro.color = new Color(textIntro.color.r, textIntro.color.g, textIntro.color.b, k);
    }

    public void OnClickEnterChat()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = players[0];
        if (players.Length < 1)
        {
            return;
        }
        player.GetComponent<PlayerGO>().DisplayChat(false);
        player.GetComponent<PlayerGO>().GetPlayerMineGO().GetComponent<PlayerGO>().displayChatInput = false;

        player.GetComponent<PlayerGO>().SetTextChat(player.GetComponent<PlayerGO>().chatPanel.transform.Find("Panel_background").Find("Chat").GetComponent<InputField>().text);
    }
    public void OnClickBackChat()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = players[0];
        if (players.Length < 1)
        {
            return;
        }
        player.GetComponent<PlayerGO>().GetPlayerMineGO().GetComponent<PlayerGO>().DisplayChat(false);
    }

    public void CopyCode(Text textObj)
    {
        GUIUtility.systemCopyBuffer = textObj.text;
    }

    public void CheckIndexSkinOutOfBounds()
    {
        if(indexSkinUI == 0)
        {
            ArrowLeftSkin.SetActive(false);
            
        }
        else
        {
            ArrowLeftSkin.SetActive(true);
            if(indexSkinUI == PanelAllSkin.transform.childCount - 1)
            {
                ArrowRightSkin.SetActive(false);
            }
            else
            {
                ArrowRightSkin.SetActive(true);
            }
        }
    }

    public void SetLabelSearchPlayer(bool displaySearchPlayer)
    {
        if (!displaySearchPlayer)
        {
            label_text.SetActive(true);
            //code_Text.text = lobby.code;
            //label_text.GetComponent<Text>().text = "Code :";
            labelSearch.SetActive(false);
        }
        else
        {
            label_text.SetActive(false);
            labelSearch.SetActive(true);

        }
    }

    public void SetIndexSkin(bool isRight)
    {
        int oldIndexSkin = indexSkinUI;
        if (isRight)
        {
            indexSkinUI++;
        }
        else
        {
            indexSkinUI--;
        }

        UpdateSkin(oldIndexSkin);
    }
    public void UpdateSkin(int oldIndexSkin)
    {
        PanelAllSkin.transform.GetChild(oldIndexSkin).gameObject.SetActive(false);
        PanelAllSkin.transform.GetChild(indexSkinUI).gameObject.SetActive(true);
    }

    public void OnclickSavenNewIndexSkin()
    {
        setting.GetComponent<Setting>().INDEX_SKIN = indexSkinUI;
        lobby.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendindexSkin(indexSkinUI);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SendDisplayReadyButton(bool display)
    {

        photonView.RPC("DisplayReadyButton", RpcTarget.All, display);
    }

    [PunRPC]
    public void DisplayReadyButton(bool display)
    {
        readyButton.SetActive(display);
        launcGameInfoText.SetActive(!display);
    } 

    public void DisplayMenuDoorMatchmaking(bool display)
    {
        panelDoorLocalGame.SetActive(!display);
        panelDoorMatchmaking.SetActive(display);
    }
    public void DisplayMenuDoorLocalGame(bool display)
    {
        panelDoorMatchmaking.SetActive(!display);
        panelDoorLocalGame.SetActive(display);
    }

    public void DisplayReadyButtonOnly(bool display)
    {
        readyButton.SetActive(display);
    }

    public void DisplayOpenRoomButton(bool display)
    {
        openRoomButton.SetActive(display);
    }
    public void DisplayStartButton(bool display)
    {
        buttonStartGame.SetActive(display);
    }
/*    public void DisplayLaunchGameText()
    {
        DisplayLaunchGameText.SetActive(display);
    }*/

    public void DisplaySettingButton(bool display)
    {
        settingButton.SetActive(display);
    }

    public void DisabledBackButton()
    {
        backButtonInDoor.GetComponent<Button>().interactable = false;
        backButtonInDoormatchmaking.GetComponent<Button>().interactable = false;
    }

    public void OnClickChatButton()
    {
        lobby.GetPlayerMineGO().GetComponent<PlayerGO>().OnClickChat();
    }

    public void DisplayChatInputMobile(bool display)
    {

        CanvasChatInput.SetActive(display);
    }

    public IEnumerator CoroutineDisplayChatInputMobile(bool display)
    {
    
            yield return new WaitForSeconds(1.5f);

        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            display = false;
        }
        CanvasChatInput.SetActive(display);


    }

    public void CheckVersion()
    {
        //StartCoroutine(CheckVersionCoroutine());
    }

    public IEnumerator CheckVersionCoroutine()
    {
        yield return new WaitForSeconds(3f);

        if (!lobby.versionIsCorrect)
        {
            
            PhotonNetwork.Disconnect();
            OnClickBackInWaitingRoom(false);
            DisplayErrorPanel("Your game version is too old");
        }
    }

    public void HideButtonForNoMobile()
    {

#if !UNITY_IOS && !UNITY_ANDROID
        ButtonChatInputMobile.SetActive(false);
#endif
    }

   public void HideSettingForMobile()
    {
#if UNITY_IOS || UNITY_ANDROID
    video_settingButton.SetActive(false);
    controle_settingButton.SetActive(false);
#endif
    }


    public void SendSetting(string objectName, bool newValue)
    {
        photonView.RPC("SetNewSetting", RpcTarget.Others, objectName, newValue);
    }

    [PunRPC]
    public void SetNewSetting(string objectName, bool newValue)
    {
        if(GameObject.Find(objectName))
            GameObject.Find(objectName).GetComponent<Toggle>().isOn = newValue;
    }

    public void SendSettingDoubleChoice(string objectName,int index)
    {
        photonView.RPC("SetSettingDoubleChoice", RpcTarget.Others, objectName, index);
    }

    [PunRPC]
    public void SetSettingDoubleChoice(string objectName, int index)
    {
        if (!GameObject.Find(objectName))
            return;

        if (index == 0)
            GameObject.Find(objectName).GetComponent<ToggleChange>().OnClickToggle1();
        else
            GameObject.Find(objectName).GetComponent<ToggleChange>().OnClickToggle2();
    }

    public void SendKeyAdditionalSetting(int newNb)
    {
        photonView.RPC("SetKeyAdditionalSetting", RpcTarget.Others, newNb);
    }
    [PunRPC]
    public void SetKeyAdditionalSetting(int newNb)
    {
        setting.KEY_ADDITIONAL = newNb;
    }

    public void SendKeyTorchSetting(int newNb)
    {
        photonView.RPC("SetKeyTorchSetting", RpcTarget.Others, newNb);
    }
    [PunRPC]
    public void SetKeyTorchSetting(int newNb)
    {
        setting.TORCH_ADDITIONAL = newNb;
    }

    public void SendNBImpostorSetting(int newNb)
    {
        photonView.RPC("SetNBImpostorSetting", RpcTarget.Others, newNb);
    }
    [PunRPC]
    public void SetNBImpostorSetting(int newNb)
    {
        setting.NB_IMPOSTOR = newNb;
    }

    public void DisabledButton(Button button)
    {
        button.interactable = !button.interactable;
    }

    public void OnClickSettingGame()
    {
        foreach(ToogleSpeciallyRoom toogle in list_toogle_speciallyRoom)
        {
            toogle.GetComponent<ToogleSpeciallyRoom>().UpdateToggle();
        }
        foreach (ToggleTrialRoom toogle in list_toogle_trialRoom)
        {
            toogle.GetComponent<ToggleTrialRoom>().UpdateToggle();
        }
        foreach (ToggleTeamTrial toogle in list_toogle_teamTrialRoom)
        {
            toogle.GetComponent<ToggleTeamTrial>().UpdateToggle();
        }
        foreach (ToggleTrapRoom toogle in list_toogle_trapRoom)
        {
            toogle.GetComponent<ToggleTrapRoom>().UpdateToggle();
        }
        foreach (ToogleObject toogle in list_toogle_object)
        {
            toogle.GetComponent<ToogleObject>().UpdateToggle();
        }
        foreach (ToggleObjectImpostor toogle in list_toogle_objectImpostor)
        {
            toogle.GetComponent<ToggleObjectImpostor>().UpdateToggle();
        }
    }

    public IEnumerator LaunchMenuMusic()
    {
        yield return new WaitForSeconds(1);
        menuAudio.Play();
    }

    public IEnumerator DisplayFirstConnexionPanel()
    {
        yield return new WaitForSeconds(1);
        if (setting.firstTimePanel)
        {
            panelFirstConnexion.gameObject.SetActive(true);
            setting.firstTimePanel = true;
        }
    }

    

}
