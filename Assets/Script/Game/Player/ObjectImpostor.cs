using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Pun;

public class ObjectImpostor : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerGO player;
    public int indexPower = 0;
    public bool powerIsUsed = false;
    public int timerToUsing = 0;
    public bool canUsed = false;
    public bool cantTemporyUsed = false;
    public bool powerIsReset = false;
    public Collider2D collision;
    public PlayerGO otherplayerInvisble;
    public bool isClickedInButtonPower = false;
    public bool isInvisible = false;
    public bool isNearOfPlayer = false;
    public bool OtherIsInvisible = false;
    public bool isOtherToInvisible = false;
    public bool colliderIsImpostor = false;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.GetComponent<PlayerGO>();
        gameManager = player.gameManager;
        if (indexPower == -1)
            return;
        //GetTimerToUsingByIndex(indexPower);
        //DisplayButtonDesactivateTimer(true, timerToUsing);
        //StartCoroutine(CoroutineSetCanUsed());
        canUsed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<PhotonView>().IsMine)
            return;
        if (indexPower == -1)
            return;
        if (powerIsReset)
            return;
            
        //GetAllSituationToCanUsed();
        ChangeScaleByPlayer();
        
        if (isNearOfPlayer)
            DisplayButtonCanUsed(true);
        else
            DisplayButtonCanUsed(false);

        if(gameManager.timer.timeLeft < 0 && !cantTemporyUsed)
        {
            cantTemporyUsed = true;
        }
    }

    public void UsePower()
    {
        if (!canUsed)
            return;
        if (powerIsUsed)
            return;
        if (cantTemporyUsed)
            return;
        if (colliderIsImpostor && !isOtherToInvisible) 
        {
            return;
        }
        LaunchPowerByindex(indexPower);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!canUsed)
            return;
        if (powerIsUsed)
            return;
        if (!player.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.tag != "CollisionTrigerPlayer" && indexPower != 3)
            return;
        if (indexPower == 3 && collision.tag != "Door")
            return;
        colliderIsImpostor = false;
        if (indexPower != 3 && !gameManager.SamePositionAtMine(collision.transform.parent.GetComponent<PhotonView>().ViewID))
            return;
        if (gameManager.speciallyIsLaunch)
            return;
        if (player.isSacrifice)
            return;
        isNearOfPlayer = true;
        if(indexPower != 3)
        {
            if (collision.transform.parent.GetComponent<PlayerGO>().isImpostor)
                colliderIsImpostor = true;
            else
                colliderIsImpostor = false;
        }
        if (isClickedInButtonPower)
        {               
            if (indexPower != 3  && collision.transform.parent.GetComponent<PlayerGO>().isImpostor)
                return;
            this.collision = collision;
            LaunchPowerByindex(indexPower);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "CollisionTrigerPlayer" && indexPower != 3)
            return;
        if (indexPower == 3 && collision.tag != "Door")
            return;
        if (player.isSacrifice)
            return;
        if (indexPower != 3)
        {
            if (!gameManager || !gameManager.SamePositionAtMine(collision.transform.parent.GetComponent<PhotonView>().ViewID))
                return;
        }
        if (!canUsed)
            return;
        isNearOfPlayer = true;
        if(indexPower != 3)
            DisplayPrevisualisationLightRed(true, collision.transform.parent.GetComponent<PhotonView>().ViewID);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "CollisionTrigerPlayer" && indexPower != 3)
        {
            isNearOfPlayer = false;
            colliderIsImpostor = false;
            DisplayPrevisualisationLightRed(false, collision.transform.parent.GetComponent<PhotonView>().ViewID);
        }
        if(collision.tag == "Door" && indexPower == 3)
        {
            isNearOfPlayer = false;
            colliderIsImpostor = false;
            // previsu
        }
        
    }
    public void DisplayPrevisualisationLightRed(bool display, int indexPlayer)
    {
        PlayerGO playerSelected = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        playerSelected.transform.Find("Skins").GetChild(playerSelected.indexSkin).Find("Light_red").gameObject.SetActive(display);
        if (display)
            playerSelected.transform.Find("Skins").GetChild(playerSelected.indexSkin).Find("Light_red").GetComponent<SpriteRenderer>().color = new Color(221 / 255f, 14 / 255f, 14 / 255f, 210 / 255f);
        else
            playerSelected.transform.Find("Skins").GetChild(playerSelected.indexSkin).Find("Light_red").GetComponent<SpriteRenderer>().color = new Color(221 / 255f, 14 / 255f, 14 / 255f, 255 / 255f);
    }

    public IEnumerator CoroutineSetCanUsed()
    {
        yield return new WaitForSeconds(timerToUsing);
        canUsed = true;
        //DisplayButtonDesactivateTimer(false, timerToUsing);
    }

    public void GetTimerToUsingByIndex(int index)
    {
        int initalTimerPlus = 13;
        switch (index)
        {
            case 0:
                timerToUsing = 0;
                break;
            case 1:
                timerToUsing = 0;
                break;
            case 2:
                timerToUsing = 0;
                break;
        }
        timerToUsing += initalTimerPlus;
    }

    public void LaunchPowerByindex(int index)
    {
        switch (index)
        {
            case 0:
                BlindPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
                break;
            case 1:
                if (collision.transform.parent.GetComponent<PlayerGO>().haveToGoToExpedition)
                    return;
                MurderPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
                gameManager.gameManagerNetwork.SendUpdateDataPlayer(player.GetComponent<PhotonView>().ViewID);
                break;
            case 2:
                CursedPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
                break;
            case 3:
                KeyTrapPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
                break;
        }
    }
    public void InvisibilityPower()
    {
        player.GetComponent<PlayerNetwork>().SendColorInvisible(true);
        isInvisible = true;
    }
    public void InvisibilityPowerOtherPlayer()
    {
        collision.transform.parent.GetComponent<PlayerNetwork>().SendColorInvisible(true);
        otherplayerInvisble = collision.transform.parent.GetComponent<PlayerGO>();
        //isInvisible = true;
    }

    public void ResetInvisiblityPower()
    {
        player.GetComponent<PlayerNetwork>().SendColorInvisible(false);
    }

    public void ResetOtherInvisiblityPower()
    {
        if(collision)
            otherplayerInvisble.GetComponent<PlayerNetwork>().SendColorInvisible(false);
    }

    public void MurderPower()
    {
        collision.transform.parent.GetComponent<PlayerNetwork>().SendDeathSacrifice(false);
    }
    public void CursedPower()
    {
        collision.transform.parent.GetComponent<PlayerNetwork>().SendIsCursed(true);
        gameManager.ui_Manager.DisplayImgInHexagoneUseWhenCursedPlayer();
    }
    public void BlindPower()
    {
        collision.transform.parent.GetComponent<PlayerNetwork>().SendIsBlind(true);
    }

    public void KeyTrapPower()
    {
        // display 
        gameManager.ui_Manager.OnClickButtonKeyTraped();
    }

    public void GetAllSituationToResetInvisiblity()
    {
        if (!canUsed)
            return;
        if (powerIsReset)
            return;
        if (!isInvisible)
            return;
        if (!isOtherToInvisible)
            return;
        if (gameManager.timer.timerLaunch)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
            powerIsReset = true;
            return;
        }
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
            powerIsReset = true;
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
            powerIsReset = true;
            return;
        };
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
            powerIsReset = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor(false);
            powerIsReset = true;
            return;
        }
    }
    public void GetAllSituationToCanUsed()
    {
        if (!gameManager)
            return;
        if (gameManager.timer.timerLaunch)
        {
            cantTemporyUsed = true;
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 15);
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            cantTemporyUsed = true;
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 15);
            return;
        };
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            cantTemporyUsed = true;
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 15);
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            cantTemporyUsed = true;
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 15);
            return;
        }

        cantTemporyUsed = false;
    }

    public void ChangeScaleByPlayer()
    {
        float scale_x = this.transform.parent.Find("Skins").GetChild(player.indexSkin).localScale.x;
        if (scale_x > 0)
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        else
            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
    }

    public void DisplayButtonCanUsed(bool display)
    {
        if (powerIsUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
        if (!canUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
/*        if (cantTemporyUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }*/
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(false);
        gameManager.ui_Manager.DisplayObjectPowerBigger(display);
    }

    public void DisplayInvisibleResetButton(bool display)
    {
        gameManager.ui_Manager.DisplayObjectResetInvisibility(display);
    }

    public void DisplayButtonDesactivateTimer(bool display , float timer)
    {
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(display, timer);
    }

}
