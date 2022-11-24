using Pathfinding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_NPC : MonoBehaviourPun
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.TeleportAllPlayerInRoomOfBoss();
            StartCoroutine(StartDeathNPCRoomAfterTeleportation());
        }
            
    }



    public IEnumerator StartDeathNPCRoomAfterTeleportation()
    {
       
        yield return new WaitForSeconds(2);
        gameManager.deathNPCIsLaunch = true;
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
        gameManager.speciallyIsLaunch = true;
        SetTargetOfPathFinding();
        StartCoroutine(ChangerSpeedCoroutine());
        photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, false) ;
        //StartCoroutine(UpdatePositionCoroutine());
    }
    [PunRPC]
    public void SendIgnoreCollisionPlayer(bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(ignore);
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
        SetTargetOfPathFinding();
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
                || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
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
        SetMaxSpeed(4f);
    }

    public IEnumerator ChangerSpeedCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (gameManager.deathNPCIsLaunch)
        {
            this.GetComponent<AIPath>().maxSpeed += 0.1f;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss || !gameManager.deathNPCIsLaunch)
        {
            return;
        }

        if (collision.gameObject.tag == "CollisionTrigerPlayer")
        {
            Collider2D parent = collision.transform.parent.GetComponent<Collider2D>();
            DeathTouchPlayerEvent(parent);
        }

    }

    public void DeathTouchPlayerEvent(Collider2D collision)
    {
        SetTargetOfPathFinding();
        SetPlayerColor(collision);
        if (TestLastPlayer())
        {
            GameObject lastPlayer = GetLastPlayer();
            Debug.Log(lastPlayer.GetComponent<PhotonView>().ViewID);
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
            lastPlayer.gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
            photonView.RPC("SetCanLunchExploration", RpcTarget.All , lastPlayer.GetComponent<PhotonView>().ViewID);
            photonView.RPC("HideAndResetNPC", RpcTarget.All);
            photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
            StartCoroutine(CouroutineDesactivateAll());
            gameManager.deathNPCIsLaunch = false;
        }
       
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        gameManager.game.nbTorch++;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }

    public void SetPlayerColor(Collider2D collision)
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
    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PlayerGO>().isTouchByDeath  && !player.GetComponent<PlayerGO>().isSacrifice
                && gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
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
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
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
/*        this.gameObject.transform.Find("eyes").gameObject.SetActive(true);
        this.gameObject.transform.Find("body_gfx").gameObject.SetActive(true);
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;*/
        gameManager.ChangeLeverDeathNPC();
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
        gameManager.speciallyIsLaunch = false;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(false);
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        this.gameObject.SetActive(false);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(this.gameObject);

    }

    [PunRPC]
    public void HideAndResetNPC()
    {
        this.gameObject.transform.Find("eyes").gameObject.SetActive(false);
        this.gameObject.transform.Find("body_gfx").gameObject.SetActive(false);
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        this.transform.position = new Vector3(0, 0, 0);
        this.GetComponent<AIPath>().destination = new Vector3(0, 0, 0);
        this.GetComponent<AIPath>().maxSpeed = 0;
        gameManager.deathNPCIsLaunch = false;
    }

    public IEnumerator CouroutineDesactivateAll()
    {
        yield return new WaitForSeconds(0.5f);
        SendResetColor();
        
        photonView.RPC("DesactivateNPC", RpcTarget.All);
    }


}
