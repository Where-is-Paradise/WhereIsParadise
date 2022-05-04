using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{

    public int NB_PLAYER_MAX = 10;
    public int KEY_ADDITIONAL = 2;
    public int NB_IMPOSTOR = 1;
    public int RATIO_OBSTACLE = 5;
    public int DISTANCE_EXIT_DOOR_MAX = 8;
    public int NUMBER_EXPEDTION_MAX = 3;
    public int TORCH_ADDITIONAL = 1;
    public int WIDTH_DUNGEON = 10;
    public int HEIGHT_DUNGEON = 10;
    public bool DISTANCE_WITHOUT_OBSTACLE = true;
    public bool FOGGY_ROOM = false;
    public bool VIRUS_ROOM = false;
    public bool RANDOM_ROOM_ADDKEYS = false;
    public bool HELL_ROOM = true;
    public bool DISPLAY_MINI_MAP = true;
    public bool DISPLAY_DISTANCE_T1 = true;
    public bool DISPLAY_OBSTACLE_MAP = true;
    public bool DISPLAY_KEY_MAP = true;
    public bool LIMITED_TORCH = false;

    // server
    public string codeRoom;
    public bool isMatchmaking = false;

    // Player controller
    public KeyCode INPUT_MOVE_FORWARD = KeyCode.Z;
    public KeyCode INPUT_MOVE_BACKWARD =  KeyCode.S;
    public KeyCode INPUT_MOVE_LEFT = KeyCode.Q;
    public KeyCode INPUT_MOVE_RIGHT = KeyCode.D;
    public KeyCode INPUT_LAUCNH_EXPLORATION = KeyCode.E;
    public KeyCode INPUT_LAUNCH_VOTE_DOOR = KeyCode.F;
    public KeyCode INPUT_DISPLAY_MAP = KeyCode.M;


    // Audio
    public float volume_music = 15;
    public float volume_global = 15;
    public bool mute = false;

    //video
    public int resolution_width_index = 0;
    public int resolution_height_index = 0;
    public bool fullscreen = false;

    //languge
    public string langage = "fr";
    public List<string> listLangage;

    //tutorial
    public bool displayTutorial = true;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        InputManager.Load();
        INPUT_MOVE_FORWARD = InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Positive;
        INPUT_MOVE_BACKWARD = InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Negative;
        INPUT_MOVE_LEFT = InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Negative;
        INPUT_MOVE_RIGHT = InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Positive;
        INPUT_LAUCNH_EXPLORATION = InputManager.PlayerOneControlScheme.Actions[2].Bindings[0].Positive;
        INPUT_LAUNCH_VOTE_DOOR = InputManager.PlayerOneControlScheme.Actions[3].Bindings[0].Positive;
        INPUT_DISPLAY_MAP = InputManager.PlayerOneControlScheme.Actions[4].Bindings[0].Positive;

        listLangage.Add("en");
        listLangage.Add("fr");
        listLangage.Add("kr");
    }


    // Update is called once per frame
    void Update()
    {
        
    }






}
