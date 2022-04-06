using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expedition : ScriptableObject
{
    public PlayerDun player;

    public RoomHex room;

    public int indexNeigbour;

    public void Init(PlayerDun player, RoomHex room, int indexNeigbour)
    {
        this.player = player;
        this.room = room;
        this.indexNeigbour = indexNeigbour;
    }
    public static  Expedition CreateInstance(PlayerDun player, RoomHex room, int indexNeigbour)
    {
        var data = ScriptableObject.CreateInstance<Expedition>();
        data.Init(player , room,indexNeigbour);
        return data;

    }
}
