using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CollisionTorch : MonoBehaviourPun
{
    public bool isTaken;
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
        if(collision.tag == "CollisionTrigerPlayer")
        {
            if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
                return;
            isTaken = true;
            CollisionWithPlayer(collision);
        }
    }

    public void  CollisionWithPlayer(Collider2D collision)
    {
        photonView.RPC("ChangeParent", RpcTarget.All , collision.transform.parent.GetComponent<PhotonView>().ViewID);
        photonView.RPC("DesactivateCollider", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeParent(int indexPlayer) 
    {
        this.transform.parent.parent = gameManager.GetPlayer(indexPlayer).transform;
        this.transform.parent.GetComponent<LostTorch>().currentPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        this.transform.parent.localPosition = new Vector3(0, 0);
        this.transform.parent.localPosition += new Vector3(0.3f, 0);
    }

    [PunRPC]
    public void DesactivateCollider()
    {
        this.GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
