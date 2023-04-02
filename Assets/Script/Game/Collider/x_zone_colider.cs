using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class x_zone_colider : MonoBehaviour
{
    public int nbVote = 0;

    public GameManager gameManager;
    public bool canSend = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
        if(nbVote < 0)
        {
            nbVote = 0;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (collision.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.timer.timerLaunch)
        {
            return;
        }
        StartCoroutine(CouroutineTimerCanSend());
        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoorX(collision.GetComponent<PhotonView>().ViewID, true, false);

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (collision.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.timer.timerLaunch)
        {
            return;
        }
        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoorX(collision.GetComponent<PhotonView>().ViewID, false, false);

    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player"))
        {
            return;
        }
        if (collision.GetComponent<PlayerGO>().isSacrifice)
        {
            return;
        }
        if (!collision.GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        if (!gameManager.timer.timerLaunch)
        {
            return;
        }

        if (canSend)
        {
            gameManager.gameManagerNetwork.SendCollisionZoneVoteDoorX(collision.GetComponent<PhotonView>().ViewID, false, true);
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
