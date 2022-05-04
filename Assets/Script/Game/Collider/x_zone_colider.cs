using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class x_zone_colider : MonoBehaviour
{
    public int nbVote = 0;

    public GameManager gameManager;
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
        if (!collision.GetComponent<PhotonView>())
        {
            return;
        }
        if (!gameManager.timer.timerLaunch)
        {
            return;
        }

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
        if (!collision.GetComponent<PhotonView>())
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
        if (!collision.GetComponent<PhotonView>())
        {
            return;
        }
        if (!gameManager.timer.timerLaunch)
        {
            return;
        }

        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoorX(collision.GetComponent<PhotonView>().ViewID, false, true);

    }
}
