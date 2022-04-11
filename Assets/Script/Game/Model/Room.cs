using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHex : ScriptableObject
{

    private int index;
    private int pos_X;
    private int pos_Y;

    private bool isExit;
    private bool isObstacle;
    private bool isInitialeRoom;
    private bool isVirus;

    private int distance_exit;
    private int distance_pathFinding;
    public int distance_pathFinding_initialRoom;
    public int distance_reel_initialRoom;

    public List<RoomHex> listNeighbour;

    public RoomHex up_Right_neighbour;
    public RoomHex up_Left_neighbour;
    public RoomHex right_neighbour;
    public RoomHex left_neighbour;
    public RoomHex down_Right_neighbour;
    public RoomHex down_Left_neighbour;

    public int hcost;
    public int gcost;

    public RoomHex parent;

    public Dictionary<RoomHex, int> listIndexRoom;
    public List<RoomHex> listNeigbour_pathfinding;

    public List<bool> door_isOpen;

    public bool isHell = false;
    public bool isTraversed = false;
    public bool isFoggy = false;
    public bool hasKey = false;
    public bool availableKey = true;
    public bool availableKeyAnimation = true;
    public bool chest = false;

    public List<Chest> chestList = null;

    public int nbKeyInPath = 0;

    public void Init(int pos_X, int pos_Y)
    {
        this.pos_X = pos_X;
        this.pos_Y = pos_Y;
        distance_pathFinding = 0;
        listIndexRoom = new Dictionary<RoomHex, int>();
        listNeigbour_pathfinding = new List<RoomHex>();
        door_isOpen = new List<bool>();
        isVirus = false;
        isObstacle = false;
        isInitialeRoom = false;
        isExit = false;
        availableKeyAnimation = true;
        for (int i = 0; i< 6; i++)
        {
            door_isOpen.Add(false);
        }
        chestList = new List<Chest>();
    }

    public static RoomHex CreateInstance(int pos_X, int pos_Y)
    {
        var data = ScriptableObject.CreateInstance<RoomHex>();
        data.Init(pos_X , pos_Y);
        return data;
    }

    public void AddNeighbour(List<RoomHex> listRoom)
    {
        listNeighbour = new List<RoomHex>();
        foreach ( RoomHex room in listRoom)
        {
            int distance = Dungeon.GetDistance(room, this);
            if(distance == 1)
            {
                listNeighbour.Add(room);
            }
        }

        if (isInitialeRoom)
        {
            gcost = 5;
        }
    }


    public Dictionary<RoomHex, int> SetSpecificNeighbour()
    {
       
        int indexRoom = -1;
        foreach(RoomHex room in listNeighbour)
        {
            if(this.pos_Y%2 == 0)
            {
                if(room.GetPos_X() == this.pos_X && room.GetPos_Y() < this.pos_Y)
                {
                    up_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 2;
                    listIndexRoom.Add(up_Right_neighbour,indexRoom);
                }
                else if(room.GetPos_X() < this.pos_X && room.GetPos_Y() < this.pos_Y)
                {
                    up_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 1;
                    listIndexRoom.Add(up_Left_neighbour,indexRoom);
                }
                else if(room.GetPos_X() > this.pos_X  && room.GetPos_Y() == this.pos_Y)
                {
                    right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 3;
                    listIndexRoom.Add(right_neighbour,indexRoom);
                }
                else if( room.GetPos_X() < this.pos_X && room.GetPos_Y() == this.pos_Y)
                {
                    left_neighbour = room;
                    indexRoom = 0;
                    room.gcost = 5;
                    listIndexRoom.Add(left_neighbour,indexRoom);
                }
                else if (room.GetPos_X() == this.pos_X && room.GetPos_Y() > this.pos_Y)
                {
                    down_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 4;
                    listIndexRoom.Add(down_Right_neighbour,indexRoom);
                }
                else if (room.GetPos_X() < this.pos_X && room.GetPos_Y() > this.pos_Y)
                {
                    down_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 5;
                    listIndexRoom.Add(down_Left_neighbour,indexRoom);
                }
            }
            else
            {
                if (room.GetPos_X() > this.pos_X && room.GetPos_Y() < this.pos_Y)
                {
                    up_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 2;
                    listIndexRoom.Add(up_Right_neighbour,indexRoom);
                }
                else if (room.GetPos_X() == this.pos_X && room.GetPos_Y() < this.pos_Y)
                {
                    up_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 1;
                    listIndexRoom.Add(up_Left_neighbour,indexRoom);
                }
                else if (room.GetPos_X() > this.pos_X && room.GetPos_Y() == this.pos_Y)
                {
                    right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 3;
                    listIndexRoom.Add(right_neighbour,indexRoom);
                }
                else if (room.GetPos_X() < this.pos_X && room.GetPos_Y() == this.pos_Y)
                {
                    left_neighbour = room;
                    indexRoom = 0;
                    room.gcost = 5;
                    listIndexRoom.Add(left_neighbour,indexRoom);
                }
                else if (room.GetPos_X() > this.pos_X && room.GetPos_Y() > this.pos_Y)
                {
                    down_Right_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 4;
                    listIndexRoom.Add(down_Right_neighbour,indexRoom);
                }
                else if (room.GetPos_X() == this.pos_X && room.GetPos_Y() > this.pos_Y)
                {
                    down_Left_neighbour = room;
                    room.gcost = 5;
                    indexRoom = 5;
                    listIndexRoom.Add(down_Left_neighbour,indexRoom);
                }
            }
            
        }

        return listIndexRoom;


    }

    

    public int GetNumberOfNeigbourNoneObstacleAndNotOpen()
    {
        int counter = 0;
        foreach(RoomHex room in listNeighbour){

            if (!room.isObstacle  )
            {
                counter++;
            }
        }
        for(int i = 0; i < 6; i++)
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
        foreach (RoomHex room in listNeighbour)
        {
            
            if (!room.isObstacle && !room.isTraversed && room.distance_pathFinding < this.distance_pathFinding)
            {
                listHasKey.Add(room.SetNeigbourCloser_pathfinding((nbKey - 1) + nbKeyAdditional));
            }
            
        }
        foreach(bool hasKey in listHasKey)
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

    public int GetPos_X()
    {
        return this.pos_X;
    }
    public void SetPos_X(int pos_X)
    {
        this.pos_X = pos_X;
    }

    public int GetPos_Y()
    {
        return this.pos_Y;
    }
    public void SetPos_Y(int pos_Y)
    {
        this.pos_Y = pos_Y;
    }

    public bool GetIsExit()
    {
        return this.isExit;
    }

    public void SetIsExit(bool isExit)
    {
        this.isExit = isExit;
    }

    public bool GetIsVirus()
    {
        return this.isVirus;
    }

    public void SetIsVirus(bool isVirus)
    {
        this.isVirus = isVirus;
    }

    public bool GetIsObstacle()
    {
        return this.isObstacle;
    }

    public void SetIsObstacle(bool isObstacle)
    {
        this.isObstacle = isObstacle;
    }

    public int GetDistance_exit()
    {
        return this.distance_exit;
    }
    public void SetDistance_exit(int distance_exit)
    {
        this.distance_exit = distance_exit;
    }

    public int GetDistance_pathfinding()
    {
        return this.distance_pathFinding;
    }
    public void SetDistance_pathfinding(int distance_pathFinding)
    {
        this.distance_pathFinding = distance_pathFinding;
    }

    public void SetIsInitialeRoom(bool isInitialeRoom)
    {
        this.isInitialeRoom = isInitialeRoom;
    }
    public bool GetIsInitialeRoom()
    {
        return this.isInitialeRoom;
    }

    public override string  ToString()
    {
        for(int i =0; i < 6; i++)
        {
            return door_isOpen[i] + " ";
        }
        return "";
    }
}
