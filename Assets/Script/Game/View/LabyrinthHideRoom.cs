using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthHideRoom : MonoBehaviour
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

    public List<ObstacleLabyrinth> path = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth inverseObstacleWithTorch;

    public int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        obstacleClone = this.transform.Find("Obstacle").gameObject;

        SpawnObtacles();
        AddNeighboursToEachObstacle();
        DesactivateObstaclesInMiddle();
        SetListObstacleBorder();
        AddTorch();
        AddRandomObstaclesPathFinding();
        
        //AddExitPathFinding();
        SetInvertTorchPosition();
        SetListObstacleBorderWithoutTorch();
        //pathfinding
        /*        
                openList.Add(obstacleWithTorch);
                currentNode = exitPathFinding;*/

        //StartCoroutine(CreateWayToTorch2());
        //StartCoroutine(CreateFalseWay());

    }

    // Update is called once per frame
    void Update()
    {
        if (!pathIsFinish)
        {
            CreateWayToTorch2();
        }
        else
        {
            if (!pathFalseOneIsFinish)
                CreateFalseWay();
        }

        //Debug.Log(retourErrorPathFindFunction);
        /*        if(pathIsFinish && path.Count < 20)
                {
                    retourErrorPathFindFunction = -1;
                    pathIsFinish = false;
                    ResetPath();
                }
                if (retourErrorPathFindFunction == 1 )
                {
                    RetracePath(obstacleWithTorch);
                    return;
                }*/
        /*        if(retourErrorPathFindFunction == 0)
                {
                    CreateWayToTorch();
                }

                if (retourErrorPathFindFunction == -1)
                {
                    openList.Add(obstacleWithTorch);
                    if(closeList.Count > 0)
                        closeList.RemoveRange(0, closeList.Count-1);
                    retourErrorPathFindFunction = 0;
                    ResetObstaclesPathFinding();
                    AddRandomObstaclesPathFinding();
                }   */
    }

    public void SpawnObtacles()
    {
        for(int i= 0; i< height; i++)
        { 
            for (int j =0; j < width; j++)
            {
                GameObject newObstacle = Instantiate(obstacleClone, this.transform.Find("ListObstacle"));
                newObstacle.transform.position = new Vector3(initialPositionObstacle_x + ( j * decalageObstacleRight),
                    initialPositionObstacle_y + ( i * -decalageObstacleDown));
                newObstacle.GetComponent<ObstacleLabyrinth>().isEmpty = false;
                newObstacle.GetComponent<ObstacleLabyrinth>().position_x = j;
                newObstacle.GetComponent<ObstacleLabyrinth>().position_y = i;
                listObstacles.Add(newObstacle);
            }
        }
    }

    public void DesactivateObstaclesInMiddle()
    {

        ObstacleLabyrinth middleObstacle = SetAndGetObstacleInMiddle();
        middleObstacle.isEmpty = true;
        middleObstacle.isMiddle = true;
        foreach(ObstacleLabyrinth neigbour in middleObstacle.listNeigbour)
        {
            neigbour.isEmpty = true;
            neigbour.isMiddle = true;
            foreach (ObstacleLabyrinth neigbour2 in neigbour.listNeigbour)
            {
                neigbour2.isPotentialExit = true;
                listPotentialExitPathfinding.Add(neigbour2);
            }
        }
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

    public ObstacleLabyrinth SetAndGetObstacleInMiddle()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            ObstacleLabyrinth obtacleComponenet = obstacle.GetComponent<ObstacleLabyrinth>();
            if (obtacleComponenet.position_x == (width/2)-1 && obtacleComponenet.position_y == (height/2)-1)
            {
                obtacleComponenet.isMiddle = true;
                obtacleComponenet.isEmpty = true;
                return obtacleComponenet;
            }
        }
        return null;
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


    public void CreateRandomWay()
    {

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
            path.Add(currentNode);
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
        foreach(ObstacleLabyrinth obstacle in path)
        {
            if (obstacle.GetComponent<ObstacleLabyrinth>().isMiddle)
                continue;
            obstacle.isEmpty = false;
            obstacle.parent = null;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }
        path.RemoveRange(0, path.Count - 1);
    }
    private void ResetParent()
    {
        foreach (GameObject obstacle in listObstacles)
        {
            obstacle.GetComponent<ObstacleLabyrinth>().parent = null;
        }
    }
    private void ResetParentOnPath()
    {
        foreach (ObstacleLabyrinth obstacle in path)
        {
            obstacle.parent = null;
            obstacle.GetComponent<ObstacleLabyrinth>().ResetListNeibourNoneObstacle();
        }
        path.RemoveRange(0, path.Count - 1);
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

    public void DesactivateAllObtacle()
    {

    }


    public void CreateWayToTorch2()
    {
        GetNeigbourUntilExit(obstacleWithTorch);
    }

    public void GetNeigbourUntilExit(ObstacleLabyrinth current)
    {
        if (current.isPotentialExit)
        {
            Debug.Log(GetDistance(current, obstacleWithTorch) + " " + GetDistance(current, obstacleWithTorch) + 8 + " " + path.Count);
            if (path.Count <= GetDistance(current, obstacleWithTorch) + 50)
            {
                ResetObstacles();
                ResetPath();
                return;
            }
            path.RemoveRange(0, path.Count - 1);
            pathIsFinish = true;
            return;
        }
        current.SetParentToAllNeigbour(true);
        current.SetListNeigbourNoneObstacle();
        ObstacleLabyrinth obstacleNeigbour = current.GetRandomNeigbourNoneObstacle();
        current.SetParentToAllNeigbour(false);
        path.Add(current);
        if (obstacleNeigbour == null)
        {
            ResetObstacles();
            ResetPath();
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
            return;
        }
        path.Add(obstacleNeigbour);
        //Debug.Log(obstacleNeigbour.position_x + " " + obstacleNeigbour.position_y);
        obstacleNeigbour.parent = current;
        obstacleNeigbour.isEmpty = true;

        GetNeigbourUntilFalseExit(obstacleNeigbour);
    }
}
