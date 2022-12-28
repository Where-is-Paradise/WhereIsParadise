using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject
{
    public int viewId;
    public string namePlayer;
    public float positionMineX;
    public float positionMineY;
    public int positionMineX_dungeon;
    public int positionMineY_dungeon;
    public bool isImpostor;
    public bool isBoss;
    public bool isSacrifice;
    public bool isInJail;
    public bool isInvisible;
    public int indexSkin;
    public bool hasWinFireBallRoom;
    public string userId;

    // impostor
    public int indexPowerTrap;
    public bool powerIsUsed;
    public int indexObject;
    public bool objectIsUsed;

    


    public void Init (int viewId)
    {
        this.viewId = viewId;
    }

    public static PlayerData CreateInstance(int viewId)
    {
        var data = ScriptableObject.CreateInstance<PlayerData>();
        data.Init(viewId);
        return data;
    }
}
