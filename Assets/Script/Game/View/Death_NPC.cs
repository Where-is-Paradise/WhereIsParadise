using Pathfinding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_NPC : MonoBehaviourPun
{
    public GameManager gameManager;
    public DeathNpcRoom deathNPC_Room;
    public bool CanHideDeathGod = false;
    public bool isInvisible = false;
    public bool isTranparencying = false;
    public bool isInvertTranparencying = false;
    public float tranparency = 255;
    private float oldHorizontal;
    private float oldVertical;
    public int index;
   

    private Vector2 direction = new Vector2(0, 0);
    private bool canDash = false;
    private bool canDashTarget = false;
    private bool canCircleDash = false;
    private bool canTransition = false;

    private Vector3 target = new Vector3(0,0,0);

    private float old_x = 0;
    private float old_y = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        deathNPC_Room = GameObject.Find("DeathNPCRoom").GetComponent<DeathNpcRoom>();
        
/*        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.TeleportAllPlayerInRoomOfBoss();
            StartCoroutine(StartDeathNPCRoomAfterTeleportation());
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        ChangeScaleForSituation();
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (!gameManager.deathNPCIsLaunch)
        {
            return;
        }
        gameManager.CloseDoorWhenVote(true);
        if (canDash)
        {
            DashDirection();
        }
        if (canCircleDash)
        {
            DashCircle();
        }
        if (canDashTarget)
            DashDirectionTarget(target);
        CalculVelocity();
    }

    public void ChangeTransparencyToInvisibilty()
    {
        if (isInvisible && !isTranparencying)
        {
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency / 255);
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency / 255);
            tranparency = (tranparency - 2f);
            if (tranparency < 0)
            {
                isTranparencying = true;
                isInvertTranparencying = false;
            }


        }
        if (!isInvisible && !isInvertTranparencying)
        {
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency / 255);
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency / 255);
            tranparency = (tranparency + 2f);
            if (tranparency > 255)
            {
                isInvertTranparencying = true;
                isTranparencying = false;
            }

        }
    }




    public IEnumerator SetIsInvisibleCoroutine()
    {
        if (this.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            photonView.RPC("SendIsInvisible", RpcTarget.All, true);
            int randomInt = Random.Range(2, 7);
            yield return new WaitForSeconds(randomInt);
            StartCoroutine(SetNotInvisibleCoroutine());
        }     
    }
    public IEnumerator SetNotInvisibleCoroutine()
    {
        if (this.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            photonView.RPC("SendIsInvisible", RpcTarget.All, false);
            yield return new WaitForSeconds(1.2f);
            StartCoroutine(SetIsInvisibleCoroutine());
        }
    }

    [PunRPC]
    public void SendIsInvisible(bool isInvisible)
    {
        this.isInvisible = isInvisible;
    }

    public IEnumerator CanHideDeathGodCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        CanHideDeathGod = true;  
    }

    public void ChangeScaleForSituation()
    {
        
        if (direction.x >= 0.01f)
        {
            this.transform.localScale = new Vector2(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
        else if (direction.x <= -0.01f)
        {
            this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
    }

    public GameObject GetPlayerWithMinDistance()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject playerWithMinDistance = listPlayer[0];
        float minDistance = 1000;
        foreach(GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || player.GetComponent<PlayerGO>().isSacrifice 
                || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID) || player.GetComponent<PlayerGO>().isInJail)
            {
                Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), true);
                continue;
            }
            float subX = Mathf.Abs(player.transform.position.x - this.transform.position.x);
            float subY = Mathf.Abs(player.transform.position.y - this.transform.position.y);
            float distance = (subX + subY) / 2;
            if(distance < minDistance)
            {
                minDistance = distance;
                playerWithMinDistance = player;
            }
        }

        return playerWithMinDistance;
    }

    public void SetTargetOfPathFinding()
    {
        GameObject playerTarget = GetPlayerWithMinDistance();
        SetIndexTarget(playerTarget.GetComponent<PhotonView>().ViewID);
        //SetMaxSpeed(4f);
    }

    public IEnumerator ChangerSpeedCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (gameManager.deathNPCIsLaunch)
        {
            float newSpeed = this.GetComponent<AIPath>().maxSpeed + 0.1f;
            SetMaxSpeed(newSpeed);
            StartCoroutine(ChangerSpeedCoroutine());
        }
        else
        {
            this.GetComponent<AIPath>().maxSpeed = 0;
        }
       
    }

/*    public IEnumerator UpdatePositionCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        SendUpdatePosition(this.transform.position.x, this.transform.position.y);
        StartCoroutine(UpdatePositionCoroutine());
    }*/

    public void SendUpdatePosition(float x, float y)
    {
        photonView.RPC("SetUpdatePosition", RpcTarget.All, x, y);
    }
    [PunRPC]
    public void SetUpdatePosition(float x, float y)
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        this.transform.position = new Vector2(x, y);
    }

    public void SendMaxSpeed(float maxSpeed)
    {
        photonView.RPC("SetMaxSpeed", RpcTarget.All, maxSpeed);
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        this.GetComponent<AIPath>().maxSpeed = maxSpeed;
    }

    public void SendIndexTarget(int indexTarget)
    {
        photonView.RPC("SetIndexTarget", RpcTarget.All, indexTarget);
    }

    public void SetIndexTarget(int indexTarget)
    {
        this.GetComponent<AIPath>().destination = gameManager.GetPlayer(indexTarget).transform.position;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(false);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameManager || !gameManager.deathNPCIsLaunch)
        {
            return;
        }
        if(collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Door")
        {
            if (canDash)
            {
                DashStraight();
            }
        }
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
                return;
            SetPlayerColor(collision.gameObject);
            photonView.RPC("DeathTouchPlayerEvent", RpcTarget.All, collision.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    public void DeathTouchPlayerEvent(int indexPlayer)
    {
        SendIgnoreCollisionPlayer(true);
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isTouchInTrial = true;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerNetwork>().SendIstouchInTrial(true);

        if (TestLastPlayer())
        {
           
            photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
            photonView.RPC("SendLooseGame", RpcTarget.All);
            //StartCoroutine(CouroutineDesactivateAll());
            gameManager.deathNPCIsLaunch = false;
            deathNPC_Room.SendHideTimer();
            deathNPC_Room.SendDesactivateNPC();
        }  
    }

    [PunRPC]
    public void SendIgnoreCollisionPlayer(bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(ignore);
    }

    [PunRPC]
    public void SendLooseGame()
    {
        deathNPC_Room.DesactivateRoom();
        deathNPC_Room.loose = true;
        deathNPC_Room.ReactivateCurrentRoom();
        gameManager.ui_Manager.DisplayLeverVoteDoor(true);
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            gameManager.RandomWinFireball();
        }
        if (TestLastPlayer())
        {
            GameObject lastPlayer = GetLastPlayer();
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
            photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
            deathNPC_Room.SendDesactivateNPC();
            photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
            StartCoroutine(CouroutineDesactivateAll());
            gameManager.deathNPCIsLaunch = false;
        }
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }

    public void SetPlayerColor(GameObject collision)
    {
        //collision.gameObject.GetComponent<PlayerNetwork>().SendIstouchByDeath(true);
        collision.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach(GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length ) )
            return true;
        return false;
    }
    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == listPlayer.Length)
            return true;
        return false;
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PlayerGO>().isTouchInTrial && !player.GetComponent<PlayerGO>().isSacrifice
                && gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID) && !player.GetComponent<PlayerGO>().isInJail)
            {
                return player;
            }
        }
        return null;


    }

    public void SendResetColor()
    {
        photonView.RPC("ResetColorAllPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void ResetColorAllPlayer()
    {
        if (!gameManager.SamePositionAtBoss())
            return;

        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)){
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
        }
    }

    [PunRPC]
    public void DesactivateNPC()
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.ChangeLeverDeathNPC(); 
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        gameManager.speciallyIsLaunch = false;
        gameManager.ActivateCollisionTPOfAllDoor(true);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(false);
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        this.gameObject.SetActive(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
           

    }

    public void SendHideAndResetNPC()
    {
        photonView.RPC("HideAndResetNPC", RpcTarget.All);
    }

    [PunRPC]
    public void HideAndResetNPC()
    {
        gameManager.deathNPCIsLaunch = false;
        if (!gameManager.SamePositionAtBoss())
            return;
        this.gameObject.transform.Find("Faux").gameObject.SetActive(false);
        this.gameObject.transform.Find("body_gfx").gameObject.SetActive(false);
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        this.transform.position = new Vector3(0, 0, 0);
        this.GetComponent<AIPath>().destination = new Vector3(0, 0, 0);
        this.GetComponent<AIPath>().maxSpeed = 0;
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            PhotonNetwork.Destroy(this.gameObject);
       
    }

    public IEnumerator CouroutineDesactivateAll()
    {
        yield return new WaitForSeconds(0.5f);
        SendResetColor();
        
        photonView.RPC("DesactivateNPC", RpcTarget.All);
    }

    public void ChangeSizeCollision()
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            GetComponent<CapsuleCollider2D>().size = new Vector2(1.57f, 1.7f);
        }
        else
        {
            GetComponent<CapsuleCollider2D>().size = new Vector2(2.7f, 1.7f);
        }
    }

    public IEnumerator SendPostionCouroutine()
    {
        yield return new WaitForSeconds(0.75f);
        SendPosition(this.transform.position.x, this.transform.position.y);

        StartCoroutine(SendPostionCouroutine());
    }
    public void SendPosition(float x, float y)
    {
        photonView.RPC("SetPosition", RpcTarget.Others,x,y);
    }
    [PunRPC]
    public void SetPosition(float x, float y)
    {
        this.transform.position = new Vector3(x, y);

    }


    public void ChangeDirectionBrutally()
    {
        float horizontal = GetComponent<Rigidbody2D>().velocity.y;
        float vertical = GetComponent<Rigidbody2D>().velocity.x;

        if (((oldHorizontal == 0 && Mathf.Abs(horizontal) > 0) || (oldHorizontal > 0 && horizontal < 0) || (oldHorizontal < 0 && horizontal > 0))
            || (oldVertical == 0 && Mathf.Abs(vertical) > 0 || (oldVertical > 0 && vertical < 0) || (oldVertical < 0 && vertical > 0)))
        {
            SendPosition(this.transform.position.x, this.transform.position.y);
        }
        oldHorizontal = horizontal;
        oldVertical = vertical;
    }

    public IEnumerator RandomScenario()
    {
        // 0 dash
        // circle dash
        // teleportation
        // invisibilité

        int indexScenario = Random.Range(0, 3);
        //Debug.Log(indexScenario);
        //indexScenario = 1;
        Debug.Log("SCENARIO :  " + (indexScenario+1));
        if (indexScenario == 0)
        {
            DashStraight();
            canDash = true;
            yield return new WaitForSeconds(6);
            canDash = false;
        }
        if (indexScenario == 1)
        {
            //canDashTarget = true;
            timeCounter = 0;
            randomX = Random.Range(-5, 4);
            initialTimeCounter = Random.Range(0f, 10f);
            width = Random.Range(1f, 5.5f);
            height = Random.Range(0.5f, 3.75f);
            target = GetFuturePositionOfDashCircle();
            canDashTarget = true;
            yield return new WaitForSeconds(6);
            canCircleDash = false;

        }
        if(indexScenario == 2)
        {
            StartCoroutine(Teleportation());
            yield return new WaitForSeconds(8);
        }
        StartCoroutine(RandomScenario());

    }

    // nouvelle mise a jour de ce mini jeu
    public void DashStraight()
    {
        bool canDashUp = true;
        bool canDashDown = true;
        bool canDashRight = true;
        bool canDashLeft = true;

        if (transform.position.x <= -5)
            canDashLeft = false;
        if (transform.position.x >= 4.75f)
            canDashRight = false;
        if (transform.position.y <= -2)
            canDashDown = false;
        if (transform.position.y >= 1.45)
            canDashUp = false;

        List<int> listScenario = new List<int>();

        if (canDashUp && canDashRight)
        {
            listScenario.Add(0);
        }
        if ( canDashUp && canDashLeft)
        {
            listScenario.Add(1);
        }
        if(canDashDown && canDashRight)
        {
            listScenario.Add(2);
        }
        if ( canDashDown && canDashLeft)
        {
            listScenario.Add(3);
        }
        int randomIndexScenario = Random.Range(0, listScenario.Count);



        if(listScenario[randomIndexScenario] == 0)
        {
            float dash_X = Random.Range(this.transform.position.x + 3, 10);
            float dash_Y = Random.Range(this.transform.position.y + 3, 7);
            direction = new Vector2(dash_X - this.transform.position.x , dash_Y - this.transform.position.y );;
        }
        if(listScenario[randomIndexScenario] == 1)
        {
            float dash_X = Random.Range(this.transform.position.x - 3, -10);
            float dash_Y = Random.Range(this.transform.position.y + 3, 7);
            direction = new Vector2(dash_X - this.transform.position.x , dash_Y - this.transform.position.y);
        }
        if(listScenario[randomIndexScenario] == 2)
        {
            float dash_X = Random.Range(this.transform.position.x + 3, 10);
            float dash_Y = Random.Range(this.transform.position.y - 3, -7);
            direction = new Vector2(dash_X - this.transform.position.x, dash_Y - this.transform.position.y);
        }
        if(listScenario[randomIndexScenario] == 3)
        {
            float dash_X = Random.Range(this.transform.position.x - 3, -10);
            float dash_Y = Random.Range(this.transform.position.y - 3, -7);
            direction = new Vector2(dash_X - this.transform.position.x, dash_Y - this.transform.position.y );
        }
        //Dash(randomIntDirection);
    }

    public void DashDirection()
    {
        old_x = this.transform.position.x;
        old_y = this.transform.position.y;
        transform.Translate(direction.normalized * 7 * Time.deltaTime);
    }
    public void DashDirectionTarget(Vector3 target)
    {
        old_x = this.transform.position.x;
        old_y = this.transform.position.y;
        transform.Translate((target - this.transform.position).normalized * 5 * Time.deltaTime);

        if(Mathf.Abs(this.transform.position.x - target.x) <0.01 && Mathf.Abs(this.transform.position.y - target.y) < 0.01)
        {
            canCircleDash = true;
            canDashTarget = false;
        }

    }

    float timeCounter = 0;
    float speed = 2.5f;
    float width = 1;
    float height = 1;
    float initialTimeCounter = 0;
    float randomX = 0;
    
    public void DashCircle()
    {
        Vector2 circlePosition = GetFuturePositionOfDashCircle();
        old_x = this.transform.position.x;
        old_y = this.transform.position.y;
        transform.position = new Vector3(circlePosition.x, circlePosition.y);

       
    }

    public Vector2 GetFuturePositionOfDashCircle()
    {
        /*        float x = Mathf.Cos(Mathf.Acos(this.transform.position.x / width)) * width;
                float y = Mathf.Sin(Mathf.Acos(this.transform.position.x / width)) * height;*/

        timeCounter += Time.deltaTime * speed;
        float x = Mathf.Cos(timeCounter) * width;
        float y = Mathf.Sin(timeCounter) * height;
        x +=  randomX;
        if (y > 2)
            y = 2;
        if (x < -6)
            x = -6;
        if (x > 6.2f)
            x = 6.2f;
        if (y < -3.5f)
            y = -3.5f;
        return new Vector2(x, y);
    }

    public void TransitionDashCircle(Vector3 target)
    {
        float x = target.x - this.transform.position.x;
        float y = target.y - this.transform.position.y;
        this.transform.Translate(new Vector3(x,y) * 6 * Time.deltaTime);
        if (Mathf.Abs(x) + Mathf.Abs(y) < 0.5f)
            canTransition = false;
    }

    int counterTeleporation = 0;
    public IEnumerator Teleportation()
    {
        yield return new WaitForSeconds(1f);
        float random_x_position = Random.Range(-7.32f, 7.32f);
        float random_y_position = Random.Range(-3.5f, 3.5f);

        transform.position = new Vector2(random_x_position, random_y_position);
        if(counterTeleporation == 5)
        {
            counterTeleporation = 0;
        }
        else
        {
            StartCoroutine(Teleportation());
            counterTeleporation++;
        }
    }
    public void Teleportation(Vector3 destination)
    {
        transform.position = destination;
    }

    public void Invisibility()
    {

    }
    public void CalculVelocity()
    {
        if( old_x < this.transform.position.x)
        {
            this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y); 
        }
        if (old_x > this.transform.position.x)
        {
            this.transform.localScale = new Vector2(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
    }

    public void SendIndex(int index)
    {
        photonView.RPC("SetIndex", RpcTarget.All, index);
    }

    [PunRPC]
    public void SetIndex(int index)
    {
        this.index = index;
        name = "DeathNPC_" + index;
    }






}
