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
        GetTimerToUsingByIndex(indexPower);
        DisplayButtonDesactivateTimer(true, timerToUsing);
        StartCoroutine(CoroutineSetCanUsed());
    }

    // Update is called once per frame
    void Update()
    {
        if (indexPower == -1)
            return;
        if (powerIsReset)
            return;
        if(indexPower == 0)
            GetAllSituationToResetInvisiblity();

        GetAllSituationToCanUsed();
        ChangeScaleByPlayer();

        if (indexPower != 0)
        {
            if (isNearOfPlayer)
                DisplayButtonCanUsed(true);
            else
                DisplayButtonCanUsed(false);
        }
        if(indexPower == 0)
        {
            if (isNearOfPlayer && colliderIsImpostor)
                DisplayButtonCanUsed(true);
            else
                DisplayButtonCanUsed(false);
        }
    
    }

    public void UsePower()
    {
        if (!canUsed)
            return;
        if (powerIsUsed)
            return;
        if (indexPower != 0)
            return;
        if (colliderIsImpostor && !isOtherToInvisible) 
        {
            return;
        }
        if(isOtherToInvisible)
            LaunchPowerByindex(4);
        else
            LaunchPowerByindex(indexPower);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!canUsed)
            return;
        if (powerIsUsed)
            return;
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        colliderIsImpostor = false;
        if (!gameManager.SamePositionAtMine(collision.transform.parent.GetComponent<PhotonView>().ViewID))
                return;
        isNearOfPlayer = true;
        if (collision.transform.parent.GetComponent<PlayerGO>().isImpostor)
            colliderIsImpostor = true;
        else
            colliderIsImpostor = false;

        if (isClickedInButtonPower)
        {
            if (indexPower == 0 && !isOtherToInvisible)
            {
                if (collision.transform.parent.GetComponent<PlayerGO>().isImpostor)
                {
                    this.collision = collision;
                    LaunchPowerByindex(4);
                }
                return;
            }
               
            if (collision.transform.parent.GetComponent<PlayerGO>().isImpostor)
                return;
            this.collision = collision;

            LaunchPowerByindex(indexPower);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (indexPower == 0)
            return;
        if (!gameManager || !gameManager.SamePositionAtMine(collision.transform.parent.GetComponent<PhotonView>().ViewID))
            return;
        isNearOfPlayer = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "CollisionTrigerPlayer")
        {
            isNearOfPlayer = false;
            colliderIsImpostor = false;
        }
            
    }

    public IEnumerator CoroutineSetCanUsed()
    {
        yield return new WaitForSeconds(timerToUsing);
        canUsed = true;
        DisplayButtonDesactivateTimer(false, timerToUsing);
    }

    public void GetTimerToUsingByIndex(int index)
    {
        int initalTimerPlus = 13;
        switch (index)
        {
            case 0:
                timerToUsing = 5;
                break;
            case 1:
                timerToUsing = 5;
                break;
            case 2:
                timerToUsing = 5;
                break;
        }
        timerToUsing += initalTimerPlus;
    }

    public void LaunchPowerByindex(int index)
    {
        switch (index)
        {
            case 0:
                if (!isInvisible)
                {
                    InvisibilityPower();
                    gameManager.ui_Manager.DisplayN2PotionObject(true);
                    DisplayInvisibleResetButton(true);
                }
                else {
                    ResetInvisiblityPower();
                    DisplayButtonCanUsed(false);
                    DisplayInvisibleResetButton(false);
                    gameManager.ui_Manager.DisplayN2PotionObject(false);
                    gameManager.ui_Manager.DesactivateObjectPowerImpostor();
                    powerIsUsed = true;
                }
                break;
            case 1:
                MurderPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor();
                break;
            case 2:
                CursedPower();
                powerIsUsed = true;
                gameManager.ui_Manager.DesactivateObjectPowerImpostor();
                break;
            case 4:
                if (!isOtherToInvisible)
                {
                    InvisibilityPowerOtherPlayer();
                    gameManager.ui_Manager.DisplayN2PotionObject(true);
                    DisplayInvisibleResetButton(true);
                    isOtherToInvisible = true;
                }    
                else{
                    ResetOtherInvisiblityPower();
                    DisplayInvisibleResetButton(false);
                    DisplayButtonCanUsed(false);
                    gameManager.ui_Manager.DisplayN2PotionObject(false);
                    gameManager.ui_Manager.DesactivateObjectPowerImpostor();
                    powerIsUsed = true;
                    isOtherToInvisible = false;
                }
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
        isInvisible = true;
    }

    public void ResetInvisiblityPower()
    {
        player.GetComponent<PlayerNetwork>().SendColorInvisible(false);
    }

    public void ResetOtherInvisiblityPower()
    {
        if(collision)
            collision.transform.parent.GetComponent<PlayerNetwork>().SendColorInvisible(false);
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


    public void GetAllSituationToResetInvisiblity()
    {
        if (!canUsed)
            return;
        if (powerIsReset)
            return;
        if (gameManager.timer.timerLaunch)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        }
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        };
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            ResetInvisiblityPower();
            ResetOtherInvisiblityPower();
            DisplayInvisibleResetButton(false);
            DisplayButtonCanUsed(false);
            gameManager.ui_Manager.DisplayN2PotionObject(false);
            gameManager.ui_Manager.DesactivateObjectPowerImpostor();
            powerIsReset = true;
            return;
        }

    }
    public void GetAllSituationToCanUsed()
    {
        if (gameManager.timer.timerLaunch)
        {
            cantTemporyUsed = true;
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            cantTemporyUsed = true;
            return;
        };
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            cantTemporyUsed = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            cantTemporyUsed = true;
            return;
        }

        cantTemporyUsed = false;
    }

    public void ChangeScaleByPlayer()
    {
        float scale_x = this.transform.parent.Find("Perso").localScale.x;
        if (scale_x > 0)
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        else
            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
    }

    public void DisplayButtonCanUsed(bool display)
    {
        if (powerIsUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate();
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
        if (!canUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate();
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
        if(cantTemporyUsed)
        {
            gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate();
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
        gameManager.ui_Manager.HideObjectPowerButtonDesactivate();
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
