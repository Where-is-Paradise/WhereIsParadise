using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LostTorchRoom : MonoBehaviourPun
{

    public GameObject listSpawn;
    public GameManager gameManager;
    public bool timerFinish = false;
    public LostTorch lostTorch;
    // Start is called before the first frame update
    void Start()
    {
        lostTorch = this.transform.Find("Torch").GetComponent<LostTorch>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartLostTorchRoom()
    {
        StartCoroutine(LaunchLostTorchRoomAfterTeleportation());
    }

    public IEnumerator LaunchLostTorchRoomAfterTeleportation()
    {
        yield return new WaitForSeconds(2);
        SpawnLostTorch();
        StartCoroutine(TimerCouroutine());
    }
    public void SpawnLostTorch()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        int randomSpawnIndex = Random.Range(0, listSpawn.transform.childCount);
        photonView.RPC("SendSpawn", RpcTarget.All, randomSpawnIndex);
    }

    [PunRPC]
    public void SendSpawn(int indexSpawn)
    {
        GameObject spawn = listSpawn.transform.GetChild(indexSpawn).gameObject;
        this.transform.Find("Torch").gameObject.SetActive(true);
        this.transform.position = spawn.transform.position;
    }

    public IEnumerator TimerCouroutine()
    {
        yield return new WaitForSeconds(30);
        timerFinish = true;
        photonView.RPC("SendEndGame", RpcTarget.All);
    }

    [PunRPC]
    public void SendEndGame()
    {
        AssignAwardToPlayer();
        DesactivateLostTorchRoom();
    }

    public void DesactivateLostTorchRoom()
    {
        lostTorch.currentPlayer = null;
        lostTorch.transform.parent = this.transform;
        lostTorch.gameObject.SetActive(false);
        lostTorch.transform.position = new Vector3(0, 0);
        lostTorch.transform.Find("CollisionTorch").GetComponent<CapsuleCollider2D>().enabled = true;
        timerFinish = false;
        this.gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
    }

    public void AssignAwardToPlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PlayerGO PlayerWiner = lostTorch.currentPlayer;
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, PlayerWiner.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        gameManager.game.nbTorch++;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }
}
