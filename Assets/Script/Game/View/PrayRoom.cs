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
    public int numberZone = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        //DisplayResultForImpostro();
        if (roomIsLaunched && CheckAllZonePray())
        {
            DisplayResultOfPray(CheckThereIsImpostorInZone());
            
        }
        else
        {
            if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            {
                DisplayAllTimeDistance();
            }
        }
            
    }

    public void LaunchPrayRoom()
    {
        HideTorchAllPlayer();
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            SetRoomCursed();
        StartCoroutine(LaunchPrayRoomCouroutine());
    }
    public IEnumerator LaunchPrayRoomCouroutine()
    {
        HideTorchForALLPlayer();
        yield return new WaitForSeconds(1);
        ActiveZoneByNumberPlayer(true);
        roomIsLaunched = true;
        gameManager.PrayIsLaunch = true;
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

    public bool CheckThereIsImpostorInZone()
    {
        for (int i = 0; i < this.transform.Find("ZonesPray").childCount; i++)
        {
            if (this.transform.Find("ZonesPray").GetChild(i).GetComponent<ZonePray>().isImpostor)
                return true;
        }
        return false;
    }

    public void ActiveZoneByNumberPlayer(bool active)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int numberZone = 3;

        if (players.Length == 3)
            numberZone = 1;
        else if (players.Length == 4)
            numberZone = 2;

        this.numberZone = numberZone;

        for (int i = 0; i < numberZone; i++)
        {
            this.transform.Find("ZonesPray").GetChild(i).gameObject.SetActive(active);
            if (!active)
                this.transform.Find("ZonesPray").GetChild(i).Find("Animation").gameObject.SetActive(false);
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

    public void DisplayResultOfPray(bool thereIsImpostorInZone)
    {
        ActiveAnimationZone();
        DisplayDistanceInSituation(thereIsImpostorInZone);
        StartCoroutine(CouroutineHideZone());
        gameManager.CloseDoorWhenVote(false);
        roomIsLaunched = false;
        gameManager.PrayIsLaunch = false;
        gameManager.game.currentRoom.speciallyPowerIsUsed = true;
        ResetPrayer();      
    }

    public void DisplayDistanceInSituation(bool thereIsImpostorInZone)
    {
        if (thereIsImpostorInZone)
        {
            DisplayFalseDistance();
            gameManager.game.currentRoom.distancePray = gameManager.distancePrayRoom;
        }
        else
        {
            DisplayDistance();
            gameManager.game.currentRoom.distancePray = gameManager.game.currentRoom.DistancePathFinding;
        }
    }

    public void DisplayAllTimeDistance()
    {
        int distance = gameManager.game.currentRoom.distancePray;
        this.transform.Find("Status").Find("DistanceParadise").Find("Canvas").Find("Text").GetComponent<Text>().text = distance.ToString();
        this.transform.Find("Status").Find("DistanceParadise").gameObject.SetActive(true);
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
                if (gameManager.game.currentRoom.DistancePathFinding != gameManager.game.dungeon.GetPathFindingDistance(room, gameManager.game.currentRoom))
                {
                    listPossiblityRoomWithMoreDistance.Add(room);
                }
                listPossiblityRoom.Add(room);
            }
        }
        
        if(listPossiblityRoomWithMoreDistance.Count > 0)
        {
            listPossiblityRoom = listPossiblityRoomWithMoreDistance;
        }

        if (listPossiblityRoom.Count == 0)
        {
            gameManager.distancePrayRoom = 5;
            distanceImpostor = 5;
            gameManager.gameManagerNetwork.SendDistancePrayRoom(gameManager.distancePrayRoom);
            return;
        }
      
        this.roomUsedWhenCursed = listPossiblityRoom[Random.Range(0, listPossiblityRoom.Count)];
        gameManager.distancePrayRoom = gameManager.game.dungeon.GetPathFindingDistance(gameManager.game.currentRoom, roomUsedWhenCursed);
        gameManager.gameManagerNetwork.SendDistancePrayRoom(gameManager.distancePrayRoom);

        
    }

    public void DesactivateCollisionZone(bool desactivate)
    {
        Debug.Log(this.transform.Find("ZonesPray").childCount);
        for(int i =0; i < this.transform.Find("ZonesPray").childCount; i++)
        {
            this.transform.Find("ZonesPray").GetChild(i).Find("Collision").gameObject.SetActive(!desactivate);
        }
        
    }

    public void HideTorchForALLPlayer()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerNetwork>().SendDisplayBlueTorch(false);
        }
    }

    public void ResetPrayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerGO>().canPray = false;
            player.transform.Find("Prayeur").gameObject.SetActive(false);
        }

    }

    public void HideTorchAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            gameManager.ui_Manager.DisplaySupportTorch(true);
            player.GetComponent<PlayerNetwork>().SetDisplayBlueTorch(false);
        }
    }

}
