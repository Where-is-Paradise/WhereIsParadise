using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviourPun
{
    public int index;
    public float frequency;
    public GameObject fireBall;
    public GameManager gameManager;
    public bool canFire = false;
    public int categorie = 0;
    public bool therenotMasterClient = false;
    public FireBallRoom fireballRoom;

    // Start is called before the first frame update
    void Start()
    {
        fireballRoom = this.transform.parent.parent.GetComponent<FireBallRoom>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        fireballRoom = this.transform.parent.parent.GetComponent<FireBallRoom>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void ShotFireBall()
    {
        GameObject fireball = PhotonNetwork.Instantiate("FireBall", this.transform.Find("SpawnFireball").position, Quaternion.identity);
        fireball.GetComponent<FireBall>().direction = -this.transform.up;
        fireball.transform.parent = this.gameObject.transform;
        fireball.GetComponent<FireBall>().SendParent(fireball.transform.parent.GetComponent<Turret>().index);
        fireball.GetComponent<FireBall>().turretParent = this.gameObject;
        fireball.GetComponent<FireBall>().speed = 3f;
  /*      if (fireballRoom.frequency > 1.25f)
            fireballRoom.gameManager.ui_Manager.fireball.Play();*/
    }
    public void DestroyFireBalls()
    {
        for(int i = 0; i < transform.childCount; i++){
            if(this.transform.GetChild(i).GetComponent<FireBall>())
                this.transform.GetChild(i).GetComponent<FireBall>().SendDestroy();
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "FireBall")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        }
    }
    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByFireBall || !fireballRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public bool Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            Debug.LogError("Victory 1");
            gameManager.RandomWinFireball("FireBallRoom");
            fireballRoom.DesactivateFireBallRoom();
            return true;
        }
        if (TestLastPlayer())
        {
            Debug.LogError("Victory 2");
            GameObject playerWin = GetPlayerRemaning();
            fireballRoom.GetAward(playerWin.GetComponent<PhotonView>().ViewID);
            fireballRoom.DesactivateRoom();
            fireballRoom.DesactivateFireBallRoom();
            fireballRoom.gameManager.fireBallIsLaunch = false;
            fireballRoom.gameManager.speciallyIsLaunch = false;
            fireballRoom.gameManager.ActivateCollisionTPOfAllDoor(true);
            return true;
        }
        return false;
    }

    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (!player)
            {
                counter++;
                continue;
            }
                
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter >= listPlayer.Length)
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
}
