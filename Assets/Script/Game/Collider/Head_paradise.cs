using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_paradise : MonoBehaviour
{
    public int nbPlayer = 0;
    public UI_Manager ui_manager;
    private bool paradiseIsOpen;
    // Start is called before the first frame update
    void Start()
    {
        paradiseIsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length <= 4 )
        {
            if (nbPlayer >= (GameObject.FindGameObjectsWithTag("Player").Length - 1) && !paradiseIsOpen)
            {
                OpenParadise();
            }
        }
        else
        {
            if (nbPlayer >= GameObject.FindGameObjectsWithTag("Player").Length - 2 && !paradiseIsOpen)
            {
                OpenParadise();
            }
        }

    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !paradiseIsOpen)
        {
            if (collision.GetComponent<PhotonView>().IsMine &&  !collision.gameObject.GetComponent<PlayerGO>().isImpostor)
            {
                ui_manager.gameManager.gameManagerNetwork.SendCollisionHeadParadise(collision.GetComponent<PhotonView>().ViewID, true);
            }

        }

    }


    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !paradiseIsOpen)
        {

            if (collision.GetComponent<PhotonView>().IsMine && !collision.GetComponent<PlayerGO>().isImpostor)
            {
                ui_manager.gameManager.gameManagerNetwork.SendCollisionHeadParadise(collision.GetComponent<PhotonView>().ViewID, false);
            }
        }
    }

    public void OpenParadise()
    {

        ui_manager.gameManager.GetPlayerMineGO().transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
        ui_manager.OpenDoorParadiseAnimation();
        ui_manager.DisabledWallBehindParadiseDoor();
        paradiseIsOpen = true;
    }




}
