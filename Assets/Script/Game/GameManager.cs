using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Luminosity.IO;

public class GameManager : MonoBehaviourPun
{

    public List<PlayerDun> listPlayer;
    public GameObject[] listPlayerTab;
    public List<GameObject> dungeon;
    public Game game;
    public GameObject hexagone;
    public GameObject hexagone_current;
    public GameObject spawn;

    public Setting setting;
    public GameObject map;

    public GameManagerNetwork gameManagerNetwork;

    public bool expeditionHasproposed = false;
    public bool voteDoorHasProposed = false;
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
    public RoomHex roomTeam;
    public RoomHex hell;

    public int nbPlayerFinishLoading = 0;

    public bool launchExpedtion_inputButton = false;
    public bool launchVote_inputButton = false;

    private int nbPlayerInParadise = 0;
    private int nbPlayerInHell = 0;


    public AudioSource soundOpenDoor;
    public AudioSource soundTakeDoor;
    

    public Head_paradise headParadise;

    public bool alreadyPass = false;

    public int distanceInitial =  0;
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

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
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


        if (PhotonNetwork.IsMasterClient)
        {
            int randomWidth = Random.Range(15, 30);
            int randomHeight = Random.Range(15, 30);
            gameManagerNetwork.SendWidthHeightMap(randomWidth, randomHeight);
        }
  
        
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
        if (PhotonNetwork.IsMasterClient)
        {

            gameManagerNetwork.SendSetting(setting.NUMBER_EXPEDTION_MAX, setting.DISPLAY_MINI_MAP,
                setting.DISPLAY_OBSTACLE_MAP, setting.DISPLAY_KEY_MAP, setting.RANDOM_ROOM_ADDKEYS,
                setting.LIMITED_TORCH, setting.TORCH_ADDITIONAL);
            game.CreationMap();
            game.ChangeBoss();
            game.AssignRole();
            SendRole();
            SendMap();
            ui_Manager.SetDistanceRoom(game.currentRoom.GetDistance_pathfinding(), game.currentRoom);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        // vote doorr
        // in if( timer.timerFinish && MajorityHasVotedDoor())
        if ( timer.timerFinish && voteDoorHasProposed)
        {
            ui_Manager.DesactivateLightAroundPlayers();
            
            GameObject door = GetDoorWithMajority();
           
            if (SamePositionAtBoss() && VerifyVoteVD(door.GetComponent<Door>().nbVote))
            {
                if (door.GetComponent<Door>().nbVote == 0 || game.currentRoom.GetIsVirus())
                {
                    if(PhotonNetwork.IsMasterClient)
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
            //ui_Manager.display

            if (game.dungeon.initialRoom.GetPos_X() == game.currentRoom.GetPos_X())
            {
                if (game.dungeon.initialRoom.GetPos_Y() == game.currentRoom.GetPos_Y())
                {
                    ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.GetDistance_pathfinding(), null);
                }
              
            }
            
            CloseDoorWhenVote(false);
            ui_Manager.zones_X.GetComponent<x_zone_colider>().nbVote = 0;
        }

        // vote expedition
        if (AllPlayerHasVoted() && !endGame  && !alreaydyExpeditionHadPropose && !isLoading)
        {
            timer.ResetTimer();
            ui_Manager.HideZoneVote();
            if (VerifyVote())
            {
                if(setting.LIMITED_TORCH)
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
            }
            ResetVoteCP();
            ui_Manager.DisplayKeyAndTorch(true);

            if (game.dungeon.initialRoom.GetPos_X() == game.currentRoom.GetPos_X())
            {
                if (game.dungeon.initialRoom.GetPos_Y() == game.currentRoom.GetPos_Y())
                {
                    ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.GetDistance_pathfinding(), null);
                }

            }
            CloseDoorWhenVote(false);
            alreadyPass = false;
            SetAlreadyHideForAllPlayers();
            canResetTimerBoss = true;
        }


        if (timer.timerFinish && isLoading)
        {
            timer.ResetTimer();
            SpawnPlayer();
            isLoading = false;
            ResetVoteCP();
            gameManagerNetwork.SendLoadingFinish();
        }


        if(nbPlayerFinishLoading == listPlayerTab.Length && !alreadyPasseLoading)
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

    public void GenerateHexagone(float initial_X, float initial_Y)
    {

        foreach (RoomHex room in game.dungeon.rooms)
        {

            GameObject newHexagone = GameObject.Instantiate(hexagone);
            float positionTransformationX;
            float positionTransformationY;
            if (room.GetPos_Y()%2 != 0)
            {
                 positionTransformationX = (initial_X + 0.82f) + (1.6f * room.GetPos_X());
                 positionTransformationY = (initial_Y)  + (-1.41f * room.GetPos_Y());
            }
            else
            {
                
                 positionTransformationX = initial_X + (1.6f * room.GetPos_X());
                 positionTransformationY = initial_Y + (-1.41f * room.GetPos_Y());
               
            }
            newHexagone.transform.position = new Vector3(positionTransformationX, positionTransformationY,-4);
            newHexagone.GetComponent<Hexagone>().index = room.GetIndex();
            newHexagone.GetComponent<Hexagone>().index_text.text = room.GetIndex().ToString();
            newHexagone.GetComponent<Hexagone>().isObstacle = room.GetIsObstacle();
            newHexagone.GetComponent<Hexagone>().isExit = room.GetIsExit();
            newHexagone.GetComponent<Hexagone>().distance_exit = room.GetDistance_exit();
            newHexagone.GetComponent<Hexagone>().isInitialeRoom = room.GetIsInitialeRoom();
            if (setting.DISPLAY_OBSTACLE_MAP || GetPlayerMine().GetIsImpostor())
            {
                newHexagone.GetComponent<Hexagone>().distance_pathFinding = room.distance_pathFinding_initialRoom;
            }
            else
            {
                newHexagone.GetComponent<Hexagone>().distance_pathFinding = room.distance_reel_initialRoom;
            }
               
            newHexagone.GetComponent<Hexagone>().isFoggy = room.isFoggy;
            newHexagone.GetComponent<Hexagone>().isVirus = room.GetIsVirus();
            newHexagone.GetComponent<Hexagone>().hasKey = room.hasKey;
            newHexagone.GetComponent<Hexagone>().chest = room.chest;
            newHexagone.GetComponent<Hexagone>().pos_X = room.GetPos_X();
            newHexagone.GetComponent<Hexagone>().pos_Y = room.GetPos_Y();

            newHexagone.GetComponent<Hexagone>().listNeighbour = room.listNeighbour;
            newHexagone.transform.parent = map.transform;
            dungeon.Add(newHexagone);
        }

    }

    public void GenerateObstacle()
    {
        foreach (GameObject room in dungeon)
        {
            SetHexagoneColor(room);
        }
    }

    public void SetHexagoneColor(GameObject room)
    {
        

        room.GetComponent<Hexagone>().distanceText.text = room.GetComponent<Hexagone>().distance_pathFinding.ToString();
        if (room.GetComponent<Hexagone>().isObstacle)
        {
            if (setting.DISPLAY_OBSTACLE_MAP || GetPlayerMine().GetIsImpostor())
            {
                room.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                room.GetComponent<Hexagone>().distanceText.text = "";
                room.GetComponent<Hexagone>().index_text.text = "";
                //Destroy(room);
            }


        }
        if (room.GetComponent<Hexagone>().isInitialeRoom)
        {
            room.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            if(!room.GetComponent<Hexagone>().isExit)
                room.GetComponent<Hexagone>().isTraversed = true;
            hexagone_current = room;
        }
        if (room.GetComponent<Hexagone>().hasKey)
        {
            if (setting.DISPLAY_KEY_MAP || GetPlayerMine().GetIsImpostor())
            {
                room.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
/*                room.GetComponent<Hexagone>().distanceText.text = "";
                room.GetComponent<Hexagone>().index_text.text = "";*/
            }
        }
        if (room.GetComponent<Hexagone>().isFoggy && GetPlayerMine().GetIsImpostor())
        {
            room.GetComponent<SpriteRenderer>().color = new Color(87/255f, 89/255f, 96/255f);
        }
        if (room.GetComponent<Hexagone>().isVirus && GetPlayerMine().GetIsImpostor())
        {
            room.GetComponent<SpriteRenderer>().color = new Color(66 / 255f, 0 / 255f, 117 / 255f);
        }
        if (room.GetComponent<Hexagone>().isExit && GetPlayerMine().GetIsImpostor())
        {
            room.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            room.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            if (room.GetComponent<Hexagone>().hasKey)
            {
                room.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                room.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            }
        }

        if (room.GetComponent<Hexagone>().chest)
        {
            room.GetComponent<SpriteRenderer>().color = new Color(44 / 255f, 70 / 255f, 30 / 255f);
        }


    }

    public void SpawnPlayer()
    {
        GameObject myPlayer = GetPlayerMineGO();

        int indexPlayer_viewID = myPlayer.GetComponent<PhotonView>().ViewID;
        string indexPlayer_string = indexPlayer_viewID.ToString();
        char indexPlayer_char = indexPlayer_string[0];
        int indexPlayer = int.Parse(indexPlayer_char.ToString());


        myPlayer.transform.position = spawn.transform.GetChild(indexPlayer%listPlayer.Count).transform.position;
    }

    public void SetCurrentRoomColor()
    {

        foreach (GameObject room in dungeon)
        {
            if( room.GetComponent<Hexagone>().index  == game.currentRoom.GetIndex())
            {
                room.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
                if(!room.GetComponent<Hexagone>().isExit)
                    room.GetComponent<Hexagone>().isTraversed = true;
               
            }
            else
            {
                if (room.GetComponent<Hexagone>().isTraversed)
                {
                    if (!room.GetComponent<Hexagone>().isExit)
                        room.GetComponent<SpriteRenderer>().color = new Color( (float) (16f/255f), (float) 78f /255f, (float) 29f /255f,1);
                }
            }
            if(hell && GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                if(room.GetComponent<Hexagone>().pos_X == hell.GetPos_X() && room.GetComponent<Hexagone>().pos_Y == hell.GetPos_Y())
                {
                    room.GetComponent<SpriteRenderer>().color = new Color((float) (255 / 255f), (float) 0f / 255f, (float) 0f / 255f, 1);
                    room.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);

                    if (room.GetComponent<Hexagone>().hasKey)
                    {
                        room.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                        room.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
                    }
                }
            } 
        }
    }

    public void SetRoomColor(RoomHex room, bool inExpedition)
    {
        foreach(GameObject roomInList in dungeon)
        {
            if(roomInList.GetComponent<Hexagone>().index == room.GetIndex())
            {
                if (inExpedition)
                {
                    roomInList.GetComponent<SpriteRenderer>().color = new Color((float)(241f / 255f), (float)130f / 255f, (float)70f / 255f, 1);
                }
                else
                {
                    roomInList.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                }
                
            }
        }
    }

    public void SetDoorObstacle(RoomHex room)
    {
        List<int> listDoorObstacleIndex = game.GetDoorObstacle(room);

        foreach (int indexRoomObstacle in listDoorObstacleIndex)
        {
            ui_Manager.DisplayObstacleInDoor(indexRoomObstacle , true);
        }
    }


    public void SetDoorNoneObstacle(RoomHex room)
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
        foreach(GameObject player in listPlayerTab) {

            SetPositionPlayer(player);
        }
    }

    public void SetPositionPlayer(GameObject player)
    {
        player.GetComponent<PlayerGO>().position_X = game.currentRoom.GetPos_X();
        player.GetComponent<PlayerGO>().position_Y = game.currentRoom.GetPos_Y();
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
        for(int i =0; i < 6; i++)
        {
            door_idPlayer.Remove(i);
        }
        
    }

    public void OpenDoor(GameObject door, bool isExpedition)
    {
        int indexDoor = door.GetComponent<Door>().index;
        roomTeam = game.GetRoomById(indexDoor);

        //door.GetComponent<Animator>().SetBool("open", true);
        soundOpenDoor.Play();


        door.transform.GetChild(6).GetComponent<Animator>().SetBool("open", true);
        
        if (!isExpedition)
        {
            door.GetComponent<Door>().isOpenForAll = true;
            game.currentRoom.door_isOpen[door.GetComponent<Door>().index] = true;

            //game.currentRoom.listNeighbour[door.GetComponent<Door>().index].isTraversed = true;
            roomTeam.isTraversed = true;
            if(roomTeam.hasKey && PhotonNetwork.IsMasterClient)
                gameManagerNetwork.AddKey(roomTeam.GetIndex());
            if(roomTeam.GetIsExit() || roomTeam.isHell)
            {
                gameManagerNetwork.SendParadiseOrHellFind(roomTeam.GetIsExit(), roomTeam.isHell);

            }
            
        }

        gameManagerNetwork.SendOpenDoor(indexDoor, game.currentRoom.GetPos_X(), game.currentRoom.GetPos_Y(), isExpedition, roomTeam.GetIndex());



    }

    public void OpenDoorsToExpedition()
    {
        foreach(Expedition expe in game.current_expedition)
        {

            GameObject[] doors = TreeDoorById();

            foreach (GameObject door in doors)
            {
                if(door.GetComponent<Door>().index == expe.indexNeigbour)
                {
                    OpenDoor(door, true) ;
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


    public void ProposeExpedition(Dictionary<int, int> door_idPlayer )
    {
        
        foreach(KeyValuePair<int, int> dic in door_idPlayer)
        {
            SendExpedition(dic.Value, dic.Key); 
        }
        gameManagerNetwork.LaunchTimerExpedition();
    }
    
    public bool VerificationExpedition(Dictionary<int, int> door_idPlayer)
    {
        if(door_idPlayer.Count == 0)
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
        foreach(PlayerDun player in listPlayer)
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
        foreach(RoomHex room in game.dungeon.rooms)
        {
            gameManagerNetwork.SendMap(room.GetIndex(), room.GetIsExit(), room.GetIsObstacle(), 
                room.GetIsInitialeRoom(),room.GetDistance_exit(), room.GetDistance_pathfinding(),
                room.distance_pathFinding_initialRoom, counter == game.dungeon.rooms.Count , room.isFoggy, 
                room.GetIsVirus(), room.hasKey, room.chest);

            counter++;
        }
    }

    public void DisplayPlayerLog()
    {
        foreach(PlayerDun player in game.list_player)
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
        if(listGO != null)
            game.list_player = listGO;
    }

    public void UpdateListPlayerGO()
    {
        listPlayerTab = GameObject.FindGameObjectsWithTag("Player");
    }


    public bool IsPlayerMine(int id)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            if(player.GetComponent<PhotonView>().ViewID == id && player.GetComponent<PhotonView>().IsMine)
            {
                return true;
            }
        }
        return false;
    }


    public void VoteCP(int vote)
    {
        PlayerDun player = GetPlayerMine();
        gameManagerNetwork.SendVoteCP(player.GetId(),vote); 
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
        foreach(PlayerDun player in game.list_player)
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
        foreach(PlayerDun player in listPlayer)
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

        foreach(GameObject door in doors)
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
            if(player.GetVote_CP() == -1)
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
        if(voteMax == 0)
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
       
        RoomHex roomExpedition = GetDoorExpedition(GetPlayerMine().GetId());
        GetPlayerMineGO().GetComponent<PlayerGO>().position_X = roomExpedition.GetPos_X();
        GetPlayerMineGO().GetComponent<PlayerGO>().position_Y = roomExpedition.GetPos_Y();
        HidePlayerNotInSameRoom();
        ui_Manager.DisplayAllGost(false);


        ui_Manager.DisplayKeyAndTorch(false);
        if (roomExpedition.hasKey && !roomExpedition.isFoggy)
        {
            ui_Manager.DisplayKeyPlusOne(true);
        }

        if (roomExpedition.isFoggy)
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

        gameManagerNetwork.SendPositionPlayer(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, roomExpedition.GetPos_X(), roomExpedition.GetPos_Y());
        ui_Manager.DisplayBlackScreen(false, true);
        if(roomExpedition.GetIsExit())
            ui_Manager.DisplayParadise(true , indexDoorExit);
        if (roomExpedition.isHell)
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

        int distance = game.currentRoom.GetDistance_pathfinding();
        ui_Manager.SetDistanceRoom(distance, game.currentRoom);
        SetDoorNoneObstacle(game.currentRoom);
        SetDoorObstacle(game.currentRoom);
        RoomHex roomInExpe = GetDoorExpedition(GetPlayerMine().GetId());
        GetPlayerMineGO().GetComponent<PlayerGO>().position_X = game.currentRoom.GetPos_X();
        GetPlayerMineGO().GetComponent<PlayerGO>().position_Y = game.currentRoom.GetPos_Y();
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

        StartCoroutine(PauseWhenPlayerTakeDoor(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, game.currentRoom.GetPos_X(), game.currentRoom.GetPos_Y(), 0.2f));
        //gameManagerNetwork.SendPositionPlayer(GetPlayerMineGO().GetComponent<PhotonView>().ViewID, game.currentRoom.GetPos_X(), game.currentRoom.GetPos_Y());
        ui_Manager.DisplayBlackScreen(false, true);
        OpenDoorMustBeOpen();
        ui_Manager.HideParadise();
        ui_Manager.HideHell();

        GetPlayerMineGO().GetComponent<PlayerGO>().isGoInExpeditionOneTime = true;
    }

    public void ChangePositionPlayerWhenTakeDoor(GameObject player, GameObject doorEnter)
    {
        int indexDoorExit = GetIndexDoorAfterCrosse(doorEnter.GetComponent<Door>().index);
        GameObject door = GetDoorGo(indexDoorExit);
        player.transform.position = door.transform.GetChild(3).transform.position;
    }

    public void CloseAllDoor(RoomHex room , bool isInExepedtion , GameObject doorReverse =null)
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
        while( i < 6)
        {

            if(doors[j].GetComponent<Door>().index == i)
            {
                NexListDoors[i] = doors[j];
                i++;
            }
            j++;
            if( j == 6)
            {
                j = 0;
            }
        }

        return NexListDoors;
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
        if(indexDoor == 0)
        {
            return 3;
        }
        else if( indexDoor == 1)
        {
            return 4;
        }
        else if(indexDoor == 2)
        {
            return 5;
        }else if( indexDoor == 3)
        {
            return 0;
        } else if (indexDoor == 4)
        {
            return 1;
        }else if (indexDoor == 5)
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
            if(door.GetComponent<Door>().index == index)
            {
                return door;
            }
        }
        return null;
    }

    public RoomHex GetDoorExpedition(int idPlayer)
    {
        foreach (Expedition expe in game.current_expedition)
        {
            if(expe.player.GetId() == idPlayer)
            {
                return expe.room;
            }
        }
        return null;
    }
    public bool MineIsInExpedition()
    {
        bool isInExpedition = false;
        foreach(Expedition expe in game.current_expedition)
        {
            if (expe.player.GetId() == GetPlayerMine().GetId())
            {
                isInExpedition =  true;
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
            for(int i =0; i< player.Length; i++)
            {
                if(player[i].GetComponent<PhotonView>().ViewID == expe.player.GetId())
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
        foreach(Expedition expe in listExpe)
        {
            
            if(expe.player.GetId() == GetPlayerMine().GetId()) {
                return expe;
            }
        }

        return null;
    }



    public int GetDistanceInExpedition(List<Expedition> listExepdition)
    {
        foreach(Expedition expe in listExepdition)
        {
            if(expe.player.GetId() == GetPlayerMine().GetId())
            {
                return expe.room.GetDistance_pathfinding();
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
            PlayerDun boss = game.ChangeBoss();
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


        if (!CheckDoorIsOpenByRoom(game.currentRoom.GetPos_X(), game.currentRoom.GetPos_Y(), indexNeWDoor3))
        {
            ResetVoteVD();
            GameObject newDoor = GetDoorGo(indexNeWDoor3);
            game.currentRoom.door_isOpen[newDoor.GetComponent<Door>().index] = true;
        }
       
        ui_Manager.SetDistanceRoom(game.currentRoom.GetDistance_pathfinding(), game.currentRoom);
        SetDoorNoneObstacle(game.currentRoom);
        SetDoorObstacle(game.currentRoom);
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

        if(voteDoorHasProposed)
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

        if (game.currentRoom.hasKey && game.currentRoom.availableKeyAnimation)
        {
            
            game.currentRoom.availableKeyAnimation = false;
            ui_Manager.LaunchAnimationAddKey();
        }

        if (game.dungeon.initialRoom.GetPos_X() == game.currentRoom.GetPos_X())
        {
            if (game.dungeon.initialRoom.GetPos_Y() == game.currentRoom.GetPos_Y())
            {
                ui_Manager.SetDistanceRoom(game.dungeon.initialRoom.GetDistance_pathfinding(), null);
            }

        }

        if (PhotonNetwork.IsMasterClient && setting.HELL_ROOM && !alreadyHell)
        {
            InsertHell();
            //
        }

        if (!OnePlayerFindParadise && ( (game.key_counter == 0 && !game.currentRoom.GetIsExit() && !game.currentRoom.hasKey) || game.currentRoom.isHell || isAlreadyLoose))
        {
            Loose();
        }

        if (game.currentRoom.GetIsExit())
        {
            ResetDoor();
            Win();
            
        }
        UpdateSpecialsRooms(game.currentRoom);

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

        foreach(GameObject player in listPlayerTab)
        {
            if(player.GetComponent<PhotonView>().ViewID != GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
            {
                PlayerGO other_player = player.GetComponent<PlayerGO>();
                if (myPlayer.position_X == other_player.position_X &&  myPlayer.position_Y == other_player.position_Y)
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
        RoomHex room = game.dungeon.GetRoomByPosition(x, y);
        return room.door_isOpen[indexDoor];
    }

    public bool RoomIsCurrent(int x, int y)
    {
        RoomHex room = game.dungeon.GetRoomByPosition(x, y);
        if(room.GetIndex() == roomTeam.GetIndex())
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
            if(door.GetComponent<Door>().nbVote >= voteMax)
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
                if(player.GetComponent<PhotonView>().ViewID != playerB.GetComponent<PhotonView>().ViewID)
                {
                    if((player.GetComponent<PlayerGO>().position_X != playerB.GetComponent<PlayerGO>().position_X) || (player.GetComponent<PlayerGO>().position_Y != playerB.GetComponent<PlayerGO>().position_Y))
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
            RoomHex roomPivot = null;
            int i = 0;
            do
            {
                random = Random.Range(0, 6);
                roomPivot = game.GetRoomById(i);
                if (!roomPivot.GetIsObstacle() && !roomPivot.isTraversed && !roomPivot.GetIsExit() && roomPivot.distance_pathFinding_initialRoom == game.dungeon.exit.distance_pathFinding_initialRoom)
                {
                    isCorrectRoom = true;
                }
                i++;
            } while (!isCorrectRoom && i < 6);
            if(isCorrectRoom)
                 gameManagerNetwork.SendHell(roomPivot.GetIndex());

            alreadyHell = true;
        }
    }

    public bool NbKeySufficient()
    {
        foreach (RoomHex room in game.dungeon.rooms)
        {
            if (room.isTraversed)
            {
                if( game.dungeon.GetPathFindingDistance(room, game.dungeon.exit) <=  ( game.key_counter ) )
                {
                    return true;
                }
            }
        }
        if(game.dungeon.GetPathFindingDistance(game.currentRoom, game.dungeon.exit) <= (game.key_counter))
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
            } while ((randomInt == door.GetComponent<Door>().index) ||  indexAlreadyInsert.Contains(randomInt));

            gameManagerNetwork.SendMixDoor(door.GetComponent<Door>().index, randomInt);
            door.GetComponent<Door>().index = randomInt;
            indexAlreadyInsert.Add(randomInt);
        }


    }

    public List<GameObject> GetDoorAvailable()
    {
        List<GameObject> listReturn = new List<GameObject>();

        GameObject[] doors = TreeDoorById();

        foreach(GameObject door in doors)
        {
            if(  !door.GetComponent<Door>().isOpenForAll && !door.GetComponent<Door>().barricade)
            {
                listReturn.Add(door);
            }
        }

        return listReturn;

    }
    public IEnumerator PauseWhenPlayerTakeDoor(int idPlayer, int x, int y, float secondePause)
    {
        yield return new WaitForSeconds(secondePause);
        gameManagerNetwork.SendPositionPlayer(idPlayer,x,y);
    }

    public void Win()
    {
        paradiseIsFind = true;
        gameManagerNetwork.SendParadiseIsFind(GetPlayerMine().GetId());
        CloseDoorOveride();
        ui_Manager.DisplayParadise(false , -1);
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

        foreach(GameObject player in players)
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

    public bool  AllPlayerGoneToParadise()
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

    public void CloseDoorWhenVote( bool close)
    {
        GameObject[] doors = TreeDoorById();

        foreach(GameObject door in doors)
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

        foreach(GameObject player in players)
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
        if(GetBoss().GetComponent<PlayerGO>().position_X == GetPlayerMineGO().GetComponent<PlayerGO>().position_X) 
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
        if (game.dungeon.initialRoom.GetPos_X() == GetPlayerMineGO().GetComponent<PlayerGO>().position_X)
        {
            if (game.dungeon.initialRoom.GetPos_Y() == GetPlayerMineGO().GetComponent<PlayerGO>().position_Y)
            {
                return true;
            }
        }
        return false;

    }

    public void DesactivateColliderDoorToExplorater(int indexDoor, int indexPlayer)
    {
        if( indexPlayer == GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
        {
            GetDoorGo(indexDoor).transform.GetChild(5).GetComponent<CapsuleCollider2D>().enabled = false;
        }

    }
    
    public List<GameObject> GetAllImpostor()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listImpostor = new List<GameObject>();
        foreach(GameObject player in players)
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
        foreach(GameObject player in players)
        {
            listNamePlayer.Add(player.GetComponent<PlayerGO>().playerName);
        }
    }

    public void UpdateSpecialsRooms(RoomHex room)
    {
        if (room.chest)
        {
            UpdateChestRoom();
            return;
        }
        ClearSpecialRoom();
    }

    public void UpdateChestRoom()
    {
        ui_Manager.DisplayChestRoom();
    }

    public void ClearSpecialRoom() {
        ui_Manager.ClearSpecialRoom();
    }

    public void  GetImpostorName()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if(player.GetComponent<PlayerGO>().isImpostor)
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
        gameManagerNetwork.SendAskReset(playerMine.GetComponent<PhotonView>().ViewID, playerMine.position_X , playerMine.position_Y);
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


}
