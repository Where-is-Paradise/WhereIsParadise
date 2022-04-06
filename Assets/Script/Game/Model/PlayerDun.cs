using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDun : ScriptableObject
{
    private int id;
    private string playerName;

    private bool isBoss;
    private bool isInExpedition;
    private int vote_CP;
    private bool hasVoted_CP;
    private int vote_VD;
    private bool hasVoted_VD;
    private bool isImpostor;

    private int pos_X;
    private int pos_Y;

    public void Init(int id, string playerName)
    {
        this.id = id;
        this.playerName = playerName;
        this.pos_X = 0;
        this.pos_Y = 0;

        isBoss = false;
        isInExpedition = false;
        hasVoted_CP = false;
        hasVoted_VD = false;
        isImpostor = false;


    }
    public static PlayerDun CreateInstance(int id, string playerName)
    {
        var data = ScriptableObject.CreateInstance<PlayerDun>();
        data.Init(id, playerName);
        return data;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
    public string GetPlayerName()
    {
        return this.playerName;
    }

    public void SetPosition_X(int pos_X)
    {
        if( pos_X < 0)
        {
             throw new System.Exception("position X does not be less than zero ");
        }
        this.pos_X = pos_X;
    }

    public int GetPosition_X()
    {

        return pos_X;
    }

    public void SetPosition_Y(int pos_Y)
    {
        if (pos_Y < 0)
        {
            throw new System.Exception("position Y  does not be less than zero ");
        }
        this.pos_Y = pos_Y;
    }

    public int GetPosition_Y()
    {
        return pos_Y;
    }


    public int GetId()
    {
        return this.id;
    }

    public int SetId(int id)
    {
        return this.id = id;
    }


    public bool GetIsBoss()
    {
        return isBoss;
    }
    public void SetIsBoss( bool isBoss)
    {
         this.isBoss = isBoss;
    }


    public bool GetIsInExpedition()
    {
        return isInExpedition;
    }

    public void SetIsInExpedition(bool isInExpedition)
    {
        this.isInExpedition = isInExpedition;
    }

    public int GetVote_CP()
    {
        return this.vote_CP;
    }

    public void SetVote_CP(int vote_CP)
    {
        this.vote_CP = vote_CP;
    }

    public bool GetHasVoted_CP()
    {
        return hasVoted_CP;
    }

    public void SetHasVoted_CP(bool hasVoted_CP)
    {
        this.hasVoted_CP = hasVoted_CP;
    }

    public int GetVote_VD()
    {
        return this.vote_VD;
    }

    public void SetVote_VD(int vote_VD)
    {
        this.vote_VD = vote_VD;
    }

    public bool GetHasVoted_VD()
    {
        return hasVoted_VD;
    }
    public void SetHasVoted_VD(bool hasVoted_VD)
    {
        this.hasVoted_VD = hasVoted_VD;
    }

    public bool GetIsImpostor()
    {
        return isImpostor;
    }
    public void SetIsImpostor(bool isImpostor)
    {
        this.isImpostor = isImpostor;
    }

}
