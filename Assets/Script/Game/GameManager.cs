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
    Dictionary<int, int> door_idPlayer;
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


    private void Awake()
    {
        gameManagerNetwork = gameObject.GetComponent<GameManagerNetwork>();
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ui_Manager.DisplayLoadPage(true);
        timer.LaunchTimer(2, false);
        //Camera.main.aspect = 1920 / 1080f;
        door_idPlayer = new Dictionary<int, int>();
        game = Game.CreateInstance(listPlayer);
        listPlayer = new List<PlayerDun>();
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
        SetTABToList(listPlayerTab, listPlayer);
        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<Setting>();
        game.setting = setting;
        game.Launch(15, 15);
        MasterClientCreateMap();
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


    }


    public void MasterClientCreateMap()
    {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        

        gameManagerNetwork.SendSetting(setting.NUMBER_EXPEDTION_MAX, setting.DISPLAY_MINI_MAP,
            setting.DISPLAY_OBSTACLE_MAP, setting.DISPLAY_KEY_MAP, setting.RANDOM_ROOM_ADDKEYS,
            setting.LIMITED_TORCH, setting.TORCH_ADDITIONAL);

        game.CreationMap();
        game.ChangeBoss();
        game.AssignRole();
        SendRole();
        SendMap();
        ui_Manager.SetDistanceRoom(game.currentRoom.DistancePathFinding, game.currentRoom);
        if (setting.LIMITED_TORCH)
        {
            game.nbTorch = game.dungeon.GetPathFindingDistance(game.currentRoom, game.dungeon.exit) + setting.TORCH_ADDITIONAL;
            gameManagerNetwork.SendTorchNumber(game.nbTorch);
            ui_Manager.SetTorchNumber();
        }
        GenerateHexagone(-7, 3.5f);
        GenerateObstacle();
        SetDoorObstacle(game.currentRoom);
        SetPositionHexagone();
        SendBoss();
        game.SetKeyCounter();
        gameManagerNetwork.SendKey(game.key_counter);
        ui_Manager.SetNBKey();
        SetInitialPositionPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        // vote doorr
        // in if( timer.timerFinish && MajorityHasVotedDoor())
        if (timer.timerFinish && voteDoorHasProposed)
        {
            ui_Manager.DesactivateLightAroundPlayers();

            GameObject door = GetDoorWithMajority();

            if (SamePositionAtBoss() && VerifyVoteVD(door.GetComponent<Door>().nbVote))
            {
                if (door.GetComponent<Door>().nbVote == 0 || game.currentRoom.IsVirus)
                {
                    if (PhotonNetwork.IsMasterClient)
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
                    //game.key_counter--;
                    alreaydyExpeditionHadPropose = false;
                }
            }
            StartCoroutine(ChangeBossCoroutine(0.5f));
            ui_Manager.ResetNbVote();
            ui_Manager.DesactiveZoneDoor();
            voteDoorHasProposed = false;
            timer.ResetTimer();
            ClearDoor();
            ClearExpedition();
            ui_Manager.DisplayKeyAndTorch(true);
            alreadyPass = false;
            SetAlreadyHideForAllPlayers();
            ui_Manager.DisplayMainLevers(true);
            //ui_Manager.display

            if (game.dungeon.initialRoom.HasSameLocation(game.currentRoom))
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);

            }

            CloseDoorWhenVote(false);
            ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = 0;
        }

        // vote expedition
        LaunchExploration(AllPlayerHasVoted(), VerifyVote());


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
            ui_Manager.DisplayEchapMenu();
        }

        if (OnePlayerisGoInExpeditionOneTime())
        {
            if (!OnePlayerHaveToGoToExpedition())
            {
                if (setting.displayTutorial && !ui_Manager.listTutorialBool[11])
                {
                    gameManagerNetwork.SendTutorialN11();
                }
            }
        }
    }


    public void LaunchExploration(bool AllPlayerHasVoted, bool VerifyVote)
    {
        if (AllPlayerHasVoted && !endGame && !alreaydyExpeditionHadPropose && !isLoading)
        {
            timer.ResetTimer();
            ui_Manager.HideZoneVote();
            if (VerifyVote)
            {
                if (setting.LIMITED_TORCH)
                    SetNbTorch(game.current_expedition.Count);
                if (SamePositionAtBoss())
                    OpenDoorsToExpedition();
                alreaydyExpeditionHadPropose = true;
                SetPlayersHaveTogoToExpeditionBool();
                if (setting.displayTutorial)
                {
                    if (!ui_Manager.listTutorialBool[8])
                    {
                        ui_Manager.tutorial_parent.SetActive(true);
                        ui_Manager.tutorial[8].SetActive(true);
                        ui_Manager.listTutorialBool[8] = true;
                    }


                }

            }
            else
            {
                ui_Manager.DisplayAllGost(false);
                ChangeBoss();
                ClearExpedition();
                ClearDoor();
                expeditionHasproposed = false;
                ui_Manager.DisplayMainLevers(true);
            }
            ResetVoteCP();
            ui_Manager.DisplayKeyAndTorch(true);

            if (game.dungeon.initialRoom.X == game.currentRoom.X)
            {
                if (game.dungeon.initialRoom.Y == game.currentRoom.Y)
                {
                    ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);
                }

            }
            CloseDoorWhenVote(false);
            alreadyPass = false;
            SetAlreadyHideForAllPlayers();
            canResetTimerBoss = true;
        }
    }

    public void GenerateHexagone(float initial_X, float initial_Y)
    {
        foreach (Room room in game.dungeon.rooms)
        {
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

    public void GenerateObstacle()
    {
        foreach (Hexagone hex in dungeon)
        {
            SetHexagoneColor(hex);
        }
    }

    public void SetHexagoneColor(Hexagone hex)
    {
        Room room = hex.Room;
        //hex.GetComponent<Hexagone>().distanceText.text = room.distance_pathFinding_initialRoom.ToString();
        hex.GetComponent<Hexagone>().distanceText.text = game.dungeon.GetPathFindingDistance(room, game.currentRoom).ToString();


        if (room.IsObstacle)
        {
            if (setting.DISPLAY_OBSTACLE_MAP || GetPlayerMine().GetIsImpostor())
            {
                hex.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                hex.GetComponent<Hexagone>().distanceText.text = "";
                hex.GetComponent<Hexagone>().index_text.text = "";
                //Destroy(room);
            }
        }

        if (room.IsInitiale)
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
/*            if (!room.IsExit)
                room.IsTraversed = true;*/
            hexagone_current = hex;
        }
        if (room.HasKey)
        {
            if (setting.DISPLAY_KEY_MAP || GetPlayerMine().GetIsImpostor())
            {
                hex.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                /*                room.GetComponent<Hexagone>().distanceText.text = "";
                                room.GetComponent<Hexagone>().index_text.text = "";*/
            }
        }
        if (room.IsFoggy && GetPlayerMine().GetIsImpostor())
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(87 / 255f, 89 / 255f, 96 / 255f);
        }
        if (room.IsVirus && GetPlayerMine().GetIsImpostor())
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(66 / 255f, 0 / 255f, 117 / 255f);
        }
        if (room.IsExit && GetPlayerMine().GetIsImpostor())
        {
            hex.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            hex.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            if (room.HasKey)
            {
                hex.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                hex.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            }
        }
        if (room.chest)
        {
            //hex.GetComponent<SpriteRenderer>().color = new Color(15 / 255f, 15 / 255f, 15 / 255f);
        }
        if (room.IsTraversed)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
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
            hexagone.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);

            if (hexagone.GetComponent<Hexagone>().Room.Index == game.currentRoom.GetIndex())
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                if (!hexagone.GetComponent<Hexagone>().Room.IsExit)
                    hexagone.GetComponent<Hexagone>().Room.IsTraversed = true;

            }
            else
            {
                if (hexagone.GetComponent<Hexagone>().Room.IsTraversed)
                {
                    if (!hexagone.GetComponent<Hexagone>().Room.IsExit)
                        hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
                }
            }
            if (hell && GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                if (hexagone.GetComponent<Hexagone>().Room.X == hell.X && hexagone.GetComponent<Hexagone>().Room.Y == hell.Y)
                {
                    hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(255 / 255f), (float)0f / 255f, (float)0f / 255f, 1);
                    hexagone.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);

                    if (hexagone.GetComponent<Hexagone>().Room.HasKey)
                    {
                        hexagone.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                        hexagone.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
                    }
                }
            }
            if (hexagone.GetComponent<Hexagone>().Room.IsExit && GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                hexagone.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
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
                }
                else
                {
                    hexagone.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                }

            }
        }
    }

    public void SetDoorObstacle(Room room)
    {
        List<int> listDoorObstacleIndex = game.GetDoorObstacle(room);

        foreach (int indexRoomObstacle in listDoorObstacleIndex)
        {
            ui_Manager.DisplayObstacleInDoor(indexRoomObstacle, true);
        }
    }


    public void SetDoorNoneObstacle(Room room)
    {
        List<int> listDoorObstacleIndex = game.GetDoorNoneObstacle(room);
        foreach (int indexRoomObstacle in listDoorObstacleIndex)
        {
            ui_Manager.DisplayObstacleInDoor(indexRoomObstacle, false);
        }
    }


    public void SetPositionHexagone()
    {
        map.transform.position = new Vector3(-2.7f, 4.8f, -1);
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
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

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
        roomTeam = game.GetRoomById(indexDoor);

        soundOpenDoor.Play();


        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);

        if (!isExpedition)
        {
            door.GetComponent<Door>().isOpenForAll = true;
            game.currentRoom.door_isOpen[door.GetComponent<Door>().index] = true;

            //roomTeam.IsTraversed = true;
            if (roomTeam.HasKey && PhotonNetwork.IsMasterClient)
                gameManagerNetwork.AddKey(roomTeam.GetIndex());
            if (roomTeam.IsExit || roomTeam.IsHell)
            {
                gameManagerNetwork.SendParadiseOrHellFind(roomTeam.IsExit, roomTeam.IsHell);

            }

        }

        gameManagerNetwork.SendOpenDoor(indexDoor, game.currentRoom.X, game.currentRoom.Y, isExpedition, roomTeam.GetIndex());



    }

    public void OpenDoorsToExpedition()
    {
        foreach (Expedition expe in game.current_expedition)
        {

            GameObject[] doors = TreeDoorById();

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

            GameObject[] players = listPlayerTab;

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
            ui_Manager.DisplayAllGost(true);
            StartCoroutine(CoroutineLaunchExploration());
        }
        else
        {
            gameManagerNetwork.LaunchTimerExpedition();
        }

        
    }

    public IEnumerator CoroutineLaunchExploration()
    {
        yield return new WaitForSeconds(0.2f);
        LaunchExploration(true, true);
    }

    public bool VerificationExpedition(Dictionary<int, int> door_idPlayer)
    {
        if (door_idPlayer.Count == 0)
            return false;

        if (setting.LIMITED_TORCH && game.nbTorch > 0 && door_idPlayer.Count <= game.nbTorch)
            return true;

        if (door_idPlayer.Count == game.NumberExpeditionAvailable() && !setting.LIMITED_TORCH)
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

        //  set L'UI
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
            gameManagerNetwork.SendMap(room.Index, room.IsExit, room.IsObstacle,
                room.IsInitiale, room.DistanceExit, room.DistancePathFinding,
                room.distance_pathFinding_initialRoom, counter == game.dungeon.rooms.Count, room.IsFoggy, room.IsVirus, room.HasKey, room.chest);

            if (room.chest)
            {
                for (int i = 0; i < 2; i++)
                {
                    gameManagerNetwork.SendChestData(room.GetIndex(), room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward);
                }
            }
            if (room.fireBall)
            {
                gameManagerNetwork.SendFireBallData(room.GetIndex(), room.fireBall);
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
    /*    public int NbPlayerAccept()
        {
            int counter = 0;
            foreach (PlayerDun player in listPlayer)
            {
                if (player.GetVote_CP())
                {
                    counter++;
                }
            }
            return counter;
        }
        public int NbPlayerRefuse()
        {
            int counter = 0;
            foreach (PlayerDun player in listPlayer)
            {
                if (!player.GetVote_CP())
                {
                    counter++;
                }
            }
            return counter;
        }*/

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
        GameObject[] doors = TreeDoorById();

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
        GameObject[] doors = TreeDoorById();

        foreach (GameObject door in doors)
        {
            door.GetComponent<Door>().IsCloseNotPermantly = false;

        }
    }

    public bool VerifyVote()
    {
        int counterYes = 0;
        int counterNo = 0;
        foreach (PlayerDun player in game.list_player)
        {
            if (player.GetVote_CP() == 1)
            {
                counterYes++;
            }
            if (player.GetVote_CP() == -1)
            {
                counterYes--;
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
        //Debug.Log(" X vote : " + x_zone.GetComponent<x_zone_colider>().nbVote + "  VoteDoorMax : " + voteDoorMax);
        if (x_zone.GetComponent<x_zone_colider>().nbVote > voteDoorMax)
        {
            return false;
        }

        return true;
    }

    public bool NobodyHasVoted()
    {
        int voteMax = 0;
        int indexDoorCurrent = -1;
        GameObject[] listDoor = TreeDoorById();
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
        else
        {
            ui_Manager.SetDistanceRoom(distance, roomExpedition);
        }

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
                ui_Manager.tutorial_parent.SetActive(true);
                ui_Manager.tutorial[9].SetActive(true);
                ui_Manager.listTutorialBool[9] = true;
            }

        }
    }

    public void BackToExpedition()
    {
        GetPlayerMine().SetIsInExpedition(false);
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

        ui_Manager.DisplayKeyPlusOne(false);
        ui_Manager.DisplayKeyAndTorch(true);
        SetRoomColor(roomInExpe, false);
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

        if (!OnePlayerHaveToGoToExpedition())
        {
            gameManagerNetwork.SendDisplayMainLevers();
        }
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

        int i = 0;
        foreach (GameObject door in doors)
        {

            if (room.door_isOpen[i] && i == door.GetComponent<Door>().index)
            {
                //door.GetComponent<Animator>().SetBool("open", true);
                door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
                door.GetComponent<Door>().isOpenForAll = true;
            }
            else
            {
                //door.GetComponent<Animator>().SetBool("open", false);
                door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", false);
                door.GetComponent<Door>().isOpenForAll = false;
            }
            i++;
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

    public void CloseDoorOveride()
    {
        GameObject[] doors = TreeDoorById();
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
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] NexListDoors = new GameObject[6];
        int i = 0;
        int j = 0;
        while (i < 6)
        {

            if (doors[j].GetComponent<Door>().index == i)
            {
                NexListDoors[i] = doors[j];
                i++;
            }
            j++;
            if (j == 6)
            {
                j = 0;
            }
        }

        return NexListDoors;
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
        GameObject[] doors = TreeDoorById();

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
            } while (GetPlayer(boss.GetId()).GetComponent<PlayerGO>().isSacrifice && counter < 20);
            if(boss == null)
            {
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
        PlayerDun player = GetPlayerMine();

        if (player.GetIsBoss())
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
        game.currentRoom = game.GetRoomById(indexDoor);
        int indexNeWDoor3 = GetIndexDoorAfterCrosse(indexDoor);
        CloseAllDoor(game.currentRoom, false);


        if (!CheckDoorIsOpenByRoom(game.currentRoom.X, game.currentRoom.Y, indexNeWDoor3))
        {
            ResetVoteVD();
            GameObject newDoor = GetDoorGo(indexNeWDoor3);
            game.currentRoom.door_isOpen[newDoor.GetComponent<Door>().index] = true;
        }

        
        UpdateSpecialsRooms(game.currentRoom);
        ui_Manager.SetDistanceRoom(game.currentRoom.DistancePathFinding, game.currentRoom);
        SetDoorNoneObstacle(game.currentRoom);
        SetDoorObstacle(game.currentRoom);
        GenerateObstacle();
        SetCurrentRoomColor();
        ui_Manager.HideDistanceRoom();

        SetPositionPlayer(player);
        PlayerGO playerGo = player.GetComponent<PlayerGO>();
        //playerGo.isChooseForExpedition = false;
        //gameManagerNetwork.SendIsChooseForExpedition(playerGo.GetComponent<PhotonView>().ViewID);

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

        if (game.dungeon.initialRoom.X == game.currentRoom.X)
        {
            if (game.dungeon.initialRoom.Y == game.currentRoom.Y)
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.DistancePathFinding, null);
            }

        }

        if (PhotonNetwork.IsMasterClient && setting.HELL_ROOM && !alreadyHell)
        {
            InsertHell();
            //
        }

        if (!OnePlayerFindParadise && ((game.key_counter == 0 && !game.currentRoom.IsExit && !game.currentRoom.HasKey) || game.currentRoom.IsHell || isAlreadyLoose))
        {
            Loose();
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

        if (setting.displayTutorial)
        {
            if (!ui_Manager.listTutorialBool[12])
            {
                ui_Manager.tutorial_parent.SetActive(true);
                ui_Manager.tutorial[12].SetActive(true);
                ui_Manager.listTutorialBool[12] = true;
            }

        }

    }


    public void HidePlayerNotInSameRoom()
    {
        PlayerGO myPlayer = GetPlayerMineGO().GetComponent<PlayerGO>();

        foreach (GameObject player in listPlayerTab)
        {
            if (player.GetComponent<PhotonView>().ViewID != GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
            {
                PlayerGO other_player = player.GetComponent<PlayerGO>();
                if (myPlayer.position_X == other_player.position_X && myPlayer.position_Y == other_player.position_Y)
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
        GameObject[] listDoor = TreeDoorById();
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
            if (!player.GetComponent<PlayerGO>().hellIsFind)
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
                roomPivot = game.GetRoomById(i);
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
            Debug.Log(game.currentRoom + " " + game.key_counter);
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

        GameObject[] doors = TreeDoorById();
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

        GameObject[] doors = TreeDoorById();

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
        ui_Manager.DisplayHell(false);
        ui_Manager.HideNbKey();
        ui_Manager.ShowAllDataInMap();
        ui_Manager.ShowImpostor();
        ui_Manager.DisplayKeyAndTorch(false);
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
            if (!player.GetComponent<PlayerGO>().isImpostor)
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
            if (nbPlayerInParadise == players.Length - 1)
            {
                return true;
            }
        }
        else
        {
            if (nbPlayerInParadise == players.Length - 2)
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
            if (nbPlayerInHell == players.Length - 1)
            {
                return true;
            }
        }
        else
        {
            if (nbPlayerInHell == players.Length - 2)
            {
                return true;
            }

        }

        return false;
    }

    public void CloseDoorWhenVote(bool close)
    {
        GameObject[] doors = TreeDoorById();

        foreach (GameObject door in doors)
        {
            if (door.GetComponent<Door>().isOpenForAll)
            {
                door.GetComponent<Door>().IsCloseNotPermantly = close;
            }
        }
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


    public void UpdateSpecialsRooms(Room room)
    {
        if (room.chest && !room.speciallyPowerIsUsed)
        {
            ui_Manager.DisplayChestRoom(true);
            ui_Manager.DisplayMainLevers(false);
            return;
        }
        if(room.fireBall && !room.speciallyPowerIsUsed)
        {
            ui_Manager.DisplayFireBallRoom(true);
            ui_Manager.DisplayMainLevers(false);
            return;
        }
        if(room.isSacrifice && !room.speciallyPowerIsUsed)
        {
            ui_Manager.DisplaySacrificeRoom(true);
            ui_Manager.DisplayMainLevers(false);
            return;
        }

        ui_Manager.ClearSpecialRoom();
        ui_Manager.ResetLevers();
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
        yield return new WaitForSeconds(5);
        voteChestHasProposed = false;
        ui_Manager.DisplayAwardAndPenaltyForImpostor(false);
        ui_Manager.ResetAllPlayerLightAround();
        GameObject chest = CalculVoteChest();
        ui_Manager.ActiveZoneVoteChest(false);
        ChestManagement(chest);
        StartCoroutine(CoroutineHideChest());
        game.currentRoom.speciallyPowerIsUsed = true;
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
                chest.transform.Find("Award").gameObject.SetActive(true);
                chest.transform.Find("Award").transform.GetChild(game.currentRoom.chestList[0].indexAward).gameObject.SetActive(true);
            }
            else
            {
                chest.transform.Find("Penalty").gameObject.SetActive(true);
                chest.transform.Find("Penalty").transform.GetChild(game.currentRoom.chestList[0].indexAward).gameObject.SetActive(true);
            }
            AddBonusAndPenaltyChest(0,game.currentRoom.chestList[0].indexAward, game.currentRoom.chestList[0].isAward);
        }
        else
        {
            if (chest.name == "RedChest")
            {
                if (game.currentRoom.chestList[1].isAward)
                {
                    chest.transform.Find("Award").gameObject.SetActive(true);
                    chest.transform.Find("Award").transform.GetChild(game.currentRoom.chestList[1].indexAward).gameObject.SetActive(true);
                }
                else
                {
                    chest.transform.Find("Penalty").gameObject.SetActive(true);
                    chest.transform.Find("Penalty").transform.GetChild(game.currentRoom.chestList[1].indexAward).gameObject.SetActive(true);

                }
            }
            AddBonusAndPenaltyChest(1,game.currentRoom.chestList[1].indexAward, game.currentRoom.chestList[1].isAward);
        }
    }
    public void AddBonusAndPenaltyChest(int indexChest, int indexAward, bool isAward)
    {
        switch (indexAward)
        {
            case 0:
                if (isAward)
                {
                    game.key_counter++;
                    ui_Manager.LaunchAnimationAddKey();
                }
                else
                {
                    game.key_counter--;
                    ui_Manager.LaunchAnimationBrokenKey();
                }
                return;
            case 1:
                if (isAward)
                {
                    game.nbTorch++;
                }
                else
                {
                    game.nbTorch--;
                }
                ui_Manager.SetTorchNumber();
                return;
            case 2:
                if (isAward)
                {
                    ui_Manager.SetDistanceTextAwardChest(indexChest);
                }
                else
                {
                    ChangePositionParadise();
                }

                return;
        }
    }

    public IEnumerator CoroutineHideChest()
    {
        yield return new WaitForSeconds(4);

        ui_Manager.ResetChestRoom();
        ui_Manager.DisplayChestRoom(false);
    }

    public void ChangePositionParadise()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        game.dungeon.exit.IsExit = false;
        int distanceParadiseCurrentRoom = game.currentRoom.DistancePathFinding;
        List<Room> listRoomWithCorrectDistance = game.dungeon.GetListRoomByDistance(game.currentRoom, distanceParadiseCurrentRoom);
        if (listRoomWithCorrectDistance.Count == 0){
            Debug.Log("return");
            return;
        }
        int randomIndex = Random.Range(0, listRoomWithCorrectDistance.Count);
        Room newParadise = listRoomWithCorrectDistance[randomIndex];
        game.dungeon.exit = newParadise;
        newParadise.IsExit = true;
        gameManagerNetwork.SendNewParadise(newParadise.Index);
        SetCurrentRoomColor();
        game.dungeon.SetPathFindingDistanceAllRoom();
        Debug.Log(randomIndex);
        ui_Manager.SetDistanceTextAwardChest(0);
        ui_Manager.SetDistanceTextAwardChest(1);
    }

    public void ResetFireBallRoom()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach(GameObject turret in turrets)
        {
            //turret.GetComponent<Turret>().canFire = true;
            turret.GetComponent<Turret>().DestroyFireBalls();
        }
    }



}
