using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Luminosity.IO;

public class GameManager : MonoBehaviourPun
{

    public List<PlayerDun> listPlayer;
    public GameObject[] listPlayerTab;
    public List<Hexagone> dungeon;
    public Game game;
    public Hexagone hexagone;
    public Hexagone hexagone_current;
    public GameObject spawn;

    public Setting setting;
    public GameObject map;
    public GameObject map2;

    public GameManagerNetwork gameManagerNetwork;

    public bool expeditionHasproposed = false;
    public bool voteDoorHasProposed = false;
    public bool voteChestHasProposed = false;
    public bool alreaydyExpeditionHadPropose = false;
    public bool allPlayerAreComeBackOfExpedition = false;
    public bool endGame = false;
    public bool isLoading = true;
    public bool paradiseIsFind = false;
    public bool hellIsFind = false;
    public bool isAlreadyLoose = false;
    public bool DoorParadiseOrHellisOpen = false;
    public bool OnePlayerFindParadise = false;
    public bool OnePlayerFindHell = false;

    public UI_Manager ui_Manager;
    public Dictionary<int, int> door_idPlayer;
    public Dictionary<int, int> door_idPlayer_voteDoor;
    public Timer timer;
    public Room roomTeam;
    public Room hell;

    public int nbPlayerFinishLoading = 0;

    public bool launchExpedtion_inputButton = false;
    public bool launchVote_inputButton = false;

    private int nbPlayerInParadise = 0;
    private int nbPlayerInHell = 0;


    public AudioSource soundOpenDoor;
    public AudioSource soundTakeDoor;


    public Head_paradise headParadise;

    public bool alreadyPass = false;

    public int distanceInitial = 0;
    public int nbKeyBroken = 0;
    public int nbTorchOff = 0;

    public bool alreadyPasseLoading = false;
    public bool alreadyHell = false;

    public int numberPlayer = 0;
    public bool alreadyVerifyKeyInPath = false;
    public int old_distancePathfinding = 0;

    public bool canResetTimerBoss = false;

    public List<string> listNamePlayer = new List<string>();
    public List<string> listNamePlayerImpostor = new List<string>();

    public int nbReceiveWidthAndHeightMap = 0;
    public bool isActuallySpecialityTime = false;
    public bool speciallyIsLaunch = false;
    public Hexagone initialHexagone;

    public Room old_Paradise;

    public List<GameObject> listDoor = new List<GameObject>();

    public bool fireBallIsLaunch = false;
    public bool deathNPCIsLaunch = false;
    public bool damoclesIsLaunch = false;

    public bool gameIsReady = false;

    public bool waitForEndVote = false;

    public int counterRoom = 0;

    public bool labyrinthIsUsed = false;
    public bool NPCIsUsed = false;
    public bool PrayIsUsed = false;
    public bool ResurectionIsUsed = false;
    public bool PurificationIsUsed = false;
    public bool SacrificeIsUsedOneTimes = false;

    public int coutnerDoorOpenToJail = 0;
    public int indexDoorExplorationInJail = 0;
    public bool onePlayerInJail = false;
    public int nbKeyWhenJail = 0;

    public int viewIdIsMine;

    public List<float> listProbalitySpecialyRoom = new List<float>();

    public SaveDataNetwork dataGame;

    public bool timerStart = false;

    public bool paradiseHasChange = false;

    public bool alreadySacrifice = false;

    public GameObject doorsParent;
    private void Awake()
    {
        gameManagerNetwork = gameObject.GetComponent<GameManagerNetwork>();
        //PhotonNetwork.SendRate = 50;
        //PhotonNetwork.SerializationRate = 30;
    }
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ui_Manager.DisplayLoadPage(true);
        timer.LaunchTimer(10, false);
        //Camera.main.aspect = 1920 / 1080f;
        door_idPlayer = new Dictionary<int, int>();
        door_idPlayer_voteDoor = new Dictionary<int, int>();
        game = Game.CreateInstance(listPlayer);
        listPlayer = new List<PlayerDun>();
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        SetTABToList(listPlayerTab, listPlayer);
        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<Setting>();
        game.setting = setting;
        game.Launch(25, 25);
        GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);

        StartCoroutine(MasterClientCreateMap());
        /*        if (PhotonNetwork.IsMasterClient)
                {
                    int randomWidth = Random.Range(15, 16);
                    int randomHeight = Random.Range(15, 16);
                    gameManagerNetwork.SendWidthHeightMap(randomWidth, randomHeight);
                }*/
        roomTeam = game.currentRoom;
        numberPlayer = listPlayerTab.Length;
        SetNamePlayerInList();
        if (setting.displayTutorial)
        {
            GetPlayerMineGO().GetComponent<PlayerNetwork>().SendQuitTutorialN7(false);
        }
        /*        listProbalitySpecialyRoom.Add(0);
                listProbalitySpecialyRoom.Add(70);
                listProbalitySpecialyRoom.Add(0);*/
        listProbalitySpecialyRoom.Add(35);
        listProbalitySpecialyRoom.Add(57);
        listProbalitySpecialyRoom.Add(65);

        viewIdIsMine = GetPlayerMineGO().GetComponent<PhotonView>().ViewID;

        StartCoroutine(CouroutineTimerStart());
    }
    public IEnumerator MasterClientCreateMap()
    {
        yield return new WaitForSeconds(3);
        if (PhotonNetwork.IsMasterClient)
        {
            /*            gameManagerNetwork.SendSetting(setting.NUMBER_EXPEDTION_MAX, setting.DISPLAY_MINI_MAP,
                       setting.DISPLAY_OBSTACLE_MAP, setting.DISPLAY_KEY_MAP, setting.RANDOM_ROOM_ADDKEYS,
                       setting.LIMITED_TORCH, setting.TORCH_ADDITIONAL);*/
            game.CreationMap();
            game.ChangeBoss();
            game.AssignRole();
            SendRole();
            SendMap();
            ui_Manager.SetDistanceRoom(game.currentRoom.DistancePathFinding, game.currentRoom);
            game.nbTorch = Mathf.CeilToInt(game.dungeon.GetPathFindingDistance(game.currentRoom, game.dungeon.exit) / 2) + setting.TORCH_ADDITIONAL;
            //game.nbTorch = listPlayerTab.Length;
            if (game.dungeon.GetNumberOfPossiblityOfExit() > 7)
            {
                game.nbTorch = game.nbTorch + 1;
            }
            //game.nbTorch = 50;
            //sgame.nbTorch = 25;
            gameManagerNetwork.SendTorchNumber(game.nbTorch);
            ui_Manager.SetTorchNumber();
            GenerateHexagone(-7, 3.5f);
            Hexagone InitialHexa = GenerateObstacle();
            SetDoorObstacle(game.currentRoom);
            SetPositionHexagone(InitialHexa);
            initialHexagone = InitialHexa;
            SendBoss();
            game.SetKeyCounter();
            game.key_counter = game.key_counter + setting.KEY_ADDITIONAL;
            gameManagerNetwork.SendKey(game.key_counter);
            ui_Manager.SetNBKey();
            SetInitialPositionPlayers();
            AssignPowerOfImposter();
            AssignObjectPowerOfImposter();
            gameManagerNetwork.SendDisplayLightAllAvailableDoor(true);
            gameManagerNetwork.SendDisplayPowerImpostorInGame();
            gameManagerNetwork.SendDisplayObjectPowerImpostor();
            GetPlayerMineGO().GetComponent<PlayerGO>().SetRoomCursed();
            GetPlayerMineGO().GetComponent<PlayerNetwork>().
               SendDistanceCursed(GetPlayerMineGO().GetComponent<PlayerGO>().distanceCursed,
               GetPlayerMineGO().GetComponent<PlayerGO>().roomUsedWhenCursed.Index);

            counterRoom = Random.Range(0, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.timerFinish && isLoading)
        {
            timer.ResetTimer();
            SpawnPlayer();
            isLoading = false;
            ResetVoteCP();
            gameManagerNetwork.SendLoadingFinish();
        }
        if (nbPlayerFinishLoading == listPlayerTab.Length && !alreadyPasseLoading)
        {
            ui_Manager.DisplayLoadPage(false);
            ui_Manager.DisplayRoleInformation();
            alreadyPasseLoading = true;
            GetImpostorName();
        }
        if (InputManager.GetButtonDown("Escape") || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (ui_Manager.map.activeSelf && gameIsReady)
                ui_Manager.DisplayMap();
            else
                ui_Manager.DisplayEchapMenu();
        }
        if (GetPlayerMineGO() && GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition)
        {
            ui_Manager.MixDistanceForExploration();
        }

        /*        if(timerStart && !VerifyHasWinFireBall() && (!speciallyIsLaunch || !fireBallIsLaunch) && 
                    !ThereIsLever() && !timer.timerLaunch && !OnePlayerHaveToGoToExpedition() && !game.currentRoom.IsHell && !game.currentRoom.IsExit)
                {
                    RandomWinFireball();
                }*/

       
    }

    public IEnumerator CouroutineTimerStart()
    {
        yield return new WaitForSeconds(20);
        timerStart = true;
    }

    public IEnumerator LauchVoteDoorCoroutine()
    {
        yield return new WaitForSeconds(5.4f);
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        //StartCoroutine(ui_Manager.DesactivateLightAroundPlayers());
        GameObject door = GetDoorWithMajority();
        if (SamePositionAtBoss() && VerifyVoteVD(door.GetComponent<Door>().nbVote))
        {
            if (door.GetComponent<Door>().nbVote == 0 || game.currentRoom.IsVirus)
            {
                if (GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                {
                    List<GameObject> listDoorAvailable = GetDoorAvailable();
                    int indexDoorCurrent = Random.Range(0, listDoorAvailable.Count);
                    gameManagerNetwork.SendRandomIndexDoor(listDoorAvailable[indexDoorCurrent].GetComponent<Door>().index);
                }
            }
            else
            {
                ui_Manager.SetNBKey();
                ui_Manager.LaunchAnimationBrokenKey();
                if (GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                    gameManagerNetwork.SendKeyNumber();

                if (SamePositionAtBoss())
                    OpenDoor(door, false);
                expeditionHasproposed = false;
                alreaydyExpeditionHadPropose = false;


            }
        }
        StartCoroutine(ChangeBossCoroutine(0.1f));
        ui_Manager.ResetNbVote();
        ui_Manager.DesactiveZoneDoor();
        voteDoorHasProposed = false;
        timer.ResetTimer();
        ClearDoor();
        ClearExpedition();
        ui_Manager.DisplayKeyAndTorch(true);
        alreadyPass = false;
        SetAlreadyHideForAllPlayers();
        UpdateSpecialsRooms(game.currentRoom);
        if (game.currentRoom.IsVirus)
            ui_Manager.ResetLetterDoor();
        if (game.dungeon.initialRoom.HasSameLocation(game.currentRoom))
        {
            ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);

        }
        CloseDoorWhenVote(false);
        ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = 0;
    }
/*    public void voteDoorCouroutineLittleBefore()
    {
        voteDoorHasProposed
    }*/

    public IEnumerator LaunchExploration()
    {
        //if (AllPlayerHasVoted && !endGame && !alreaydyExpeditionHadPropose && !isLoading)
        //{
        waitForEndVote = true;
        yield return new WaitForSeconds(5.1f);
        timer.ResetTimer();
        StartCoroutine(ui_Manager.DesactivateLightAroundPlayers());
        if (PhotonNetwork.IsMasterClient)
        {
            if (VerifyVote())
            {
                gameManagerNetwork.SendVoteYesToExploration(false);
            }
            else
            {
                gameManagerNetwork.SendVoteNoToExploration();
            }
        }
        yield return new WaitForSeconds(2f);
        if (waitForEndVote)
        {
            ui_Manager.DisplayPanelInWaiting(true);
        }

        yield return new WaitForSeconds(11f);
        if (waitForEndVote)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (VerifyVote())
                {
                    gameManagerNetwork.SendVoteYesToExploration(false);
                }
                else
                {
                    gameManagerNetwork.SendVoteNoToExploration();
                }
            }
        }

    }

    public void LaunchExplorationAward()
    {
        timer.ResetTimer();
        if (GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            gameManagerNetwork.SendVoteYesToExploration(true);
            GetPlayerMineGO().GetComponent<PlayerGO>().canLaunchExplorationLever = false;
            GetPlayerMineGO().GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(false);
        }
    }
    public void AssignPowerOfImposter()
    {
        List<int> listIndexPower = new List<int>();
        if (setting.listTrapRoom[0])
            listIndexPower.Add(0);
        if (setting.listTrapRoom[1])
            listIndexPower.Add(1);
        if (setting.listTrapRoom[2])
            listIndexPower.Add(2);
        if (setting.listTrapRoom[3])
            listIndexPower.Add(3);

        if (listIndexPower.Count == 1)
        {
            foreach (GameObject player in GetAllImpostor())
            {
                player.GetComponent<PlayerNetwork>().SendIndexPower(listIndexPower[0]);
            }
            return;
        }
        if (listIndexPower.Count == 0)
        {
            foreach (GameObject player in GetAllImpostor())
            {
                player.GetComponent<PlayerNetwork>().SendIndexPower(-1);
            }
            return;
        }
        foreach (GameObject player in GetAllImpostor())
        {
            int randomInt = Random.Range(0, listIndexPower.Count);
            player.GetComponent<PlayerNetwork>().SendIndexPower(listIndexPower[randomInt]);
            //player.GetComponent<PlayerNetwork>().SendIndexPower(listIndexPower[0]);
            listIndexPower.RemoveAt(randomInt);
        }

    }
    public void AssignObjectPowerOfImposter()
    {
        List<int> listIndexPower = new List<int>();
        if (setting.listObjectImpostor[0])
            listIndexPower.Add(0);
        if (setting.listObjectImpostor[1])
            listIndexPower.Add(1);
        if (setting.listObjectImpostor[2])
            listIndexPower.Add(2);
        if (listIndexPower.Count == 1)
        {
            foreach (GameObject player in GetAllImpostor())
            {
                player.GetComponent<PlayerNetwork>().SendIndexObjectPower(listIndexPower[0]);
            }
            return;
        }
        if (listIndexPower.Count == 0)
        {
            foreach (GameObject player in GetAllImpostor())
            {
                player.GetComponent<PlayerNetwork>().SendIndexObjectPower(-1);
            }
            return;
        }
        foreach (GameObject player in GetAllImpostor())
        {
            int randomInt = Random.Range(0, listIndexPower.Count);
            player.GetComponent<PlayerNetwork>().SendIndexObjectPower(listIndexPower[randomInt]);
            //player.GetComponent<PlayerNetwork>().SendIndexObjectPower(listIndexPower[1]);
            listIndexPower.RemoveAt(randomInt);
        }
    }

    public void ResetVoteExploration()
    {
        ui_Manager.HideZoneVote();
        ResetVoteCP();
        ui_Manager.DisplayKeyAndTorch(true);

        if (game.dungeon.initialRoom.X == game.currentRoom.X)
        {
            if (game.dungeon.initialRoom.Y == game.currentRoom.Y)
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);
            }

        }
        GetPlayerMineGO().GetComponent<PlayerNetwork>().SendHideVoteExplorationDisplay();
        CloseDoorWhenVote(false);
        alreadyPass = false;
        SetAlreadyHideForAllPlayers();
        canResetTimerBoss = true;
    }

    public void GenerateHexagone(float initial_X, float initial_Y)
    {
        foreach (Room room in game.dungeon.rooms)
        {

            if (room.isTooFar)
                continue;

            Hexagone newHexagone = Instantiate(hexagone);
            newHexagone.Room = room;


            float rankGap = room.Y % 2 != 0 ? 0.82f : 0;

            float positionTransformationX = (initial_X + rankGap) + (1.6f * room.X);
            float positionTransformationY = (initial_Y) + (-1.41f * room.Y);

            newHexagone.transform.position = new Vector3(positionTransformationX, positionTransformationY);

            newHexagone.GetComponent<Hexagone>().index_text.text = room.GetIndex().ToString();
            newHexagone.transform.parent = map.transform;
            dungeon.Add(newHexagone);


        }

    }

    public Hexagone GetHexagone(int index)
    {

        foreach (Hexagone hexagone in dungeon)
        {
            if (hexagone.Room.Index == index)
            {
                return hexagone;
            }
        }
        return null;
    }

    public Hexagone GenerateObstacle()
    {
        Hexagone hexaReturn = null;
        foreach (Hexagone hex in dungeon)
        {
            SetHexagoneColor(hex);
            if (hex.Room.IsInitiale)
            {
                hexaReturn = hex;
            }
        }
        return hexaReturn;
    }

    public void SetHexagoneColor(Hexagone hex)
    {
        Room room = hex.Room;
        hex.GetComponent<Hexagone>().distanceText.text = game.dungeon.GetPathFindingDistance(room, game.currentRoom).ToString();
        if (room.IsObstacle)
        {
            if (setting.DISPLAY_OBSTACLE_MAP || GetPlayerMine().GetIsImpostor())
            {
                hex.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                hex.GetComponent<Hexagone>().distanceText.text = "";
                hex.GetComponent<Hexagone>().index_text.text = "";
            }
            return;
        }

        if (room.IsInitiale)
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            map.transform.position += new Vector3(-hex.transform.position.x, -hex.transform.position.y);
            hex.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(true);
            if (!room.IsExit)
                room.IsTraversed = true;
            hexagone_current = hex;
        }
        if (room.HasKey)
        {
            if (setting.DISPLAY_KEY_MAP || GetPlayerMine().GetIsImpostor())
            {
                hex.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            }
        }
        if (room.IsExit && GetPlayerMine().GetIsImpostor())
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            hex.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);
        }
        if (room.IsTraversed)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
        }


        if (room.isSpecial && !room.isTrial)
        {
            GameObject Information_Speciality = hex.transform.Find("Information_Speciality").gameObject;
            Information_Speciality.SetActive(true);
            Information_Speciality.transform.Find("Hexagone").Find("SpeciallyRoom").gameObject.SetActive(true);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(5f / 255f, 156f / 255f, 154f / 255f);
        }
        if (room.isTrial)
        {
            GameObject Information_Speciality = hex.transform.Find("Information_Speciality").gameObject;
            Information_Speciality.SetActive(true);
            Information_Speciality.transform.Find("Hexagone").Find("TrailRoom").gameObject.SetActive(true);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);
        }

    }

    public void SpawnPlayer()
    {
        GameObject myPlayer = GetPlayerMineGO();

        int indexPlayer_viewID = myPlayer.GetComponent<PhotonView>().ViewID;
        string indexPlayer_string = indexPlayer_viewID.ToString();
        char indexPlayer_char = indexPlayer_string[0];
        int indexPlayer = int.Parse(indexPlayer_char.ToString());


        myPlayer.transform.position = spawn.transform.GetChild(indexPlayer % listPlayer.Count).transform.position;
    }

    public void SetCurrentRoomColor()
    {
        foreach (Hexagone hexagone in dungeon)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            hexagone.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(false);
            hexagone.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(false);
            if (hexagone.GetComponent<Hexagone>().Room.Index == game.currentRoom.GetIndex())
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                if (!hexagone.GetComponent<Hexagone>().Room.IsExit)
                    hexagone.GetComponent<Hexagone>().Room.IsTraversed = true;
                if (!MineIsInExpedition())
                    hexagone.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(game.currentRoom.Index == hexagone.GetComponent<Hexagone>().Room.Index);
                else
                    hexagone.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(GetDoorExpedition(GetPlayerMine().GetId()).Index == hexagone.GetComponent<Hexagone>().Room.Index);
            }
            else
            {
                if (hexagone.GetComponent<Hexagone>().Room.IsTraversed)
                {
                    if (!hexagone.GetComponent<Hexagone>().Room.IsExit)
                        hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
                }
            }
            if (hell && GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation)
            {
                if (hexagone.GetComponent<Hexagone>().Room.X == hell.X && hexagone.GetComponent<Hexagone>().Room.Y == hell.Y)
                {
                    hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(255 / 255f), (float)0f / 255f, (float)0f / 255f, 1);
                    hexagone.transform.Find("Canvas").Find("Hell").gameObject.SetActive(true);
                }
            }
            if (hexagone.GetComponent<Hexagone>().Room.IsExit && GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation)
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                hexagone.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);
            }
            if (hexagone.GetComponent<Hexagone>().Room.IsObstacle)
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            }
        }
    }

    public void SetRoomColor(Room room, bool inExpedition)
    {
        foreach (Hexagone hexagone in dungeon)
        {
            if (hexagone.GetComponent<Hexagone>().Room.Index == room.GetIndex())
            {
                if (inExpedition)
                {
                    hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(241f / 255f), (float)130f / 255f, (float)70f / 255f, 1);
                    hexagone.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(true);
                }
                else
                {
                    hexagone.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                }
            }

            if (inExpedition)
            {
                if (hexagone.GetComponent<Hexagone>().Room.Index == game.currentRoom.GetIndex())
                {
                    hexagone.transform.Find("Canvas").Find("Player_identification").gameObject.SetActive(false);
                }
            }
        }
    }

    public void DesaciveAllDoor()
    {
        for(int i = 0; i < doorsParent.transform.childCount; i++)
        {
            doorsParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetDoorObstacle(Room room)
    {
        List<int> listDoorObstacleIndex = game.GetDoorObstacle(room);

        foreach (int indexRoomObstacle in listDoorObstacleIndex)
        {
            ui_Manager.DisplayObstacleInDoor(indexRoomObstacle, true);
            //GetDoorGo(indexRoomObstacle).SetActive(false);
        }
        DisplayZoneDoorForEachSituation();
    }


    public void SetDoorNoneObstacle(Room room)
    {
        List<int> listDoorObstacleIndex = game.GetDoorNoneObstacle(room);
      
        foreach (int indexRoomObstacle in listDoorObstacleIndex)
        {
            ui_Manager.DisplayObstacleInDoor(indexRoomObstacle, false);
            //GetDoorGo(indexRoomObstacle).SetActive(true);
        }
        DisplayZoneDoorForEachSituation();
    }


    public void SetPositionHexagone(Hexagone initial)
    {
        map.transform.position = new Vector3(-2.7f - (initial.transform.localPosition.x / 4), 4.8f - (initial.transform.localPosition.y / 4), -1);
        map.transform.localScale = new Vector3(0.48f, 0.5f, 1);

    }

    public void SetInitialPositionPlayers()
    {
        foreach (GameObject player in listPlayerTab)
        {

            SetPositionPlayer(player);
        }
    }

    public void SetPositionPlayer(GameObject player)
    {
        player.GetComponent<PlayerGO>().position_X = game.currentRoom.X;
        player.GetComponent<PlayerGO>().position_Y = game.currentRoom.Y;
    }

    /**
     * 
     *  Get all player near of door to add in dictionnary 
     *  Door ID -> Id player   [ list ]
     * 
     */
    public Dictionary<int, int> SetPlayerNearOfDoor()
    {
        //GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] doors = TreeDoorById();

        ClearDictionnaryPlayerNearDoor();
        foreach (GameObject door in doors)
        {
            if (door.gameObject.GetComponent<Door>().player)
            {
                door_idPlayer[door.GetComponent<Door>().index] = door.gameObject.GetComponent<Door>().player.GetComponent<PhotonView>().ViewID;
            }

        }
        return door_idPlayer;

    }

    public void ClearDictionnaryPlayerNearDoor()
    {
        for (int i = 0; i < 6; i++)
        {
            door_idPlayer.Remove(i);
        }

    }

    public void OpenDoor(GameObject door, bool isExpedition)
    {
        int indexDoor = door.GetComponent<Door>().index;
        roomTeam = game.GetRoomByNeigbourID(indexDoor);
        soundOpenDoor.Play();
        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
        if (!isExpedition)
        {
            door.GetComponent<Door>().isOpenForAll = true;
            game.currentRoom.door_isOpen[door.GetComponent<Door>().index] = true;
            if (roomTeam.HasKey && PhotonNetwork.IsMasterClient)
                gameManagerNetwork.AddKey(roomTeam.GetIndex());
            if (roomTeam.IsExit || roomTeam.IsHell)
            {
                gameManagerNetwork.SendParadiseOrHellFind(roomTeam.IsExit, roomTeam.IsHell);

            }
        }
        if (!isExpedition)
        {
            gameManagerNetwork.SendIsDiscorved(true, roomTeam.Index);
        }
        //InsertSpeciallyRoom(roomTeam, isExpedition);
        GameObject newDoor = GetDoorGo(indexDoor);
        gameManagerNetwork.SendOpenDoor(indexDoor, game.currentRoom.X, game.currentRoom.Y, isExpedition, roomTeam.GetIndex(), newDoor.GetComponent<Door>().GetRoomBehind().Index);
    }

    public void OpenDoorsToExpedition()
    {
        foreach (Expedition expe in game.current_expedition)
        {

            //GameObject[] doors = TreeDoorById();
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

            foreach (GameObject door in doors)
            {
                if (door.GetComponent<Door>().index == expe.indexNeigbour)
                {
                    OpenDoor(door, true);
                }
            }


        }

    }

    public void SetPlayersHaveTogoToExpeditionBool()
    {
        foreach (Expedition expe in game.current_expedition)
        {

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<PhotonView>().ViewID == expe.player.GetId())
                {
                    player.GetComponent<PlayerGO>().haveToGoToExpedition = true;
                }
            }


        }
    }


    public void ProposeExpedition(Dictionary<int, int> door_idPlayer)
    {
        bool playerWinAward = false;
        foreach (KeyValuePair<int, int> dic in door_idPlayer)
        {
            SendExpedition(dic.Value, dic.Key);
            if (GetPlayer(dic.Value).GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
                playerWinAward = true;
            }
        }
        if (playerWinAward)
        {
            timer.timerLaunch = true;
            expeditionHasproposed = true;
            gameManagerNetwork.SendExpeditionHadPropose(expeditionHasproposed);
            gameManagerNetwork.SendDisplayAllGost(true);
            StartCoroutine(CoroutineLaunchExploration());

        }
        else
        {
            gameManagerNetwork.LaunchTimerExpedition();
        }
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
    }

    public IEnumerator CoroutineLaunchExploration()
    {
        yield return new WaitForSeconds(0.2f);
        LaunchExplorationAward();
    }

    public bool VerificationExpedition(Dictionary<int, int> door_idPlayer)
    {
        if (door_idPlayer.Count == 0)
            return false;
        if (GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            return true;
        if ((setting.LIMITED_TORCH && game.nbTorch > 0) && (door_idPlayer.Count <= game.nbTorch))
            return true;
        return false;
    }

    public void SetNbTorch(int nbExpedition)
    {
        game.nbTorch = game.nbTorch - nbExpedition;
        nbTorchOff++;
        ui_Manager.SetTorchNumber();
        //gameManagerNetwork.SendTorchNumber(game.nbTorch);
    }

    public void SendRole()
    {
        int counter = 1;
        foreach (PlayerDun player in listPlayer)
        {
            gameManagerNetwork.SendRole(player.GetId(), player.GetIsImpostor(), counter == listPlayer.Count);
            counter++;
        }
    }

    public void SendBoss()
    {
        PlayerDun playerBoss = game.GetBoss();
        if (!playerBoss)
        {
            int randomInt = Random.Range(0, game.list_player.Count);
            gameManagerNetwork.SendBoss(game.list_player[randomInt].GetId());
        }
        else
        {
            gameManagerNetwork.SendBoss(playerBoss.GetId());
        }

        GetBoss().GetComponent<PlayerNetwork>().SendDisplayCrown(true);
    }

    public void SendExpedition(int idPlayer, int idRoom)
    {
        gameManagerNetwork.SendExpedition(idPlayer, idRoom);
    }


    public void SendMap()
    {
        int counter = 1;
        foreach (Room room in game.dungeon.rooms)
        {
            gameManagerNetwork.SendMap(room.Index, room.IsExit, room.IsObstacle, room.isTooFar,
                room.IsInitiale, room.DistanceExit, room.DistancePathFinding,
                room.distance_pathFinding_initialRoom, counter == game.dungeon.rooms.Count, room.IsFoggy, room.IsVirus,
                room.HasKey, room.chest, room.isHide);

            if (room.chest)
            {
                game.dungeon.InsertChestRoom(room.Index);
                for (int i = 0; i < 2; i++)
                {
                    gameManagerNetwork.SendChestData(room.GetIndex(), room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward);
                }
            }
            if (room.fireBall)
            {
                gameManagerNetwork.SendFireBallData(room.GetIndex(), room.fireBall);
            }
            if (room.isSacrifice)
            {
                gameManagerNetwork.SendSacrificeData(room.GetIndex(), room.isSacrifice);
            }
            if (room.isAx)
            {
                gameManagerNetwork.SendAxData(room.GetIndex(), room.isAx);
            }
            if (room.isSword)
            {
                gameManagerNetwork.SendSwordData(room.GetIndex(), room.isSword);
            }
            if (room.isSwordDamocles)
            {
                gameManagerNetwork.SendDamoclesData(room.GetIndex(), room.isSwordDamocles);
            }
            if (room.isDeathNPC)
            {
                gameManagerNetwork.SendDeahtNPCData(room.GetIndex(), room.isDeathNPC);
            }
            if (room.isLostTorch)
            {
                gameManagerNetwork.SendLostTorchData(room.GetIndex(), room.isLostTorch);
            }
            if (room.isMonsters)
            {
                gameManagerNetwork.SendMonstersData(room.GetIndex(), room.isMonsters);
            }
            if (room.isPurification)
            {
                gameManagerNetwork.SendPurificationData(room.GetIndex(), room.isPurification);
            }
            if (room.isResurection)
            {
                gameManagerNetwork.SendResurectionData(room.GetIndex(), room.isResurection);
            }
            if (room.isPray)
            {
                gameManagerNetwork.SendPrayData(room.GetIndex(), room.isPray);
            }
            if (room.isNPC)
            {
                gameManagerNetwork.SendNPCData(room.GetIndex(), room.isNPC);
            }
            if (room.isLabyrintheHide)
            {
                gameManagerNetwork.SendLabyrinthData(room.GetIndex(), room.isLabyrintheHide);
            }
            counter++;
        }
    }

    public void DisplayPlayerLog()
    {
        foreach (PlayerDun player in game.list_player)
        {
            Debug.Log(player.GetPlayerName() + " " + player.GetId() + " " + player.GetIsBoss());
        }

    }

    public void SetTABToList(GameObject[] tab, List<PlayerDun> listGO)
    {
        listGO.Clear();
        for (int i = 0; i < tab.Length; i++)
        {
            PlayerDun newPlayer = PlayerDun.CreateInstance(tab[i].GetComponent<PhotonView>().ViewID, tab[i].GetComponent<PlayerGO>().playerName);

            newPlayer.SetIsBoss(tab[i].GetComponent<PlayerGO>().isBoss);
            //newPlayer.SetIsImpostor(tab[i].GetComponent<PlayerGO>().isImpostor);
            listGO.Add(newPlayer);
        }
        if (listGO != null)
            game.list_player = listGO;
    }


    public void UpdateListPlayerGO()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
    }


    public bool IsPlayerMine(int id)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PhotonView>().ViewID == id && player.GetComponent<PhotonView>().IsMine)
            {
                return true;
            }
        }
        return false;
    }


    public void VoteCP(int vote)
    {
        PlayerDun player = GetPlayerMine();
        gameManagerNetwork.SendVoteCP(player.GetId(), vote);
    }

    public PlayerDun GetPlayerMine()
    {
        foreach (PlayerDun player in game.list_player)
        {
            if (IsPlayerMine(player.GetId()))
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
            if (player.GetComponent<PhotonView>().IsMine)
            {
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

    public GameObject GetPlayerUserId(string userId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerNetwork>().userId.Equals(userId))
            {
                return player;
            }
        }
        return null;
    }

    public bool AllPlayerHasVoted()
    {
        foreach (PlayerDun player in game.list_player)
        {
            //
            if (!player.GetHasVoted_CP())
            {
                return false;
            }

        }
        return true;
    }

    public void ResetVoteCP()
    {
        foreach (PlayerDun player in listPlayer)
        {
            player.SetHasVoted_CP(false);
        }
    }

    public void ResetVoteVD()
    {
        foreach (PlayerDun player in listPlayer)
        {
            player.SetHasVoted_VD(false);
        }
    }


    public void ClearExpedition()
    {
        game.ClearExpedition();
        // ui_Manager.ClearAllPanel();
    }
    public void ClearDoor()
    {
        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            door.GetComponent<Door>().old_player = null;
            door.GetComponent<Door>().player = null;
            door.GetComponent<Door>().counterPlayerInDoorZone = 0;
            door.GetComponent<Door>().letter_displayed = false;
        }
    }

    public void ResetDoor()
    {
        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");


        for(int i =0; i< doorsParent.transform.childCount;i++)
        {
            Debug.Log(doorsParent.transform.GetChild(i).GetComponent<Door>().doorName);
            doorsParent.transform.GetChild(i).GetComponent<Door>().IsCloseNotPermantly = false;
        }


    }
    public void ResetDoorsActive()
    {
        foreach (GameObject door in listDoor)
        {
            door.SetActive(true);

        }
    }

    public bool VerifyVote()
    {
        int counterYes = 0;
        int counterNo = 0;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerGO>().vote_cp == 1)
            {
                counterYes++;
            }
            if (player.GetComponent<PlayerGO>().vote_cp == -1)
            {
                counterNo++;
            }
        }
        if (counterYes >= counterNo)
        {
            if (counterYes == 0)
                return false;
            return true;
        }
        return false;
    }

    public bool VerifyVoteVD(int voteDoorMax)
    {
        GameObject x_zone = GameObject.Find("X_zone");
        if (!x_zone)
        {
            return true;
        }
        if (game.currentRoom.IsVirus)
        {
            if (x_zone.GetComponent<x_zone_colider>().nbVote > CountPlayerNoneSacrifice() / 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        if (x_zone.GetComponent<x_zone_colider>().nbVote > voteDoorMax)
        {
            return false;
        }
        return true;
    }

    public int CountPlayerNoneSacrifice()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int count = 0;
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().isSacrifice)
            {
                count++;
            }
        }
        return count;
    }

    public bool NobodyHasVoted()
    {
        int voteMax = 0;
        int indexDoorCurrent = -1;
        //GameObject[] listDoor = TreeDoorById();
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in listDoor)
        {
            if (door.GetComponent<Door>().nbVote >= voteMax)
            {
                indexDoorCurrent = door.GetComponent<Door>().index;
                voteMax = door.GetComponent<Door>().nbVote;
            }

        }
        if (voteMax == 0)
        {
            return true;
        }

        return false;
    }

    public void TeleportPlayerToExpedition()
    {
        GetPlayerMine().SetIsInExpedition(true);
        SendDataMine();
        ResetDoorsActive();
        gameManagerNetwork.SendIsInExpedition(GetPlayerMine().GetId(), true);
        int distance = GetDistanceInExpedition(game.current_expedition);

        Room roomExpedition = GetDoorExpedition(GetPlayerMine().GetId());
        GetPlayerMineGO().GetComponent<PlayerGO>().position_X = roomExpedition.X;
        GetPlayerMineGO().GetComponent<PlayerGO>().position_Y = roomExpedition.Y;
        HidePlayerNotInSameRoom();
        ui_Manager.DisplayAllGost(false);


        ui_Manager.DisplayKeyAndTorch(false);
        if (roomExpedition.HasKey && !roomExpedition.IsFoggy)
        {
            ui_Manager.DisplayKeyPlusOne(true);
        }

        if (roomExpedition.IsFoggy)
        {
            ui_Manager.DisplayInterrogationPoint();
        }

        ui_Manager.HideImgInMiddleOfSpeciallyRoom(roomExpedition, false);
        /*        else
                {
                    if(GetPlayerMineGO().GetComponent<PlayerGO>().isCursed || roomExpedition.isCursedTrap)
                    {
                        int falseDistance = game.dungeon.GetPathFindingDistance(GetExpeditionOfPlayerMine().room,GetPlayerMineGO().GetComponent<PlayerGO>().roomUsedWhenCursed);
                        ui_Manager.SetDistanceRoomFalse(falseDistance);
                    }
                    else
                        ui_Manager.SetDistanceRoom(distance, roomExpedition);
                }*/

        SetDoorNoneObstacle(roomExpedition);
        SetDoorObstacle(roomExpedition);
        SetRoomColor(roomExpedition, true);

        Expedition expe = GetExpeditionOfPlayerMine();
        int indexDoorExit = GetIndexDoorAfterCrosse(expe.indexNeigbour);
        GameObject player = GetPlayerMineGO();
        GameObject door = GetDoorGo(indexDoorExit);
        player.transform.position = door.transform.GetChild(3).transform.position;
        player.GetComponent<PlayerGO>().isInExpedition = true;
        //gameManagerNetwork.SendPositionReel(player.GetComponent<PhotonView>().ViewID, player.transform.position.x, player.transform.position.y);   

        //door.GetComponent<Animator>().SetBool("open", true);
        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);

        door.GetComponent<Door>().isOpen = true;
        door.GetComponent<Door>().old_player = player;

        CloseAllDoor(roomExpedition, true, door);
        ui_Manager.DisplayTutorialAutel(false);

        gameManagerNetwork.SendPositionPlayer(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, roomExpedition.X, roomExpedition.Y);
        ui_Manager.DisplayBlackScreen(false, true);
        if (roomExpedition.IsExit)
            ui_Manager.DisplayParadise(true, indexDoorExit);
        if (roomExpedition.IsHell)
            ui_Manager.DisplayHell(true);

        if (setting.displayTutorial)
        {
            if (!ui_Manager.listTutorialBool[9])
            {
                ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
                ui_Manager.tutorial_parent.SetActive(true);
                ui_Manager.tutorial[9].SetActive(true);
                ui_Manager.listTutorialBool[9] = true;
            }

        }
        if (roomExpedition.isJail && !roomExpedition.speciallyPowerIsUsed)
        {
            door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
            door.GetComponent<Door>().isOpen = false;
            door.GetComponent<Door>().old_player = null;
            ui_Manager.DisplayJailRoom(true);
            indexDoorExplorationInJail = door.GetComponent<Door>().index;
            GetPlayerMineGO().GetComponent<PlayerGO>().haveToGoToExpedition = false;
            gameManagerNetwork.SendCatchInJailRoom(door.GetComponent<Door>().index);
            GetPlayerMine().SetIsInExpedition(false);
            gameManagerNetwork.SendHavetoGoToExpedition(false, GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
            gameManagerNetwork.SendDoorToClose(expe.indexNeigbour);
            GetPlayerMineGO().GetComponent<PlayerGO>().isGoInExpeditionOneTime = true;
            GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition = false;
            gameManagerNetwork.SendIsInJail(true, GetPlayerMineGO().GetComponent<PhotonView>().ViewID, roomExpedition.Index);
            game.currentRoom = roomExpedition;
            if (!OnePlayerHaveToGoToExpedition())
            {
                gameManagerNetwork.SendDisplayDoorLever(true);
            }
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            CloseDoorOveride();
            ui_Manager.timerMixExploration = true;
            onePlayerInJail = true;
            nbKeyWhenJail = nbKeyBroken;
            if (GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            {
                ChangeBoss();
            }
            gameManagerNetwork.SendUpdateDataPlayer(player.GetComponent<PhotonView>().ViewID);

        }
        gameManagerNetwork.SendDisplayLightAllAvailableDoor(false);

        UpdateSpecialsRooms(roomExpedition);
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);

        if (!roomExpedition.IsHell && !roomExpedition.IsExit)
        {
            CloseDoorExplorationWhenVote(true);
            /*            string trueLetter2 = GetDoorToTakeExploration(roomExpedition);
                        string falseLetter2 = GetDoorNotToTakeExploration(roomExpedition);*/
            StartCoroutine(ui_Manager.MixDistanceExplorationStopCoroutine());
        }
        if (roomExpedition.IsVirus)
        {
            ui_Manager.DisplaySpeciallyLevers(false, 0);
        }
    }

    public void BackToExpedition()
    {
        GetPlayerMine().SetIsInExpedition(false);
        ResetDoorsActive();
        gameManagerNetwork.SendIsInExpedition(GetPlayerMine().GetId(), false);
        //ui_Manager.DisplayAllGost(true);

        int distance = game.currentRoom.DistancePathFinding;
        ui_Manager.SetDistanceRoom(distance, game.currentRoom);
        SetDoorNoneObstacle(game.currentRoom);
        SetDoorObstacle(game.currentRoom);
        Room roomInExpe = GetDoorExpedition(GetPlayerMine().GetId());
        GetPlayerMineGO().GetComponent<PlayerGO>().position_X = game.currentRoom.X;
        GetPlayerMineGO().GetComponent<PlayerGO>().position_Y = game.currentRoom.Y;
        HidePlayerNotInSameRoom();
        GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayWhiteLight(false);

        ui_Manager.DisplayKeyPlusOne(false);
        ui_Manager.DisplayKeyAndTorch(true);
        ui_Manager.HideImgInMiddleOfSpeciallyRoom(roomInExpe, true);
        SetRoomColor(roomInExpe, false);
        SetCurrentRoomColor();
        ui_Manager.HideDistanceRoom();
        //gameManagerNetwork.SendBackToExpe(GetPlayerMine().GetId());
        GetPlayerMineGO().GetComponent<PlayerGO>().isInExpedition = false;
        GetPlayerMineGO().GetComponent<PlayerGO>().haveToGoToExpedition = false;
        gameManagerNetwork.SendHavetoGoToExpedition(false, GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        Expedition expe = GetExpeditionOfPlayerMine();
        int indexDoorExit = expe.indexNeigbour;


        GameObject door = GetDoorGo(indexDoorExit);
        GetPlayerMineGO().transform.position = door.transform.GetChild(3).transform.position;
        //door.GetComponent<Animator>().SetBool("open", false);
        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
        door.GetComponent<Door>().old_player = null;
        door.GetComponent<Door>().player = null;
        door.GetComponent<Door>().isOpen = false;
        door.GetComponent<Door>().counterPlayerInDoorZone = 0;
        door.GetComponent<Door>().letter_displayed = false;

        CloseAllDoor(game.currentRoom, false);



        int index_old_door = GetIndexDoorAfterCrosse(indexDoorExit);
        GameObject old_door = GetDoorGo(index_old_door);
        //old_door.GetComponent<Animator>().SetBool("open", false);
        old_door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
        old_door.GetComponent<Door>().old_player = null;
        old_door.GetComponent<Door>().player = null;
        old_door.GetComponent<Door>().isOpen = false;
        old_door.GetComponent<Door>().counterPlayerInDoorZone = 0;
        door.GetComponent<Door>().letter_displayed = false;


        gameManagerNetwork.SendDoorToClose(indexDoorExit);

        StartCoroutine(PauseWhenPlayerTakeDoor(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, game.currentRoom.X, game.currentRoom.Y, 0.2f));
        //gameManagerNetwork.SendPositionPlayer(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, game.currentRoom.X, game.currentRoom.Y);
        ui_Manager.DisplayBlackScreen(false, true);
        OpenDoorMustBeOpen();
        ui_Manager.HideParadise();
        ui_Manager.HideHell();

        GetPlayerMineGO().GetComponent<PlayerGO>().isGoInExpeditionOneTime = true;

        if (!OnePlayerHaveToGoToExpedition() && !game.currentRoom.IsVirus)
        {
            gameManagerNetwork.SendDisplayDoorLever(true);
            gameManagerNetwork.DisplayLightAllAvailableDoor(true);
        }

        if (game.dungeon.initialRoom.X == game.currentRoom.X)
        {
            if (game.dungeon.initialRoom.Y == game.currentRoom.Y)
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);
                ui_Manager.DisplayTutorialAutel(true);
            }

        }
        UpdateSpecialsRooms(game.currentRoom);
        ui_Manager.timerMixExploration = true;
        gameManagerNetwork.SendDisplayLightAllAvailableDoorN2(true);
        ResetDoorExploration();
        //CloseDoorExplorationWhenVote(false);
    }

    public void ChangePositionPlayerWhenTakeDoor(GameObject player, GameObject doorEnter)
    {
        int indexDoorExit = GetIndexDoorAfterCrosse(doorEnter.GetComponent<Door>().index);
        GameObject door = GetDoorGo(indexDoorExit);
        player.transform.position = door.transform.GetChild(3).transform.position;
    }

    public void CloseAllDoor(Room room, bool isInExepedtion, GameObject doorReverse = null)
    {
        GameObject[] doors = TreeDoorById();
        //GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
/*        int i = 0;
        foreach (GameObject door in doors)
        {
            if (room.door_isOpen[i] && i == door.GetComponent<Door>().index)
            {
                door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                door.GetComponent<Door>().isOpenForAll = true;
            }
            else
            {
                door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
                door.GetComponent<Door>().isOpenForAll = false;
            }
            i++;
        }*/

        for(int i = 0; i< doorsParent.transform.childCount; i++)
        {
            if (room.door_isOpen[i] && i == doorsParent.transform.GetChild(i).GetComponent<Door>().index)
            {
                doorsParent.transform.GetChild(i).transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                doorsParent.transform.GetChild(i).GetComponent<Door>().isOpenForAll = true;
            }
            else
            {
                doorsParent.transform.GetChild(i).transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
                doorsParent.transform.GetChild(i).GetComponent<Door>().isOpenForAll = false;
            }
        }

        if (isInExepedtion)
        {
            foreach (GameObject door in doors)
            {
                if (doorReverse.GetComponent<Door>().index == door.GetComponent<Door>().index)
                {
                    //door.GetComponent<Animator>().SetBool("open", true);
                    door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                }

            }

        }

    }

    public string GetDoorToTakeExploration(Room room)
    {
        List<string> listDoorToTake = new List<string>();
        if (!room.left_neighbour.IsObstacle && room.left_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("A");
        }
        if (!room.up_Left_neighbour.IsObstacle && room.up_Left_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("B");
        }
        if (!room.up_Right_neighbour.IsObstacle && room.up_Right_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("C");
        }
        if (!room.right_neighbour.IsObstacle && room.right_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("D");
        }
        if (!room.down_Right_neighbour.IsObstacle && room.down_Right_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("E");
        }
        if (!room.down_Left_neighbour.IsObstacle && room.down_Left_neighbour.DistancePathFinding < room.DistancePathFinding)
        {
            listDoorToTake.Add("F");
        }
        if (listDoorToTake.Count == 0)
        {
            return "";
        }

        return listDoorToTake[Random.Range(0, listDoorToTake.Count)];
    }

    public string GetDoorNotToTakeExploration(Room room)
    {
        List<string> listDoorToTake = new List<string>();
        if (!room.left_neighbour.IsObstacle && room.left_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("A");
        }
        if (!room.up_Left_neighbour.IsObstacle && room.up_Left_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("B");
        }
        if (!room.up_Right_neighbour.IsObstacle && room.up_Right_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("C");
        }
        if (!room.right_neighbour.IsObstacle && room.right_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("D");
        }
        if (!room.down_Right_neighbour.IsObstacle && room.down_Right_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("E");
        }
        if (!room.down_Left_neighbour.IsObstacle && room.down_Left_neighbour.DistancePathFinding >= room.DistancePathFinding)
        {
            listDoorToTake.Add("F");
        }
        if (listDoorToTake.Count == 0)
        {
            return "";
        }

        return listDoorToTake[Random.Range(0, listDoorToTake.Count)];
    }


    public List<Room> GetNeighbourWidthSuperiorDistance(Room room)
    {
        List<Room> listRoomWidthSuperiorDistance = new List<Room>();
        foreach (Room neighbour in room.listNeighbour)
        {
            if (neighbour.IsObstacle)
                continue;
            if (neighbour.DistancePathFinding >= room.DistancePathFinding)
            {
                listRoomWidthSuperiorDistance.Add(neighbour);
            }
        }

        return listRoomWidthSuperiorDistance;
    }

    public void CloseDoorOveride()
    {
        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
            door.GetComponent<Door>().isOpenForAll = false;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), door.GetComponent<BoxCollider2D>(), false);
            }

        }
    }

    public GameObject[] TreeDoorById()
    {
        /*        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                List<int> intDoor = 
                List<int> blackList = new List<int>();

                return NexListDoors;*/
        return GameObject.FindGameObjectsWithTag("Door");
    }

    public GameObject[] TreeChestById()
    {

        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        GameObject[] nestListChest = new GameObject[2];
        int i = 0;
        int j = 0;
        while (i < 2)
        {

            if (chests[j].transform.Find("VoteZone").GetComponent<ChestZoneVote>().indexChest == i)
            {
                nestListChest[i] = chests[j];
                i++;
            }
            j++;
            if (j == 2)
            {
                j = 0;
            }
        }

        return nestListChest;
    }



    public void OpenDoorMustBeOpen()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().isOpenForAll)
            {
                //door.GetComponent<Animator>().SetBool("open", true);
                door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);

            }

        }
    }


    public int GetIndexDoorAfterCrosse(int indexDoor)
    {
        if (indexDoor == 0)
        {
            return 3;
        }
        else if (indexDoor == 1)
        {
            return 4;
        }
        else if (indexDoor == 2)
        {
            return 5;
        }
        else if (indexDoor == 3)
        {
            return 0;
        }
        else if (indexDoor == 4)
        {
            return 1;
        }
        else if (indexDoor == 5)
        {
            return 2;
        }

        return -1;

    }

    public GameObject GetDoorGo(int index)
    {
        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().index == index)
            {
                return door;
            }
        }
        return null;
    }

    public Room GetDoorExpedition(int idPlayer)
    {
        foreach (Expedition expe in game.current_expedition)
        {
            if (expe.player.GetId() == idPlayer)
            {
                return expe.room;
            }
        }
        return null;
    }
    public bool MineIsInExpedition()
    {
        bool isInExpedition = false;
        foreach (Expedition expe in game.current_expedition)
        {
            if (expe.player.GetId() == GetPlayerMine().GetId())
            {
                isInExpedition = true;
            }
            expe.player.SetIsInExpedition(true);
        }
        return isInExpedition;
    }

    public void HidePlayerInExpedition()
    {
        foreach (Expedition expe in game.current_expedition)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].GetComponent<PhotonView>().ViewID == expe.player.GetId())
                {
                    expe.player.SetIsInExpedition(true);
                    player[i].transform.GetChild(0).gameObject.SetActive(false);
                    player[i].transform.GetChild(1).gameObject.SetActive(false);
                }
            }

        }
    }

    public Expedition GetExpeditionOfPlayerMine()
    {
        List<Expedition> listExpe = game.current_expedition;
        foreach (Expedition expe in listExpe)
        {

            if (expe.player.GetId() == GetPlayerMine().GetId())
            {
                return expe;
            }
        }

        return null;
    }



    public int GetDistanceInExpedition(List<Expedition> listExepdition)
    {
        foreach (Expedition expe in listExepdition)
        {
            if (expe.player.GetId() == GetPlayerMine().GetId())
            {
                return expe.room.DistancePathFinding;
            }

        }
        return 0;
    }

    public IEnumerator ChangeBossCoroutine(float seconde)
    {
        yield return new WaitForSeconds(seconde);
        ChangeBoss();
    }

    public void ChangeBoss()
    {
        ResetTimerBoss(true);
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerDun boss = null;
            int counter = 0;
            do
            {
                boss = game.ChangeBoss();
                counter++;
            } while (!GetPlayer(boss.GetId()) || (GetPlayer(boss.GetId()).GetComponent<PlayerGO>().isSacrifice || GetPlayer(boss.GetId()).GetComponent<PlayerGO>().isInJail) && counter < 20);
            if (boss == null)
            {
                Debug.LogError("sa passe");
                boss = game.ChangeBoss();
            }
            gameManagerNetwork.SendBoss(boss.GetId());
        }
    }
    public void ResetTimerBoss(bool reset)
    {
        if (GetBoss())
        {
            GetBoss().GetComponent<PlayerGO>().timerBossLaunch = !reset;
        }

    }

    public bool IsBoss()
    {
        GameObject player = GetPlayerMineGO();

        if (player.GetComponent<PlayerGO>().isBoss)
        {
            return true;
        }
        return false;
    }


    public void ActiveZoneDoor()
    {
        gameManagerNetwork.SendCloseDoorWhenVote();
        StartCoroutine(gameManagerNetwork.SendActiveZoneDoor());
    }



    public void TakeDoor(GameObject door, GameObject player)
    {
        int indexDoor = door.GetComponent<Door>().index;
        ResetDoorsActive();
        game.currentRoom = game.GetRoomByNeigbourID(indexDoor);
        int indexNeWDoor3 = GetIndexDoorAfterCrosse(indexDoor);
        CloseAllDoor(game.currentRoom, false);


        if (!CheckDoorIsOpenByRoom(game.currentRoom.X, game.currentRoom.Y, indexNeWDoor3))
        {
            ResetVoteVD();
            GameObject newDoor = GetDoorGo(indexNeWDoor3);
            game.currentRoom.door_isOpen[newDoor.GetComponent<Door>().index] = true;
        }

        ui_Manager.DisplayAllDoorLightExploration(false);
        UpdateSpecialsRooms(game.currentRoom);
        ui_Manager.SetDistanceRoom(game.currentRoom.DistancePathFinding, game.currentRoom);
        SetDoorNoneObstacle(game.currentRoom);
        SetDoorObstacle(game.currentRoom);
        SetCurrentRoomColor();
        ui_Manager.HideDistanceRoom();
        SetPositionPlayer(player);
        PlayerGO playerGo = player.GetComponent<PlayerGO>();
        ChangePositionPlayerWhenTakeDoor(player, door);

        int indexNeWDoor2 = GetIndexDoorAfterCrosse(door.GetComponent<Door>().index);
        GameObject newDoor2 = GetDoorGo(indexNeWDoor2);
        newDoor2.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
        newDoor2.GetComponent<Door>().isOpenForAll = true;
        newDoor2.GetComponent<Door>().old_player = player;
        gameManagerNetwork.SendPositionPlayer(player.GetComponent<PhotonView>().ViewID, playerGo.position_X, playerGo.position_Y);
        HidePlayerNotInSameRoom();
        if (voteDoorHasProposed)
        {
            ui_Manager.ResetNbVote();
            ui_Manager.DesactiveZoneDoor();
            voteDoorHasProposed = false;
            timer.ResetTimer();
            ClearDoor();
            ClearExpedition();
            ui_Manager.DisplayKeyAndTorch(true);
            alreadyPass = false;
            SetAlreadyHideForAllPlayers();
            ResetDoor();
        }
        ClearDoor();
        if (game.currentRoom.HasKey && game.currentRoom.availableKeyAnimation)
        {

            game.currentRoom.availableKeyAnimation = false;
            ui_Manager.LaunchAnimationAddKey();
        }
        ui_Manager.DisplayTutorialAutel(false);
        if (game.dungeon.initialRoom.X == game.currentRoom.X)
        {
            if (game.dungeon.initialRoom.Y == game.currentRoom.Y)
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);
                ui_Manager.DisplayTutorialAutel(true);
                if (paradiseHasChange)
                    ui_Manager.DisplayInterogationPoint();
            }

        }
        if (PhotonNetwork.IsMasterClient && setting.HELL_ROOM && !alreadyHell)
        {
            InsertHell();
        }
       
        if (!OnePlayerFindParadise && ((game.key_counter == 0 && !game.currentRoom.IsExit && !game.currentRoom.chest && (!game.currentRoom.isSacrifice || SacrificeIsUsedOneTimes))
            || game.currentRoom.IsHell || isAlreadyLoose))
        {
            if (!HaveMoreKeyInTraversedRoom() && !game.currentRoom.IsHell)
            {
                StartCoroutine(SacrificeAllLostSoul());
            }
            if (game.currentRoom.IsHell)
            {
                Loose();
                
            }
        }
        if (game.currentRoom.IsExit)
        {
            ResetDoor();
            Win();
        }
        if (GetPlayerMineGO().GetComponent<PlayerGO>().isChooseForExpedition)
        {
            GetPlayerMineGO().GetComponent<PlayerNetwork>().SendOnclickToExpedition();
        }
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);

        if (GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
        {
            GetDoorGo(indexNeWDoor3).GetComponent<Door>().isOpenForAll = false;
            GetDoorGo(indexNeWDoor3).GetComponent<Door>().gameObject.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
            GetPlayerMineGO().GetComponent<PlayerGO>().isInJail = false;
            gameManagerNetwork.SendIsInJail(false, GetPlayerMineGO().GetComponent<PhotonView>().ViewID, game.currentRoom.Index);
        }
        if(ISTrailsRoom(game.currentRoom) || game.nbTorch <= 0 || game.currentRoom.explorationIsUsed)
            ui_Manager.DisabledButtonPowerExploration(true);
        else
        {
            if(GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                ui_Manager.DisabledButtonPowerExploration(false);
        }
        gameManagerNetwork.SendUpdateDataPlayer(GetPlayerMineGO().GetComponent<PhotonView>().ViewID);

        ui_Manager.HideLightExplorationAllDoor();
        ui_Manager.DisplayAllDoorLightExploration(true);
    }


    public bool HaveMoreKeyInTraversedRoom()
    {
        game.dungeon.SetListRoomTraversed();
        foreach (Room room in game.dungeon.GetListRoomDiscoverd())
        {
            if (((room.isSacrifice && !SacrificeIsUsedOneTimes )|| room.chest) && !room.speciallyPowerIsUsed)
            {
                return true;
            }
        }
        return false;

    }

    public void HidePlayerNotInSameRoom()
    {
        PlayerGO myPlayer = GetPlayerMineGO().GetComponent<PlayerGO>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().ViewID != GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
            {
                PlayerGO other_player = player.GetComponent<PlayerGO>();
                if (myPlayer.position_X == other_player.position_X && myPlayer.position_Y == other_player.position_Y && !other_player.isSacrifice && !other_player.isInvisible)
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    player.transform.GetChild(0).gameObject.SetActive(false);
                    player.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    public void HidePlayerNotInSameRoom(int indexPlayer, bool hide)
    {

        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PhotonView>().ViewID == indexPlayer && !GetPlayer(indexPlayer).GetComponent<PlayerGO>().isSacrifice)
            {
                player.transform.GetChild(0).gameObject.SetActive(!hide);
                player.transform.GetChild(1).gameObject.SetActive(!hide);
            }
        }
    }

    public bool CheckDoorIsOpenByRoom(int x, int y, int indexDoor)
    {
        Room room = game.dungeon.GetRoomByPosition(x, y);
        return room.door_isOpen[indexDoor];
    }

    public bool RoomIsCurrent(int x, int y)
    {
        Room room = game.dungeon.GetRoomByPosition(x, y);
        if (room.GetIndex() == roomTeam.GetIndex())
        {
            return true;
        }
        return false;
    }

    public bool MajorityHasVotedDoor()
    {
        int counter = 0;
        bool isMajority;
        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PlayerGO>().hasVoteVD)
            {
                counter++;
            }
        }
        if (listPlayerTab.Length % 2 == 0)
        {
            isMajority = (counter >= listPlayerTab.Length / 2);
        }
        else
        {
            isMajority = (counter >= (listPlayerTab.Length / 2) + 1);
        }

        return isMajority;
    }
    public GameObject GetDoorWithMajority()
    {
        int voteMax = 0;
        int indexDoorCurrent = -1;
        //GameObject[] listDoor = TreeDoorById();
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in listDoor)
        {
            if (door.GetComponent<Door>().nbVote >= voteMax)
            {
                indexDoorCurrent = door.GetComponent<Door>().index;
                voteMax = door.GetComponent<Door>().nbVote;
            }

        }
        return GetDoorGo(indexDoorCurrent);

    }

    public bool AllPlayerAreInTheSameRoom()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayerTab)
        {
            foreach (GameObject playerB in listPlayerTab)
            {
                if (player.GetComponent<PhotonView>().ViewID != playerB.GetComponent<PhotonView>().ViewID)
                {
                    if ((player.GetComponent<PlayerGO>().position_X != playerB.GetComponent<PlayerGO>().position_X) || (player.GetComponent<PlayerGO>().position_Y != playerB.GetComponent<PlayerGO>().position_Y))
                    {
                        return false;
                    }
                }
            }
        }

        return true;

    }

    public List<GameObject> GetPlayerSameRoom(int indexPlayer)
    {
        GameObject[] listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listReturn = new List<GameObject>();
        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PlayerGO>().position_X == GetPlayer(indexPlayer).GetComponent<PlayerGO>().position_X)
            {
                if (player.GetComponent<PlayerGO>().position_Y == GetPlayer(indexPlayer).GetComponent<PlayerGO>().position_Y)
                {
                    if (!player.GetComponent<PlayerGO>().isSacrifice)
                    {
                        listReturn.Add(player);
                    }

                }
            }
        }
        return listReturn;
    }


    public bool AllPlayerBackToExpedition()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PlayerGO>().isInExpedition)
            {
                return false;
            }
        }
        return true;
    }

    public bool AllPlayerHaveHell()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayerTab)
        {
            if (!player.GetComponent<PlayerGO>().hellIsFind && !player.GetComponent<PlayerGO>().isInJail && !player.GetComponent<PlayerGO>().isSacrifice)
            {
                return false;
            }
        }
        return true;
    }

    public bool OnePlayerHaveToGoToExpedition()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
            {
                return true;
            }
        }
        return false;
    }
    public bool OnePlayerisGoInExpeditionOneTime()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PlayerGO>().isGoInExpeditionOneTime)
            {
                return true;
            }
        }
        return false;
    }

    public void InsertHell()
    {
        bool isCorrectRoom = false;
        if (Dungeon.GetDistance(game.currentRoom, game.dungeon.exit) == 1)
        {
            int random;
            Room roomPivot = null;
            int i = 0;
            do
            {
                random = Random.Range(0, 6);
                roomPivot = game.GetRoomByNeigbourID(i);
                if (!roomPivot.IsObstacle && !roomPivot.IsTraversed && !roomPivot.IsExit && roomPivot.distance_pathFinding_initialRoom == game.dungeon.exit.distance_pathFinding_initialRoom)
                {
                    isCorrectRoom = true;
                }
                i++;
            } while (!isCorrectRoom && i < 6);
            if (isCorrectRoom)
                gameManagerNetwork.SendHell(roomPivot.GetIndex());

            alreadyHell = true;
        }
    }

    public bool NbKeySufficient()
    {
        foreach (Room room in game.dungeon.rooms)
        {
            if (room.IsTraversed)
            {
                if (game.dungeon.GetPathFindingDistance(room, game.dungeon.exit) <= (game.key_counter))
                {
                    return true;
                }
            }
        }
        if (game.dungeon.GetPathFindingDistance(game.currentRoom, game.dungeon.exit) <= (game.key_counter))
        {
            return true;
        }

        return false;
    }




    /*
     * *
     *  Not Used, permit to mix index door 
     */
    public void MixLetterDoor()
    {
        List<int> indexAlreadyInsert = new List<int>();

        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        int randomInt;
        foreach (GameObject door in doors)
        {
            do
            {
                randomInt = Random.Range(0, 6);
            } while ((randomInt == door.GetComponent<Door>().index) || indexAlreadyInsert.Contains(randomInt));

            gameManagerNetwork.SendMixDoor(door.GetComponent<Door>().index, randomInt);
            door.GetComponent<Door>().index = randomInt;
            indexAlreadyInsert.Add(randomInt);
        }


    }

    public List<GameObject> GetDoorAvailable()
    {
        List<GameObject> listReturn = new List<GameObject>();

        //GameObject[] doors = TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (!door.GetComponent<Door>().isOpenForAll && !door.GetComponent<Door>().barricade)
            {
                listReturn.Add(door);
            }
        }

        return listReturn;

    }
    public IEnumerator PauseWhenPlayerTakeDoor(int idPlayer, int x, int y, float secondePause)
    {
        yield return new WaitForSeconds(secondePause);
        gameManagerNetwork.SendPositionPlayer(idPlayer, x, y);
    }

    public void Win()
    {
        paradiseIsFind = true;
        gameManagerNetwork.SendParadiseIsFind(GetPlayerMine().GetId());
        CloseDoorOveride();
        ui_Manager.DisplayParadise(false, -1);
        ui_Manager.ShowAllDataInMap();
        ui_Manager.ShowImpostor();
        ui_Manager.DisplayKeyAndTorch(false);



    }

    public void Loose()
    {
        hellIsFind = true;
        gameManagerNetwork.SendHellIsFind(hellIsFind, GetPlayerMine().GetId());
        CloseDoorOveride();
        ui_Manager.DisplayMainLevers(false);
        ui_Manager.DisplaySpeciallyLevers(false, 0);
        ui_Manager.DisplayHell(false);
        ui_Manager.HideNbKey();
        ui_Manager.ShowAllDataInMap();
        ui_Manager.ShowImpostor();
        ui_Manager.DisplayKeyAndTorch(false);
        ui_Manager.HideSpeciallyDisplay();
        ui_Manager.DisplayAutelTutorialSpeciallyRoom(false);

    }

    public void SendLoose()
    {
        gameManagerNetwork.SendLoose();
    }


    public IEnumerator CouroutineOpenDoorParadise()
    {
        yield return new WaitForSeconds(3);
        gameManagerNetwork.SendOpenDoorParadiseForAll();
    }

    public bool AllLostSoulFindParadise()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().isImpostor && !player.GetComponent<PlayerGO>().isInJail && !player.GetComponent<PlayerGO>().isSacrifice)
            {
                if (!player.GetComponent<PlayerGO>().paradiseIsFind)
                {
                    return false;
                }
            }
        }

        return true;

    }

    public bool AllPlayerGoneToParadise()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        nbPlayerInParadise++;

        if (players.Length == 3)
        {
            if (nbPlayerInParadise == players.Length - 1 - GetNbPlayerSacrifice())
            {
                return true;
            }
        }
        else
        {
            if (nbPlayerInParadise == players.Length - 2 - GetNbPlayerSacrifice())
            {
                return true;
            }

        }

        return false;
    }

    public bool AllPlayerGoneToHell()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        nbPlayerInHell++;

        if (players.Length == 3)
        {
            if (nbPlayerInHell == (players.Length - 1) - GetNbPlayerSacrifice())
            {
                return true;
            }
        }
        else
        {
            if (nbPlayerInHell == (players.Length - 2) - GetNbPlayerSacrifice())
            {
                return true;
            }

        }

        return false;
    }

    public int GetNbPlayerSacrifice()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                counter++;
        }

        return counter;
    }

    public void CloseDoorWhenVote(bool close)
    {
        for (int i = 0; i < doorsParent.transform.childCount; i++)
        {

            if (doorsParent.transform.GetChild(i).GetComponent<Door>().isOpenForAll)
            {
                doorsParent.transform.GetChild(i).GetComponent<Door>().IsCloseNotPermantly = close;
            }
        }
    }

    public void CloseDoorExplorationWhenVote(bool close)
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().isOpen)
            {
                //door.GetComponent<Door>().closeForTimerExploration = close;
                door.transform.Find("CollisionExplorationTimer").gameObject.SetActive(close);
            }
        }
    }

    public void ResetDoorExploration()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().isOpen && !door.GetComponent<Door>().isOpenForAll)
            {
                door.GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    public IEnumerator CloseDoorWhenVoteCoroutine(bool close)
    {
        yield return new WaitForSeconds(2);
        CloseDoorWhenVote(close);
    }

    public GameObject GetBoss()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isBoss)
            {
                return player;
            }
        }
        return null;
    }

    public bool SamePositionAtBoss()
    {
        if (GetBoss().GetComponent<PlayerGO>().position_X == GetPlayerMineGO().GetComponent<PlayerGO>().position_X)
        {
            if (GetBoss().GetComponent<PlayerGO>().position_Y == GetPlayerMineGO().GetComponent<PlayerGO>().position_Y)
            {
                return true;
            }
        }
        return false;
    }

    public bool SamePositionAtBossWithIndex(int index)
    {
        if (GetBoss().GetComponent<PlayerGO>().position_X == GetPlayer(index).GetComponent<PlayerGO>().position_X)
        {
            if (GetBoss().GetComponent<PlayerGO>().position_Y == GetPlayer(index).GetComponent<PlayerGO>().position_Y)
            {
                return true;
            }
        }
        return false;
    }

    public bool SamePositionAtMine(int index)
    {
        if (GetPlayerMineGO().GetComponent<PlayerGO>().position_X == GetPlayer(index).GetComponent<PlayerGO>().position_X)
        {
            if (GetPlayerMineGO().GetComponent<PlayerGO>().position_Y == GetPlayer(index).GetComponent<PlayerGO>().position_Y)
            {
                return true;
            }
        }
        return false;
    }

    public void SetAlreadyHideForAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerGO>().isAlreadyHide = false;
        }
    }

    public bool SamePositionAtInitialRoom()
    {
        if (game.dungeon.initialRoom.X == GetPlayerMineGO().GetComponent<PlayerGO>().position_X)
        {
            if (game.dungeon.initialRoom.Y == GetPlayerMineGO().GetComponent<PlayerGO>().position_Y)
            {
                return true;
            }
        }
        return false;

    }

    public void DesactivateColliderDoorToExplorater(int indexDoor, int indexPlayer)
    {
        if (indexPlayer == GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
        {
            GetDoorGo(indexDoor).transform.GetChild(5).GetComponent<CapsuleCollider2D>().enabled = false;
        }

    }

    public List<GameObject> GetAllImpostor()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listImpostor = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isImpostor)
            {
                listImpostor.Add(player);
            }
        }

        return listImpostor;
    }
    public void SetNamePlayerInList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            listNamePlayer.Add(player.GetComponent<PlayerGO>().playerName);
        }
    }

    public void  SendDataMine()
    {
        PlayerGO playerMineN2 = GetPlayerMineGO().GetComponent<PlayerGO>();
        PowerImpostor playerPowerImpostorTrap = playerMineN2.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
        ObjectImpostor playerObjectImpostor = playerMineN2.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
        dataGame.SetDataPlayerMine(playerMineN2.GetComponent<PhotonView>().ViewID, playerMineN2.transform.position.x, playerMineN2.transform.position.y,
            playerMineN2.position_X, playerMineN2.position_Y, playerMineN2.isImpostor, playerMineN2.isBoss, playerMineN2.isSacrifice,
            playerMineN2.isInJail, playerMineN2.isInvisible, playerMineN2.indexSkin, playerMineN2.playerName, playerMineN2.hasWinFireBallRoom,
            playerMineN2.GetComponent<PlayerNetwork>().userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
            playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, playerMineN2.isInExpedition);
    }

    public void UpdateSpecialsRooms(Room room)
    {
        isActuallySpecialityTime = false;
        ui_Manager.ClearSpecialRoom();
        ui_Manager.DisplaySpeciallyLevers(false, 0);
        ui_Manager.DisplayMainLevers(true);
        ui_Manager.DisplayAutelTutorialSpeciallyRoom(false);
        ui_Manager.ChangeColorAllPlayerSkinToFoggy(false);
        ui_Manager.DisplayLetterInSkull(false);
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);

        UpdateColorDoor(room);
        if (room.explorationIsUsed)
        {
            //ui_Manager.DisplayLeverExploration(false);
        }
        if (room.chest)
        {
            ui_Manager.DisplayChestRoom(true);
            ui_Manager.DisplayMainLevers(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.fireBall)
        {
            ui_Manager.DisplayFireBallRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isSacrifice)
        {
            ui_Manager.DisplaySacrificeRoom(true);
            if (!room.explorationIsUsed)
                ui_Manager.DisplayMainLevers(true);
            else
                ui_Manager.DisplayLeverVoteDoor(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed || SacrificeIsUsedOneTimes)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isJail)
        {
            ui_Manager.DisplayJailRoom(true);
            ui_Manager.HideDistanceRoom();
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplayJailRoom(false);
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.IsFoggy)
        {
            ui_Manager.DisplayFoggyRoom(true);
            ui_Manager.ChangeColorAllPlayerSkinToFoggy(true);
            ui_Manager.DisplayLeverVoteDoor(true);
            UpdateColorDoor(room);
            return;
        }
        if (room.IsVirus)
        {
            ui_Manager.DisplayVirusRoom(true);
            ui_Manager.DisplayLeverVoteDoor(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            UpdateColorDoor(room);
            return;
        }
        if (room.isDeathNPC)
        {
            ui_Manager.DisplayDeathNPCRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                //ui_Manager.DisplayDeathNPCRoom(false);
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isSwordDamocles)
        {
            ui_Manager.DisplayDamoclesSwordRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
             isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isAx)
        {
            ui_Manager.DisplayAxRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;

            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isSword)
        {
            ui_Manager.DisplaySwordRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;

            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isLostTorch)
        {
            ui_Manager.DisplayLostTorchRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplayLostTorchRoom(false);
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;

            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isMonsters)
        {
            ui_Manager.DisplayMonstersRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isPurification)
        {
            ui_Manager.DisplayPurificationRoom(true);
            if (!room.explorationIsUsed)
                ui_Manager.DisplayMainLevers(true);
            else
                ui_Manager.DisplayLeverVoteDoor(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            if (room.speciallyPowerIsUsed || PurificationIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isResurection)
        {
            ui_Manager.DisplayResurectionRoom(true);
            if (!room.explorationIsUsed)
                ui_Manager.DisplayMainLevers(true);
            else
                ui_Manager.DisplayLeverVoteDoor(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            if (room.speciallyPowerIsUsed || ResurectionIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isPray)
        {
            ui_Manager.DisplayPrayRoom(true);
            if (!room.explorationIsUsed)
                ui_Manager.DisplayMainLevers(true);
            else
                ui_Manager.DisplayLeverVoteDoor(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            if (room.speciallyPowerIsUsed || PrayIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isNPC)
        {
            ui_Manager.DisplayNPCRoom(true);
            if (!room.explorationIsUsed)
                ui_Manager.DisplayMainLevers(true);
            else
                ui_Manager.DisplayLeverVoteDoor(true);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            if (room.speciallyPowerIsUsed || NPCIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.isLabyrintheHide)
        {
            ui_Manager.DisplayLabyrinthRoom(true);
            ui_Manager.DisplayMainLevers(false);
            ui_Manager.DisplayAutelTutorialSpeciallyRoom(true);
            isActuallySpecialityTime = true;
            if (room.speciallyPowerIsUsed)
            {
                ui_Manager.DisplaySpeciallyLevers(false, 0);
                ui_Manager.DisplayLeverVoteDoor(true);
                isActuallySpecialityTime = false;
            }
            UpdateColorDoor(room);
            return;
        }
        if (room.IsExit || room.IsHell)
        {
            ui_Manager.DisplaySpeciallyLevers(false, 0);
            ui_Manager.DisplayMainLevers(false);
        }
    }




    public void UpdateColorDoor(Room room)
    {
        if (!GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;


        for(int i =0; i < doorsParent.transform.childCount; i++)
        {
            Door door = doorsParent.transform.GetChild(i).GetComponent<Door>();
            if (!doorsParent.transform.GetChild(i).GetComponent<Door>().GetRoomBehind().isTraped)
            {

                door.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(door.GetComponent<Door>().index, false);
            }
            else
            {
                door.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                door.transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(door.GetComponent<Door>().index, true);
            }
        }
        

        //GameObject[] doors = TreeDoorById();

        //doors[0].GetComponent<Door>().GetRoomBehind().istr;
        /*if (!room.left_neighbour.isTraped)
        {
            if (doors[0])
            {
                doors[0].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[0].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[0].GetComponent<Door>().index, false);
            }  
        }
        else
        {
            if (doors[0])
            {
                doors[0].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[0].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[0].GetComponent<Door>().index, true);

            }
        }

        if (!room.up_Left_neighbour.isTraped)
        {
            if (doors[1])
            {
                doors[1].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[1].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[1].GetComponent<Door>().index, false);
            }
        }
        else
        {
            if (doors[1])
            {
                doors[1].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[1].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[1].GetComponent<Door>().index, true);
            } 
        }

        if (!room.up_Right_neighbour.isTraped)
        {
            if (doors[2])
            {
                doors[2].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[2].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[2].GetComponent<Door>().index, false);
            }
        }
        else
        {
            if (doors[2])
            {
                doors[2].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[2].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[2].GetComponent<Door>().index, true);
            }
        }

        if (!room.right_neighbour.isTraped)
        {
            if (doors[3])
            {
                doors[3].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[3].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[3].GetComponent<Door>().index, false);
            }
        }
        else
        {
            if (doors[3])
            {
                doors[3].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[3].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[3].GetComponent<Door>().index, true);
            }
        }

        if (!room.down_Right_neighbour.isTraped)
        {
            if (doors[4])
            {
                doors[4].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[4].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[4].GetComponent<Door>().index, false);
            }
        }
        else
        {
            if (doors[4])
            {
                doors[4].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[4].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[4].GetComponent<Door>().index, true);
            }   
        }

        if (!room.down_Left_neighbour.isTraped)
        {
            if (doors[5])
            {
                doors[5].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                doors[5].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[5].GetComponent<Door>().index, false);
            }
        }
        else
        {
            if (doors[5])
            {
                doors[5].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                doors[5].transform.Find("couliss").GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                ui_Manager.SetRedColorDoorTrapedSpeciallyRoom(doors[5].GetComponent<Door>().index, true);
            }
        }*/

    }

    public void GetImpostorName()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isImpostor)
                listNamePlayerImpostor.Add(player.GetComponent<PlayerGO>().playerName);
        }
    }

    public int GetNumberOfPlayerSelected(int indexDoor)
    {
        int counter = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isChooseForExpedition && player.GetComponent<PlayerGO>().collisionInDoorIndex == indexDoor)
            {
                counter++;
            }
        }
        return counter;
    }

    public void UnBlockPlayer()
    {
        //ResetPosition();
        PlayerGO playerMine = GetPlayerMineGO().GetComponent<PlayerGO>();
        GetPlayerMineGO().transform.position = new Vector2(0, 0);
        gameManagerNetwork.SendAskReset(playerMine.GetComponent<PhotonView>().ViewID, playerMine.position_X, playerMine.position_Y);
    }

    public void ResetPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (GetPlayerMineGO().GetComponent<PhotonView>().ViewID != player.GetComponent<PhotonView>().ViewID)
            {
                GetPlayerMineGO().GetComponent<PlayerGO>().position_X = player.GetComponent<PlayerGO>().position_X;
                GetPlayerMineGO().GetComponent<PlayerGO>().position_Y = player.GetComponent<PlayerGO>().position_Y;


                return;
            }
        }
    }

    public bool AllPlayerHasQuitTutorielN7()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().quitTutorialN7)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator LaunchTimerChest()
    {
        speciallyIsLaunch = true;
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        yield return new WaitForSeconds(10);
        voteChestHasProposed = false;
        isActuallySpecialityTime = false;
        ui_Manager.DisplayAwardAndPenaltyForImpostor(false);
        ui_Manager.ResetAllPlayerLightAround();
        StartCoroutine(ResetAllPlayerLightAroundCoroutine());
        gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        if (SamePositionAtBoss())
        {
            GameObject chest = CalculVoteChest();
            ui_Manager.ActiveZoneVoteChest(false);
            ChestManagement(chest);
            StartCoroutine(CoroutineHideChest());
        }


        GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        game.dungeon.rooms[GetRoomOfBoss().GetComponent<Hexagone>().Room.Index].speciallyPowerIsUsed = true;
        CloseDoorWhenVote(false);
        speciallyIsLaunch = false;
        if (game.key_counter <= 0 && !HaveMoreKeyInTraversedRoom() && GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            SendLoose();
        }
    }
    public IEnumerator ResetAllPlayerLightAroundCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        ui_Manager.ResetAllPlayerLightAround();
    }
    public GameObject CalculVoteChest()
    {
        int pivot = 0;
        GameObject chestWithMoreVote = null;
        foreach (GameObject chest in TreeChestById())
        {
            int nbVote = chest.transform.Find("VoteZone").GetComponent<ChestZoneVote>().nbVote;

            if (pivot <= nbVote)
            {
                pivot = nbVote;
                chestWithMoreVote = chest;
            }
        }
        return chestWithMoreVote;
    }

    public void ChestManagement(GameObject chest)
    {
        if (chest.name == "BlueChest")
        {
            if (game.currentRoom.chestList[0].isAward)
            {
                if (!GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                {
                    chest.transform.Find("Award").gameObject.SetActive(true);
                    chest.transform.Find("Award").transform.GetChild(game.currentRoom.chestList[0].indexAward).gameObject.SetActive(true);
                }
            }
            else
            {
                if (GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                {
                    chest.transform.Find("Penalty").gameObject.SetActive(true);
                    chest.transform.Find("Penalty").transform.GetChild(game.currentRoom.chestList[0].indexAward).gameObject.SetActive(true);
                }
            }
            AddBonusAndPenaltyChest(0, game.currentRoom.chestList[0].indexAward, game.currentRoom.chestList[0].isAward);
        }
        else
        {
            if (chest.name == "RedChest")
            {
                if (game.currentRoom.chestList[1].isAward)
                {
                    if (!GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                    {
                        chest.transform.Find("Award").gameObject.SetActive(true);
                        chest.transform.Find("Award").transform.GetChild(game.currentRoom.chestList[1].indexAward).gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                    {
                        chest.transform.Find("Penalty").gameObject.SetActive(true);
                        chest.transform.Find("Penalty").transform.GetChild(game.currentRoom.chestList[1].indexAward).gameObject.SetActive(true);
                    }
                }
            }
            AddBonusAndPenaltyChest(1, game.currentRoom.chestList[1].indexAward, game.currentRoom.chestList[1].isAward);
        }
    }
    public void AddBonusAndPenaltyChest(int indexChest, int indexAward, bool isAward)
    {
        if (!GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }

        switch (indexAward)
        {
            case 0:
                if (isAward)
                {
                    if (!GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                    {
                        game.key_counter++;
                        game.nbTorch++;
                        gameManagerNetwork.SendAnimationAddKey();
                        ui_Manager.LaunchAnimationAddKey();
                        gameManagerNetwork.SendTorchNumber(game.nbTorch);
                        ui_Manager.SetTorchNumber();
                    }

                }
                else
                {
                    if (GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                    {
                        game.key_counter--;
                        game.nbTorch--;
                        gameManagerNetwork.SendAnimationBrokenKey();
                        ui_Manager.LaunchAnimationBrokenKey();
                        gameManagerNetwork.SendTorchNumber(game.nbTorch);
                        ui_Manager.SetTorchNumber();
                    }
                }
                gameManagerNetwork.SendKey(game.key_counter);
                ui_Manager.SetNBKey();
                return;
            case 1:
                if (isAward)
                {
                    if (!GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                        game.nbTorch++;

                }
                else
                {
                    if (GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                        game.nbTorch--;
                }
                gameManagerNetwork.SendTorchNumber(game.nbTorch);
                ui_Manager.SetTorchNumber();
                return;
            case 2:
                if (isAward)
                {
                    if (!GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                        gameManagerNetwork.SendDistanceAwardChest(indexChest);
                }
                else
                {
                    if (GetRoomOfBoss().GetComponent<Hexagone>().Room.isTraped)
                        ChangePositionParadise();
                }

                return;
        }
    }

    public IEnumerator CoroutineHideChest()
    {
        yield return new WaitForSeconds(4);

        ui_Manager.ResetChestRoom();
        //ui_Manager.DisplayChestRoom(false);
        //CloseAllDoor(game.currentRoom, false);
    }

    public void ChangePositionParadise()
    {
        if (!GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        game.dungeon.exit.isOldParadise = true;

        List<Room> listRoomWithCorrectDistance = listRoomAtDistanceOfIsTraversed();

        if (listRoomWithCorrectDistance.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, listRoomWithCorrectDistance.Count);
        Room newParadise = listRoomWithCorrectDistance[randomIndex];
        game.dungeon.exit.IsExit = false;
        game.dungeon.exit = newParadise;
        newParadise.IsExit = true;
        gameManagerNetwork.SendChangementParadiseBool(true);
        gameManagerNetwork.SendNewParadise(newParadise.Index);
        SetCurrentRoomColor();
        game.dungeon.SetPathFindingDistanceAllRoom();
        ui_Manager.SetDistanceTextAwardChest(0);
        ui_Manager.SetDistanceTextAwardChest(1);
    }

    public List<Room> listRoomAtDistanceOfIsTraversed()
    {
        List<Room> listRoomWithCorrectDistance = new List<Room>();
        game.dungeon.SetListRoomTraversed();
        int distanceParadiseCurrentRoom = game.currentRoom.DistancePathFinding;
        foreach (Room room in game.dungeon.listRoomTraversed)
        {
            if (game.key_counter <= 0)
                listRoomWithCorrectDistance.AddRange(game.dungeon.GetListRoomByDistance(room, Random.Range(1, 2)));
            else
                listRoomWithCorrectDistance.AddRange(game.dungeon.GetListRoomByDistance(room, game.key_counter));
        }
        return listRoomWithCorrectDistance;
    }



    public void ResetFireBallRoom()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            turret.GetComponent<Turret>().DestroyFireBalls();
            turret.GetComponent<Turret>().canFire = false;
        }
        CloseDoorWhenVote(false);
    }
    public void InsertSpeciallyRoom(Room room, bool isInExpedition)
    {
        if (!(GetPlayerMineGO().GetComponent<PlayerGO>().isBoss || (GetPlayerMineGO().GetComponent<PlayerGO>().isChooseForExpedition && isInExpedition)))
        {
            return;
        }

        if ((room.IsExit || room.IsHell || room.isSpecial || room.IsInitiale || room.isTraped) || !room.isHide)
        {
            return;
        }
        if (game.dungeon.GetPathFindingDistance(room, game.dungeon.initialRoom) == game.dungeon.GetPathFindingDistance(game.dungeon.initialRoom, game.dungeon.exit))
            return;

        int indexSpeciality = -1;
        if (counterRoom % 2 != 0)
        {
            indexSpeciality = InsertRandomSpeciallity(room);
        }
        else
        {
            indexSpeciality = InsertSpeciallyN2(room);
        }
        counterRoom++;

        if (indexSpeciality > -1)
        {
            room.isSpecial = true;
            gameManagerNetwork.SendUpdateNeighbourSpeciality(room.Index, indexSpeciality);

        }
        else
        {
            room.isNotSpecial = true;
        }
    }

    public bool ISTrailsRoom(Room room)
    {
        if(room.fireBall || room.isAx || room.isSword)
            return true;
        if (room.isSwordDamocles || room.isDeathNPC || room.isMonsters)
            return true;
        if (room.isLabyrintheHide || room.isLostTorch)
            return true;

        return false;
    }

    public int InsertRandomSpeciallity(Room room)
    {
        if (room.HaveOneNeighbour())
        {
            return -1;
        }
        int randomMain = Random.Range(0, 6);
        if (randomMain == 0 && setting.listSpeciallyRoom[0])
        {
            game.dungeon.InsertChestRoom(room.Index);
            room.chest = true;
            return 0;
        }
        if (randomMain == 1 && setting.listSpeciallyRoom[1])
        {
            room.isSacrifice = true;
            return 2;
        }
        if (randomMain == 2 && setting.listSpeciallyRoom[3])
        {
            room.isNPC = true;
            return 10;
        }
        if (randomMain == 3)
        {
            int randomStatus = Random.Range(0, 3);
            if (randomStatus == 0 && setting.listSpeciallyRoom[4])
            {
                room.isResurection = true;
                return 11;
            }

            if (randomStatus == 1 && setting.listSpeciallyRoom[5])
            {
                room.isPurification = true;
                return 12;
            }
            if (randomStatus == 2 && setting.listSpeciallyRoom[6])
            {
                room.isPray = true;
                return 13;
            }
        }
        return -1;
    }

    public int InsertSpeciallyN2(Room room)
    {
        if (room.HaveOneNeighbour())
        {
            return -1;
        }

        int randomMain = Random.Range(0, 8);
        //randomMain = 7;
/*        if (randomMain == 0 && setting.listTrialRoom[0])
        {
            room.fireBall = true;
            return 1;
        }*/
        if (randomMain == 1 && setting.listTrialRoom[1])
        {
            room.isDeathNPC = true;
            return 3;
        }
        if (randomMain == 2 && setting.listTrialRoom[2])
        {
            room.isSwordDamocles = true;
            return 4;
        }
        if (randomMain == 3 && setting.listTrialRoom[3])
        {
            room.isAx = true;
            return 5;
        }
        if (randomMain == 4 && setting.listTrialRoom[4])
        {
            room.isSword = true;
            return 6;
        }
        if (randomMain == 5 && setting.listTrialRoom[6])
        {
            room.isLostTorch = true;
            room.isLostTorch = true;
            return 7;
        }
        if (randomMain == 6 && setting.listTrialRoom[5])
        {
            room.isMonsters = true;
            return 8;
        }
        if (randomMain == 7 && setting.listTrialRoom[7])
        {
            room.isLabyrintheHide = true;
            return 9;
        }
        return -1;
    }

    public int GetWitdhOfAllHexagone()
    {
        int X_max = -1;
        int X_min = 1000;

        int index_x_max = 0;
        int inde_x_min = 0;

        foreach (Hexagone hexagone in dungeon)
        {
            if (hexagone.Room.IsObstacle)
            {
                continue;
            }
            if (hexagone.Room.distance_pathFinding_initialRoom == -1)
            {
                continue;
            }
            if (hexagone.Room.X > X_max)
            {
                index_x_max = hexagone.Room.Index;
                X_max = hexagone.Room.X;
            }
            if (hexagone.Room.X < X_min)
            {
                inde_x_min = hexagone.Room.Index;
                X_min = hexagone.Room.X;
            }
        }
        return (game.dungeon.GetRoomByIndex(index_x_max).X
            - game.dungeon.GetRoomByIndex(inde_x_min).X);

    }

    public int GetHeightOfAllHexagone()
    {
        int y_max = 0;
        int y_min = 1000;

        int index_y_max = 0;
        int index_y_min = 0;

        foreach (Hexagone hexagone in dungeon)
        {
            if (hexagone.Room.IsObstacle)
            {
                continue;
            }
            if (hexagone.Room.distance_pathFinding_initialRoom == -1)
            {
                continue;
            }
            if (hexagone.Room.Y > y_max)
            {
                y_max = hexagone.Room.Y;
                index_y_max = hexagone.Room.Index;
            }
            if (hexagone.Room.Y < y_min)
            {
                y_min = hexagone.Room.Y;
                index_y_min = hexagone.Room.Index;
            }
        }

        return (game.dungeon.GetRoomByIndex(index_y_max).Y - game.dungeon.GetRoomByIndex(index_y_min).Y);
    }

    public GameObject GetRoomOfBoss()
    {
        int X_boss = GetBoss().GetComponent<PlayerGO>().position_X;
        int Y_boss = GetBoss().GetComponent<PlayerGO>().position_Y;

        foreach (Hexagone hexa in dungeon)
        {
            if (hexa.Room.X == X_boss && hexa.Room.Y == Y_boss)
                return hexa.gameObject;
        }

        return null;
    }

    public void HideReadAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.gameObject.transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(false);
            player.gameObject.transform.Find("ActivityCanvas").Find("X_vote").gameObject.SetActive(false);
        }
    }

    public void ChangeLeverDeathNPC()
    {
        gameManagerNetwork.SendChangeLeverDeathNPC();

    }

    public void TeleportAllPlayerInRoomOfBoss()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
            {
                if (player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
                    continue;
                player.GetComponent<PlayerNetwork>().SendTeleportPlayerToSameRoomOfBoss();
            }
        }
    }
    public void TeleportAllPlayerInRoomOfBossEvenSameRoom()
    {
        GameObject playerMine = GetPlayerMineGO();
        if (playerMine.GetComponent<PlayerGO>().isSacrifice || playerMine.GetComponent<PlayerGO>().isInJail)
            return;

        playerMine.transform.position = new Vector3(0, 0);
        playerMine.GetComponent<PlayerGO>().canMove = false;

    }

    public void InstantiateDeathNPC()
    {
        if (!GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        GameObject DeathNPCRoom = GameObject.Find("Special").transform.Find("DeathNPCRoom").gameObject;
        PhotonNetwork.Instantiate("DeathNpc", DeathNPCRoom.transform.Find("SpawnDeathNPC").transform.position, Quaternion.identity);
    }


    public void CancelDoorExplorationWhenDisconnection(int indexDoor)
    {
        if (expeditionHasproposed)
        {
            gameManagerNetwork.SendCancelDoorExploration(indexDoor);
            ResetLeverDisconnect();
        }
    }

    public Door GetDoorExplorator(int indexPlayer)
    {
        DisplayDictionnaryDoorPlayer();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            if (!door_idPlayer.ContainsKey(door.GetComponent<Door>().index))
                continue;
            if (door_idPlayer[door.GetComponent<Door>().index] == indexPlayer)
                return door.GetComponent<Door>();
        }

        foreach( Expedition expe in game.current_expedition)
        {
            Debug.LogError(expe.indexNeigbour);
            if (expe.player.GetId() == indexPlayer)
                return GetDoorGo(expe.indexNeigbour).GetComponent<Door>();
        }

        return null;
    }

    public Door GetDoorExploraterWithRoom(Room roomExplore)
    {
        if( roomExplore.X < game.currentRoom.X && roomExplore.Y == game.currentRoom.Y)
        {
            return GetDoorGo(0).GetComponent<Door>();
        }
        if (roomExplore.X == game.currentRoom.X && roomExplore.Y < game.currentRoom.Y)
        {
            return GetDoorGo(1).GetComponent<Door>();
        }
        if (roomExplore.X > game.currentRoom.X && roomExplore.Y < game.currentRoom.Y)
        {
            return GetDoorGo(2).GetComponent<Door>();
        }
        if (roomExplore.X > game.currentRoom.X && roomExplore.Y == game.currentRoom.Y)
        {
            return GetDoorGo(3).GetComponent<Door>();
        }
        if (roomExplore.X > game.currentRoom.X && roomExplore.Y > game.currentRoom.Y)
        {
            return GetDoorGo(4).GetComponent<Door>();
        }
        if (roomExplore.X == game.currentRoom.X && roomExplore.Y > game.currentRoom.Y)
        {
            return GetDoorGo(5).GetComponent<Door>();
        }
        return null;
    }

    public void ResetLeverDisconnect()
    {
        gameManagerNetwork.SendDisplayDoorLever(true);
        gameManagerNetwork.DisplayLightAllAvailableDoor(true);
        GetRoomOfBoss().GetComponent<Hexagone>().Room.explorationIsUsed = true;
    }

    public void DisplayDictionnaryDoorPlayer()
    {
        Debug.Log("display Dictionnay : ");


        foreach (KeyValuePair<int, int> dic in door_idPlayer)
        {
            Debug.LogError(dic.Key + " " + dic.Value);
        }
    }
    public void UpdataDataPlayer(int indexPlayer)
    {
        dataGame.SetDataPlayerById(indexPlayer);
    }

    public void TestLastPlayerSpeciallayRoom()
    {
        if (GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
            return;
        if (GetPlayerMineGO().GetComponent<PlayerGO>().isSacrifice)
            return;
        if (game.currentRoom.fireBall)
        {
            GameObject.Find("Turret").GetComponent<Turret>().Victory();
            GameObject.Find("FireBallRoom").GetComponent<FireBallRoom>().DisplayLeverToRelauch();
        }
        if (game.currentRoom.isSword)
        {
            int indexSkin = GetPlayerMineGO().GetComponent<PlayerGO>().indexSkin;
            GetPlayerMineGO().transform.Find("Skins").GetChild(indexSkin).Find("Sword").Find("Final").GetComponent<Sword>().Victory();
        }
        if (game.currentRoom.isSwordDamocles)
        {
            GameObject.Find("DamoclesSwordRoom").GetComponent<DamoclesSwordRoom>().Victory();
        }
        if (game.currentRoom.isDeathNPC)
        {
            GameObject.Find("DeathNPCRoom").GetComponent<DeathNpcRoom>().DisplayLeverToRelauch();
            if(GameObject.Find("DeathNpc(Clone)"))
                GameObject.Find("DeathNpc(Clone)").GetComponent<Death_NPC>().Victory();
            
        }
        if (game.currentRoom.isAx)
        {
            GameObject.Find("Ax").GetComponent<Ax>().Victory();
        }

        if (game.currentRoom.isMonsters)
        {
            GameObject.Find("MonsterInRoom").GetComponent<MonsterNPC>().Victory();
            GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>().DisplayLeverToRelauch();
        }
        if (game.currentRoom.isLabyrintheHide)
        {
            StartCoroutine(GameObject.Find("LabyrinthHideRoom").GetComponent<LabyrinthHideRoom>().DisplayLeverToRelauch());
        }

    }


    public bool VerifyHasWinFireBall()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
                return true;
        }
        return false;
    }
    public int GetRandomPlayerID()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        return players[Random.Range(0, players.Length)].GetComponent<PhotonView>().ViewID;
    }

    public bool ThereIsLever()
    {
        if (ui_Manager.MainRoomGraphic.transform.Find("Levers").transform.Find("OpenDoor_lever").gameObject.activeSelf)
        {
            return true;
        }

        int countChild = ui_Manager.MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").Find("Specially").childCount;
        for (int i = 0;  i < countChild; i++)
        {
            if (ui_Manager.MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").Find("Specially").GetChild(i).gameObject.activeSelf)
            {
                return true;
            }
        }

        Debug.Log("return false");
        return false;
    }

    public void RandomWinFireball()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int randomInt = Random.Range(0, players.Length);
        players[randomInt].GetComponent<PlayerGO>().hasWinFireBallRoom = true;
        players[randomInt].GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        players[randomInt].GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        players[randomInt].GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        players[randomInt].GetComponent<PlayerNetwork>().SendCanLaunchExploration();
       
    }

    public IEnumerator VerifyBugExplorationCouroutine()
    {
        yield return new WaitForSeconds(1.5f);
        if (GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            if (alreaydyExpeditionHadPropose)
            {
                if (GetPlayerMineGO().GetComponent<PlayerGO>().canLaunchExplorationLever)
                {
                    alreaydyExpeditionHadPropose = false;
                }
            }
            else
            {
                if (!GetPlayerMineGO().GetComponent<PlayerGO>().canLaunchExplorationLever)
                {
                    GetPlayerMineGO().GetComponent<PlayerGO>().canLaunchExplorationLever = true;
                }
            }
            
        }

    }

    public IEnumerator CouroutineSacrificeAllPlayer()
    {
        yield return new WaitForSeconds(15);
        if(!alreadySacrifice)
            StartCoroutine(SacrificeAllLostSoul());
    }

    public IEnumerator SacrificeAllLostSoul()
    {
        ui_Manager.HideSpeciallyDisplay();
        yield return new WaitForSeconds(4);
        if (!GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDeathSacrifice(false);
        }
        UpdateDataInformationInEndGame();
        alreadySacrifice = true;
        StartCoroutine(CouroutineDisplayEndPanel());
        StartCoroutine(SacrificeAFKplayer());
    }

    public void UpdateDataInformationInEndGame()
    {
        ui_Manager.DisplayMainLevers(false);
        ui_Manager.DisplaySpeciallyLevers(false, 0);
        ui_Manager.ShowAllDataInMap();
        ui_Manager.ShowImpostor();
    }

    public IEnumerator CouroutineDisplayEndPanel()
    {
        yield return new WaitForSeconds(5);
        ui_Manager.DisplayBlackScreenToDemonWhenAllGone();
    }
    public IEnumerator SacrificeAFKplayer()
    {
        yield return new WaitForSeconds(10);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerNetwork>().SendSacrificePlayerAfk();
        }
    }

    public void ActivateCollisionTPOfAllDoor(bool activate)
    {
        if (!SamePositionAtBoss())
            return;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in doors)
        {
            door.transform.Find("teleportation").gameObject.SetActive(activate);
        }
    }

    public void ResetZoneDoor(string doorName)
    {
        for(int i=1; i < doorsParent.transform.Find(doorName).transform.Find("Zones").childCount; i++)
        {
            doorsParent.transform.Find(doorName).transform.Find("Zones").GetChild(i).gameObject.SetActive(false);
        }
    }
    public void DisplayZoneDoorForEachSituation()
    {
        ResetZoneDoor("A");
        ResetZoneDoor("B");
        ResetZoneDoor("C");
        ResetZoneDoor("D");
        ResetZoneDoor("E");
        ResetZoneDoor("F");
        if (HasDoor(0) && HasDoor(1) && HasDoor(5)){
            doorsParent.transform.Find("A").transform.Find("Zones").Find("AllZone").gameObject.SetActive(true);
        }
        else if(HasDoor(0) && HasDoor(1))
        {
            doorsParent.transform.Find("A").transform.Find("Zones").Find("TwoZoneB").gameObject.SetActive(true);
        }
        else if (HasDoor(0) && HasDoor(5))
        {
            doorsParent.transform.Find("A").transform.Find("Zones").Find("TwoZoneF").gameObject.SetActive(true);
        }
        else if (HasDoor(1) && HasDoor(5))
        {
            doorsParent.transform.Find("B").transform.Find("Zones").Find("middleZone").gameObject.SetActive(true);
        }
        else if (HasDoor(0))
        {
            doorsParent.transform.Find("A").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }
        else if (HasDoor(1))
        {
            doorsParent.transform.Find("B").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }
        else if (HasDoor(5))
        {
            doorsParent.transform.Find("F").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }

        if (HasDoor(3) && HasDoor(2) && HasDoor(4))
        {
            doorsParent.transform.Find("D").transform.Find("Zones").Find("AllZone").gameObject.SetActive(true);
        }
        else if (HasDoor(3) && HasDoor(2))
        {
            doorsParent.transform.Find("D").transform.Find("Zones").Find("TwoZoneB").gameObject.SetActive(true);
        }
        else if (HasDoor(3) && HasDoor(4))
        {
            doorsParent.transform.Find("D").transform.Find("Zones").Find("TwoZoneF").gameObject.SetActive(true);
        }
        else if (HasDoor(2) && HasDoor(4))
        {
            doorsParent.transform.Find("C").transform.Find("Zones").Find("middleZone").gameObject.SetActive(true);
        }
        else if (HasDoor(3))
        {
            doorsParent.transform.Find("D").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }
        else if (HasDoor(2))
        {
            doorsParent.transform.Find("C").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }
        else if (HasDoor(4))
        {
            doorsParent.transform.Find("E").transform.Find("Zones").Find("OnlyZone").gameObject.SetActive(true);
        }
    }

    public bool HasDoor(int index)
    {

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in doors)
        {
            if (door.GetComponent<Door>().index == index)
                return true;
        }

        return false;
    }
}
