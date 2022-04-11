using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ScriptableObject
{
    public int index;

    public bool isAward;

    public int indexAward;


    public void Init(int index, bool isAward, int indexAward)
    {
        this.index = index;
        this.isAward = isAward;
        this.indexAward = indexAward;
    }

    public static Chest CreateInstance(int index, bool isAward, int indexAward)
    {
        var data = ScriptableObject.CreateInstance<Chest>();
        data.Init(index,isAward, indexAward);
        return data;
    }

}
