using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Skin
{
    public int id;
    public string name;
    public string _id;
}

[System.Serializable]
public class SkinsPlayer
{
    public string steamId;
    public string pseudoSteam;
    public Skin[] skins;
    public string _id;
    public string __v;
    public int id;
}

[System.Serializable]
public class RequestSkin
{
    public SkinsPlayer response;
}
