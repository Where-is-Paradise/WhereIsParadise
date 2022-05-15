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

    public Text nbKeyText;
    public GameObject zonesVote;
    public GameObject zones_X;

    public GameObject blackWallPaper;
    public GameObject blueWallPaper;
    public GameObject whiteWallPaper;
    public GameObject LoadPage;
    public GameObject roleInformation;

    private float durationTransitionBlackScreen = 0.7f;
    private float t = 0;
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
    public Text torch_number;

    private bool key_broken_animation;
    private bool addKeyAnimation;

    public GameObject x_zone_red;
    

    public GameObject identification_expedition;
    public GameObject identification_voteDoor;

    public GameObject resumePanel;
    public GameObject map_resumePanel;


    public Text textChatMessage;

    public AudioSource soundChrono;
    public AudioSource soundDemonicLaugh;
    public AudioSource soundAmbianceHell;
    public AudioSource soundAmbianceParadise;

    public GameObject panelTutoriel;

    public GameObject ui_button;
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

    // Start is called before the first frame update
    void Start()
    {

#if !UNITY_IOS && !UNITY_ANDROID
        ui_button.SetActive(false);



#else

OutsideMap.SetActive(false);
setting_button_echapMenu.SetActive(false);

#endif

        for(int i =0;  i< tutorial.Count; i++)
        {
            listTutorialBool.Add(false);
        }
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
            TransitionToBlack(t, blackWallPaper);
            if (t < 1 )
            {
                t += (Time.deltaTime / durationTransitionBlackScreen);
            }
        }

        if (whiteWallPaper.activeSelf)
        {
            TransitionToBlack(t, whiteWallPaper);
            if (t < 1)
            {
                t += (Time.deltaTime / durationTransitionBlackScreen);
            }
        }

        if (key_broken_animation)
        {
            AnimationBrokenKey();
        }
        if (addKeyAnimation)
        {
            AniamtionAddKey();
        }

        EchapButton_unblockPlayer.interactable = !gameManager.timer.timerLaunch;

    }


    public void DisplayMap()
    {
        if (blueWallPaper.activeSelf)
        {
            Camera.main.orthographicSize = 5;

        }
        if (!echap_menu.activeSelf)
        {
            map.SetActive(!map.activeSelf);
            blueWallPaper.SetActive(!blueWallPaper.activeSelf);

        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = !map.activeSelf;

        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            DisplayPowerButton(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower, map.activeSelf);
    }

    public void DisplayTimerInMap(bool active)
    {
        blueWallPaper.transform.Find("Canvas").Find("Back").GetComponent<Button>().interactable = !active;
        //blueWallPaper.transform.Find("Canvas").Find("TimerText").GetComponent<Text>().text = gameManager.timer.timeLeft.ToString();
        //waitingPage_PowerImpostor.transform.Find("timer").GetComponent<Text>().text = gameManager.timer.timeLeft.ToString();
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
    public void HideDistanceRoom()
    {
        text_distance_room.GetComponent<Text>().text = "";
    }

    public void DisplayInterrogationPoint()
    {
        text_distance_room.GetComponent<Text>().text = "?";
    }





    public void DisplayObstacleInDoor(int indexDoor , bool active)
    {
        GameObject[] listDoor = GameObject.FindGameObjectsWithTag("Door");
        
        foreach(GameObject door in listDoor)
        {
            if(door.GetComponent<Door>().index  == indexDoor)
            {
                door.transform.GetChild(0).GetChild(1).gameObject.SetActive(active);
                door.GetComponent<Door>().barricade = active;
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
            zones_X.SetActive(true);
            zones_X.GetComponent<Animator>().SetBool("zone_x", true);
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
        zones_X.GetComponent<Animator>().SetBool("zone_x", false);
    }

    public void ActiveXzone(bool active)
    {
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
        t = 0;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = !display;

        if (isBlack)
        {
            blackWallPaper.SetActive(display);
            blackWallPaper.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
        else
        {
            whiteWallPaper.SetActive(display);
            whiteWallPaper.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
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
        DisplayPowerImpostor(true);
        gameManager.timer.LaunchTimer(30, false);
        yield return new WaitForSeconds(30);
        DisplayPowerImpostor(false);
        yield return new WaitForSeconds(0.5f);
        DisplayTutorial();
    }


    public void DisplayPowerImpostor(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            waitingPage_PowerImpostor.SetActive(display);
        else
        {
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

        string letter = random switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            3 => "D",
            4 => "E",
            5 => "F",
            _ => "X",
        };

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            door.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            door.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = letter;
            door.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }


    }
    public void ResetLetterDoor()
    {
        GameObject[] doors = gameManager.TreeDoorById();

        foreach (GameObject door in doors)
        {
            int indexDoor = door.GetComponent<Door>().index;
            string letter = indexDoor switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                4 => "E",
                5 => "F",
                _ => "X",
            };
            door.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            door.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = letter;
            door.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
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


    public void LaunchAnimationAddKey()
    {
        addKeyAnimation = true;
        addKey.SetActive(true);
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
    
    public void ResetAllPlayerLightAround()
    {
        foreach(GameObject player  in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.transform.GetChild(1).GetChild(9).gameObject.SetActive(false);
            player.transform.GetChild(1).GetChild(6).gameObject.SetActive(false);
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
        List<string> listImpostorsName = gameManager.listNamePlayerImpostor;
        if (gameManager.numberPlayer < 5)
        {
            resumePanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            resumePanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            resumePanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            resumePanel.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = listImpostorsName[0];

        }
        else
        {
            if (gameManager.numberPlayer > 6)
            {
                resumePanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                for (int i = 0; i < 3; i++)
                {
                    resumePanel.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = listImpostorsName[i];
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    resumePanel.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = listImpostorsName[i];
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

    public void DesactivateLightAroundPlayers()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in listPlayer)
        {
            player.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
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
                player.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        }
    }
    public void ShowAllDataInMap()
    {
        foreach (Hexagone hexagone in gameManager.dungeon)
        {
            ShowDataMapInOneRoom(hexagone);
        }
    }

    public void ShowDataMapInOneRoom(Hexagone hexagone)
    {
        Room room = hexagone.Room;
        if (room.IsObstacle)
        {
                hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                hexagone.GetComponent<Hexagone>().distanceText.text = "";
                hexagone.GetComponent<Hexagone>().index_text.text = "";
        }
        if (room.IsInitiale)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            room.IsTraversed = true;
        }
        if (room.IsExit)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            hexagone.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);

        }
        if (room.IsTraversed)
        {
            hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
        }
        if (room.isOldParadise)
        {
            hexagone.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(true);
            hexagone.transform.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
        }
        if (gameManager.hell)
        {
            if (room.X == gameManager.hell.X && room.Y == gameManager.hell.Y)
            {
                hexagone.GetComponent<SpriteRenderer>().color = new Color((float)(255 / 255f), (float)0f / 255f, (float)0f / 255f, 1);
                hexagone.transform.Find("Canvas").Find("Hell").gameObject.SetActive(true);

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
                    for(int i = 0; i < 6; i++)
                    {
                        door.transform.GetChild(5).GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
                    }
                }
                door.transform.GetChild(5).gameObject.SetActive(display);
                door.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<Text>().text = playerName;

                if(display)
                    door.transform.GetChild(5).GetChild(1).GetChild(1).GetChild(player.indexSkin).gameObject.SetActive(display);
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
            for (int i = 0; i < 6; i++)
            {
                door.transform.GetChild(5).gameObject.SetActive(false);
                door.transform.GetChild(5).GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void DisplayChestRoom(bool display)
    {
        if (display)
        {
            if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            {
                return;
            }
            if (gameManager.game.currentRoom.IsExit)
            {
                return;
            }
            if (gameManager.game.currentRoom.IsHell)
            {
                return;
            }
            if (gameManager.game.key_counter == 0)
            {
                return;
            }
        }
        MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display,0);
        DisplayAwardAndPenaltyForImpostor(display);
        if (!display)
        {
            ResetChestRoom();
        }
    }
    public void DisplayAwardAndPenaltyForImpostor(bool display)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            return;
        }
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject;
        foreach (Chest chest in gameManager.game.currentRoom.chestList)
        {
            if (chest.isAward)
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
                GameObject chestRoomPenalty = chestRoom.transform.GetChild(chest.index).Find("Penalty").gameObject;
                chestRoomPenalty.SetActive(display);
                chestRoomPenalty.transform.GetChild(chest.indexAward).gameObject.SetActive(display);
                if (display)
                {
                    chestRoomPenalty.transform.GetChild(chest.indexAward).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
                    chestRoomPenalty.transform.GetChild(chest.indexAward).Find("Canvas").transform.Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 0.4f);
                }
                else
                {
                    chestRoomPenalty.transform.GetChild(chest.indexAward).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
                    chestRoomPenalty.transform.GetChild(chest.indexAward).Find("Canvas").transform.Find("LessOne").GetComponent<Text>().color = new Color(255, 255, 255, 1f);
                }
            }
            
           
        }

        SetDistanceTextAwardChest(0);
        SetDistanceTextAwardChest(1);
    }

    public void ResetChestRoom()
    {
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject;
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
        DisplaySpeciallyLevers(display,1);
    }
    public void DisplaySacrificeRoom(bool display)
    {
        MainRoomGraphic.transform.Find("Special").transform.Find("SacrificeRoom").gameObject.SetActive(display);
        DisplaySpeciallyLevers(display,2);
    }

    
    public void DisplaySpeciallyLevers(bool display, int indexSpecially)
    {
        for (int i = 0; i < MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").Find("Specially").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").Find("Specially").GetChild(i).gameObject.SetActive(false);
        }
        MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").Find("Specially").GetChild(indexSpecially).gameObject.SetActive(display);
        MainRoomGraphic.transform.Find("Levers").transform.Find("SpeciallyRoom_lever").gameObject.SetActive(display);
    }

    public void DisplayMainLevers(bool display)
    {
        DisplayLeverExploration(display);
        DisplayLeverVoteDoor(display);
       
    }

    public void DisplayLeverExploration(bool display)
    {
        MainRoomGraphic.transform.Find("Levers").transform.Find("Exploration_lever").gameObject.SetActive(display);
    }
    public void DisplayLeverVoteDoor(bool display)
    {
        MainRoomGraphic.transform.Find("Levers").transform.Find("OpenDoor_lever").gameObject.SetActive(display);
    }


    public void SetDistanceTextAwardChest(int indexChest)
    {
        GameObject chestRoom = MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").gameObject;

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
        for (int i = 0; i < MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").childCount; i++)
        {
            MainRoomGraphic.transform.Find("Special").transform.Find("ChestRoom").GetChild(i).Find("VoteZone").gameObject.SetActive(display);
        }
    }

    public void DisplayTutorial()
    {
        if (gameManager.setting.displayTutorial)
        {
            panelTutoriel.SetActive(true);
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                panelTutoriel.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                panelTutoriel.transform.GetChild(0).gameObject.SetActive(true);
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
        gameManager.GetPlayerMineGO().transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
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
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(true);
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
    public void DisplayVirusRoom(bool display)
    {
        GameObject.Find("Special").transform.Find("VirusRoom").gameObject.SetActive(display);
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
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter = !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter;
    }
}




