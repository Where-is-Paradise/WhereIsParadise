using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class ObjectImpostor : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerGO player;
    public int indexPower = 0;
    public bool powerIsUsed = false;
    public int timerToUsing = 0;
    public bool canUsed = false;
    public bool powerIsReset = false;
    public Collider2D collision;
    public bool isClickedInButtonPower = false;
    public bool isInvisible = false;
    public bool isNearOfPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.GetComponent<PlayerGO>();
        gameManager = player.gameManager;
        GetTimerToUsingByIndex(indexPower);
        DisplayButtonDesactivateTimer(true, timerToUsing);
        StartCoroutine(CoroutineSetCanUsed());
    }

    // Update is called once per frame
    void Update()
    {
        if (powerIsReset)
            return;
        if(indexPower == 0)
            GetAllSituationToResetInvisiblity();

        GetAllSituationToCanUsed();
        ChangeScaleByPlayer();

        if (indexPower != 0)
        {
            if (isNearOfPlayer && canUsed)
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
        isNearOfPlayer = true;
        if (isClickedInButtonPower)
        {
            if (indexPower == 0)
                return;
            
            this.collision = collision;
            LaunchPowerByindex(indexPower);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "CollisionTrigerPlayer")
            isNearOfPlayer = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "CollisionTrigerPlayer")
            isNearOfPlayer = false;
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
                timerToUsing = 60;
                break;
            case 1:
                timerToUsing = 250;
                break;
            case 2:
                timerToUsing = 120;
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
                    DisplayButtonCanUsed(true);
                }
                else {
                    ResetInvisiblityPower();
                    DisplayButtonCanUsed(false);
                    gameManager.ui_Manager.DisplayN2PotionObject(false);
                    gameManager.ui_Manager.DesactivateObjectPowerImpostor();
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
        }
    }
    public void InvisibilityPower()
    {
        player.GetComponent<PlayerNetwork>().SendColorInvisible(true);
        isInvisible = true;
    }
    public void ResetInvisiblityPower()
    {
        player.GetComponent<PlayerNetwork>().SendColorInvisible(false);
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
        if (gameManager.timer.timerLaunch)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        }
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        };
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            ResetInvisiblityPower();
            powerIsReset = true;
            return;
        }

    }
    public void GetAllSituationToCanUsed()
    {
        if (gameManager.timer.timerLaunch)
        {
            canUsed = false;
            return;
        }
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            canUsed = false;
            return;
        }
        if (gameManager.speciallyIsLaunch)
        {
            ResetInvisiblityPower();
            canUsed = false;
            return;
        };
        if (gameManager.voteDoorHasProposed)
        {
            ResetInvisiblityPower();
            canUsed = false;
            return;
        }
        if (player.GetComponent<PlayerGO>().hasWinFireBallRoom)
        {
            ResetInvisiblityPower();
            canUsed = false;
            return;
        }
        if (player.GetComponent<PlayerGO>().haveToGoToExpedition)
        {
            ResetInvisiblityPower();
            canUsed = false;
            return;
        }
        canUsed = true;
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
            gameManager.ui_Manager.DisplayObjectPowerBigger(false);
            return;
        }
        gameManager.ui_Manager.DisplayObjectPowerBigger(display);
    }
    public void DisplayButtonDesactivateTimer(bool display , float timer)
    {
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivateTime(display, timer);
    }

}
