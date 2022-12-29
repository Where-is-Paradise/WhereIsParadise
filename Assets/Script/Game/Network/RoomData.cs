using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ScriptableObject
{

    public int indexRoom;
    public bool speciallyPowerIsUsed;
    public bool isChest;
    public bool isSacrifice;
    public bool isJail;
    public bool isVirus;
    public bool isFireball;
    public bool isFoggy;
    public bool isDeathNPC;
    public bool isSwordDamocles;
    public bool isAx;
    public bool isSword;
    public bool isLostTorch;
    public bool isMonsters;
    public bool isPurification;
    public bool isResurection;
    public bool isPray;
    public bool isNPC;
    public bool isLabyrinthHide;
    public bool isCursedTrap;
    public bool isTrap;

    public void Init(int indexRoom , bool speciallyPowerIsUsed )
    {
        this.indexRoom = indexRoom;
        this.speciallyPowerIsUsed = speciallyPowerIsUsed;
    }

    public static RoomData CreateInstance(int indexRoom, bool speciallyPowerIsUsed)
    {
        var data = ScriptableObject.CreateInstance<RoomData>();
        data.Init(indexRoom , speciallyPowerIsUsed);
        return data;
    }



}
