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
            float sign_x = this.transform.parent.transform.Find("Perso").localScale.x / Mathf.Abs(this.transform.parent.transform.Find("Perso").localScale.x);
            this.transform.localScale = new Vector3(-sign_x * Mathf.Abs(this.transform.localScale.x),this.transform.localScale.y);
            if(this.transform.localScale.x < 0)
            {
                this.transform.localPosition = new Vector3(-0.3f, 0);
            }
            else
            {
                this.transform.localPosition = new Vector3(0.3f, 0);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!currentPlayer)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID != currentPlayer.GetComponent<PhotonView>().ViewID)
            return;
        if (!canChangePlayer)
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (collision.transform.parent.GetComponent<PlayerGO>().isSacrifice)
                return;
            if (collision.transform.parent.GetComponent<PhotonView>().ViewID == currentPlayer.GetComponent<PhotonView>().ViewID)
                return;
            photonView.RPC("ChangeCurrentPlayer", RpcTarget.All, collision.transform.parent.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    public void ChangeCurrentPlayer(int indexPlayer)
    {
        this.currentPlayer = gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>();
        this.transform.parent = gameManager.GetPlayer(indexPlayer).transform;
        this.transform.localPosition = new Vector3(0, 0);
        this.transform.localPosition += new Vector3(0.3f, 0);
        canChangePlayer = false;
        StartCoroutine(CanChangePlayerCouroutine());
    }

    public IEnumerator CanChangePlayerCouroutine()
    {
        yield return new WaitForSeconds(0.75f);
        photonView.RPC("SetCanChangePlayer", RpcTarget.All, true);
    }

    [PunRPC]
    public void SetCanChangePlayer(bool canChangePlayer)
    {
        this.canChangePlayer = canChangePlayer;
    }
}
