using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{

    public GameManager gameManager;
    public GameObject map;
    public GameObject MainRoomGraphic;

    public GameObject text_distance_room;
    public GameObject letterOne;
    public GameObject letterTwo;

    public Text nbKeyText;
    public GameObject zonesVote;
    public GameObject zones_X;
    public GameObject zones_X_double;

    public GameObject blackWallPaper;
    public GameObject blueWallPaper;
    public GameObject whiteWallPaper;
    public GameObject LoadPage;
    public GameObject roleInformation;

    private float durationTransitionBlackScreen = 0.7f;
    private float transition_timer = 0;
    private bool verifTransi = false;

    public GameObject zoneX_startAnmation;

    public GameObject hell;
    public GameObject paradise;

    public GameObject echap_menu;

    public GameObject DoorRight_paradise;
    public GameObject DoorLeft_paradise;

    public GameObject wallEight;
    public GameObject nbKey;
    public GameObject KeyPlus1;
    public GameObject key_image;
    public GameObject Key_broken;
    public GameObject addKey;
    public GameObject torch_broken;
    public Text torch_number;

    private bool key_broken_animation;
    private bool addKeyAnimation;

    public bool torch_broken_animation;

    public GameObject addTorch;
    public bool addTorchAnimation;

    public GameObject x_zone_red;
    

    public GameObject identification_expedition;
    public GameObject identification_voteDoor;

    public GameObject resumePanel;
    public GameObject map_resumePanel;


    public Text textChatMessage;

    public AudioSource soundChrono;
    public AudioSource soundChrono2;
    public AudioSource soundChrono_8sec;
    public AudioSource soundChrono_10sec;
    public AudioSource soundDemonicLaugh;
    public AudioSource soundAmbianceHell;
    public AudioSource soundAmbianceParadise;

    public GameObject panelTutoriel;

    public GameObject setting_button_echapMenu;

    public GameObject OutsideMap;

    public GameObject oneExplorationTutoPanel;
    public Button EchapButton_unblockPlayer;

    public GameObject tutorial_parent;
    public List<GameObject> tutorial;
    public List<bool> listTutorialBool = new List<bool>();

    public GameObject chatPanelInputParent;
    public GameObject mobileCanvas;

    public GameObject autelTutorial;
    public GameObject autelTutorialSpeciallyRoom;

    public List<GameObject> listButtonPowerImpostor;

    public GameObject waitingPage_PowerImpostor;

    public bool timerMixExploration = true;

    public GameObject panelInWaiting;

    public GameObject canvasInGame;
    public GameObject ReconnexionPanel;

    public GameObject panelChooseRoom;
    public GameObject panelChooseRoomTrap;

    public GameObject panelInformationObjectWon;
    public GameObject panelInformationPowerWon;

    public GameObject panelBossInformation;

    public GameObject mapLostSoul;
    public GameObject buttonDisplayMapImpostor;

    public GameObject panelChooseAwardTeamTrial;

    public AudioSource impactSword;
    public AudioSource musicFight;

    public AudioSource BasesMusic;
    public AudioSource BasesMusic2;
    public AudioSource BasesMusic3;
    public AudioSource CaveMusicAmbiance;

    public AudioSource currentMusic;

    public float timerMusic;
    public float timerMusic2;
    public float timerMusic3;
    public float timerCave;

    public AudioSource dashSound;
    public AudioSource monsterExplosion;

    public AudioSource damoclesExplosion;

    public AudioSource axeLaunch;
    public AudioSource axeEnd;

    public AudioSource heartBroken;

    public AudioSource doorTorched;
    public AudioSource doorTorched_black;
    
    public AudioSource traped_door;
    public AudioSource magical_key;

    public AudioSource invisibility;

    public AudioSource fireball;

    public AudioSource DeathSound;

    public int currentMusic_index = -1;
    public bool launchIncreaseVolumLittleToLittle = false;

    public GameObject map_interaction;
    public GameObject changeBoss_interaction;

    public GameObject listHexa;

    public GameObject supportTorch;

    public GameObject chests;

    public GameObject virusRoomFloor;

    public GameObject hexagoneImpostorInformation;
    public GameObject hexagoneNoneImpostorInformation;

    public GameObject doorsParent;


    // Start is called before the first frame update
    void Start()
    {

#if !UNITY_IOS && !UNITY_ANDROID
        mobileCanvas.transform.parent.gameObject.SetActive(false);
#else

setting_button_echapMenu.SetActive(false);

#endif

        for(int i =0;  i< tutorial.Count; i++)
        {
            listTutorialBool.Add(false);
        }

        currentMusic_index = -1;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.voteDoorHasProposed && gameManager.timer.timerLaunch && gameManager.game.currentRoom.IsVirus)
        {
            MixLetterDoorUI();
        }
  
       
        if (blackWallPaper.activeSelf)
        {
            TransitionToBlack(transition_timer, blackWallPaper);
            if (transition_timer < 1 )
            {
                transition_timer += (Time.deltaTime / durationTransitionBlackScreen);
            }
        }

        if (whiteWallPaper.activeSelf)
        {
            TransitionToBlack(transition_timer, whiteWallPaper);
            if (transition_timer < 1)
            {
                transition_timer += (Time.deltaTime / durationTransitionBlackScreen);
            }
        }
        if (torch_broken_animation)
        {
            AnimationBrokenTorch();
        }
        if (key_broken_animation)
        {
            AnimationBrokenKey();
        }
        if (addKeyAnimation)
        {
            AniamtionAddKey();
        }
        if (addTorchAnimation)
        {
            AnimationAddTorch();
        }

        EchapButton_unblockPlayer.interactable = !gameManager.timer.timerLaunch;

        if (launchIncreaseVolumLittleToLittle)
        {
            currentMusic.volume += (0.01f * Time.deltaTime);
            if ((currentMusic.volume * 5) >= gameManager.setting.volume_music)
                launchIncreaseVolumLittleToLittle = false;
        }

       //ebug.Log(BasesMusic.time);

        if(currentMusic_index != -1)
        {
            if (!BasesMusic.isPlaying && currentMusic_index == 0)
            {
                StartCoroutine(gameManager.PauseMusic());
                currentMusic_index = -1;
            }
            if (!BasesMusic2.isPlaying && currentMusic_index == 1)
            {
                StartCoroutine(gameManager.PauseMusic());
                currentMusic_index = -1;
            }
            if (!BasesMusic3.isPlaying && currentMusic_index == 2)
            {
                StartCoroutine(gameManager.PauseMusic());
                currentMusic_index = -1;
            }
        }

      

    }


    public void DisplayMap()
    {
        if (mapLostSoul.activeSelf)
            return;

        if (!echap_menu.activeSelf)
        {
            map.SetActive(!map.activeSelf);
            blueWallPaper.SetActive(!blueWallPaper.activeSelf);

        }
        if (gameManager.gameIsReady)
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = !map.activeSelf;

        if (map.activeSelf)
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasMap)
                map = gameManager.CenterMapByPositionPlayer();
        }
         
        if (!blueWallPaper.activeSelf)
        {
            Camera.main.orthographicSize = 5.1f;
        }

        if (!map.activeSelf)
        {
            DesactivateInformationSpecallyRoomAllHexagone();
            RemoveLightHexagone();
        }
            


        //gameManager.SetCurrentRoomColor();
    }

    public void HideLostSoulMap()
    {
        mapLostSoul.SetActive(false);
        map.SetActive(false);
        blueWallPaper.SetActive(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
        RemoveLightHexagone();
    }

    public void RemoveLightHexagone()
    {
        foreach (Hexagone hexagone in gameManager.dungeon)
        {
            if(hexagone.GetComponent<Hexagone>().isLighted)
                hexagone.SetLight2(false);
        }
        if (mapLostSoul.transform.Find("LostSoulMap"))
        {
            for (int i = 0; i < mapLostSoul.transform.Find("LostSoulMap").childCount; i++)
            {
                if (mapLostSoul.transform.Find("LostSoulMap").GetChild(i).GetComponent<HexagoneLostSoul>())
                {
                    if (mapLostSoul.transform.Find("LostSoulMap").GetChild(i).GetComponent<HexagoneLostSoul>().isLighted  )
                    {
                        mapLostSoul.transform.Find("LostSoulMap").GetChild(i).GetComponent<HexagoneLostSoul>().SetLight2(false);
                        gameManager.gameManagerNetwork.SendLightHexagoneLostSoul(mapLostSoul.transform.Find("LostSoulMap").GetChild(i).GetComponent<HexagoneLostSoul>().room.Index, mapLostSoul.transform.Find("LostSoulMap").GetChild(i).GetComponent<HexagoneLostSoul>().isLighted); 
                    }
                       
                }
                   
            }
        }
        else
        {
            for (int i = 0; i < mapLostSoul.transform.childCount; i++)
            {
                if (mapLostSoul.transform.GetChild(i).GetComponent<HexagoneLostSoul>())
                {
                     if (mapLostSoul.transform.GetChild(i).GetComponent<HexagoneLostSoul>().isLighted)
                    {
                        mapLostSoul.transform.GetChild(i).GetComponent<HexagoneLostSoul>().SetLight2(false);
                        gameManager.gameManagerNetwork.SendLightHexagoneLostSoul(mapLostSoul.transform.GetChild(i).GetComponent<HexagoneLostSoul>().room.Index, mapLostSoul.transform.GetChild(i).GetComponent<HexagoneLostSoul>().isLighted);
                    }
                      
                }
                    
            }
        }
        
    }

    public void DisplayMapLostSoul(bool isBackButton)
    {
        //RemoveLightHexagone();
        if (!mapLostSoul.activeSelf && isBackButton)
        {
            return;
        }
        if (!echap_menu.activeSelf)
        {
            mapLostSoul.SetActive(!mapLostSoul.activeSelf);
            blueWallPaper.SetActive(!blueWallPaper.activeSelf);
        }

        if (gameManager.gameIsReady)
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = !mapLostSoul.activeSelf;

        if (mapLostSoul.activeSelf)
        {
            mapLostSoul = gameManager.CenterMapLostSoulByPostionPlayer();
        }
        else
        {
            
            RemoveLightHexagone();

        }
        if (!blueWallPaper.activeSelf)
        {
            Camera.main.orthographicSize = 5.1f;
        }
    }

    public void DisplayTimerInMap(bool active)
    {
        blueWallPaper.transform.Find("Canvas").Find("Back").GetComponent<Button>().interactable = !active;
    }

    public void DisplayPowerButton(int indexPower, bool display)
    {
        listButtonPowerImpostor[indexPower].SetActive(display);
    }

    public void HidePlayer(bool hide, bool hidePlayerInExpedition)
    {
        GameObject[] players = gameManager.listPlayerTab;
        foreach(GameObject player in players)
        {
            if (!gameManager.IsPlayerMine(player.GetComponent<PhotonView>().ViewID))
            {
                if(hidePlayerInExpedition && gameManager.game.GetPlayerById(player.GetComponent<PhotonView>().ViewID).GetIsInExpedition())
                {
                    player.transform.GetChild(0).gameObject.SetActive(hide);
                    player.transform.GetChild(1).gameObject.SetActive(hide);
                }
                else
                {
                    player.transform.GetChild(0).gameObject.SetActive(!hide);
                    player.transform.GetChild(1).gameObject.SetActive(!hide);
                }
                
            }
        }
    }

    public void HideMyPlayer(bool hide)
    {
        GameObject player = gameManager.GetPlayerMineGO();

        player.transform.GetChild(0).gameObject.SetActive(!hide);
        player.transform.GetChild(1).gameObject.SetActive(!hide);

    }

    public void SetDistanceRoom(int distance, Room room)
    {
        text_distance_room.GetComponent<Text>().text = distance.ToString();

    }
    public void SetDistanceRoomFalse(int distance)
    {
        text_distance_room.GetComponent<Text>().text = distance.ToString();
    }

    public void HideDistanceRoom()
    {
        text_distance_room.GetComponent<Text>().text = "";
        letterOne.GetComponent<Text>().text = "";
        letterTwo.GetComponent<Text>().text = "";
    }

    public void DisplayInterrogationPoint()
    {
        text_distance_room.GetComponent<Text>().text = "?";
    }
    public void SetLetterExploration(string trueLetter, string falseLetter)
    {
        int randomInt = Random.Range(0, 2);
        letterOne.SetActive(true);
        letterTwo.SetActive(true);
        if (randomInt == 0)
        {
            letterOne.GetComponent<Text>().text = trueLetter;
            letterTwo.GetComponent<Text>().text = falseLetter;
        }
        else
        {
            letterOne.GetComponent<Text>().text = falseLetter;
            letterTwo.GetComponent<Text>().text = trueLetter;
        }
        
    }





    public void DisplayObstacleInDoor(int indexDoor , bool active)
    {
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        
        foreach(GameObject door in listDoor)
        {
            if(door.GetComponent<Door>().index  == indexDoor)
            {
                //door.transform.GetChild(0).GetChild(1).gameObject.SetActive(active);
                door.GetComponent<Door>().barricade = active;
                door.SetActive(!active);
            }
        }
        
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void SetNBKey()
    {
        nbKeyText.text = gameManager.game.key_counter.ToString();
    }


    public void DisplayZoneVote()
    {
        zonesVote.SetActive(true);
        zonesVote.GetComponent<Animator>().SetBool("zone_VX", true);
        soundChrono.Play();
    }

    public void HideZoneVote()
    {
        zonesVote.SetActive(false);
        zonesVote.GetComponent<Animator>().SetBool("zone_VX", false);
    }
    public void HideXZone()
    {
        
    }

    public void ActiveZoneDoor(bool samePositionAtBoss)
    {
        GameObject[] zonesDoors = GameObject.FindGameObjectsWithTag("Zone_Door");
        foreach (GameObject zoneDoor in zonesDoors)
        {
            GameObject doorParent = zoneDoor.transform.parent.gameObject;
            if (!doorParent.GetComponent<Door>().isOpenForAll && !doorParent.GetComponent<Door>().barricade )
            {
                if (samePositionAtBoss)
                {
                    zoneDoor.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
                    zoneDoor.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                }
            } 
        }
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isInJail)
        {
            if (gameManager.game.currentRoom.chest || gameManager.game.currentRoom.isNPC)
            {
                zones_X_double.SetActive(true);
                zones_X_double.GetComponent<Animator>().SetBool("zone_x", true);
                zones_X_double.transform.Find("X_zone_right").GetComponent<Animator>().SetBool("zone_x", true);
            }
            else
            {
                zones_X.SetActive(true);
                zones_X.GetComponent<Animator>().SetBool("zone_x", true);
            }
          
        }
        soundChrono.Play();
    }

    public void DesactiveZoneDoor()
    {
        GameObject[] zonesDoors = GameObject.FindGameObjectsWithTag("Zone_Door");

        foreach (GameObject zoneDoor in zonesDoors)
        {
            zoneDoor.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.25f);
            zoneDoor.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        }

        zones_X.SetActive(false);
        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoorX(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID, false,false);
        DesactivateLightAroundPlayersN2();
        zones_X.GetComponent<Animator>().SetBool("zone_x", false);

        zones_X_double.SetActive(false);
        zones_X_double.GetComponent<Animator>().SetBool("zone_x", false);
        zones_X_double.transform.Find("X_zone_right").GetComponent<Animator>().SetBool("zone_x", false);
    }

    public void ActiveXzone(bool active)
    {
        if (gameManager.game.currentRoom.chest || gameManager.game.currentRoom.isNPC)
        {
            zones_X_double.SetActive(active);
            zones_X_double.GetComponent<Animator>().SetBool("zone_x", active);
            zones_X_double.transform.Find("X_zone_right").GetComponent<Animator>().SetBool("zone_x", active);
            return;
        }
        zones_X.SetActive(active);
        zones_X.GetComponent<Animator>().SetBool("zone_x", active);
    }

    public void ResetNbVote()
    {
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in listDoor)
        {
            door.GetComponent<Door>().nbVote = 0;

        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerGO>().hasVoteVD = false;

        }


    }

    public void SetTextNbVote(int indexDoor)
    {
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in listDoor)
        {
            if (door.GetComponent<Door>().index == indexDoor)
            {
                GameObject textNbVoteDoor = door.transform.GetChild(0).GetChild(5).gameObject;
                door.GetComponent<Door>().nbVote++;
                textNbVoteDoor.GetComponent<Text>().text = door.GetComponent<Door>().nbVote.ToString();
            }
            
        }
    }

    public void OnClickHidePanel(GameObject panel)
    {
        HidePanel(panel);
    }
    public void OnClickDisplayPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void DisplayBlackScreen(bool display , bool isBlack)
    {
        transition_timer = 0;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = !display;

        if (isBlack)
        {
            blackWallPaper.SetActive(display);
            blackWallPaper.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            if(display)
                StartCoroutine(HideBlackSreenCoroutine());
        }
        else
        {
            whiteWallPaper.SetActive(display);
            whiteWallPaper.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        
    }

    public IEnumerator HideBlackSreenCoroutine()
    {
        yield return new WaitForSeconds(1.1f);
        if (blackWallPaper.activeSelf)
            DisplayBlackScreen(false, true);
    }

    public void TransitionToBlack(float t, GameObject wallPaper)
    {
        float r = blackWallPaper.GetComponent<Image>().color.r;
        float g = blackWallPaper.GetComponent<Image>().color.g;
        float b = blackWallPaper.GetComponent<Image>().color.b;

        wallPaper.GetComponent<Image>().color = new Color(r, g, b, t);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        foreach (GameObject objectDonDesroy in this.gameObject.scene.GetRootGameObjects())
        {
            Destroy(objectDonDesroy);
        }
        Destroy(GameObject.Find("Setting"));

        SceneManager.LoadScene("Menu");
        
    }

    public void DisplayGostPlayer(GameObject door)
    {
        door.transform.GetChild(5).gameObject.SetActive(true);
    }

    public void DisplayLoadPage(bool active)
    {
        LoadPage.SetActive(active);
    }
    public void DisplayRoleInformation()
    {
        roleInformation.SetActive(true);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            soundDemonicLaugh.Play();
            roleInformation.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            roleInformation.transform.GetChild(0).gameObject.SetActive(true);
        }
        StartCoroutine(HideRoleInformation());

    }

    public IEnumerator HideRoleInformation()
    {
        yield return new WaitForSeconds(2);
        roleInformation.SetActive(false);
        //waitingPage_PowerImpostor.SetActive(true);
        //int launchTimer = 20;
        int launchTimer = 2;
        if (true)
        {
            Camera.main.orthographicSize = 5.1f;
            gameManager.gameIsReady = true;
            StartCoroutine(DisplayTutorialCoroutine());
            if (gameManager.setting.displayTutorial)
            {
                tutorial[21].SetActive(false);
            }
        }
        else
        {
            DisplayPowerImpostor(true);
            Camera.main.orthographicSize = 4f;
            gameManager.timer.LaunchTimer(launchTimer, false); // 20
            yield return new WaitForSeconds(launchTimer); // 20
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower != -1)
                DisplayPowerImpostor(false);
            Camera.main.orthographicSize = 5.1f;
            yield return new WaitForSeconds(0.5f);
           StartCoroutine(DisplayTutorialCoroutine());
            gameManager.gameIsReady = true;
            if (gameManager.setting.displayTutorial)
            {
                tutorial[21].SetActive(false);
            }
        }
        
    }


    public void DisplayPowerImpostor(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            waitingPage_PowerImpostor.SetActive(display);
        else
        {
            if (gameManager.setting.displayTutorial)
            {
                if (!listTutorialBool[21])
                {
                    tutorial[21].SetActive(true);
                }
            }
            DisplayMap();
            DisplayTimerInMap(display);
            listButtonPowerImpostor[gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower].GetComponent<Button>().interactable = display;
            if(!display)
                gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasValidPowerImposter = true;
        }

        blueWallPaper.transform.Find("Canvas").Find("Text_timer").gameObject.SetActive(display);
        
    }


    public void MixLetterDoorUI()
    {
        int random = Random.Range(0, 6);

/*        string letter = random switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            3 => "D",
            4 => "E",
            5 => "F",
            _ => "X",
        };*/

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            door.transform.Find("Zones").Find("childZone").GetComponent<SpriteRenderer>().enabled = false; 
            for (int i =0; i < door.transform.Find("LettersVirus").childCount; i++)
            {
                door.transform.Find("LettersVirus").GetChild(i).gameObject.SetActive(false);
            }

            door.transform.Find("LettersVirus").GetChild(random).gameObject.SetActive(true);
        }


    }
    public void MixDistanceForExploration()
    {
        if (!timerMixExploration || gameManager.GetExpeditionOfPlayerMine().room.IsFoggy ||
            gameManager.GetExpeditionOfPlayerMine().room.IsHell || gameManager.GetExpeditionOfPlayerMine().room.IsExit)
        {

            return;
        }

        int random = Random.Range(0, 10);
        

        text_distance_room.GetComponent<Text>().text = "";

      
    }

    public IEnumerator MixDistanceExplorationStopCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(5, 11));
        if (!gameManager.GetExpeditionOfPlayerMine().room.IsFoggy)
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isCursed || gameManager.GetExpeditionOfPlayerMine().room.isCursedTrap)
            {
                int distance = gameManager.game.dungeon.GetPathFindingDistance(gameManager.GetExpeditionOfPlayerMine().room, gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().roomUsedWhenCursed);
                SetDistanceRoomFalse(distance);
            }
            else
            {
                text_distance_room.GetComponent<Text>().text =
                    gameManager.game.dungeon.GetPathFindingDistance(gameManager.GetExpeditionOfPlayerMine().room, gameManager.game.dungeon.exit).ToString();
            }


        }
        timerMixExploration = false;
        gameManager.CloseDoorExplorationWhenVote(false);
    }


    public void ResetLetterDoor()
    {
        //GameObject[] doors = gameManager.TreeDoorById();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            int indexDoor = door.GetComponent<Door>().index;
            for(int i = 0; i < door.transform.Find("LettersVirus").childCount; i++)
            {
                door.transform.Find("LettersVirus").GetChild(i).gameObject.SetActive(false);
            }

  
            door.transform.Find("Zones").Find("childZone").GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    public void DisplayParadise( bool isInExploration , int indexDoorExit)
    {
        paradise.SetActive(true);
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        foreach ( GameObject door in listDoor)
        {
            if(door.GetComponent<Door>().doorName == "B" || door.GetComponent<Door>().doorName == "C")
            {
                door.GetComponent<SpriteRenderer>().enabled = false;
                door.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
                door.transform.GetChild(6).GetComponent<SpriteRenderer>().enabled = false;
                door.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
                //door.SetActive(false);
            }
        }
        key_image.SetActive(false);
        HideDistanceRoom();
        if (!isInExploration)
        {
            soundAmbianceParadise.Play();
        }
            
        else
        {
            
            if (indexDoorExit == 1)
            {
                paradise.transform.GetChild(3).gameObject.SetActive(false);
                paradise.transform.GetChild(8).gameObject.SetActive(false);
            }
            if(indexDoorExit == 2)
            {
                paradise.transform.GetChild(4).gameObject.SetActive(false);
                paradise.transform.GetChild(9).gameObject.SetActive(false);
            }
        }
    }

    public void DisplayHell(bool isInExploration)
    {
        hell.SetActive(true);
        HideDistanceRoom();
        if (!isInExploration)
            soundAmbianceHell.Play();
    }

    public void HideParadise( )
    {
        paradise.SetActive(false);
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in listDoor)
        {
            if (door.GetComponent<Door>().doorName == "B" || door.GetComponent<Door>().doorName == "C")
            {
                door.GetComponent<SpriteRenderer>().enabled = true;
                door.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
                door.transform.GetChild(6).GetComponent<SpriteRenderer>().enabled = true;
                door.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
                //door.SetActive(false);
            }
        }
        key_image.SetActive(true);
        paradise.transform.GetChild(3).gameObject.SetActive(true);
        paradise.transform.GetChild(4).gameObject.SetActive(true);
    }

    public void HideHell()
    {
        hell.SetActive(false);
    }

    public void DisplayEchapMenu()
    {
        if (!map.activeSelf)
        {
            echap_menu.SetActive(!echap_menu.activeSelf);
        }
        
        //Cursor.visible = echap_menu.activeSelf;
    }


    public void Zoom(GameObject map, float power)
    {
        map.transform.localScale = new Vector2(map.transform.localScale.x + power , map.transform.localScale.y + power);

        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        map.transform.localPosition += new Vector3(-x * 0.001f, -y * 0.001f);
    }

    public void Dezoom(GameObject map, float power)
    {
        map.transform.localScale = new Vector2(map.transform.localScale.x - power, map.transform.localScale.y - power);
    }

    public void OnclickLauchExpedition()
    {
        gameManager.launchExpedtion_inputButton = true;
        StartCoroutine(WaitButtonInputExpeAndVoteDoor());
    }

    public void OnClickLaunchVoteDoor()
    {
        gameManager.launchVote_inputButton = true;
        StartCoroutine(WaitButtonInputExpeAndVoteDoor());
    }

    public IEnumerator WaitButtonInputExpeAndVoteDoor()
    {
        yield return new WaitForSeconds(1);
        gameManager.launchExpedtion_inputButton = false;
        gameManager.launchVote_inputButton = false;
    }

    public void  OpenDoorParadiseAnimation()
    {
        DoorRight_paradise.GetComponent<Animator>().SetBool("paradise_animation", true);
        DoorLeft_paradise.GetComponent<Animator>().SetBool("paradise_animation", true);
        if (!gameManager.GetPlayerMine().GetIsImpostor())
        {
            paradise.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;

        }
        
    }

    public void DisplayKeyPlusOne( bool display)
    {
        key_image.SetActive(display);
        KeyPlus1.SetActive(display);
        //DisplayKeyNumber(!display);
        HideNbKey();
    }

    public void DisplayKeyAndTorch(bool display)
    {
        key_image.SetActive(display);
        nbKey.SetActive(display);
        if (gameManager.setting.LIMITED_TORCH)
        {
            torch_number.transform.parent.parent.gameObject.SetActive(display);
        }
        
    }

    public void HideNbKey()
    {
        nbKey.SetActive(false);
    }


    public void LaunchAnimationBrokenKey()
    {
        key_broken_animation = true;
        Key_broken.SetActive(true);
        StartCoroutine(CoroutineBrokenKey(1));
    }


    public void AnimationBrokenKey()
    {
        Key_broken.transform.Translate(Vector3.up * Time.deltaTime * 3);
    }

    private IEnumerator CoroutineBrokenKey(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        key_broken_animation = false;
        Key_broken.SetActive(false);
        Key_broken.transform.position = new Vector2(-0.46f, -1.048f);
    }

    public void LaunchAnimationBrokenTorch()
    {
        torch_broken_animation = true;
        torch_broken.SetActive(true);
        StartCoroutine(CoroutineBrokenTorch(1));
    }

    public void AnimationBrokenTorch()
    {
        torch_broken.transform.Translate(Vector3.up * Time.deltaTime * 3);
    }

    private IEnumerator CoroutineBrokenTorch(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        torch_broken_animation = false;
        torch_broken.SetActive(false);
        torch_broken.transform.position = new Vector2(-0.46f, -1.048f);
    }


    public void LaunchAnimationAddKey()
    {
        addKeyAnimation = true;
        addKey.SetActive(true);
    }
    public IEnumerator LaunchAnimationAddKeyCouroutine()
    {
        yield return new WaitForSeconds(1);
        addKeyAnimation = true;
        addKey.SetActive(true);
        gameManager.game.key_counter++;
    }

    public void AniamtionAddKey()
    {
        if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
        {
            addKey.transform.Translate(Vector3.down * Time.deltaTime * 3);
            if (addKey.transform.position.y < -1)
            {
                addKeyAnimation = false;
                addKey.SetActive(false);
                addKey.transform.position = new Vector2(-0.12f, 2.03f);
                SetNBKey();
            }
        }
       
    }

    public void LaunchAnimationAddTorch()
    {
        addTorchAnimation = true;
        addTorch.SetActive(true);
    }

    public void AnimationAddTorch()
    {
        if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
        {
            addTorch.transform.Translate(Vector3.down * Time.deltaTime * 3);
            if (addTorch.transform.position.y < -1)
            {
                addTorchAnimation = false;
                addTorch.SetActive(false);
                addTorch.transform.position = new Vector2(-0.12f, 2.03f);
                SetTorchNumber();
            }
        }

    }

    public void ResetAllPlayerLightAround()
    {
        foreach(GameObject player  in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_blue").gameObject.SetActive(false);
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_red").gameObject.SetActive(false);
        }
    }

    public void DisplayBlackScreenToDemonWhenAllGone()
    {
        DisplayBlackScreen(true, true);
        StartCoroutine(CoroutineWaitToTransition());
    }

    public IEnumerator CoroutineWaitToTransition()
    {
        yield return new WaitForSeconds(2);
        resumePanel.SetActive(true);
        //DesactiveUIInhexagone();
        ResumeImpostor();
        ResumeDataKeyAndTorch();
    }

    public void DisplayBlackScreenToNoneImpostor()
    {
        DisplayBlackScreen(true, true);
        StartCoroutine(CoroutineWaitToTransition2());
    }

    public IEnumerator CoroutineWaitToTransition2()
    {
        yield return new WaitForSeconds(3f);
        resumePanel.SetActive(true);
        //DesactiveUIInhexagone();
        ResumeImpostor();
        ResumeDataKeyAndTorch();

    }
    public void DesactiveUIInhexagone()
    {
        GameObject[] hexagones = GameObject.FindGameObjectsWithTag("Hexagone");

        foreach(GameObject hexagone in hexagones)
        {
            hexagone.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            hexagone.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }

    }

    public void ResumeImpostor()
    {
        List<GameObject> listImpostorsName = gameManager.GetAllImpostor();
        if (gameManager.numberPlayer < 5)
        {
            resumePanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            resumePanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            resumePanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            resumePanel.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = listImpostorsName[0].GetComponent<PlayerGO>().playerName;
            Debug.Log(listImpostorsName[0].GetComponent<PlayerGO>().indexSkin);
            resumePanel.transform.Find("Impostors").Find("Impostor_solo").Find("Image").GetComponent<Image>().sprite = 
                listImpostorsName[0].transform.Find("Skins").GetChild(listImpostorsName[0].GetComponent<PlayerGO>().indexSkin)
                .Find("Colors").GetChild(listImpostorsName[0].GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            if (gameManager.numberPlayer > 6)
            {
                resumePanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                for (int i =0; i < 3; i++)
                {
                    resumePanel.transform.Find("Impostors").Find("Impostor" + i).Find("Text").GetComponent<Text>().text = listImpostorsName[i].GetComponent<PlayerGO>().playerName;
                    resumePanel.transform.Find("Impostors").Find("Impostor"+i).Find("Image").GetComponent<Image>().sprite =
                        listImpostorsName[i].transform.Find("Skins").GetChild(listImpostorsName[i].GetComponent<PlayerGO>().indexSkin)
                        .Find("Colors").GetChild(listImpostorsName[i].GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().sprite;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    resumePanel.transform.Find("Impostors").Find("Impostor" + i).Find("Text").GetComponent<Text>().text = listImpostorsName[i].GetComponent<PlayerGO>().playerName;
                    resumePanel.transform.Find("Impostors").Find("Impostor"+i).Find("Image").GetComponent<Image>().sprite =
                        listImpostorsName[i].transform.Find("Skins").GetChild(listImpostorsName[i].GetComponent<PlayerGO>().indexSkin)
                        .Find("Colors").GetChild(listImpostorsName[i].GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }

    public void  ResumeDataKeyAndTorch()
    {
        resumePanel.transform.Find("Key").GetChild(1).GetComponent<Text>().text = gameManager.game.key_counter.ToString();
        resumePanel.transform.Find("Key_Broken").GetChild(1).GetComponent<Text>().text = gameManager.nbKeyBroken.ToString();
        resumePanel.transform.Find("Torch").GetChild(1).GetComponent<Text>().text = gameManager.game.nbTorch.ToString();
        resumePanel.transform.Find("Torch_off").GetChild(1).GetComponent<Text>().text = gameManager.nbTorchOff.ToString();

    }


    public void SetTorchNumber()
    {
        torch_number.transform.parent.parent.gameObject.SetActive(true);
        torch_number.text = gameManager.game.nbTorch + "";
    }

    public IEnumerator DesactivateLightAroundPlayers()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in listPlayer)
        {
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(false);
        }
    }
    public void DesactivateLightAroundPlayersN2()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in listPlayer)
        {
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_around").gameObject.SetActive(false);
        }
    }

    public void DisplayXzoneRed()
    {
        x_zone_red.SetActive(true);
        StartCoroutine(HideXzoneRed());
    }

    public IEnumerator HideXzoneRed()
    {
        yield return new WaitForSeconds(0.2f);
        x_zone_red.SetActive(false);
    }

    public void ShowImpostor()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in listPlayer)
        {
            if(player.GetComponent<PlayerGO>().isImpostor)
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Horns").gameObject.SetActive(true);
        }
    }
    public void ShowAllDataInMap()
    {
        foreach (Hexagone hexagone in gameManager.dungeon)
        {
            ShowDataMapInOneRoom(hexagone, true);
        }
    }

    public void ShowDataMapInOneRoom(Hexagone hexagone , bool display)
    {
        Room room = hexagone.Room;
        if (room.IsObstacle)
        {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                hexagone.GetComponent<Hexagone>().distanceText.text = "";
                hexagone.GetComponent<Hexagone>().index_text.text = "";
        }
        if (room.IsTraversed)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
        }
        if (room.Index == gameManager.game.currentRoom.Index)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color(0,255,0, 1);
        }
        if (room.IsExit)
        {
            if(!display)
                hexagone.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            else
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);

            hexagone.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(display);
        }
       
        if (room.isOldParadise)
        {
            hexagone.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(display);
            if(!display)
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            else
                hexagone.transform.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
        }
        if (gameManager.hell)
        {
            if (room.X == gameManager.hell.X && room.Y == gameManager.hell.Y)
            {
                if(!display)
                    hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                else
                    hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(255 / 255f), (float)0f / 255f, (float)0f / 255f, 1);
                hexagone.transform.Find("Canvas").Find("Hell").gameObject.SetActive(display);

            }
        }
        if (room.isSpecial)
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation)
            {
                hexagone.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(false);
            }
            else
            {
                hexagone.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(true);
            }
        }
       
        
    }


    public void Display_identificationExpedition(bool display)
    {
        identification_expedition.SetActive(display);
    }

    public void Display_identificationVoteDoor(bool display)
    {
        identification_voteDoor.SetActive(display);
    }


    public void DisplayAllGost(bool display)
    {
        foreach ( Expedition expedition in gameManager.game.current_expedition){
            DisplayGostPlayer(expedition.indexNeigbour, display, expedition.player.GetId(), expedition.player.GetPlayerName()) ;
        }
    }

   public void DisplayGostPlayer(int indexDoor , bool display , int indexPlayer , string playerName = "")
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        PlayerGO player = null;
        if (display)
            player = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();

        foreach(GameObject door in doors)
        {
            if (door.GetComponent<Door>().index == indexDoor)
            {
                if (!display)
                {
                    door.transform.GetChild(5).GetComponent<CapsuleCollider2D>().enabled = true;
                    door.transform.Find("Player_gost").Find("Perso").Find("Skin").gameObject.SetActive(false);
                }
                door.transform.GetChild(5).gameObject.SetActive(display);
                door.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<Text>().text = playerName;

                if (display)
                {
                    door.transform.Find("Player_gost").Find("Perso").Find("Skin").gameObject.SetActive(display);
                    door.transform.Find("Player_gost").Find("Perso").Find("Skin").GetComponent<SpriteRenderer>().sprite =
                        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().sprite;

                }
                    //door.transform.GetChild(5).GetChild(1).GetChild(1).GetChild(player.indexSkin).gameObject.SetActive(display);
             
            }
 
        }
    }


    public void OnClickBack()
    {
        foreach (GameObject objectDonDesroy in this.gameObject.scene.GetRootGameObjects())
        {
            Destroy(objectDonDesroy);
        }
        Destroy(GameObject.Find("Setting"));
        Destroy(GameObject.Find("Input Manager"));

        SceneManager.LoadScene("Menu");
    }

    public void ResetExplorationGost()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            door.transform.Find("Player_gost").gameObject.SetActive(false);
        }
    }
    public void DisplayChestRoom(bool display)
    {
        if (display)
        {
            if (gameManager.game.currentRoom.IsExit)
            {
                return;
            }
            if (gameManager.game.currentRoom.IsHell)
            {
                return;
            }
        }
        MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display,0 , "SpeciallyRoom_levers");

        if(!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation && !gameManager.game.currentRoom.speciallyPowerIsUsed)
            DisplayAwardAndPenaltyForImpostor(display);
      
        if (!display)
        {
            ResetChestRoom();
        }
    }

    public void DisplayChest(bool display)
    {
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject;
        chestRoom.transform.Find("Chests").gameObject.SetActive(display);
    }

    public void DisplayAwardAndPenaltyForImpostor(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
        {
            return;
        }
        
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").Find("Chests").gameObject;
        foreach (Chest chest in gameManager.game.currentRoom.chestList)
        {
            if (chest.isAward)
            {
               
                if (!gameManager.game.currentRoom.isTraped || (gameManager.game.currentRoom.isTraped && (gameManager.game.currentRoom.IsFoggy || gameManager.game.currentRoom.isIllustion || gameManager.game.currentRoom.IsVirus)))
                {
                    GameObject chestRoomAward = chestRoom.transform.GetChild(chest.index).Find("Award").gameObject;
                    chestRoomAward.SetActive(display);
                    chestRoomAward.transform.GetChild(chest.indexAward).gameObject.SetActive(display);
                    if (display)
                    {
                        chestRoomAward.transform.GetChild(chest.indexAward).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
                        chestRoomAward.transform.GetChild(chest.indexAward).Find("Canvas").Find("PlusOne").GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
                    }
                    else
                    {
                        chestRoomAward.transform.GetChild(chest.indexAward).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
                        chestRoomAward.transform.GetChild(chest.indexAward).Find("Canvas").Find("PlusOne").GetComponent<Text>().color = new Color(255, 255, 255, 1f);
                    }
                }
                else
                {
                    GameObject chestRoomAward = chestRoom.transform.GetChild(chest.index).Find("Penalty").gameObject;
                    chestRoomAward.SetActive(display);
                    chestRoomAward.transform.GetChild(chest.indexTrap).gameObject.SetActive(display);
                    if (display)
                    {
                        chestRoomAward.transform.GetChild(chest.indexTrap).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
                        chestRoomAward.transform.GetChild(chest.indexTrap).Find("Canvas").Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
                    }
                    else
                    {
                        chestRoomAward.transform.GetChild(chest.indexTrap).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
                        chestRoomAward.transform.GetChild(chest.indexTrap).Find("Canvas").Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 1f);
                    }
                }
            }
            else
            {
                GameObject chestRoomPenalty = chestRoom.transform.GetChild(chest.index).Find("Penalty").gameObject;
                if (gameManager.game.currentRoom.isTraped && !gameManager.game.currentRoom.IsFoggy && !gameManager.game.currentRoom.isIllustion && !gameManager.game.currentRoom.IsVirus)
                {
                    chestRoomPenalty.SetActive(display);
                    chestRoomPenalty.transform.GetChild(chest.indexTrap).gameObject.SetActive(display);
                    if (display)
                    {
                        chestRoomPenalty.transform.GetChild(chest.indexTrap).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
                        chestRoomPenalty.transform.GetChild(chest.indexTrap).Find("Canvas").transform.Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
                    }
                    else
                    {
                        chestRoomPenalty.transform.GetChild(chest.indexTrap).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
                        chestRoomPenalty.transform.GetChild(chest.indexTrap).Find("Canvas").transform.Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 1f);
                    }
                }   
            }
        }

        SetDistanceTextAwardChest(0);
        SetDistanceTextAwardChest(1);
    }

    public void ResetChestRoom()
    {
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").Find("Chests").gameObject;
        for (int i = 0; i < chestRoom.transform.childCount; i++)
        {
            GameObject chestRoomAward = chestRoom.transform.GetChild(i).Find("Award").gameObject;
            for (int j = 0; j< chestRoomAward.transform.childCount; j++)
            {
                chestRoomAward.transform.GetChild(j).gameObject.SetActive(false);
            }
            GameObject chestRoomPenalty = chestRoom.transform.GetChild(i).Find("Penalty").gameObject;
            for (int h = 0; h < chestRoomPenalty.transform.childCount; h++)
            {
                chestRoomPenalty.transform.GetChild(h).gameObject.SetActive(false);
            }
            GameObject chestRoomVoteZone = chestRoom.transform.GetChild(i).Find("VoteZone").gameObject;
            chestRoomVoteZone.GetComponent<ChestZoneVote>().nbVote = 0;
        }

        DisplayAwardAndPenaltyForImpostor(false);
        DisplayMainLevers(true);

    }

    public void DisplayFireBallRoom(bool display)
    {
        MainRoomGraphic.transform.Find("Special").transform.Find("FireBallRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display,0 , "TrialRoom_lever");
    }
    public void DisplaySacrificeRoom(bool display)
    {
        MainRoomGraphic.transform.Find("Special").transform.Find("SacrificeRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 1 , "SpeciallyRoom_levers");
    }

    
    public void DisplaySpeciallyLevers(bool display, int indexSpecially, string groupSpeciality)
    {
        for (int i = 0; i < MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("SpeciallyRoom_levers").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("SpeciallyRoom_levers").GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoom_lever").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoom_lever").GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoomTeam_lever").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoomTeam_lever").GetChild(i).gameObject.SetActive(false);
        }
        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("SpeciallyRoom_levers").gameObject.SetActive(false);
        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoom_lever").gameObject.SetActive(false);
        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find("TrialRoomTeam_lever").gameObject.SetActive(false);

        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find(groupSpeciality).gameObject.SetActive(display);
        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").Find(groupSpeciality).GetChild(indexSpecially).gameObject.SetActive(display);
        MainRoomGraphic.transform.Find("Levers").transform.Find("All_SpeciallyRoom_lever").gameObject.SetActive(display);
 

    }
    public void HideAllLever()
    {
        DisplaySpeciallyLevers(false, 0, "TrialRoom_lever");
        DisplaySpeciallyLevers(false, 0, "SpeciallyRoom_levers");
        DisplaySpeciallyLevers(false, 0, "TrialRoomTeam_lever");
    }

    public void DisplayMainLevers(bool display)
    {
        DisplayLeverVoteDoor(display);  
    }

    public void DisplayLeverVoteDoor(bool display)
    {
        MainRoomGraphic.transform.Find("Levers").transform.Find("OpenDoor_lever").gameObject.SetActive(display);
    }


    public void SetDistanceTextAwardChest(int indexChest)
    {
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").Find("Chests").gameObject;

        chestRoom.transform.GetChild(indexChest).Find("Award").Find("Distance").Find("Canvas").Find("PlusOne").GetComponent<Text>().text = gameManager.game.currentRoom.DistancePathFinding.ToString();
    }

    public void ClearSpecialRoom()
    {
        for(int i = 0; i < MainRoomGraphic.transform.Find("Special").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Special").transform.GetChild(i).gameObject.SetActive(false);
        }
        ResetChestRoom();
    }

    public void DisplayVoteChest(bool display)
    {
        for (int i = 0; i < MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").Find("Chests").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").Find("Chests").GetChild(i).Find("VoteZone").gameObject.SetActive(display);
        }
    }

    public IEnumerator DisplayTutorialCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (gameManager.setting.displayTutorial)
        {
            panelTutoriel.SetActive(true);
            panelTutoriel.transform.parent.gameObject.SetActive(true);
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                tutorial[1].gameObject.SetActive(true);
            }
            else
            {
                tutorial[0].gameObject.SetActive(true);
            }
            
        }
    }

    public void ActiveZoneVoteChest(bool active)
    {
        foreach(GameObject chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            chest.transform.Find("VoteZone").gameObject.SetActive(active);
        }
        
    }

    public void OpenParadise()
    {
        //gameManager.GetPlayerMineGO().transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
        OpenDoorParadiseAnimation();
    }

    public void DisplayPanelOneExplorationTuto()
    {
        oneExplorationTutoPanel.SetActive(true);
    }

    public void QuitTutorialN7()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendQuitTutorialN7(true);
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

    public void DisplayNuVoteSacrificeForAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            /*            if (!gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                        {
                            player.transform.GetChild(0).gameObject.SetActive(true);
                            player.transform.GetChild(1).gameObject.SetActive(true);
                            player.transform.GetChild(1).GetChild(1).GetChild(player.GetComponent<PlayerGO>().indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                        }*/
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(true);
        }
    }

    public void HideNuVoteSacrificeForAllPlayer()
    {
        if (!gameManager.SamePositionAtBoss())
        {
            return;
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
        }
    }



    public void DisplayJailRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("JailRoom").gameObject.SetActive(display);
    }
    public void DisplayFoggyRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("FoggyRoom").gameObject.SetActive(display);
    }
    public GameObject DisplayVirusRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("VirusRoom").gameObject.SetActive(display);

        return GameObject.Find("Special").transform.Find("VirusRoom").gameObject;
    }
    public void DisplayDeathNPCRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("DeathNPCRoom").gameObject.SetActive(display);
        //GameObject.Find("Special").transform.Find("DeathNPCRoom").Find("DeathNpc").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 0 , "TrialRoomTeam_lever");
    }
    public void DisplayDamoclesSwordRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("DamoclesSwordRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 1, "TrialRoom_lever");
    }
    public void DisplayAxRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("AxRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 2 , "TrialRoom_lever");
    }
    public void DisplaySwordRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("SwordRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 3, "TrialRoom_lever");
    }
    public void DisplayLostTorchRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("LostTorchRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 4, "TrialRoom_lever");
    }
    public void DisplayMonstersRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("MonstersRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 1, "TrialRoomTeam_lever");
    }
    public void DisplayPurificationRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("PurificationRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 2 , "SpeciallyRoom_levers");
    }
    public void DisplayResurectionRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("ResurectionRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 3 , "SpeciallyRoom_levers");
    }
    public void DisplayPrayRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("PrayRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 4 , "SpeciallyRoom_levers");
    }
    public void DisplayNPCRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("NPCRoom").gameObject.SetActive(display);
        //DisplaySpeciallyLevers(display, 13);
    }
    public void DisplayLabyrinthRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("LabyrinthHideRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display, 5 , "TrialRoom_lever");
    }
    public void DisplayImpostorRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("ImpostorRoom").gameObject.SetActive(display);
    }
    public void DisplayInformationEndRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("InformationEndRoom").gameObject.SetActive(display);
    }

    public void DisplayIllustionRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("IllusionRoom").gameObject.SetActive(display);
    }

    public void DisplayUI_Mobile_SpecialRoom(bool display)
    {
        if (gameManager.game.currentRoom.chest)
            mobileCanvas.transform.Find("Chest_panel").gameObject.SetActive(display);
        if (gameManager.game.currentRoom.isSacrifice)
            mobileCanvas.transform.Find("Sacrifice_panel").gameObject.SetActive(display);
        if (gameManager.game.currentRoom.fireBall)
            mobileCanvas.transform.Find("FireBall_panel").gameObject.SetActive(display);
        if (gameManager.game.currentRoom.IsVirus)
            mobileCanvas.transform.Find("Damned_panel").gameObject.SetActive(display);
    }

    public void SetDescriptionLoadPage(string description , float secondes)
    {
       //LoadPage.transform.Find("text_description").GetComponent<Text>().text = description;
        StartCoroutine(CouroutineSetText(LoadPage.transform.Find("text_description").GetComponent<Text>(), description, secondes));
    }

    public IEnumerator CouroutineSetText(Text textObj , string newText , float secondes)
    {
        yield return new WaitForSeconds(secondes);
        textObj.text = newText;
    }

    public void OnClickChatButton()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().OnClickChat();
    }

    public void DisplayTutorialAutel(bool display)
    {
        autelTutorial.SetActive(display);
    }
    public void DisplayAutelTutorialSpeciallyRoom(bool display)
    {
        autelTutorialSpeciallyRoom.SetActive(display);
    }


    public void OnClickHexagonePowerImpostor()
    {
        if (gameManager.setting.displayTutorial)
        {
            tutorial[21].transform.Find("First").gameObject.SetActive(false);
            tutorial[21].transform.Find("Second").gameObject.SetActive(true);
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter = !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter;
    }

    public void HideAllImpostorInformation(bool hide)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isImpostor)
            {
                player.gameObject.transform.Find("Skins").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexSkin).Find("Horns").gameObject.SetActive(hide);
            }
        }

        foreach (Hexagone hexagone in gameManager.dungeon)
        {
            ShowDataMapInOneRoom(hexagone, hide);
        }
    }

    public void OnclickHideImpostorInformation()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            return;
        }
        bool hideImpostorInformation = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation = !hideImpostorInformation;
        HideAllImpostorInformation(hideImpostorInformation);
        DisplayAwardAndPenaltyForImpostor(hideImpostorInformation);
    }

    public void DisplayPanelInWaiting(bool display)
    {
        panelInWaiting.SetActive(display);
    }


    public void HideAllTutorial()
    {
        for(int i =0; i < tutorial_parent.transform.childCount; i++)
        {
            tutorial_parent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DisplayPowerImpostorInGame()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower == -1)
            return;
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).gameObject.SetActive(true);
        DisplayTrapPowerButtonDesactivateTime(true, 30);
    }
    public void DisplayObjectPowerImpostorInGame()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower == -1)
            return;
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).gameObject.SetActive(true);
    }

    public void HidePowerButtonImpostor()
    {
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).gameObject.SetActive(false);
    }

    public void DesactivateObjectPowerImpostor(bool interactable)
    {
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower)
            .GetComponent<Button>().interactable = interactable;
    }
    public void DisplayN2PotionObject(bool display)
    {
        /*        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower)
                    .Find("Potion").gameObject.SetActive(!display);*/
        if (canvasInGame.transform.Find("Object"))
        {
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower)
                .Find("BiggerReset").gameObject.SetActive(display);
        }
          
    }

    public void DisplayObjectPowerBigger(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower == -1)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Bigger").gameObject.SetActive(display);
    }
    public void DisplayObjectResetInvisibility(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower == -1)
            return;

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("BiggerReset").gameObject.SetActive(display);
    }

    public void DisplayObjectPowerButtonDesactivateTime(bool display, float timer)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower == -1)
            return;
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").gameObject.SetActive(display);
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").Find("Text_timer").gameObject.SetActive(display);
        if (display)
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().canUsed = false;
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").Find("Text_timer").GetComponent<TimerDisplay>().timeLeft = timer;
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").Find("Text_timer").GetComponent<TimerDisplay>().timerLaunch = true;
        }
           
    }
    public void DisplayTrapPowerButtonDesactivateTime(bool display, float timer)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower == -1)
            return;
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").gameObject.SetActive(display);
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").Find("Text_timer").gameObject.SetActive(true);
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").Find("Text_timer").GetComponent<TimerDisplay>().timeLeft = 0;
        if (display) {
            canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").Find("Text_timer").GetComponent<TimerDisplay>().timeLeft = timer;
            canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").Find("Text_timer").GetComponent<TimerDisplay>().timerLaunch = true;
        }
    }

    public void DisplayObjectPowerButtonDesactivate(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower == -1)
            return;
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").gameObject.SetActive(display);
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").Find("Text_timer").gameObject.SetActive(!display);

        if (display)
        {
            gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().canUsed = false;
            canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Bigger").gameObject.SetActive(false);
        }

    }
    public void HideObjectPowerButtonDesactivate()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").gameObject.SetActive(false);
        canvasInGame.transform.Find("Object").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexObjectPower).Find("Timer").Find("Text_timer").gameObject.SetActive(false);
    }

    public void DisplayTrapPowerButtonDesactivate(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower == -1)
            return;
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").gameObject.SetActive(display);
        canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Timer").Find("Text_timer").gameObject.SetActive(false);

        if (display)
        {
            gameManager.GetPlayerMineGO().transform.Find("PowerImpostor").GetComponent<PowerImpostor>().canUsed = false;
            canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Bigger").gameObject.SetActive(false);   
        }

    }


    public void DisplayTrapPowerBigger(bool display)
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower != -1)
            canvasInGame.transform.Find("Power").GetChild(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower).Find("Bigger").gameObject.SetActive(display);
    }

    public void DisplayButtonPowerExplorationBigger(bool display)
    {
        canvasInGame.transform.Find("Exploration").Find("Torch").Find("Bigger").gameObject.SetActive(display);
    }
    public void OnClickButtonPowerExplorationBigger()
    {
        if (gameManager.game.nbTorch <= 0 && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            return;
        if (gameManager.game.currentRoom.explorationIsUsed)
            return;
        int indexDoor = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collsionDoorIndexForExploration;
        if (gameManager.GetDoorGo(indexDoor))
        {
            gameManager.GetDoorGo(indexDoor).GetComponent<Door>().DisplayColorLightToExploration();
            gameManager.ui_Manager.doorTorched.Play();
            gameManager.gameManagerNetwork.SendDisplayLightExplorationTransparency(indexDoor);
            if (gameManager.GetPlayerWithTorchBarre())
                gameManager.GetPlayerWithTorchBarre().GetComponent<PlayerNetwork>().SendExplorationPowerIsAvailable(true);
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().explorationPowerIsAvailable = false;
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendExplorationPowerIsAvailable(false);

            canvasInGame.transform.Find("Exploration").Find("Torch").Find("Bigger").gameObject.SetActive(false);
            canvasInGame.transform.Find("Exploration").Find("Torch").Find("Disabled").gameObject.SetActive(true);

            // set player when win trial game
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayWhiteLight(false);
            
            if(!(gameManager.game.currentRoom.isTeamTrial && !gameManager.game.currentRoom.speciallyPowerIsUsed))
                gameManager.gameManagerNetwork.SendDisplayMainLevers(true);
           

            // set gameManager
            if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
                gameManager.game.nbTorch--;
                gameManager.gameManagerNetwork.SendTorchNumber(gameManager.game.nbTorch);
            }
            else
            {
                gameManager.gameManagerNetwork.SendTimerForcePause(false);
            }
            if (gameManager.game.nbTorch == 0)
                DisabledButtonPowerExploration(true);
            gameManager.gameManagerNetwork.SendExplorationIsUsed(gameManager.game.currentRoom.Index, true);

            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(false);

            //gameManager.ui_Manager.DisplayAllDoorLightExploration(false);
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayBlueTorch(false);
            gameManager.gameManagerNetwork.SendDisplaySupportTorch(false);
            gameManager.gameManagerNetwork.SendIndexPreviousExplorater(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        }
           
    }

    public void DisabledAllButtonBigger()
    {
        DisplayButtonPowerExplorationBigger(false);
        DisplayButtonChnageBossBigger(false);
        DisplayButtonMapBigger(false);
        DisplayButtonMainKeyBigger(false);
    }

    public void DisabledButtonPowerExploration(bool display)
    {
        canvasInGame.transform.Find("Exploration").Find("Torch").Find("Disabled").gameObject.SetActive(display);
        if (display)
            DisplayButtonPowerExplorationBigger(false);
    }

    public void DisplayImgInHexagoneUseWhenCursedPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int indexRoom = -1;
        foreach(GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().isCursed)
                continue;
            indexRoom = player.GetComponent<PlayerGO>().roomUsedWhenCursed.Index;
        }
        gameManager.GetHexagone(indexRoom).transform.Find("Canvas").Find("Cursed").gameObject.SetActive(true);
    }
    public void HideSpeciallyDisplay()
    {
        if (!GameObject.Find("Special"))
            return;
        GameObject.Find("Special").gameObject.SetActive(false);
        HideAllLever();
        gameManager.DisplayZoneDoorForEachSituation();
        //DisplayLightLeverSpeciallyRoom(false);
    }

    public void HideImgInMiddleOfSpeciallyRoom(Room room, bool display)
    {
        if (room.isPurification)
        {
            GameObject.Find("Special").transform.Find("PurificationRoom").Find("Status").gameObject.SetActive(display);
            GameObject.Find("Special").transform.Find("PurificationRoom").Find("zone").gameObject.SetActive(display);
        }
        if (room.isResurection)
        {
            GameObject.Find("Special").transform.Find("ResurectionRoom").Find("Status").gameObject.SetActive(display);
        }
        if (room.isPray)
        {
            GameObject.Find("Special").transform.Find("PrayRoom").Find("Status").gameObject.SetActive(display);
            GameObject.Find("Special").transform.Find("PrayRoom").Find("ZonesPray").gameObject.SetActive(display);
        }
        if (room.isNPC)
        {
            GameObject.Find("Special").transform.Find("NPCRoom").Find("NPC").gameObject.SetActive(display);
        }

    }

    public void ChangeColorAllPlayerSkinToFoggy(bool display)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (display)
            {
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255,255,255,0.7f);
            }
            else
            {
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }    
        }
    }

    public bool SkinHasEyes(int indexSkin)
    {
        if (indexSkin == 0 || indexSkin == 1 || indexSkin == 3 || indexSkin == 5 || indexSkin == 8)
            return true;
        return false;
    }

    public void DesactivateInformationSpecallyRoomAllHexagone()
    {
        foreach (Hexagone hexagone in gameManager.dungeon)
        {
            DiplayInformationHexagoneSpeciallyRoom(true, hexagone.GetComponent<Hexagone>());
        }
    }

    public void DiplayInformationHexagoneSpeciallyRoom(bool display , Hexagone hexagone)
    {
        Room room = hexagone.Room;
        //Debug.Log(room.Index);
        if (!room)
            return;
        if (room.IsObstacle || room.IsInitiale || room.IsTraversed )
            return;
        if (room.isHide)
        {
            GameObject interogationPoint = hexagone.transform.Find("Integoration_point").gameObject;
            interogationPoint.SetActive(false);
            return;
        }
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            if (room.IsExit)
                hexagone.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);
            if (room.IsHell)
                hexagone.transform.Find("Canvas").Find("Hell").gameObject.SetActive(true);
            if (room.isImpostorRoom)
                hexagone.transform.Find("Information_Speciality").gameObject.SetActive(true);
        }
       
        if (room.isSpecial)
        {
            hexagone.gameObject.transform.Find("Information_Speciality").gameObject.SetActive(true);
        }
            
    }
    
    public void DisplayLightLeverSpeciallyRoom(bool display)
    {
        Room room = gameManager.game.currentRoom;
        if (!GameObject.Find("Room").transform.Find("Levers").Find("All_SpeciallyRoom_lever").gameObject.activeSelf)
            return;

        if (gameManager.timer.timerLaunch)
            return;

        if (room.isTeamTrial)
        {
            GameObject lever = GameObject.Find("All_SpeciallyRoom_lever").transform.Find("TrialRoomTeam_lever").gameObject;
            if (room.isMonsters)
                lever.transform.Find("MonsterRoom").Find("Light").gameObject.SetActive(display);
            if (room.isDeathNPC)
                lever.transform.Find("DeathNPC").Find("Light").gameObject.SetActive(display);
            return;
        }

        if (room.isTrial)
        {
            GameObject lever = GameObject.Find("All_SpeciallyRoom_lever").transform.Find("TrialRoom_lever").gameObject;
            if (room.fireBall)
                lever.transform.Find("FireBall").Find("Light").gameObject.SetActive(display);
            if (room.isSwordDamocles)
                lever.transform.Find("DamoclesSword").Find("Light").gameObject.SetActive(display);
            if (room.isAx)
                lever.transform.Find("Ax").Find("Light").gameObject.SetActive(display);
            if (room.isSword)
                lever.transform.Find("Sword").Find("Light").gameObject.SetActive(display);
            if (room.isLostTorch)
                lever.transform.Find("LostTorchLever").Find("Light").gameObject.SetActive(display);
            if (room.isLabyrintheHide)
                lever.transform.Find("LabyrinthRoom").Find("Light").gameObject.SetActive(display);

            return;
        }
        if (room.isVerySpecial || room.speciallyIsInsert || room.isTraped)
        {
            GameObject lever = GameObject.Find("All_SpeciallyRoom_lever").transform.Find("SpeciallyRoom_levers").gameObject;
            if (room.chest)
                lever.transform.Find("Chest").Find("Light").gameObject.SetActive(display);
            if (room.isSacrifice)
                lever.transform.Find("Sacrifice").Find("Light").gameObject.SetActive(display);
            if (room.isPurification)
                lever.transform.Find("Purification").Find("Light").gameObject.SetActive(display);
            if (room.isPray)
                lever.transform.Find("Pray").Find("Light").gameObject.SetActive(display);
        }
        
    }

    public void SetRedColorDoorTrapedSpeciallyRoom(int indexDoor , bool display)
    {
        //Door door = gameManager.GetDoorGo(indexDoor).GetComponent<Door>();
        Room room = gameManager.game.currentRoom;


        if (room.IsVirus)
        {
            if (!GameObject.Find("VirusRoom"))
                return;
            GameObject doors = GameObject.Find("VirusRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color.a);
            Debug.Log("sa passe " + display);
            if (display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(123f / 255, 35f / 255, 35f / 255, doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color.a);
            }
            else
            { 
                 doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255f / 255, 255f / 255, 255f / 255, doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color.a);
            }
        }

        if (room.isSwordDamocles)
        {
            if (!GameObject.Find("DamoclesSwordRoom"))
                return;
            GameObject doors = GameObject.Find("DamoclesSwordRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
        }
        if (room.isAx)
        {
            if (!GameObject.Find("AxRoom"))
                return;
            GameObject doors = GameObject.Find("AxRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
        }
        if (room.isSword)
        {
            if (!GameObject.Find("SwordRoom"))
                return;
            GameObject doors = GameObject.Find("SwordRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(115f/255, 115f/255, 115f/255);
            }
        }
        if (room.isMonsters)
        {
/*            if (!GameObject.Find("MonstersRoom"))
                return;
            GameObject doors = GameObject.Find("MonstersRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(155f/255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color((118f/255), (100f/255), (100f/255));
            }*/
        }
        if (room.isNPC)
        {
            if (!GameObject.Find("NPCRoom"))
                return;
            GameObject doors = GameObject.Find("NPCRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(132f/255, 132f/255, 132f/255);
            }
        }
        if (room.isLabyrintheHide)
        {
            if (!GameObject.Find("LabyrinthHideRoom"))
                return;
            GameObject doors = GameObject.Find("LabyrinthHideRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(111f/255, 111f/255, 111f/255);
            }
        }
        if (room.isSacrifice)
        {
            if (!GameObject.Find("SacrificeRoom"))
                return;
/*            GameObject doors = GameObject.Find("SacrificeRoom").transform.Find("Doors").gameObject;
            doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (!display)
            {
                doors.transform.GetChild(indexDoor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }*/
        }
    }
    public void DisplayLetterInSkull(bool display)
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in doors)
        {
            door.transform.Find("Canvas").Find("Text").gameObject.SetActive(display);
            door.transform.Find("Canvas").Find("Image").gameObject.SetActive(display);
        }
    }

    public void DisplayReconnexionPanel(bool display)
    {
        ReconnexionPanel.SetActive(display);
    }

    public void DisplayInterogationPoint()
    {
        text_distance_room.GetComponent<Text>().text = "?";
    }

    public void SetRandomObstacles(GameObject speciallyRoom)
    {
        speciallyRoom.transform.Find("Obstacles").gameObject.SetActive(true);

        int childCount = speciallyRoom.transform.Find("Obstacles").childCount;
        for(int i = 0;i < childCount ; i++)
        {
            int randomGroupObstacle = Random.Range(0, speciallyRoom.transform.Find("Obstacles").GetChild(i).childCount);
            speciallyRoom.transform.Find("Obstacles").GetChild(i).GetChild(randomGroupObstacle).gameObject.SetActive(true);
        }
    }

    

    public void DesactivateAllobstacles(string nameObject, bool display)
    {
        if (!GameObject.Find(nameObject).transform.Find("Obstacles"))
            return;
        GameObject.Find(nameObject).transform.Find("Obstacles").gameObject.SetActive(display);
        GameObject speciallyRoom = GameObject.Find(nameObject);
        if (display)
        {
            for (int i = 0; i < speciallyRoom.transform.Find("Obstacles").childCount; i++)
            {
                if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                    speciallyRoom.transform.Find("Obstacles").GetChild(i).GetComponent<Obstacle>().LaunchRandomDesactive();
            }
        }
    }


    public void HideLightExplorationAllDoor()
    {
        for(int i =0; i < gameManager.doorsParent.transform.childCount; i++)
        {
            for (int j = 0; j < gameManager.doorsParent.transform.GetChild(i).Find("LightsExploration").childCount; j++)
            {
                gameManager.doorsParent.transform.GetChild(i).Find("LightsExploration").GetChild(j).gameObject.SetActive(false);
            }
        }
    }
    public void DisplayAllDoorLightExploration(bool display)
    {
        if (display)
        {
            PlayerGO playerMine = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
            if (!playerMine.explorationPowerIsAvailable && !playerMine.hasWinFireBallRoom)
                return;
            if (gameManager.game.currentRoom.explorationIsUsed)
                return;
            if (gameManager.voteDoorHasProposed)
                return;
        }
        
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        for (int i =0; i < doors.Length; i++)
        {
            DisplayDoorLightExploration(doors[i].GetComponent<Door>().index, display);
        }
    }

    public void DisplayDoorLightExploration(int index, bool display)
    {
        Door door = gameManager.GetDoorGo(index).gameObject.GetComponent<Door>();
        if (door.isOpenForAll && display)
            return;

        door.gameObject.transform.Find("LightInformation").Find("White").gameObject.SetActive(display);
    }

    public void DisplayAllDoorLightOther(bool display)
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        for (int i = 0; i < doors.Length; i++)
        {
            DisplayDoorLightOther(doors[i].GetComponent<Door>().index, display);
        }
    }

    public void DisplayDoorLightOther(int index, bool display)
    {
        Door door = gameManager.GetDoorGo(index).gameObject.GetComponent<Door>();
        if ((door.isOpenForAll || door.GetRoomBehind().IsExit || door.GetRoomBehind().IsHell) && display )
            return;

        door.gameObject.transform.Find("LightInformation").Find("White").gameObject.SetActive(display);
    }

    public void DisplayMagicalKeyButton()
    {
        canvasInGame.transform.Find("Exploration").Find("Torch").gameObject.SetActive(false);
        canvasInGame.transform.Find("Exploration").Find("MagicalKey").gameObject.SetActive(true);
    }
    public void DisplayMagicalKeyButtonBigger(bool display)
    {
        canvasInGame.transform.Find("Exploration").Find("MagicalKey").Find("Bigger").gameObject.SetActive(display);
    }

    public void OnClickButtonMagicalKey()
    {
        panelChooseRoom.SetActive(true);
        canvasInGame.transform.Find("Exploration").Find("MagicalKey").Find("Bigger").gameObject.SetActive(false);
        canvasInGame.transform.Find("Exploration").Find("MagicalKey").gameObject.SetActive(false);
        canvasInGame.transform.Find("Exploration").Find("Torch").gameObject.SetActive(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayMagicalKey(false);
        DisplayAllDoorLightExploration(false);
        gameManager.gameManagerNetwork.SendDisplayDoorLever(true);
    }

    public void OnClickButtonKeyTraped()
    {
        panelChooseRoomTrap.SetActive(true);        
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayMagicalKey(false);
        DisplayAllDoorLightExploration(false);
        gameManager.ui_Manager.traped_door.Play();
    }

    public void UpdateRoomWithMagicalkey(int indexChoice)
    {
        int indexDoor = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collsionDoorIndexForExploration;
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(false);
        if (!gameManager.GetDoorGo(indexDoor))
            return;
       
        gameManager.gameManagerNetwork.SendNewSpeciallyRoom(gameManager.GetDoorGo(indexDoor).GetComponent<Door>().GetRoomBehind().Index, indexChoice);
        gameManager.gameManagerNetwork.SendOrangeDoor(gameManager.GetDoorGo(indexDoor).GetComponent<Door>().index);
        gameManager.gameManagerNetwork.SendTemporyCloseDoor();
    }
    public void UpdateRoomWithTrapedkey(int indexChoice)
    {
        int indexDoor = gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().collision.GetComponent<Door>().index;
        if (!gameManager.GetDoorGo(indexDoor))
            return;

        gameManager.gameManagerNetwork.SendNewTrapedRoom(gameManager.GetDoorGo(indexDoor).GetComponent<Door>().GetRoomBehind().Index, indexChoice);
        gameManager.gameManagerNetwork.SendDisplayTrappedDoor(indexDoor);
        gameManager.GetPlayerMineGO().transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().SetRedLightColorDoor(false, gameManager.GetDoorGo(indexDoor).GetComponent<Door>().index);
    }

    public void DisplayButtonBlackTorch(bool display)
    {
        canvasInGame.transform.Find("Exploration").Find("BlackTorch").gameObject.SetActive(display);
    }

    public void DisplayButtonBlackTorchBigger(bool display)
    {
        canvasInGame.transform.Find("Exploration").Find("BlackTorch").Find("Bigger").gameObject.SetActive(display);
    }
    public void OnClickBlackTorchButton()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach(GameObject door in doors)
        {
            door.GetComponent<Door>().DisplayColorLightToExploration();
        }
        canvasInGame.transform.Find("Exploration").Find("BlackTorch").Find("Bigger").gameObject.SetActive(false);
        canvasInGame.transform.Find("Exploration").Find("BlackTorch").gameObject.SetActive(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(false);
        gameManager.gameManagerNetwork.SendDisplayMainLevers(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayWhiteLight(false);
        if (!gameManager.ISTrailsRoom(gameManager.game.currentRoom))
        {
            gameManager.game.nbTorch--;
            gameManager.gameManagerNetwork.SendTorchNumber(gameManager.game.nbTorch);
        }
        gameManager.gameManagerNetwork.SendExplorationIsUsed(gameManager.game.currentRoom.Index, true);
        if (gameManager.GetPlayerWithTorchBarre())
            gameManager.GetPlayerWithTorchBarre().GetComponent<PlayerNetwork>().SendExplorationPowerIsAvailable(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendExplorationPowerIsAvailable(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayBlackTorch(false);
    }

    public void DisplayInformationObjectWon(int index)
    {
        panelInformationObjectWon.gameObject.SetActive(true);
        panelInformationObjectWon.gameObject.transform.Find("Panel").GetChild(index).gameObject.SetActive(true);

    }
    public void DisplayInformationPowerWon(int index)
    {
        panelInformationPowerWon.gameObject.SetActive(true);
        panelInformationPowerWon.gameObject.transform.Find("Panel").GetChild(index).gameObject.SetActive(true);
    }

    public void DisplayButtonNPCBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("NPC_interaction").gameObject.SetActive(display);
    }

    public void DisplayButtonMainKeyBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("Key_intercation").gameObject.SetActive(display);
    }
    public void OnClickButtonMainKey()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().launchVoteDoorMobile = true;
        canvasInGame.transform.Find("Interaction").Find("Key_intercation").gameObject.SetActive(false);

    }
    public void DisplayButtonMapBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("Map_interaction").gameObject.SetActive(display);
    }
    public void OnClickMapBigger()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().displayMap = true;
    }
    public void DisplayButtonChnageBossBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("Boss_interaction").gameObject.SetActive(display);
    }
    public void OnClickChangeBossBigger()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().changeBoss = true;

        if (gameManager.usedChangeBossPower)
        {
            gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayOpenDoorBeforeChangeBoss();
        }
    }

    public void DisplayButtonTutorialBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("Tutorial_interaction").gameObject.SetActive(display);
    }

    public void OnClickTutorialBigger()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().displayTutorial = true;
    }

    public void DisplayButtonSpeciallyRoomKeyBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("SpeciallyRoom_intercation").gameObject.SetActive(display);
    }

    public void DisplayButtonTrialRoomKeyBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("TrialRoom_intercation").gameObject.SetActive(display);
    }

    public void OnClickSpeciallyRoomKeyBigger()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().onClickButtonKeySpecially = true;
    }
    public void OnClickTrialRoomKeyBigger()
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().onClickButtonKeySpecially = true;
    }


    public void OnClickButtonNPC()
    {
        int indexNPC = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexNpc;
        GameObject.Find("NPCRoom").GetComponent<NPCRoom>().SendDisplayDistanceByNpc(indexNPC);
    }

    public void DisplayButtonNPC_InformationEndBigger(bool display)
    {
        canvasInGame.transform.Find("Interaction").Find("NPCInformationEnd_interaction").gameObject.SetActive(display);
    }
    public void OnClickButtonNPCInformationEnd()
    {
        int indexNPC = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexNpc;
        GameObject.Find("InformationEndRoom").GetComponent<informationEndRoom>().LaunchInformationEndRoom();
    }

    public void DisplayPanelBossInformation(bool display)
    {
        if (gameManager.speciallyIsLaunch)
            return;
        panelBossInformation.SetActive(display); 
    }

   

    public void DisplayMapImpostorInButtonPanel()
    {
        RemoveLightHexagone();
        if (map.activeSelf)
        {
            DisplayMap();
            DisplayMapLostSoul(false);
        }
        else
        {
            DisplayMapLostSoul(false);
            DisplayMap();
        }
        hexagoneImpostorInformation.gameObject.SetActive(false);
        hexagoneNoneImpostorInformation.gameObject.SetActive(true);
    }

    public void DisplayMapImpostorInButtonPanel2()
    {
        RemoveLightHexagone();
        if (map.activeSelf)
        {
            DisplayMap();
            DisplayMapLostSoul(false);
        }
        else
        {
            DisplayMapLostSoul(false);
            DisplayMap();
        }
        hexagoneImpostorInformation.gameObject.SetActive(true);
        hexagoneNoneImpostorInformation.gameObject.SetActive(false);
    }

    public bool lastIsImpostor = false;

    public void DisplayMapImpostor()
    {
        //RemoveLightHexagone();

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            buttonDisplayMapImpostor.SetActive(true);

        if (map.activeSelf || mapLostSoul.activeSelf)
        {
            if (map.activeSelf)
                lastIsImpostor = true;
            else
                lastIsImpostor = false;
            HidePanel(mapLostSoul);
            HidePanel(map);
            HidePanel(blueWallPaper);
            gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
            DesactivateInformationSpecallyRoomAllHexagone();
            RemoveLightHexagone();
        }
        else
        {
            DisplayMapImpostorInSituation();
        }
        

    }

    public void OnclickBackButtonMap()
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            DisplayMapImpostor();
        }
        else
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasMap)
                gameManager.ui_Manager.DisplayMap();
            else
                gameManager.ui_Manager.DisplayMapLostSoul(false);
        }
    }

    public void DisplayMapImpostorInSituation()
    {

        if (lastIsImpostor)
        {   
            DisplayMap();
        }
        else
        {
            DisplayMapLostSoul(false);
        }

    }

    public void ActiveSlideMap()
    {
        map.GetComponent<Map_zoom>().enabled = true;
    }

    public void ImpactSword()
    {
        impactSword.Play();
    }
    
    public void LaunchFightMusic()
    {
        musicFight.Play();
        timerMusic = BasesMusic.time;
        timerMusic2 = BasesMusic2.time;
        timerMusic3 = BasesMusic3.time;
        timerCave = CaveMusicAmbiance.time;

        BasesMusic.Stop();
        BasesMusic2.Stop();
        BasesMusic3.Stop();
        CaveMusicAmbiance.Stop();
        //CaveMusicAmbiance.gameObject.SetActive(false);
        currentMusic_index = -1;
    }

    public void HideFightMusic()
    {
        musicFight.Stop();
        currentMusic_index = -1;
        CaveMusicAmbiance.volume = 0;
        IncreaseVolumeLittleToLittle(CaveMusicAmbiance, timerMusic3);
      
        Debug.LogError("sa passe 2");
        StartCoroutine(gameManager.StartAmbianceBeginingMusic());
       
    }
    public void LaunchDashSound()
    {
        dashSound.Play();
    } 


    public void IncreaseVolumeLittleToLittle(AudioSource music, float timerMusic)
    {
        music.gameObject.SetActive(true);
        music.time = timerMusic;
        music.Play();
        currentMusic = music;
        launchIncreaseVolumLittleToLittle = true;
    }

    public void DisplayInteractionObject(bool display)
    {
        map_interaction.SetActive(display);
        changeBoss_interaction.SetActive(display);
    }

    public void OnClickDisplayTutorialExplorationAfterBossPanel()
    {
        StartCoroutine(CouroutineDisplayTutorialExplorationAfterBossPanel());
    }
    public IEnumerator CouroutineDisplayTutorialExplorationAfterBossPanel()
    {
        yield return new WaitForSeconds(1.25f);
        if (gameManager.setting.displayTutorial)
        {
            if (!gameManager.ui_Manager.listTutorialBool[4])
            {
                gameManager.ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
                gameManager.ui_Manager.tutorial_parent.SetActive(true);
                gameManager.ui_Manager.tutorial[4].SetActive(true);
                gameManager.ui_Manager.listTutorialBool[4] = true;
            }

        }
    }

    public void DisplayMapForEnd()
    {
        SetColorWallPaper();
        CanMapMoveAtEnd();
        DisplayImpostorRoomForEnd();

    }
    
    public void DisplayImpostorRoomForEnd()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;

        int indexImpostorRoom = gameManager.GetImpostorRoom();
        gameManager.gameManagerNetwork.SendImpostorRoomForEnd(indexImpostorRoom);
    }

    public void CanMapMoveAtEnd()
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasMap)
            return;

        listHexa.GetComponent<Map_zoom>().enabled = true;
    }

    public void HideAllZoneDoor()
    {
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in listDoor)
        {
            int childCount = door.transform.Find("Zones").transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (i == 0)
                    continue;
                door.transform.Find("Zones").GetChild(i).transform.gameObject.SetActive(false);
            }
            
        }
    }

    public void DisplayZoneWithoutWay(string speciallyRoomString)
    {
        GameObject doorPivot = GameObject.FindGameObjectWithTag("Door");
        GameObject parentDoor = doorPivot.transform.parent.gameObject;
        GameObject speciallyRoom = GameObject.Find(speciallyRoomString);

        for(int i =0; i < parentDoor.transform.childCount; i++)
        {
            GameObject door = parentDoor.transform.GetChild(i).gameObject;
            if (door.GetComponent<Door>().doorName == "A")
            {
                speciallyRoom.transform.Find("Zones").Find("A").gameObject.SetActive(door.activeSelf);
            }
            if (door.GetComponent<Door>().doorName == "B")
            {
                speciallyRoom.transform.Find("Zones").Find("B").gameObject.SetActive(door.activeSelf);
            }
            if (door.GetComponent<Door>().doorName == "C")
            {
                speciallyRoom.transform.Find("Zones").Find("C").gameObject.SetActive(door.activeSelf);
            }
            if (door.GetComponent<Door>().doorName == "D")
            {
                speciallyRoom.transform.Find("Zones").Find("D").gameObject.SetActive(door.activeSelf);
            }
            if (door.GetComponent<Door>().doorName == "E")
            {
                speciallyRoom.transform.Find("Zones").Find("E").gameObject.SetActive(door.activeSelf);
            }
            if (door.GetComponent<Door>().doorName == "F")
            {
                speciallyRoom.transform.Find("Zones").Find("F").gameObject.SetActive(door.activeSelf);
            }
        }
    }

    public void DisplaySupportTorch(bool display)
    {
        if (display && gameManager.game.currentRoom.explorationIsUsed)
            return;
        supportTorch.SetActive(display);
    }

    public void DisplayAllDoorLight(bool display)
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach(GameObject door in doors)
        {
            door.transform.Find("Light").gameObject.SetActive(display);
            door.transform.Find("LightDoor").gameObject.SetActive(display);
        }
    }

    public void DisplayAllZoneDoorInNormalRoom(bool display)
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            for(int i = 1; i < door.transform.Find("Zones").childCount; i++)
            {
                door.transform.Find("Zones").GetChild(i).gameObject.SetActive(display);
            }
        }
    }

    public void DisplayAllZoneInSpeciallyRoomExceptVirusRoom(bool display)
    {
        GameObject SpecialRoomParent = GameObject.Find("Room").transform.Find("Special").gameObject;

        for(int i =0; i < SpecialRoomParent.transform.childCount; i++)
        {
            if (SpecialRoomParent.transform.GetChild(i).name == "AwardObject")
                continue;
            if (!SpecialRoomParent.transform.GetChild(i).gameObject.activeSelf)
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "VirusRoom" || SpecialRoomParent.transform.GetChild(i).name == "LabyrinthHideRoom")
                continue;

            if (SpecialRoomParent.transform.GetChild(i).name == "PurificationRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "ResurectionRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "PrayRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "ImpostorRoom")
                continue;


            if (SpecialRoomParent.transform.GetChild(i).name == "ChestRoom" || SpecialRoomParent.transform.GetChild(i).name == "NPCRoom")
            {
                for (int n = 0; n < SpecialRoomParent.transform.GetChild(i).Find("Zones").childCount; n++)
                {
                    SpecialRoomParent.transform.GetChild(i).Find("Zones").GetChild(n).gameObject.SetActive(display);
                }
            }
            else
            {
                GameObject leftZones = SpecialRoomParent.transform.GetChild(i).Find("DoorZone").Find("Left").gameObject;
                for (int j = 0; j < leftZones.transform.childCount; j++)
                {
                    leftZones.transform.GetChild(j).gameObject.SetActive(display);
                }

                GameObject RightZones = SpecialRoomParent.transform.GetChild(i).Find("DoorZone").Find("Right").gameObject;
                for (int h = 0; h < RightZones.transform.childCount; h++)
                {
                    RightZones.transform.GetChild(h).gameObject.SetActive(display);
                }

            }
        }
    }

    public bool IsOnlyVirusRoom()
    {
        GameObject SpecialRoomParent = GameObject.Find("Room").transform.Find("Special").gameObject;
        for (int i = 0; i < SpecialRoomParent.transform.childCount; i++)
        {
            if (SpecialRoomParent.transform.GetChild(i).name == "AwardObject")
                continue;
            if (!SpecialRoomParent.transform.GetChild(i).gameObject.activeSelf)
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "VirusRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "ChestRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "PurificationRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "NPCRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "ResurectionRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "PrayRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "ImpostorRoom")
                continue;
            if (SpecialRoomParent.transform.GetChild(i).name == "SacrificeRoom")
                continue;
            return false;
        }
        return true;
    }



    public void SetColorWallPaper()
    {
        blueWallPaper.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 255f);
    }
    
    public void DisplaySpecificDoorInSpeciallyRoom(GameObject speciality)
    {
        ResetSpecificDoorInSpeciallyRoom(speciality);
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            for(int i =0; i < speciality.transform.Find("Doors").childCount; i++)
            {


                if (speciality.transform.Find("Doors").GetChild(i).name == door.GetComponent<Door>().doorName)
                {
                    speciality.transform.Find("Doors").GetChild(i).gameObject.SetActive(true);
                }
    
              
            }

        }
    }

    public void ResetSpecificDoorInSpeciallyRoom(GameObject speciality)
    {
        for (int i = 0; i < speciality.transform.Find("Doors").childCount; i++)
        {
            speciality.transform.Find("Doors").GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DisplayChests(bool display)
    {
        chests.gameObject.SetActive(display);
    }

    public void DisplayNPCs(bool display)
    {
        GameObject[] listNpc = GameObject.FindGameObjectsWithTag("NPC");

        foreach(GameObject npc in listNpc)
        {
            npc.SetActive(display);
        }
    }

    public void DisplayFloorVirusTransparency()
    {
        virusRoomFloor.transform.Find("Floor").GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);

       
        for(int i =0; i < virusRoomFloor.transform.Find("Doors").childCount; i++)
        {
            virusRoomFloor.transform.Find("Doors").GetChild(i).GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
        }

        if(gameManager.game.currentRoom.chest || gameManager.game.currentRoom.isNPC)
        {
            virusRoomFloor.transform.Find("DoubleZone_circle").GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
            for (int i = 0; i < virusRoomFloor.transform.Find("Zones").childCount; i++)
            {
                virusRoomFloor.transform.Find("Zones").GetChild(i).GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
            }
        }
        else
        {
            virusRoomFloor.transform.Find("Circle").GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Left").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Left").GetChild(i).GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
            }
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Right").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Right").GetChild(i).GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(2f);
            }
        }
        
        gameManager.game.currentRoom.virus_spawned = true;
        DisplayAllZoneInSpeciallyRoomExceptVirusRoom(false);
        soundDemonicLaugh.Play();
    }

    public void DesactivateTransparencyFloor()
    {
        virusRoomFloor.transform.Find("Floor").GetComponent<Display_Tranparency>().DesactivateTransition();

        for (int i = 0; i < virusRoomFloor.transform.Find("Doors").childCount; i++)
        {
            virusRoomFloor.transform.Find("Doors").GetChild(i).GetComponent<Display_Tranparency>().DesactivateTransition();

        }


        if (gameManager.game.currentRoom.chest || gameManager.game.currentRoom.isNPC)
        {
            virusRoomFloor.transform.Find("DoubleZone_circle").GetComponent<Display_Tranparency>().DesactivateTransition();
            for (int i = 0; i < virusRoomFloor.transform.Find("Zones").childCount; i++)
            {
                virusRoomFloor.transform.Find("Zones").GetChild(i).GetComponent<Display_Tranparency>().DesactivateTransition();
            }
        }
        else
        {
            virusRoomFloor.transform.Find("Circle").GetComponent<Display_Tranparency>().DesactivateTransition();
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Left").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Left").GetChild(i).GetComponent<Display_Tranparency>().DesactivateTransition();
            }
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Right").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Right").GetChild(i).GetComponent<Display_Tranparency>().DesactivateTransition();
            }
        }
    }
    public void DisplayVirusFloorWithoutTransition()
    {
        virusRoomFloor.transform.Find("Floor").GetComponent<Display_Tranparency>().DisplayWithoutTransition();


        for (int i = 0; i < virusRoomFloor.transform.Find("Doors").childCount; i++)
        {
            virusRoomFloor.transform.Find("Doors").GetChild(i).GetComponent<Display_Tranparency>().DisplayWithoutTransition();

        }


        if (gameManager.game.currentRoom.chest || gameManager.game.currentRoom.isNPC)
        {
            virusRoomFloor.transform.Find("DoubleZone_circle").GetComponent<Display_Tranparency>().DisplayWithoutTransition();
            for (int i = 0; i < virusRoomFloor.transform.Find("Zones").childCount; i++)
            {
                virusRoomFloor.transform.Find("Zones").GetChild(i).GetComponent<Display_Tranparency>().DisplayWithoutTransition();
            }
        }
        else
        {
            virusRoomFloor.transform.Find("Circle").GetComponent<Display_Tranparency>().DisplayWithoutTransition();
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Left").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Left").GetChild(i).GetComponent<Display_Tranparency>().DisplayWithoutTransition();
            }
            for (int i = 0; i < virusRoomFloor.transform.Find("DoorZone").Find("Right").childCount; i++)
            {
                virusRoomFloor.transform.Find("DoorZone").Find("Right").GetChild(i).GetComponent<Display_Tranparency>().DisplayWithoutTransition();
            }
        }

    }

    public void DisplayFoggyRoomTransparyAnimation()
    {
        if (gameManager.game.currentRoom.animationFoogyAlreayHapped)
            return;

        GameObject foggyRoom = GameObject.Find("Room").transform.Find("Special").Find("FoggyRoom").gameObject;
        foggyRoom.transform.Find("FoggyAnimation").GetComponent<Display_Tranparency>().LaunchTransitionUnDisplayToDisplay(6f, 0.3f);
        soundDemonicLaugh.Play();
        gameManager.game.currentRoom.animationFoogyAlreayHapped = true;
    }

    public void DisplayCircleZoneColorChest(bool display)
    {
        GameObject chestRoom = GameObject.Find("Room").transform.Find("Special").Find("ChestRoom").gameObject;

        chestRoom.transform.Find("ColorCircleZone").gameObject.SetActive(display);

        if (!display)
        {
            chestRoom.transform.Find("ColorCircleZone").transform.Find("CircleZoneLeft").Find("Light").gameObject.SetActive(false);
            chestRoom.transform.Find("ColorCircleZone").transform.Find("CircleZoneRight").Find("Light").gameObject.SetActive(false);
        }
    }

    public void StopAllMusic()
    {
        BasesMusic.Stop();
        BasesMusic2.Stop();
        BasesMusic3.Stop();
        CaveMusicAmbiance.Stop();
    }
}




