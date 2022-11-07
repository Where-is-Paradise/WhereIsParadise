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
        if (!roomIsLaunched)
            return;
        CanAttack();
    }
    public void LaunchSwordRoom()
    {
        DisplaySwordAllPlayer(true);
        roomIsLaunched = true;
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
    }
    public void DisplaySwordAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.transform.Find("Perso").Find("Sword").gameObject.SetActive(display);
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
/*        gameManager.GetPlayerMineGO().transform.Find("Perso").Find("Sword").Find("Initial").gameObject.SetActive(false);
        gameManager.GetPlayerMineGO().transform.Find("Perso").Find("Sword").Find("middle1").gameObject.SetActive(true);*/

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
        ResetIsTouchBySwordAllPlayer();
        roomIsLaunched = false;
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManager.speciallyIsLaunch = false;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
    }

    public void ResetIsTouchBySwordAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().isTouchBySword = false;
        }
    }
}
