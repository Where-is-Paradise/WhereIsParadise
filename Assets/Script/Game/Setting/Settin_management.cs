using Luminosity.IO;
using CI.QuickSave;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class Settin_management : MonoBehaviour
{

    private Setting setting;

    public GameObject input_manager;
    public List<Text> listTextInput;

    // Video
    public GameObject resolution;
    public GameObject fullscren;

    // Audio
    public Scrollbar globalVolum_scrollbar;
    public Scrollbar musicVolum_scrollbar;
    public GameObject mute;
    public AudioSource music;
    public List<AudioSource> allSound;

    public Text textVolume;
    public Text textGlobalVolume;

    //Language
    public GameObject language;

    private GameObject currentFormInput;
    private bool inputIsPress = false;

    public GameObject panelLanguageReset;

    // Start is called before the first frame update
    void Start()
    {
       
        //SaveLanguage();
        //Screen.SetResolution(1920, 1080,true);
        SetTextDropDownResolution();
        setting = GameObject.Find("Setting").GetComponent<Setting>();
        if(input_manager)
            DontDestroyOnLoad(input_manager);

               
#if !UNITY_IOS && !UNITY_ANDROID
        LoadAudio();
        LoadVideo();
#endif
        LoadLanguage();
        LoadTutorial();

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


    public void SetTextDropDownResolution()
    {
        Dropdown m_Dropdown = resolution.GetComponent<Dropdown>();
        Resolution[] resolutions = Screen.resolutions;
        List<string> resolution_string = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolution_string.Add(resolutions[i].ToString());
        }

        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(resolution_string);
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
        SaveVideo();
    }

    public void OnClickApplyLanguage()
    {
        Dropdown language_dropdown = language.GetComponent<Dropdown>();
        int language_index = language_dropdown.value;
        setting.langage = setting.listLangage[language_index];
        SaveLanguage(setting.langage);

        StartCoroutine(CoroutineReset(1));
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
        music.volume = (musicVolum_scrollbar.value / 20);
        int volume_int = (int) (music.volume * 1000);
        textVolume.text = volume_int  + "";
        music.mute = mute.transform.GetChild(0).GetComponent<Image>().enabled;

        setting.volume_music = musicVolum_scrollbar.value /20;        
        SaveAudio();

    }


    public void SetGlobalVolume()
    {
     
        foreach (AudioSource sound in allSound)
        {
            sound.volume = (globalVolum_scrollbar.value /10);
            int volume_int = (int)(sound.volume * 500);
            textGlobalVolume.text = volume_int + "";
            sound.mute = mute.transform.GetChild(0).GetComponent<Image>().enabled;
        }

        setting.volume_global = (globalVolum_scrollbar.value /10);
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
      

        music.volume = setting.volume_music;
        musicVolum_scrollbar.value = music.volume * 20;
        int volume_int = (int)(music.volume * 1000 );

        textVolume.text = volume_int + "";

        foreach(AudioSource sound in allSound)
        {
            sound.volume = setting.volume_global;
            globalVolum_scrollbar.value = sound.volume * 10;
            int volume_int2 = (int)(sound.volume * 500);
            textGlobalVolume.text = volume_int2 + "";
            sound.mute = setting.mute;
        }

        mute.transform.GetChild(0).GetComponent<Image>().enabled = setting.mute;
        music.mute = setting.mute;
    }


    public void SaveVideo()
    {

        QuickSaveWriter.Create("Video")
                        .Write("Index_width", setting.resolution_width_index)
                        .Write("Index_height", setting.resolution_height_index)
                        .Write("Fullscreen", setting.fullscreen)
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
                           .Read<bool>("Fullscreen", (r) => { setting.fullscreen = r; });
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
            TextAsset jsonTextFile = Resources.Load<TextAsset>(setting.langage);
            System.IO.File.WriteAllText(Path.Combine(Application.persistentDataPath, "QuickSave/" + setting.langage + ".json"), jsonTextFile.text);
        } 
    }

    public void LoadTutorial()
    {
        bool displayTutorial = true;
        try
        {
            QuickSaveReader.Create("tutorial")
                      .Read<bool>("display_tutorial", (r) => { displayTutorial = r; });
        }
        catch (Exception e)
        {
            SaveTutorial(true);
        }

        setting.displayTutorial = displayTutorial;

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


}
