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
    public bool awardIsTorch = false;
    public int indexObject = -1;
    public List<ObstacleLabyrinth> listNeigbour = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> listNeigbourNoneBroken = new List<ObstacleLabyrinth>();
    public int indexObjectInList = -1;
    public bool activeModeTest = false;

    public LabyrinthRoom labyrinthRoom;
    // Start is called before the first frame update
    void Start()
    {
        SetListNeighbourNoneBroken();
        labyrinthRoom = this.transform.parent.GetComponent<LabyrinthRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeModeTest)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            return;
        }
        if (isBrokable)
            this.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
        else
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "collisionLeft" || collision.name == "collisionUp" || collision.name == "collisionDown")
        {
            if (collision.transform.parent.parent.GetComponent<PlayerGO>().isSacrifice)
                return;
            if(isBrokable && !isBroke)
            {
                BrokeObstacle();
                labyrinthRoom.SendBrokeObstacle(X_position,Y_position);
            }
               
              
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
    public bool HasOneNeibourBroken()
    {
        int counter = 0;
        foreach (ObstacleLabyrinth neigbour in listNeigbour)
        {
            if (neigbour.isBrokable || neigbour.isBroke)
                counter++;
        }
        if (counter >= 1)
            return true;
        return false;
    }

    public int DisplayAward(bool torchIsAlreadyUsed, int indexObjectInList)
    {
        int indexAward;
        if (!torchIsAlreadyUsed)
        {
            indexAward  = Random.Range(0, 4);
        }
        else
        {
            indexAward = Random.Range(1, 3);
        }
        this.transform.GetChild(indexAward).gameObject.SetActive(true);
        hasAward = true;
        this.indexObject = indexAward;
        if (indexAward == 0 || indexAward == 3)
        {
            awardIsTorch = true;
            labyrinthRoom.thereIsTorchInAward = true;
        }
            
        this.indexObjectInList = indexObjectInList;
        return indexAward;
    }

    public void DisplayAwardSimple()
    {
        this.transform.GetChild(indexObject).gameObject.SetActive(true);
    }

    public void SetZIndexOfChild(int zIndex)
    {
        for(int i =0; i< 4; i++)
        {
            this.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = zIndex+ 2;
        }
    }

    public void DesactivateAward()
    {
        if (!hasAward)
            return;
        for(int i = 0; i < 7; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);

       
    }


}
