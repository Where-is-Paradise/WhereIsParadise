using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResurectionRoom : MonoBehaviourPun
{
    public GameObject playerRevive;
    public GameManager gameManager;
    public GameObject spawnResurection;
    List<GameObject> listSacrifiedplayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchResurectionRoom()
    {
        StartCoroutine(LaunchResurationRoomCouroutine());
    }

    public IEnumerator LaunchResurationRoomCouroutine()
    {
        yield return new WaitForSeconds(2);
        GetRandomSacrificedPlayer();
    }

    public void GetRandomSacrificedPlayer()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        listSacrifiedplayer = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice )
                listSacrifiedplayer.Add(player);
        }
        if(listSacrifiedplayer.Count > 0)
        {
            int randomIndexPlayer = Random.Range(0, listSacrifiedplayer.Count);
            gameManager.gameManagerNetwork.SendRandomSacrificePlayer(listSacrifiedplayer[randomIndexPlayer].GetComponent<PhotonView>().ViewID);
            return;
        }
        gameManager.gameManagerNetwork.SendRelaunchRoom();
        gameManager.CloseDoorWhenVote(false);

    }



  

}
