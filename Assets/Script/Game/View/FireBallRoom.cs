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
        GameObject turret = this.transform.GetChild(randomInt).gameObject;
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
        yield return new WaitForSeconds(3);
        if(frequency > 0.25f)
            frequency -= 0.15f;

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
        for(int i=0; i < transform.childCount;i++)
        {
            transform.GetChild(i).GetComponent<Turret>().DestroyFireBalls();
        }
    }

}
