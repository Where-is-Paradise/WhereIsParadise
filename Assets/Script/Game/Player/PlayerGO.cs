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

    private PlayerNetwork playerNetwork;

    public bool isBoss = false;
    public bool isImpostor;

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
    private bool touchStart = false;

    public bool launchVoteDoorMobile = false;
    private bool launchExpeditionMobile = false;
    public bool launchExpeditionWithAnimation = false;
    private float timeStayTouch_voteDoor = 2f;
    private float timeStayTouch_expediton = 4f;
    private float timeStayTouch = 0;

    private bool takeDoor = false;
    private bool isCollisionInDoorTakeDoor = false;
    private bool isCollisionInDoorBackExpedition = false;
    private bool isCollisionInDoorExpedition = false;
    public bool collisionParadise = false;
    public bool collisionHell = false;

    private bool takeDoorBackExpedition = false;
    private bool takeDoorExpededition = false;
    private GameObject doorCollision = null;

    private bool isMoving = false;
    public bool animateEyes = false;
    private bool isFirstTouch = false;
    private int TapCount;
    public bool comeToParadise = false;
    public int indexSkin = 0;
    public bool ui_isOpen = false;

    private float horizontalInput = 0;
    private float verticalInput = 0;

    public bool isChooseForExpedition = false;
    public bool isAlreadyHide = false;

    public bool displayChatInput = false;

    private bool displayMessage = false;
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

    public GameObject chatPanel;

    private void Awake()
    {
        displayChatInput = false;
        playerNetwork = gameObject.GetComponent<PlayerNetwork>();
        currentlyMessageDisplay = new List<string>();
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        setIsBoss();

        AnimateEyes();

        setCollider();

        // Used to prevent multi tap on mobile
        TapCount = 0;

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
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    private void FixedUpdate()
    {
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

            // change isBoss and isImpostor of all gameObject player
            foreach (PlayerDun player in gameManager.game.list_player)
            {
                if (player.GetId() == this.GetComponent<PhotonView>().ViewID)
                {
                    isBoss = player.GetIsBoss();
                    isImpostor = player.GetIsImpostor();
                }
            }

            SetSkinBoss(isBoss);
            SetSkinImpostor(isImpostor);

            if (gameManager.timer.timerFinish && !gameManager.alreadyPass)
            {
                this.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
                gameManager.alreadyPass = true;
                StartCoroutine(CoroutineIsChooseForExpedition());

            }

            /*            if (!isChooseForExpedition)
                        {
                            transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = "";
                        }*/
        }

        gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerName;

        if (GetComponent<PhotonView>().IsMine && isBoss && gameManager)
        {
            /*            ui_isOpen = gameManager.ui_Manager.echap_menu.activeSelf;
                        if (ui_isOpen)
                        {
                            return;
                        }*/

            if (!gameManager.expeditionHasproposed && !gameManager.voteDoorHasProposed)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                    InputExplorationAnimation();
            }

            // Input Exploration
            // ||  IsDoubleTap()
            if (launchExpeditionWithAnimation || gameManager.launchExpedtion_inputButton)
            {
                if (!gameManager.paradiseIsFind && !gameManager.hellIsFind)
                {
                    if (!gameManager.alreaydyExpeditionHadPropose && gameManager.game.key_counter > 0 && !gameManager.DoorParadiseOrHellisOpen)
                    {
                        Dictionary<int, int> door_idPlayer = gameManager.SetPlayerNearOfDoor();

                        if (gameManager.VerificationExpedition(door_idPlayer))
                        {
                            gameManager.ProposeExpedition(door_idPlayer);

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
                    InputVoteDoorAnimation();
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

        if (InputManager.GetButtonDown("Enter") && GetComponent<PhotonView>().IsMine)
        {
            displayChatInput = !displayChatInput;
            if (!displayChatInput)
            {
                SetTextChat(chatPanel.transform.GetChild(1).GetComponent<InputField>().text);
            }
            DisplayChat(displayChatInput);

        }

        if (gameManager)
        {
            if (InputManager.GetButtonDown("Map") && GetComponent<PhotonView>().IsMine && !displayChatInput)
            {
                if (gameManager.setting.DISPLAY_MINI_MAP || gameManager.hellIsFind || gameManager.paradiseIsFind)
                {
                    gameManager.ui_Manager.DisplayMap();

                    if (gameManager.ui_Manager.map.activeSelf)
                    {
                        Camera.main.orthographicSize = 5.1f;
                    }
                    else
                    {
                        Camera.main.orthographicSize = resolution.currentOrthographicSize;
                    }
                }
                else
                {
                    if (isImpostor || gameManager.paradiseIsFind || gameManager.hellIsFind)
                    {
                        gameManager.ui_Manager.DisplayMap();
                    }
                }
            }





            if (GetComponent<PhotonView>().IsMine && gameManager.timer.timerFinish && !gameManager.GetPlayerMine().GetHasVoted_CP() && !gameManager.voteDoorHasProposed && !gameManager.isLoading)
            {
                if (!isCollisionInDoorTakeDoor && !isCollisionInDoorBackExpedition && !isCollisionInDoorExpedition)
                {
                    gameManager.VoteCP(vote_cp);
                    gameManager.GetPlayerMine().SetHasVoted_CP(true);
                    vote_cp = 0;
                }
            }
        }
        // Mobile moving character

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            pointA = Camera.main.ScreenToWorldPoint(touch.position);

        }

        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Stationary))
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


        if (animateEyes)
        {
            this.transform.GetChild(1).GetChild(0).GetComponent<Animator>().SetBool("OpenEyes", true); ;
            animateEyes = false;
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


        if (displayMessage)
        {
            this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }

        SetZIndexByPositionY();
        ActionnWantToChangeBoss();
        LaunchVoteChest();

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


    public static bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait;

#if UNITY_IOS
        MaxTimeWait = 0.07f;
#else
        MaxTimeWait = 0.01f;
#endif

        float VariancePosition = 1f;

        if (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }



    public void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Zone_vote"))
        {
            this.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);

            if (collision.name == "vote_X")
            {
                vote_cp = -1;
            }
            else if (collision.name == "vote_V")
            {
                vote_cp = 1;
            }
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

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);

        this.transform.Translate(
            new Vector3(
                InputManager.GetAxis("Horizontal") * movementlControlSpeed * 1.3f * Time.deltaTime,
                InputManager.GetAxis("Vertical") * movementlControlSpeed * 1.3f * Time.deltaTime,
                0
            )
        );
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
            Physics2D.IgnoreCollision(collision.transform.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>());
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("teleport_changeRoom"))
        {
            if (GetComponent<PhotonView>().IsMine)
            {

                if (collision.transform.parent.GetComponent<Door>().isOpenForAll && !haveToGoToExpedition)
                {
                    if (!gameManager.timer.timerLaunch)
                    {

                        gameManager.timer.LaunchTimer(1f, false);
                        isCollisionInDoorTakeDoor = true;
                        doorCollision = collision.gameObject;
                        canMove = false;
                        gameManager.ui_Manager.DisplayBlackScreen(true, true);
                    }

                }
                else
                {
                    if (isInExpedition)
                    {
                        gameManager.timer.LaunchTimer(1f, false);
                        isCollisionInDoorBackExpedition = true;
                        doorCollision = collision.gameObject;
                        canMove = false;
                        gameManager.ui_Manager.DisplayBlackScreen(true, true);
                    }
                    else
                    {
                        gameManager.timer.LaunchTimer(1f, false);
                        isCollisionInDoorExpedition = true;
                        doorCollision = collision.gameObject;
                        canMove = false;
                        gameManager.ui_Manager.DisplayBlackScreen(true, true);
                        collisionToGost = false;
                    }
                }
            }
        }

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
                Physics2D.IgnoreCollision(collision.transform.GetComponent<CircleCollider2D>(), this.GetComponent<BoxCollider2D>(), true);
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
                    vote_cp = -1;
                }
                else if (collision.name == "vote_V")
                {
                    vote_cp = 1;
                }

            }
            if (collision.CompareTag("Door"))
            {
                collisionInDoorIndex = collision.GetComponent<Door>().index;
                GetComponent<PlayerNetwork>().SendCollisionDoorIndex(collisionInDoorIndex);
            }
        }



    }



    public void InputExplorationAnimation()
    {
        if (InputManager.GetButtonDown("Exploration"))
        {
            timeStayTouch = 0;
        }
        if (InputManager.GetButton("Exploration"))
        {
            timeStayTouch += Time.deltaTime;

            if (timeStayTouch > 0.5f)
            {
                canMove = false;
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(true);
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().Play("animation_zone_circle_tomobile");
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().speed = 6f;
                gameManager.ui_Manager.Display_identificationExpedition(true);
                gameManager.ui_Manager.DisplayKeyAndTorch(false);
                gameManager.ui_Manager.HideDistanceRoom();
            }
            if (timeStayTouch > 1.5f)
            {
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
                launchExpeditionWithAnimation = true;
                timeStayTouch = 0;
                canMove = true;
                gameManager.ui_Manager.Display_identificationExpedition(false);
                gameManager.ui_Manager.DisplayKeyAndTorch(true);
                if (gameManager.SamePositionAtInitialRoom())
                    gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.DistancePathFinding, null);
            }
        }
        if (InputManager.GetButtonUp("Exploration"))
        {
            gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
            timeStayTouch = 0;
            //isFirstTouch = false;
            //canMove = true;
            canMove = true;
            canMove = true;
            gameManager.ui_Manager.Display_identificationExpedition(false);
            gameManager.ui_Manager.DisplayKeyAndTorch(true);
            if (gameManager.SamePositionAtInitialRoom())
                gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.DistancePathFinding, null);
        }


    }

    public void InputVoteDoorAnimation()
    {
        if (InputManager.GetButtonDown("VoteDoor"))
        {
            timeStayTouch = 0;
        }
        if (InputManager.GetButton("VoteDoor"))
        {
            timeStayTouch += Time.deltaTime;

            if (timeStayTouch > 0.5f)
            {
                canMove = false;
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(true);
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().Play("animation_zone_circle_tomobile");
                gameManager.ui_Manager.zoneX_startAnmation.GetComponent<Animator>().speed = 6f;
                gameManager.ui_Manager.Display_identificationVoteDoor(true);
                gameManager.ui_Manager.DisplayKeyAndTorch(false);
                gameManager.ui_Manager.HideDistanceRoom();
            }
            if (timeStayTouch > 1.5f)
            {
                gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
                launchVoteDoorMobile = true;
                timeStayTouch = 0;
                canMove = true;
                gameManager.ui_Manager.Display_identificationVoteDoor(false);
                gameManager.ui_Manager.DisplayKeyAndTorch(true);
                if (gameManager.SamePositionAtInitialRoom())
                    gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.DistancePathFinding, null);
            }
        }
        if (InputManager.GetButtonUp("VoteDoor"))
        {
            gameManager.ui_Manager.zoneX_startAnmation.SetActive(false);
            timeStayTouch = 0;
            //isFirstTouch = false;
            //canMove = true;
            canMove = true;
            gameManager.ui_Manager.Display_identificationVoteDoor(false);
            gameManager.ui_Manager.DisplayKeyAndTorch(true);
            if (gameManager.SamePositionAtInitialRoom())
                gameManager.ui_Manager.SetDistanceRoom(gameManager.game.dungeon.initialRoom.DistancePathFinding, null);
        }
    }

    public void LaunchVoteChest()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!Input.GetKeyUp(KeyCode.G))
        {
            return;
        }
        gameManager.gameManagerNetwork.SendActiveZoneVoteChest();
       
    }


    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zone_vote"))
        {
            this.transform.GetChild(1).GetChild(4).gameObject.SetActive(false);
            vote_cp = 0;
        }

        if (collision.CompareTag("Door"))
        {
            collisionInDoorIndex = -1;
            GetComponent<PlayerNetwork>().SendCollisionDoorIndex(-1);
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
        if (gameManager.GetPlayerMine().GetIsImpostor())
        {
            this.transform.GetChild(1).GetChild(2).gameObject.SetActive(isImpostor);
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
                // afficher panel explication comme quoi on peut pas echainer les explorations
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

            GetComponent<PlayerNetwork>().SendOnclickToExpedition();
        }

    }

    public void DisplayChat(bool display)
    {
        if (GetComponent<PhotonView>().IsMine && !display)
        {
            displayChatInput = false;
        }
        else
        {
            chatPanel.transform.GetChild(1).GetComponent<InputField>().text = "";
        }
        chatPanel.SetActive(display);
        chatPanel.transform.GetChild(1).GetComponent<InputField>().ActivateInputField();

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

        if (text.Length > 4 && threeFirstLetter.Equals("/all"))
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

        }
    }


    public void SendMessagePlayerInTimes()
    {
        currentlyMessageDisplay.Add("tkt");
        StartCoroutine(CouroutineTextChat(currentlyMessageDisplay.Count - 1, currentlyMessageDisplay.Count));
    }

    public IEnumerator CouroutineTextChat(int index, int count)
    {
        displayMessage = true;
        yield return new WaitForSeconds(3);
        currentlyMessageDisplay.RemoveAt(index - (count - 1));
        if (currentlyMessageDisplay.Count == 0)
            displayMessage = false;
    }

    public IEnumerator CoroutineIsChooseForExpedition()
    {
        yield return new WaitForSeconds(0.5f);
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
        if (displayChatInput)
        {
            return;
        }
        if (isBoss)
        {
            playerNetwork.SendResetWantToChangeBoss();
            return;
        }
        if (!Input.GetKeyDown(KeyCode.T))
        {
            return;
        }
        playerNetwork.SendWantToChangeBoss();

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


}
