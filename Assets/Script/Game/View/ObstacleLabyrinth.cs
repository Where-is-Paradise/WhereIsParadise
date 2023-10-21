using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleLabyrinth : MonoBehaviourPun
{
    public int X_position = 0;
    public int Y_position = 0;
    public bool isBroke = false;
    public bool hasAward = false;
    public bool isMiddle = false;
    public bool isBrokable = false;
    public List<ObstacleLabyrinth> listNeigbour = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> listNeigbourNoneBroken = new List<ObstacleLabyrinth>();

    // Start is called before the first frame update
    void Start()
    {
/*        int random = Random.Range(0,4);
        random = 0;
        if (random == 0)
        {
            isBrokable = true;
        }*/
        if (X_position == 0 && Y_position == 0)
            hasAward = true;
        if (X_position == 51 && Y_position == 0)
            hasAward = true;
        if (X_position == 0 && Y_position == 23)
            hasAward = true;
        if (X_position == 51 && Y_position == 23)
            hasAward = true;

        SetListNeighbourNoneBroken();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBrokable)
            this.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
        else
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "collisionLeft" || collision.name == "collisionUp")
        {
            if (collision.transform.parent.parent.GetComponent<PlayerGO>().isSacrifice)
                return;
            if(isBrokable && !isBroke)
                BrokeObstacle();
        }
    }

    public void HideObstacle()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void BrokeObstacle()
    {
        isBroke = true; 
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void ReverseBrokeObstacle()
    {
        isBroke = false;
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<BoxCollider2D>().enabled = true;
    }

    public ObstacleLabyrinth GetRandomNeigbour()
    {
        if (listNeigbour.Count == 0)
            return null;
        return listNeigbour[Random.Range(0, listNeigbour.Count)];
    }
    public void SetListNeighbourNoneBroken()
    {
        listNeigbourNoneBroken.Clear();
        foreach (ObstacleLabyrinth obstacle in listNeigbour)
        {
            if(!obstacle.isBroke && !obstacle.isBrokable && !HasMoreOFOneNeibourBroken(obstacle))
                listNeigbourNoneBroken.Add(obstacle);
        }
    }
    public ObstacleLabyrinth GetRandomNeigbourNoneBroken()
    {
        if (listNeigbourNoneBroken.Count == 0)
            return null;
        return listNeigbourNoneBroken[Random.Range(0, listNeigbourNoneBroken.Count)];
    }
    public bool HasMoreOFOneNeibourBroken(ObstacleLabyrinth osbtacle)
    {
        int counter = 0;
        foreach(ObstacleLabyrinth neigbour in osbtacle.listNeigbour)
        {
            if (neigbour.isBrokable || neigbour.isBroke)
                counter++;
        }
        if (counter > 1)
            return true;
        return false;
    }

}
