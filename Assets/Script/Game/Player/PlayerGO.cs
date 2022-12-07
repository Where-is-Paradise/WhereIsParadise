using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Luminosity.IO;

public class PlayerGO : MonoBehaviour
{

    public float movementlControlSpeed = 1;
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
    public bool ui_isOpen = false;

    public bool isChooseForExpedition = false;
    public bool isAlreadyHide = false;

    public bool displayChatInput = false;

    public bool displayMessage = false;
    private List<string> currentlyMessageDisplay;

    public ResolutionManagement resolution;

    public int collisionInDoorIndex = -1;

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

    public bool isCursed = false;

    public bool lightHexagoneIsOn = false;

    public int distanceCursed = 0;
    public Room roomUsedWhenCursed;

    public bool isInvincible = false;
    private void Awake()
    {
        displayChatInput = false;
        playerNetwork = gameObject.GetComponent<PlayerNetwork>();
        currentlyMessageDisplay = new List<string>();

        isMovingAutomaticaly = true;
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        setIsBoss();

        AnimateEyes();

        setCollider();

        // Used to prevent multi tap on mobile

        enhanceOwners();

    }

    private void enhanceOwners()
    {
        List<string> owners = new List<string> { "Homertimes   ", "Anis   ", "Onestla   " };

        if (owners.Contains(playerName))
        {
            transform.localScale = new Vector2(0.85f, 0.85f);
            this.transform.GetChild(1).GetChild(5).gameObject.SetActive(true);
            this.transform.GetChild(1).GetChild(7).gameObject.SetActive(true);
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
            this.transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("OpenEyes", true); ;
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
            this.GetComponent<CapsuleCollider2D>().enabled = true;
            isMovingAutomaticaly = false;
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

        
    }


    void Update()
    {
        if (GameObject.Find("Lobby_Manager"))
        {
            if (this.GetComponent<PhotonView>().IsMine && !GameObject.Find("Lobby_Manager").GetComponent<Lobby>().matchmaking)
            {
                if(PhotonNetwork.IsMasterClient)
                    GetComponent<PlayerNetwork>().SendDisplayCrown(true);
                else
                    GetComponent<PlayerNetwork>().SendDisplayCrown(false);
            }

        }

        if (isMovingAutomaticaly)
        {
            return;
        }
        TurnChat();
        if (displayMessage)
        {
            this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
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
            
            if(!gameManager.isActuallySpecialityTime)
                SetSkinBoss(isBoss);
            if (!hideImpostorInformation)
                SetSkinImpostor(isImpostor);
            else
                SetSkinImpostor(false);

            if (gameManager.timer.timerFinish && !gameManager.alreadyPass)
            {
                this.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
                gameManager.alreadyPass = true;
                StartCoroutine(CoroutineIsChooseForExpedition());

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
                            Debug.Log("propose expedition");
                            gameManager.ProposeExpedition(door_idPlayer);
                            gameManager.ui_Manager.DisplayMainLevers(false);
                            gameManager.gameManagerNetwork.SendDisplayMainLevers(false);

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
                            gameManager.gameManagerNetwork.SendDisplayMainLevers(false);
                           

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
            if (displayMap && GetComponent<PhotonView>().IsMine && !displayChatInput)
            {
                if (gameManager.setting.DISPLAY_MINI_MAP || gameManager.hellIsFind || gameManager.paradiseIsFind)
                {
                    gameManager.ui_Manager.DisplayMap();
                    displayMap = false;

/*                    if (gameManager.ui_Manager.map.activeSelf)
                    {
                        Camera.main.orthographicSize = 3f;
                    }
                    else
                    {
                        Camera.main.orthographicSize = resolution.currentOrthographicSize;
                    }*/
                }
                else
                {
                    if (isImpostor || gameManager.paradiseIsFind || gameManager.hellIsFind)
                    {
                        gameManager.ui_Manager.DisplayMap();
                        displayMap = false;
                    }
                }
            }
            if (displayTutorial)
            {
                if (gameManager.game.currentRoom.isSpecial)
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
                this.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
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
            }
            else
            {
                DisplayNamePlayer(true);
            }
        }
    }

    public void SetZIndexByPositionY()
    {
        for (int i = 0; i < this.transform.GetChild(1).GetChild(1).childCount; i++)
        {
            if (this.transform.GetChild(1).GetChild(1).GetChild(i).gameObject.activeSelf)
            {
                this.transform.GetChild(1).GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = ((int)(this.transform.position.y * 10)) * -1;
            }
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
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

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



    private void handlePlayerMove()
    {
       
        handleDesktopMove();
        handleMobileMove();
      
    }

    private void handleDesktopMove()
    {
        turnPlayer();


        //this.GetComponent<BoxCollider2D>().enabled = true;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
        oldPosition = transform.position;
        float horizontal = InputManager.GetAxis("Horizontal");
        float vertical = InputManager.GetAxis("Vertical");
/*        if(Mathf.Abs(InputManager.GetAxis("Horizontal")) + Mathf.Abs(InputManager.GetAxis("Vertical")) > 1f)
        {
            horizontal = 0.5f * Mathf.Sign(InputManager.GetAxis("Horizontal"));
            vertical = 0.5f * Mathf.Sign(InputManager.GetAxis("Vertical")); ;
        }*/
        this.transform.Translate(
            new Vector3(
                horizontal * movementlControlSpeed * 1.3f * Time.deltaTime,
                vertical * movementlControlSpeed * 1.3f * Time.deltaTime,
                0
            )
        );
/*        if(horizontal > 0 || vertical > 0)
            movement = transform.position - oldPosition;*/


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
        Transform perso = transform.Find("Perso");
        if (perso.localScale.x < 0)
        {
            chat.localPosition = new Vector3(561, 182);
            chat.Find("NormalChat").Find("ChatPanel").gameObject.SetActive(true);
            chat.Find("NormalChat").Find("ChatLeft").gameObject.SetActive(false);
        }
        else
        { 
            chat.localPosition = new Vector3(-223, 182);
            chat.Find("NormalChat").Find("ChatPanel").gameObject.SetActive(false);
            chat.Find("NormalChat").Find("ChatLeft").gameObject.SetActive(true);
        }
       


    }

    private void turnLeft()
    {
        Transform perso = transform.Find("Perso");
        perso.localScale = new Vector3(
            Mathf.Abs(perso.localScale.x),
            perso.localScale.y
        );
       
    }

    private void turnRight()
    {
        Transform perso = transform.Find("Perso");
        perso.localScale = new Vector3(
            -Mathf.Abs(perso.localScale.x),
            perso.localScale.y
        );
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager && gameManager.fireBallIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isTouchByFireBall)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            if (gameManager && gameManager.deathNPCIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isTouchByDeath)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            if(gameManager && gameManager.damoclesIsLaunch && !collision.gameObject.GetComponent<PlayerGO>().isDeadBySwordDamocles)
            {
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
                return;
            }
            Physics2D.IgnoreCollision(collision.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>());
        }

    }

    public void IgnoreCollisionAllPlayer(bool ignore)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if(GetPlayerMineGO().GetComponent<PhotonView>().IsMine  )
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
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
                this.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
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
                GetComponent<PlayerNetwork>().SendCollisionDoorIndex(collisionInDoorIndex);
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

       
        launchExpeditionWithAnimation = true;

  
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
            gameManager.InstantiateDeathNPC();
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

        if (gameManager.game.currentRoom.IsVirus && gameManager.game.key_counter > 0)
        {
            launchVoteDoorMobile = true;
        }

        gameManager.gameManagerNetwork.SendCloseDoorWhenVote();
        gameManager.gameManagerNetwork.SendDisplaySpeciallyLevers(false, 0);
        //gameManager.ui_Manager.DisplaySpeciallyLevers(false, 0);
    }





    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        if (collision.CompareTag("Zone_vote"))
        {
            this.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
            GetComponent<PlayerNetwork>().SendHideVoteExplorationDisplay();
        }
        if (collision.CompareTag("Door"))
        {
            collisionInDoorIndex = -1;
            GetComponent<PlayerNetwork>().SendCollisionDoorIndex(-1);
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

    public void DisplayCharacter(bool display)
    {
        GetComponent<PlayerNetwork>().SendDisplayCharacter(display);

        if (display)
        {
            if (gameManager.SamePositionAtBossWithIndex(this.GetComponent<PhotonView>().ViewID))
                transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            return;
        }
        if (gameManager.SamePositionAtBossWithIndex(this.GetComponent<PhotonView>().ViewID))
            transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }

    public void SetPlayerNameServer()
    {
        if (playerNetwork)
        {
            playerNetwork.SendNamePlayer(playerName);

            if (GetComponent<PhotonView>().IsMine)
            {
                playerNetwork.SendindexSkin(indexSkin);
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
        if (gameManager.GetPlayerMine().GetIsImpostor() && this.GetComponent<PhotonView>().IsMine)
        {
            foreach(GameObject impostor in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(impostor.GetComponent<PlayerGO>().isImpostor)
                    impostor.transform.GetChild(1).GetChild(2).gameObject.SetActive(isImpostor);
            }
           
        }
    }


    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("OpenEyes", false);
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
        for (int i = 0; i < 9; i++)
        {
            transform.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
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

        if (gameManager.timer.timerLaunch)
        {
            return;
        }
        if (gameManager.expeditionHasproposed)
        {
            return;
        }

        if (gameManager.ui_Manager.map.activeSelf)
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
        if (gameManager.game.currentRoom.isSword)
        {
            return;
        }
        if (gameManager.game.currentRoom.isSwordDamocles)
        {
            return;
        }
        if (gameManager.game.currentRoom.isDeathNPC)
        {
            return;
        }
        if (gameManager.game.currentRoom.fireBall)
        {
            return;
        }
        if (gameManager.game.currentRoom.isAx)
        {
            return;
        }
        if (gameManager.speciallyIsLaunch)
            return;
        GetComponent<PlayerNetwork>().SendOnclickToExpedition();
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
        transform.Find("Perso").Find("Light_red").gameObject.SetActive(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice);
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
        /*        gameManager.GetPlayerMineGO().transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                gameManager.GetPlayerMineGO().transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = text.text;*/
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

/*        if (text.Length > 4 && threeFirstLetter.Equals("/all"))
        {
            string textWithoutSlash = text.Substring(4, text.Length - 4);
            if (textWithoutSlash == "" || textWithoutSlash == " " || textWithoutSlash == "  ")
            {
                return;
            }
            GetComponent<PlayerNetwork>().SendTextChat(indexPlayer, textWithoutSlash);
        }
        else
        {
            if (GetComponent<PlayerGO>().isImpostor)
            {
                GetComponent<PlayerNetwork>().SendTextChatToImpostor(indexPlayer, text);
            }
            else
            {
                GetComponent<PlayerNetwork>().SendTextChat(indexPlayer, text);
            }

        }*/

        if (GetComponent<PlayerGO>().isImpostor && threeFirstLetter.Equals("/imp") && text.Length > 3   )
        {
            string textWithoutSlash = text.Substring(4, text.Length -4);
            if (textWithoutSlash == "" || textWithoutSlash == " " || textWithoutSlash == "  ")
            {
                return;
            }
            //GetComponent<PlayerNetwork>().SendTextChat(indexPlayer, textWithoutSlash);
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
        //GetComponent<PlayerNetwork>().SendDisplayMessage(true);
        displayMessage = true;
        yield return new WaitForSeconds(time);
        currentlyMessageDisplay.RemoveAt(index - (count - 1));
        if (currentlyMessageDisplay.Count == 0)
            //GetComponent<PlayerNetwork>().SendDisplayMessage(false);
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
        if (gameManager && (gameManager.expeditionHasproposed || gameManager.timer.timerLaunch || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed))
        {
            playerNetwork.SendResetWantToChangeBoss();
            changeBoss = false;
            return;
        }
        if (gameManager && gameManager.fireBallIsLaunch)
        {
            playerNetwork.SendResetWantToChangeBoss();
            changeBoss = false;
            return;
        }
        if (displayChatInput)
        {
            return;
        }
        if (isBoss)
        {
            playerNetwork.SendResetWantToChangeBoss();
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


        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
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
            //Debug.Log(player.GetComponent<PhotonView>().ViewID);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                //Debug.Log(player.GetComponent<PhotonView>().ViewID);
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
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }

        canDisplayMap = isEnter;
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.mobileCanvas.transform.Find("Map_panel").gameObject.SetActive(isEnter);

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
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
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
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(false);
            return;
        }
        if(collision.name != "Exploration_lever")
        {
            return;
        }
        canLaunchExplorationLever = isEnter;
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
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
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
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
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
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
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
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
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
        gameManager.ui_Manager.DisplayUI_Mobile_SpecialRoom(isEnter);
    }

    public void CanChangeBoss(GameObject collision, bool isEnter)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (isBoss)
        {
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            return;
        }
        if (gameManager.expeditionHasproposed || gameManager.timer.timerLaunch || gameManager.voteDoorHasProposed || gameManager.voteChestHasProposed){
            transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(false);
            gameManager.ui_Manager.mobileCanvas.transform.Find("Change_Boss").gameObject.SetActive(false);
            return;
        }
        if (hasWinFireBallRoom)
        {
            return;
        }

        DisplayTutorial(22);
  

        canLaunchChangeBoss = isEnter;
        transform.Find("ActivityCanvas").Find("E_inputImage").gameObject.SetActive(isEnter);
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
        if (this.lifeTrialRoom == 2)
        {
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
        this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(false);
        this.transform.Find("Life").Find("OneHeart").gameObject.SetActive(false);
    }
    public void DisiplayHeartInitial(bool display)
    {
        this.transform.Find("Life").Find("TwoHeart").gameObject.SetActive(display);
    }

    public IEnumerator ResetInvincibleCouroutine()
    {
        yield return new WaitForSeconds(1);
        this.isInvincible = false;
    }
}
