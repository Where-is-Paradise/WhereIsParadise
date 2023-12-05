using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathNpcRoom : TrialsRoom
{
    public GameManager gameManager;
    public Death_NPC death_NPC;
    public Death_NPC death_NPC_2;
    public bool loose = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLeverToRelauch()
    {
        if (!GameObject.FindGameObjectWithTag("GodDeath"))
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 4);
        }
    }
    public IEnumerator StartDeathNPCRoomAfterTeleportation()
    {
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.CloseDoorWhenVote(true);
        gameManager.InstantiateDeathNPC(1);
        gameManager.InstantiateDeathNPC(2);
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManagerParent.DisplayTorchBarre(false);
        
        yield return new WaitForSeconds(2);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.deathNPCIsLaunch = true;
        gameManager.ActivateCollisionTPOfAllDoor(false);
        StartDeathNPCRoom();
    }



    public void StartDeathNPCRoom()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (!gameManager.deathNPCIsLaunch)
        {
            return;
        }
        loose = false;
        photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, false);
        death_NPC = GameObject.Find("DeathNPC_1").GetComponent<Death_NPC>();
        death_NPC_2 = GameObject.Find("DeathNPC_2").GetComponent<Death_NPC>();
        StartCoroutine(death_NPC.RandomScenario());
        StartCoroutine(death_NPC_2.RandomScenario());
        float randomTimer = Random.Range(25, 80);
        photonView.RPC("SendSpeciallyIsLaucnh", RpcTarget.All, randomTimer);
    }

    [PunRPC]
    public void SendSpeciallyIsLaucnh(float randomTimer)
    {
        gameManager.speciallyIsLaunch = true;
        gameManager.deathNPCIsLaunch = true;
        gameManager.ActivateCollisionTPOfAllDoor(false);
        StartCoroutine(CouroutineEndGame(randomTimer));
        DisplayTimer(randomTimer);
    }
    public void DisplayTimer(float randomTimer)
    {
        gameManager.ui_Manager.DisplayKeyAndTorch(false);
        this.transform.Find("X_zone_animation").gameObject.SetActive(true);
        this.transform.Find("X_zone_animation").Find("Timer").GetComponent<Timer>().LaunchTimer(randomTimer, true);
        this.transform.Find("X_zone_animation").GetComponent<Animator>().speed = (this.transform.Find("X_zone_animation").GetComponent<Animator>().speed / (CalculAnimationSpeed(randomTimer)));
    }
    public void HideTimer()
    {
        this.transform.Find("X_zone_animation").gameObject.SetActive(false);
        this.transform.Find("X_zone_animation").GetComponent<Animator>().speed = 1;
        this.transform.Find("X_zone_animation").Find("Timer").GetComponent<Timer>().ResetTimer();
        gameManager.ui_Manager.DisplayKeyAndTorch(true);
    }


    public void SendHideTimer()
    {
        photonView.RPC("SetHideTimer", RpcTarget.All);
    }

    [PunRPC]
    public void SetHideTimer()
    {
        HideTimer();
    }

    public float CalculAnimationSpeed(float timer)
    {
        return timer / 60;
    }

    public void GiveAward(int randomInt)
    {
        gameManager.teamHasWinTrialRoom = true;
        DisplayGloballyAward(randomInt);
    }
    public IEnumerator CouroutineEndGame(float secondes)
    {
        yield return new WaitForSeconds(secondes);
        if (!loose && gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
           
            photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
            photonView.RPC("SendVictoryTeam", RpcTarget.All, Random.Range(0, 2));
            SendDesactivateNPC();
            gameManager.deathNPCIsLaunch = false;
        }
      
    }
    [PunRPC]
    public void SendVictoryTeam(int randomInt)
    {
        if (death_NPC && death_NPC_2)
        {
            death_NPC.DisplayTargetImg(false);
            death_NPC_2.DisplayTargetImg(false);
        }
        GiveAward(randomInt);
        HideTimer();
    }

    [PunRPC]
    public void SendIgnoreCollisionPlayer(bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(ignore);
    }

    public void SendDesactivateNPC()
    {
        photonView.RPC("DesactivateNPC", RpcTarget.All);
    }

    [PunRPC]
    public void DesactivateNPC()
    {
        if(death_NPC && death_NPC_2)
        {
            death_NPC.SendHideAndResetNPC();
            death_NPC_2.SendHideAndResetNPC();

        }
        HideTargetOfAllPlayer();
    }

    public void HideTargetOfAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.transform.Find("TargetImgInDeathRoom").gameObject.SetActive(false);
        }
    }
}
