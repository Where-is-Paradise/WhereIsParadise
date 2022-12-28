using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ScriptableObject
{

    public int indexRoom;
    public bool speciallyPowerIsUsed;


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
