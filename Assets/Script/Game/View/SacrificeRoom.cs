using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeRoom : MonoBehaviour
{

    public bool sacrificeVoteIsLaunch = false;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LaunchTimerVote()
    {
        StartCoroutine(CouroutineTimerVote());
    }

    public IEnumerator CouroutineTimerVote()
    {
        gameManager.gameManagerNetwork.SendSacrificeVoteIsLaunch(true);
        gameManager.TeleportAllPlayerInRoomOfBoss();
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.gameManagerNetwork.SendTimerForcePause(true);
        gameManager.gameManagerNetwork.SendDisplayTimerForce(false);
        gameManager.gameManagerNetwork.SendChronoSacrifice();
        gameManager.CloseDoorWhenVoteCoroutine(true);

        yield return new WaitForSeconds(10f);
        GameObject player = GetPlayerWithMaxVote();
        player.GetComponent<PlayerNetwork>().SendDeathSacrifice(true);
        player.GetComponent<PlayerNetwork>().SendResetVoteSacrifice();
        gameManager.gameManagerNetwork.SendSacrificeVoteIsLaunch(false);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoor(true);
        gameManager.gameManagerNetwork.SendTimerForcePause(false);
        gameManager.gameManagerNetwork.SendDisplayTimerForce(true);
    }



    public GameObject GetPlayerWithMaxVote()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int max = -1;
        GameObject indexPlayer  = null;
        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerGO>().nbVoteSacrifice > max)
            {
                max = player.GetComponent<PlayerGO>().nbVoteSacrifice;
                indexPlayer = player;
            }
        }
        return indexPlayer;
    }

   

   
}
