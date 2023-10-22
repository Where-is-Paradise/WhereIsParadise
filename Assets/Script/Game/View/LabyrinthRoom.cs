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

    public bool activeModeTest = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().ActivateCollisionLabyrinth();

        if (!gameManagerParent.speciallyIsLaunch)
            return;

        FirstRandomPATH();
        SecondeRandomPATH();
        AddPathInside();
        if (pathInsideIsFind && !objectIsInsert)
            AttributeObjectInObstacle();

        if (activeModeTest)
            ActiveModeTestAllObtacle(true);
        else
            ActiveModeTestAllObtacle(false);

    }
    public void StartRoom()
    {
        DisplayAllObstacle();
        ChangeScalePlayer();
        ChangeColliderSize();
       
        AddNeighboursToEachObstacle();



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

        FirstRadnomObstacleBroke = listInitalObstacleRight[Random.Range(0, listInitalObstacleRight.Count)];
        SecondeRadnomObstacleBroke = listInitalObstacleLeft[Random.Range(0, listInitalObstacleLeft.Count)];
        FirstRadnomObstacleBroke.BrokeObstacle();
        SecondeRadnomObstacleBroke.BrokeObstacle();
        gameManagerParent.speciallyIsLaunch = true;
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

    public void ChangeScalePlayer()
    {
        gameManagerParent.GetPlayerMineGO().transform.localScale = new Vector3(0.4f, 0.4f);
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = true;
        gameManagerParent.GetPlayerMineGO().GetComponent<PlayerGO>().movementlControlSpeed = 2;
    }
    public void ChangeColliderSize()
    {
        gameManagerParent.GetPlayerMineGO().GetComponent<CapsuleCollider2D>().enabled = false;
        gameManagerParent.GetPlayerMineGO().transform.Find("CollisionLabyrinth").Find("Collider").gameObject.SetActive(true);
    }

    public void FirstRandomPATH()
    {
        if (firsPathIsFind)
            return;
/*        if (!canBreak)
            return;*/
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour =  FirstRadnomObstacleBroke.GetRandomNeigbourNoneBroken();
        ///Debug.Log(FirstRadnomObstacleBroke.X_position + " " + FirstRadnomObstacleBroke.Y_position);
        if (!neigbour)
        {
            //StartCoroutine(CanBreakCoroutine());
            //FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
            //counterPath2 = 0;
            firsPathIsFind = true;
            if(counterPath1 > 45)
                listPotentialAward.Add(FirstRadnomObstacleBroke);

            listPotentialAwardSecondOption.Add(FirstRadnomObstacleBroke2);
            canLauchSecondePath = true;
            return;
        }
        counterPath1++;
        //neigbour.BrokeObstacle();
        neigbour.isBrokable = true;
        listFirstPath.Add(neigbour);
        FirstRadnomObstacleBroke = neigbour;
        canBreak = false;
        
        //StartCoroutine(CanBreakCoroutine());
    }

    public void SecondeRandomPATH()
    {
        if (!canLauchSecondePath)
            return;
        if (secondePathIsFind)
            return;

        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour = SecondeRadnomObstacleBroke.GetRandomNeigbourNoneBroken();
        ///Debug.Log(FirstRadnomObstacleBroke.X_position + " " + FirstRadnomObstacleBroke.Y_position);
        if (!neigbour)
        {
            //StartCoroutine(CanBreakCoroutine());

            FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
            counterPath2 = 0;
            secondePathIsFind = true;
            if (counter2Path2 > 45)
                listPotentialAward.Add(SecondeRadnomObstacleBroke);

            listPotentialAwardSecondOption.Add(FirstRadnomObstacleBroke2);
            return;
        }
        counter2Path2++;
        //neigbour.BrokeObstacle();
        neigbour.isBrokable = true;
        listFirstPath.Add(neigbour);
        SecondeRadnomObstacleBroke = neigbour;
        canBreak = false;

        //StartCoroutine(CanBreakCoroutine());
    }



    public void AddPathInside()
    {
        if (pathInsideIsFind)
            return;
        if (!firsPathIsFind)
            return;
        if (!secondePathIsFind)
            return;
/*        if (!canBreak)
            return;*/
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour = FirstRadnomObstacleBroke2.GetRandomNeigbourNoneBroken();
        if (!neigbour)
        {
            if (listPath2.Count < 35 && counterPath2 < 70)
            {

                //ReverseBroke(listPath2);
                ReverseBreakable(listPath2);
                UpdateListNeibourAllObstacl();
                listPath2.Clear();
                FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
                coutnerPathInside = 0;
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
        //neigbour.BrokeObstacle();
        neigbour.isBrokable = true;
        counterPath2++;
        listPath2.Add(neigbour);
        FirstRadnomObstacleBroke2 = neigbour;
        canBreak = false;
      
        //StartCoroutine(CanBreakCoroutine());
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
            ChooseRandomObject(listPotentialAwardSecondOption, Random.Range(0, listPotentialAwardSecondOption.Count), false);
            ChooseRandomObject(listPotentialAwardSecondOption, Random.Range(0, listPotentialAwardSecondOption.Count), true);
            return;
        }
        int indexObstacle = Random.Range(0, listPotentialAward.Count);
        int indexAward = ChooseRandomObject(listPotentialAward,indexObstacle, false);
        int indexObstacle2 = Random.Range(0, listPotentialAward.Count);
        if (indexAward == 0 || indexAward == 4)
        {
            ChooseRandomObject(listPotentialAward, indexObstacle2, true);
        }
        else
        {
            ChooseRandomObject(listPotentialAward, indexObstacle2, false);
        }
       
      
    }

    public int ChooseRandomObject(List<ObstacleLabyrinth> listAward, int indexObject, bool torchIsAlreadyUsed)
    {
        ObstacleLabyrinth obstacleWithAward = listAward[indexObject];
        obstacleWithAward.BrokeObstacle();
        int indexAward = obstacleWithAward.DisplayAward(torchIsAlreadyUsed);
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
}
