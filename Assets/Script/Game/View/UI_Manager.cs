using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Luminosity.IO;


public class UI_Manager : MonoBehaviour
{

    public GameManager gameManager;
    public GameObject map;

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

        if (gameManager.voteDoorHasProposed && gameManager.timer.timerLaunch && gameManager.game.currentRoom.GetIsVirus() )
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

    public void SetDistanceRoom(int distance, RoomHex room)
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
                door.GetComponent<Door>().barricade = active ;
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

        zones_X.SetActive(true);

        zones_X.GetComponent<Animator>().SetBool("zone_x", true);

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
        yield return new WaitForSeconds(0.5f);
        DisplayTutorial();
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
    public void DisabledWallBehindParadiseDoor()
    {
        wallEight.SetActive(false);
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
    
    public void DisplayBlackScreenToDemonWhenAllGone()
    {
        DisplayBlackScreen(true, true);
        StartCoroutine(CoroutineWaitToTransition());
    }

    public IEnumerator CoroutineWaitToTransition()
    {
        yield return new WaitForSeconds(2);
/*        blackWallPaper.transform.GetChild(0).gameObject.SetActive(true);
        blackWallPaper.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);*/
        resumePanel.SetActive(true);
        map.transform.parent = map_resumePanel.transform;
        map.SetActive(true);
        map.transform.localScale = new Vector3(70, 70, 0);
        map.transform.position = new Vector2(-10, 10);
        map.GetComponent<SpriteMask>().enabled = false;
        DesactiveUIInhexagone();
        ResumeImpostor();
        ResumeDataKeyAndTorch();
        //PhotonNetwork.LeaveRoom();
    }

    public void DisplayBlackScreenToNoneImpostor()
    {
        DisplayBlackScreen(true, true);
        StartCoroutine(CoroutineWaitToTransition2());
    }

    public IEnumerator CoroutineWaitToTransition2()
    {
        yield return new WaitForSeconds(3f);
        /*        blackWallPaper.transform.GetChild(0).gameObject.SetActive(true);
                blackWallPaper.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);*/
        resumePanel.SetActive(true);
        map.transform.parent = map_resumePanel.transform;
        map.SetActive(true);
        map.transform.localScale = new Vector3(70, 70, 0);
        map.transform.position = new Vector2(-2, 8);
        map.GetComponent<SpriteMask>().enabled = false;
        DesactiveUIInhexagone();
        ResumeImpostor();
        ResumeDataKeyAndTorch();
        //PhotonNetwork.LeaveRoom();

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
            resumePanel.transform.GetChild(0).gameObject.SetActive(false);
            resumePanel.transform.GetChild(1).gameObject.SetActive(false);
            resumePanel.transform.GetChild(3).gameObject.SetActive(true);
            resumePanel.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = listImpostorsName[0];

        }
        else
        {
            if (gameManager.numberPlayer > 6)
            {
                resumePanel.transform.GetChild(2).gameObject.SetActive(true);
                for (int i = 0; i < 3; i++)
                {
                    resumePanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = listImpostorsName[i];
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    resumePanel.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = listImpostorsName[i];
                }
            }
        }
    }

    public void  ResumeDataKeyAndTorch()
    {
        resumePanel.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = gameManager.game.key_counter.ToString();
        resumePanel.transform.GetChild(7).GetChild(1).GetComponent<Text>().text = gameManager.nbKeyBroken.ToString();
        resumePanel.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = gameManager.game.nbTorch.ToString();
        resumePanel.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = gameManager.nbTorchOff.ToString();

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
        foreach (GameObject room in gameManager.dungeon)
        {
            ShowDataMapInOneRoom(room);
        }
    }

    public void ShowDataMapInOneRoom(GameObject room)
    {

        //room.GetComponent<Hexagone>().distanceText.text = room.GetComponent<Hexagone>().distance_pathFinding.ToString();
        if (room.GetComponent<Hexagone>().isObstacle)
        {
                room.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                room.GetComponent<Hexagone>().distanceText.text = "";
                room.GetComponent<Hexagone>().index_text.text = "";
        }
        if (room.GetComponent<Hexagone>().isInitialeRoom)
        {
            room.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            room.GetComponent<Hexagone>().isTraversed = true;
        }
        if (room.GetComponent<Hexagone>().hasKey)
        {
            room.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            room.GetComponent<Hexagone>().distanceText.text = "";
            room.GetComponent<Hexagone>().index_text.text = "";
        }
        if (room.GetComponent<Hexagone>().isFoggy)
        {
            room.GetComponent<SpriteRenderer>().color = new Color(87 / 255f, 89 / 255f, 96 / 255f);
        }
        if (room.GetComponent<Hexagone>().isVirus )
        {
            room.GetComponent<SpriteRenderer>().color = new Color(66 / 255f, 0 / 255f, 117 / 255f);
        }
        if (room.GetComponent<Hexagone>().isExit)
        {
            room.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
            room.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);

            if (room.GetComponent<Hexagone>().hasKey)
            {
                room.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                room.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            }
        }
        if (room.GetComponent<Hexagone>().isTraversed)
        {
            room.GetComponent<SpriteRenderer>().color = new Color((float)(16f / 255f), (float)78f / 255f, (float)29f / 255f, 1);
        }
        if (gameManager.hell)
        {
            if (room.GetComponent<Hexagone>().pos_X == gameManager.hell.GetPos_X() && room.GetComponent<Hexagone>().pos_Y == gameManager.hell.GetPos_Y())
            {
                room.GetComponent<SpriteRenderer>().color = new Color((float)(255 / 255f), (float)0f / 255f, (float)0f / 255f, 1);
                room.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);

                if (gameManager.hell.hasKey)
                {
                    room.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    room.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
                }

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
    public void OpenParadise()
    {
        gameManager.GetPlayerMineGO().transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
        OpenDoorParadiseAnimation();
        DisabledWallBehindParadiseDoor();
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

        player.GetComponent<PlayerGO>().SetTextChat(player.GetComponent<PlayerGO>().chatPanel.transform.GetChild(1).GetComponent<InputField>().text);
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

}




