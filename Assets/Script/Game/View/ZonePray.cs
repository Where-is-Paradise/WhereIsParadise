using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePray : MonoBehaviourPun
{
    public bool onePlayerPray = false;
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
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
            return;

        photonView.RPC("SetOnePlayerPray", RpcTarget.All, true);
        SetColorPlayerZone(collision.transform.parent.GetComponent<PlayerGO>(), true);

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "CollisionTrigerPlayer")
            return;
        if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
            return;
        if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
            return;

        photonView.RPC("SetOnePlayerPray", RpcTarget.All, true);
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

        photonView.RPC("SetOnePlayerPray", RpcTarget.All, false);
        SetColorPlayerZone(collision.transform.parent.GetComponent<PlayerGO>(), false);
    }

    [PunRPC]
    public void SetOnePlayerPray(bool onePlayerPray)
    {
        this.onePlayerPray = onePlayerPray;
    }

    public void SetColorPlayerZone(PlayerGO player, bool display)
    {
        player.GetComponent<PlayerNetwork>().SendDisplayWhiteLight(display);
    }
}
