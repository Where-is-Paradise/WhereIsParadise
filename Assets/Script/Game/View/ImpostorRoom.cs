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


        //int numberObject = GetNumberOfObjectInSetting();
      


       
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        if (playerList.Length > 4)
        {
           
            float randomLeft = 0;
            float randomRight = 100;
            if (!gameManager.setting.listObjectImpostor[0])
            {
                randomLeft += (float)(100 / 3f);
            }
            if (!gameManager.setting.listObjectImpostor[1])
            {
                randomRight -= (float)(100 / 3f);
            }
            if (!gameManager.setting.listObjectImpostor[2])
            {
                randomRight -= (float)(100 / 3f);
            }

            float randomfloat = Random.Range(randomLeft, randomRight);

            if (randomfloat < 33f && gameManager.setting.listObjectImpostor[0])
            {
                this.transform.Find("potion").gameObject.SetActive(true);
            }
            else if (randomfloat < 66f && gameManager.setting.listObjectImpostor[1])
            {
                this.transform.Find("book").gameObject.SetActive(true);
            }
            else if (randomfloat < 100 && gameManager.setting.listObjectImpostor[2])
            {
                this.transform.Find("key").gameObject.SetActive(true);
            }
            else
            {
                //if(gameManager.setting.listObjectImpostor[3])
                this.transform.Find("knife").gameObject.SetActive(true);
            }
        }
        else
        {
            float randomLeft = 0;
            float randomRight = 100;
            if (!gameManager.setting.listObjectImpostor[0])
            {
                randomLeft += 15;
            }
            if (!gameManager.setting.listObjectImpostor[1])
            {
                randomRight -= 75;
            }
            if (!gameManager.setting.listObjectImpostor[2])
            {
                randomRight -= 10;
            }

            float randomfloat = Random.Range(randomLeft, randomRight);
            Debug.Log(randomLeft + " / " + randomRight);
            Debug.Log(randomfloat);
            if (randomfloat < 15f && gameManager.setting.listObjectImpostor[0])
            {
                this.transform.Find("potion").gameObject.SetActive(true);
            }
            else if (randomfloat < 90f && gameManager.setting.listObjectImpostor[1])
            {
                this.transform.Find("book").gameObject.SetActive(true);
            }
            else if (randomfloat < 100 && gameManager.setting.listObjectImpostor[2])
            {
                this.transform.Find("key").gameObject.SetActive(true);
            }
            else
            {
                //if(gameManager.setting.listObjectImpostor[3])
                this.transform.Find("knife").gameObject.SetActive(true);
            }
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

    public int GetNumberOfObjectInSetting()
    {
        int counter = 0;
        for(int i = 0; i< gameManager.setting.listObjectImpostor.Count; i++)
        {
            if (gameManager.setting.listObjectImpostor[i])
                counter++;
        }
        return counter;
        
    }
}
