using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostTorch : MonoBehaviourPun
{

    public PlayerGO currentPlayer;
    public GameManager gameManager;
    public bool canChangePlayer = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer)
        {
            float sign_x = currentPlayer.gameObject.transform.Find("Skins").GetChild(currentPlayer.indexSkin).localScale.x / Mathf.Abs(currentPlayer.gameObject.transform.Find("Skins").GetChild(currentPlayer.transform.GetComponent<PlayerGO>().indexSkin).localScale.x);
            this.transform.localScale = new Vector3(-sign_x * Mathf.Abs(this.transform.localScale.x),this.transform.localScale.y);
            if (this.transform.localScale.x < 0)
            {
                this.transform.localPosition = new Vector3(-0.4f, 0);
            }
            else
            {
                this.transform.localPosition = new Vector3(0.4f, 0);
            }
        }
        if (!this.transform.parent)
        {
            if (GameObject.Find("Waiting_map"))
            {
                Destroy(this.gameObject);
                return;
            }
            this.transform.parent = GameObject.Find("LostTorchRoom").transform;
        }
            
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canChangePlayer)
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (!currentPlayer)
            {
                photonView.RPC("ChangeCurrentPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
                return;
            }
            if (collision.transform.parent.GetComponent<PhotonView>().ViewID == currentPlayer.GetComponent<PhotonView>().ViewID)
                return;
            if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
                return;
            photonView.RPC("ChangeCurrentPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    public void ChangeCurrentPlayer(int indexPlayer)
    {
        this.currentPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        this.transform.parent = this.currentPlayer.transform;
        canChangePlayer = false;
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            StartCoroutine(CanChangePlayerCouroutine());
    }

    public IEnumerator CanChangePlayerCouroutine()
    {
        yield return new WaitForSeconds(0.75f);
        photonView.RPC("SetCanChangePlayer", RpcTarget.All, true);
    }

    public IEnumerator CanChangePlayerCouroutineOnlyMine()
    {
        this.canChangePlayer = false;
        yield return new WaitForSeconds(2f);
        this.canChangePlayer = true;
    }

    [PunRPC]
    public void SetCanChangePlayer(bool canChangePlayer)
    {
        this.canChangePlayer = canChangePlayer;
    }
}
