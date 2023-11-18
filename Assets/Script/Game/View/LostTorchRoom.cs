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
        yield return new WaitForSeconds(2);
        SpawnLostTorch();
        gameManager.speciallyIsLaunch = true;
      
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
       

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            SendObstalceGroup();
            //gameManager.gameManagerNetwork.SendActivateAllObstacles(true, this.name);

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
        lostTorch.gameObject.SetActive(true);
        lostTorch.transform.position = spawn.transform.position;
    }

    public void LaunchTimer()
    {
        StartCoroutine(TimerCouroutine());
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
        //this.GetAward();

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
