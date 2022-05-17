using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviourPun
{
    public int speed = 2;
    public Vector2 direction = new Vector2(0,0);

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().SamePositionAtBoss())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GetComponent<Rigidbody2D>().velocity = direction * speed;

       
    }

    public void ChangeDirection(string nameWall)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (nameWall == "Left" || nameWall == "Right")
        {
            if (direction.y < 0)
            {
                direction = new Vector2(-direction.x, -1 * Random.Range(0.5f, 1f));
            }
            else
            {
                direction = new Vector2(-direction.x, Random.Range(0.5f, 1f));
            }

        }
        if (nameWall == "Top" || nameWall == "Bottom")
        {
            if (direction.x < 0)
            {
                direction = new Vector2(-1 * Random.Range(0.5f, 1f), -direction.y);
            }
            else
            {
                direction = new Vector2(Random.Range(0.5f, 1f), -direction.y);
            }

        }
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
           
            if (collision.gameObject.GetComponent<PlayerGO>().rankTouchBall == 1)
            {
                photonView.RPC("ResetIsTouchFireBall", RpcTarget.All);
                collision.gameObject.GetComponent<PlayerGO>().DisplayCharacter(true);
                collision.gameObject.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.SendDisplayFireBallRoom(false);
                collision.gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedition();
                collision.gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
                collision.gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
            }
            
            SendDestroy();
        }
        else
        {
            if (collision == null)
            {
                return;
            }
            ChangeDirection(collision.gameObject.name);
        }
    }

    public void SendDestroy()
    {
        photonView.RPC("SetDestoy", RpcTarget.All);
    }


    [PunRPC]
    public void SetDestoy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
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
            if(gameManager.SamePositionAtBoss())
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
