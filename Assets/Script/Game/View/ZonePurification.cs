using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePurification : MonoBehaviourPun
{

    public PlayerGO currentPlayer;
    public PlayerGO secondPlayer;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (secondPlayer)
            return;
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
            return;
        if (currentPlayer && !currentPlayer.GetComponent<PhotonView>().IsMine)
        {
            photonView.RPC("SendSecondPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
            return;
        }
        photonView.RPC("SendCurrentPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
        SetColorPlayerZone(collision.transform.parent.GetComponent<PlayerGO>(), true);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (secondPlayer)
            return;
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
            return;
        if (currentPlayer && !currentPlayer.GetComponent<PhotonView>().IsMine)
        {
            photonView.RPC("SendSecondPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
            return;
        }
        photonView.RPC("SendCurrentPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
        SetColorPlayerZone(collision.transform.parent.GetComponent<PlayerGO>(), true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
            return;
        if (secondPlayer && currentPlayer.GetComponent<PhotonView>().IsMine)
        {
            SetColorPlayerZone(currentPlayer.GetComponent<PlayerGO>(), false);
            currentPlayer = secondPlayer;
            photonView.RPC("SendCurrentPlayer", RpcTarget.All, currentPlayer.GetComponent<PhotonView>().ViewID);
            photonView.RPC("ResetSecondPlayer", RpcTarget.All);
            SetColorPlayerZone(currentPlayer.GetComponent<PlayerGO>(), true);
            return;
        }
        if (secondPlayer && secondPlayer.GetComponent<PhotonView>().IsMine)
        {
            SetColorPlayerZone(secondPlayer.GetComponent<PlayerGO>(), false);
            photonView.RPC("ResetSecondPlayer", RpcTarget.All);
            return;
        }
        if (!secondPlayer)
        {
            photonView.RPC("ResetCurrentPlayer", RpcTarget.All);
            SetColorPlayerZone(collision.transform.parent.GetComponent<PlayerGO>(), false);
        }
    }


    public void SetColorPlayerZone(PlayerGO player , bool display)
    {
        player.GetComponent<PlayerNetwork>().SendDisplayWhiteLight(display);
    }

    [PunRPC]
    public void SendCurrentPlayer( int indexPlayer)
    {
        currentPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
    }
    [PunRPC]
    public void SendSecondPlayer(int indexPlayer)
    {
        secondPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
    }

    [PunRPC]
    public void ResetCurrentPlayer()
    {
        currentPlayer = null;
    }
    [PunRPC]
    public void ResetSecondPlayer()
    {
        secondPlayer = null;
    }
}
