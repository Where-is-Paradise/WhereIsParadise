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
    }
    void OnMouseOver()
    {
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
        int indexPower = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower;
        this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.SetActive(true);
        if (!Input.GetMouseButtonDown(0)){
            // Whatever you want it to do.
            return;
        }
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasValidPowerImposter = true;
        gameManager.gameManagerNetwork.SendHexagoneNewPower(this.room.Index, indexPower);
    }

    void OnMouseExit()
    {
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
        int indexPower = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().indexPower;
        this.transform.Find("Canvas").Find("ImpostorPower").GetChild(indexPower).gameObject.SetActive(false);
    }


}
