using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestZoneVote : MonoBehaviour
{
    public int nbVote = 0;
    public int indexChest = 0;

    public GameManager gameManager;
    public bool canSend = true;
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
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.voteChestHasProposed)
        {
            return;
        }
        if (collision.gameObject.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        StartCoroutine(CouroutineTimerCanSend());
        gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, true, false);

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.voteChestHasProposed)
        {
            return;
        }
        if (collision.gameObject.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, false, false);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.voteChestHasProposed)
        {
            return;
        }
        if (collision.gameObject.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        if (canSend)
        {
            gameManager.gameManagerNetwork.SendCollisionChestVote(collision.GetComponent<PhotonView>().ViewID, indexChest, false, true);
            canSend = false;
        }
                
    }

    public IEnumerator CouroutineTimerCanSend()
    {
        yield return new WaitForSeconds(0.5f);
        canSend = true;
        if (gameManager.timer.timerLaunch)
            StartCoroutine(CouroutineTimerCanSend());
    }
}
