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
    }

    public void LaunchPurificationRoom()
    {
        StartCoroutine(LaunchPurificationRoomCouroutine());
    }
    public IEnumerator LaunchPurificationRoomCouroutine()
    {
        yield return new WaitForSeconds(0.25f);
        this.transform.Find("zone").Find("InitialZone").gameObject.SetActive(false);
        if (PurificationPlayer())
        {
            gameManager.game.currentRoom.speciallyPowerIsUsed = true;
            gameManager.PurificationIsUsed = true;
            gameManager.ui_Manager.DisplaySpeciallyLevers(false, 2 , "SpeciallyRoom_levers");
            this.transform.Find("zone").Find("Animation").GetChild(0).gameObject.SetActive(true);
            StartCoroutine(ActivateZone(false));
        }
        else
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 2 , "SpeciallyRoom_levers");
            StartCoroutine(CoutoutineDisplayInitialZone());
           
        }
        StartCoroutine(CloseDoorCouroutine(false));
    }

    public bool PurificationPlayer()
    {
        if (!this.transform.Find("zone").gameObject.GetComponent<ZonePurification>().currentPlayer)
            return false;
        this.transform.Find("zone").gameObject.GetComponent<ZonePurification>().currentPlayer.GetComponent<PlayerNetwork>().SendPurification();
        return true;
    }
    public IEnumerator ActivateZone(bool active)
    {
        yield return new WaitForSeconds(1);
        this.transform.Find("zone").gameObject.SetActive(active);
    }
    public IEnumerator CoutoutineDisplayInitialZone()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.Find("zone").Find("InitialZone").gameObject.SetActive(true);
    }

    public IEnumerator CloseDoorCouroutine(bool close)
    {
        yield return new WaitForSeconds(2.5f);
        gameManager.CloseDoorWhenVote(close);
    }
}
