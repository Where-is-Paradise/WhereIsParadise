using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrayRoom : MonoBehaviour
{
    public bool roomIsLaunched = false;
    public bool powerIsUsed = false;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        DisplayResultForImpostro();
        if (roomIsLaunched && CheckAllZonePray() && !powerIsUsed)
            DisplayResultOfPray();
    }

    public void LaunchPrayRoom()
    {
        StartCoroutine(LaunchPrayRoomCouroutine());
    }
    public IEnumerator LaunchPrayRoomCouroutine()
    {
        yield return new WaitForSeconds(1);
        ActiveZoneByNumberPlayer();
        roomIsLaunched = true;
    }

    public bool CheckAllZonePray()
    {
        for (int i = 0; i < this.transform.Find("ZonesPray").childCount; i++)
        {
            if (!this.transform.Find("ZonesPray").GetChild(i).GetComponent<ZonePray>().onePlayerPray &&
                this.transform.Find("ZonesPray").GetChild(i).gameObject.activeSelf)
                return false;
        }
        return true;
    }

    public void ActiveZoneByNumberPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
                continue;
            this.transform.Find("ZonesPray").GetChild(i).gameObject.SetActive(true);
            i++;
        }


    }
    public void DisplayResultOfPray()
    {
        if (gameManager.game.currentRoom.isTraped)
        {
            DisplayChangeParadise();
            gameManager.ChangePositionParadise();
            powerIsUsed = true;
        }    
        else
            DisplayDistance();

        gameManager.CloseDoorWhenVote(false);
        roomIsLaunched = false;
        gameManager.PrayIsUsed = true;
    }
    public void DisplayDistance()
    {
        int distance = gameManager.game.currentRoom.DistancePathFinding;
        this.transform.Find("Status").Find("DistanceParadise").Find("Canvas").Find("Text").GetComponent<Text>().text = distance.ToString();
        this.transform.Find("Status").Find("DistanceParadise").gameObject.SetActive(true);
    }

    public void DisplayChangeParadise()
    {
        this.transform.Find("Status").Find("ChangeParadise").gameObject.SetActive(true);
    }

    public void DisplayResultForImpostro()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            return;

        int distance = gameManager.game.currentRoom.DistancePathFinding;
        if (!gameManager.game.currentRoom.isTraped)
        {
            this.transform.Find("Status").Find("DistanceParadiseImpostorView").Find("Canvas").Find("Text").GetComponent<Text>().text = distance.ToString();
            this.transform.Find("Status").Find("DistanceParadiseImpostorView").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("Status").Find("ChangeParadiseImpostorView").gameObject.SetActive(true);
        }
    }
    
}
