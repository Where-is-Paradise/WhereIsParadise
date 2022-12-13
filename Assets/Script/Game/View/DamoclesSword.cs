using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamoclesSword : MonoBehaviourPun
{
    public DamoclesSwordRoom damoclesRoom;
    public bool canChangePlayer = true;

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
            if (!canChangePlayer)
                return;
            if (collision.transform.parent.GetComponent<PlayerGO>().isDeadBySwordDamocles)
                return;
            int indexplayer = collision.transform.parent.gameObject.GetComponent<PhotonView>().ViewID;
            damoclesRoom.SendCurrentPlayer(indexplayer);
            damoclesRoom.SendChangePositionAtPlayer(indexplayer);
            SendCanChangePlayer(false);
            StartCoroutine(CanChangePlayerCoroutine());
            
        }
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
