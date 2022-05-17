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

    public bool isGenerate = false;

    public Hexagone(Room room) {
        this.room = room;
    }
    void Start()
    {
        
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        
    }

    void Update()
    {
        if (room == null) {
            //Debug.LogError("Room is null");
            return;
        }
        if(room.DistancePathFinding == -1)
        {
            this.gameObject.SetActive(false);
        }
        if (room.isOldParadise && !room.IsExit && !room.IsHell)
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            {
                this.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(true);
                this.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241/255f);
            }
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell)
            {
                this.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(true);
                this.GetComponent<SpriteRenderer>().color = new Color(58/255f, 187/255f, 241/255f);
            }
        }
        if (room.IsHell)
        {
            this.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(false);
        }

        //this.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(!(room.Index == gameManager.game.currentRoom.Index));

    }
    void OnMouseOver()
    {
        if ((this.room.isSpecial || this.room.IsExit || this.room.IsHell ) && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            if (!gameManager.ui_Manager.blueWallPaper.transform.Find("Canvas").Find("Text_timer").gameObject.activeSelf)
            {
                if (this.room.isJail || this.room.IsFoggy || this.room.IsVirus || this.room.IsExit || this.room.IsHell)
                {
                    this.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(false);
                    this.transform.Find("Canvas").Find("Distance_text").gameObject.SetActive(true);
                    if (this.room.IsExit)
                        this.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(false);
                    if (this.room.IsHell)
                        this.transform.Find("Canvas").Find("Hell").gameObject.SetActive(false);
                }
               
            }
        }

        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter)
        {
            return;
        }
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasValidPowerImposter)
        {
            return;
        }
        if (room.IsObstacle || room.IsExit || room.IsInitiale)
        {
            return;
        }
        if(room.DistanceExit == 1)
        {
            return;
        }
        int indexPower = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower;
        this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.SetActive(true);
        if (!Input.GetMouseButtonDown(0)){
            // Whatever you want it to do.
            return;
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasValidPowerImposter = true;
        gameManager.gameManagerNetwork.SendHexagoneNewPower(this.room.Index, indexPower);
        gameManager.ui_Manager.listButtonPowerImpostor[indexPower].GetComponent<Button>().interactable = false;
    }

    void OnMouseExit()
    {
        if ((this.room.isSpecial || this.room.IsExit || this.room.IsHell) && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            if (!gameManager.ui_Manager.blueWallPaper.transform.Find("Canvas").Find("Text_timer").gameObject.activeSelf)
            {
                if (this.room.isJail || this.room.IsFoggy || this.room.IsVirus || this.room.IsExit || this.room.IsHell)
                {
                    this.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(true);
                    this.transform.Find("Canvas").Find("Distance_text").gameObject.SetActive(false);
                    if (this.room.IsExit)
                        this.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);
                    if (this.room.IsHell)
                        this.transform.Find("Canvas").Find("Hell").gameObject.SetActive(true);
                }
            }
          
        }
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            return;
        }
        if (room.DistanceExit == 1)
        {
            return;
        }

        int indexPower = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower;
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasClickInPowerImposter)
        {
            if (this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.activeSelf)
            {
                SetSpecalityPower(indexPower);
            }
            return;
        }
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasValidPowerImposter)
        {
            if (this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.activeSelf)
            {
                SetSpecalityPower(indexPower);
            }
            return;
        }
        if (room.IsObstacle || room.IsExit || room.IsInitiale)
        {
            return;
        }
        //int indexPower = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower;
        this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.SetActive(false);
        SetSpecalityPower(indexPower, false);


        this.transform.Find("Canvas").Find("Old_Paradise").gameObject.SetActive(room.isOldParadise && gameManager.game.currentRoom.Index != room.Index);

    }


    public void SetSpecalityPower(int indexPower, bool change = true)
    {

        if (indexPower == 0)
            this.room.IsFoggy = change;
        if (indexPower == 1)
            this.room.IsVirus = change;
        if (indexPower == 2)
            this.room.isJail = change;

        this.room.isSpecial = change;
    }

}
