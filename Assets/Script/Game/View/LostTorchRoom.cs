using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LostTorchRoom : TrialsRoom
{

    public GameObject listSpawn;
    public GameManager gameManager;
    public bool timerFinish = false;
    public LostTorch lostTorch;
    // Start is called before the first frame update
    void Start()
    {
        lostTorch = this.transform.Find("LostTorch").GetComponent<LostTorch>();
        
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
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.CloseDoorWhenVote(true);
        gameManagerParent.DisplayTorchBarre(false);
        gameManagerParent.ui_Manager.DisplayInteractionObject(false);
        yield return new WaitForSeconds(2);
        
        SpawnLostTorch();
        LaunchTimer();
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            SendObstalceGroup();

    }
    public void SpawnLostTorch()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int randomSpawnIndex = Random.Range(0, listPlayer.Length);
        photonView.RPC("SendSpawn", RpcTarget.All, listPlayer[randomSpawnIndex].GetComponent<PhotonView>().ViewID);
       
    }

    [PunRPC]
    public void SendSpawn(int indexPlayer)
    {
        GameObject spawn = gameManager.GetPlayer(indexPlayer);
        lostTorch.gameObject.SetActive(true);
        lostTorch.transform.parent = spawn.transform;
        lostTorch.currentPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
    }

    public void LaunchTimer()
    {
        StartCoroutine(TimerCouroutine());
    }
    public IEnumerator TimerCouroutine()
    {
        yield return new WaitForSeconds(150);
        timerFinish = true;
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            photonView.RPC("SendEndGame", RpcTarget.All);
    }

    [PunRPC]
    public void SendEndGame()
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            if (!lostTorch.currentPlayer)
            {
                GetAward(gameManager.GetRandomPlayerID());
                return;
            }
            GetAward(lostTorch.currentPlayer.GetComponent<PhotonView>().ViewID);
            DesactivateRoom();
        }
        DesactivateLostTorchRoom();
        SendResetObstacle();
    }

    public void DesactivateLostTorchRoom()
    {
        lostTorch.currentPlayer = null;
        lostTorch.transform.parent = this.transform;
        lostTorch.gameObject.SetActive(false);
        lostTorch.transform.localPosition = new Vector3(0, 0);
        timerFinish = false;
    }
}
