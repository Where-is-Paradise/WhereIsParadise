using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviourPun
{
    public float speed = 0;
    public Vector2 direction = new Vector2(0,0);

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(CoroutineActiveCollision(0.2f));
        //speed = Random.Range(2, 10);
    }

    // Update is called once per frame
    void Update()
    {
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
        string nameWall = nameWallColsion.gameObject.name;
        if (nameWall == "Left" || nameWall == "Right" 
            || (CollisionDoor(nameWallColsion) && nameWall == "A")
            || (CollisionDoor(nameWallColsion) && nameWall == "D"))
        {

            if (direction.y < 0)
            {
                direction = new Vector2(-direction.x, -1 * Random.Range(1, 1f));
            }
            else
            {
                direction = new Vector2(-direction.x, Random.Range(1, 1f));
            }

        }
        if (nameWall == "Top" || nameWall == "Bottom" 
            || (CollisionDoor(nameWallColsion) && nameWall == "B")
            || (CollisionDoor(nameWallColsion) && nameWall == "C") 
            || (CollisionDoor(nameWallColsion) && nameWall == "E")
            || (CollisionDoor(nameWallColsion) && nameWall == "F"))
        {
            if (direction.x < 0)
            {
                direction = new Vector2(-1 * Random.Range(1, 1f), -direction.y);
            }
            else
            {
                direction = new Vector2(Random.Range(1, 1f), -direction.y);
            }

        }

        if(nameWall  == "Turret")
        {
            direction = new Vector2(-direction.x * Random.Range(1, 1f), -direction.y * Random.Range(1, 1f));
        }
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
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isTouchByFireBall)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isSacrifice)
            {
                return;
            }

            collision.gameObject.GetComponent<PlayerGO>().DisplayCharacter(false);
            collision.gameObject.GetComponent<PlayerGO>().rankTouchBall = gameManager.GetPlayerSameRoom(gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID) .Count -  GetAllPlayerTouchByFireBall();
            collision.gameObject.GetComponent<PlayerGO>().isTouchByFireBall = true;
            collision.gameObject.GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(true);
            


            if (collision.gameObject.GetComponent<PlayerGO>().rankTouchBall == 2)
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
            
            SendDestroy();
        }
        else
        {
            if (collision == null)
            {
                return;
            }
            ChangeDirection(collision);
        }
    }

    public void SendDestroy()
    {
        //photonView.RPC("SetDestoy", RpcTarget.All);
        PhotonNetwork.Destroy(this.gameObject);
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

    //[PunRPC]
/*    public void SetDestoy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
        }
    }*/

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
            }
        }
    }



}
