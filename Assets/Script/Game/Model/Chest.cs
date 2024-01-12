using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ScriptableObject
{
    public int index;

    public bool isAward;

    public int indexAward;

    public int indexTrap;


    public void Init(int index, bool isAward, int indexAward, int indexTrap)
    {
        this.index = index;
        this.isAward = isAward;
        this.indexAward = indexAward;
        this.indexTrap = indexTrap;
    }

    public static Chest CreateInstance(int index, bool isAward, int indexAward, int indexTrap)
    {
        var data = ScriptableObject.CreateInstance<Chest>();
        data.Init(index,isAward, indexAward,indexTrap);
        return data;
    }

}
