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
    public ObstacleLabyrinth FirstRadnomObstacleBroke;
    public bool canBreak = true;

    public List<ObstacleLabyrinth> listFirstPath = new List<ObstacleLabyrinth>();
    public ObstacleLabyrinth FirstRadnomObstacleBroke2;
    public List<ObstacleLabyrinth> listInitalObstacle;

    public List<ObstacleLabyrinth> listPath2 = new List<ObstacleLabyrinth>();
    public bool secondPathIsFind = false;
    public int counterPath2;
    public int coutnerNumberOfPath = 0;
    public List<ObstacleLabyrinth> listPotentialAward = new List<ObstacleLabyrinth>();

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
        SecondeRandomPath();
    }
    public void StartRoom()
    {
        DisplayAllObstacle();
        ChangeScalePlayer();
        ChangeColliderSize();
       
        AddNeighboursToEachObstacle();

        FirstRadnomObstacleBroke = GetObtacleByPosition(30, 11);

        listInitalObstacle.Add(FirstRadnomObstacleBroke);
        listInitalObstacle.Add(GetObtacleByPosition(21, 12));
        FirstRadnomObstacleBroke.BrokeObstacle();
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


    public void AttributeObjectInObstacle()
    {

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
            StartCoroutine(CanBreakCoroutine());
            FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
            counterPath2 = 0;
            firsPathIsFind = true;
            listPotentialAward.Add(FirstRadnomObstacleBroke);
            return;
        }
        
        //neigbour.BrokeObstacle();
        neigbour.isBrokable = true;
        listFirstPath.Add(neigbour);
        FirstRadnomObstacleBroke = neigbour;
        if (FirstRadnomObstacleBroke.hasAward)
            firsPathIsFind = true;
        canBreak = false;
        
        //StartCoroutine(CanBreakCoroutine());
    }
    public void SecondeRandomPath()
    {
        if (secondPathIsFind)
            return;
        if (!firsPathIsFind)
            return;
/*        if (!canBreak)
            return;*/
        UpdateListNeibourAllObstacl();
        ObstacleLabyrinth neigbour = FirstRadnomObstacleBroke2.GetRandomNeigbourNoneBroken();
        if (!neigbour)
        {
            if (listPath2.Count < 35 && counterPath2 < 50)
            {

                //ReverseBroke(listPath2);
                ReverseBreakable(listPath2);
                UpdateListNeibourAllObstacl();
                listPath2.Clear();
                FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listFirstPath);
            }
            else
            {
               
                listPotentialAward.Add(FirstRadnomObstacleBroke2);
                FirstRadnomObstacleBroke2 = GetObstaclebrokenInitialInFirstPath(listPath2);
                listPath2.Clear();
                coutnerNumberOfPath++;
                counterPath2 = 0;
                if(coutnerNumberOfPath > 5)
                    secondPathIsFind = true;
            }
            return;
        }
        //neigbour.BrokeObstacle();
        neigbour.isBrokable = true;
        Debug.Log("sa passe");
        counterPath2++;
        listPath2.Add(neigbour);
        FirstRadnomObstacleBroke2 = neigbour;
        if (FirstRadnomObstacleBroke.hasAward)
            firsPathIsFind = true;
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
}
