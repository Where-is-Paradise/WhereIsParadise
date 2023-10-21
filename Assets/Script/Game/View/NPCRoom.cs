using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NPCRoom : MonoBehaviourPun
{
    public bool isTrue = false;
    public bool playerChooseIsImpostor = false;
    public GameManager gameManager;
    public bool evilIsleft = false;
    public string baseText = "Vous devez prendre la porte ";
    public bool powerIsUsed = false;
    public string door = "";

    // Start is called before the first frame update
    void Start()
    {
/*        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        SendRandomNpc();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LaunchNPCRoom()
    {

        //GenerateAndSetMessage();



    }

    public void SendRandomNpc()
    {
        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            return;

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            evilIsleft = true;
        }
        else
        {
            evilIsleft = false;
        }
 
        gameManager.gameManagerNetwork.SendEvilInNPCRoom(gameManager.game.currentRoom.Index, evilIsleft);
    }


    public void SendDisplayDistanceByNpc(bool isLeft)
    {
        if (!gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            //SendRandomNpc();
            int randomInt = Random.Range(0, 2);
            gameManager.game.currentRoom.randomIntEvil = randomInt;
            string doorNameLonger = gameManager.GetRandomDoorLonger().doorName;
            gameManager.game.currentRoom.doorNameLongerNPC = doorNameLonger;
            string doorNameShorter = gameManager.GetDoorShorter().doorName;
            gameManager.game.currentRoom.doorNameShorterNPC = doorNameShorter;
        } 
        else
        {
            door = gameManager.game.currentRoom.doorInNpc;
           
        }
        evilIsleft = gameManager.game.currentRoom.evilIsLeft;
        DisplayDistanceByNpc(isLeft,evilIsleft, gameManager.game.currentRoom.doorNameShorterNPC, gameManager.game.currentRoom.doorNameLongerNPC, gameManager.game.currentRoom.randomIntEvil);
        //photonView.RPC("DisplayDistanceByNpc", RpcTarget.All, isLeft, evilIsleft, gameManager.game.currentRoom.doorNameShorterNPC, gameManager.game.currentRoom.doorNameLongerNPC, gameManager.game.currentRoom.randomIntEvil);
    }

    public void DisplayDistanceByNpc(bool leftIsChoose, bool evilIsleft,string doorNameShorter, string doorNameLonger, int randomInt)
    {
        if (leftIsChoose)
        {
            if (evilIsleft)
            {
                this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                if (randomInt == 0){
                    this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                        baseText + doorNameLonger;
                    door = doorNameLonger;
                }
                else
                {
                    this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                        baseText + doorNameShorter;
                    door = doorNameShorter;
                }

            }
            else
            {
                this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
               baseText + doorNameShorter;
                door = doorNameShorter;

            }
            this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(true);
            this.transform.Find("NPCRight").gameObject.SetActive(false);

        }
        else
        {
            if (evilIsleft)
            {
                this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                    baseText + doorNameShorter;
                door = doorNameShorter;
            }
            else
            {
                this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);

                if (randomInt == 0)
                {
                    this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                        baseText + doorNameLonger;
                    door = doorNameLonger;
                }
                else
                {
                    this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                        baseText + doorNameShorter;
                    door = doorNameShorter;
                }
            }
            this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(true);
            this.transform.Find("NPCLeft").gameObject.SetActive(false);
           
        }
        photonView.RPC("SendDisplayResult", RpcTarget.Others, leftIsChoose, evilIsleft, door);
        StartCoroutine(CoroutineHideMessage());
        gameManager.gameManagerNetwork.SendDoorInNPCRoom(gameManager.game.currentRoom.Index, door);
        gameManager.gameManagerNetwork.SendNpcChooseLeft(gameManager.game.currentRoom.Index, leftIsChoose);
        gameManager.gameManagerNetwork.SendNpcDoorShorterAndLonger(gameManager.game.currentRoom.Index, doorNameShorter, doorNameLonger);
        powerIsUsed = true;

        gameManager.gameManagerNetwork.SendPowerIsUsed(gameManager.game.currentRoom.Index, true);
    }

    [PunRPC]
    public void SendDisplayResult(bool leftIsChoose, bool evilIsleft, string doorName)
    {
        if (leftIsChoose)
        {
            if (evilIsleft)
            {
                this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                    baseText + doorName;
            }
            else
            {
                this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
               baseText + doorName;
            }
            this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(true);
            this.transform.Find("NPCRight").gameObject.SetActive(false);
        }
        else
        {
            if (evilIsleft)
            {
                this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                    baseText + doorName;
            }
            else
            {
                this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                    baseText + doorName;
            }
            this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(true);
            this.transform.Find("NPCLeft").gameObject.SetActive(false);
        }
        StartCoroutine(CoroutineHideMessage());
    }
    public void ActivateRoom()
    {

        this.transform.Find("NPCLeft").gameObject.SetActive(false);
        this.transform.Find("NPCRight").gameObject.SetActive(false);


        this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(false);

        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            if (gameManager.game.currentRoom.npcChooseIsLeft)
            {
                this.transform.Find("NPCLeft").gameObject.SetActive(true);
                if (gameManager.game.currentRoom.evilIsLeft)
                {
                    this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                }
                else
                {
                    this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                }
            }
            else
            {
                this.transform.Find("NPCRight").gameObject.SetActive(true);
                if (gameManager.game.currentRoom.evilIsLeft)
                {
                    this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                }
                else
                {
                    this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                }
            }
        }
        else
        {
            this.transform.Find("NPCLeft").gameObject.SetActive(true);
            this.transform.Find("NPCRight").gameObject.SetActive(true);

            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            {
                if (gameManager.game.currentRoom.evilIsLeft)
                {
                    this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                }
                else
                {
                    this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                }
            }
        }
        HideMessage();
    }
    public void DisplayImpostorInformation()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            return;

    }

    public void HideMessage()
    {
        this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(false);
    }
    public IEnumerator CoroutineHideMessage()
    {
        yield return new WaitForSeconds(5);
        this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(false);
    }


}
