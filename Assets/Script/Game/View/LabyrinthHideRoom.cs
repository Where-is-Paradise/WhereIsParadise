using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LabyrinthHideRoom : MonoBehaviourPun
{
    public GameObject obstacleClone;
    public List<GameObject> listObstacles;
    public List<GameObject> listObstaclesborder;
    public List<GameObject> listObstaclesborderWithoutTorch;
    public ObstacleLabyrinth obstacleWithTorch;
    public float initialPositionObstacle_x = -6f;
    public float initialPositionObstacle_y = 2f;
    public float decalageObstacleRight = 0.9f;
    public float decalageObstacleDown = 0.9f;
    public int width = 15;
    public int height = 7;

    // Pathfinding
    public List<ObstacleLabyrinth> openList = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> closeList = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> listPotentialExitPathfinding;
    public List<ObstacleLabyrinth> listPotentialEmpty  = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth exitPathFinding;
    public ObstacleLabyrinth currentNode;
    public int retourErrorPathFindFunction =  0;
    public bool pathIsFinish = false;
    public bool pathFalseOneIsFinish = false;
    public bool pathFalseSemiOneOneIsFinish = false;
    public bool pathFalseSemiTwoIsFinish = false;

    public List<ObstacleLabyrinth> currentPath = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth inverseObstacleWithTorch;
    public List<ObstacleLabyrinth> pathOne = new List<ObstacleLabyrinth>();
    public List<ObstacleLabyrinth> pathTwo = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth randomExitToSemiWay;
    public int waySize = 50;
    public int counter = 0;

    public GameManager gameManager;
    public bool DataObstacleAreSent = false;
    public bool roomIsLaunched = false;

    // Start is called before the first frame update
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (!roomIsLaunched)
            return;
        if (!pathIsFinish)
        {
            CreateWayToTorch2();
        }
        else
        {
            if (!pathFalseOneIsFinish)
            {
                CreateFalseWay();
            }
            else
            {
                if (!DataObstacleAreSent)
                    SendStatusToAllObstacles();
            }      
        }
    }

    public void LaunchLabyrintheRoom()
    {
        gameManager.speciallyIsLaunch = true;
        StartCoroutine(LauchLabyrintheAfterTp());
    }
    public IEnumerator LauchLabyrintheAfterTp()
    {
        yield return new WaitForSeconds(2);
      
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
        gameManager.ui_Manager.DisplayKeyAndTorch(false); 
        this.transform.Find("ListSeparation").Find("SeparationsMiddleUp").gameObject.SetActive(true);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.CloseDoorWhenVote(true);
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObtacles();
            AddNeighboursToEachObstacle();
            DesactivateObstaclesInMiddle();
            SetListObstacleBorder();
            AddTorch();
            SetInvertTorchPosition();
            SetListObstacleBorderWithoutTorch();
            waySize = Random.Range(15, 60);
            roomIsLaunched = true;
        }
    }

    public void SpawnObtacles()
    {
        for(int i= 0; i< height; i++)
        { 
            for (int j =0; j < width; j++)
            {
                GameObject newObstacle = PhotonNetwork.Instantiate("Obstacle", this.transform.Find("ListObstacle").position , Quaternion.identity);
                newObstacle.GetComponent<ObstacleLabyrinth>().SendInitiationData(i,j);
                listObstacles.Add(newObstacle);
            }
        }
    }

    public void DesactivateObstaclesInMiddle()
    {

        List<ObstacleLabyrinth> ListmiddleObstacle = SetAndGetObstacleInMiddle();
        foreach (ObstacleLabyrinth middleObstacle in ListmiddleObstacle)
        {
            middleObstacle.isEmpty = true;
            middleObstacle.isMiddle = true;
            foreach (ObstacleLabyrinth neigbour in middleObstacle.listNeigbour)
            {
                neigbour.isEmpty = true;
                neigbour.isMiddle = true;
                foreach (ObstacleLabyrinth neigbour2 in neigbour.listNeigbour)
                {
                    neigbour2.isPotentialExit = true;
                    if (neigbour2.isEmpty)
                        continue;
                    listPotentialExitPathfinding.Add(neigbour2);
                }
            }
        }
        listPotentialExitPathfinding = DeleteEmptyObstacle(listPotentialExitPathfinding);
    }

    public List<ObstacleLabyrinth> DeleteEmptyObstacle(List<ObstacleLabyrinth> list)
    {
        List<ObstacleLabyrinth> ListObstacleWithoutIsEmpty = new List<ObstacleLabyrinth>();
        foreach (ObstacleLabyrinth obstacle in list)
        {
            if (!obstacle.isEmpty)
                ListObstacleWithoutIsEmpty.Add(obstacle);
        }
        return ListObstacleWithoutIsEmpty;
    }

    public ObstacleLabyrinth GetObtacleByPosition(int position_x, int position_y)
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if(obtacleComponenet.position_x == position_x && obtacleComponenet.position_y == position_y)
            {
                return obtacleComponenet;
            }
        }
        return null;
    }

    public List<ObstacleLabyrinth> SetAndGetObstacleInMiddle()
    {
        List<ObstacleLabyrinth> listMiddleObstacle = new List<ObstacleLabyrinth>();
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if (obtacleComponenet.position_x > (width/2)-4 && obtacleComponenet.position_x < (width / 2) + 3)
            {
                if (obtacleComponenet.position_y > (height / 2) - 1 && obtacleComponenet.position_y < (height / 2) + 2)
                {
                    obtacleComponenet.isMiddle = true;
                    obtacleComponenet.isEmpty = true;
                    listMiddleObstacle.Add(obtacleComponenet);
                }
            }
        }
        return listMiddleObstacle;
    }

    public void SetListObstacleBorder()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if (obtacleComponenet.position_x == 0 || obtacleComponenet.position_x == width - 1)
            {
                listObstaclesborder.Add(obstacle);
                continue;
            }
            if(obtacleComponenet.position_y == 0 || obtacleComponenet.position_y == height - 1)
            {
                listObstaclesborder.Add(obstacle);
                continue;
            }
        }
    }

    public void SetListObstacleBorderWithoutTorch()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if (obtacleComponenet.hasTorch)
                continue;
            if (obtacleComponenet.position_x == 0 || obtacleComponenet.position_x == width - 1)
            {
                listObstaclesborderWithoutTorch.Add(obstacle);
                continue;
            }
            if (obtacleComponenet.position_y == 0 || obtacleComponenet.position_y == height - 1)
            {
                listObstaclesborderWithoutTorch.Add(obstacle);
                continue;
            }
        }
    }

    public void AddNeighboursToEachObstacle()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if (GetObtacleByPosition(obtacleComponenet.position_x + 1, obtacleComponenet.position_y)){
                obtacleComponenet.listNeigbour.Add(GetObtacleByPosition(obtacleComponenet.position_x + 1, obtacleComponenet.position_y));
            }
            if (GetObtacleByPosition(obtacleComponenet.position_x - 1, obtacleComponenet.position_y))
            {
                obtacleComponenet.listNeigbour.Add(GetObtacleByPosition(obtacleComponenet.position_x - 1, obtacleComponenet.position_y));
            }
            if (GetObtacleByPosition(obtacleComponenet.position_x, obtacleComponenet.position_y - 1))
            {
                obtacleComponenet.listNeigbour.Add(GetObtacleByPosition(obtacleComponenet.position_x, obtacleComponenet.position_y - 1));
            }
            if (GetObtacleByPosition(obtacleComponenet.position_x, obtacleComponenet.position_y + 1))
            {
                obtacleComponenet.listNeigbour.Add(GetObtacleByPosition(obtacleComponenet.position_x, obtacleComponenet.position_y + 1));
            }
           
        }
    }

    public void SendStatusToAllObstacles()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            obtacleComponenet.SendData(obtacleComponenet.isEmpty , obtacleComponenet.hasTorch , obtacleComponenet.isMiddle);
        }
        DataObstacleAreSent = true;
    }
    public void AddNeighboursNoneObstacle()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            obtacleComponenet.SetListNeigbourNoneObstacle();
        }
    }

    public void AddExitPathFinding()
    {
        int randomIndex = Random.Range(0, listPotentialExitPathfinding.Count);
        exitPathFinding = listPotentialExitPathfinding[randomIndex];
        exitPathFinding.isEmpty = true;
    }
    public void AddTorch()
    {
        int randomIndex = Random.Range(0, listObstaclesborder.Count);
        listObstaclesborder[randomIndex].GetComponent<ObstacleLabyrinth>().hasTorch = true;
        obstacleWithTorch = listObstaclesborder[randomIndex].GetComponent<ObstacleLabyrinth>();
        obstacleWithTorch.isObtacleToPathFinding = false;
    }
    public void AddRandomObstaclesPathFinding()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            float random = Random.Range(0, 10);
            if(random < 0f)
            {
                if (!obstacle.GetComponent<ObstacleLabyrinth>().hasTorch && !obstacle.GetComponent<ObstacleLabyrinth>().isPotentialExit)
                    obstacle.GetComponent<ObstacleLabyrinth>().isObtacleToPathFinding = true;
            }
        }
    }
    public void ResetObstaclesPathFinding()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            obstacle.GetComponent<ObstacleLabyrinth>().isObtacleToPathFinding = false;
        }
    }

    public void CreateWayToTorch()
    {
        if (openList.Count > 0)
        {
            ObstacleLabyrinth current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                
                if (openList[i].Fcost() < current.Fcost() || (openList[i].Fcost() == current.Fcost()))
                {
                    if (openList[i].hcost < current.hcost)
                        current = openList[i];
                }
            }
            openList.Remove(current);
            closeList.Add(current);

            if (current.position_x == exitPathFinding.position_x && current.position_y == exitPathFinding.position_y)
            {
                retourErrorPathFindFunction =  1;
                return;
            }
            foreach (ObstacleLabyrinth neighbour in current.listNeigbour)
            {
                if (neighbour.position_x == exitPathFinding.position_x && neighbour.position_y == exitPathFinding.position_y)
                {
                    neighbour.parent = current;
                    retourErrorPathFindFunction =  1;
                    return;
                }
                if (neighbour.isObtacleToPathFinding || closeList.Contains(neighbour))
                {
                    continue;
                }
                int newCostToNeighbour = current.gcost + (GetDistance(neighbour, current) * 5);
                if (newCostToNeighbour < neighbour.gcost || !openList.Contains(neighbour) || GetDistance(neighbour, current) == 0)
                {
                    neighbour.hcost = GetDistance(neighbour, exitPathFinding);
                    neighbour.gcost = newCostToNeighbour;
                    neighbour.parent = current;
                }

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);

                }
            }
        }
        else
        {
            retourErrorPathFindFunction = -1;
            Debug.Log("-1 ££££");
            foreach(GameObject obstacle in listObstacles)
            {
                obstacle.GetComponent<ObstacleLabyrinth>().parent = null;
            }
        }
       
    }

    private void RetracePath(ObstacleLabyrinth startNode)
    {
        if (currentNode != startNode)
        {
            currentPath.Add(currentNode);
            currentNode = currentNode.parent;
            currentNode.isEmpty = true;
        }
        else
        {
            pathIsFinish = true;
        }
    }

    private void ResetObstacles()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            if (obstacle.GetComponent<ObstacleLabyrinth>().isMiddle)
                continue;
            obstacle.GetComponent<ObstacleLabyrinth>().isEmpty = false;
            obstacle.GetComponent<ObstacleLabyrinth>().parent = null;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }
    }
    private void ResetPath()
    {
        foreach(ObstacleLabyrinth obstacle in currentPath)
        {
            if (obstacle.GetComponent<ObstacleLabyrinth>().isMiddle)
                continue;
            obstacle.isEmpty = false;
            obstacle.parent = null;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }


        currentPath.RemoveRange(0, currentPath.Count - 1);
        
    }
    private void ResetParent()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            obstacle.GetComponent<ObstacleLabyrinth>().parent = null;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }
        currentPath.RemoveRange(0, currentPath.Count - 1);
    }
    private void ResetParentOnPath()
    {
        foreach (ObstacleLabyrinth obstacle in currentPath)
        {
            if (obstacle.GetComponent<ObstacleLabyrinth>().isMiddle)
                continue;
            obstacle.parent = null;
            //obstacle.isEmpty = false;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }
        currentPath.RemoveRange(0, currentPath.Count - 1);
    }

    static public int GetDistance(ObstacleLabyrinth a, ObstacleLabyrinth b)
    {
        return (Mathf.Abs(a.position_x - b.position_x)  + Mathf.Abs(a.position_y - b.position_y));
    }

    public void HideRandomNeigbourObstacle(ObstacleLabyrinth obstacle )
    {
        int randomNeigbour = Random.Range(0, obstacle.listNeigbour.Count);
        obstacle.listNeigbour[randomNeigbour].isEmpty = true;
        //HideRandomNeigbourObstacle()

    }

    public void DesactivateRoom(int indexPlayer)
    {
        GameObject playerWinner = gameManager.GetPlayer(indexPlayer);
        GiveAwardToPlayer(playerWinner);
        SendResetColor();
        SendHideAllObstacles();
        photonView.RPC("SendSpeciallyPowerIsUsed", RpcTarget.All, true);


    }
    public void SendHideAllObstacles()
    {
        this.transform.Find("ListObstacle").gameObject.SetActive(false);
        this.transform.Find("ListSeparation").gameObject.SetActive(false);
        gameManager.ui_Manager.DisplayKeyAndTorch(true);
        gameManager.speciallyIsLaunch = false;
        this.transform.Find("ListSeparation").Find("SeparationsMiddleUp").gameObject.SetActive(false);
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        gameManager.game.nbTorch++;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().canLaunchExplorationLever = true;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }
    public void SendResetColor()
    {
        photonView.RPC("ResetColorAllPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void ResetColorAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().ResetHeart();
            player.GetComponent<PlayerGO>().isTouchByAx = false;
        }
    }

    [PunRPC]
    public void SendSpeciallyPowerIsUsed(bool speciallyPowerIsUsed)
    {
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = speciallyPowerIsUsed;
        gameManager.labyrinthIsUsed = speciallyPowerIsUsed;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(false);
    }

    public void CreateWayToTorch2()
    {
        GetNeigbourUntilExit(obstacleWithTorch);
    }

    public void GetNeigbourUntilExit(ObstacleLabyrinth current)
    {
        if (current.isPotentialExit)
        {
            if (currentPath.Count <= GetDistance(current, obstacleWithTorch) + waySize)
            {
                ResetObstacles();
                ResetPath();
                pathOne.RemoveRange(0, pathOne.Count - 1);
                return;
            }
            currentPath.RemoveRange(0, currentPath.Count - 1);
            pathIsFinish = true;
            return;
        }
        current.SetParentToAllNeigbour(true);
        current.SetListNeigbourNoneObstacle();
        ObstacleLabyrinth obstacleNeigbour = current.GetRandomNeigbourNoneObstacle();
        current.SetParentToAllNeigbour(false);
        currentPath.Add(current);
        pathOne.Add(current);
        if (obstacleNeigbour == null)
        {
            ResetObstacles();
            ResetPath();
            if (pathOne.Count > 0 )
                pathOne.RemoveRange(0, pathOne.Count - 1);
            return;
        }
       
        Debug.Log(obstacleNeigbour.position_x + " " + obstacleNeigbour.position_y);
        obstacleNeigbour.parent = current;
        obstacleNeigbour.isEmpty = true;
        
        GetNeigbourUntilExit(obstacleNeigbour);
    }

    public void SetInvertTorchPosition()
    {
        int invertX = Mathf.Abs(obstacleWithTorch.position_x - (width - 1 ));
        int invertY = Mathf.Abs(obstacleWithTorch.position_y - (height - 1));
        Debug.Log(invertX + " " + invertY);
        inverseObstacleWithTorch = GetObtacleByPosition(invertX, invertY);
        inverseObstacleWithTorch.isObtacleToPathFinding = false;
        inverseObstacleWithTorch.isEmpty = true;
    }

    public void CreateFalseWay()
    {
        int randomIndex = Random.Range(0, listObstaclesborderWithoutTorch.Count);
        GetNeigbourUntilFalseExit(listObstaclesborderWithoutTorch[randomIndex].GetComponent<ObstacleLabyrinth>());
    }


    public void GetNeigbourUntilFalseExit(ObstacleLabyrinth current)
    {
        if (current.isPotentialExit)
        {
            ResetParent();
            pathFalseOneIsFinish = true;
            return;
        }
        current.SetParentToAllNeigbour(true);
        current.SetListNeigbourNoneObstacle();
        ObstacleLabyrinth obstacleNeigbour = current.GetRandomNeigbourNoneObstacle();
        current.SetParentToAllNeigbour(false);
        
        if (obstacleNeigbour == null)
        {
            ResetParentOnPath();
            if(pathTwo.Count > 0)
                pathTwo.RemoveRange(0, pathTwo.Count - 1);
            return;
        }
        currentPath.Add(obstacleNeigbour);
        pathTwo.Add(obstacleNeigbour);
        //Debug.Log(obstacleNeigbour.position_x + " " + obstacleNeigbour.position_y);
        obstacleNeigbour.parent = current;
        obstacleNeigbour.isEmpty = true;

        GetNeigbourUntilFalseExit(obstacleNeigbour);
    }

    public void AddSemiWayOne()
    {
        randomExitToSemiWay = listObstaclesborderWithoutTorch[Random.Range(0, listObstaclesborderWithoutTorch.Count)].GetComponent<ObstacleLabyrinth>();
        GetNeigbourUntilFalseExitSemiOne(pathOne[Random.Range(0, pathOne.Count)]);
    }
    public void AddSemiWayTwo()
    {
        randomExitToSemiWay = listObstaclesborderWithoutTorch[Random.Range(0, listObstaclesborderWithoutTorch.Count)].GetComponent<ObstacleLabyrinth>();
        GetNeigbourUntilFalseExitSemiTwo(pathTwo[Random.Range(0, pathTwo.Count)]);
    }

    public void GetNeigbourUntilFalseExitSemiOne(ObstacleLabyrinth current)
    {
        if (current.SameObstacleByPosition(randomExitToSemiWay))
        {
            ResetParent();
            pathFalseSemiOneOneIsFinish = true;
            return;
        }
        current.SetParentToAllNeigbour(true);
        current.SetListNeigbourNoneObstacle();
        ObstacleLabyrinth obstacleNeigbour = current.GetRandomNeigbourNoneObstacle();
        current.SetParentToAllNeigbour(false);

        if (obstacleNeigbour == null)
        {
            ResetParentOnPath();
            return;
        }
        currentPath.Add(obstacleNeigbour);
        Debug.Log(obstacleNeigbour.position_x + " " + obstacleNeigbour.position_y);
        obstacleNeigbour.parent = current;
        obstacleNeigbour.isEmpty = true;

        GetNeigbourUntilFalseExit(obstacleNeigbour);
    }

    public void GetNeigbourUntilFalseExitSemiTwo(ObstacleLabyrinth current)
    {
        if (current.SameObstacleByPosition(randomExitToSemiWay))
        {
            ResetParent();
            pathFalseSemiTwoIsFinish = true;
            return;
        }
        current.SetParentToAllNeigbour(true);
        current.SetListNeigbourNoneObstacle();
        ObstacleLabyrinth obstacleNeigbour = current.GetRandomNeigbourNoneObstacle();
        current.SetParentToAllNeigbour(false);

        if (obstacleNeigbour == null)
        {
            ResetParentOnPath();
            return;
        }
        currentPath.Add(obstacleNeigbour);
        //Debug.Log(obstacleNeigbour.position_x + " " + obstacleNeigbour.position_y);
        obstacleNeigbour.parent = current;
        obstacleNeigbour.isEmpty = true;

        GetNeigbourUntilFalseExit(obstacleNeigbour);
    }
    
}
