using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpostorRoom : TrialsRoom
{
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        AssignRandomObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignRandomObject()
    {

        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
            return;

        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasImpostorObject)
            return;
        
        float randomfloat = Random.Range(0, 100);
        if (randomfloat < 30)
        {
            this.transform.Find("potion").gameObject.SetActive(true);
        }
        else if (randomfloat < 60)
        {
            this.transform.Find("book").gameObject.SetActive(true);
        }
        else if (randomfloat < 90)
        {
            this.transform.Find("key").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("knife").gameObject.SetActive(true);
        }
    }

    public void CollisionObject(string nameObject)
    {
        switch (nameObject)
        {
            case "potion":
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManager.listIndexImpostorObject[0]);
                gameManager.ui_Manager.DisplayInformationObjectWon(5);
                this.transform.Find("potion").gameObject.SetActive(false);
                break;
            case "book":
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManager.listIndexImpostorObject[2]);
                gameManager.ui_Manager.DisplayInformationObjectWon(6);
                this.transform.Find("book").gameObject.SetActive(false);

                break;
            case "knife":
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManager.listIndexImpostorObject[1]);
                gameManager.ui_Manager.DisplayInformationObjectWon(7);
                this.transform.Find("knife").gameObject.SetActive(false);
                break;
            case "key":
                gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendIndexObjectPower(gameManager.listIndexImpostorObject[3]);
                gameManager.ui_Manager.DisplayInformationObjectWon(8);
                this.transform.Find("key").gameObject.SetActive(false);
                break;

        }
        gameManager.ui_Manager.DisplayObjectPowerImpostorInGame();
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasImpostorObject = true;
    }
}
