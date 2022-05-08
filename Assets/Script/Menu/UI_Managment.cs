using CI.QuickSave;
using Photon.Pun;
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

    public Toggle foggyRoom2;
    public Toggle virusRoom2;
    public Toggle randomRoomKeys2;
    public Toggle hellRoom2;
    public Toggle miniMap2;
    public Toggle distanceT12;
    public Toggle displayObstacle2;
    public Toggle displayKeyMap2;
    public Toggle limitedTorch2;

    public Text code_Text;
    public GameObject label_text;
    public InputField playerNameJoin_input;
    public InputField playerName_matchmaking;
    public GameObject labelSearch;


    public Setting setting;
    public GameObject waitingMap;
    public GameObject buttonStartGame;
    public GameObject panelErrorCode;
    public GameObject panelErrorForm;

    public Text textNbPlayer;

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

    float k = -1;
    // Start is called before the first frame update
    void Start()
    {
        index_menu = 1;
        //ChangeColorButton(listButton_menu1, index_menu);
        if (GameObject.Find("BackToMenu"))
            backToMenu = true;
        StartCoroutine(TranslateDropdown());
        StartCoroutine(HideSkipTextTrailer());


    }

    // Update is called once per frame
    void Update()
    {
/*        if (PhotonNetwork.IsMasterClient)
        {
            if (buttonStartGame)
            {
*//*                if ((lobby.nbPlayer > 3 && lobby.nbPlayer < 9) || GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerGO>().playerName == "Homertimes   ")
                {
                    buttonStartGame.SetActive(true);
                }
                else
                {
                    buttonStartGame.SetActive(false);
                }*//*

                buttonStartGame.SetActive(true);

            }

            ActivateAllFormSetting(true);
            canChange = true;
        }
        else
        {
            buttonStartGame.SetActive(false);
            ActivateAllFormSetting(false);
            canChange = false;
        }*/
       

        if (isLoadingConnection)
        {
            GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
            if(listPlayer.Length > 0)
            {
                StartCoroutine(HideLoadingConnection());
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
            presentation.SetActive(true);
        }

        k += (Time.deltaTime / 4f);
        AnimationClickIntro(k);
        if(k > 1)
        {
            k = 0;
        }

        CheckIndexSkinOutOfBounds();

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
    }
    public void HideTrailer()
    {
        trailer.gameObject.SetActive(false);
        //presentation.SetActive(true);
        menuAudio.Play();
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
        setting.FOGGY_ROOM = foggyRoom.isOn;
        setting.VIRUS_ROOM = virusRoom.isOn;
        setting.RANDOM_ROOM_ADDKEYS = randomRoomKeys.isOn;
        setting.HELL_ROOM = hellRoom.isOn;
        setting.DISPLAY_MINI_MAP = miniMap.isOn;
        setting.DISPLAY_DISTANCE_T1 = true;
        setting.DISPLAY_OBSTACLE_MAP = true;
        setting.DISPLAY_KEY_MAP = displayKeyMap.isOn;
        setting.LIMITED_TORCH = true;
        SetCodeText();
        DisplayLoadingConnection();
        SetGameSettingFirstTime();
        canChange = true;
    }

    public void SetSettingInWaitingRoom()
    {
        if (canChange)
        {
            SetDifficulty(difficulty2);
            setting.FOGGY_ROOM = foggyRoom2.isOn;
            setting.VIRUS_ROOM = virusRoom2.isOn;
            setting.RANDOM_ROOM_ADDKEYS = randomRoomKeys2.isOn;
            setting.HELL_ROOM = hellRoom2.isOn;
            setting.DISPLAY_MINI_MAP = miniMap2.isOn;
            setting.DISPLAY_DISTANCE_T1 = true;
            setting.DISPLAY_OBSTACLE_MAP = true;
            setting.DISPLAY_KEY_MAP = displayKeyMap2.isOn;
            setting.LIMITED_TORCH = true;
            photonView.RPC("SendGameSetting", RpcTarget.Others, difficulty2.GetComponent<Dropdown>().value, foggyRoom2.isOn, virusRoom2.isOn, hellRoom2.isOn, randomRoomKeys2.isOn, miniMap2.isOn, displayKeyMap2.isOn);
        }
    }

    public void SetCanChange()
    {
        canChange = true;
    }

    public void SetGameSettingFirstTime()
    {
        difficulty2.GetComponent<Dropdown>().value = difficulty.GetComponent<Dropdown>().value;
        SetDifficulty(difficulty2);
        foggyRoom2.isOn = setting.FOGGY_ROOM;
        virusRoom2.isOn = setting.VIRUS_ROOM;
        randomRoomKeys2.isOn = setting.RANDOM_ROOM_ADDKEYS;
        hellRoom2.isOn = setting.HELL_ROOM;
        miniMap2.isOn = setting.DISPLAY_MINI_MAP;
        displayKeyMap2.isOn = setting.DISPLAY_KEY_MAP;

        photonView.RPC("SendGameSetting",
            RpcTarget.Others,
            difficulty2.GetComponent<Dropdown>().value,
            foggyRoom2.isOn ,
            virusRoom2.isOn,
            hellRoom2.isOn,
            randomRoomKeys2.isOn,
            miniMap2.isOn,
            displayKeyMap2.isOn);
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
        pannel_loadingConnection.SetActive(false);
        if(!loadingCancel)
            LauchWaitingRoom();
        SetCanChange();

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
    public void SendGameSetting(int difficulty , bool foggyRoom, bool virusRoom, bool hellRoom, bool randomRoomKey, bool map, bool displayKey)
    {
        canChange = false;
        difficulty2.GetComponent<Dropdown>().value = difficulty;
        foggyRoom2.isOn = foggyRoom;
        virusRoom2.isOn = virusRoom;
        randomRoomKeys2.isOn = randomRoomKey;
        hellRoom2.isOn = hellRoom;
        miniMap2.isOn = map;
        displayKeyMap2.isOn = displayKey;
    }

    public void ActivateAllFormSetting( bool activate)
    {
        difficulty2.GetComponent<Dropdown>().interactable = activate;
        foggyRoom2.interactable = activate;
        virusRoom2.interactable = activate;
        randomRoomKeys2.interactable = activate;
        hellRoom2.interactable = activate;
        miniMap2.interactable = activate;
        displayKeyMap2.interactable = activate;


    }

    public void LauchWaitingRoom()
    {
        SetCodeText();
        createLobby_panel.SetActive(!createLobby_panel.activeSelf);
        backgroundImage.SetActive(false);
        Canvas.SetActive(false);
        waitingMap.SetActive(true);
        
    }

   
    public void SetDifficulty(GameObject difficulty_var)
    {
        Dropdown difficulty_dp = difficulty_var.GetComponent<Dropdown>();

        int distanceParadise = 0;
        if(difficulty_dp.value == 0)
        {
            distanceParadise = UnityEngine.Random.Range(4, 8);
        }
        if(difficulty_dp.value == 1)
        {
            distanceParadise = UnityEngine.Random.Range(8, 12);
        }
/*        if(difficulty_dp.value == 2)
        {
            distanceParadise = UnityEngine.Random.Range(8, 11);
        }*/
        setting.DISTANCE_EXIT_DOOR_MAX = distanceParadise;
        //setting.TORCH_ADDITIONAL = randomTorch;
    }

    public void OnClickJoinLobby()
    {
        string code = code_in_input.text;

        lobby.ConnectToRoom(code);
        isLoadingConnection = true;
        pannel_loadingConnection.SetActive(true);

        /*        joinLobby_panel.SetActive(!joinLobby_panel.activeSelf);
                backgroundImage.SetActive(false);
                Canvas.SetActive(false);
                waitingMap.SetActive(true);*/
    }

    public void SetPlayerName(GameObject player)
    {
        
        if(playerName_input.text.Length == 0)
        {
            if(playerNameJoin_input.text.Length == 0)
            {
                player.GetComponent<PlayerGO>().SetPlayerName(lobby.oldPlayerName);
            }
            else
            {
                player.GetComponent<PlayerGO>().SetPlayerName(playerNameJoin_input.text);
            }
           
        }
        else
        {
            player.GetComponent<PlayerGO>().SetPlayerName(playerName_input.text);
        }
 
    }

    public void SetPlayerNameMatchmaking(GameObject player)
    {

        if (playerName_input.text.Length == 0)
        {
            player.GetComponent<PlayerGO>().SetPlayerName(playerName_matchmaking.text);
        }
        else
        {
            player.GetComponent<PlayerGO>().SetPlayerName(playerName_matchmaking.text);
        }
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


    public void DisplayErroCodeRoom()
    {
        panelErrorCode.SetActive(true);
        waitingMap.SetActive(false);
        backgroundImage.SetActive(true);
        Canvas.SetActive(true);
        joinLobby_panel.SetActive(true);
        createLobby_panel.SetActive(false);
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
        if(nbPlayer < 4)
        {
            textNbPlayer.color = new Color(255, 0, 0);
        }
        else
        {
            textNbPlayer.color = new Color(255, 255, 255);
        }
    }

    public void DisplayLoadingPage()
    {
        loading_page.SetActive(true);
    }
    
    public void DisplayErrorPanel(string message)
    {
        if (panelErrorForm)
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


    public void OnClickBackInWaitingRoom()
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
        lobby.matchmaking = false;
        canChange = false;
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
    public void DisplaySettingButton(bool display)
    {
        settingButton.SetActive(display);
    }

    public void DisabledBackButton()
    {
        backButtonInDoor.GetComponent<Button>().interactable = false;
    }

}
