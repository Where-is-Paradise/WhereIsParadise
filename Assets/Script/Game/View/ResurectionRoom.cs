using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResurectionRoom : MonoBehaviourPun
{
    public GameObject playerRevive;
    public GameManager gameManager;
    public GameObject spawnResurection;
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
        List<GameObject> listSacrifiedplayer = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                listSacrifiedplayer.Add(player);
        }
        if(listSacrifiedplayer.Count > 0)
        {
            int randomIndexPlayer = Random.Range(0, listSacrifiedplayer.Count);
            photonView.RPC("SendRandomSacrificedPlayer", RpcTarget.All, listSacrifiedplayer[randomIndexPlayer].GetComponent<PhotonView>().ViewID);
            return;
        }
        photonView.RPC("RelaunchRoom", RpcTarget.All);

    }

    public void RevivePlayer()
    {
        playerRevive.GetComponent<PlayerNetwork>().SendResetSacrifice();
    }

    [PunRPC]
    public void  SendRandomSacrificedPlayer(int indexPlayer)
    {
        playerRevive =  gameManager.GetPlayer(indexPlayer);
        playerRevive.transform.position = spawnResurection.transform.position;
        RevivePlayer();
        ResetRoom();
    }

    [PunRPC]
    public void RelaunchRoom()
    {
        gameManager.UpdateSpecialsRooms(this.gameManager.game.currentRoom);
    }

    [PunRPC]
    public void ResetRoom()
    {
        playerRevive = null;
        this.gameManager.game.currentRoom.speciallyPowerIsUsed = true;
    }
}
