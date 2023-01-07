using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleLabyrinth : MonoBehaviourPun
{
    public int index;
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

    public float timerLight = 100;
    public int zIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
               
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

        if (hasTorch)
        {
            timerLight -= (Time.deltaTime * 50);
            DisplayLightBlink();
            if (timerLight < 0)
            {
                timerLight = 100;
            }

        }

        if (!labyrinthRoom)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.transform.Find("Torch").gameObject.SetActive(false);
        }
        else
        {
            if (!labyrinthRoom.gameManager.SamePositionAtBoss())
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
                this.GetComponent<BoxCollider2D>().enabled = false;
                this.transform.Find("Torch").gameObject.SetActive(false);
            }
        }
        this.GetComponent<SpriteRenderer>().sortingOrder = zIndex;
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
            if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
                return;
            if (AllMyNeibourIsNotEmpty())
                return;
            SendIsTouchByPlayer(true);
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

    public void SendData(bool isEmpty , bool hasTorch , bool isMiddle)
    {
        photonView.RPC("SetData", RpcTarget.Others, isEmpty  , hasTorch , isMiddle);
    }

    [PunRPC]
    public void SetData(bool isEmpty , bool hasTorch , bool isMiddle)
    {
        this.isEmpty = isEmpty;
        this.hasTorch = hasTorch;
        this.isMiddle = isMiddle;
    }

    public void DisplayLightBlink()
    {
        this.transform.Find("Torch").Find("TorchLight").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, timerLight/100);
        this.transform.Find("Torch").Find("TorchLight2").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, timerLight / 100);
        this.transform.Find("Torch").Find("TorchLight3").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, timerLight / 100);
    }

    public void SendInitiationData(int i, int j)
    {
        photonView.RPC("SetInitiationData", RpcTarget.All, i, j);
    }

    [PunRPC]
    public void SetInitiationData(int i, int j)
    {
        labyrinthRoom = GameObject.Find("LabyrinthHideRoom").GetComponent<LabyrinthHideRoom>();
        this.transform.parent = labyrinthRoom.transform.Find("ListObstacle").transform;
        this.transform.position = new Vector3(labyrinthRoom.initialPositionObstacle_x + (j * labyrinthRoom.decalageObstacleRight),
            labyrinthRoom.initialPositionObstacle_y + (i * -labyrinthRoom.decalageObstacleDown));
        this.isEmpty = false;
        this.position_x = j;
        this.position_y = i;
    }
    public void SendIsTouchByPlayer(bool isTouchByPlayer)
    {
        labyrinthRoom.SendObstacleIsTouchByIndex(index, isTouchByPlayer);
    }

    public bool AllMyNeibourIsNotEmpty()
    {
        if (isMiddle)
            return false;
        int counter = 0;
        foreach(ObstacleLabyrinth obstacle in listNeigbour)
        {
            if (obstacle.isTouchByPlayer || obstacle.isMiddle)
            {
                return false;
            }
            counter++;
        }
        if(counter == 3)
            return false;
        return true;
    }
}
