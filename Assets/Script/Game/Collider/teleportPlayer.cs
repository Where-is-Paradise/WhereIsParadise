using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportPlayer : MonoBehaviour
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
        if (collision.gameObject.tag == "CollisionTrigerPlayer")
        {
            PlayerGO player = collision.transform.parent.GetComponent<PlayerGO>();
            if (player.GetComponent<PhotonView>().IsMine)
            {
                if (this.transform.parent.GetComponent<Door>().isOpenForAll && !player.haveToGoToExpedition)
                {
                    if (!gameManager.timer.timerLaunch)
                    {
                        gameManager.timer.LaunchTimer(1f, false);
                        player.isCollisionInDoorTakeDoor = true;
                        player.doorCollision = this.gameObject;
                        player.canMove = false;
                        gameManager.ui_Manager.DisplayBlackScreen(true, true);
                    }
                }
                else
                {
                    if (player.isInExpedition)
                    {
                        gameManager.timer.LaunchTimer(1f, false);
                        player.isCollisionInDoorBackExpedition = true;
                        player.doorCollision = this.gameObject;
                        player.canMove = false;
                        gameManager.ui_Manager.DisplayBlackScreen(true, true);
                    }
                    else
                    {
                        if (player.haveToGoToExpedition)
                        {
                            gameManager.timer.LaunchTimer(1f, false);
                            player.isCollisionInDoorExpedition = true;
                            player.doorCollision = this.gameObject;
                            player.canMove = false;
                            gameManager.ui_Manager.DisplayBlackScreen(true, true);
                            player.collisionToGost = false;
                        }
                    }
                }
            }
        }
    }
}
