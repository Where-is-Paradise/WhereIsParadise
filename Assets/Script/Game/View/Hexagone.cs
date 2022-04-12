using UnityEngine;
using UnityEngine.UI;

public class Hexagone : MonoBehaviour
{

    [SerializeField]
    private Room room;
    public Room Room { get { return room; } set { room = value;} }
        
    private GameManager gameManager;

    public Text distanceText;
    public Text index_text;


    void Start()
    {
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        
    }

    void Update()
    {
        if(room.DistancePathFinding == -1)
        {
            this.gameObject.SetActive(false);
        }

        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell)
        {
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

        if (room.HasKey)
        {
            this.transform.GetChild(0).GetChild(2).gameObject.SetActive(display);
            this.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = room.DistancePathFinding.ToString();
            this.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = room.Index.ToString();
        }

        if (room.IsExit)
        {
            this.transform.GetChild(0).GetChild(3).gameObject.SetActive(display);
            if (room.HasKey)
            {
                this.transform.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                this.transform.transform.GetChild(0).GetChild(5).gameObject.SetActive(display);
            }
          
        }

        if (gameManager.hell && room.IsHell)
        {
            if (room.HasKey)
            {
                this.transform.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                this.transform.transform.GetChild(0).GetChild(5).gameObject.SetActive(display);
            }
        }
       
        this.transform.GetChild(0).GetComponent<Canvas>().overrideSorting = true;
    }
}
