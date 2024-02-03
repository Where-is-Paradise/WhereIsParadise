using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallRoom : TrialsRoom
{
    public GameManager gameManager;
    public float frequencyRang = 0.1f;
    public float frequency = 4;
    public bool roomIsLaunch = false;
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
        if (!GameObject.FindGameObjectWithTag("FireBall") && gameManager.fireBallIsLaunch)
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 2);
            gameManager.fireBallIsLaunch = false;
        }
    }

    public void LanchFireBallRoom()
    {
        gameManager.speciallyIsLaunch = true;
        gameManager.CloseDoorWhenVote(true);
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManagerParent.DisplayTorchBarre(false);
        gameManager.ui_Manager.LaunchFightMusic();
        DisplayHeartsFoAllPlayer(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        roomIsLaunch = true;
        StartCoroutine(ShotFireBallFrequency());
        StartCoroutine(ReduceFrequencyCouroutine());
        photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, false);

    }

    public void ChooseRandomTurret()
    {
        int randomInt = Random.Range(0, 4);
        GameObject turret = this.transform.Find("Turrets").GetChild(randomInt).gameObject;
        turret.GetComponent<Turret>().ShotFireBall();
    }

    public IEnumerator ShotFireBallFrequency()
    {
        if (roomIsLaunch)
        {
            yield return new WaitForSeconds(frequency);
            if (roomIsLaunch)
            {
                ChooseRandomTurret();
                StartCoroutine(ShotFireBallFrequency());
            }
        }
    }

    public IEnumerator ReduceFrequencyCouroutine()
    {
        yield return new WaitForSeconds(4);
        if(frequency > 0.5f)
            frequency -= 0.10f;

        StartCoroutine(ReduceFrequencyCouroutine());
    }

    public void DesactivateFireBallRoom()
    {
        DestroyAllFireball();
        roomIsLaunch = false;
        gameManager.speciallyIsLaunch = false;
        photonView.RPC("SendIgnoreCollisionPlayer", RpcTarget.All, true);
    }

    [PunRPC]
    public void SendIgnoreCollisionPlayer(bool ignore)
    {
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().IgnoreCollisionAllPlayer(ignore);
    }

    public void DestroyAllFireball()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        for(int i=0; i < transform.transform.Find("Turrets").childCount;i++)
        {
            transform.Find("Turrets").GetChild(i).GetComponent<Turret>().DestroyFireBalls();
        }
    }

    public void DisplayHeartsFoAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
                continue;

            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }

}
