using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthRoom : TrialsRoom
{
    private int width = 52;
    private int height = 24;
    private float initialPositionX = -6.3f;
    private float initialPositionY = 2.63f;
    public List<ObstacleLabyrinth> listObstacles = new List<ObstacleLabyrinth>();
    public GameObject prefabObstacle;
    public bool firsPathIsFind = false;
    public bool secondePathIsFind = false;
    public bool canLauchSecondePath = false;
    public ObstacleLabyrinth FirstRadnomObstacleBroke;
    public ObstacleLabyrinth SecondeRadnomObstacleBroke;
    public bool canBreak = true;

    public List<ObstacleLabyrinth> listFirstPath = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth FirstRadnomObstacleBroke2;
    public List<ObstacleLabyrinth> listInitalObstacleRight = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> listInitalObstacleLeft = new List<ObstacleLabyrinth>();

    public List<ObstacleLabyrinth> listPath2 = new List<ObstacleLabyrinth>();
    public bool pathInsideIsFind = false;
    public int counterPath2;
    public int coutnerNumberOfPath = 0;
    public List<ObstacleLabyrinth> listPotentialAward = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> listPotentialAwardSecondOption = new List<ObstacleLabyrinth>();

    public bool objectIsInsert = false;
    public int counterPath1 = 0;
    public int counter2Path2 = 0;
    public int coutnerPathInside = 0;
    public int coutnerPathInside2 = 0;

    public bool activeModeTest = false;

    public bool firstObjectFind = false;
    public bool secondObjectFind = false;
    public bool thereIsTorchInAward = false;
    public int indexPlayerWithTorch = -1;

    //public Dictionary<int,int> listIndexAwardByPlayer = new Dictionary<int, int>();
    public List<KeyValuePair<int, int>> listIndexAwardByPlayer = new List<KeyValuePair<int, int>>();

    public SaveDataNetwork dataGame;

    // Start is called before the first frame update
    void Start()
    {
        dataGame = GameObject.Find("DataReconnexion").GetComponent<SaveDataNetwork>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerParent = GameObject.Find("GameManager").GetComponent<GameManager>();


        if (activeModeTest)
            ActiveModeTestAllObtacle(true);
        else
            ActiveModeTestAllObtacle(false);

        if (!gameManagerParent.speciallyIsLaunch)
            return;
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().ActivateCollisionLabyrinth();
        if (!gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        if (objectIsInsert)
            return;

        FirstRandomPATH();
        SecondeRandomPATH();
        AddPathInside();
        if (pathInsideIsFind && !objectIsInsert)
        {
            if (!dataGame.isDisconnect)
            {
                AttributeObjectInObstacle();
                SendAllObstacleData();
            }
            else
            {
                 // on impose objctIsInsert = true, alors que c faux  ( cela evite que le perso qui deco peut avoir des objets aussi ) 
                objectIsInsert = true;
            }
        }
            
    }

    public void StartRoom()
    {
        
        DisplayAllObstacle();
        ChangeScalePlayer();
        ChangeColliderSize(true);          
        AddNeighboursToEachObstacle();
        ActiveInvisibleWallInMiddle(true);
        DisplayListSeparation(true);
        gameManagerParent.ui_Manager.DisplayInteractionObject(false);

        listInitalObstacleRight.Add(GetObtacleByPosition(30, 9));
        listInitalObstacleRight.Add(GetObtacleByPosition(30, 10));
        listInitalObstacleRight.Add(GetObtacleByPosition(30, 11));
        listInitalObstacleRight.Add(GetObtacleByPosition(30, 12));
        listInitalObstacleRight.Add(GetObtacleByPosition(30, 13));
        listInitalObstacleRight.Add(GetObtacleByPosition(30, 14));

        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 9));
        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 10));
        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 11));
        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 12));
        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 13));
        listInitalObstacleLeft.Add(GetObtacleByPosition(21, 14));
        gameManagerParent.speciallyIsLaunch = true;
        gameManagerParent.ActivateCollisionTPOfAllDoor(false);
        gameManagerParent.CloseDoorWhenVote(true);
        gameManagerParent.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManagerParent.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManagerParent.DisplayTorchBarre(false);
        gameManagerParent.ui_Manager.LaunchFightMusic();

        if (gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            int randomInitialRight = Random.Range(0, listInitalObstacleRight.Count);
            FirstRadnomObstacleBroke = listInitalObstacleRight[randomInitialRight];
            int randomInitialLeft = Random.Range(0, listInitalObstacleLeft.Count);
            SecondeRadnomObstacleBroke = listInitalObstacleLeft[randomInitialLeft];
            photonView.RPC("SendInitialObstacle", RpcTarget.Others, randomInitialRight, randomInitialLeft);
            FirstRadnomObstacleBroke.BrokeObstacle();
            SecondeRadnomObstacleBroke.BrokeObstacle();
        }
    }

    [PunRPC]
    public void SendInitialObstacle(int indexRright, int indexLeft)
    {
        FirstRadnomObstacleBroke = listInitalObstacleRight[indexRright];
        SecondeRadnomObstacleBroke = listInitalObstacleLeft[indexLeft];
        FirstRadnomObstacleBroke.BrokeObstacle();
        SecondeRadnomObstacleBroke.BrokeObstacle();
    }

    public void DisplayAllObstacle()
    {
        int sortingOrder = -11;
        for(int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = initialPositionX + (float) (j * 0.25) ;
                float y = initialPositionY - (float) (i * 0.25);
                GameObject obstacle = GameObject.Instantiate(prefabObstacle, new Vector3(x, y), Quaternion.identity);
                HideMiddleObstacle(j,i,obstacle.GetComponent<ObstacleLabyrinth>());
                obstacle.GetComponent<ObstacleLabyrinth>().X_position = j;
                obstacle.GetComponent<ObstacleLabyrinth>().Y_position = i;
                obstacle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder + (i);
                obstacle.GetComponent<ObstacleLabyrinth>().SetZIndexOfChild((sortingOrder + (i)));
                obstacle.transform.parent = this.transform;
                listObstacles.Add(obstacle.GetComponent<ObstacleLabyrinth>());
            }
        }
    }
    public void HideMiddleObstacle(int x , int y, ObstacleLabyrinth obstacle)
    {
        if (x < 22 || x > 29)
            return;
        if (y < 8 || y > 15)
            return;
        if ((x == 22 && y == 8) || (x == 29 && y == 8) || (x == 29 && y == 15) || (x == 22 && y == 15))
            return;

        obstacle.isMiddle = true;
        obstacle.isBroke = true;
        obstacle.HideObstacle();
    }

    public void SendChangeScalePlayer()
    {
        photonView.RPC("ChangeScalePlayer", RpcTarget.All);

    }

    [PunRPC]
    public void ChangeScalePlayer()
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.transform.localScale = new Vector3(0.4f, 0.4f);
            player.GetComponent<PlayerGO>().canMove = true;
            player.GetComponent<PlayerGO>().movementlControlSpeed = 2;
        }
        
    }

    public void ResetScalePlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.transform.localScale = new Vector3(0.65f, 0.65f);
            player.GetComponent<PlayerGO>().canMove = true;
            player.GetComponent<PlayerGO>().movementlControlSpeed = 4.5f;
        }
    }
    public void ChangeColliderSize(bool change)
    {
        gameManagerParent.GetPlayerMineGO().GetComponent<CapsuleCollider2D>().enabled = !change;
        gameManagerParent.GetPlayerMineGO().transform.Find("CollisionLabyrinth").Find("Collider").gameObject.SetActive(change);
    }

    public void FirstRandomPATH()
    {
        if (firsPathIsFind)
            return;
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour =  FirstRadnomObstacleBroke.GetRandomNeigbourNoneBroken();
        if (!neigbour)
        {
            firsPathIsFind = true;
            if(counterPath1 > 45)
                listPotentialAward.Add(FirstRadnomObstacleBroke);

            listPotentialAwardSecondOption.Add(FirstRadnomObstacleBroke2);
            canLauchSecondePath = true;
            return;
        }
        counterPath1++;
        neigbour.isBrokable = true;
        listFirstPath.Add(neigbour);
        FirstRadnomObstacleBroke = neigbour;
        canBreak = false;
    }

    public void SecondeRandomPATH()
    {
        if (!canLauchSecondePath)
            return;
        if (secondePathIsFind)
            return;
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour = SecondeRadnomObstacleBroke.GetRandomNeigbourNoneBroken();
        if (!neigbour)
        {
            FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
            counterPath2 = 0;
            secondePathIsFind = true;
            if (counter2Path2 > 45)
                listPotentialAward.Add(SecondeRadnomObstacleBroke);

            listPotentialAwardSecondOption.Add(FirstRadnomObstacleBroke2);
            return;
        }
        counter2Path2++;
        neigbour.isBrokable = true;
        listFirstPath.Add(neigbour);
        SecondeRadnomObstacleBroke = neigbour;
        canBreak = false;
    }



    public void AddPathInside()
    {
        if (pathInsideIsFind)
            return;
        if (!firsPathIsFind)
            return;
        if (!secondePathIsFind)
            return;
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour = FirstRadnomObstacleBroke2.GetRandomNeigbourNoneBroken();
        if (!neigbour)
        {
            if (listPath2.Count < 35 && counterPath2 < 70 && coutnerPathInside2 < 100)
            {
                ReverseBreakable(listPath2);
                UpdateListNeibourAllObstacl();
                listPath2.Clear();
                FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
                coutnerPathInside = 0;
                coutnerPathInside2++;
            }
            else
            {
                if (coutnerPathInside > 40)
                    listPotentialAward.Add(FirstRadnomObstacleBroke2);
                listPotentialAwardSecondOption.Add(FirstRadnomObstacleBroke2);
                FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listPath2);
                listPath2.Clear();
                coutnerNumberOfPath++;
                counterPath2 = 0;
                if(coutnerNumberOfPath > 8)
                    pathInsideIsFind = true;
            }
            
            return;
        }
        coutnerPathInside++;
        coutnerPathInside2 = 0;
        neigbour.isBrokable = true;
        counterPath2++;
        listPath2.Add(neigbour);
        FirstRadnomObstacleBroke2 = neigbour;
        canBreak = false;
    }




    public void ReverseBroke(List<ObstacleLabyrinth> path)
    {
        foreach(ObstacleLabyrinth obstacle in path)
        {
            obstacle.ReverseBrokeObstacle();
        }
    }

    public void ReverseBreakable(List<ObstacleLabyrinth> path)
    {
        foreach (ObstacleLabyrinth obstacle in path)
        {
            obstacle.isBrokable = false;
        }
    }

    public ObstacleLabyrinth GetObstaclebrokenInitialInFirstPath(List<ObstacleLabyrinth> path)
    {
        if (path.Count == 0)
            return null;
        return path[Random.Range(0, path.Count)];
    }

    public IEnumerator CanBreakCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        canBreak = true;
    }

    public void AddNeighboursToEachObstacle()
    {
        foreach (ObstacleLabyrinth obstacle in listObstacles)
        {
            if (GetObtacleByPosition(obstacle.X_position + 1, obstacle.Y_position))
            {
                obstacle.listNeigbour.Add(GetObtacleByPosition(obstacle.X_position + 1, obstacle.Y_position));
            }
            if (GetObtacleByPosition(obstacle.X_position - 1, obstacle.Y_position))
            {
                obstacle.listNeigbour.Add(GetObtacleByPosition(obstacle.X_position - 1, obstacle.Y_position));
            }
            if (GetObtacleByPosition(obstacle.X_position, obstacle.Y_position - 1))
            {
                obstacle.listNeigbour.Add(GetObtacleByPosition(obstacle.X_position, obstacle.Y_position - 1));
            }
            if (GetObtacleByPosition(obstacle.X_position, obstacle.Y_position + 1))
            { 
                obstacle.listNeigbour.Add(GetObtacleByPosition(obstacle.X_position, obstacle.Y_position + 1));
            }

        }
    }
    public ObstacleLabyrinth GetObtacleByPosition(int position_x, int position_y)
    {
        foreach (ObstacleLabyrinth obstacle in listObstacles)
        {
            if (obstacle.X_position == position_x && obstacle.Y_position == position_y)
            {
                return obstacle;
            }
        }
        return null;
    }

    public void UpdateListNeibourAllObstacl()
    {
        foreach (ObstacleLabyrinth obstacle in listObstacles)
        {
            obstacle.SetListNeighbourNoneBroken();
        }
    }

    public void AttributeObjectInObstacle()
    {
        objectIsInsert = true;
       
        if (listPotentialAward.Count == 0)
        {
            ChooseRandomObject(listPotentialAwardSecondOption, Random.Range(0, listPotentialAwardSecondOption.Count), false,0);
            ChooseRandomObject(listPotentialAwardSecondOption, Random.Range(0, listPotentialAwardSecondOption.Count), true,1);
            return;
        }
        int indexObstacle = Random.Range(0, listPotentialAward.Count);
        int indexAward = ChooseRandomObject(listPotentialAward,indexObstacle, false,0);
        int indexObstacle2 = Random.Range(0, listPotentialAward.Count);
        if (indexAward == 0 || indexAward == 3)
        {
            ChooseRandomObject(listPotentialAward, indexObstacle2, true,1);
        }
        else
        {
            ChooseRandomObject(listPotentialAward, indexObstacle2, false,1);
        }
       
      
    }

    public int ChooseRandomObject(List<ObstacleLabyrinth> listAward, int indexObject, bool torchIsAlreadyUsed, int indexObjectInList)
    {
        ObstacleLabyrinth obstacleWithAward = listAward[indexObject];
        obstacleWithAward.BrokeObstacle();
        int indexAward = obstacleWithAward.DisplayAward(torchIsAlreadyUsed, indexObjectInList);
        listAward.RemoveAt(indexObject);
        return indexAward;
    }

    public void ActiveModeTestAllObtacle(bool active) 
    {
        foreach (ObstacleLabyrinth obstaclePotential in listObstacles)
        {
            obstaclePotential.activeModeTest = active;
        }
    }

    public void SendAllObstacleData()
    {
        foreach(ObstacleLabyrinth obstacle in listObstacles)
        {
            photonView.RPC("SendDataObstacle", RpcTarget.Others,obstacle.X_position, obstacle.Y_position, obstacle.isBrokable, obstacle.isBroke, obstacle.hasAward, obstacle.indexObject, obstacle.indexObjectInList);
        }
        photonView.RPC("SendPathIsFind", RpcTarget.All, true);
    }
    [PunRPC]
    public void SendDataObstacle(int x, int y, bool isBrokable, bool isBroke, bool isAward , int indexAward, int indexAwardList)
    {
        ObstacleLabyrinth obstacle = GetObtacleByPosition(x, y);
        obstacle.isBrokable = isBrokable;
        obstacle.isBroke = isBroke;
        obstacle.hasAward = isAward;
        obstacle.indexObject = indexAward;
        obstacle.indexObjectInList = indexAwardList;
        if (obstacle.hasAward)
        {
            obstacle.DisplayAwardSimple();
            obstacle.BrokeObstacle();
        }
    }

    public void SendObjectAwardFind(int indexPlayer, int indexAward, int indexObjectList)
    {
        photonView.RPC("SetObjectAwardFind", RpcTarget.All,indexPlayer, indexAward, indexObjectList);
    }
    [PunRPC]
    public void SetObjectAwardFind(int indexPlayer, int indexAward, int indexObjectList)
    {
        if (!gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        if (indexObjectList == 0)
        {
            if (!firstObjectFind)
            {
                photonView.RPC("SendListIndexAward", RpcTarget.All, indexPlayer, indexAward);
                photonView.RPC("SendObjectFind", RpcTarget.All, 1);
                firstObjectFind = true;
            }
            else
                return;
        }
        if(indexObjectList == 1)
        {
            if (!secondObjectFind)
            {
                photonView.RPC("SendListIndexAward", RpcTarget.All, indexPlayer, indexAward);
                photonView.RPC("SendObjectFind", RpcTarget.All, 2);
                secondObjectFind = true;
            }
               
            else
                return;
        }
        if (!(firstObjectFind && secondObjectFind))
            return;

       
        DesactivateRoomChild();
    }
    
    [PunRPC]
    public void SendListIndexAward(int indexPlayer, int indexAward)
    {
        listIndexAwardByPlayer.Add(new KeyValuePair<int, int>(indexPlayer, indexAward));
    }

    [PunRPC]
    public void SendObjectFind(int indexObjectFind)
    {
        if(indexObjectFind == 1 )
            firstObjectFind = true;
        if (indexObjectFind == 2)
            secondObjectFind = true;
    }

    public void DesactivateRoomChild()
    {
        SendDestroyAllObstacle();

        ReactivateCurrentRoom();
        foreach (KeyValuePair<int, int> indexAwardAndPlayer in listIndexAwardByPlayer)
        {
            photonView.RPC("SendActivateObject", RpcTarget.All, indexAwardAndPlayer.Key, indexAwardAndPlayer.Value);
            Debug.LogError(indexAwardAndPlayer.Key);
        }
        DesactivateRoom();
        photonView.RPC("SendClearKeyValuePair", RpcTarget.All);
    }
    
    [PunRPC]
    public void SendActivateObject(int key , int indexObject)
    {
        this.indexObject = indexObject;
        ActivateObjectPower(key);
        ActivateImpostorObject(key);
    }

    [PunRPC]
    public void SendClearKeyValuePair()
    {
        listIndexAwardByPlayer.Clear();
        //gameManagerParent.ActivateCollisionTPOfAllDoor(true);
        gameManagerParent.CloseDoorWhenVote(false);
        gameManagerParent.ui_Manager.DisplayTrapPowerButtonDesactivateTime(true, 6);
        gameManagerParent.ui_Manager.DisplayObjectPowerButtonDesactivateTime(true, 6);
    }

    public void SendDestroyAllObstacle()
    {
        photonView.RPC("DestroyAllObstacle", RpcTarget.All);
    }
    [PunRPC]
    public void DestroyAllObstacle()
    {
        foreach(ObstacleLabyrinth obstacle in listObstacles)
        {
            Destroy(obstacle.gameObject);
        }
        ResetScalePlayer();
        ResetAllData();


    }

    public void ResetAllData()
    {
        listInitalObstacleRight.Clear();
        listObstacles.Clear();
        pathInsideIsFind = false;
        firsPathIsFind = false;
        secondePathIsFind = false;
        listFirstPath.Clear();
        listPath2.Clear();
        listPotentialAward.Clear();
        listPotentialAwardSecondOption.Clear();
        listInitalObstacleRight.Clear();
        listInitalObstacleLeft.Clear();
        firstObjectFind = false;
        secondObjectFind = false;
        coutnerNumberOfPath = 0;
        coutnerPathInside = 0;
        objectIsInsert = false;
        gameManagerParent.speciallyIsLaunch = false;
        gameManagerParent.game.currentRoom.speciallyPowerIsUsed = true;
        ChangeColliderSize(false);
        DisplayListSeparation(false);

    }


    public void SendDesactivateAward(int x, int y)
    {
        photonView.RPC("SetDesactivateAward", RpcTarget.Others, x,y);
    }

    [PunRPC]
    public void SetDesactivateAward(int x, int y)
    {
        ObstacleLabyrinth obstacleWithAward = GetObtacleByPosition(x, y);
        if(obstacleWithAward)
            obstacleWithAward.DesactivateAward();

    }

    public void SendBrokeObstacle(int x, int y)
    {
        photonView.RPC("SetBrokeObstacle", RpcTarget.Others, x, y);
    }
    [PunRPC]
    public void SetBrokeObstacle(int x, int y)
    {
        ObstacleLabyrinth obstacleWithAward = GetObtacleByPosition(x, y);
        obstacleWithAward.BrokeObstacle();
    }

    [PunRPC]
    public void SendPathIsFind(bool objectIsInsert)
    {
        this.objectIsInsert = objectIsInsert;
        ActiveInvisibleWallInMiddle(false);
    }

    public void ActiveInvisibleWallInMiddle(bool active)
    {
        this.transform.Find("MiddleSeparation").gameObject.SetActive(active);
    }
    public void DisplayListSeparation(bool  display)
    {
        this.transform.Find("ListSeparation").gameObject.SetActive(display);
    }

    public void DesactivateAllAwardDeconnexion()
    {
        foreach(ObstacleLabyrinth obstacle in listObstacles)
        {
            if (obstacle.hasAward)
            {
                obstacle.hasAward = false;
                for (int i = 0; i < 7; i++)
                    obstacle.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}
