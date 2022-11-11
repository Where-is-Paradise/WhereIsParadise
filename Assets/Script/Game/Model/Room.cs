using System.Collections.Generic;
using UnityEngine;


public class Room : ScriptableObject
{
    [SerializeField]
    private int index;
    public int Index { get { return index; } }
    private int x;
    public int X { get { return x; } }
    private int y;
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
    private bool isTraversed = false;
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

    public List<Chest> chestList = null;

    public int nbKeyInPath = 0;

    public bool isOldParadise = false;

    public bool isTraped = false;

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
        }
        chestList = new List<Chest>();
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
}

