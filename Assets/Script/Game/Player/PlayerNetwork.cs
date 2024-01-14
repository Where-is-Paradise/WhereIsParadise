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


    public void SendindexSkin(int indexSkin)
    {
        photonView.RPC("SetIndexSkin", RpcTarget.All, indexSkin);
    }

    [PunRPC]
    public void SetIndexSkin(int indexSkin)
    {
        player.indexSkin = indexSkin;
        player.DesactivateAllSkin();
        player.transform.Find("Skins").GetChild(indexSkin).gameObject.SetActive(true);
    }

    public void SendindexSkinColor(int indexSkinColor, bool sendBeginning)
    {
        photonView.RPC("SetIndexSkinColor", RpcTarget.All, indexSkinColor, sendBeginning);
    }

    [PunRPC]
    public void SetIndexSkinColor(int indexSkinColor, bool sendBeginning)
    {

        // a changé, quand il aura chaque couleur par skin
        if (!sendBeginning)
            SetIndexSkin(0);

        player.indexSkinColor = indexSkinColor;
        player.DesactivateAllSkinColor();
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors")
            .GetChild(indexSkinColor).gameObject.SetActive(true);
    }

    public void SendDisplayHorn(bool display)
    {
        photonView.RPC("SetDisplayHorn", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayHorn(bool display)
    {
        if (!player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Horns").gameObject.SetActive(display);
    }

    public void SendOnclickToExpedition()
    {
        photonView.RPC("SetOnclickToExpedition", RpcTarget.All);
    }

    [PunRPC]
    public void SetOnclickToExpedition()
    {
        GetComponent<PlayerGO>().isChooseForExpedition = !GetComponent<PlayerGO>().isChooseForExpedition;
        this.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);
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

    public void SendResetClickToExpedition()
    {
        photonView.RPC("SetResetClickToExpedition", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetClickToExpedition()
    {
        GetComponent<PlayerGO>().isChooseForExpedition = false;
        this.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(GetComponent<PlayerGO>().isChooseForExpedition);
        transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = false;
    }

    public void SendOnclickToExpedtionN2()
    {
        photonView.RPC("SetOnclickToExpedtionN2", RpcTarget.All);
    }

    [PunRPC]
    public void SetOnclickToExpedtionN2()
    {
        //GetComponent<PlayerGO>().isChooseForExpedition = true;
        this.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(true);
        //transform.GetChild(3).GetComponent<BoxCollider2D>().enabled = true;

        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.explorationPowerIsAvailable = true;
            player.gameManager.ui_Manager.DisabledButtonPowerExploration(false);
        }

    }

    public void SendHasWinFireBallRoom(bool hasWinFireBall)
    {
        photonView.RPC("SetHasWinFireBallRoom", RpcTarget.All, hasWinFireBall);
    }

    [PunRPC]
    public void SetHasWinFireBallRoom(bool hasWinFireBall)
    {
        player.hasWinFireBallRoom = hasWinFireBall;
      
        if (player.GetComponent<PhotonView>().IsMine)
            SendDisplayBlueTorch(hasWinFireBall);
        //player.gameManager.ui_Manager.DisplayAllDoorLightExploration(true);

        PowerImpostor playerPowerImpostorTrap = player.transform.Find("PowerImpostor").GetComponent<PowerImpostor>();
        ObjectImpostor playerObjectImpostor = player.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>();
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.gameManager.dataGame.SetDataPlayerMine(player.GetComponent<PhotonView>().ViewID, player.transform.position.x, player.transform.position.y,
           player.position_X, player.position_Y, player.isImpostor, player.isBoss, player.isSacrifice, player.isInJail, player.isInvisible,
           player.indexSkin, player.playerName, player.hasWinFireBallRoom, userId, playerPowerImpostorTrap.indexPower, playerPowerImpostorTrap.powerIsUsed,
           playerObjectImpostor.indexPower, playerObjectImpostor.powerIsUsed, player.isInExpedition);
        }
        else
        {
            player.gameManager.dataGame.SetDataOtherPlayers(player.GetComponent<PhotonView>().ViewID, player.transform.position.x, player.transform.position.y,
          player.position_X, player.position_Y, player.isImpostor, player.isBoss, player.isSacrifice, player.isInJail, player.isInvisible,
          player.indexSkin, player.playerName, player.hasWinFireBallRoom, userId);
        }

    }

    public void SendBlackTorch(bool hasWinFireBall)
    {
        photonView.RPC("BlackTorch", RpcTarget.All, hasWinFireBall);
    }

    [PunRPC]
    public void BlackTorch(bool hasWinFireBall)
    {
        player.hasWinFireBallRoom = hasWinFireBall;
        if (player.GetComponent<PhotonView>().IsMine)
            SendDisplayBlackTorch(hasWinFireBall);
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
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("ChangeBoss").gameObject.SetActive(player.wantToChangeBoss);

        if (!player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counterChangeBoss = 0;
        foreach (GameObject player in listPlayer)
        {
            PlayerGO playerGo = player.GetComponent<PlayerGO>();
            if ((!playerGo.isBoss && playerGo.wantToChangeBoss) || playerGo.isSacrifice)
            {
                counterChangeBoss++;
            }
        }
        if (listPlayer.Length < 4)
        {
            if (counterChangeBoss == (listPlayer.Length - 1))
            {
                StartCoroutine(player.gameManager.ChangeBossCoroutine(0.2f));
                foreach (GameObject player in listPlayer)
                {
                    StartCoroutine(player.GetComponent<PlayerNetwork>().CouroutineResetWantToChangeBoss());
                }
            }
        }
        else
        {
            if (counterChangeBoss > (listPlayer.Length / 2))
            {
                StartCoroutine(player.gameManager.ChangeBossCoroutine(0.2f));
                foreach (GameObject player in listPlayer)
                {
                    StartCoroutine(player.GetComponent<PlayerNetwork>().CouroutineResetWantToChangeBoss());
                }
            }
        }
    }
    public IEnumerator CouroutineResetWantToChangeBoss()
    {
        yield return new WaitForSeconds(0.25f);
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
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("ChangeBoss").gameObject.SetActive(player.wantToChangeBoss);
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
            player.GetPlayer(indexPlayer).transform.Find("InfoCanvas").Find("ChatPanel").Find("NormalChat").Find("ChatText").GetComponent<Text>().color = new Color(219f / 255, 55f / 255, 38f / 255);
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
        photonView.RPC("SetDisplayCharacter", RpcTarget.Others, display);
    }

    [PunRPC]
    public void SetDisplayCharacter(bool display)
    {
        player.isTouchByFireBall = !display;
        if (display)
        {
            transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            return;
        }
        transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0f);
    }


    public void SendDeathSacrifice(bool KeyPlus)
    {
        photonView.RPC("SetDeathSacrifice", RpcTarget.All, KeyPlus);
    }

    [PunRPC]
    public void SetDeathSacrifice(bool keyPlus)
    {

        if (player.GetComponent<PlayerGO>().isBoss)
        {
            player.GetComponent<PlayerGO>().gameManager.ChangeBoss();
        }
        StartCoroutine(HidePlayerCouroutine());
        if (keyPlus)
        {
            player.GetComponent<PlayerGO>().gameManager.game.key_counter++;
            player.gameManager.ui_Manager.LaunchAnimationAddKey();
        }
        player.GetComponent<PlayerGO>().isSacrifice = true;
        player.gameManager.gameManagerNetwork.SendUpdateDataPlayer(player.GetComponent<PhotonView>().ViewID);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(false);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_Cursed").gameObject.SetActive(false);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_red").gameObject.SetActive(false);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_redDark").gameObject.SetActive(false);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Crown").gameObject.SetActive(false);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Horns").gameObject.SetActive(false);
        player.transform.Find("TrialObject").gameObject.SetActive(false);
        player.transform.Find("TorchBarre").gameObject.SetActive(false);

        LaunchSacrificeAnimation();

        player.gameManager.SacrificeIsUsedOneTimes = true;

        if (player.isImpostor)
        {
            player.transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed = true;
            player.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().powerIsUsed = true;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(false);
                player.GetComponent<PlayerGO>().gameManager.ui_Manager.DisplayTrapPowerBigger(false);
                player.gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
                player.gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            }
        }
    }

    public void LaunchSacrificeAnimation()
    {
        GetComponent<PhotonTransformViewClassic>().enabled = false;
        GetComponent<PhotonRigidbody2DView>().enabled = false;
        GetComponent<Lag_Compensation>().enabled = false;
        player.canMove = false;
        if (!player.gameManager.SamePositionAtMine(player.GetComponent<PhotonView>().ViewID))
            return;
        player.transform.Find("DeathPlayerAnimation").GetComponent<Animator>().SetBool("death", true);
        player.transform.Find("DeathPlayerAnimation").GetComponent<CircleCollider2D>().enabled = true;
        if (player.transform.position.y < -2.35f)
            player.transform.position = new Vector3(this.transform.position.x, -2f);
        if (player.transform.position.x < -6.6f)
            player.transform.position = new Vector3(-6.6f, this.transform.position.y);
        if (player.transform.position.x > 6.6)
            player.transform.position = new Vector3(6.6f, this.transform.position.y);
        player.old_y_position = player.transform.position.y;
        this.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = -54;
        StartCoroutine(player.CanMoveActiveCoroutine());
        StartCoroutine(player.MovingDeathAnimationWaitCouroutine());


    }

    public IEnumerator LaunchSacrificeAnimationCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<PhotonTransformViewClassic>().m_PositionModel.SynchronizeEnabled = false;
        GetComponent<PhotonRigidbody2DView>().enabled = false;
        GetComponent<Lag_Compensation>().enabled = false;
        player.canMove = false;
        if (player.gameManager.SamePositionAtMine(player.GetComponent<PhotonView>().ViewID))
        {
            player.transform.Find("DeathPlayerAnimation").GetComponent<Animator>().SetBool("death", true);
            player.transform.Find("DeathPlayerAnimation").GetComponent<CircleCollider2D>().enabled = true;
            if (player.transform.position.y < -2.35f)
                player.transform.position = new Vector3(this.transform.position.x, -2f);
            if (player.transform.position.x < -6.6f)
                player.transform.position = new Vector3(-6.6f, this.transform.position.y);
            if (player.transform.position.x > 6.6)
                player.transform.position = new Vector3(6.6f, this.transform.position.y);
            player.old_y_position = player.transform.position.y;
            this.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = -54;
            StartCoroutine(player.CanMoveActiveCoroutine());
            StartCoroutine(player.MovingDeathAnimationWaitCouroutine());
        }
    }

    public IEnumerator HidePlayerCouroutine()
    {
        yield return new WaitForSeconds(5);
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.transform.Find("InfoCanvas").gameObject.SetActive(true);
            player.transform.Find("Skins").GetChild(player.indexSkin).gameObject.SetActive(true);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).gameObject.SetActive(true);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            player.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
            player.transform.Find("Collision").gameObject.SetActive(false);
            player.SetIconDeath(true);
        }
        else
        {
            for (int i = 0; i < player.transform.childCount; i++)
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
        player.GetComponent<PlayerGO>().isSacrifice = true;
        player.transform.Find("DeathPlayerAnimation").GetComponent<Animator>().SetBool("death", false);
        player.transform.Find("DeathPlayerAnimation").GetComponent<CircleCollider2D>().enabled = false;
    }

    public void SendResetSacrifice()
    {
        photonView.RPC("SetResetSacrifice", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetSacrifice()
    {
        player.GetComponent<PlayerGO>().isSacrifice = false;
        if (!player.gameManager.SamePositionAtBoss())
            return;
        player.transform.Find("Skins").GetChild(player.indexSkin).gameObject.SetActive(true);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).gameObject.SetActive(true);
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        player.transform.Find("Collision").gameObject.SetActive(true);
        //ResetPositionOFSkin();
        player.GetComponent<PhotonRigidbody2DView>().enabled = true;
        for (int i = 0; i < player.transform.childCount; i++)
        {
            player.transform.GetChild(i).gameObject.SetActive(true);
        }
        player.SetIconDeath(false);
    }

    public void ResetPositionOFSkin()
    {
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).transform.position = new Vector2(0.04f, -7.72f);
    }

    public void SendColorInvisible(bool invisible)
    {
        photonView.RPC("SetColorInvisible", RpcTarget.All, invisible);
    }

    [PunRPC]
    public void SetColorInvisible(bool invisible)
    {
        player.isInvisible = invisible;
        if (!invisible)
        {
            player.transform.Find("Skins").GetChild(player.indexSkin).gameObject.SetActive(true);
            player.transform.Find("Skins").GetChild(player.indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Eyes1").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            player.transform.Find("InfoCanvas").gameObject.SetActive(true);
            if (player.isBoss)
                player.transform.Find("Skins").GetChild(player.indexSkin).Find("Crown").gameObject.SetActive(true);
            player.transform.Find("Skins").GetChild(player.indexSkin).gameObject.SetActive(true);
            return;
        }
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.transform.Find("Skins").GetChild(player.indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(false);
        }
        else
        {
            player.transform.Find("Skins").GetChild(player.indexSkin).gameObject.SetActive(false);
            player.transform.Find("Skins").GetChild(player.indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0f);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Eyes1").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0f);
            player.transform.Find("InfoCanvas").gameObject.SetActive(false);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(false);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Crown").gameObject.SetActive(false);
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

        foreach (GameObject playeritem in players)
        {
            playeritem.GetComponent<PlayerGO>().nbVoteSacrifice = 0;
            playeritem.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").GetComponent<Text>().text = playeritem.GetComponent<PlayerGO>().nbVoteSacrifice.ToString();
            playeritem.transform.Find("ActivityCanvas").Find("NumberVoteSacrifice").gameObject.SetActive(false);
            playeritem.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_red").gameObject.SetActive(false);
        }
        player.GetComponent<PlayerGO>().gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        player.GetComponent<PlayerGO>().gameManager.UpdateSpecialsRooms(player.GetComponent<PlayerGO>().gameManager.game.currentRoom);
        player.GetComponent<PlayerGO>().gameManager.CloseDoorWhenVote(false);
        player.GetComponent<PlayerGO>().gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasVoteSacrifice = false;
        player.gameManager.ui_Manager.HideNuVoteSacrificeForAllPlayer();
        //player.gameManager.HidePlayerNotInSameRoom();


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
        this.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(true);

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
        if (player.GetComponent<PhotonView>().IsMine && indexPower != -1)
            player.transform.Find("PowerImpostor").gameObject.SetActive(true);
    }

    public void SendIndexObjectPower(int indexObjectPower)
    {
        photonView.RPC("SetIndexObjectPower", RpcTarget.All, indexObjectPower);
    }
    [PunRPC]
    public void SetIndexObjectPower(int indexObjectPower)
    {
        player.GetComponent<PlayerGO>().indexObjectPower = indexObjectPower;
        player.transform.Find("ImpostorObject").GetComponent<ObjectImpostor>().indexPower = indexObjectPower;
        if (player.GetComponent<PhotonView>().IsMine)
            player.transform.Find("ImpostorObject").gameObject.SetActive(true);
    }




    public void SendMoving(int indexPlayer, float horizontal, float vertical)
    {
        photonView.RPC("SetMoving", RpcTarget.All, indexPlayer, horizontal, vertical);
    }

    [PunRPC]
    public void SetMoving(int indexPlayer, float horizontal, float vertical)
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


    public void SendIstouchByWord(bool isTouch)
    {
        photonView.RPC("SetIsTouchBySword", RpcTarget.All, isTouch);
    }

    [PunRPC]
    public void SetIsTouchBySword(bool isTouch)
    {
        this.player.isTouchBySword = isTouch;
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
    public void SendIstouchInTrial(bool isTouch)
    {
        photonView.RPC("SetIstouchInTrial", RpcTarget.All, isTouch);
    }

    [PunRPC]
    public void SetIstouchInTrial(bool isTouch)
    {
        this.player.isTouchInTrial = isTouch;
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
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Colors").GetChild(player.indexSkinColor).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Sword").gameObject.SetActive(false);
        }
        else
        {
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(false);
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Sword").gameObject.SetActive(false);
            if (player.hasProtection)
                player.transform.Find("TrialObject").Find("AuraProtection").gameObject.SetActive(false);
        }
    }

    public void SendRedColor(bool display)
    {
        photonView.RPC("SetRedColor", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetRedColor(bool display)
    {
        if (!player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || !player.GetComponent<PhotonView>().IsMine)
            return;
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_red").gameObject.SetActive(display);
    }

    public void SendCanLaunchExploration()
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All);
    }

    [PunRPC]
    public void SetCanLunchExploration()
    {
        player.gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
        player.gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }

    public void SendIsCursed(bool isCursed)
    {
        photonView.RPC("SetIsCursed", RpcTarget.All, isCursed);
    }

    [PunRPC]
    public void SetIsCursed(bool isCursed)
    {
        player.GetComponent<PlayerGO>().isCursed = isCursed;
        if (player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_Cursed").gameObject.SetActive(isCursed);
    }

    public void SendPurification()
    {
        photonView.RPC("SetPurification", RpcTarget.All);
    }

    [PunRPC]
    public void SetPurification()
    {
        player.GetComponent<PlayerGO>().isCursed = false;
        player.GetComponent<PlayerGO>().isBlind = false;
    }


    public void SendDistanceCursed(int distanceCursed, int indexRoom)
    {
        photonView.RPC("SetDistanceCursed", RpcTarget.All, distanceCursed, indexRoom);
    }

    [PunRPC]
    public void SetDistanceCursed(int distanceCursed, int indexRoom)
    {
        player.distanceCursed = distanceCursed;
        player.roomUsedWhenCursed = player.gameManager.GetHexagone(indexRoom).Room;
    }


    public void SendIsBlind(bool isBlind)
    {
        photonView.RPC("SetIsBlind", RpcTarget.All, isBlind);
    }

    [PunRPC]
    public void SetIsBlind(bool isBlind)
    {
        player.GetComponent<PlayerGO>().isBlind = isBlind;
        if (player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || player.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_Cursed").gameObject.SetActive(isBlind);
    }

    public void SendTeleportPlayerToSameRoomOfBoss()
    {
        photonView.RPC("TeleportPlayerToSameRoomOfBoss", RpcTarget.All);
    }

    [PunRPC]
    public void TeleportPlayerToSameRoomOfBoss()
    {
        player.position_X = player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.X;
        player.position_Y = player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Y;
        if (!player.GetComponent<PhotonView>().IsMine)
            return;
        player.gameManager.game.currentRoom = player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room;
        player.gameManager.UpdateSpecialsRooms(player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room);
        player.gameManager.ResetDoorsActive();
        player.gameManager.SetDoorNoneObstacle(player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room);
        player.gameManager.SetDoorObstacle(player.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room);
        player.gameManager.SetCurrentRoomColor();
        player.gameManager.HidePlayerNotInSameRoom();
        player.gameManager.CloseAllDoor(player.gameManager.game.currentRoom, false);
        player.gameManager.gameManagerNetwork.SendUpdateHidePlayer();
        player.gameManager.ui_Manager.HideDistanceRoom();
        StartCoroutine(player.gameManager.SetMapOFLostSoul(0.1f));
    }

    public void SendInsertPowerToDoor(int indexRoom, int indexPower)
    {
        photonView.RPC("InsertPowerToDoor", RpcTarget.All, indexRoom, indexPower);
    }

    [PunRPC]
    public void InsertPowerToDoor(int indexRoom, int indexPower)
    {
        Room room = player.gameManager.game.dungeon.GetRoomByIndex(indexRoom);
        
        switch (indexPower)
        {
            case 0:
                room.IsFoggy = true;
                break;
            case 1:
                player.gameManager.ResetSpeciallyRoomState(room);
                room.chest = true;
                room.isTraped = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    player.gameManager.game.dungeon.InsertChestRoom(room.Index);
                    for (int i = 0; i < 2; i++)
                    {
                        player.gameManager.gameManagerNetwork.SendChestData(indexRoom, room.chestList[i].index, room.chestList[i].isAward, room.chestList[i].indexAward, room.chestList[i].indexTrap);
                    }
                   
                }
                break;
            case 2:
                room.IsVirus = true;
                break;
            case 3:
                player.gameManager.ResetSpeciallyRoomState(room);
                room.isPray = true;
                room.isTraped = true;      
                break;
            case 4:
                room.isIllustion = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    player.gameManager.MixRoomNeigbour(room);
                }
                break;
        }
        room.isTraped = true;

        player.transform.Find("PowerImpostor").GetComponent<PowerImpostor>().powerIsUsed = true;
    }

    public void SendLifeTrialRoom(int nbLife)
    {
        photonView.RPC("SetLifeTrialRoom", RpcTarget.All, nbLife);
    }

    [PunRPC]
    public void SetLifeTrialRoom(int nbLife)
    {
        player.lifeTrialRoom = nbLife;
        player.isInvincible = true;
        player.transform.Find("HearBrokenAnimation").GetChild(0).gameObject.SetActive(true);
        player.DisplayHeartInSituation();
        StartCoroutine(player.ResetInvincibleCouroutine());
        
    }

    public void SendDisplayWhiteLight(bool display)
    {
        photonView.RPC("SetDisplayWhiteLight", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayWhiteLight(bool display)
    {
        player.transform.Find("Skins").GetChild(player.indexSkin).Find("Light_around").gameObject.SetActive(display);
    }

    public void SendDisplayCrown(bool display)
    {

        photonView.RPC("SetDisplayCrown", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayCrown(bool display)
    {
        if (player)
            player.transform.Find("Skins").GetChild(player.indexSkin).Find("Crown").gameObject.SetActive(display);
    }

    public void SendIsReady(bool isReady)
    {
        photonView.RPC("SetIsReady", RpcTarget.All, isReady);
    }

    [PunRPC]
    public void SetIsReady(bool isReady)
    {
        player.GetComponent<PlayerGO>().isReady = isReady;
        player.transform.Find("ActivityCanvas").Find("Ready_V").gameObject.SetActive(isReady);
    }


    public void SendGlobalVariabel(bool isImpostor, bool isSacrifice, bool isInJail, bool isInvisible)
    {
        photonView.RPC("SetGlobalVariabel", RpcTarget.All, isImpostor, isSacrifice, isInJail, isInvisible);
    }

    [PunRPC]
    public void SetGlobalVariabel(bool isImpostor, bool isSacrifice, bool isInJail, bool isInvisible)
    {
        player.isImpostor = isImpostor;
        player.isSacrifice = isSacrifice;
        player.isInJail = isInJail;
        player.isInvisible = isInvisible;
    }
    public void SendDungeonPosition(int positionX, int positionY)
    {
        photonView.RPC("SetDungeonPosition", RpcTarget.All, positionX, positionY);
    }

    [PunRPC]
    public void SetDungeonPosition(int positionX, int positionY)
    {
        player.position_X = positionX;
        player.position_Y = positionY;

        player.gameManager.HidePlayerNotInSameRoom();
    }

    public void SendHorizontalAndVertical(float horizontal, float vertical)
    {
        float[] tabFloat = new float[2];
        tabFloat[0] = horizontal;
        tabFloat[1] = vertical;

        float speed = Mathf.Max(tabFloat);
        if (Mathf.Abs(speed) < 0.35)
        {
            photonView.RPC("SetHorizontalAndVertical", RpcTarget.All, true);
        }
        else
        {
            photonView.RPC("SetHorizontalAndVertical", RpcTarget.All, false);
        }
        //this.GetComponent<PhotonTransformViewClassic>().SetSynchronizedValues(new Vector3(speed,speed), 0);
        //this.GetComponent<PhotonTransformViewClassic>().
    }

    [PunRPC]
    public void SetHorizontalAndVertical(bool activeRigibody)
    {
        this.GetComponent<PhotonTransformViewClassic>().enabled = !activeRigibody;
        this.GetComponent<PhotonRigidbody2DView>().enabled = activeRigibody;
    }

    public void SendSpacePosition(float x, float y, int indexPlayer)
    {
        photonView.RPC("SetSpacePosition", RpcTarget.Others, x, y, indexPlayer);
    }

    [PunRPC]
    public void SetSpacePosition(float x, float y, int indexPlayer)
    {
        /*        player.x_sended = x;
                player.y_sended = y;
                player.positionSended = true;*/
        Debug.LogError("sa passe");
        Vector3 newPosition = new Vector3(x, y);
        Vector3 distance = newPosition - this.transform.position;
        this.transform.Translate(distance * 10 * Time.deltaTime);

        //this.GetComponent<Rigidbody2D>().position = Vector3.MoveTowards(this.GetComponent<Rigidbody2D>().position, distance, Time.fixedDeltaTime);
    }


    public void SendChangeSystemtoUpdatePosition(int indexPlayer, bool inferior)
    {
        photonView.RPC("SetChangeSystemToUpdatePosition", RpcTarget.Others, indexPlayer, inferior);
    }

    [PunRPC]
    public void SetChangeSystemToUpdatePosition(int indexPlayer, bool inferior)
    {
        if (player.GetComponent<PhotonView>().ViewID == indexPlayer)
        {
            if (inferior)
            {
                this.GetComponent<PhotonTransformViewClassic>().enabled = false;
                this.GetComponent<PhotonTransformView>().enabled = true;
            }
            else
            {
                this.GetComponent<PhotonTransformViewClassic>().enabled = true;
                this.GetComponent<PhotonTransformView>().enabled = false;
            }
        }
    }

    public void SendSacrificePlayerAfk()
    {
        photonView.RPC("SetSacrificePlayerAfk", RpcTarget.Others);
    }

    [PunRPC]
    public void SetSacrificePlayerAfk()
    {
        if ((!player.isImpostor || (player.isImpostor && player.isSacrifice)) && player.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(player.gameManager.CouroutineDisplayEndPanel());
            player.gameManager.UpdateDataInformationInEndGame();

        }
    }

    public void SendChangeSyncFunction(bool change)
    {
        photonView.RPC("SetChangeSyncFunction", RpcTarget.All, change);
    }

    [PunRPC]
    public void SetChangeSyncFunction(bool change)
    {
        this.GetComponent<PhotonRigidbody2DView>().preciseSyncPosition = change;
    }

    public void SendResetHeart()
    {
        photonView.RPC("SetResetHeart", RpcTarget.All);
    }

    [PunRPC]
    public void SetResetHeart()
    {
        player.ResetHeart();
    }
    public void SendHaveToGotoExpedition(bool havetoGo)
    {
        photonView.RPC("SetHaveToGotoExpedition", RpcTarget.All, havetoGo);
    }

    [PunRPC]
    public void SetHaveToGotoExpedition(bool havetoGo)
    {
        player.GetComponent<PlayerGO>().haveToGoToExpedition = havetoGo;
    }

    public void SendDisplayBlueTorch(bool display)
    {
        photonView.RPC("SetDisplayBlueTorch", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayBlueTorch(bool dislay)
    {
        this.transform.Find("TrialObject").Find("BlueTorch").Find("BlueTorchImg").gameObject.SetActive(dislay);
        player.explorationPowerIsAvailable = dislay;
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.gameManager.ui_Manager.DisabledButtonPowerExploration(!dislay);
            player.gameManager.ui_Manager.DisplayAllDoorLightExploration(dislay);
        }
        player.gameManager.onePlayerHasTorch = dislay;


    }

    public void SendDisplayBlackTorch(bool display)
    {
        photonView.RPC("SetDisplayBlackTorch", RpcTarget.All, display);
    }

    [PunRPC]
    public void SetDisplayBlackTorch(bool dislay)
    {
        this.transform.Find("TrialObject").Find("BlackTorch").gameObject.SetActive(dislay);
        player.explorationPowerIsAvailable = dislay;
        if (player.GetComponent<PhotonView>().IsMine)
        {

           player.gameManager.ui_Manager.DisplayButtonBlackTorch(dislay);
            player.gameManager.ui_Manager.DisplayAllDoorLightExploration(dislay);
        }


    }

    public void SendDesactivateObject(int indexPlayer)
    {
        photonView.RPC("DesactivateObject", RpcTarget.All, indexPlayer);
    }

    [PunRPC]
    public void DesactivateObject(int indexPlayer)
    {
        GameObject awardObject = player.GetOnlyChildActive(GameObject.Find("Room").transform.Find("Special").Find("AwardObject").gameObject);
        awardObject.SetActive(false);
        GameObject speciallyRoom = player.GetOnlyChildActive(GameObject.Find("Room").transform.Find("Special").gameObject);
        speciallyRoom.GetComponent<TrialsRoom>().ReactivateCurrentRoom();
        speciallyRoom.GetComponent<TrialsRoom>().ActivateObjectPower(indexPlayer);
        if(player.isImpostor && !player.hasOneTrapPower)
            speciallyRoom.GetComponent<TrialsRoom>().ActivateImpostorObject(indexPlayer);
    }


    public void SendDesactivateObjectTeam()
    {
        photonView.RPC("DesactivateObjectTeaM", RpcTarget.All);
    }

    [PunRPC]
    public void DesactivateObjectTeaM()
    {
        GameObject awardObject = player.GetOnlyChildActive(GameObject.Find("Room").transform.Find("Special").Find("AwardObject").gameObject);
        awardObject.SetActive(false);
        GameObject speciallyRoom = player.GetOnlyChildActive(GameObject.Find("Room").transform.Find("Special").gameObject);
        if (!speciallyRoom.GetComponent<TrialsRoom>())
            return;
        speciallyRoom.GetComponent<TrialsRoom>().ReactivateCurrentRoom();
        speciallyRoom.GetComponent<TrialsRoom>().ApplyGlobalAward();

    }


    public void SendDisplayMagicalKey(bool display)
    {
        photonView.RPC("DisplayMagicalKey", RpcTarget.All, display);
    }

    [PunRPC]
    public void DisplayMagicalKey(bool display)
    {
        player.transform.Find("TrialObject").Find("MagicalKey").gameObject.SetActive(display);
    }
}