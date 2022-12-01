using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleLabyrinth : MonoBehaviourPun
{
    public int position_x;
    public int position_y;
    public bool isEmpty = false;
    public bool hasTorch = false;
    public bool isMiddle = false; 
    public LabyrinthHideRoom labyrinthRoom;
    public bool isTouchByPlayer = false;

    public bool display = false;

    // pathfinding
    public ObstacleLabyrinth parent;
    public int hcost;
    public int gcost;
    public bool isObtacleToPathFinding = false;
    public bool isPotentialExit = false;

    public List<ObstacleLabyrinth> listNeigbour;
    public List<ObstacleLabyrinth> listNeigbourNoneObstaclePathFinding;
    // Start is called before the first frame update
    void Start()
    {
        labyrinthRoom = this.transform.parent.parent.gameObject.GetComponent<LabyrinthHideRoom>();
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<SpriteRenderer>().enabled = display;

        if (isTouchByPlayer)
        {
            if (isEmpty)
            {
                this.GetComponent<BoxCollider2D>().enabled = false;
                this.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                this.GetComponent<BoxCollider2D>().enabled = true;
                this.GetComponent<SpriteRenderer>().enabled = display;
            }
        }
        else
        {
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
       
        if(isMiddle)
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (hasTorch)
        {
            this.GetComponent<BoxCollider2D>().isTrigger = true;
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.transform.Find("Torch").gameObject.SetActive(true);
        }
        this.transform.Find("Canvas").Find("Text").gameObject.SetActive(display);
        if (isObtacleToPathFinding)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            this.transform.Find("Canvas").Find("Text").GetComponent<Text>().color = new Color(255, 255, 255);
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            this.transform.Find("Canvas").Find("Text").GetComponent<Text>().color = new Color(0, 0, 0);
        }
        this.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = position_x + "/" + position_y;
      
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "CollisionTrigerPlayer")
        {
            if (hasTorch)
                DesactivateRoom(collision);
            //labyrinthRoom.DesactivateAllObtacle();
            if (!isEmpty)
                return;
            isTouchByPlayer = true;
        }
    }
    public int Fcost()
    {
        return (gcost + hcost) - 1;
    }
    public ObstacleLabyrinth GetRandomNeigbour()
    {
        if (listNeigbour.Count == 0)
            return null;
        return listNeigbour[Random.Range(0, listNeigbour.Count)];
    }
    public void SetListNeigbourNoneObstacle()
    {
        foreach(ObstacleLabyrinth neigbour in listNeigbour)
        {
            if (this.parent)
            {
                if (neigbour.SameObstacleByPosition(this.parent))
                    continue;
            }

            if (neigbour.isPotentialExit || (!neigbour.isObtacleToPathFinding && !neigbour.HasNeigbourEmpty() &&  !neigbour.isEmpty))
                listNeigbourNoneObstaclePathFinding.Add(neigbour);
        }
    }

    public void ResetListNeibourNoneObstacle()
    {

        listNeigbourNoneObstaclePathFinding.RemoveRange(0, listNeigbourNoneObstaclePathFinding.Count);

    }

    public ObstacleLabyrinth GetRandomNeigbourNoneObstacle()
    {
        if (listNeigbourNoneObstaclePathFinding.Count == 0)
            return null;
        return listNeigbourNoneObstaclePathFinding[Random.Range(0, listNeigbourNoneObstaclePathFinding.Count)];
    }
    public bool HasNeigbourEmpty()
    {
        foreach (ObstacleLabyrinth neigbour in listNeigbour)
        {
            if (neigbour.parent)
            {
                if (this.parent.SameObstacleByPosition(neigbour))
                    continue;
                Debug.Log(neigbour.isEmpty + " " + this.parent.SameObstacleByPosition(neigbour));
            }
            if (neigbour.isEmpty)
                return true;
        }
        return false;
    }

    public bool SameObstacleByPosition(ObstacleLabyrinth compare)
    {
        if(this.position_x == compare.position_x && this.position_y == compare.position_y)
            return true;
        return false;
    }

    public void SetParentToAllNeigbour(bool active)
    {
        foreach (ObstacleLabyrinth neigbour in listNeigbour){

            if (active)
                neigbour.parent = this;
            else
                neigbour.parent = null;
        }
    }
    public void DesactivateRoom(Collider2D playerCollision)
    {
        int indexPlayer = playerCollision.transform.parent.GetComponent<PhotonView>().ViewID;
        labyrinthRoom.DesactivateRoom(indexPlayer);
    }

    public void SendData(bool isEmpty , bool hasTorch)
    {
        photonView.RPC("SetData", RpcTarget.All, isEmpty  , hasTorch);
    }

    [PunRPC]
    public void SetData(bool isEmpty , bool hasTorch)
    {
        this.isEmpty = isEmpty;
        this.hasTorch = hasTorch;
    }


}
