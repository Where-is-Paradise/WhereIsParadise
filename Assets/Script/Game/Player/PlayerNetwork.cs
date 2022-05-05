using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviourPun
{

    private PlayerGO player;

    public string userId;

    private void Awake()
    {
        player = gameObject.GetComponent<PlayerGO>();
    }
    // Start is called before the first frame update
    void Start()
    {
      

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendNamePlayer(string namePlayer)
    {
        photonView.RPC("SetNamePlayer", RpcTarget.All, namePlayer);
    }

    [PunRPC]
    public void SetNamePlayer(string namePlayer)
    {
        player.playerName = namePlayer;
    }


    public void SendUserId(string userId)
    {
        photonView.RPC("SetUserId", RpcTarget.All, userId);
    }

    [PunRPC]
    public void SetUserId(string userId)
    {
        this.userId = userId;
    }


    public void SendindexSkin( int indexSkin)
    {
        photonView.RPC("SetIndexSkin", RpcTarget.All, indexSkin);
    }

    [PunRPC]
    public void SetIndexSkin(int indexSkin)
    {
        player.indexSkin = indexSkin;
        player.DesactivateAllSkin();
        Debug.Log("sendSkin : " + indexSkin);
        player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).gameObject.SetActive(true);
    }

    public void SendOnclickToExpedition()
    {
        photonView.RPC("SetOnclickToExpedition", RpcTarget.All);
    }

    [PunRPC]
    public void SetOnclickToExpedition()
    {
        GetComponent<PlayerGO>().isChooseForExpedition = !GetComponent<PlayerGO>().isChooseForExpedition;
        this.transform.GetChild(1).GetChild(4).gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);

        if (GetComponent<PlayerGO>().isChooseForExpedition)
        {
            transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = false;
        }

        if (player.gameManager.setting.displayTutorial)
        {
            if (!player.gameManager.ui_Manager.listTutorialBool[4])
            {
                StartCoroutine(DisplayTurorialWhenExpeditionCouroutine());
            }
           
        }

    }

    public void SendVoteToSacrifice(bool hasVote)
    {
        photonView.RPC("SetVoteToSacrifice", RpcTarget.All, hasVote);
    }

    [PunRPC]
    public void SetVoteToSacrifice(bool hasVote)
    {
        if (hasVote)
        {
            player.nbVoteSacrifice++;
        }
        else
        {
            player.nbVoteSacrifice--;
        }
        
        player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").GetComponent<Text>().text = player.nbVoteSacrifice.ToString();   
    }

    public IEnumerator DisplayTurorialWhenExpeditionCouroutine()
    {
        yield return new WaitForSeconds(0.4f);
        player.gameManager.ui_Manager.tutorial_parent.SetActive(true);
        player.gameManager.ui_Manager.tutorial[4].SetActive(true);
        player.gameManager.ui_Manager.listTutorialBool[4] = true;
    }

    public void ResetIsChooseForExpedtion()
    {
        photonView.RPC("ResetSetIsChooseForExpedtion", RpcTarget.All);
    }

    [PunRPC]
    public void ResetSetIsChooseForExpedtion()
    {
        player.isChooseForExpedition = false;
    }


    public void SendCollisionDoorIndex(int index)
    {
        photonView.RPC("SetCollisionDoorIndex", RpcTarget.All, index);
    }

    [PunRPC]
    public void SetCollisionDoorIndex(int index)
    {
        player.collisionInDoorIndex = index;
    }

    public void SendQuitTutorialN7(bool quitTutorialN7)
    {
        photonView.RPC("SetQuitTutorialN7", RpcTarget.All, quitTutorialN7);
    }

    [PunRPC]
    public void SetQuitTutorialN7(bool quitTutorialN7)
    {
        player.quitTutorialN7 = quitTutorialN7;
    }


    public void SendWantToChangeBoss()
    {
        photonView.RPC("SetWantToChangeBoss", RpcTarget.All);
    }

    [PunRPC]
    public void SetWantToChangeBoss()
    {
        player.wantToChangeBoss = !player.wantToChangeBoss;
        player.transform.GetChild(1).GetChild(8).gameObject.SetActive(player.wantToChangeBoss);

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            PlayerGO playerGo = player.GetComponent<PlayerGO>();
            if (!playerGo.isBoss && !playerGo.wantToChangeBoss)
            {
                return;
            }
        }
        StartCoroutine(player.gameManager.ChangeBossCoroutine(0.5f));
        foreach (GameObject player in listPlayer)
        {
            StartCoroutine(player.GetComponent<PlayerNetwork>().CouroutineResetWantToChangeBoss());
        }


    }
    public IEnumerator CouroutineResetWantToChangeBoss()
    {
        yield return new WaitForSeconds(0.5f);
        SendResetWantToChangeBoss();
    }

    public void SendResetWantToChangeBoss()
    {
        photonView.RPC("SetResetWantToChangeBoss", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetWantToChangeBoss()
    {
        player.wantToChangeBoss = false;
        player.transform.GetChild(1).GetChild(8).gameObject.SetActive(player.wantToChangeBoss);
    }


    public void SendTextChat(int indexPlayer, string message)
    {
        photonView.RPC("SetTextChat", RpcTarget.All, indexPlayer, message);
    }

    [PunRPC]
    public void SetTextChat(int indexPlayer, string message)
    {
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().color = new Color(0, 0, 0);
        player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes();
    }

    public void SendTextChatToImpostor(int indexPlayer, string message)
    {
        photonView.RPC("SetTextChatToImpostor", RpcTarget.All, indexPlayer, message);
    }

    [PunRPC]
    public void SetTextChatToImpostor(int indexPlayer, string message)
    {
        if (!player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            return;
        }
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
        player.GetPlayer(indexPlayer).transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().color = new Color(255, 0, 0);
        player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes();
    }

    public void SendDisplayCharacter(bool display)
    {
        photonView.RPC("SetDisplayCharacter", RpcTarget.Others ,display);
    }

    [PunRPC]
    public void SetDisplayCharacter(bool display)
    {

        player.isTouchByFireBall = !display;

        for(int i = 0; i < player.transform.childCount; i++)
        {
            player.transform.GetChild(i).gameObject.SetActive(display);
        }
       
    }


    public void SendDeathSacrifice()
    {
        photonView.RPC("SetDeathSacrifice", RpcTarget.All);
    }

    [PunRPC]
    public void SetDeathSacrifice()
    {
        player.GetComponent<PlayerGO>().isSacrifice = true;
        if (player.GetComponent<PlayerGO>().isBoss)
        {
            player.GetComponent<PlayerGO>().gameManager.ChangeBoss();
        }
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.transform.Find("Perso").Find("Body_skins").GetChild(player.indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
            player.transform.Find("Collision").gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < player.transform.childCount; i++)
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
       
    }
    public void SendResetVoteSacrifice()
    {
        photonView.RPC("SetResetVoteSacrifice", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetVoteSacrifice()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerGO>().nbVoteSacrifice = 0;
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
           
        }
        player.GetComponent<PlayerGO>().gameManager.game.currentRoom.speciallyPowerIsUsed = true;
        player.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayMainLevers(true);
        player.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplaySpeciallyLevers(false,0);
        player.GetComponent<PlayerGO>().gameManager.CloseAllDoor(player.GetComponent<PlayerGO>().gameManager.game.currentRoom, false);

    }

}
