using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciallyRoomData : ScriptableObject
{

    // damocles
    public int currentPlayer_damolcesSword;
    public bool canChangePlayer;

    //lostTorch
    public int currentPlayer_lostTorch;


    public void Init()
    {
        this.currentPlayer_damolcesSword = -1;
        this.currentPlayer_lostTorch = -1;
        canChangePlayer = false;
    }

    public static SpeciallyRoomData CreateInstance()
    {
        var data = ScriptableObject.CreateInstance<SpeciallyRoomData>();
        data.Init();
        return data;
    }

}
