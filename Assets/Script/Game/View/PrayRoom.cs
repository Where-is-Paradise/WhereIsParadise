using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrayRoom : MonoBehaviour
{
    public bool roomIsLaunched = false;
    public bool powerIsUsed = false;
    public GameManager gameManager;
    public Room roomUsedWhenCursed;
    public int distanceImpostor = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            SetRoomCursed();
       
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
        numberZone = players.Length % 2 != 0 ? (numberZone + 1) : numberZone;
        for (int i = 0; i < numberZone; i++)
        {
            this.transform.Find("ZonesPray").GetChild(i).gameObject.SetActive(active);
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
            //SetRoomCursed();
            DisplayFalseDistance();
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
    public void DisplayFalseDistance()
    {
        this.transform.Find("Status").Find("DistanceParadise").Find("Canvas").Find("Text").GetComponent<Text>().text = gameManager.distancePrayRoom.ToString();
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
            this.transform.Find("Status").Find("DistanceParadiseImpostorView").Find("Canvas").Find("Text").GetComponent<Text>().text = gameManager.distancePrayRoom.ToString();
            this.transform.Find("Status").Find("DistanceParadiseImpostorView").gameObject.SetActive(true);
        }
    }

    public IEnumerator CouroutineHideZone()
    {
        yield return new WaitForSeconds(2f);
        ActiveZoneByNumberPlayer(false);
    }

    public void SetRoomCursed()
    {
        List<Room> listPossiblityRoom = new List<Room>();
        List<Room> listPossiblityRoomWithMoreDistance = new List<Room>();
        foreach (Room room in gameManager.game.dungeon.rooms)
        {
            if (room.distance_pathFinding_initialRoom == gameManager.game.dungeon.exit.distance_pathFinding_initialRoom)
            {
                if (room.IsObstacle || room.IsExit)
                    continue;
                if (gameManager.game.currentRoom.DistancePathFinding == gameManager.game.dungeon.GetPathFindingDistance(room, gameManager.game.currentRoom))
                    continue;
                listPossiblityRoom.Add(room);
            }
        }
        if (listPossiblityRoom.Count == 0)
        {
            gameManager.distancePrayRoom = 5;
            distanceImpostor = 5;
            gameManager.gameManagerNetwork.SendDistancePrayRoom(gameManager.distancePrayRoom);
            return;
        }
      
        this.roomUsedWhenCursed = listPossiblityRoom[Random.Range(0, listPossiblityRoom.Count)];
        gameManager.distancePrayRoom = gameManager.game.dungeon.GetPathFindingDistance(gameManager.game.currentRoom, gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().roomUsedWhenCursed);
        gameManager.gameManagerNetwork.SendDistancePrayRoom(gameManager.distancePrayRoom);

        
    }

}
