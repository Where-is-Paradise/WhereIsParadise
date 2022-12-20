using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ax : MonoBehaviourPun
{
    public float speed = 3;
    public Vector2 direction = new Vector2(0,0);
    public AxRoom axRoom;
    public PlayerGO player;
    public int nbBounds = 0;
    public bool canChangeDirection = true;
    public PlayerGO launcher;
    // Start is called before the first frame update
    void Start()
    {
        nbBounds = 0;
        axRoom = GameObject.Find("AxRoom").GetComponent<AxRoom>();
        this.GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(ActiveColliderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!axRoom)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = false;
            return;
        }

        if(axRoom && !axRoom.gameManager.SamePositionAtBoss())
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = false;
            return;
        }


        if (!axRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        this.GetComponent<Rigidbody2D>().velocity = direction * speed;
        if(nbBounds == 1)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

    }

    public IEnumerator ActiveColliderCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<CircleCollider2D>().enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D nameWallColsion)
    {
        if (!axRoom || !axRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }

        IsTouchPlayer(nameWallColsion);
        if (!canChangeDirection)
            return;

        string nameWall = nameWallColsion.gameObject.name;
        if (nameWall == "Left" || nameWall == "Right"
            || (CollisionDoor(nameWallColsion) && nameWall == "A")
            || (CollisionDoor(nameWallColsion) && nameWall == "D"))
        {

            if (direction.y < 0)
            {
                direction = new Vector2(-direction.x, -1 * Random.Range(1, 1f));
                nbBounds++;
                canChangeDirection = false;
                StartCoroutine(CanChangeDirectionCoroutine());
            }
            else
            {
                direction = new Vector2(-direction.x, Random.Range(1, 1f));
                nbBounds++;
                canChangeDirection = false;
                StartCoroutine(CanChangeDirectionCoroutine());
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
                nbBounds++;
                canChangeDirection = false;
                StartCoroutine(CanChangeDirectionCoroutine());
            }
            else
            {
                direction = new Vector2(Random.Range(1, 1f), -direction.y);
                nbBounds++;
                canChangeDirection = false;
                StartCoroutine(CanChangeDirectionCoroutine());
            }

        }

        
    }
    public IEnumerator CanChangeDirectionCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        canChangeDirection = true;
    }

    public bool CollisionDoor(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return false;
        }
        return true;
    }

    public void IsTouchPlayer(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PhotonView>().ViewID == this.launcher.GetComponent<PhotonView>().ViewID)
                return;
            if (collision.gameObject.GetComponent<PlayerGO>().isInvincible)
                return;
            collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom--;
            collision.gameObject.GetComponent<PlayerNetwork>()
                .SendLifeTrialRoom(collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom);
            if (collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom == 0)
            {
                SetPlayerColor(collision.gameObject);

                if (TestLastPlayer())
                {
                    GiveAwardToPlayer(GetLastPlayer());
                    SendResetColor();
                    DesactivateAxRoom();
                }
            }
        }
    }



    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchByAx(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByAx || !axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchByAx)
                return player;
        }
        Debug.Log("return null");
        return null;
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        //axRoom.gameManager.game.nbTorch++;
        axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
        axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
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
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().ResetHeart();
            player.GetComponent<PlayerGO>().isTouchByAx = false;
            player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
        }
    }

    [PunRPC]
    public void SetSpeedAndDirection(float speed, float directionX, float directionY) {

        this.speed = speed;
        this.direction.x = directionX;
        this.direction.y = directionY;
    }

    public void SendSpeedAndDirection(float speed, float directionX, float directionY)
    {
        photonView.RPC("SetSpeedAndDirection", RpcTarget.All, speed, directionX, directionY);
    }

    public void DesactivateAxRoom()
    {
        this.axRoom.DesactivateRoom();
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void SendLancher(int indexPlayer)
    {
        photonView.RPC("SetLancher", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void SetLancher(int indexPlayer)
    {
        axRoom = GameObject.Find("AxRoom").GetComponent<AxRoom>();
        launcher = axRoom.gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
    }
}
