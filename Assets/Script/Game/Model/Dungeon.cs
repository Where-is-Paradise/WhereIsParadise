using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : ScriptableObject
{
    public List<RoomHex> rooms;
    public RoomHex exit;
    private int height;
    private int width;
    private int ratio_obstacle;
    public RoomHex initialRoom;
    private List<RoomHex> listObstaclePivot;
    private List<RoomHex> listRoomTraversed;



    public void Init(int width , int height , int ratio_obstacle)
    {
        this.rooms = new List<RoomHex>();
        this.height = height;
        this.width = width;
        this.ratio_obstacle = ratio_obstacle;
        listRoomTraversed = new List<RoomHex>();
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
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                RoomHex newRoom  = RoomHex.CreateInstance(j , i);
                rooms.Add(newRoom);
                newRoom.SetIndex(counterIndex);
                counterIndex++;
            }
        }
        InitiateNeigbour();
        //SetInitialRoom();

        //initialRoom.SetSpecificNeighbour();
    }

    public void SetInitialRoom()
    {
        initialRoom = GetInitialRoom();
        initialRoom.SetIsObstacle(false);
        initialRoom.SetIsInitialeRoom(true);
    }

    public void InitiateNeigbour()
    {
        foreach(RoomHex room in rooms)
        {
            room.AddNeighbour(rooms);
            room.SetSpecificNeighbour();
        }
    }


    public bool AssignRandomExit(int distance)
    {
        if (this.exit)
        {
            this.exit.SetIsExit(false);
        }
        List<RoomHex> potentialExit = new List<RoomHex>();
        int counter = 0;
        foreach (RoomHex room in rooms)
        {
            if(room.distance_pathFinding_initialRoom == distance)
            {
                potentialExit.Add(room);
                counter++;
            }
        }
        if(counter == 0)
        {
            return false;
        }
        int randomIndexRoom = Random.Range(0, potentialExit.Count);
        this.exit = potentialExit[randomIndexRoom];
        if (this.exit.GetIsObstacle())
        {
            return false;
        }
        this.exit.SetIsExit(true);
        this.exit.SetIsObstacle(false);
        return true;
    }


    public void AddObstacles()
    {
        int randomInt;
        foreach (RoomHex room in rooms)
        {
            if (room.GetIsInitialeRoom() || room.GetIsExit())
            {
                continue;
            }
            randomInt = Random.Range(0, 4);
            if (randomInt == 0)
            {
                room.SetIsObstacle(true);
            }
            else
            {
                room.SetIsObstacle(false);
            }
        }

        List<RoomHex> listObstacle = GetListObstacle();

        foreach(RoomHex obstacle in listObstacle)
        {
            randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            { 
                foreach( RoomHex room in obstacle.listNeighbour)
                {
                    randomInt = Random.Range(0, 4);
                    if (randomInt  == 0 && !room.GetIsInitialeRoom() && !room.GetIsExit())
                    {
                        room.SetIsObstacle(true);
                    }
                }
            }
        }
        
    }

    public void AddObstacles_two(float percentage)
    {
        float randomInt;
        foreach (RoomHex room in rooms)
        {
          
            if (room.GetIsInitialeRoom() || room.GetIsExit())
            {
                continue;
            }
            randomInt = Random.Range(0,100);
            if (randomInt < percentage)
            {
                room.SetIsObstacle(true);
            }
            else
            {
                room.SetIsObstacle(false);
            }
        }
        listObstaclePivot = GetListObstacle();
    }

    public void PropagationObstacle(float initial, float percentage, int direction = -1 )
    {
        if(initial == 100)
        {
            return;
        }
        List<RoomHex> listObstacle = GetListObstacle();
        float randomInt;
        foreach (RoomHex obstacle in listObstacle)
        {
            randomInt = Random.Range(0,100);
            if(randomInt < 100 - initial)
            {
                if(direction == -1)
                {
                    int randomIntTwo = Random.Range(0, obstacle.listNeighbour.Count);
                    for (int i = 0; i < obstacle.listNeighbour.Count; i++)
                    {
                        if (randomIntTwo == i && !obstacle.listNeighbour[i].GetIsInitialeRoom() && !obstacle.listNeighbour[i].GetIsExit())
                        {
                            obstacle.listNeighbour[i].SetIsObstacle(true);
                            PropagationObstacle(initial + percentage, percentage, randomIntTwo);

                            return;
                            
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < obstacle.listNeighbour.Count; i++)
                    {
                        if (direction == i && !obstacle.listNeighbour[i].GetIsInitialeRoom() && !obstacle.listNeighbour[i].GetIsExit())
                        {
                            obstacle.listNeighbour[i].SetIsObstacle(true);
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
        foreach(RoomHex room in rooms)
        {
            int distance = GetPathFindingDistance(room, exit);
            room.SetDistance_pathfinding(distance);
        }
    }


    public void SetPathFindingDistanceInitiateRoom()
    {
        foreach (RoomHex room in rooms)
        {
            int distance = GetPathFindingDistance(room, initialRoom);
            room.distance_pathFinding_initialRoom = distance;
             
        }
    }

    public void SetDistanceReelInitialRoom()
    {
        foreach (RoomHex room in rooms)
        {
            int distance = GetDistance(room, initialRoom);
            room.distance_reel_initialRoom = distance;

        }

    }

    public int GetPathFindingDistance(RoomHex initialRoom, RoomHex exit)
    {
        List<RoomHex> openList = new List<RoomHex>();
        List<RoomHex> closeList = new List<RoomHex>();
        openList.Add(initialRoom);
        while (openList.Count > 0)
        {
            RoomHex current = openList[0];
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


            if (current.GetPos_X() == exit.GetPos_X() && current.GetPos_Y() == exit.GetPos_Y())
            {
                return RetracePath(initialRoom, exit);
            }
                

            foreach (RoomHex neighbour in current.listNeighbour)
            {
                if(neighbour.GetPos_X() == exit.GetPos_X() && neighbour.GetPos_Y() == exit.GetPos_Y())
                {
                    neighbour.parent = current;
                    return RetracePath(initialRoom, exit);
                }
                if (neighbour.GetIsObstacle() || closeList.Contains(neighbour))
                {
                    continue;
                }
                int newCostToNeighbour = current.gcost + (GetDistance(neighbour, current)*5) ;
                if (newCostToNeighbour < neighbour.gcost || !openList.Contains(neighbour) || GetDistance(neighbour,current) == 0)
                {
                    neighbour.hcost = GetDistance(neighbour, exit);
                    neighbour.gcost = newCostToNeighbour ;
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

    private int RetracePath(RoomHex startNode, RoomHex endNode)
    {
        List<RoomHex> path = new List<RoomHex>();
        RoomHex currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            
        }
        return path.Count;

    }


    static public int  GetDistance(RoomHex a, RoomHex b) {

        var x = a.GetPos_X() - ((a.GetPos_Y() - (a.GetPos_Y()&1)) / 2);
        var z = a.GetPos_Y();
        var y = (-x) - z;


        var x2 = b.GetPos_X() - ((b.GetPos_Y() - (b.GetPos_Y()&1)) / 2);
        var z2 = b.GetPos_Y();
        var y2 = (-x2) - z2;


        return Mathf.Max(Mathf.Abs(x  - x2), Mathf.Abs(y - y2), Mathf.Abs(z - z2));

    }



    private RoomHex GetInitialRoom()
    {
        int X_reference = Random.Range(0, width);
        int Y_reference = Random.Range(0, height);

        foreach (RoomHex room in rooms)
        {

            if (room.GetPos_X() == X_reference && room.GetPos_Y() == Y_reference)
            {
                initialRoom = room;
                initialRoom.isTraversed = true;
                return room;
            }
        }

        return null;
    }

    private List<RoomHex> GetListObstacle()
    {
        List<RoomHex> listObstacle = new List<RoomHex>();
        foreach(RoomHex room in rooms)
        {
            if (room.GetIsObstacle())
            {
                listObstacle.Add(room);
            }
        }
        return listObstacle;
    }

    public void SetDistanceAllRoom()
    {
        foreach(RoomHex room in rooms)
        {
            int distance = GetDistance(room, initialRoom);
            room.SetDistance_exit(distance);
        }
    }
    public RoomHex GetRoomByPosition(int x , int y)
    {
        foreach(RoomHex room in rooms)
        {
            if(room.GetPos_X() == x && room.GetPos_Y() == y)
            {
                return room;
            }
        }
        return null;
    }

    public RoomHex GetRoomByIndex(int index)
    {
        foreach ( RoomHex room in rooms)
        {
            if(index == room.GetIndex())
            {
                return room;
            }
        }
        return null;
    }

    public void InsertRandomFoggyRoom()
    {
        foreach(RoomHex room in rooms)
        {
            if (!room.GetIsObstacle() && !room.GetIsInitialeRoom() && !room.GetIsExit())
            {
                int randomInt = Random.Range(0, 8);
                if (randomInt == 0)
                {
                    room.isFoggy = true;
                }
            }
            
        }
    }

    public void InserKeyInRandomRoom()
    {
        foreach (RoomHex room in rooms)
        {
            if (!room.GetIsObstacle() && !room.GetIsInitialeRoom() && !room.GetIsExit() && (room.GetDistance_pathfinding() != exit.GetDistance_pathfinding()))
            {
                int randomInt = Random.Range(0, 6);
                if (randomInt == 0)
                {
                    room.hasKey = true;
                }
            }

        }
    }

    public void InsertRandomVirusRoom()
    {
        foreach (RoomHex room in rooms)
        {
            if (!room.GetIsObstacle() && !room.GetIsInitialeRoom() && (room.GetDistance_pathfinding()-1) > 0)
            {
                int randomInt = Random.Range(0, 25);
                if (randomInt == 0)
                {
                    room.SetIsVirus(true);
                }
            }

        }
    }

    public void InsertRandomChestRoom()
    {
        foreach (RoomHex room in rooms)
        {
            if (!room.GetIsObstacle() && !room.GetIsInitialeRoom() && (room.GetDistance_pathfinding() - 1) > 0)
            {
                int randomInt = Random.Range(0, 2);
                if (randomInt == 0)
                {
                    room.chest = true;
                }
            }
        }
    }

    public void SetListRoomTraversed()
    {
        foreach (RoomHex room in rooms)
        {
            if (room.isTraversed)
            {
                listRoomTraversed.Add(room);
            }
        }
    }

    public bool GetIfThereisKeyInShortsPath(int nbkey)
    {
       SetListRoomTraversed();
       foreach (RoomHex roomTraversed in listRoomTraversed)
       {
            if (roomTraversed.SetNeigbourCloser_pathfinding(nbkey))
                return true;
           
       }
        return false;
    }

    public int GetNumberOfPossiblityOfExit()
    {
        int counter = 0;
        int distanceReference = exit.distance_pathFinding_initialRoom;
        foreach (RoomHex room in rooms)
        {
            if(!room.GetIsObstacle() && room.distance_pathFinding_initialRoom == distanceReference)
            {
                counter++;
            }
        }
        return counter;

    }

    public int getWidth()
    {
        return height;
    }

    public int getHeight()
    {
        return width;
    }

    public void SetExit(RoomHex room)
    {
        this.exit = room;
    }
}
