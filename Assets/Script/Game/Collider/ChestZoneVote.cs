using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestZoneVote : MonoBehaviour
{
    public int nbVote = 0;
    public int indexChest = 0;

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
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine && gameManager.voteChestHasProposed)
        {
            gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, true, false);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine && gameManager.voteChestHasProposed)
        {
            gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, false, false);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine && gameManager.voteChestHasProposed)
        {
            gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, false, true);
        }
    }
}
