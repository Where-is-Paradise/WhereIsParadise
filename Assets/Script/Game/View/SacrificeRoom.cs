using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeRoom : MonoBehaviour
{

    public bool sacrificeVoteIsLaunch = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
        GameObject.Find("GameManager").GetComponent<GameManager>().gameManagerNetwork.SendSacrificeVoteIsLaunch(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().TeleportAllPlayerInRoomOfBoss();
        GameObject.Find("GameManager").GetComponent<GameManager>().gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        GameObject.Find("GameManager").GetComponent<GameManager>().PauseTimerFroce(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().ui_Manager.soundChrono_10sec.Play();
        yield return new WaitForSeconds(10f);
        GameObject player = GetPlayerWithMaxVote();
        player.GetComponent<PlayerNetwork>().SendDeathSacrifice(true);
        player.GetComponent<PlayerNetwork>().SendResetVoteSacrifice();
        GameObject.Find("GameManager").GetComponent<GameManager>().gameManagerNetwork.SendSacrificeVoteIsLaunch(false);
        //player.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.SendKey();
        GameObject.Find("GameManager").GetComponent<GameManager>().gameManagerNetwork.DisplayLightAllAvailableDoor(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().PauseTimerFroce(false);
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
