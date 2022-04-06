using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDoorCollider : MonoBehaviour
{
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
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine &&  gameManager.timer.timerLaunch)
        {
            int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
            gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, true, false);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine && gameManager.timer.timerLaunch)
        {
            int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
            gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, false, false);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine &&  gameManager.timer.timerLaunch)
        {
            int parentDoorIndex = this.transform.parent.transform.parent.GetComponent<Door>().index;
            gameManager.gameManagerNetwork.SendCollisionZoneVoteDoor(collision.gameObject.GetComponent<PhotonView>().ViewID, parentDoorIndex, false, true);
        }
    }

}
