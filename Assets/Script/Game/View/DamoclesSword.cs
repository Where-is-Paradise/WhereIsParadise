using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamoclesSword : MonoBehaviourPun
{
    public DamoclesSwordRoom damoclesRoom;
    public bool canChangePlayer = true;
    private bool canKillPlayer = true;

    public void Update()
    {
        if (GameObject.Find("Waiting_map"))
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!damoclesRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().damoclesSwordIsAbove)
            return;
        if (!damoclesRoom.canChangePlayer)
            return;
        if (!damoclesRoom.gameManager.damoclesIsLaunch)
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (!damoclesRoom.currentPlayer)
                return;
            if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
                return;
            if (collision.transform.parent.GetComponent<PhotonView>().ViewID == damoclesRoom.currentPlayer.GetComponent<PhotonView>().ViewID)
                return;
            if (collision.transform.parent.GetComponent<PlayerGO>().isTouchInTrial)
                return;
            int indexplayer = collision.transform.parent.gameObject.GetComponent<PhotonView>().ViewID;
            photonView.RPC("SendChangeToBoss", RpcTarget.All, indexplayer);  
        }
    }

    [PunRPC]
    public void SendChangeToBoss(int indexPlayer)
    {
        if (!damoclesRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        if (!damoclesRoom.canChangePlayer)
            return;
        if (!canChangePlayer)
            return;
        Debug.LogError(" sa passe sendChangeToBoss ");
        damoclesRoom.SendChangePositionAtPlayer(indexPlayer);
        damoclesRoom.SetCurrentPlayer(indexPlayer);
        damoclesRoom.SendCurrentPlayer(indexPlayer);
       
        SendCanChangePlayer(false);
        StartCoroutine(CanChangePlayerCoroutine());

    }

    public void SendCanChangePlayer(bool canChangePlayer)
    {
        photonView.RPC("SetCanChangePlayer", RpcTarget.All, canChangePlayer);
    }

    [PunRPC]
    public void SetCanChangePlayer(bool canChangePlayer)
    {
        this.canChangePlayer = canChangePlayer;
    }

   public IEnumerator CanChangePlayerCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        SendCanChangePlayer(true);
    }

}
