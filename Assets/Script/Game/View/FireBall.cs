using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviourPun
{
    public float speed = 0;
    public Vector2 direction = new Vector2(0,0);
    public GameObject turretParent;

    public GameManager gameManager;

    public FireBallRoom fireballRoom;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //StartCoroutine(CoroutineActiveCollision(0.2f));
        fireballRoom = this.transform.parent.parent.parent.GetComponent<FireBallRoom>();
        
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), turretParent.GetComponent<BoxCollider2D>(), true);
        StartCoroutine(ActiveCollisionTurretCouroutine());
        speed = Random.Range(1f, 4.5f);
    }

    // Update is called once per frame
    void Update()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        fireballRoom = this.transform.parent.parent.parent.GetComponent<FireBallRoom>();
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().SamePositionAtBoss())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        GetComponent<Rigidbody2D>().velocity = direction * speed;
       
       
    }

    public IEnumerator CoroutineActiveCollision(float seconde)
    {
        this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
        yield return new WaitForSeconds(seconde);
        this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void ChangeDirection(Collider2D nameWallColsion)
    {
        if (gameManager)
        {
            if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
                return;
        }
        string nameWall = nameWallColsion.gameObject.name;
        if (nameWall == "Left" || nameWall == "Right" )
        {
            if (direction.y < 0)
            {
                direction = new Vector2(-direction.x, -1 * Random.Range(0, 1f));
            }
            else
            {
                direction = new Vector2(-direction.x, Random.Range(0, 1f));
            }

        }
        if (nameWall == "Top" || nameWall == "Bottom" )
        {
            if (direction.x < 0)
            {
                direction = new Vector2(-1 * Random.Range(0, 1f), -direction.y);
            }
            else
            {
                direction = new Vector2(Random.Range(0, 1f), -direction.y);
            }

        }

        if(nameWall  == "Turret")
        {
            direction = new Vector2(-direction.x * Random.Range(0, 1f), -direction.y * Random.Range(0, 1f));
        }

        direction.Normalize();
    }

    public bool CollisionDoor(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return false;
        }
        return true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChangeDirection(collision);
        if (collision.gameObject.tag == "Player")
        {
            
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isTouchInTrial)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isSacrifice)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isInvincible)
                return;

            collision.gameObject.GetComponent<PlayerGO>().isInvincible = true;
            StartCoroutine(collision.gameObject.GetComponent<PlayerGO>().SetInvincibility());
            photonView.RPC("SendMineIsTouch", RpcTarget.All, gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID);
        }

    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        if (collision.tag == "Wall" ||  collision.tag == "Turret")
        {
            SendDestroy();
        }
    }

    [PunRPC]
    public void SendMineIsTouch(int indexPlayer)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        GameObject player = gameManager.GetPlayer(indexPlayer);

        player.GetComponent<PlayerGO>().lifeTrialRoom--;
        player.GetComponent<PlayerNetwork>()
            .SendLifeTrialRoom(player.GetComponent<PlayerGO>().lifeTrialRoom);

        if (player.GetComponent<PlayerGO>().lifeTrialRoom <= 0)
        { 
            photonView.RPC("SendIgnoreCollisionOnePlayer", RpcTarget.All, indexPlayer, true);
            gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().isTouchInTrial = true;
            SetPlayerColor(gameManager.GetPlayer(indexPlayer));
            if (TestLastPlayer())
            {
                fireballRoom.roomIsLaunch = false;
                fireballRoom.GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
                fireballRoom.DesactivateRoom();
                fireballRoom.DesactivateFireBallRoom();
            }  
        }
        SendDestroy();
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!fireballRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchInTrial)
                return player;
        }
        return listPlayer[Random.Range(0, listPlayer.Length)];
    }


    public void SendDestroy()
    {
        if(GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    public void SendIgnoreCollisionOnePlayer(int indexPlayer, bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionPlayer(indexPlayer, ignore);
    }


    public void Victory()
    {

        if(TestLastPlayer())
        {
            GameObject playerWin = GetPlayerRemaning();
            photonView.RPC("ResetIsTouchFireBall", RpcTarget.All);
            //playerWin.gameObject.GetComponent<PlayerGO>().DisplayCharacter(true);
            playerWin.gameObject.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.SendDisplayFireBallRoom(false);
            playerWin.gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedition();
            playerWin.gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
            playerWin.gameObject.GetComponent<PlayerNetwork>().SendCanLaunchExploration();
            playerWin.gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
            if (gameManager.setting.displayTutorial)
            {
                if (!gameManager.ui_Manager.listTutorialBool[23])
                {
                    gameManager.ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
                    gameManager.ui_Manager.tutorial_parent.SetActive(true);
                    gameManager.ui_Manager.tutorial[23].SetActive(true);
                    gameManager.ui_Manager.listTutorialBool[23] = true;
                }

            }
        }
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        Debug.LogError(counter + " " + (listPlayer.Length - 1));
        if (counter >= (listPlayer.Length - 1))
            return true;
        return false;
    }


    public GameObject GetPlayerRemaning()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerGO>().isTouchByFireBall && !player.GetComponent<PlayerGO>().isSacrifice && !player.GetComponent<PlayerGO>().isInJail)
            {
                return player;
            }
        }
        return null;
    }


    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public int GetAllPlayerTouchByFireBall()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isTouchByFireBall)
            {
                counter++;
            }
        }
        return counter;

    }


    [PunRPC]
    public void ResetIsTouchFireBall()
    {
        List<GameObject> players = gameManager.GetPlayerSameRoom(gameManager.GetBoss().GetComponent<PhotonView>().ViewID);
        foreach (GameObject player in players)
        {
            if(gameManager.SamePositionAtBoss() && !player.GetComponent<PlayerGO>().isSacrifice && !player.GetComponent<PlayerGO>().isInJail)
                player.GetComponent<PlayerGO>().DisplayCharacter(true);
        }
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players2)
        {
            player.GetComponent<PlayerGO>().isTouchByFireBall = false;
            player.GetComponent<PlayerGO>().hasWinFireBallRoom = false;
        }
        StartCoroutine(CouroutineResetPlayerColor());
    }

    public IEnumerator CouroutineResetPlayerColor()
    {
        yield return new WaitForSeconds(1);
        List<GameObject> players = gameManager.GetPlayerSameRoom(gameManager.GetBoss().GetComponent<PhotonView>().ViewID);
        foreach (GameObject player in players)
        {
            if (gameManager.SamePositionAtBoss() && !player.GetComponent<PlayerGO>().isSacrifice && !player.GetComponent<PlayerGO>().isInJail)
                player.GetComponent<PlayerGO>().DisplayCharacter(true);
        }
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players2)
        {
            player.GetComponent<PlayerGO>().isTouchByFireBall = false;
            player.GetComponent<PlayerGO>().hasWinFireBallRoom = false;
        }
    }

    public void SendParent(int indexParent)
    {
        photonView.RPC("SetParent", RpcTarget.All, indexParent);
    }

    [PunRPC]
    public void SetParent(int indexParent)
    {
        GameObject[] parents = GameObject.FindGameObjectsWithTag("Turret");
        foreach(GameObject parent in parents)
        {
            if(parent.GetComponent<Turret>().index == indexParent)
            {
                this.transform.parent = parent.transform;
                turretParent = parent.gameObject;
            }
        }
        Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), turretParent.GetComponent<BoxCollider2D>(), true);
        StartCoroutine(ActiveCollisionTurretCouroutine());
    }

    public IEnumerator ActiveCollisionTurretCouroutine()
    {
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), turretParent.GetComponent<BoxCollider2D>(), false) ;
    }

   

}
