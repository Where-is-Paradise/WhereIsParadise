using Pathfinding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_NPC : MonoBehaviourPun
{
    public GameManager gameManager;
    public GameObject godDeath2;
    public bool CanHideDeathGod = false;
    public bool isInvisible = false;
    public bool isTranparencying = false;
    public bool isInvertTranparencying = false;
    public float tranparency = 255;
    private float oldHorizontal;
    private float oldVertical;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.TeleportAllPlayerInRoomOfBoss();
            StartCoroutine(StartDeathNPCRoomAfterTeleportation());
            //StartCoroutine(SendPostionCouroutine());
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().enabled = false;
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            Instantiate(godDeath2, this.transform.position, Quaternion.identity);
        }

    }



    public IEnumerator StartDeathNPCRoomAfterTeleportation()
    {
       
        yield return new WaitForSeconds(2);
        gameManager.deathNPCIsLaunch = true;
        StartCoroutine(SetNotInvisibleCoroutine());
        StartDeathNPCRoom();
    }
    public void StartDeathNPCRoom()
    {
        this.GetComponent<AIPath>().maxSpeed = 0;
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (!gameManager.deathNPCIsLaunch)
        {
            return;
        }
        SetMaxSpeed(3);
        photonView.RPC("SendSpeciallyIsLaucnh", RpcTarget.All);
        SetTargetOfPathFinding();
        StartCoroutine(ChangerSpeedCoroutine());
        photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, false) ;
        //StartCoroutine(UpdatePositionCoroutine());
        StartCoroutine(CanHideDeathGodCoroutine());
        //Instantiate(godDeath2, this.transform.position, Quaternion.identity);
    }
    [PunRPC]
    public void SendIgnoreCollisionPlayer(bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(ignore);
    }


    [PunRPC]
    public void SendSpeciallyIsLaucnh()
    {
        gameManager.speciallyIsLaunch = true;
        gameManager.deathNPCIsLaunch = true;
        gameManager.ActivateCollisionTPOfAllDoor(false);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeScaleForSituation();
        ChangeTransparencyToInvisibilty();
        if (!gameManager.SamePositionAtBoss() && CanHideDeathGod)
        {
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().enabled = false;
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CapsuleCollider2D>().enabled = false;
        }
        
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (!gameManager.deathNPCIsLaunch)
        {
            return;
        }
        gameManager.CloseDoorWhenVote(true);
        SetTargetOfPathFinding();

        ChangeDirectionBrutally();
        

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
        if (GetComponent<AIPath>().desiredVelocity.x >= 0.01f)
        {
            this.transform.localScale = new Vector2(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
        else if (GetComponent<AIPath>().desiredVelocity.x <= -0.01f)
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
            if (player.GetComponent<PlayerGO>().isTouchByDeath || player.GetComponent<PlayerGO>().isSacrifice 
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
        if (!gameManager.deathNPCIsLaunch)
        {
            return;
        }

        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
                return;
            DeathTouchPlayerEvent(collision.gameObject) ;
        }

    }

    public void DeathTouchPlayerEvent(GameObject collision)
    {
        SetTargetOfPathFinding();
        SetPlayerColor(collision);
        if (TestLastPlayer())
        {
            GameObject lastPlayer = GetLastPlayer();
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
            photonView.RPC("SetCanLunchExploration", RpcTarget.All , lastPlayer.GetComponent<PhotonView>().ViewID);
            photonView.RPC("HideAndResetNPC", RpcTarget.All);
            photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
            StartCoroutine(CouroutineDesactivateAll());
            gameManager.deathNPCIsLaunch = false;
        }
       
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
            photonView.RPC("HideAndResetNPC", RpcTarget.All);
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
        collision.gameObject.GetComponent<PlayerNetwork>().SendIstouchByDeath(true);
        collision.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach(GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByDeath || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1) )
            return true;
        return false;
    }
    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByDeath || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
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
            if (!player.GetComponent<PlayerGO>().isTouchByDeath  && !player.GetComponent<PlayerGO>().isSacrifice
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
                player.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)){
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), false);
            player.GetComponent<PlayerGO>().isTouchByDeath = false;
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
}
