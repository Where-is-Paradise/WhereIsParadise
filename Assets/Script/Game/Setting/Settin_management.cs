using Luminosity.IO;
using CI.QuickSave;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class Settin_management : MonoBehaviour
{

    private Setting setting;

    public GameObject input_manager;
    public List<Text> listTextInput;

    // Video
    public GameObject resolution;
    public GameObject fullscren;
    public GameObject serverRegion;
    public int current_index_fullscreenMode = 1;
    public List<GameObject> fullScreeMode;

    // Audio
    public Scrollbar globalVolum_scrollbar;
    public Scrollbar musicVolum_scrollbar;
    public GameObject mute;
/*    public List<AudioSource> allSound;*/
    public GameObject allSoundParent;
    public GameObject allMusicParent;

    public Text textVolume;
    public Text textGlobalVolume;

    //Language
    public GameObject language;

    private GameObject currentFormInput;
    private bool inputIsPress = false;

    public GameObject panelLanguageReset;
    public bool serverRegionIsUpdated = false;

    public GameObject resolutionFirstConnexion;
    public GameObject languageFistConnexion;

    public Text  formIp_value;

    // Start is called before the first frame update
    void Start()
    {
       
        //SaveLanguage();
        //Screen.SetResolution(1920, 1080,true);
        SetTextDropDownResolution(resolution.GetComponent<Dropdown>());
/*        if(resolutionFirstConnexion)
            SetTextDropDownResolution(resolutionFirstConnexion.GetComponent<Dropdown>());*/
        setting = GameObject.Find("Setting").GetComponent<Setting>();
        if(input_manager)
            DontDestroyOnLoad(input_manager);

               
#if !UNITY_IOS && !UNITY_ANDROID
        LoadAudio();
        LoadVideo();
#endif
        LoadLanguage();
        LoadTutorial();
        LoadWelcome();
        LoadTestSkinIP();

        //SetFullScreenModeListView();
        SetLanguageDropdown();

        SetGlobalVolume();
        SetMusicVolume();

        LoadServerRegion("");

        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && inputIsPress)
        {
            currentFormInput.GetComponent<Text>().text = e.keyCode + "";
            inputIsPress = false;
            SetInput(int.Parse(currentFormInput.transform.parent.name), e.keyCode);
        }
    }


    public void SetTextDropDownResolution(Dropdown m_Dropdown)
    {
        //Dropdown m_Dropdown = resolution.GetComponent<Dropdown>();
        Resolution[] resolutions = Screen.resolutions;
        List<string> resolution_string = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolution_string.Add(resolutions[i].ToString());
        }

        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(resolution_string);

     
    }

    public void SetFullScreenModeListView()
    {
        for (int i = 0; i < fullScreeMode.Count; i++)
        {
            if (i == setting.fullscreenMode)
            {
                fullScreeMode[i].SetActive(true);
            }
            else
            {
                fullScreeMode[i].SetActive(false);
            }
        }
    }

    public void OnClickApplySetResolution()
    {
        Dropdown resolution_dropdown = resolution.GetComponent<Dropdown>();
        int resolution_int = resolution_dropdown.value;
        Resolution[] resolutions = Screen.resolutions;
        setting.resolution_width_index = resolution_int;
        setting.resolution_height_index = resolution_int;
        setting.fullscreen = fullscren.transform.GetChild(0).GetComponent<Image>().enabled;
        Screen.SetResolution(resolutions[resolution_int].width, resolutions[resolution_int].height,
            setting.fullscreen);
        Camera.main.orthographicSize = 5.1f;
        //SetFullScreenMode(current_index_fullscreenMode);
        SaveVideo();
        
    }

    public void OnClickApplyLanguage()
    {
        if (setting.canUpdate)
            return;
        Dropdown language_dropdown = language.GetComponent<Dropdown>();
        int language_index = language_dropdown.value;
        setting.langage = setting.listLangage[language_index];
        SaveLanguage(setting.langage);

        setting.canUpdate = true;
        StartCoroutine(SetCanUpdateCouroutine());
        //StartCoroutine(CoroutineReset(1));
    }

    public void SetLanguageDropdown()
    {
        if (!language)
        {
            return;
        }
        Dropdown language_dropdown = language.GetComponent<Dropdown>();

        if( setting.langage == "en")
        {
            language_dropdown.value = 0;
        }
        else if (setting.langage == "fr")
        {
            language_dropdown.value = 1;
        }
        else
        {
            language_dropdown.value = 2;
        }
    }

    public IEnumerator CoroutineReset(float seconde)
    {
        panelLanguageReset.SetActive(true);
        yield return new WaitForSeconds(seconde);
        Destroy(GameObject.Find("Input Manager"));
        Destroy(GameObject.Find("Setting"));
        SceneManager.LoadScene("Menu");
    }

    public void SetMusicVolume()
    {
        for (int i = 0; i < allMusicParent.transform.childCount; i++)
        {
            allMusicParent.transform.GetChild(i).GetComponent<AudioSource>().volume = (musicVolum_scrollbar.value /5.2f );
            allMusicParent.transform.GetChild(i).GetComponent<AudioSource>().mute = mute.transform.GetChild(0).GetComponent<Image>().enabled;
        }
        int volume_int = (int) (musicVolum_scrollbar.value * 100);
        textVolume.text = volume_int  + "";
        setting.volume_music = musicVolum_scrollbar.value;
        SaveAudio();
    }


    public void SetGlobalVolume()
    {

        for(int i = 0; i < allSoundParent.transform.childCount; i++)
        {
            allSoundParent.transform.GetChild(i).GetComponent<AudioSource>().volume = (globalVolum_scrollbar.value / 5.2f);
            allSoundParent.transform.GetChild(i).GetComponent<AudioSource>().mute = mute.transform.GetChild(0).GetComponent<Image>().enabled;
        }

        int volume_int = (int)(globalVolum_scrollbar.value  * 100);
        textGlobalVolume.text = volume_int + "";
        setting.volume_global = globalVolum_scrollbar.value;
        SaveAudio();
    }

    public void OnclickMute()
    {
        mute.transform.GetChild(0).GetComponent<Image>().enabled = !mute.transform.GetChild(0).GetComponent<Image>().enabled;
        setting.mute = !setting.mute;
    }

    public void OnclickFullscreen()
    {
        fullscren.transform.GetChild(0).GetComponent<Image>().enabled = !fullscren.transform.GetChild(0).GetComponent<Image>().enabled;
        //Screen.fullScreen = !Screen.fullScreen;
    }


    public void OnClickSetInput(GameObject form)
    {
        currentFormInput = form;
        inputIsPress = true;
    }

    public void SetInput(int indexInput , KeyCode newInput)
    {
        switch (indexInput)
        {
            case 0: 
                setting.INPUT_MOVE_FORWARD = newInput;
                InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Positive = newInput;
                InputManager.Save();
                
                break;
            case 1:
                setting.INPUT_MOVE_BACKWARD = newInput;
                InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Negative = newInput;
                InputManager.Save();
                break;
            case 2:
                setting.INPUT_MOVE_LEFT = newInput;
                InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Negative = newInput;
                InputManager.Save();
                break;
            case 3:
                setting.INPUT_MOVE_RIGHT = newInput;
                InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Positive = newInput;
                InputManager.Save();
                break;
            case 4:
                setting.INPUT_LAUCNH_EXPLORATION = newInput;
                InputManager.PlayerOneControlScheme.Actions[2].Bindings[0].Positive = newInput;
                InputManager.Save();
                break;
            case 5:
                setting.INPUT_LAUNCH_VOTE_DOOR = newInput;
                InputManager.PlayerOneControlScheme.Actions[3].Bindings[0].Positive = newInput;
                InputManager.Save();
                break;
            case 6:
                setting.INPUT_DISPLAY_MAP = newInput;
                InputManager.PlayerOneControlScheme.Actions[4].Bindings[0].Positive = newInput;
                InputManager.Save();
                break;
        }
    }

    public void DisplayInputTextInEachPanel()
    {
        for(int i =0; i < 7; i++)
        {
            switch (listTextInput[i].transform.parent.name)
            {
                case "0":
                    listTextInput[i].text = setting.INPUT_MOVE_FORWARD.ToString();
                    break;
                case "1":
                    listTextInput[i].text = setting.INPUT_MOVE_BACKWARD.ToString();
                    break;
                case "2":
                    listTextInput[i].text = setting.INPUT_MOVE_LEFT.ToString();
                    break;
                case "3":
                    listTextInput[i].text = setting.INPUT_MOVE_RIGHT.ToString();
                    break;
                case "4":
                    listTextInput[i].text = setting.INPUT_LAUCNH_EXPLORATION.ToString();
                    break;
                case "5":
                    listTextInput[i].text = setting.INPUT_LAUNCH_VOTE_DOOR.ToString();
                    break;
                case "6":
                    listTextInput[i].text = setting.INPUT_DISPLAY_MAP.ToString();
                    break;
            }   
        }
    }




    public void SaveAudio()
    {
        var writer = QuickSaveWriter.Create("Audio");
        writer.Write("volume_global", setting.volume_global);
        writer.Write("volume_music", setting.volume_music);
        writer.Write("Mute",  setting.mute);
        writer.Commit();

      QuickSaveRaw.LoadString("Audio.json");
    }

    public void LoadAudio()
    {
        try
        {
            QuickSaveReader.Create("Audio")
                     .Read<float>("volume_global", (r) => { setting.volume_global = r; })
                     .Read<float>("volume_music", (r) => { setting.volume_music = r; })
                     .Read<bool>("Mute", (r) => { setting.mute = r; });
        }
        catch (Exception e)
        {   
            SaveAudio();
            setting.mute = false;
        }
        for(int i = 0; i < allMusicParent.transform.childCount; i++)
        {
            AudioSource music = allMusicParent.transform.GetChild(i).GetComponent<AudioSource>();
            music.volume = (setting.volume_music);
            musicVolum_scrollbar.value = music.volume;
            int volume_int = (int)(music.volume * 100);
            textVolume.text = volume_int + "";
            music.mute = setting.mute;
        }
        for(int i =0; i< allSoundParent.transform.childCount; i++)
        {
            AudioSource sound = allSoundParent.transform.GetChild(i).GetComponent<AudioSource>();
            sound.volume = setting.volume_global;
            globalVolum_scrollbar.value = sound.volume;
            sound.mute = setting.mute;
        }
  
        int volume_int2 = (int)(setting.volume_global * 100);
        textGlobalVolume.text = volume_int2 + "";
        mute.transform.GetChild(0).GetComponent<Image>().enabled = setting.mute;
      
    }


    public void SaveVideo()
    {

        QuickSaveWriter.Create("Video")
                        .Write("Index_width", setting.resolution_width_index)
                        .Write("Index_height", setting.resolution_height_index)
                        .Write("Fullscreen", setting.fullscreen)
                        .Write("FullscreenMode" , setting.fullscreenMode)
                        .Commit();

    }

    public void GetIndexWidthAndHeightResolution()
    {
        for (int i = 0;  i < Screen.resolutions.Length; i++)
        {
            if(Screen.resolutions[i].width == Screen.width && Screen.resolutions[i].height == Screen.height)
            {
                setting.resolution_width_index = i;
                setting.resolution_height_index = i;
                setting.fullscreen = Screen.fullScreen;
            }
        }
    }

    public void LoadVideo()
    {
        try
        {

            QuickSaveReader.Create("Video")
                           .Read<int>("Index_width", (r) => { setting.resolution_width_index = r; })
                           .Read<int>("Index_height", (r) => { setting.resolution_height_index = r; })
                           .Read<bool>("Fullscreen", (r) => { setting.fullscreen = r; })
                           .Read<int>("FullscreenMode", (r) => { setting.fullscreenMode = r; }) ;
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[setting.resolution_width_index].width, resolutions[setting.resolution_height_index].height, setting.fullscreen);
        } catch (Exception e)
        {
            GetIndexWidthAndHeightResolution();
            SaveVideo();
        }   
        fullscren.transform.GetChild(0).GetComponent<Image>().enabled = setting.fullscreen;
        Dropdown resolution_dropdown = resolution.GetComponent<Dropdown>();
        resolution_dropdown.value = setting.resolution_width_index;
        current_index_fullscreenMode = setting.fullscreenMode;
    }

    public void SaveLanguage(string index)
    {
        QuickSaveWriter.Create("language")
                        .Write("index_language", index)
                        .Commit();

        QuickSaveRaw.LoadString("language.json");
    }
    public void LoadLanguage()
    {
        string indexLanguage = "en";
        try
        {
            QuickSaveReader.Create("language")
                      .Read<string>("index_language", (r) => { indexLanguage = r; });
        }catch(Exception e)
        {
            SaveLanguage("en");
        }

        setting.langage = indexLanguage;

        CopyFilLanguage();
    }

    public void CopyFilLanguage()
    {
        if (setting)
        {  
           
            foreach(string language in setting.listLangage)
            {
                TextAsset jsonTextFile = Resources.Load<TextAsset>(language);
                System.IO.File.WriteAllText(Path.Combine(Application.persistentDataPath, "QuickSave/" + language + ".json"), jsonTextFile.text);
            }
           
        } 
    }

    public void LoadTutorial()
    {
        bool displayTutorial = true;
        bool tutorialImpostor = true;
        bool firstTimePanel = true;
        try
        {
            QuickSaveReader.Create("tutorial")
                      //.Read<bool>("display_tutorial", (r) => { displayTutorial = r; })
                      .Read<bool>("tutorial_impostor", (r) => { tutorialImpostor = r; })
                      .Read<bool>("first_time_panel", (r) => { firstTimePanel = r; })
                      .Read<bool>("display_tutorial_V2", (r) => { displayTutorial = r; }); ;
        }
        catch (Exception e)
        {
            SaveTutorial(true, true, true);
        }

        setting.displayTutorial = displayTutorial;
        setting.tutorialImpostor = tutorialImpostor;
        setting.firstTimePanel = firstTimePanel;
    }

    

    public void SaveTutorial(bool displayTutorial , bool tutorialImpostor, bool firstTimePanel)
    {
        QuickSaveWriter.Create("tutorial")
                        .Write("display_tutorial", true)
                        .Write("tutorial_impostor", tutorialImpostor)
                        .Write("first_time_panel", firstTimePanel)
                        .Write("display_tutorial_V2" , displayTutorial)
                        .Commit();

        QuickSaveRaw.LoadString("tutorial.json");
    }
    public void SaveTutorial(bool displayTutorial)
    {
        QuickSaveWriter.Create("tutorial")
                        .Write("display_tutorial", displayTutorial)
                        .Commit();

        QuickSaveRaw.LoadString("tutorial.json");
    }

    public void QuitTutorial()
    {
        SaveTutorial(false);
    }
    public void QuitTutorialImpostor()
    {
        SaveTutorial(false , false, false);
    }

    public void QuitFirstPanel()
    {
        SaveTutorial(setting.displayTutorial, setting.tutorialImpostor, false);
    }

    public void SaveWelcome(bool welcome)
    {
        QuickSaveWriter.Create("welcome")
                .Write("welcome", welcome)
                .Commit();
        setting.welcome = welcome;    
        QuickSaveRaw.LoadString("welcome.json");
    }


    public void LoadWelcome()
    {

        bool displayWelcome = true;
        try
        {
            QuickSaveReader.Create("welcome")
                      .Read<bool>("welcome", (r) => { displayWelcome = r; });
        }
        catch (Exception e)
        {
            SaveWelcome(true);
        }

        setting.welcome = displayWelcome;
    }

    public void SaveTestSkinIp()
    {
        QuickSaveWriter.Create("testSkinIp")
                .Write("name", setting.ip)
                .Commit();
        QuickSaveRaw.LoadString("testSkinIp.json");
    }

    public void LoadTestSkinIP()
    {
        String displayWelcome = "";
        try
        {
            QuickSaveReader.Create("testSkinIp")
                      .Read<String>("name", (r) => { displayWelcome = r; });
        }
        catch (Exception e)
        {
            SaveTestSkinIp();
        }

        setting.ip = displayWelcome;
    }
    public void SetIpSettingValue()
    {
        setting.ip = formIp_value.text;
        SaveTestSkinIp();
    }

    public void SetFullScreenMode(int index)
    {
        switch (index)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void OnclickFullScreenModeArrow(int index)
    {
        current_index_fullscreenMode = index;
        setting.fullscreenMode = current_index_fullscreenMode;
    }

    public void OnclickChangeServerRegion(GameObject regionText)
    {
        if (serverRegionIsUpdated)
        {
            SaveServerRegion(regionText.GetComponent<Text>().text);
            setting.region = regionText.GetComponent<Text>().text;

            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectToRegion(setting.region);
            //PhotonNetwork.LoadOrCreateSettings();
            //StartCoroutine(CoroutineReset(2));
        }   
    }

    public void UpdateServerRegion()
    {
        //if(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion.Length != 0)
        string serverRegionString = setting.region;
        Dropdown server_dropdown = serverRegion.GetComponent<Dropdown>();
        GetIndexServerWithName(serverRegionString);
        serverRegionIsUpdated = true;
    }

    public void GetIndexServerWithName(string serverRegionString)
    {
        Dropdown server_dropdown = serverRegion.GetComponent<Dropdown>();
        for(int i =0; i < server_dropdown.options.Count; i++)
        {
            if(server_dropdown.options[i].text == serverRegionString)
            {
                server_dropdown.value = i;
            }
        }
    }

   
    public void LoadServerRegion(string nearServer)
    {
        string serverRegion = nearServer;
        
        try
        {
            QuickSaveReader.Create("Server")
                      .Read<string>("region", (r) => { serverRegion = r; });
        }
        catch (Exception e)
        {
            SaveServerRegion(nearServer);
        }
        setting.region = serverRegion;

        if (this.serverRegion)
            UpdateServerRegion();
    }

    public void SaveServerRegion(string serverRegion)
    {
        QuickSaveWriter.Create("Server")
                      .Write("region", serverRegion)
                      .Commit();

        QuickSaveRaw.LoadString("Server.json");
    }
    public void OnClickApplySetResolutionFirstConnexion(Dropdown resolution_dropdown)
    {
        int resolution_int = resolution_dropdown.value;
        Resolution[] resolutions = Screen.resolutions;
        setting.resolution_width_index = GetIndexByResolution(Screen.currentResolution.width , Screen.currentResolution.height);
        setting.resolution_height_index = GetIndexByResolution(Screen.currentResolution.width, Screen.currentResolution.height);
        setting.fullscreen = fullscren.transform.GetChild(0).GetComponent<Image>().enabled;

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
            true);
        Camera.main.orthographicSize = 5.1f;
        SetFullScreenMode(1);
        SaveVideo();

    }

    public int GetIndexByResolution(int width, int height)
    {
        Resolution[] resolutions = Screen.resolutions;
        int index = 0;
        foreach(Resolution resolution in resolutions)
        {
            if(resolution.width == width && resolution.height == height)
            {
                return index;
            }
            index++;
        }
        return index + 7;
    }

    public void OnClickApplyLanguageWithoutReset(Dropdown language_dropdown)
    {
        //Dropdown language_dropdown = language.GetComponent<Dropdown>();
        
        int language_index = language_dropdown.value;
        setting.langage = setting.listLangage[language_index];
        SaveLanguage(setting.langage);
    }
    public void OnClickApplyFirstConnexion(Toggle azerty)
    {
        setting.canUpdate = true;
        ChangeControlToAzerty(azerty.isOn);
        DisplayInputTextInEachPanel();
        StartCoroutine(SetCanUpdateCouroutine());
    }

    public IEnumerator SetCanUpdateCouroutine()
    {
        yield return new WaitForSeconds(5);
        setting.canUpdate = false;
    }

    public void ChangeControlToAzerty(bool azerty)
    {
        if (azerty)
        {
            setting.INPUT_MOVE_FORWARD = KeyCode.Z;
            InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Positive = KeyCode.Z;
            setting.INPUT_MOVE_BACKWARD = KeyCode.S;
            InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Negative = KeyCode.S;
            setting.INPUT_MOVE_LEFT = KeyCode.Q;
            InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Negative = KeyCode.Q;
            setting.INPUT_MOVE_RIGHT = KeyCode.D;
            InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Positive = KeyCode.D;
            InputManager.Save();
        }
        else
        {
            setting.INPUT_MOVE_FORWARD = KeyCode.W;
            InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Positive = KeyCode.W;
            setting.INPUT_MOVE_BACKWARD = KeyCode.S;
            InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Negative = KeyCode.S;
            setting.INPUT_MOVE_LEFT = KeyCode.A;
            InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Negative = KeyCode.A;
            setting.INPUT_MOVE_RIGHT = KeyCode.D;
            InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Positive = KeyCode.D;
            InputManager.Save();
        }
    }

}
