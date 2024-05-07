using System.Collections.Generic;
using UnityEngine;


public class Room : ScriptableObject
{
    [SerializeField]
    private int index;
    public int Index { get { return index; } }
    public int x;
    public int X { get { return x; } }
    public int y;
    public int Y { get { return y; } }

    private bool isExit;
    public bool IsExit { get { return isExit; } set { isExit = value; } }
    private bool isObstacle;
    public bool IsObstacle { get { return isObstacle; } set { isObstacle = value; } }

    public bool isTooFar;
    private bool isInitiale;
    public bool IsInitiale { get { return isInitiale; } set { isInitiale = value; } }
    private bool isVirus;
    public bool IsVirus { get { return isVirus; } set { isVirus = value; } }

    private int distanceExit;
    public int DistanceExit { get { return distanceExit; } set { distanceExit = value; } }
    private int distancePathFinding;
    public int DistancePathFinding { get { return distancePathFinding; } set { distancePathFinding = value; } }
    public int distance_pathFinding_initialRoom;
    public int distance_reel_initialRoom;

    public List<Room> listNeighbour;

    public Room up_Right_neighbour;
    public Room up_Left_neighbour;
    public Room right_neighbour;
    public Room left_neighbour;
    public Room down_Right_neighbour;
    public Room down_Left_neighbour;

    public int hcost;
    public int gcost;

    public Room parent;

    public Dictionary<Room, int> listIndexRoom;
    public List<Room> listNeigbour_pathfinding;

    public List<bool> door_isOpen;

    private bool isHell = false;
    public bool IsHell { get { return isHell; } set { isHell = value; } }
    public bool isTraversed = false;
    public bool IsTraversed { get { return isTraversed; } set { isTraversed = value; } }
    private bool isDiscovered = false;
    public bool IsDiscovered { get { return isDiscovered; } set { isDiscovered = value; } }

    private bool isFoggy = false;
    public bool IsFoggy { get { return isFoggy; } set { isFoggy = value; } }
    private bool hasKey = false;
    public bool HasKey { get { return hasKey; } set { hasKey = value; } }


    public bool availableKey = true;
    public bool availableKeyAnimation = true;
    public bool chest = false;
    public bool speciallyPowerIsUsed = false;
    public bool explorationIsUsed = false;
    public bool fireBall = false;
    public bool isSacrifice = false;
    public bool isSpecial = false;
    public bool isNotSpecial = false;
    public bool isJail = false;
    public bool isDeathNPC = false;
    public bool isSwordDamocles = false;
    public bool isAx = false;
    public bool isSword = false;
    public bool isNPC = false;
    public bool isCursedTrap = false;
    public bool isLostTorch = false;
    public bool isMonsters = false;
    public bool isPurification = false;
    public bool isResurection = false;
    public bool isPray = false;
    public bool isLabyrintheHide = false;
    public bool isIllustion = false;

    public bool isHide = false;
    public bool isTrial = false;
    public bool speciallyIsInsert = false;
    public bool isTeamTrial = false;
    public bool isVerySpecial = false;

    public bool isImpostorRoom = false;

    public List<Chest> chestList = null;

    public int nbKeyInPath = 0;

    public bool isOldParadise = false;

    public bool isTraped = false;
    public bool isUseOneTime = false;

    public string doorInNpc = "Z";
    public bool evilIsLeft = false;
    public int indexEvilNPC = 0;
    public int indexEvilNPC_2 = 0;
    public bool npcChooseIsLeft = false;
    public int npcChooseIndex = 0;
    public float randomIntEvil = 0;
    public string doorNameLongerNPC = "Z";
    public string doorNameShorterNPC = "Z";

    public bool doorsMixed = false;
    public List<int> listIndexDoor = new List<int>();

    public bool isNewParadise = false;
    public int isOldSpeciality = 0;

    public bool virus_spawned = false;

    public int[] listLittleObject;

    public bool animationFoogyAlreayHapped = false;

    public void Init(int pos_X, int pos_Y)
    {
        this.x = pos_X;
        this.y = pos_Y;
        distancePathFinding = 0;
        listIndexRoom = new Dictionary<Room, int>();
        listNeigbour_pathfinding = new List<Room>();
        door_isOpen = new List<bool>();
        isVirus = false;
        isObstacle = false;
        isInitiale = false;
        isExit = false;
        isTooFar = false;
        availableKeyAnimation = true;
        for (int i = 0; i < 6; i++)
        {
            door_isOpen.Add(false);
            listIndexDoor.Add(i);
        }
        chestList = new List<Chest>();

        listLittleObject = new int[8];
        //AddLittleObjectInRoom();
    }

    public static Room CreateInstance(int pos_X, int pos_Y)
    {
        var data = ScriptableObject.CreateInstance<Room>();
        data.Init(pos_X, pos_Y);
        return data;
    }

    public void AddNeighbour(List<Room> roomList)
    {
        listNeighbour = new List<Room>();
        foreach (Room room in roomList)
        {
            int distance = Dungeon.GetDistance(room, this);
            if (distance == 1)
            {
                listNeighbour.Add(room);
            }
        }

        if (isInitiale)
        {
            gcost = 5;
        }
    }


    public Dictionary<Room, int> SetSpecificNeighbour()
    {

        int indexRoom = -1;
        foreach (Room room in listNeighbour)
        {
            if (this.y % 2 == 0)
            {
                if (room.X == this.x && room.Y < this.y)
                {
                    up_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 2;
                    listIndexRoom.Add(up_Right_neighbour, indexRoom);
                }
                else if (room.X < this.x && room.Y < this.y)
                {
                    up_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 1;
                    listIndexRoom.Add(up_Left_neighbour, indexRoom);
                }
                else if (room.X > this.x && room.Y == this.y)
                {
                    right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 3;
                    listIndexRoom.Add(right_neighbour, indexRoom);
                }
                else if (room.X < this.x && room.Y == this.y)
                {
                    left_neighbour = room;
                    indexRoom = 0;
                    room.gcost = 5;
                    listIndexRoom.Add(left_neighbour, indexRoom);
                }
                else if (room.X == this.x && room.Y > this.y)
                {
                    down_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 4;
                    listIndexRoom.Add(down_Right_neighbour, indexRoom);
                }
                else if (room.X < this.x && room.Y > this.y)
                {
                    down_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 5;
                    listIndexRoom.Add(down_Left_neighbour, indexRoom);
                }
            }
            else
            {
                if (room.X > this.x && room.Y < this.y)
                {
                    up_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 2;
                    listIndexRoom.Add(up_Right_neighbour, indexRoom);
                }
                else if (room.X == this.x && room.Y < this.y)
                {
                    up_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 1;
                    listIndexRoom.Add(up_Left_neighbour, indexRoom);
                }
                else if (room.X > this.x && room.Y == this.y)
                {
                    right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 3;
                    listIndexRoom.Add(right_neighbour, indexRoom);
                }
                else if (room.X < this.x && room.Y == this.y)
                {
                    left_neighbour = room;
                    indexRoom = 0;
                    room.gcost = 5;
                    listIndexRoom.Add(left_neighbour, indexRoom);
                }
                else if (room.X > this.x && room.Y > this.y)
                {
                    down_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 4;
                    listIndexRoom.Add(down_Right_neighbour, indexRoom);
                }
                else if (room.X == this.x && room.Y > this.y)
                {
                    down_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 5;
                    listIndexRoom.Add(down_Left_neighbour, indexRoom);
                }
            }

        }

        return listIndexRoom;


    }



    public int GetNumberOfNeigbourNoneObstacleAndNotOpen()
    {
        int counter = 0;
        foreach (Room room in listNeighbour)
        {

            if (!room.isObstacle)
            {
                counter++;
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if (door_isOpen[i])
            {
                counter--;
            }
        }
        return counter;
    }

    public bool SetNeigbourCloser_pathfinding(int nbKey)
    {
        int nbKeyAdditional = 0;
        if (this.hasKey && this.availableKey)
        {
            nbKeyAdditional++;
        }
        if (this.isExit)
        {
            return true;
        }
        if (nbKey == 0 && nbKeyAdditional == 0)
        {
            return false;
        }


        List<bool> listHasKey = new List<bool>();
        foreach (Room room in listNeighbour)
        {

            if (!room.isObstacle && !room.isTraversed && room.distancePathFinding < this.distancePathFinding)
            {
                listHasKey.Add(room.SetNeigbourCloser_pathfinding((nbKey - 1) + nbKeyAdditional));
            }

        }
        foreach (bool hasKey in listHasKey)
        {
            if (hasKey)
            {
                nbKeyInPath++;
                return true;
            }

        }
        return false;

    }

    public int GetNbDoor()
    {
        int counter = 0;
        foreach (Room room in listNeighbour)
        {

            if (!room.isObstacle)
            {
                counter++;
            }
        }
        return counter;
    }

    public List<int> ReturnIndexDoorNeigbour()
    {
        List<int> tabReturn = new List<int>();

        if (!left_neighbour.IsObstacle)
            tabReturn.Add(0);
        if (!up_Left_neighbour.IsObstacle)
            tabReturn.Add(1);
        if (!up_Right_neighbour.IsObstacle)
            tabReturn.Add(2);
        if (!right_neighbour.IsObstacle)
            tabReturn.Add(3);
        if (!down_Right_neighbour.IsObstacle)
            tabReturn.Add(4);
        if (!down_Left_neighbour.IsObstacle)
            tabReturn.Add(5);

        return tabReturn;

    }

    public Room GetNeigbourShortByRoomDestination(Room destination)
    {
        int min = 5000;
        Room currentRoom = null;
        foreach (Room neigbour in listNeighbour)
        {
            if (neigbour.IsObstacle)
                continue;
            int distanceNeibour = GetPathFindingDistance(neigbour, destination);
            if (distanceNeibour < min)
            {
                min = distanceNeibour;
                currentRoom = neigbour;
            }
                
        }
        return currentRoom;
    }

    public static void GetShortPathByDestination(Room initial, Room destination , List<Room> listRoomWay, int limit, int distanceParadise)
    {
        if (initial.index == destination.index)
            return;
        if (limit > 500)
        {
            Debug.LogError("Destination unFoudable");
            return;
        }
            

        Room neigbourShorter = initial.GetNeigbourShortByRoomDestination(destination);
        //Debug.Log(neigbourShorter.distancePathFinding);
        if(!neigbourShorter.isExit && neigbourShorter.distance_pathFinding_initialRoom != distanceParadise )
            listRoomWay.Add(neigbourShorter);
        limit++;
        GetShortPathByDestination(neigbourShorter, destination, listRoomWay, limit, distanceParadise) ;

    }

    public int Fcost()
    {
        return (gcost + hcost) - 1;
    }
    public int GetIndex()
    {
        return index;
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public bool HasSameLocation(Room otherRoom) {
        return this.x == otherRoom.X && this.y == otherRoom.Y;
    }

    public override string ToString()
    {
        for (int i = 0; i < 6; i++)
        {
            return door_isOpen[i] + " ";
        }
        return "";
    }

    public bool HaveOneNeighbour()
    {
        int counter = 0;
        foreach(Room roomNeigbour in listNeighbour)
        {
            if (!roomNeigbour.IsObstacle)
                counter++;
        }
        return counter == 1;
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
                int newCostToNeighbour = current.gcost + (Dungeon.GetDistance(neighbour, current) * 5);
                if (newCostToNeighbour < neighbour.gcost || !openList.Contains(neighbour) || Dungeon.GetDistance(neighbour, current) == 0)
                {
                    neighbour.hcost = Dungeon.GetDistance(neighbour, exit);
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

    public void AddLittleObjectInRoom()
    {

        GameObject room = GameObject.Find("Room").gameObject;
        GameObject listLittleObject_Go = room.transform.Find("LittleObject").gameObject;

        for (int i = 0; i < listLittleObject_Go.transform.childCount; i++)
        {
            int randomEmpty = Random.Range(0, 4);
            if (randomEmpty == 0)
            {
                listLittleObject[i] = -1;
                continue;
            }
               
            int randomIndex = Random.Range(0, listLittleObject_Go.transform.GetChild(i).childCount);
            listLittleObject[i] = randomIndex;

        }
    }
}

