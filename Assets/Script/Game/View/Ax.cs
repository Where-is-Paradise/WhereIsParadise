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
    public int maxBoudns = 1;
    public bool canChangeDirection = true;
    public PlayerGO launcher;
    // Start is called before the first frame update
    void Start()
    {
        nbBounds = 0;
        axRoom = GameObject.Find("AxRoom").GetComponent<AxRoom>();
        this.GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(ActiveColliderCoroutine());
        StartCoroutine(CouroutineAnimationCircle());

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
/*        if (!axRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }*/
        this.GetComponent<Rigidbody2D>().velocity = direction * speed;
        if(nbBounds == maxBoudns)
        {
            //PhotonNetwork.Destroy(this.gameObject);
            axRoom.gameManager.ui_Manager.axeEnd.Play();
            Destroy(this.gameObject);
        }
        if(speed == 0)
        {
            //PhotonNetwork.Destroy(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ActiveColliderCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<CircleCollider2D>().enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D nameWallColsion)
    {
/*        if (!axRoom || !axRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }*/
        if (!axRoom)
        {
            return;
        }

        if (nameWallColsion.gameObject.tag  == "Obstacle")
        {
            //PhotonNetwork.Destroy(this.gameObject);
            axRoom.gameManager.ui_Manager.axeEnd.Play();
            Destroy(this.gameObject);
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
            if (collision.gameObject.GetComponent<PlayerGO>().isTouchInTrial)
                return;

            SendIsTouchPlayer(collision.gameObject.GetComponent<PhotonView>().ViewID);
            //photonView.RPC("SendIsTouchPlayer", RpcTarget.All, collision.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

     [PunRPC]
    public void SendIsTouchPlayer(int indexPlayer) {

        if (!axRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;

        axRoom.gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().lifeTrialRoom--;
        axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>()
            .SendLifeTrialRoom(axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().lifeTrialRoom);
        if (axRoom.gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().lifeTrialRoom == 0)
        {
            SetPlayerColor(axRoom.gameManager.GetPlayer(indexPlayer).gameObject);
            if (GetNumberLastPlayer() == 2)
                photonView.RPC("SendBouds", RpcTarget.All, 4);
            if (TestLastPlayer())
            {
                axRoom.GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
                axRoom.DesactivateRoom();
                DesactivateAxRoom();
            }
        }
    }

    [PunRPC]
    public void  SendBouds(int nbBouds)
    {
        this.nbBounds = nbBouds;
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            axRoom.gameManager.RandomWinFireball();
        }
        if (TestLastPlayer())
        {
            GiveAwardToPlayer(GetLastPlayer());
            SendResetColor();
            DesactivateAxRoom();
            axRoom.beforeLastDisconnect = true;
        }
        
    }

    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchInTrial(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }
    public int GetNumberLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        return listPlayer.Length - counter;
    }

    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
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
            if (!axRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchInTrial)
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
                player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
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
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
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
        this.axRoom.DesactivateRoomChild();
        Destroy(this.gameObject);
        //PhotonNetwork.Destroy(this.gameObject);
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

    public void SendBounds(int bounds)
    {
        photonView.RPC("SetBounds", RpcTarget.All, bounds);
    }

    [PunRPC]
    public void SetBounds(int bounds)
    {
        maxBoudns = bounds;
    }

    public IEnumerator CouroutineAnimationCircle()
    {
        yield return new WaitForSeconds(0.6f);
        this.transform.Find("Animation").GetChild(0).gameObject.SetActive(false);
        this.transform.Find("Animation").GetChild(0).gameObject.SetActive(true);
        axRoom.gameManager.ui_Manager.axeLaunch.Play();
        StartCoroutine(CouroutineAnimationCircle());
    }


}
