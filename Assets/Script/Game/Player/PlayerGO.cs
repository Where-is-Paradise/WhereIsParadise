using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Luminosity.IO;
using UnityEngine.Networking;
using Steamworks;

public class PlayerGO : MonoBehaviour
{

    public float movementlControlSpeed = 1;
    public float decreaseSpeed = 1;
    public string playerName;
    public bool isMovingAutomaticaly = true;
    public Vector3 oldPosition;
    private PlayerNetwork playerNetwork;

    public bool isBoss = false;
    public bool isImpostor;

    public bool isReady = false;
    public GameManager gameManager;

    /// Vote for exploration
    public int vote_cp = 0;
    public bool isInExpedition = false;
    public bool haveToGoToExpedition = false;
    public bool hasVoteVD = false;
    public bool isGoInExpeditionOneTime = false;

    public int position_X;
    public int position_Y;

    public bool collisionToGost = false;
    public bool canMove = true;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 oldPointB;
    private bool touchStart = false;

    public bool launchVoteDoorMobile = false;
    private bool launchExpeditionMobile = false;
    public bool launchExpeditionWithAnimation = false;
    private float timeStayTouch_voteDoor = 2f;
    private float timeStayTouch_expediton = 4f;
    private float timeStayTouch = 0;

    private bool takeDoor = false;
    public bool isCollisionInDoorTakeDoor = false;
    public bool isCollisionInDoorBackExpedition = false;
    public bool isCollisionInDoorExpedition = false;
    public bool collisionParadise = false;
    public bool collisionHell = false;

    public bool isMoving = false;

    private bool takeDoorBackExpedition = false;
    private bool takeDoorExpededition = false;
    public GameObject doorCollision = null;

    public bool animateEyes = false;
    public bool comeToParadise = false;
    public int indexSkin = 0;
    public int indexSkinColor = 0;
    public bool ui_isOpen = false;

    public bool isChooseForExpedition = false;
    public bool isAlreadyHide = false;

    public bool displayChatInput = false;

    public bool displayMessage = false;
    private List<string> currentlyMessageDisplay;

    public ResolutionManagement resolution;

    public int collisionInDoorIndex = -1;
    public int collsionDoorIndexForExploration = -1;

    public bool firstAtDoorToExploration = false;
    public bool hellIsFind = false;
    public bool paradiseIsFind = false;

    public float timerBoss = 110;
    public bool timerBossLaunch = false;
    public bool isAlreadyTimerBoss = false;

    public float additionalSizeTextToMobile = 0.05f;

    public bool quitTutorialN7 = true;
    public bool wantToChangeBoss = false;
    public bool changeBoss = false;

    public bool isTouchByFireBall = false;
    public int rankTouchBall = 0;
    public bool hasWinFireBallRoom = false;

    public int nbVoteSacrifice = 0;
    public bool hasVoteSacrifice = false;
    public bool isSacrifice = false;
    public int lastPlayerIndexVote = -1;
    public bool damoclesSwordIsAbove = false;
    public int lifeTrialRoom = 2;

    public GameObject chatPanel;

    public bool canLaunchExplorationLever = false;
    public bool canLaunchDoorVoteLever = false;
    public bool canLaunchSpeciallyRoomPower = false;
    public bool canLaunchChangeBoss = false;
    public bool canDisplayMap = false;
    public bool displayMap = false;
    public bool canDisplayTutorial = false;
    public bool displayTutorial = false;

    public bool explorationPowerIsAvailable = true;

    public bool isInJail = false;

    public int indexPower = -1;
    public int indexObjectPower = -1;

    public bool hasClickInPowerImposter = false;
    public bool hasValidPowerImposter = false;

    public bool hideImpostorInformation = false;
    public Vector3 movement = new Vector3(0,0,0);
    public bool isTouchByDeath = false;
    public bool isDeadBySwordDamocles = false;
    public bool isTouchByAx = false;
    public bool isTouchBySword = false;
    public bool isTouchByMonster = false;

    public bool isTouchInTrial = false;

    public bool isCursed = false;
    public bool isBlind = false;

    public bool lightHexagoneIsOn = false;

    public int distanceCursed = 0;
    public Room roomUsedWhenCursed;

    public bool isInvincible = false;
    public bool isInvisible = false;

    public bool isAlreaySerCanLauchLeverExplorationCouroutine = false;

    private float oldHorizontal;
    private float oldVertical;

    public bool animationDeath = false;
    public bool animationDeathUpFinish = false;
    public bool animationDeathDownFinis = false;
    public float old_y_position;


    public bool positionSended = false;
    public float x_sended = 0;
    public float y_sended = 0;
    // is for recon


    public bool hasMap = false;
    public bool hasProtection = false;
    public bool hasTrueEyes = false;
    public bool hasBlackTorch = false;

    public List<bool> listTrialObject;

    public bool isLeftNpc = false;
    public int indexNpc = 0;
    public bool hasImpostorObject = false;
    public bool hasOneTrapPower = false;

    public bool canCollisionToDoorExploration = true;


    public List<int> Inventory = new List<int>();

    public bool hasCurrentTorch = false;

    [HideInInspector]
    public int blackSoul_money = 900; 

    private void Awake()
    {
        displayChatInput = false;
        playerNetwork = gameObject.GetComponent<PlayerNetwork>();
        currentlyMessageDisplay = new List<string>();
        if(!GameObject.Find("GameManager"))
            isMovingAutomaticaly = true;
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        setIsBoss();

        //AnimateEyes();

        setCollider();

        // Used to prevent multi tap on mobile

        enhanceOwners();
        /*        if(this.GetComponent<PhotonView>().IsMine)
                    StartCoroutine(SendPositionCoroutine());*/

        listTrialObject.Add(hasMap);
        listTrialObject.Add(hasProtection);
        listTrialObject.Add(hasTrueEyes);
        //StartCoroutine(GetListSkinIndexInServer());
        StartCoroutine(GetListSkinIndexInServerTestIP());
    }

    private void enhanceOwners()
    {
        List<string> owners = new List<string> { "Homertimes   ", "Anis   ", "Onestla   " };

        if (owners.Contains(playerName))
        {
            transform.localScale = new Vector2(0.85f, 0.85f);
/*            this.transform.GetChild(1).GetChild(5).gameObject.SetActive(true);
            this.transform.GetChild(1).GetChild(7).gameObject.SetActive(true);*/
        }
    }
    private bool setIsBoss()
    {
        return isBoss = PhotonNetwork.IsMasterClient && GetComponent<PhotonView>().IsMine;
    }

    private void setCollider()
    {
        
        if (!GetComponent<PhotonView>().IsMine)
        {
            GetComponent<CapsuleCollider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void FixedUpdate()
    {
        if (animateEyes)
        {
            //this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").GetComponent<Animator>().SetBool("OpenEyes", true); ;
            animateEyes = false;
        }
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerName;
        if (isMovingAutomaticaly && GetComponent<PhotonView>().IsMine)
        {
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            this.transform.position += new Vector3(-3f * Time.deltaTime, 0, 0);
            if (this.transform.position.x < 5)
            {
                isMovingAutomaticaly = false;
            }
            return;
        }
        else
        {
            isMovingAutomaticaly = false;
            this.GetComponent<CapsuleCollider2D>().enabled = true;

        } 
            // prevent update if chat input is displayed
        if (displayChatInput)
        {
            return;
        }
        if (GetComponent<PhotonView>().IsMine && canMove)
        {
            handlePlayerMove();
        }

        ChangeSystemSyncPostionToTrial();
        Dash();
        DashIsAvailable();
        
    }


    void Update()
    {

        //hasMap = true;

        if (isMovingAutomaticaly)
        {
            return;
        }
        TurnChat();

        if (!gameManager)
        {
            if (displayMessage)
            {
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            if (displayMessage && !gameManager.game.currentRoom.IsFoggy)
            {
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }
        if (GameObject.Find("UI_Management"))
        {
            chatPanel = GameObject.Find("UI_Management").GetComponent<UI_Managment>().chatPanelInputParent;
        }
        else
        {
            if (GameObject.Find("UI_Manager"))
            {
                chatPanel = GameObject.Find("UI_Manager").GetComponent<UI_Manager>().chatPanelInputParent;
            }

        }


        if (GameObject.Find("SquareResolution"))
            resolution = GameObject.Find("SquareResolution").GetComponent<ResolutionManagement>();
        
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            /*            if(!gameManager.isActuallySpecialityTime)
                            SetSkinBoss(isBoss);*/
            if (!isSacrifice)
            {
                if (!hideImpostorInformation)
                    SetSkinImpostor(isImpostor);
                else
                    SetSkinImpostor(false);
            }
          

            if (gameManager.timer.timerFinish && !gameManager.alreadyPass)
            {
                this.transform.Find("Skins").GetChild(indexSkin).Find("Light_around").gameObject.SetActive(false);
                gameManager.alreadyPass = true;
                StartCoroutine(CoroutineIsChooseForExpedition());

            }
        }


        if (gameManager)
        {
            if (!explorationPowerIsAvailable && (gameManager.speciallyIsLaunch || gameManager.timer.timerLaunch))
            {
                this.transform.Find("TorchBarre").gameObject.SetActive(false);
            }
            else
            {
                if (!explorationPowerIsAvailable && gameManager.SamePositionAtMine(this.GetComponent<PhotonView>().ViewID) && !this.isSacrifice)
                {
                    this.transform.Find("TorchBarre").gameObject.SetActive(true);
                }
                else
                {
                    this.transform.Find("TorchBarre").gameObject.SetActive(false);

                }
            }
        }

        if (GetComponent<PhotonView>().IsMine && (isBoss || hasWinFireBallRoom) && gameManager)
        {
            if (!gameManager.expeditionHasproposed && !gameManager.voteDoorHasProposed)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                    InputExplorationAnimation();
            }
            
            if (launchExpeditionWithAnimation || gameManager.launchExpedtion_inputButton)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                {
                   
                    if (!gameManager.alreaydyExpeditionHadPropose && !gameManager.DoorParadiseOrHellisOpen)
                    {
                        Dictionary<int, int> door_idPlayer = gameManager.SetPlayerNearOfDoor();

                        if (gameManager.VerificationExpedition(door_idPlayer))
                        {
                            gameManager.ProposeExpedition(door_idPlayer);
                            gameManager.ui_Manager.DisplayMainLevers(false);
                            gameManager.gameManagerNetwork.SendDisplayMainLevers(false);

                        }
                        else
                        {
                            gameManager.ui_Manager.DisplayXzoneRed();
                            Debug.LogError("VerificationExpedition error");
                        }
                    }
                    else
                    {
                        gameManager.ui_Manager.DisplayXzoneRed();
                        Debug.LogError("alreaydyExpeditionHadPropose error or  gameManager.DoorParadiseOrHellisOpen");
                    }
                }
                launchExpeditionWithAnimation = false;
            }
            if (!gameManager.voteDoorHasProposed)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                {
                    InputVoteDoorAnimation();
                    LaunchRoomSpeciallyPower();
                }  
            }
            // Input Vote  Door
            if (launchVoteDoorMobile || gameManager.launchVote_inputButton)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                {
                    if (!gameManager.timer.timerLaunch && gameManager.game.key_counter > 0 && !gameManager.DoorParadiseOrHellisOpen && gameManager.GetDoorAvailable().Count > 0)
                    {
                        if (!gameManager.OnePlayerHaveToGoToExpedition())
                        {
                            gameManager.ActiveZoneDoor();
                       

                        }
                        else
                        {
                            gameManager.ui_Manager.DisplayXzoneRed();
                        }

                    }
                    else
                    {
                        gameManager.ui_Manager.DisplayXzoneRed();
                    }
                }
                launchVoteDoorMobile = false;
            }
        }

        if (gameManager)
        {

            if ((InputManager.GetButtonDown("Enter") || Input.GetKeyDown(KeyCode.KeypadEnter)) && GetComponent<PhotonView>().IsMine && !gameManager.ui_Manager.map.activeSelf)
            {
                if (!this.isSacrifice && gameManager.gameIsReady)
                    OnClickChat();
            }
            
        }
        else
        {
            if ((InputManager.GetButtonDown("Enter") || Input.GetKeyDown(KeyCode.KeypadEnter)) && GetComponent<PhotonView>().IsMine)
            {
                if (!this.isSacrifice)
                    OnClickChat();
            }
        }
        

        if (gameManager)
        {
            if ((displayMap || ( isImpostor && InputManager.GetButtonDown("Map"))) && GetComponent<PhotonView>().IsMine && !displayChatInput)
            {
                if (gameManager.setting.DISPLAY_MINI_MAP || gameManager.hellIsFind || gameManager.paradiseIsFind)
                {
                    if (isImpostor)
                    {
                        gameManager.ui_Manager.DisplayMapImpostor();
                    }
                    else
                    {
                        if (hasMap)
                            gameManager.ui_Manager.DisplayMap();
                        else
                            gameManager.ui_Manager.DisplayMapLostSoul(false);
                    }

                    displayMap = false;
                }
                else
                {
                    if (isImpostor || gameManager.paradiseIsFind || gameManager.hellIsFind)
                    {
                        gameManager.ui_Manager.DisplayMap();
                    }
                }
            }
            if (displayTutorial)
            {
                if (gameManager.game.currentRoom.isSpecial || gameManager.game.currentRoom.isTraped || (gameManager.GetDoorExpedition(gameManager.GetPlayerMine().GetId()) && gameManager.GetDoorExpedition(gameManager.GetPlayerMine().GetId()).isTraped)) 
                {
                    DisplayTutorialSpecial();
                }
                else
                {
                    DisplayTutorial(15, true);
                }
                displayTutorial = false;
            }

          



            if (GetComponent<PhotonView>().IsMine && gameManager.timer.timerFinish && !gameManager.GetPlayerMine().GetHasVoted_CP() && !gameManager.voteDoorHasProposed && !gameManager.isLoading)
            {
                if (!isCollisionInDoorTakeDoor && !isCollisionInDoorBackExpedition && !isCollisionInDoorExpedition)
                {
                    gameManager.VoteCP(vote_cp);
                    gameManager.GetPlayerMine().SetHasVoted_CP(true);
                    //vote_cp = 0;
                }
            }
        }
        // Mobile moving character

        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began )
        {
            Touch touch = Input.GetTouch(0);
            pointA = Camera.main.ScreenToWorldPoint(touch.position);
            oldPointB = pointA;
        }

        if ((Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved) || (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Stationary))
        {
            Touch touch = Input.GetTouch(0);
            pointB = Camera.main.ScreenToWorldPoint(touch.position);
            touchStart = true;
        }
        else
        {
            touchStart = false;
        }



        // permit to display blackScreen with transition

        if (gameManager && GetComponent<PhotonView>().IsMine)
        {
            if (gameManager.timer.timerFinish && !takeDoor && isCollisionInDoorTakeDoor)
            {
                takeDoor = true;
                isCollisionInDoorTakeDoor = false;
                gameManager.timer.ResetTimer();
            }
            if (gameManager.timer.timerFinish && !takeDoorBackExpedition && isCollisionInDoorBackExpedition)
            {
                takeDoorBackExpedition = true;
                isCollisionInDoorBackExpedition = false;
                gameManager.timer.ResetTimer();
            }
            if (gameManager.timer.timerFinish && !takeDoorExpededition && isCollisionInDoorExpedition)
            {
                takeDoorExpededition = true;
                isCollisionInDoorExpedition = false;
                gameManager.timer.ResetTimer();
            }


            if (takeDoor)
            {
                gameManager.TakeDoor(doorCollision.transform.parent.gameObject, this.gameObject);
                takeDoor = false;
                canMove = true;
                gameManager.ui_Manager.DisplayBlackScreen(false, true);
            }
            if (takeDoorBackExpedition)
            {
                gameManager.BackToExpedition();
                takeDoorBackExpedition = false;
                canMove = true;
                gameManager.ui_Manager.DisplayBlackScreen(false, true);

            }
            if (takeDoorExpededition)
            {
                gameManager.TeleportPlayerToExpedition();
                takeDoorExpededition = false;
                canMove = true;
                gameManager.ui_Manager.DisplayBlackScreen(false, true);
            }
                
           
        }



        


        if (gameManager)
        {
            if ((gameManager.expeditionHasproposed || gameManager.voteDoorHasProposed) && !isAlreadyHide)
            {
                this.transform.Find("Skins").GetChild(indexSkin).Find("Light_around").gameObject.SetActive(false);
                isAlreadyHide = true;
                GetComponent<PlayerGO>().isChooseForExpedition = false;
                transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = false;
                if (isBoss)
                {
                    gameManager.ResetTimerBoss(true);
                    transform.GetChild(2).GetChild(0).GetComponent<Text>().enabled = false;
                    timerBoss = 40;
                }

            }


        }

        InputDisplayMap();
        InputDisplayTutorial();
        InputChangeBoss();





        SetZIndexByPositionY();
        ActionnWantToChangeBoss();


        if (gameManager)
        {
            if (!gameManager.game)
            {
                return;
            }
            if (!gameManager.game.currentRoom)
            {
                return;
            }
            if (gameManager.game.currentRoom.IsFoggy)
            {
                DisplayNamePlayer(false);
                if (GetComponent<PhotonView>().IsMine)
                {
                    if ((this.isInvisible || this.isSacrifice))
                    {;
                        IncreaseTransparency(true);
                    }
                        
                }
                
            }
            else
            {
                DisplayNamePlayer(true);
                if (GetComponent<PhotonView>().IsMine)
                {
                    if ((this.isInvisible || this.isSacrifice))
                    {
                        IncreaseTransparency(false);
                    }
                        
                }
            }
        }

        if (GetComponent<PhotonView>().IsMine)
        {
            if (!isAlreaySerCanLauchLeverExplorationCouroutine)
            {
                if (hasWinFireBallRoom && isChooseForExpedition && !canLaunchExplorationLever)
                {
                    StartCoroutine(SetCanLauchExplorationLeverCoroutine());
                }   
            }

        }

        if (animationDeath)
        {
           
            if (!animationDeathUpFinish)
                MovingUpForDeathAnimation();
            if (animationDeathUpFinish)
            {
                MovingDownForDeathAnimation();
            }
        }

        if (gameManager)
        {
            if (gameManager.game)
            {
                if (gameManager.game.currentRoom.isLabyrintheHide && gameManager.speciallyIsLaunch)
                {
                    if(this.transform.position.y > 2.9f)
                    {
                        transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = -12;
                    }
                }

            }
            if(!gameManager.SamePositionAtBoss() && this.isBoss)
            {
                this.transform.Find("TimerForceVote").Find("CanvasTimer").Find("Timer").GetComponent<Text>().enabled = false;
            }
            else
            {
                if (this.isBoss)
                {
                    this.transform.Find("TimerForceVote").Find("CanvasTimer").Find("Timer").GetComponent<Text>().enabled = true;
                }
            }
        }


    }

    public IEnumerator SetCanLauchExplorationLeverCoroutine()
    {
        yield return new WaitForSeconds(2);
        canLaunchExplorationLever = true;
        isAlreaySerCanLauchLeverExplorationCouroutine = true;
    }

    public void IncreaseTransparency(bool increase)
    {
        
        if(increase)
            this.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.2f);
        else
            this.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }


    public void SetZIndexByPositionY()
    {  
        if (GameObject.Find("GameManager"))
        {
            if(gameManager && gameManager.game && gameManager.game.currentRoom)
            {
                if (gameManager.game.currentRoom.isLabyrintheHide && gameManager.speciallyIsLaunch)
                    return;
            }
            if (isSacrifice)
                return;
        }
        if (this.transform.Find("Skins").GetChild(indexSkin).gameObject.activeSelf)
        {
            /*this.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().sortingOrder = ((int)(this.transform.position.y * 10)) * -1;*/
            this.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = ((int)((this.transform.position.y+1) * 10)) * -1;
            this.transform.Find("Skins").GetChild(indexSkin).Find("Light_around").GetComponent<SpriteRenderer>().sortingOrder = (int)(((this.transform.position.y+1) * 10) * -1) - 2;
        }
    }




    public void OnClickChat()
    {
        displayChatInput = !displayChatInput;
        if (!displayChatInput)
        {
            SetTextChat(chatPanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text);
        }
        DisplayChat(displayChatInput);
    }

    public void DisplayNamePlayer(bool display)
    {
        if (display)
        {
            if (this.isInvisible)
            {
                return;
            }
        }
            
        this.transform.Find("InfoCanvas").Find("PlayerName").gameObject.SetActive(display);
    }

    public bool IsDoubleTap()
    {

        bool result = false;
        float MaxTimeWait;

#if UNITY_IOS
        MaxTimeWait = 0.07f;
#else
        MaxTimeWait = 0.022f;
#endif

        float VariancePosition = 0.5f;

      
        if (Input.touchCount == 1 &&( Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary))
        {

            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;


            SetTextChat(DeltaTime + " " + MaxTimeWait + " " + DeltaPositionLenght + " " + VariancePosition);
            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
            {
                result = true;
            }
              
        }
        return result;
    }



    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!gameManager)
            return;

        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        CollisionWithDoorToExploration(collision, true);

        if (!collision.CompareTag("Zone_vote"))
        {
            return;
        }
        if (this.isSacrifice)
        {
            return;
        }

        if (collision.name == "vote_X")
        {
            GetComponent<PlayerNetwork>().SendVoteExplorationDisplay(false);
        }
        else if (collision.name == "vote_V")
        {
            GetComponent<PlayerNetwork>().SendVoteExplorationDisplay(true);
        }



    }

    public GameObject GetOnlyChildActive(GameObject listObject)
    {
        for(int i =0;  i< listObject.transform.childCount; i++)
        {
            if (listObject.transform.GetChild(i).gameObject.activeSelf && listObject.transform.GetChild(i).name != "AwardObject" && listObject.transform.GetChild(i).name != "VirusRoom" && listObject.transform.GetChild(i).name != "FoggyRoom")
            {
                return listObject.transform.GetChild(i).gameObject;
            }
        }
        return listObject.transform.GetChild(1).gameObject;
    }



    private void handlePlayerMove()
    {
       
        handleDesktopMove();
        handleMobileMove();
      
    }

    private void handleDesktopMove()
    {
        turnPlayer();
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
        oldPosition = transform.position;
        float horizontal = InputManager.GetAxis("Horizontal");
        float vertical = InputManager.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 1.1f)
        {

            horizontal *= 0.9f;
            vertical *= 0.9f;
        }
        this.transform.Translate(
        new Vector3(
            horizontal,
            vertical,
            0
        ) * movementlControlSpeed * Time.deltaTime
    );
        
        if( horizontal > 0 || horizontal < 0 || vertical > 0 || vertical < 0)
        {
            if(gameManager && !gameManager.speciallyIsLaunch)
            {
                if (this.transform.Find("Life").Find("TwoHeart").gameObject.activeSelf || this.transform.Find("Life").Find("OneHeart").gameObject.activeSelf
                    || this.transform.Find("Life").Find("ThreeHeart").gameObject.activeSelf)
                {
                    this.GetComponent<PlayerNetwork>().SendResetHeart();
                }
            }
        }
    }

    public void ChangeSystemSyncPostionToTrial()
    {
        if (gameManager && gameManager.speciallyIsLaunch)
        {
            if(!this.GetComponent<PhotonRigidbody2DView>().preciseSyncPosition)
                SendChangeSyncFunction(true);
            this.GetComponent<PhotonRigidbody2DView>().preciseSyncPosition = true;
            
        }
        else
        {
            if(this.GetComponent<PhotonRigidbody2DView>().preciseSyncPosition)
                SendChangeSyncFunction(false);
            this.GetComponent<PhotonRigidbody2DView>().preciseSyncPosition = false;
         
        }
    }

   public void SendChangeSyncFunction(bool change)
    {
        GetComponent<PlayerNetwork>().SendChangeSyncFunction(change);
    }
    public void InputDownORUp()
    {

        float horizontal = InputManager.GetAxis("Horizontal");
        float vertical = InputManager.GetAxis("Vertical");

        if(((oldHorizontal == 0 && Mathf.Abs(horizontal) > 0)  || (oldHorizontal >  0 && horizontal < 0 ) ||(oldHorizontal < 0 && horizontal > 0))
            || ( oldVertical == 0 && Mathf.Abs(vertical) > 0 || (oldVertical > 0 && vertical < 0) || (oldVertical < 0 && vertical > 0)))
        {
            //SendChangeSyncFunction(false);
            //this.GetComponent<PlayerNetwork>().SendSpacePosition(this.transform.position.x, this.transform.position.y, this.GetComponent<PhotonView>().ViewID);
            //StartCoroutine(SendPositionCoroutine(0.2f));
            //StartCoroutine(SendPositionCoroutine(0.5f));
        }
        oldHorizontal = horizontal;
        oldVertical = vertical;
    }


    public IEnumerator SendPositionCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        this.GetComponent<PlayerNetwork>().SendSpacePosition(this.transform.position.x, this.transform.position.y, this.GetComponent<PhotonView>().ViewID);
    }

    public void TranslateSpacePositionWhenUpdated(float x , float y)
    {
        Vector3 newPosition = new Vector3(x, y);
        Vector3 distance = newPosition - this.transform.position;
        this.transform.Translate(distance * 8f * Time.deltaTime);
        Debug.Log(Mathf.Abs(distance.x) + Mathf.Abs(distance.y));
        if(Mathf.Abs(distance.x) + Mathf.Abs(distance.y) < 0.03f)
        {
            positionSended = false;
        }
    }

    private void handleMobileMove()
    {
        isMoving = false;
        if (touchStart)
        {
            Vector2 offset = pointB - pointA;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

            if (direction.x < 0)
            {
                turnLeft();
            }
            else
            {
                turnRight();

            }
            this.transform.Translate(direction * movementlControlSpeed * Time.deltaTime);
            if (direction.x > 0.2f || direction.y > 0.2f || direction.x < -0.2f || direction.y < -0.2f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
    }

    private void turnPlayer()
    {
        if (InputManager.GetAxis("Horizontal") < 0)
        {
            turnLeft();
        }
        if (InputManager.GetAxis("Horizontal") > 0)
        {
            turnRight();
        }
    }

    public void TurnChat()
    {
        Transform chat = transform.Find("InfoCanvas").Find("ChatPanel");
        if (chat.gameObject.activeSelf)
            return;
        Transform perso = transform.Find("Skins").GetChild(indexSkin);
        if (perso.localScale.x < 0)
        {
            chat.localPosition = new Vector3(561, 182);
            chat.Find("NormalChat").Find("ChatPanel").gameObject.SetActive(true);
            chat.Find("NormalChat").Find("ChatLeft").gameObject.SetActive(false);
            chat.Find("ChatPanelMoreLarger").Find("ChatPanel").gameObject.SetActive(true);
            chat.Find("ChatPanelMoreLarger").Find("ChatLeft").gameObject.SetActive(false);
        }
        else
        { 
            chat.localPosition = new Vector3(-223, 182);
            chat.Find("NormalChat").Find("ChatPanel").gameObject.SetActive(false);
            chat.Find("NormalChat").Find("ChatLeft").gameObject.SetActive(true);
            chat.Find("ChatPanelMoreLarger").Find("ChatPanel").gameObject.SetActive(false);
            chat.Find("ChatPanelMoreLarger").Find("ChatLeft").gameObject.SetActive(true);
        }
       


    }

    private void turnLeft()
    {
        Transform perso = transform.Find("Skins").GetChild(indexSkin);
        perso.localScale = new Vector3(
            Mathf.Abs(perso.localScale.x),
            perso.localScale.y
        );
        transform.Find("TrialObject").localScale = new Vector3(
            Mathf.Abs(transform.Find("TrialObject").localScale.x),
            transform.Find("TrialObject").localScale.y);
        //transform.Find("TrialObject").transform.localPosition = new Vector3(-0.455f, transform.Find("TrialObject").transform.localPosition.y);
        //transform.Find("BlueTorch").transform.transform.eulerAngles = new Vector3(0, 0, 24); 

    }

    private void turnRight()
    {
        Transform perso = transform.Find("Skins").GetChild(indexSkin);
        perso.localScale = new Vector3(
            -Mathf.Abs(perso.localScale.x),
            perso.localScale.y
        );


        transform.Find("TrialObject").localScale = new Vector3(
            -Mathf.Abs(transform.Find("TrialObject").localScale.x),
            transform.Find("TrialObject").localScale.y);
        //transform.Find("TrialObject").transform.localPosition = new Vector3(0.455f, transform.Find("TrialObject").transform.localPosition.y);
        //transform.Find("BlueTorch").transform.transform.eulerAngles = new Vector3(0, 0, -24);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
/*        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager && gameManager.fireBallIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isTouchInTrial)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            if (gameManager && gameManager.deathNPCIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isTouchInTrial)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            if(gameManager && gameManager.damoclesIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isTouchInTrial)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            Debug.Log()
            Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>());
        }*/
        
    }



    public void CollisionWithDoorToExploration(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!hasCurrentTorch)
            return;
        if (!gameManager || !gameManager.game || !gameManager.game.currentRoom)
            return;
        if (gameManager.game.currentRoom.explorationIsUsed)
            return;
        if (!collision.gameObject.name.Equals("CollisionPowerImpostor"))
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().barricade)
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().isOpenForAll)
            return;
        if (gameManager.voteDoorHasProposed)
            return;
        if (hasBlackTorch)
            return;
        if (isBoss && !hasWinFireBallRoom)
            return;
        if (enter && !canCollisionToDoorExploration)
            return;
        
        gameManager.ui_Manager.DisplayButtonPowerExplorationBigger(enter);
        collsionDoorIndexForExploration = collision.transform.parent.gameObject.GetComponent<Door>().index;

        if (!enter)
            StartCoroutine(CanCollisionToDoorExplorationCoroutine());
    }

    public IEnumerator CanCollisionToDoorExplorationCoroutine()
    {
        canCollisionToDoorExploration = false;
        yield return new WaitForSeconds(0.5f);
        canCollisionToDoorExploration = true;
    }

    public void CollisionWithDoorToMagicalKey(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.name.Equals("CollisionPowerImpostor"))
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().barricade)
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().isOpenForAll)
            return;
        if(!gameManager)
            return;
        gameManager.ui_Manager.DisplayMagicalKeyButtonBigger(enter);
        collsionDoorIndexForExploration = collision.transform.parent.gameObject.GetComponent<Door>().index;

    }

    public void CollisionWithDoorToBlackTorch(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!hasCurrentTorch)
            return;
        if (gameManager.game.currentRoom.explorationIsUsed)
            return;
        if (!collision.gameObject.name.Equals("CollisionPowerImpostor"))
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().barricade)
            return;
        if (collision.transform.parent.gameObject.GetComponent<Door>().isOpenForAll)
            return;
        if (gameManager.voteDoorHasProposed)
            return;
        if (gameManager.ISTrailsRoom(gameManager.game.currentRoom) && !hasWinFireBallRoom)
            return;
        if (!hasBlackTorch)
            return;
        gameManager.ui_Manager.DisplayButtonBlackTorchBigger(enter);
    }

    public void CollisionWithNPC(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("NPC"))
            return;
        if (!isBoss)
            return;


        if (collision.gameObject.name.Equals("NPCLeft"))
            indexNpc = 0;
        else if (collision.gameObject.name.Equals("NPCMiddle"))
            indexNpc = 1;
        else
            indexNpc = 2;

        gameManager.ui_Manager.DisplayButtonNPCBigger(enter);
    }
    public void CollisionWithNPC_informationEnd(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("NPC_informationEnd"))
            return;
        gameManager.ui_Manager.DisplayButtonNPC_InformationEndBigger(enter);
    }

    public void CollisionWithMainKey(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!isBoss)
            return;
        if (!collision.gameObject.tag.Equals("Lever"))
            return;
        if (!collision.gameObject.name.Equals("OpenDoor_lever"))
            return;
        if (gameManager.timer.timerLaunch)
        {
            gameManager.ui_Manager.DisplayButtonMainKeyBigger(false);
            return;
        }
            
        

        gameManager.ui_Manager.DisplayButtonMainKeyBigger(enter);
    }

    public void CollisionWithMap(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("InteractionObject"))
            return;
        if (!collision.gameObject.name.Equals("Map"))
            return;
        if (gameManager.timer.timerLaunch)
        {
            gameManager.ui_Manager.DisplayButtonMainKeyBigger(false);
            return;
        }



        gameManager.ui_Manager.DisplayButtonMapBigger(enter);
    }

    public void CollisionWithChangeBoss(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("InteractionObject"))
            return;
        if (!collision.gameObject.name.Equals("Boss"))
            return;
        if (isBoss)
            return;
        if (gameManager.timer.timerLaunch)
        {
            gameManager.ui_Manager.DisplayButtonMainKeyBigger(false);
            return;
        }



        gameManager.ui_Manager.DisplayButtonChnageBossBigger(enter);
    }

    public void CollisionWithTutorial(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("InteractionObject"))
            return;
        if (!collision.gameObject.name.Equals("Tutorial"))
            return;
        if (gameManager.timer.timerLaunch)
        {
            gameManager.ui_Manager.DisplayButtonMainKeyBigger(false);
            return;
        }



        gameManager.ui_Manager.DisplayButtonTutorialBigger(enter);
    }

    public void CollisionWithTutorialSpeciality(Collider2D collision, bool enter)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (!collision.gameObject.tag.Equals("InteractionObject"))
            return;
        if (!collision.gameObject.name.Equals("TutorialSpeciallyRoom"))
            return;
        if (gameManager.timer.timerLaunch)
        {
            gameManager.ui_Manager.DisplayButtonMainKeyBigger(false);
            return;
        }



        gameManager.ui_Manager.DisplayButtonTutorialBigger(enter);
    }


    // COPIEZ LA FONCTION D4AU DESSSUS MON POTE


    public void IgnoreCollisionAllPlayer(bool ignore)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().ViewID != this.GetComponent<PhotonView>().ViewID)
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID) && !player.GetComponent<PlayerGO>().isSacrifice && 
                    !player.GetComponent<PlayerGO>().isInJail)
                {
                    player.GetComponent<CapsuleCollider2D>().isTrigger = ignore;
                    Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), ignore);
                }
            }
        }
    }

    public void IgnoreCollisionPlayer(int indexPlayer, bool ignore)
    {
        Physics2D.IgnoreCollision(this.transform.GetComponent<CapsuleCollider2D>(), gameManager.GetPlayer(indexPlayer).transform.GetComponent<CapsuleCollider2D>(), ignore);
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameManager)
            return;

        if (!GetComponent<PhotonView>().IsMine)
            return;

        if (collision.gameObject.CompareTag("teleport_paradise"))
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                gameManager.timer.LaunchTimer(100000f, false);
                collisionParadise = true;
                gameManager.ui_Manager.DisplayBlackScreen(true, false);
                gameManager.gameManagerNetwork.SendComeToParadise(this.GetComponent<PhotonView>().ViewID);
                StartCoroutine(gameManager.ui_Manager.CoroutineWaitToTransition());
                //Cursor.visible = true;
            }
        }
        if (collision.gameObject.CompareTag("Hell"))
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                gameManager.ui_Manager.DisplayBlackScreenToNoneImpostor();
                gameManager.gameManagerNetwork.SendComeToHell();
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CircleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), true);
                //Cursor.visible = true;
                collisionHell = true;
            }
        }

        if (GetComponent<PhotonView>().IsMine)
        {
            if (collision.CompareTag("Zone_vote"))
            {
                this.transform.Find("Skins").GetChild(indexSkin).Find("Light_around").gameObject.SetActive(true);
                if (collision.name == "vote_X")
                {
                    GetComponent<PlayerNetwork>().SendVoteExplorationDisplay(false);
                }
                else if (collision.name == "vote_V")
                {
                    GetComponent<PlayerNetwork>().SendVoteExplorationDisplay(true);
                }

            }
            if (collision.CompareTag("Door"))
            {
                collisionInDoorIndex = collision.GetComponent<Door>().index;
                //GetComponent<PlayerNetwork>().SendCollisionDoorIndex(collisionInDoorIndex);
            }
        }

        if (collision.gameObject.tag == "Lever")
        {
            CanLaunchExplorationLever(collision.gameObject , true); ;
            CanLaunchVoterDoorLever(collision.gameObject, true);
            CanLaunchSpeciallyRoomPower(collision.gameObject, true);
        }
        if(collision.gameObject.tag == "InteractionObject")
        {
            if(collision.gameObject.name == "Map")
                CanDisplayMap(collision.gameObject, true);
            if (collision.gameObject.name == "Tutorial")
                CanDisplayTutorialAutel(collision.gameObject, true);
            if (collision.gameObject.name == "TutorialSpeciallyRoom")
                CanDisplayTutorialAutel(collision.gameObject, true);
            if (collision.gameObject.name == "Boss")
                CanChangeBoss(collision.gameObject, true);


        }

        //CollisionWithDoorToExploration(collision, true);
        CollisionWithDoorToMagicalKey(collision, true);
        CollisionWithDoorToBlackTorch(collision, true);
        CollisionWithNPC(collision, true);
        CollisionWithNPC_informationEnd(collision, true);
        CollisionWithMainKey(collision, true);
        CollisionWithMap(collision, true);
        CollisionWithChangeBoss(collision, true);
        CollisionWithTutorial(collision, true);
        CollisionWithTutorialSpeciality(collision, true);


        if (collision.gameObject.tag == "TrialObject")
        {
            if (hasWinFireBallRoom)
                playerNetwork.SendDesactivateObject(this.GetComponent<PhotonView>().ViewID);
            else
            {
                if(gameManager.teamHasWinTrialRoom && !isTouchInTrial)
                    playerNetwork.SendDesactivateObjectTeam();
                if (gameManager.game.currentRoom.isImpostorRoom)
                    GameObject.Find("ImpostorRoom").GetComponent<ImpostorRoom>().CollisionObject(collision.gameObject.name);
            }
        }
    }



    public void InputExplorationAnimation()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canLaunchExplorationLever)
        {
            return;
        }

       
        //launchExpeditionWithAnimation = true;

  
    }

    public void InputDisplayMap()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canDisplayMap || displayChatInput)
        {
            return;
        }

        displayMap = true;
    }
    public void InputChangeBoss()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canLaunchChangeBoss)
        {
            return;
        }
        changeBoss = !changeBoss;

        if (gameManager.usedChangeBossPower)
        {
            gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayOpenDoorBeforeChangeBoss();
        }


    }

    public void InputDisplayTutorial()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canDisplayTutorial)
        {
            return;
        }

        displayTutorial = true;
    }

    public void InputVoteDoorAnimation()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canLaunchDoorVoteLever)
        {
            return;
        }

        launchVoteDoorMobile = true;


    }

    public void LaunchRoomSpeciallyPower()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!InputManager.GetButtonDown("Exploration") || !canLaunchSpeciallyRoomPower)
        {
            return;
        }
        if (!gameManager.AllPlayerBackToExpedition())
        {
            return;
        }
        if (gameManager.game.currentRoom.chest)
        {
            gameManager.gameManagerNetwork.SendActiveZoneVoteChest();
        }
        if (gameManager.game.currentRoom.fireBall)
        {
            gameManager.gameManagerNetwork.SendLaunchFireBallRoom();
        } 
        if (gameManager.game.currentRoom.isSacrifice)
        {
            gameManager.gameManagerNetwork.SendDisplayNuVoteSacrificeForAllPlayer();
            GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().LaunchTimerVote();
        }
        if (gameManager.game.currentRoom.isDeathNPC)
        {
            //gameManager.InstantiateDeathNPC();
            gameManager.gameManagerNetwork.SendLaunchDeathNPC();
        }
        if (gameManager.game.currentRoom.isSwordDamocles)
        {
            gameManager.gameManagerNetwork.SendLaunchDamoclesRoom();
        }
        if (gameManager.game.currentRoom.isAx)
        {
            gameManager.gameManagerNetwork.SendLaunchAxRoom();
        }
        if (gameManager.game.currentRoom.isSword)
        {
            gameManager.gameManagerNetwork.SendLaunchSwordRoom();
        }
        if (gameManager.game.currentRoom.isLostTorch)
        {
            gameManager.gameManagerNetwork.SendLaunchLostTorchRoom();
        }
        if (gameManager.game.currentRoom.isMonsters)
        {
            gameManager.gameManagerNetwork.SendLaunchMonsterRoom();
        }
        if (gameManager.game.currentRoom.isPurification)
        {
            gameManager.gameManagerNetwork.SendLaunchPurificationRoom();
        }
        if (gameManager.game.currentRoom.isResurection)
        {
            gameManager.gameManagerNetwork.SendLaunchResurectionRoom();
        }
        if (gameManager.game.currentRoom.isPray)
        {
            gameManager.gameManagerNetwork.SendLaunchPrayRoom();
        }
        if (gameManager.game.currentRoom.isNPC)
        {
            gameManager.gameManagerNetwork.SendNpcRoom();
        }
        if (gameManager.game.currentRoom.isLabyrintheHide)
        {
            gameManager.gameManagerNetwork.SendLaunchLabyrinthRoom();
        }

        gameManager.gameManagerNetwork.SendCloseDoorWhenVoteCoroutine();
        StartCoroutine(HideLeverCouroutine());
        //gameManager.ui_Manager.DisplaySpeciallyLevers(false, 0);
    }

    public IEnumerator HideLeverCouroutine()
    {
        yield return new WaitForSeconds(0.2f);
        gameManager.gameManagerNetwork.SendDisplaySpeciallyLevers(false, 0);
    }



    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameManager)
            return;

        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
       

        if (collision.CompareTag("Zone_vote"))
        {
            this.transform.Find("Skins").GetChild(indexSkin).Find("Light_around").gameObject.SetActive(false);
            GetComponent<PlayerNetwork>().SendHideVoteExplorationDisplay();
        }
        if (collision.CompareTag("Door"))
        {
            collisionInDoorIndex = -1;
            //GetComponent<PlayerNetwork>().SendCollisionDoorIndex(-1);
        }
        if (collision.gameObject.tag == "Lever")
        {
            CanLaunchExplorationLever(collision.gameObject, false);
            CanLaunchVoterDoorLever(collision.gameObject, false);
            CanLaunchSpeciallyRoomPower(collision.gameObject, false);
        }

        if (collision.gameObject.tag == "InteractionObject")
        {
            if (collision.gameObject.name == "Map")
                CanDisplayMap(collision.gameObject, false);
            if (collision.gameObject.name == "Tutorial")
                CanDisplayTutorialAutel(collision.gameObject, false);
            if (collision.gameObject.name == "TutorialSpeciallyRoom")
                CanDisplayTutorialAutel(collision.gameObject, false);
            if (collision.gameObject.name == "Boss")
                CanChangeBoss(collision.gameObject, false);

        }

        CollisionWithDoorToExploration(collision, false);
        CollisionWithDoorToMagicalKey(collision, false);
        CollisionWithDoorToBlackTorch(collision, false);
        CollisionWithNPC(collision, false);
        CollisionWithNPC_informationEnd(collision, false);
        CollisionWithMainKey(collision, false);
        CollisionWithMap(collision, false);
        CollisionWithChangeBoss(collision, false);
        CollisionWithTutorial(collision, false);
        CollisionWithTutorialSpeciality(collision, false);
    }


    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        SetPlayerNameServer();
    }

    public void SetSkin(int indexSkin)
    {
        this.indexSkin = indexSkin;
        SetPlayerNameServer();
    }

    public void SetIconDeath(bool display)
    {
        this.transform.Find("Skins").GetChild(indexSkin).transform.Find("DeadIcon").gameObject.SetActive(display);
    }

    public void DisplayCharacter(bool display)
    {
        GetComponent<PlayerNetwork>().SendDisplayCharacter(display);

        if (display)
        {
            if (gameManager.SamePositionAtBossWithIndex(this.GetComponent<PhotonView>().ViewID))
                transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            return;
        }
        if (gameManager.SamePositionAtBossWithIndex(this.GetComponent<PhotonView>().ViewID))
            transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }

    public void SetPlayerNameServer()
    {
        if (playerNetwork)
        {
            playerNetwork.SendNamePlayer(playerName);

            if (GetComponent<PhotonView>().IsMine)
            {
                playerNetwork.SendindexSkin(indexSkin);
                playerNetwork.SendindexSkinColor(indexSkinColor, true);
                string userId = PhotonNetwork.LocalPlayer.UserId;
                playerNetwork.SendUserId(userId);
            }
            else
            {
                DesactivateAllSkin();
            }
        }

    }

    public void SetSkinBoss(bool isBoss)
    {
        if (gameManager.game.GetBoss())
        {
            if (gameManager.game.GetBoss().GetId() == this.GetComponent<PhotonView>().ViewID)
            {
                this.transform.GetChild(1).GetChild(3).gameObject.SetActive(isBoss);

            }
            else
            {
                this.transform.GetChild(1).GetChild(3).gameObject.SetActive(isBoss);
            }
        }
    }

    public void SetSkinImpostor(bool isImpostor)
    {
        if (!gameManager.GetPlayerMine())
            return;
        if (gameManager.GetPlayerMine().GetIsImpostor() && this.GetComponent<PhotonView>().IsMine)
        {
            foreach(GameObject impostor in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (impostor.GetComponent<PlayerGO>().isImpostor)
                    impostor.transform.Find("InfoCanvas").Find("PlayerName").GetComponent<Text>().color = new Color(255, 0, 0);
            }
           
        }
    }


    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").GetComponent<Animator>().SetBool("OpenEyes", false);
        animateEyes = true;
        AnimateEyes();
    }

    public void AnimateEyes()
    {
        int secondebetweenAnimation = Random.Range(2, 15);
        StartCoroutine(Wait(secondebetweenAnimation));
    }

    public void DesactivateAllSkin()
    {
        for (int i = 0; i < transform.Find("Skins").childCount; i++)
        {
            transform.Find("Skins").GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DesactivateAllSkinColor()
    {
        for (int i = 0; i < transform.Find("Skins").GetChild(indexSkin).Find("Colors").childCount; i++)
        {
            transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClicktoExploration();
            ClickToVoteSacrifice();   
        }
    }

    public void ClicktoExploration()
    {
        if (!gameManager)
        {
            return;
        }
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (this.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (gameManager.game.currentRoom.explorationIsUsed)
        {
            gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayErrorBlueTorchNotAvaible();
            DispayRedLight();
            return;
        }
        if(gameManager.game.nbTorch <= 0)
        {
            return;
        }
        if (gameManager.timer.timerLaunch)
        {
            return;
        }
        if (gameManager.expeditionHasproposed)
        {
            return;
        }

        if (!gameManager.SamePositionAtBossWithIndex(this.GetComponent<PhotonView>().ViewID))
        {
            return;
        }
        if (isSacrifice)
        {
            return;
        }
        if (gameManager.game.currentRoom.isTrial && !gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayDoTrialBeforeUsedBlueTorch();
            DispayRedLight();
            return;
        }
        if (gameManager.speciallyIsLaunch)
            return;
        if (OnePlayerHasWinFireball())
            return;
        if (gameManager.indexPlayerPreviousExploration == this.transform.GetComponent<PhotonView>().ViewID)
        {
            gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayHeHasAlreadyTorch();
            DispayRedLight();
            return;
        }
            
        if (gameManager.onePlayerHasTorch && !this.transform.Find("TrialObject").Find("BlueTorch").Find("BlueTorchImg").gameObject.activeSelf)
            return;
        gameManager.gameManagerNetwork.SendDisplaySupportTorch(this.transform.Find("TrialObject").Find("BlueTorch").Find("BlueTorchImg").gameObject.activeSelf);
        GetComponent<PlayerNetwork>().SendDisplayBlueTorch(!this.transform.Find("TrialObject").Find("BlueTorch").Find("BlueTorchImg").gameObject.activeSelf);
    }
    public void ClickToVoteSacrifice()
    {
        if (!gameManager)
        {
            return;
        }
        if (this.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (gameManager.ui_Manager.map.activeSelf)
        {
            return;
        }
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lastPlayerIndexVote != this.GetComponent<PhotonView>().ViewID)
        {
            return;
        }
        if (!GameObject.Find("SacrificeRoom") || !GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().sacrificeVoteIsLaunch)
        {
            return;
        }
        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            return;
        }
        if (isSacrifice)
        {
            return;
        }
        if (GetPlayerMineGO().GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        

        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice = !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lastPlayerIndexVote = this.GetComponent<PhotonView>().ViewID;
        GetComponent<PlayerNetwork>().SendVoteToSacrifice(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice);
        transform.Find("Skins").GetChild(indexSkin).Find("Light_red").gameObject.SetActive(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice);
    }

    public void DisplayChat(bool display)
    {
        if (GetComponent<PhotonView>().IsMine && !display)
        {
            displayChatInput = false;
        }
        else
        {
            chatPanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text = "";
        }
        chatPanel.SetActive(display);
        chatPanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().ActivateInputField();

    }



    public void SetTextChat(string text)
    {
        int indexPlayer = GetComponent<PhotonView>().ViewID;
        if (text == "" || text == " " || text == "  ")
        {
            return;
        }

        string threeFirstLetter = "";
        try
        {
            threeFirstLetter = text.Substring(0, 4);
        }
        catch (System.Exception e)
        {
            Debug.Log("threeFirstLetter < 4");
        }


        if (GetComponent<PlayerGO>().isImpostor && threeFirstLetter.Equals("/imp") && text.Length > 3   )
        {
            string textWithoutSlash = text.Substring(4, text.Length -4);
            if (textWithoutSlash == "" || textWithoutSlash == " " || textWithoutSlash == "  ")
            {
                return;
            }
            GetComponent<PlayerNetwork>().SendTextChatToImpostor(indexPlayer, textWithoutSlash);
        }
        else
        {
            if (text[0].Equals('/'))
            {
                return;
            }
            GetComponent<PlayerNetwork>().SendTextChat(indexPlayer, text);
        }
    }


    public void SendMessagePlayerInTimes(int time)
    {
        currentlyMessageDisplay.Add("tkt");
        StartCoroutine(CouroutineTextChat(currentlyMessageDisplay.Count - 1, currentlyMessageDisplay.Count, time));
    }

    public IEnumerator CouroutineTextChat(int index, int count ,int time)
    {
        displayMessage = true;
        yield return new WaitForSeconds(time);
        currentlyMessageDisplay.RemoveAt(index - (count - 1));
        if (currentlyMessageDisplay.Count == 0)
            displayMessage = false;
    }


    public IEnumerator CoroutineIsChooseForExpedition()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<PlayerNetwork>().ResetIsChooseForExpedtion();

    }

    public void LaunchTimerBoss()
    {
        timerBossLaunch = true;
        timerBoss = 110;
    }


    public void ActionnWantToChangeBoss()
    {
        if (!this.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (gameManager && !gameManager.isLoading && (gameManager.expeditionHasproposed || gameManager.timer.timerLaunch || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed))
        {
            //playerNetwork.SendResetWantToChangeBoss();
            changeBoss = false;
            return;
        }
        if (displayChatInput)
        {
            return;
        }
        if (isBoss)
        {
            //playerNetwork.SendResetWantToChangeBoss();
            changeBoss = false;
            return;
        }
        if (!changeBoss)
        {
            return;
        }
        if (GameObject.Find("SacrificeRoom") && GameObject.Find("SacrificeRoom").GetComponent<SacrificeRoom>().sacrificeVoteIsLaunch)
        {
            return;
        }
        if (gameManager.usedChangeBossPower)
        {
            //gameManager.errorMessage.GetComponent<ErrorMessage>().DisplayOpenDoorBeforeChangeBoss();
            return;
        }



        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
        playerNetwork.SendWantToChangeBoss();
        changeBoss = false;

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

    public void CanDisplayMap(GameObject collision, bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (gameManager.speciallyIsLaunch)
            return;
        if (gameManager.timer.timerLaunch)
        {
            //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }
        canDisplayMap = isEnter;
        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.mobileCanvas.transform.Find("Map_panel").gameObject.SetActive(isEnter);
        if(isEnter)
            gameManager.ui_Manager.map_interaction.transform.Find("map_img").localScale = new Vector3(0.144f, 0.140f);
        else
            gameManager.ui_Manager.map_interaction.transform.Find("map_img").localScale = new Vector3(0.110f, 0.100f);
        if (isEnter)
        {
            DisplayTutorial(12);
        }

    }
    public void CanDisplayTutorialAutel(GameObject collision, bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (gameManager.timer.timerLaunch)
        {
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }

        canDisplayTutorial = isEnter;
        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.mobileCanvas.transform.Find("Tuto_panel").gameObject.SetActive(isEnter);


    }


    public void DisplayTutorial(int indexPanel, bool force = false)
    {
        if (gameManager.setting.displayTutorial || force)
        {

            if (!gameManager.ui_Manager.listTutorialBool[indexPanel] || force)
            {
                gameManager.ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
                gameManager.ui_Manager.tutorial_parent.SetActive(true);
                gameManager.ui_Manager.tutorial[indexPanel].SetActive(true);
                if (!force)
                    gameManager.ui_Manager.listTutorialBool[indexPanel] = true;
            }

        }
    }

    public void DisplayTutorialSpecial()
    {
        if (gameManager.game.currentRoom.chest)
            DisplayTutorial(16, true);
        if (gameManager.game.currentRoom.isSacrifice)
            DisplayTutorial(17, true);
        if (gameManager.game.currentRoom.IsVirus)
            DisplayTutorial(18, true);
        if (gameManager.game.currentRoom.fireBall)
            DisplayTutorial(19, true);
        if (gameManager.game.currentRoom.isJail)
            DisplayTutorial(20, true);
        if (gameManager.game.currentRoom.isAx)
            DisplayTutorial(24, true);
        if (gameManager.game.currentRoom.isSword)
            DisplayTutorial(25, true);
        if (gameManager.game.currentRoom.isSwordDamocles)
            DisplayTutorial(26, true);
        if (gameManager.game.currentRoom.isDeathNPC)
            DisplayTutorial(27, true);
        if (gameManager.game.currentRoom.isLostTorch)
            DisplayTutorial(28, true);
        if (gameManager.game.currentRoom.isMonsters)
            DisplayTutorial(29, true);
        if (gameManager.game.currentRoom.isLabyrintheHide)
            DisplayTutorial(30, true);
        if (gameManager.game.currentRoom.isPurification)
            DisplayTutorial(31, true);
        if (gameManager.game.currentRoom.isResurection)
            DisplayTutorial(32, true);
        if (gameManager.game.currentRoom.isPray)
            DisplayTutorial(33, true);
        if (gameManager.game.currentRoom.isNPC)
            DisplayTutorial(34, true);

    }


    public void CanLaunchExplorationLever(GameObject collision,  bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!isBoss) {
            DisplayTutorial(2);
            return;
        }
        if (gameManager.expeditionHasproposed || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed)
        {
            canLaunchExplorationLever = false;
            gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(false);
            return;
        }
        if(collision.name != "Exploration_lever")
        {
            return;
        }
        canLaunchExplorationLever = isEnter;

        gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(isEnter);

        if (isEnter)
        {
            DisplayTutorial(3);
        }
    }

    public void CanLaunchVoterDoorLever(GameObject collision, bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!isBoss && isEnter)
        {
            DisplayTutorial(2);
            return;
        }
        if ((gameManager.expeditionHasproposed && gameManager.timer.timerLaunch) || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed)
        {
            canLaunchDoorVoteLever = false;
            //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            GameObject.Find("Levers").transform.Find("OpenDoor_lever").Find("Hexagone").Find("HexagoneBiggerAndLight").gameObject.SetActive(false);
            gameManager.ui_Manager.mobileCanvas.transform.Find("Door_panel").gameObject.SetActive(false);
            return;
        }
        if (collision.name != "OpenDoor_lever")
        {
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }
        canLaunchDoorVoteLever = isEnter;
        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        GameObject.Find("Levers").transform.Find("OpenDoor_lever").Find("Hexagone").Find("HexagoneBiggerAndLight").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.mobileCanvas.transform.Find("Door_panel").gameObject.SetActive(isEnter);

        if (isEnter)
        {
            DisplayTutorial(11);
        }

    }

    public void CanLaunchSpeciallyRoomPower(GameObject collision, bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!isBoss)
        {
            DisplayTutorial(2);
            return;
        }
        if (gameManager.voteChestHasProposed)
        {
            canLaunchSpeciallyRoomPower = false;
            //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            gameManager.ui_Manager.DisplayLightLeverSpeciallyRoom(false);
            gameManager.ui_Manager.DisplayUI_Mobile_SpecialRoom(false);
            return;
        }
        if (collision.name != "SpeciallyRoom_lever")
        {
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }
        canLaunchSpeciallyRoomPower = isEnter;
        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.DisplayLightLeverSpeciallyRoom(isEnter);
        gameManager.ui_Manager.DisplayUI_Mobile_SpecialRoom(isEnter);
    }

    public void CanChangeBoss(GameObject collision, bool isEnter)
    {
        if (!gameManager)
            return;
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (isBoss)
        {
            //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            return;
        }
        if (gameManager.expeditionHasproposed || gameManager.timer.timerLaunch || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed){
            //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            gameManager.ui_Manager.mobileCanvas.transform.Find("Change_Boss").gameObject.SetActive(false);
            return;
        }
        if (isSacrifice)
            return;

        DisplayTutorial(22);
        if(isEnter)
            gameManager.ui_Manager.changeBoss_interaction.transform.Find("imageCercle").localScale = new Vector3(0.24f, 0.20f);
        else
            gameManager.ui_Manager.changeBoss_interaction.transform.Find("imageCercle").localScale = new Vector3(0.16f, 0.14f);
        canLaunchChangeBoss = isEnter;
        //transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.mobileCanvas.transform.Find("Change_Boss").gameObject.SetActive(isEnter);

    }
    public void SetRoomCursed()
    {
        List<Room> listPossiblityRoom = new List<Room>();
        List<Room> listPossiblityRoomWithMoreDistance = new List<Room>();
        foreach (Room room in gameManager.game.dungeon.rooms)
        {
            if(room.distance_pathFinding_initialRoom == gameManager.game.dungeon.exit.distance_pathFinding_initialRoom)
            {
                if (room.IsObstacle || room.IsExit)
                    continue;
                if(gameManager.game.dungeon.GetPathFindingDistance(room, gameManager.game.dungeon.exit) > 3)
                {
                    listPossiblityRoomWithMoreDistance.Add(room);
                }
                listPossiblityRoom.Add(room);
            }
        }
        if (listPossiblityRoomWithMoreDistance.Count == 0)
        {
            //this.distanceCursed = listPossiblityRoom[Random.Range(0, listPossiblityRoom.Count)].distance_pathFinding_initialRoom;
            this.roomUsedWhenCursed = listPossiblityRoom[Random.Range(0, listPossiblityRoom.Count)];
            return;
        }
        //this.distanceCursed =  listPossiblityRoomWithMoreDistance[Random.Range(0, listPossiblityRoomWithMoreDistance.Count)].distance_pathFinding_initialRoom;
        this.roomUsedWhenCursed = listPossiblityRoomWithMoreDistance[Random.Range(0, listPossiblityRoomWithMoreDistance.Count)];
    }

    public void DisplayHeartInSituation()
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        if(this.lifeTrialRoom == 3)
        {
            this.transform.Find("Life").Find("ThreeHeart").gameObject.SetActive(true);
        }
        if (this.lifeTrialRoom == 2)
        {
            this.transform.Find("Life").Find("ThreeHeart").gameObject.SetActive(false);
            this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(true);
        }
        else if (this.lifeTrialRoom == 1)
        {
            this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(false);
            this.transform.Find("Life").Find("OneHeart").gameObject.SetActive(true);
        }
        else if (this.lifeTrialRoom <= 0)
        {
            this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(false);
            this.transform.Find("Life").Find("OneHeart").gameObject.SetActive(false);
        }
    }

    public void ResetHeart()
    {
        this.transform.Find("Life").Find("ThreeHeart").gameObject.SetActive(false);
        this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(false);
        this.transform.Find("Life").Find("OneHeart").gameObject.SetActive(false);
    }
    public void DisiplayHeartInitial(bool display)
    {
        if(this.hasProtection)
            this.transform.Find("Life").Find("ThreeHeart").gameObject.SetActive(display);
        else
            this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(display);
    }

    public IEnumerator ResetInvincibleCouroutine()
    {
        yield return new WaitForSeconds(0.65f);
        this.isInvincible = false;
    }

    public void SetCanLaunchExplorationCoroutine(bool canLaunch)
    {
        
        canLaunchExplorationLever = true;
        StartCoroutine(gameManager.VerifyBugExplorationCouroutine());
    }

    public IEnumerator MovingDeathAnimationWaitCouroutine()
    {
        if(GetComponent<PhotonView>().IsMine)
            yield return new WaitForSeconds(2.6f);
        else
            yield return new WaitForSeconds(2.6f);
        animationDeath = true;
    }

    public void MovingUpForDeathAnimation()
    {
       
        this.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).Translate(new Vector3(0, 2.3f * Time.deltaTime));
        this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").Translate(new Vector3(0, 2.3f * Time.deltaTime));
        this.transform.Find("InfoCanvas").Translate(new Vector3(0, 2.3f * Time.deltaTime));
        if (Mathf.Abs(transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).position.y - old_y_position) > 0.32f)
        {
           
            animationDeathUpFinish = true;
            old_y_position = this.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).position.y;
        }
    }

    public void MovingDownForDeathAnimation()
    {
        this.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).Translate(new Vector3(0, -5f * Time.deltaTime));
        this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").Translate(new Vector3(0, -5f * Time.deltaTime));
        this.transform.Find("InfoCanvas").Translate(new Vector3(0, -5f * Time.deltaTime));
        if (Mathf.Abs(old_y_position - transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).position.y)  > 1.5f)
        {
            this.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).gameObject.SetActive(false);
            this.transform.Find("InfoCanvas").gameObject.SetActive(false);
            this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").gameObject.SetActive(false);
            animationDeathDownFinis = true;
            animationDeath = false;
        }
    }
    public IEnumerator CanMoveActiveCoroutine()
    {
        yield return new WaitForSeconds(4.5f);
        canMove = true;
/*        if (GetComponent<PhotonView>().IsMine)
        {*/
            this.transform.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).localPosition = new Vector3(0.04f, -8.60f);
            this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").localPosition = new Vector3(-0.33f, -0.24f);
            //this.transform.Find("Skins").GetChild(indexSkin).Find("Eyes1").gameObject.SetActive(true);
            this.transform.Find("InfoCanvas").localPosition = new Vector3(-0.812f, -0.14f);
            this.transform.Translate(new Vector3(0, -0.75f));
/*        }*/
    }
    private bool avaibleDash = true; 
    public void Dash()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!Input.GetKey(KeyCode.Space))
        {
            return;
        }
        if (!gameManager)
            return;
        if (!avaibleDash)
            return;

        if (!gameManager.game.currentRoom.isTrial && !gameManager.game.currentRoom.isTeamTrial)
            return;
        if (!gameManager.speciallyIsLaunch)
            return;
        if (gameManager.game.currentRoom.isLabyrintheHide)
            return;

        if (isTouchInTrial)
            return;
        float horizontal = InputManager.GetAxis("Horizontal");
        float vertical = InputManager.GetAxis("Vertical");
        if((horizontal > -0.1f && horizontal < 0.1f) && (vertical < 0.1f && vertical > -0.1f))
        {
            if(Mathf.Sign(this.transform.Find("Skins").GetChild(indexSkin).localScale.x) == 1) {
                this.transform.position += new Vector3(-1.5f,0);
            }
            else
            {
                this.transform.position += new Vector3(1.5f, 0);
            }
        }else if ( horizontal < 0.1f && horizontal > -0.1f )
        {
            this.transform.position += new Vector3(0, Mathf.Sign(vertical) * 1.5f);
        }
        if((vertical < 0.1f && vertical > -0.1f) && (horizontal > 0.1f || horizontal < -0.1f))
        {
            this.transform.position += new Vector3(Mathf.Sign(horizontal) * 1.5f, 0);
        }
        if((horizontal > 0.1f || horizontal < -0.1f) && (vertical > 0.1f || vertical < -0.1f))
        {
            this.transform.position += new Vector3(Mathf.Sign(horizontal) * 1.2f, Mathf.Sign(vertical) * 1.2f);
        }
        if(this.transform.position.y > 3.25f)
        {
            this.transform.position = new Vector2(this.transform.position.x, 3.25f);
        }
        if (this.transform.position.y < -3.1f)
        {
            this.transform.position = new Vector2(this.transform.position.x, -3.1f);
        }
        if (this.transform.position.x < -6.4)
        {
            this.transform.position = new Vector2(-6.4f, this.transform.position.y);
        }
        if (this.transform.position.x > 6.6)
        {
            this.transform.position = new Vector2(6.6f, this.transform.position.y);
        }
        //this.transform.position += new Vector3(Mathf.Sign(horizontal) * 1.5f, Mathf.Sign(vertical) * 1.5f);
        this.transform.Find("DashAnimation").GetChild(0).gameObject.SetActive(true);
        gameManager.ui_Manager.LaunchDashSound();
        StartCoroutine(CouroutineAvaibleDash());

    }

    public void DashIsAvailable()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager)
            return;
        if (!gameManager.game || !gameManager.game.currentRoom || !gameManager.game.currentRoom.isTrial)
        {
            //GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(96f / 255f, 96f / 255f, 96f / 255f);
            return;
        }
           
        if (!gameManager.speciallyIsLaunch)
        {
            //GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(96f / 255f, 96f / 255f, 96f / 255f);
            return;
        }
        if (gameManager.game.currentRoom.isLabyrintheHide)
        {
            //GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(96f / 255f, 96f / 255f, 96f / 255f);
            return;
        }
            
        if (!avaibleDash)
        {
            //GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(96f / 255f, 96f / 255f, 96f / 255f);
            return; 
        }
        //GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(255f, 255f, 255f);

    }

    public IEnumerator CoroutineDashImage()
    {
        yield return new WaitForSeconds(1.5f); 
        GameObject dashImg = this.transform.Find("DashImg").gameObject;
        dashImg.SetActive(false);

        
    }
    public IEnumerator CouroutineAvaibleDash()
    {
        avaibleDash = false;
        GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(96f/255f, 96f / 255f, 96f / 255f);
        yield return new WaitForSeconds(3.5f);
        avaibleDash = true;
        GameObject.Find("DashInformation").GetComponent<Image>().color = new Color(255f, 255f, 255f);
    }


    public void SetlistTrialObject()
    {
        listTrialObject.Clear();
        listTrialObject.Add(hasMap);
        listTrialObject.Add(hasProtection);
        listTrialObject.Add(hasTrueEyes);
    }

    public void DisplayCursedPlayers()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
           if(player.GetComponent<PlayerGO>().isCursed || player.GetComponent<PlayerGO>().isBlind)
           {
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Light_Cursed").gameObject.SetActive(true);
           }
        }
    }

    public bool OnePlayerHasWinFireball()
    {
        GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in allPlayer)
        {
            if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
                return true;
        }
        return false;
    }
    
    public void ActivateCollisionLabyrinth()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if( InputManager.GetAxis("Vertical") > 0.1)
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionUp").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionUp").gameObject.SetActive(false);
        }
        if (InputManager.GetAxis("Vertical") < -0.1)
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionDown").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionDown").gameObject.SetActive(false);
        }
        if (InputManager.GetAxis("Horizontal") < -0.1 || InputManager.GetAxis("Horizontal") > 0.1)
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionLeft").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("CollisionLabyrinth").Find("collisionLeft").gameObject.SetActive(false);
        }
    }

    
    public void AddInInventory(int index)
    {
        Inventory.Add(index);
    }
    public void ManageInventoryTest()
    {
        AddInInventory(5);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(12);
        AddInInventory(7);
        AddInInventory(7);
        AddInInventory(9);
    }

    public IEnumerator GetListSkinIndexInServer()
    {
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        string linkServer = setting.linkServerAws;
        string steamId = SteamUser.GetSteamID().ToString();
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(linkServer + "/player/find?steamId=" + steamId, form);
        www.certificateHandler = new CertifcateValidator();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.downloadHandler.text);
            StartCoroutine(GetListSkinIndexInServer());
        }
        else
        {
            Debug.LogError(www.downloadHandler.text);
            RequestSkin skinreturn = JsonUtility.FromJson<RequestSkin>(www.downloadHandler.text);

            if (skinreturn.response.skins == null)
            {
                string pseudoSteam = SteamFriends.GetPersonaName();
                UnityWebRequest www2 = UnityWebRequest.Post(linkServer + "/player/addPlayer?steamId=" + steamId +"&pseudoSteam="+ pseudoSteam, form);
                www2.certificateHandler = new CertifcateValidator();
                yield return www2.SendWebRequest();
                StartCoroutine(GetListSkinIndexInServer());
            }
            else
            {
                for (int i = 0; i < skinreturn.response.skins.Length; i++)
                {
                    AddInInventory(skinreturn.response.skins[i].id);
                    Debug.Log("  skin : " + skinreturn.response.skins[i].name);
                }
                blackSoul_money = skinreturn.response.money;
            }
        }
    }

    public IEnumerator GetListSkinIndexInServerTestIP()
    {
        Setting setting = GameObject.Find("Setting").GetComponent<Setting>();
        string linkServer = setting.linkServerAws;
        string steamId = setting.ip;
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(linkServer + "/player/find?steamId=" + steamId, form);
        www.certificateHandler = new CertifcateValidator();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.text);
            StartCoroutine(GetListSkinIndexInServerTestIP());
        }
        else
        {
            Debug.LogError(www.downloadHandler.text);
            RequestSkin skinreturn = JsonUtility.FromJson<RequestSkin>(www.downloadHandler.text);

            if (skinreturn.response.skins == null)
            {
                string pseudoSteam = setting.ip;
                UnityWebRequest www2 = UnityWebRequest.Post(linkServer + "/player/addPlayer?steamId=" + steamId + "&pseudoSteam=" + pseudoSteam, form);
                www2.certificateHandler = new CertifcateValidator();
                yield return www2.SendWebRequest();
                StartCoroutine(GetListSkinIndexInServerTestIP());
            }
            else
            {
                for (int i = 0; i < skinreturn.response.skins.Length; i++)
                {
                    AddInInventory(skinreturn.response.skins[i].id);
                    Debug.Log("  skin : " + skinreturn.response.skins[i].name);
                }
                blackSoul_money = skinreturn.response.money;
            }
        }
    }

    public IEnumerator SetInvincibility()
    {
        yield return new WaitForSeconds(2);
        isInvincible = false;
    }

    public void DispayRedLight()
    {
        if (!this.transform.Find("Skins").GetChild(indexSkin).Find("Light_red").gameObject.activeSelf)
            StartCoroutine(DispayRedLightCouroutine());

    }

    public IEnumerator DispayRedLightCouroutine()
    {
        this.transform.Find("Skins").GetChild(indexSkin).Find("Light_red").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        this.transform.Find("Skins").GetChild(indexSkin).Find("Light_red").gameObject.SetActive(false);

    }


}
