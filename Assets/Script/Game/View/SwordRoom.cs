using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordRoom : MonoBehaviourPun
{
    public GameManager gameManager;
    public bool canAttack = true;
    public bool roomIsLaunched = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!roomIsLaunched || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchBySword)
            return;
        CanAttack();
    }
    public void LaunchSwordRoom()
    {
        StartCoroutine(LaunchSwordRoomAfterTeleporation());
    }
    public IEnumerator LaunchSwordRoomAfterTeleporation() {
        yield return new WaitForSeconds(2);
        DisplaySwordAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        roomIsLaunched = true;
        gameManager.speciallyIsLaunch = true;
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(true);
    }

    public void DisplaySwordAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            if(!player.GetComponent<PlayerGO>().isSacrifice)
                player.transform.Find("Perso").Find("Sword").gameObject.SetActive(display);
        }
    }
    public void DisplayHeartsFoAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }

    public void CanAttack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            DisplaySwordAttack();
        }
    }

    public void DisplaySwordAttack()
    {
        LaunchAnimationAttack();
        canAttack = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = false;
    }


    public void LaunchAnimationAttack()
    {
        int indexPlayer = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
        photonView.RPC("DisplayMiddleOne", RpcTarget.Others, indexPlayer);
        DisplayMiddleOne(indexPlayer);
    }

    public IEnumerator DisplayInitial(int indexPlayer)
    {
        yield return new WaitForSeconds(0.4f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Initial").gameObject.SetActive(true);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().canMove = true;
        canAttack = true;
    }

    [PunRPC]
    public void DisplayMiddleOne(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Initial").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle1").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleTwo(indexPlayer));
    }
    public IEnumerator DisplayMiddleTwo(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle1").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle2").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleThree(indexPlayer));
    }
    public IEnumerator DisplayMiddleThree(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle2").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle3").gameObject.SetActive(true);
        StartCoroutine(DisplayFinal(indexPlayer));
    }
    public IEnumerator DisplayFinal(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("middle3").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("Sword").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = true;
        StartCoroutine(DisplayInitial(indexPlayer));
    }

    public void SendDesactivateRoom()
    {
        photonView.RPC("SetDesactivateRoom", RpcTarget.All);
    }

    [PunRPC]
    public void SetDesactivateRoom()
    {
        DisplaySwordAllPlayer(false);
        DisplayHeartsFoAllPlayer(false);
        ResetIsTouchBySwordAllPlayer();
        roomIsLaunched = false;
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManager.speciallyIsLaunch = false;
        gameManager.ActivateCollisionTPOfAllDoor(true);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.CloseDoorWhenVote(false);
        StartCoroutine(CanMoveActiveCoroutine());
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(true);
        }
    }

    public void ResetIsTouchBySwordAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().isTouchBySword = false;
            player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
            player.GetComponent<PlayerGO>().ResetHeart();
        }
    }

    public IEnumerator CanMoveActiveCoroutine()
    {
        yield return new WaitForSeconds(2);
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
    }
}
