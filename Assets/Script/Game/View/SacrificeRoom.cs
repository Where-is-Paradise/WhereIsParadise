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
        yield return new WaitForSeconds(10f);
        GameObject player = GetPlayerWithMaxVote();
        player.GetComponent<PlayerNetwork>().SendDeathSacrifice();
        player.GetComponent<PlayerNetwork>().SendResetVoteSacrifice();
        GameObject.Find("GameManager").GetComponent<GameManager>().gameManagerNetwork.SendSacrificeVoteIsLaunch(false);
        //player.GetComponent<PlayerGO>().gameManager.gameManagerNetwork.SendKey();
  
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
