﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : ScriptableObject
{
    public List<PlayerDun> list_player;
    public Dungeon dungeon;

    public int key_counter;

    public bool allVoted_CP;
    public bool allVoted_VD;

    public List<string> list_voteCP;
    public List<string> list_voteVD;

    public Setting setting;

    public List<Expedition> current_expedition;

    public Room currentRoom; 

    public int counter_acceptVote;
    public int counter_refuseVote;

    public int nbTorch = 0;

    public void Init(List<PlayerDun> list_player)
    {
        this.list_player = list_player;
        current_expedition = new List<Expedition>();
        counter_acceptVote = 0;
        counter_refuseVote = 0;
    }

    public static Game CreateInstance(List<PlayerDun> list_player)
    {
        var data = ScriptableObject.CreateInstance<Game>();
        data.Init(list_player);
        return data;
    }


    public void Launch(int width , int height)
    {
        dungeon = Dungeon.CreateInstance(width, height, setting.RATIO_OBSTACLE);
        dungeon.setting = setting;
        dungeon.CreateRooms();
        
    }

  
    public void CreationMap()
    {
        dungeon.SetInitialRoom();
        currentRoom = dungeon.initialRoom;
        setting.NB_IMPOSTOR = CalculNbImpostor();

        bool correctExit;
        int randomPercentageRatioObatacle = Random.Range(50, 55);
        int randomPercentagePropagation = Random.Range(2, 5);
        int randomPercentageInitialPropagation = Random.Range(2, 5);
        int limit = 0;
        int distanceExit = 6;
        do
        {
            Debug.Log("creation map");
            //dungeon.AddObstacles();
            dungeon.AddObstacles_two(randomPercentageRatioObatacle);
            dungeon.PropagationObstacle(randomPercentageInitialPropagation, randomPercentagePropagation);
            dungeon.SetDistanceAllRoom();
            dungeon.SetPathFindingDistanceInitiateRoom();
            dungeon.SetDistanceReelInitialRoom();
            distanceExit = Random.Range(5, 8);
            correctExit = dungeon.AssignRandomExit(distanceExit);
            if (correctExit)
            {
                dungeon.SetPathFindingDistanceAllRoom();
                int nbOfPossiblity = dungeon.GetNumberOfPossiblityOfExit();
                if(nbOfPossiblity < 7)
                {
                    correctExit = false;
                }
            }
            limit++;
        } while ((dungeon.initialRoom.DistancePathFinding == -1 || !correctExit) &&  limit < 50  );

        if(dungeon.initialRoom.DistancePathFinding == -1)
        {
            CreationMap();
        }

        dungeon.RemoveAllRoomTooFarAway();
        dungeon.AddAllPotentielHell();
        dungeon.InsertSpeciallyRoom(distanceExit);
    }



    public void SetKeyCounter()
    {
/*        if (dungeon.GetNumberOfPossiblityOfExit() < 5)
        {
            key_counter = currentRoom.DistancePathFinding;
            return;
        }*/
        if (dungeon.GetNumberOfPossiblityOfExit() < 7)
        {
            key_counter = currentRoom.DistancePathFinding - 1;
            return;
        }
        key_counter = currentRoom.DistancePathFinding;

    }

    public int CalculNbImpostor()
    {
        if(list_player.Count < 5)
        {
            return 1;
        }else if(list_player.Count < 7)
        {
            return 2;
        }else
        {
            if (setting.NB_IMPOSTOR == 2)
                return 2;
            else
                return 3;
        }
    }

    public void AssignRole()
    {
        //AssignBoss();
        AssignImpostor();
    }

    private void SetPositionAllPlayers(int position_X, int position_Y)
    {
        foreach(PlayerDun player in list_player)
        {
            player.SetPosition_X(position_X);
            player.SetPosition_Y(position_Y);

        }
    }


    public void AssignBoss()
    {
        int random_index = Random.Range(0, list_player.Count);
        list_player[random_index].SetIsBoss(true);
    }

    public PlayerDun ChangeBoss()
    {
        int counter = 0;
        foreach (PlayerDun player in list_player)
        {
            if (player.GetIsBoss())
            {
                if(counter+1 >= list_player.Count) 
                {
                    list_player[0].SetIsBoss(true);
                    player.SetIsBoss(false);
                    return list_player[0];
                }
                list_player[counter + 1].SetIsBoss(true);
                player.SetIsBoss(false);
                return list_player[counter + 1];
            }
            counter++;
        }

          
        return list_player[0];
    }

    private void AssignImpostor()
    {

        int nbImpostor = 1;

        if(list_player.Count <  7 && list_player.Count > 4 )
        {
            nbImpostor = 2;
        }else if (list_player.Count >= 7 && setting.NB_IMPOSTOR == 3)
        {
            nbImpostor = 3;
        }else if (list_player.Count >= 7 && setting.NB_IMPOSTOR == 2)
        {
            nbImpostor = 2;
        }

        List<PlayerDun> potentialImpostor = new List<PlayerDun>();
        potentialImpostor.AddRange(list_player);
        for (int i = 0; i < nbImpostor; i++)
        {
            if (potentialImpostor.Count > 0)
            {
                int indexRandom = Random.Range(0, potentialImpostor.Count);
                potentialImpostor[indexRandom].SetIsImpostor(true);
                potentialImpostor.RemoveAt(indexRandom);
            }
        }
/*        potentialImpostor[0].SetIsImpostor(true);
        potentialImpostor[1].SetIsImpostor(true);*/
    }


    public void CreateExpedition(int idPlayer, int roomID)
    {
        PlayerDun player = GetPlayerById(idPlayer);
        Room room = GetRoomByNeigbourID(roomID);
        current_expedition.Add(Expedition.CreateInstance(player, room, roomID));
    }

    public void ClearExpedition()
    {
        current_expedition.Clear();
        foreach(PlayerDun player in list_player)
        {
            player.SetIsInExpedition(false);
        }
    }


    public PlayerDun GetPlayerById(int idPlayer)
    {
        foreach(PlayerDun player in list_player)
        {
            if(idPlayer == player.GetId())
            {
                return player;
            }
        }
        return null;
    } 

    public Room GetRoomByNeigbourID(int roomID)
    {
        Room room = null;
        /*       if (currentRoom == null)
                   return;*/
/*        if (currentRoom.isIllustion)
            roomID = MixNumberIllusion(roomID);*/

        switch (roomID)
        {
            case 0 :
                room = currentRoom.left_neighbour;
                break;
            case 1:
                room = currentRoom.up_Left_neighbour;
                break;
            case 2:
                room = currentRoom.up_Right_neighbour;
                break;
            case 3: 
                room = currentRoom.right_neighbour;
                break;
            case 4:
                room = currentRoom.down_Right_neighbour;
                break;
            case 5:
                room = currentRoom.down_Left_neighbour;
                break;
        }
        return room;
    }

    public int MixNumberIllusion(int index)
    {
        return currentRoom.listIndexDoor[index];
    }

    public PlayerDun GetBoss()
    {
        foreach(PlayerDun player in list_player)
        {
            if (player.GetIsBoss())
            {
                return player;
            }
        }
        return null;
    }

    public void SetBoss(int indexPlayer)
    {
        foreach (PlayerDun player in list_player)
        {
            if(indexPlayer == player.GetId())
            {
                player.SetIsBoss(true);
            }
        }
    }

    public int NumberExpeditionAvailable( )
    {
        int maxNumberPlalyerInExpe = setting.NUMBER_EXPEDTION_MAX;
        if (currentRoom.GetNumberOfNeigbourNoneObstacleAndNotOpen() < maxNumberPlalyerInExpe)
        {
            return currentRoom.GetNumberOfNeigbourNoneObstacleAndNotOpen();
        }
        else
        {
            return maxNumberPlalyerInExpe;
        }   
    }

    public List<int> GetDoorObstacle(Room room)
    {
        List<int> listRoomObstacle = new  List<int>();

        Dictionary<Room, int> mapRoomObstacle;

        mapRoomObstacle = room.listIndexRoom;

        foreach (KeyValuePair<Room, int> roomWithID in mapRoomObstacle)
        {
            if (roomWithID.Key.IsObstacle)
            {
                listRoomObstacle.Add(roomWithID.Value);
            }
            
        }

        if (room.left_neighbour == null)
        {
            listRoomObstacle.Add(0);
        }
        if(room.up_Left_neighbour == null)
        {
            listRoomObstacle.Add(1);
        }
        if(room.up_Right_neighbour == null)
        {
            listRoomObstacle.Add(2);
        }
        if(room.right_neighbour == null)
        {
            listRoomObstacle.Add(3);
        }
        if(room.down_Right_neighbour == null)
        {
            listRoomObstacle.Add(4);
        }
        if(room.down_Left_neighbour == null)
        {
            listRoomObstacle.Add(5);
        }

        return listRoomObstacle;

    }

    

    public List<int> GetDoorNoneObstacle(Room room)
    {
        List<int> listRoomObstacle = new List<int>();
        Dictionary<Room, int> mapRoomObstacle;
        mapRoomObstacle = room.listIndexRoom;

        foreach (KeyValuePair<Room, int> roomWithID in mapRoomObstacle)
        {
           
            if (roomWithID.Value != -1 && !roomWithID.Key.IsObstacle)
            {
                listRoomObstacle.Add(roomWithID.Value);
            }

        }
        return listRoomObstacle;

    }




    public int WhatDoorHasMajorityVote()
    {
        int counterLeft = 0;
        int counterUpLeft = 0;
        int counterUpRight = 0;
        int counterRight = 0;
        int counterBotRight = 0;
        int counterBotLeft = 0;

        foreach(PlayerDun player in list_player)
        {
            switch (player.GetVote_VD())
            {
                case 0:
                    counterLeft++;
                break;
                case 1:
                    counterUpLeft++;
                 break;
                case 2:
                    counterUpRight++;
                break;
                case 3:
                    counterRight++;
                break;
                case 4:
                    counterBotRight++;
                break;
                case 5:
                    counterBotLeft++;
                 break;

            }

        }
        Dictionary<int, int> dic_counter = new Dictionary<int, int>();
        dic_counter.Add(0,counterLeft);
        dic_counter.Add(1,counterUpLeft);
        dic_counter.Add(2,counterUpRight);
        dic_counter.Add(3,counterRight);
        dic_counter.Add(4,counterBotRight);
        dic_counter.Add(5,counterBotLeft);

        int indexDoor = -1;
        int pivot = -1;
        foreach (KeyValuePair<int, int> dic in dic_counter)
        {
            if(dic.Value > pivot)
            {
                pivot = dic.Value;
                indexDoor = dic.Key;
            }
        }
        return indexDoor;
    }

  
}
