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
        ActiveZoneByNumberPlayer(true);
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

    public void ActiveZoneByNumberPlayer(bool active)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int numberZone = (players.Length / 2);
        numberZone = numberZone % 2 != 0 ? numberZone : (numberZone + 1);
        for(int i = 0; i < numberZone; i++)
        {
            this.transform.Find("ZonesPray").GetChild(i).gameObject.SetActive(active);
            i++;
        }
    }

    public void ActiveAnimationZone()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        foreach (GameObject player in players)
        {
            this.transform.Find("ZonesPray").GetChild(i).Find("Animation").gameObject.SetActive(true);
            i++;
        }
    }

    public void DisplayResultOfPray()
    {
        ActiveAnimationZone();
        if (gameManager.game.currentRoom.isTraped && !gameManager.game.currentRoom.IsFoggy && !gameManager.game.currentRoom.isIllustion && !gameManager.game.currentRoom.IsVirus)
        {
            DisplayChangeParadise();
            gameManager.ChangePositionParadise();
            powerIsUsed = true;
        }    
        else
            DisplayDistance();

        StartCoroutine(CouroutineHideZone());
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

    public IEnumerator CouroutineHideZone()
    {
        yield return new WaitForSeconds(2f);
        ActiveZoneByNumberPlayer(false);
    }
    
}
