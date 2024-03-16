using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hexagone : MonoBehaviourPun
{

    [SerializeField]
    private Room room;
    public Room Room { get { return room; } set { room = value;} }
        
    private GameManager gameManager;

    public Text distanceText;
    public Text index_text;

    public bool isGenerate = false;
    public bool isLighted = false;
    public bool isLightByOther = false;

    public bool isLostSoul = false;

    public Hexagone(Room room) {
        this.room = room;
    }
    void Start()
    {
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        if (Room && Room.isTooFar)
            Destroy(this.gameObject);

        DiplayInformationSpeciallyRoom(true);
    }

    void Update()
    {
        if (room == null) {
            return;
        }
        if(room.DistancePathFinding == -1)
        {
            this.gameObject.SetActive(false);
        }
        if (room.isOldParadise && !room.IsExit && !room.IsHell)
        {
            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation)
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


    private void OnMouseUp()
    {
       
    }

    void OnMouseOver()
    {

       

#if UNITY_IOS || UNITY_ANDROID
        return;
#endif
        if (!this.room.IsObstacle && (this.room.isSpecial || this.room.IsExit || this.room.IsHell || this.room.isImpostorRoom || this.room.isNewParadise))
        {
            if (gameManager.ui_Manager.map.activeSelf)
            {

                this.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(false);
                this.transform.Find("Canvas").Find("Distance_text").gameObject.SetActive(true);
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                {
                    if (this.room.IsExit)
                        this.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(false);
                    if (this.room.IsHell)
                        this.transform.Find("Canvas").Find("Hell").gameObject.SetActive(false);
                    if (this.room.isImpostorRoom)
                    {
                        this.transform.Find("Information_Speciality").gameObject.SetActive(false);
                        this.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 0 / 255f, 0 / 255f);
                    }
                }
                if (this.room.isTrial)
                {
                    this.transform.Find("Information_Speciality").gameObject.SetActive(false);
                    this.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);

                }
                else if (this.room.isTeamTrial)
                {
                    this.transform.Find("Information_Speciality").gameObject.SetActive(false);
                    this.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 135 / 255f, 0 / 255f);
                }
                else if(this.room.isSpecial && !this.room.isImpostorRoom)
                {
                    this.transform.Find("Information_Speciality").gameObject.SetActive(false);
                    this.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
                }
              

                if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                {
                    if (this.room.isNewParadise && !this.room.isHide)
                    {
                        this.transform.Find("Information_Speciality").gameObject.SetActive(false);
                    }
                        
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnClickToLight();
        }

    }

    public void OnClickToLight()
    {
        if (this.room.IsObstacle)
            return;
        if (this.isLightByOther)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lightHexagoneIsOn && !isLighted)
            return;
        isLighted = !this.transform.Find("Light").gameObject.activeSelf;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lightHexagoneIsOn = isLighted;
        this.transform.Find("Light").gameObject.SetActive(isLighted);
        gameManager.gameManagerNetwork.SendLightHexagone(this.room.Index, isLighted);
    }


    public void SetLight(bool active)
    {
        this.transform.Find("Light").gameObject.SetActive(active);
        isLightByOther = active;
    }

    void OnMouseExit()
    {

#if UNITY_IOS || UNITY_ANDROID
        return;
#endif

        if ( !this.room.IsObstacle && (this.room.isSpecial || this.room.IsExit || this.room.IsHell || this.room.isImpostorRoom || this.room.isNewParadise) && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hideImpostorInformation)
        {
            if (gameManager.ui_Manager.map.activeSelf)
            {

                this.transform.Find("Canvas").Find("ImpostorPower").gameObject.SetActive(true);
                //this.transform.Find("Canvas").Find("Distance_text").gameObject.SetActive(false);
                if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                {
                    if (this.room.IsExit)
                        this.transform.Find("Canvas").Find("Paradise_door").gameObject.SetActive(true);
                    if (this.room.IsHell)
                        this.transform.Find("Canvas").Find("Hell").gameObject.SetActive(true);
                    if (this.room.isImpostorRoom)
                        this.transform.Find("Information_Speciality").gameObject.SetActive(true);
                   
                }
                if (this.room.isSpecial)
                    this.transform.Find("Information_Speciality").gameObject.SetActive(true);

                if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
                {
                    if (this.room.isNewParadise && !this.room.isHide)
                        this.transform.Find("Information_Speciality").gameObject.SetActive(true);
                }

            }
          
        }
    }

    public void DiplayInformationSpeciallyRoom(bool display)
    {

        if (!room)
            return;
        if (!gameManager)
            return;
        
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            if (room.IsExit)
                return;
        }
        else
        {
            if (room.IsExit && !room.isNewParadise)
                return;
        }
        if ( room.IsHell || room.IsObstacle || room.IsInitiale)
            return;

        if (this.room.isHide)
        {
            GameObject Information_Speciality2 = this.transform.Find("Information_Speciality").gameObject;
            Information_Speciality2.SetActive(false);
            return;
        }
        //Debug.LogError("specillay room " + display + " " +room.Index);
            
        GameObject Information_Speciality = this.transform.Find("Information_Speciality").gameObject;
        Information_Speciality.SetActive(display);

        if (!display)
            return;

        if (!this.room.isSpecial && !this.room.isImpostorRoom && !this.room.isNewParadise)
        {
            Information_Speciality.SetActive(false);
        }
            
        if (this.room.isSpecial && !this.room.isTrial && !this.room.isTeamTrial)
        {
            Information_Speciality.transform.Find("Hexagone").Find("SpeciallyRoom").gameObject.SetActive(display);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
            Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
        } 
        if (this.room.isTrial && !(this.room.isDeathNPC || this.room.isMonsters))
        {
            Information_Speciality.transform.Find("Hexagone").Find("TrailRoom").gameObject.SetActive(display);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);
            Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);
        }
        if (this.room.isImpostorRoom)
        {
           
            Information_Speciality.transform.Find("Hexagone").Find("ImpostorRoom").gameObject.SetActive(display);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 0 / 255f, 0 / 255f);
            Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 0 / 255f, 0 / 255f);
        }
        if(this.room.isTeamTrial)
        {
            Information_Speciality.transform.Find("Hexagone").Find("TeamTrailRoom").gameObject.SetActive(display);
            Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 135 / 255f, 0 / 255f);
            Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 135 / 255f, 0 / 255f);
        }
        if (this.room.isNewParadise)
        {
            switch (this.room.isOldSpeciality)
            {
                case 1:
                    Information_Speciality.transform.Find("Hexagone").Find("SpeciallyRoom").gameObject.SetActive(display);
                    Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
                    Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(58 / 255f, 187 / 255f, 241 / 255f);
                    break;
                case 2:
                    Information_Speciality.transform.Find("Hexagone").Find("TrailRoom").gameObject.SetActive(display);
                    Information_Speciality.transform.Find("Hexagone").GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);
                    Information_Speciality.transform.parent.GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 215 / 255f, 0 / 255f);
                    break;
            }
        } 
    }
}
