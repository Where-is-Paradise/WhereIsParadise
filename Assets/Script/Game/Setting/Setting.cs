using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public bool PROD = true;
    public string linkServerAws;
    public bool MODE_TEST_SKIN_IP = false;
    public string ip = "";

    public int NB_PLAYER_MAX = 10;
    public int KEY_ADDITIONAL = 1;
    public int NB_IMPOSTOR = 1;
    public int RATIO_OBSTACLE = 5;
    public int DISTANCE_EXIT_DOOR_MAX = 6;
    public int NUMBER_EXPEDTION_MAX = 3;
    public int TORCH_ADDITIONAL = 0;
    public int WIDTH_DUNGEON = 10;
    public int HEIGHT_DUNGEON = 10;

    public List<bool> globalSetting;
    public List<bool> listSpeciallyRoom;
    public List<bool> listTrialRoom;
    public List<bool> listTeamTrialRoom;
    public List<bool> listObject;
    public List<bool> listTrapRoom;
    public List<bool> listObjectImpostor;

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
    public int INDEX_SKIN = 0;
    public int INDEX_SKIN_COLOR = 0;

    // server
    public string codeRoom;
    public string oldCodeRoom;
    public bool isMatchmaking = false;
    public float version = 0.48f;

    // Player keyboard
    public KeyCode INPUT_MOVE_FORWARD = KeyCode.Z;
    public KeyCode INPUT_MOVE_BACKWARD =  KeyCode.S;
    public KeyCode INPUT_MOVE_LEFT = KeyCode.Q;
    public KeyCode INPUT_MOVE_RIGHT = KeyCode.D;
    public KeyCode INPUT_LAUCNH_EXPLORATION = KeyCode.E;
    public KeyCode INPUT_LAUNCH_VOTE_DOOR = KeyCode.F;
    public KeyCode INPUT_DISPLAY_MAP = KeyCode.M;
    public KeyCode INPUT_ATTACK = KeyCode.Mouse0;
    public KeyCode INPUT_DASH = KeyCode.Space;

    // Player controller 
    public KeyCode INPUT_LAUCNH_EXPLORATION_controller = KeyCode.Joystick1Button0;
    public KeyCode INPUT_LAUNCH_VOTE_DOOR_controller = KeyCode.A;
    public KeyCode INPUT_DISPLAY_MAP_controller = KeyCode.Joystick1Button1;
    public KeyCode INPUT_ATTACK_controller = KeyCode.Joystick1Button6;
    public KeyCode INPUT_DASH_controller = KeyCode.Joystick1Button2;

    public int INPUT_LAUCNH_EXPLORATION_controller_axis = -1;
    public int INPUT_LAUNCH_VOTE_DOOR_controller_axis = -1;
    public int INPUT_DISPLAY_MAP_controller_axis = -1;
    public int INPUT_ATTACK_controller_axis = 9;
    public int INPUT_DASH_controller_axis = -1;

    public bool INPUT_LAUCNH_EXPLORATION_controller_isAxis = false;
    public bool INPUT_LAUNCH_VOTE_DOOR_controller_isAxis = false;
    public bool INPUT_DISPLAY_MAP_controller_isAxis = false;
    public bool INPUT_ATTACK_controller_isAxis = true;
    public bool INPUT_DASH_controller_isAxis = false;


    // Audio
    public float volume_music = 15;
    public float volume_global = 15;
    public bool mute = false;

    //video
    public int resolution_width_index = 0;
    public int resolution_height_index = 0;
    public bool fullscreen = true;
    public int fullscreenMode = 0;

    //languge
    public string langage = "fr";
    public List<string> listLangage;
    public bool canUpdate = false;

    //tutorial
    public bool displayTutorial = true;
    public bool tutorialImpostor = true;
    public bool firstTimePanel = true;
    public bool welcome = true;

    //Server
    public string region = "eu";

    //version
    public int major = 0;
    public int minor = 0;
    public int revision = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        if (GameObject.FindGameObjectsWithTag("Setting").Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
        //InputManager.Load();
        //InputManager.Reinitialize();
        InputManager.Save();
        InputManager.Load();

        listLangage.Add("en");
        listLangage.Add("fr");
        listLangage.Add("kr");

        globalSetting.Add(true); // Distance initial room
        globalSetting.Add(true); // Clue Double distance
        globalSetting.Add(true); // Map
        globalSetting.Add(true); // Trap at only start

        listSpeciallyRoom.Add(true); // ChestRoom
        listSpeciallyRoom.Add(true); // SacrificeRoom
        listSpeciallyRoom.Add(true); // HellRoom
        listSpeciallyRoom.Add(true); // NPC
        listSpeciallyRoom.Add(true); // Resurection
        listSpeciallyRoom.Add(true); // Purification
        listSpeciallyRoom.Add(true); // Pray

        listTrialRoom.Add(true); // FireBall room
        listTrialRoom.Add(true); // Damocles room
        listTrialRoom.Add(true); // Ax room
        listTrialRoom.Add(true); // Sword room
        listTrialRoom.Add(true); // Lost torch room
        listTrialRoom.Add(true); // Labyrinth room

        listTeamTrialRoom.Add(true); // GodDeath room
        listTeamTrialRoom.Add(true); // Monsters room

        listTrapRoom.Add(true); // Foggy room
        listTrapRoom.Add(true); // Cursed room
        listTrapRoom.Add(true); // ChestTraped room
        listTrapRoom.Add(true); // Pray trap room

        listObject.Add(true); // Map
        listObject.Add(true); // Field protection
        listObject.Add(true); // Black torch

        listObjectImpostor.Add(true); // Invisible potion
        listObjectImpostor.Add(true); // Satanic book
        listObjectImpostor.Add(true); // Traped Key
        listObjectImpostor.Add(false); // Satanic knife

        if (PROD)
            linkServerAws = "https://ec2-35-180-178-202.eu-west-3.compute.amazonaws.com";
        else
            linkServerAws = "localhost:8090";

        

    }

    public void LoadInputManager()
    {
        INPUT_MOVE_FORWARD = InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Positive;
        INPUT_MOVE_BACKWARD = InputManager.PlayerOneControlScheme.Actions[1].Bindings[0].Negative;
        INPUT_MOVE_LEFT = InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Negative;
        INPUT_MOVE_RIGHT = InputManager.PlayerOneControlScheme.Actions[0].Bindings[0].Positive;
        INPUT_LAUCNH_EXPLORATION = InputManager.PlayerOneControlScheme.Actions[2].Bindings[0].Positive;
        INPUT_LAUNCH_VOTE_DOOR = InputManager.PlayerOneControlScheme.Actions[3].Bindings[0].Positive;
        INPUT_DISPLAY_MAP = InputManager.PlayerOneControlScheme.Actions[4].Bindings[0].Positive;
        INPUT_ATTACK = InputManager.PlayerOneControlScheme.Actions[8].Bindings[0].Positive;
        INPUT_DASH = InputManager.PlayerOneControlScheme.Actions[9].Bindings[0].Positive;
    }


    // Update is called once per frame
    void Update()
    {
    }






}
