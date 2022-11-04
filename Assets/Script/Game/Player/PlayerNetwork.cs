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
        Debug.Log(GetComponent<PlayerGO>().isChooseForExpedition);
        if (GetComponent<PlayerGO>().isChooseForExpedition)
        {
            transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = false;
        }

        if (player.GetComponent<PhotonView>().IsMine && player.gameManager.setting.displayTutorial)
        {
            if (!player.gameManager.ui_Manager.listTutorialBool[5])
            {
                StartCoroutine(DisplayTurorialWhenExpeditionCouroutine());
            }
           
        }

    }

    public void SendOnclickToExpedtionN2()
    {
        photonView.RPC("SetOnclickToExpedition", RpcTarget.All);
    }

    public void SetOnclickToExpedtionN2()
    {
        GetComponent<PlayerGO>().isChooseForExpedition = true;
        this.transform.GetChild(1).GetChild(4).gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);
        transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = true;

    }

    public void SendHasWinFireBallRoom(bool hasWinFireBall)
    {
        photonView.RPC("SetHasWinFireBallRoom", RpcTarget.All , hasWinFireBall);
    }

    [PunRPC]
    public void SetHasWinFireBallRoom(bool hasWinFireBall)
    {
        player.hasWinFireBallRoom = hasWinFireBall;
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
        player.gameManager.ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
        player.gameManager.ui_Manager.tutorial_parent.SetActive(true);
        player.gameManager.ui_Manager.tutorial[5].SetActive(true);
        player.gameManager.ui_Manager.listTutorialBool[5] = true;
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
            if (!playerGo.isBoss && !playerGo.wantToChangeBoss && !playerGo.isSacrifice)
            {
                return;
            }
        }
        StartCoroutine(player.gameManager.ChangeBossCoroutine(0.2f));
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
        player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").gameObject.SetActive(true);

        if (message.Length < 40)
        {
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").gameObject.SetActive(false);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").gameObject.SetActive(true);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").Find("ChatText").GetComponent<Text>().text = message;
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").Find("ChatText").GetComponent<Text>().color = new Color(0, 0, 0);
            player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes(4);
        }
        else
        {
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").gameObject.SetActive(false);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").gameObject.SetActive(true);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").Find("ChatText").GetComponent<Text>().text = message;
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").Find("ChatText").GetComponent<Text>().color = new Color(0, 0, 0);
            player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes(6);
        }


        
        
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
        player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").gameObject.SetActive(true);

        if (message.Length < 40)
        {
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").gameObject.SetActive(false);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").gameObject.SetActive(true);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").Find("ChatText").GetComponent<Text>().text = message;
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").Find("ChatText").GetComponent<Text>().color = new Color(219f /255, 55f/255, 38f/255);
            player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes(4);
        }
        else
        {
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").gameObject.SetActive(false);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").gameObject.SetActive(true);
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").Find("ChatText").GetComponent<Text>().text = message;
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("ChatPanelMoreLarger").Find("ChatText").GetComponent<Text>().color = new Color(219f / 255, 55f / 255, 38f / 255);
            player.GetPlayer(indexPlayer).GetComponent<PlayerGO>().SendMessagePlayerInTimes(6);
        }
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
            if (player.gameManager.SamePositionAtBoss())
            {
                player.transform.GetChild(i).gameObject.SetActive(display);
               
            }
                
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
        player.GetComponent<PlayerGO>().gameManager.game.key_counter++;
        player.gameManager.ui_Manager.LaunchAnimationAddKey();


    }
    public void SendResetVoteSacrifice()
    {
        photonView.RPC("SetResetVoteSacrifice", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetVoteSacrifice()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playeritem in players)
        {
            playeritem.GetComponent<PlayerGO>().nbVoteSacrifice = 0;
            playeritem.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").GetComponent<Text>().text = playeritem.GetComponent<PlayerGO>().nbVoteSacrifice.ToString();
            playeritem.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
            playeritem.transform.Find("Perso").Find("Light_red").gameObject.SetActive(false);
        }
        player.GetComponent<PlayerGO>().gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        player.GetComponent<PlayerGO>().gameManager.UpdateSpecialsRooms(player.GetComponent<PlayerGO>().gameManager.game.currentRoom);
        player.GetComponent<PlayerGO>().gameManager.CloseDoorWhenVote(false);
        player.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice = false;
        player.gameManager.ui_Manager.HideNuVoteSacrificeForAllPlayer();
        player.gameManager.HidePlayerNotInSameRoom();
       

    }

    public void SendDisplayMessage(bool displayMessage)
    {
        photonView.RPC("SetDisplayMessage", RpcTarget.All, displayMessage);
    }

    [PunRPC]
    public void SetDisplayMessage(bool displayMessage)
    {
        player.displayMessage = displayMessage;
    }

    public void SendVoteExplorationDisplay(bool vote_V)
    {
        photonView.RPC("SetVoteExplorationDisplay", RpcTarget.All, vote_V);
    }

    [PunRPC]
    public void SetVoteExplorationDisplay(bool vote_V)
    {
        if (!vote_V)
            player.vote_cp = -1;
        else
            player.vote_cp = 1;
        if (!player.gameManager.SamePositionAtBoss())
        {
            return;
        }
        this.transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(vote_V);
        this.transform.Find("ActivityCanvas").Find("X_vote").gameObject.SetActive(!vote_V);
        this.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);

    }

    public void SendHideVoteExplorationDisplay()
    {
        photonView.RPC("SetHideVoteExplorationDisplay", RpcTarget.All);
    }

    [PunRPC]
    public void SetHideVoteExplorationDisplay()
    {
        this.transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(false);
        this.transform.Find("ActivityCanvas").Find("X_vote").gameObject.SetActive(false);
        player.vote_cp = 0;
    }

    public void SendFirstAtDoorToExploration(bool firstAtDoorToExploration) 
    {
        photonView.RPC("SetFirstAtDoorToExploration", RpcTarget.All, firstAtDoorToExploration);
    }

    [PunRPC]
    public void SetFirstAtDoorToExploration(bool firstAtDoorToExploration)
    {
        player.GetComponent<PlayerGO>().firstAtDoorToExploration = firstAtDoorToExploration;
    }


    public void SendIndexPower(int indexPower)
    {
        photonView.RPC("SetIndexPower", RpcTarget.All, indexPower);
    }

    [PunRPC]
    public void SetIndexPower(int indexPower)
    {
        player.GetComponent<PlayerGO>().indexPower = indexPower;
    }

    public void SendMoving(int indexPlayer, float horizontal, float vertical)
    {
        photonView.RPC("SetMoving", RpcTarget.All, indexPlayer, horizontal, vertical) ;
    }

    [PunRPC]
    public void SetMoving(int indexPlayer,float horizontal, float vertical)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        player.GetPlayer(indexPlayer).transform.Translate(
            new Vector3(
                horizontal * player.movementlControlSpeed * 1.3f * Time.deltaTime,
                vertical * player.movementlControlSpeed * 1.3f * Time.deltaTime,
                0
            )
        );
        //photonView.RPC("SetMovingToSpecificPlayer", RpcTarget.All, indexPlayer, player.GetPlayer(indexPlayer).transform.position.x, player.GetPlayer(indexPlayer).transform.position.y);
    }


    [PunRPC]
    public void SetMovingToSpecificPlayer(int indexPlayer, float x, float y)
    {
        if (player.gameManager.IsPlayerMine(indexPlayer))
        {
            player.gameManager.GetPlayer(indexPlayer).transform.position = new Vector3(x, y);
        }
    }

    public void SendIstouchByDeath(bool isTouch)
    {
        photonView.RPC("SetIsTouchByDeath", RpcTarget.All, isTouch);
    }

    [PunRPC]
    public void SetIsTouchByDeath(bool isTouch)
    {
        this.player.isTouchByDeath = isTouch;
    }


    public void SendIstouchByAx(bool isTouch)
    {
        photonView.RPC("SetIsTouchByAx", RpcTarget.All, isTouch);
    }

    [PunRPC]
    public void SetIsTouchByAx(bool isTouch)
    {
        this.player.isTouchByAx = isTouch;
    }


    public void SendIstouchBydDamoclesSword(bool isTouch)
    {
        photonView.RPC("SetIsTouchBydDamoclesSword", RpcTarget.All, isTouch);
    }

    [PunRPC]
    public void SetIsTouchBydDamoclesSword(bool isTouch)
    {
        this.player.isDeadBySwordDamocles = isTouch;
    }

    public void SendChangeColorWhenTouchByDeath()
    {
        photonView.RPC("SeetChangeColorWhenTouchByDeath", RpcTarget.All);
    }

    [PunRPC]
    public void SeetChangeColorWhenTouchByDeath()
    {
        if (player.gameObject.GetComponent<PhotonView>().IsMine)
        {
            int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
            player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
        }
        else
        {
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
