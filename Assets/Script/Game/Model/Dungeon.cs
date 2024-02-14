using System.Collections.Generic;
using UnityEngine;

public class Dungeon : ScriptableObject
{
    public List<Room> rooms;
    public Room exit;
    private int height;
    private int width;
    private int ratio_obstacle;
    public Room initialRoom;
    private List<Room> listObstaclePivot;
    public List<Room> listRoomTraversed;
    public Setting setting;
    public List<Room> trueListRoomDungeon;


    public void Init(int width, int height, int ratio_obstacle)
    {
        this.rooms = new List<Room>();
        this.height = height;
        this.width = width;
        this.ratio_obstacle = ratio_obstacle;
        listRoomTraversed = new List<Room>();
        trueListRoomDungeon = new List<Room>();
    }

    public static Dungeon CreateInstance(int width, int height, int ratio_obstacle)
    {
        var data = ScriptableObject.CreateInstance<Dungeon>();
        data.Init(width, height, ratio_obstacle);
        return data;
    }

    public void CreateRooms()
    {
        int counterIndex = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Room newRoom = Room.CreateInstance(j, i);
                rooms.Add(newRoom);
                newRoom.SetIndex(counterIndex);
                counterIndex++;
            }
        }
        InitiateNeigbour();
    }

    public void SetInitialRoom()
    {
        initialRoom = GetInitialRoom();
        initialRoom.IsObstacle = false;
        initialRoom.IsInitiale = true;
    }

    public void InitiateNeigbour()
    {
        foreach (Room room in rooms)
        {
            room.AddNeighbour(rooms);
            room.SetSpecificNeighbour();
        }
    }

    public void InsertSpeciallyRoom()
    {
        foreach (Room room in rooms)
        {
            if (room.isTooFar || room.IsInitiale || room.IsExit || GetPathFindingDistance(room, initialRoom) == GetPathFindingDistance(initialRoom, exit))
                continue;

            trueListRoomDungeon.Add(room);

        }

        InsertSpeciallyRoom2();
        //InsertImpostorRoom();
    }

    public void InsertSpeciallyRoom2()
    {
        foreach (Room room in trueListRoomDungeon)
        {
            if (room.IsObstacle)
                continue;

            int randomIsHide = Random.Range(0, 100);
            if (randomIsHide <= 5) //37
            {
                room.isHide = true;
                //room.isNPC = true;
                continue;
            }
            float randomIsTrial = Random.Range(0, 100);  // 100
            if (randomIsTrial < 100) // 47
                room.isTrial = true;
            else
            {
                if (randomIsTrial < 0) // 70
                {
                    room.isTeamTrial = true;
                }
                else
                {
                    float randomSpeciality = Random.Range(0, 100); // 100
                    if (randomSpeciality < 25 && setting.listSpeciallyRoom[0]) // 25
                    {
                        room.chest = true;
                    }
                    else if (randomSpeciality < 50 && setting.listSpeciallyRoom[5]) // 50
                    {
                        room.isPurification = true;
                    }
                    else if (randomSpeciality < 75 && setting.listSpeciallyRoom[1]) // 75
                    {
                        room.isSacrifice = true;
                    }
                    else if (randomSpeciality < 90 && setting.listSpeciallyRoom[6] ) // 90 à changé
                    {
                        room.isPray = true;
                    }
                    else if (randomSpeciality <= 100 && setting.listSpeciallyRoom[3]) // 100
                    {
                        room.isNPC = true;
                        int random = Random.Range(0, 3);
                        room.indexEvilNPC = random;
                        int random2 = 0;
                        do
                        {
                            random2 = Random.Range(0, 3);

                        } while (random2 == room.indexEvilNPC);
                        room.indexEvilNPC_2 = random2;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            room.isSpecial = true;
        }
    }

    public int InsertImpostorRoom(int distance , bool isNearOfInitial, int scale)
    {
        List<Room> listroomInDistance;

        if (!isNearOfInitial)
            listroomInDistance = GetRoomInDirectionOfParadise(distance, scale);
        else
            listroomInDistance = GetRoomInDirectionOfInitial(distance, scale);

        int randomIndex = Random.Range(0, listroomInDistance.Count);
        listroomInDistance[randomIndex].isImpostorRoom = true;
        listroomInDistance[randomIndex].isHide = false;
        ResetSpeciallyRoom(listroomInDistance[randomIndex]);
        Debug.LogError("Index impostor room : " + listroomInDistance[randomIndex].Index);

        return listroomInDistance[randomIndex].Index;
    }

    public List<Room> GetRoomInDirectionOfParadise(int distance, int scale)
    {

        List<Room> listroomInDistance = GetListRoomByDistance(exit, distance);
        List<Room> listroomInPath = new List<Room>();
        foreach (Room roomInDistance in listroomInDistance)
        {
            if ((GetPathFindingDistance(roomInDistance, initialRoom) + scale) > exit.distance_pathFinding_initialRoom)
            {
                continue;
            }
            listroomInPath.Add(roomInDistance);
        }
        return listroomInPath;

    }

    public List<Room> GetRoomInDirectionOfInitial(int distance, int scale)
    {

        List<Room> listroomInDistance = GetListRoomByDistance(initialRoom, distance);
        List<Room> listroomInPath = new List<Room>();
        Debug.Log(distance);
        foreach (Room roomInDistance in listroomInDistance)
        {
            if ((GetPathFindingDistance(roomInDistance, exit) + scale) > initialRoom.DistancePathFinding)
            {
                continue;
            }
            listroomInPath.Add(roomInDistance);
        }
        return listroomInPath;

    }

    public void ResetSpeciallyRoom(Room room)
    {
            room.chest = false;
            room.isSacrifice = false;
            room.fireBall = false;
            room.isAx = false;
            room.isSword = false;
            room.isSwordDamocles = false;
            room.isDeathNPC = false;
            room.isLostTorch = false;
            room.isMonsters = false;
            room.isPurification = false;
            room.isResurection = false;
            room.isPray = false;
            room.isNPC = false;
            room.isLabyrintheHide = false;
            room.isTrial = false;
            room.isTeamTrial = false;
    }


    public bool AssignRandomExit(int distance)
    {
        if (this.exit)
        {
            this.exit.IsExit = false;
        }
        List<Room> potentialExit = new List<Room>();
        int counter = 0;
        foreach (Room room in rooms)
        {
            if (room.distance_pathFinding_initialRoom == distance)
            {
                potentialExit.Add(room);
                counter++;
            }
        }
        if (counter == 0)
        {
            return false;
        }
        int randomIndexRoom = Random.Range(0, potentialExit.Count);
        this.exit = potentialExit[randomIndexRoom];
        if (this.exit.IsObstacle)
        {
            return false;
        }
        this.exit.IsExit = true;
        this.exit.IsObstacle = false;
        return true;
    }


    public void AddObstacles()
    {
        int randomInt;
        foreach (Room room in rooms)
        {
            if (room.IsInitiale || room.IsExit)
            {
                continue;
            }
            randomInt = Random.Range(0, 4);
            room.IsObstacle = randomInt == 0;
        }

        List<Room> listObstacle = GetListObstacle();

        foreach (Room obstacle in listObstacle)
        {
            randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            {
                foreach (Room room in obstacle.listNeighbour)
                {
                    randomInt = Random.Range(0, 4);
                    if (randomInt == 0 && !room.IsInitiale && !room.IsExit)
                    {
                        room.IsObstacle = true;
                    }
                }
            }
        }

    }

    public void AddObstacles_two(float percentage)
    {
        float randomInt;
        foreach (Room room in rooms)
        {

            if (room.IsInitiale || room.IsExit)
            {
                continue;
            }
            randomInt = Random.Range(0, 100);
            room.IsObstacle = randomInt < percentage;
        }
        listObstaclePivot = GetListObstacle();
    }

    public void PropagationObstacle(float initial, float percentage, int direction = -1)
    {
        if (initial == 100)
        {
            return;
        }
        List<Room> listObstacle = GetListObstacle();
        float randomInt;
        foreach (Room obstacle in listObstacle)
        {
            randomInt = Random.Range(0, 100);
            if (randomInt < 100 - initial)
            {
                if (direction == -1)
                {
                    int randomIntTwo = Random.Range(0, obstacle.listNeighbour.Count);
                    for (int i = 0; i < obstacle.listNeighbour.Count; i++)
                    {
                        if (randomIntTwo == i && !obstacle.listNeighbour[i].IsInitiale && !obstacle.listNeighbour[i].IsExit)
                        {
                            obstacle.listNeighbour[i].IsObstacle = true;
                            PropagationObstacle(initial + percentage, percentage, randomIntTwo);

                            return;

                        }
                    }
                }
                else
                {
                    for (int i = 0; i < obstacle.listNeighbour.Count; i++)
                    {
                        if (direction == i && !obstacle.listNeighbour[i].IsInitiale && !obstacle.listNeighbour[i].IsExit)
                        {
                            obstacle.listNeighbour[i].IsObstacle = true;
                            PropagationObstacle(initial + percentage, percentage, direction);
                            return;
                        }
                    }

                }

            }

        }

    }

    public void SetPathFindingDistanceAllRoom()
    {
        foreach (Room room in rooms)
        {
            room.DistancePathFinding = GetPathFindingDistance(room, exit);
        }
    }


    public void SetPathFindingDistanceInitiateRoom()
    {
        foreach (Room room in rooms)
        {
            int distance = GetPathFindingDistance(room, initialRoom);
            room.distance_pathFinding_initialRoom = distance;

        }
    }

    public void SetDistanceReelInitialRoom()
    {
        foreach (Room room in rooms)
        {
            int distance = GetDistance(room, initialRoom);
            room.distance_reel_initialRoom = distance;

        }

    }

    public int GetPathFindingDistance(Room initialRoom, Room exit)
    {
        List<Room> openList = new List<Room>();
        List<Room> closeList = new List<Room>();
        openList.Add(initialRoom);
        while (openList.Count > 0)
        {
            Room current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].Fcost() < current.Fcost() || (openList[i].Fcost() == current.Fcost()))
                {
                    if (openList[i].hcost < current.hcost)
                        current = openList[i];
                }
            }

            openList.Remove(current);
            closeList.Add(current);


            if (current.X == exit.X && current.Y == exit.Y)
            {
                return RetracePath(initialRoom, exit);
            }


            foreach (Room neighbour in current.listNeighbour)
            {
                if (neighbour.X == exit.X && neighbour.Y == exit.Y)
                {
                    neighbour.parent = current;
                    return RetracePath(initialRoom, exit);
                }
                if (neighbour.IsObstacle || closeList.Contains(neighbour))
                {
                    continue;
                }
                int newCostToNeighbour = current.gcost + (GetDistance(neighbour, current) * 5);
                if (newCostToNeighbour < neighbour.gcost || !openList.Contains(neighbour) || GetDistance(neighbour, current) == 0)
                {
                    neighbour.hcost = GetDistance(neighbour, exit);
                    neighbour.gcost = newCostToNeighbour;
                    neighbour.parent = current;
                }
                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);

                }
            }
        }
        return -1;
    }

    private int RetracePath(Room startNode, Room endNode)
    {
        List<Room> path = new List<Room>();
        Room currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;

        }
        return path.Count;

    }


    static public int GetDistance(Room a, Room b)
    {

        var x = a.X - ((a.Y - (a.Y & 1)) / 2);
        var z = a.Y;
        var y = (-x) - z;


        var x2 = b.X - ((b.Y - (b.Y & 1)) / 2);
        var z2 = b.Y;
        var y2 = (-x2) - z2;


        return Mathf.Max(Mathf.Abs(x - x2), Mathf.Abs(y - y2), Mathf.Abs(z - z2));

    }



    private Room GetInitialRoom()
    {
        int X_reference = Random.Range(width / 5, width - (width / 5));
        int Y_reference = Random.Range(height / 5, height - (height / 5));

        foreach (Room room in rooms)
        {

            if (room.X == X_reference && room.Y == Y_reference)
            {
                initialRoom = room;
                initialRoom.IsTraversed = true;
                return room;
            }
        }

        return null;
    }

    private List<Room> GetListObstacle()
    {
        List<Room> listObstacle = new List<Room>();
        foreach (Room room in rooms)
        {
            if (room.IsObstacle)
            {
                listObstacle.Add(room);
            }
        }
        return listObstacle;
    }

    public void SetDistanceAllRoom()
    {
        foreach (Room room in rooms)
        {
            room.DistanceExit = GetDistance(room, initialRoom);
        }
    }
    public Room GetRoomByPosition(int x, int y)
    {
        foreach (Room room in rooms)
        {
            if (room.X == x && room.Y == y)
            {
                return room;
            }
        }
        return null;
    }

    public Room GetRoomByIndex(int index)
    {
        foreach (Room room in rooms)
        {
            if (index == room.GetIndex())
            {
                return room;
            }
        }
        return null;
    }

    public void InsertRandomFoggyRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && !room.IsExit)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.IsFoggy = true;
                }
            }

        }
    }

    public void InserKeyInRandomRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && !room.IsExit && (room.DistancePathFinding != exit.DistancePathFinding))
            {
                int randomInt = Random.Range(0, 6);
                if (randomInt == 0)
                {
                    room.HasKey = true;
                }
            }

        }
    }

    public void InsertRandomVirusRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && (room.DistancePathFinding - 1) > 0)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.IsVirus = true;
                }
            }

        }
    }
    public void InsertRandomFireBallRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && (room.DistancePathFinding - 1) > 0)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.fireBall = true;
                }
            }
        }
    }

    public void InsertRandomSacrificeRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && (room.DistancePathFinding - 1) > 0)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.isSacrifice = true;
                }
            }
        }
    }
    public void InsertRandomJailRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && (room.DistancePathFinding - 1) > 0)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.isJail = true;
                }
            }
        }
    }


    public void InsertChestRoom(int indexRoom)
    {
        foreach (Room room in rooms)
        {
            if (room.Index == indexRoom)
            {
                room.chest = true;
                SetUpChests(room);
            }
        }
    }

    public void InsertRandomChestRoom()
    {
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && !room.IsInitiale && (room.DistancePathFinding - 1) > 0)
            {
                int randomInt = Random.Range(0, 1);
                if (randomInt == 0)
                {
                    room.chest = true;
                    SetUpChests(room);
                }
            }
        }
    }




    public void SetUpChests(Room room)
    {
        int randomAward = 0;
        int randomIndex = Random.Range(0, 2);
        //int randomAward = 2;

        if (randomIndex == 0)
        {
            room.chestList.Add(Chest.CreateInstance(0, true, randomAward,1));
            room.chestList.Add(Chest.CreateInstance(1, false, randomAward,2));
        }
        else
        {
            room.chestList.Add(Chest.CreateInstance(0, false, randomAward,2));
            room.chestList.Add(Chest.CreateInstance(1, true, randomAward,1));
        }


    }

    public void ResetChest(Room room)
    {
        room.chestList.Clear();
    }

    public void SetListRoomTraversed()
    {
        foreach (Room room in rooms)
        {
            if (room.IsTraversed)
            {
                listRoomTraversed.Add(room);
            }
        }
    }

    public List<Room> GetListRoomDiscoverd()
    {
        List<Room> listRoomDiscovered = new List<Room>();
        foreach (Room room in rooms)
        {
            if (room.IsDiscovered)
            {
                listRoomDiscovered.Add(room);
            }
        }
        return listRoomDiscovered;
    }

    public bool GetIfThereisKeyInShortsPath(int nbkey)
    {
        SetListRoomTraversed();
        foreach (Room roomTraversed in listRoomTraversed)
        {
            if (roomTraversed.SetNeigbourCloser_pathfinding(nbkey))
                return true;

        }
        return false;
    }

    public List<Room> GetListRoomByDistance(Room room, int distance)
    {
        List<Room> listRoomWithCorrectDistance = new List<Room>();
        foreach (Room roomIndex in rooms)
        {
            if (roomIndex.IsObstacle)
            {
                continue;
            }
            if (roomIndex.IsExit)
            {
                continue;
            }
            if (roomIndex.IsTraversed)
            {
                continue;
            }
            int distanceIndex = GetPathFindingDistance(roomIndex, room);
            if (distanceIndex != distance)
            {
                continue;
            }
            listRoomWithCorrectDistance.Add(roomIndex);
        }
        return listRoomWithCorrectDistance;
    }

    public int GetNumberOfPossiblityOfExit()
    {
        int counter = 0;
        int distanceReference = exit.distance_pathFinding_initialRoom;
        foreach (Room room in rooms)
        {
            if (!room.IsObstacle && room.distance_pathFinding_initialRoom == distanceReference)
            {
                counter++;
            }
        }
        return counter;

    }

    public void RemoveAllRoomTooFarAway()
    {
        foreach (Room room in rooms)
        {
            if (room.distance_pathFinding_initialRoom > (exit.distance_pathFinding_initialRoom + 3))
            {
                room.IsObstacle = true;
                room.isTooFar = true;
            }
        }
    }

    public int getWidth()
    {
        return height;
    }

    public int getHeight()
    {
        return width;
    }

    public void SetExit(Room room)
    {
        this.exit = room;
    }


}
