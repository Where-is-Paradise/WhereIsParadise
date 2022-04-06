using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hexagone : MonoBehaviour
{

    public int index;
    public int pos_X;
    public int pos_Y;

    public bool isExit;
    public bool isObstacle;
    public int distance_exit;
    public int distance_pathFinding;
    public bool isInitialeRoom;

    public Text distanceText;
    public Text index_text;

    public List<RoomHex> listNeighbour;

    public bool isTraversed;
    public bool isHell;
    public bool isFoggy;
    public bool isVirus;
    public bool hasKey;

    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if(distance_pathFinding == -1)
        {
            this.gameObject.SetActive(false);
        }

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell)
        {
            //DisplayTextAndImage(true);

            if (this.transform.position.x > -1.9f && this.transform.position.x < 7.4f && this.transform.position.y < 3.8f && this.transform.position.y > -3.8f)
            {
                DisplayTextAndImage(true);
            }
            else
            {
                DisplayTextAndImage(false);
            }
        }

    }


    public void DisplayTextAndImage(bool display)
    {
        this.transform.GetChild(0).GetChild(0).gameObject.SetActive(display);
        this.transform.GetChild(0).GetChild(1).gameObject.SetActive(display);
        if (hasKey)
        {
            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(display);
            this.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = distance_pathFinding.ToString();
            this.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = index.ToString();
        }
        if (isExit)
        {
            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(display);
            if (hasKey)
            {
                this.transform.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                this.transform.transform.GetChild(0).GetChild(5).gameObject.SetActive(display);
            }
          
        }
        if (gameManager.hell)
        {
            if (this.transform.GetComponent<Hexagone>().pos_X == gameManager.hell.GetPos_X() && this.transform.GetComponent<Hexagone>().pos_Y == gameManager.hell.GetPos_Y())
            {
                if (hasKey)
                {
                    this.transform.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    this.transform.transform.GetChild(0).GetChild(5).gameObject.SetActive(display);
                }
            }
        }
       
        this.transform.GetChild(0).GetComponent<Canvas>().overrideSorting = true;
    }
}
