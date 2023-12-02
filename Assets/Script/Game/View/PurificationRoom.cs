using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurificationRoom : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            ActivateZone(false);
        else
            ActivateZone(true);
    }

    public void LaunchPurificationRoom()
    {
        StartCoroutine(LaunchPurificationRoomCouroutine());
    }
    public IEnumerator LaunchPurificationRoomCouroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (PurificationPlayer())
        {
            gameManager.game.currentRoom.speciallyPowerIsUsed = true;
            gameManager.PurificationIsUsed = true;
            gameManager.ui_Manager.DisplaySpeciallyLevers(false, 10);
        }
        else
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 10);
        }
        gameManager.CloseDoorWhenVote(false);
    }

    public bool PurificationPlayer()
    {
        if (!this.transform.Find("zone").gameObject.GetComponent<ZonePurification>().currentPlayer)
            return false;
        this.transform.Find("zone").gameObject.GetComponent<ZonePurification>().currentPlayer.GetComponent<PlayerNetwork>().SendPurification();
        return true;
    }
    public void ActivateZone(bool active)
    {
        this.transform.Find("zone").gameObject.SetActive(active);
    }
}
