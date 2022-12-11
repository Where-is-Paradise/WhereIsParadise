using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{

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

    // server
    public string codeRoom;
    public bool isMatchmaking = false;
    public float version = 0.48f;

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
    public int fullscreenMode = 0;

    //languge
    public string langage = "fr";
    public List<string> listLangage;
    public bool canUpdate = false;

    //tutorial
    public bool displayTutorial = true;
    public bool tutorialImpostor = true;

    //Server
    public string region = "eu";

    // Start is called before the first frame update
    void Start()
    {
        
        if (GameObject.FindGameObjectsWithTag("Setting").Length > 1)
            Destroy(this.gameObject);

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
        listTrialRoom.Add(true); // GodDeath room
        listTrialRoom.Add(true); // Damocles room
        listTrialRoom.Add(true); // Ax room
        listTrialRoom.Add(true); // Sword room
        listTrialRoom.Add(true); // Monsters room
        listTrialRoom.Add(true); // Lost torch room
        listTrialRoom.Add(true); // Labyrinth room

        listTrapRoom.Add(true); // Foggy room
        listTrapRoom.Add(true); // Cursed room
        listTrapRoom.Add(true); // Jail room
        listTrapRoom.Add(true); // ChestTraped room
        listTrapRoom.Add(true); // Poised room
        listTrapRoom.Add(true); // Pray trap room

        listObjectImpostor.Add(true); // Invisible potion
        listObjectImpostor.Add(true); // Satanic knife
        listObjectImpostor.Add(true); // Satanic book
    }


    // Update is called once per frame
    void Update()
    {
        
    }






}
