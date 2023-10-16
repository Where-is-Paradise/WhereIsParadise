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
    public FireBallRoom FireballRoom;

    // Start is called before the first frame update
    void Start()
    {
        FireballRoom = this.transform.parent.GetComponent<FireBallRoom>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShotFireBall()
    {
        GameObject fireball = PhotonNetwork.Instantiate("FireBall", this.transform.Find("SpawnFireball").position, Quaternion.identity);
        fireball.GetComponent<FireBall>().direction = -this.transform.up;
        fireball.transform.parent = this.gameObject.transform;
        fireball.GetComponent<FireBall>().SendParent(fireball.transform.parent.GetComponent<Turret>().index);
        fireball.GetComponent<FireBall>().turretParent = this.gameObject;
        fireball.GetComponent<FireBall>().speed = 3f;
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
            if (player.GetComponent<PlayerGO>().isTouchByFireBall || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            gameManager.RandomWinFireball();
        }
        if (TestLastPlayer())
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
            gameManager.fireBallIsLaunch = false;
            gameManager.speciallyIsLaunch = false;
            gameManager.ActivateCollisionTPOfAllDoor(true);
        }
    }

    public bool LastPlayerDoesNotExist()
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
        if (counter == listPlayer.Length)
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
