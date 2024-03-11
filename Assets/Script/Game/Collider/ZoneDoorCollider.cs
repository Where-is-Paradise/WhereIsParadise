using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDoorCollider : MonoBehaviour
{
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
        if (!gameManager.canVoteDoor)
            return;
        StartCoroutine(CouroutineTimerCanSend());
        int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, true, false);
 
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
        if (!gameManager.canVoteDoor)
            return;
        int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
        gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, false, false);

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
        if (!gameManager.canVoteDoor)
            return;

        if (canSend)
        {
            int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
            gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, false, true);
            canSend = false;
        }
    }


    public IEnumerator CouroutineTimerCanSend()
    {
        yield return new WaitForSeconds(0.2f);
        canSend = true;
        if(gameManager.timer.timerLaunch)
            StartCoroutine(CouroutineTimerCanSend());
    }
}
