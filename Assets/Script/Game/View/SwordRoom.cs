using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordRoom : TrialsRoom
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
        if (!roomIsLaunched || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchInTrial)
            return;
        CanAttack();
    }
    public void LaunchSwordRoom()
    {
        StartCoroutine(LaunchSwordRoomAfterTeleporation());
    }
    public IEnumerator LaunchSwordRoomAfterTeleporation() {

        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.CloseDoorWhenVote(true);
        gameManagerParent.DisplayTorchBarre(false);
        yield return new WaitForSeconds(2);
        DisplaySwordAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        roomIsLaunched = true;
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            SendObstalceGroup();
        }
    }

    public void DisplaySwordAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            if(!player.GetComponent<PlayerGO>().isSacrifice)
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Sword").gameObject.SetActive(display);
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
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Initial").gameObject.SetActive(true);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().canMove = true;
        canAttack = true;
    }

    [PunRPC]
    public void DisplayMiddleOne(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Initial").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle1").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleTwo(indexPlayer));
    }
    public IEnumerator DisplayMiddleTwo(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle1").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle2").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleThree(indexPlayer));
    }
    public IEnumerator DisplayMiddleThree(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle2").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle3").gameObject.SetActive(true);
        StartCoroutine(DisplayFinal(indexPlayer));
    }
    public IEnumerator DisplayFinal(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("middle3").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("Sword").Find("Final").Find("SwordAnimation").GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DisplayInitial(indexPlayer));
    }

    public void SendDesactivateRoomChild()
    {
        photonView.RPC("SetDesactivateRoom", RpcTarget.All);
        SendResetObstacle();
    }

    [PunRPC]
    public void SetDesactivateRoom()
    {
        DisplaySwordAllPlayer(false);
        roomIsLaunched = false;
        
    }

    public void ResetIsTouchBySwordAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
            player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
            player.GetComponent<PlayerGO>().ResetHeart();
        }
    }

}
