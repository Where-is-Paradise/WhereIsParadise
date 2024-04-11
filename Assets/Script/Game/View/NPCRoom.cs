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
    public int indexEvilNPC = 0;
    public int indexEvilNPC_2 = 0;
    public string baseText = "Vous devez prendre la porte ";
    public bool powerIsUsed = false;
    public string door = "";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LaunchNPCRoom()
    {


    }

    public void SendRandomNpc()
    {
        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            return;

        int random = Random.Range(0, 3);
        indexEvilNPC = random;
 
        gameManager.gameManagerNetwork.SendEvilInNPCRoom(gameManager.game.currentRoom.Index, indexEvilNPC, 2);
    }


    public void SendDisplayDistanceByNpc(int indexNPC)
    {
        if (!gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            //SendRandomNpc();
            float randomInt = Random.Range(0, 100);
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
        indexEvilNPC = gameManager.game.currentRoom.indexEvilNPC;
        indexEvilNPC_2 = gameManager.game.currentRoom.indexEvilNPC_2;
        //DisplayDistanceByNpc(indexNPC, indexEvilNPC, indexEvilNPC_2, gameManager.game.currentRoom.doorNameShorterNPC, gameManager.game.currentRoom.doorNameLongerNPC, gameManager.game.currentRoom.randomIntEvil);
        photonView.RPC("DisplayDistanceByNpc", RpcTarget.All, indexNPC, indexEvilNPC, indexEvilNPC_2, gameManager.game.currentRoom.doorNameShorterNPC, gameManager.game.currentRoom.doorNameLongerNPC, gameManager.game.currentRoom.randomIntEvil);
    }

    [PunRPC]
    public void DisplayDistanceByNpc(int indexNPC, int indexEvilNpc, int indexEvilNpc2, string doorNameShorter, string doorNameLonger, float randomInt)
    {
        if (indexNPC == 0)
        {
            if (indexEvilNpc == 0)
            {
                this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                if (randomInt < 60){
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
                if (indexEvilNpc2 == 0)
                {
                    this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                       baseText + doorNameLonger;
                    door = doorNameLonger;
                }
                else
                {
                    this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                        baseText + doorNameShorter;
                    door = doorNameShorter;
                }

            }
            this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(true);
            this.transform.Find("NPCRight").gameObject.SetActive(false);
            this.transform.Find("NPCMiddle").gameObject.SetActive(false);
        }
        else
        {

            if(indexNPC  == 1)
            {
                if (indexEvilNpc == 1)
                {
                    this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(true);
                    if (randomInt < 60)
                    {
                        this.transform.Find("NPCMiddle").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                            baseText + doorNameLonger;
                        door = doorNameLonger;
                    }
                    else
                    {
                        this.transform.Find("NPCMiddle").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                            baseText + doorNameShorter;
                        door = doorNameShorter;
                    }

                }
                else
                {
                    if (indexEvilNpc2 == 1)
                    {
                        this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(true);
                        this.transform.Find("NPCMiddle").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                           baseText + doorNameLonger;
                        door = doorNameLonger;
                    }
                    else
                    {
                        this.transform.Find("NPCMiddle").Find("Angel").gameObject.SetActive(true);
                        this.transform.Find("NPCMiddle").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                            baseText + doorNameShorter;
                        door = doorNameShorter;
                    }

                }
                this.transform.Find("NPCMiddle").Find("SquareMessage").gameObject.SetActive(true);
                this.transform.Find("NPCRight").gameObject.SetActive(false);
                this.transform.Find("NPCLeft").gameObject.SetActive(false);
            }
            else
            {
                if (indexEvilNpc == 2)
                {
                    this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                    if (randomInt < 60)
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
                else
                {
                    if (indexEvilNpc2 == 2)
                    {
                        this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                        this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                           baseText + doorNameLonger;
                        door = doorNameLonger;
                    }
                    else
                    {
                        this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                        this.transform.Find("NPCRight").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text =
                            baseText + doorNameShorter;
                        door = doorNameShorter;
                    }

                }
                this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(true);
                this.transform.Find("NPCLeft").gameObject.SetActive(false);
                this.transform.Find("NPCMiddle").gameObject.SetActive(false);
            }
           
           
        }
        //photonView.RPC("SendDisplayResult", RpcTarget.Others, indexNPC, evilIsleft, door);
        StartCoroutine(CoroutineHideMessage());
        gameManager.gameManagerNetwork.SendDoorInNPCRoom(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Index, door);
        gameManager.gameManagerNetwork.SendNpcChooseLeft(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Index, indexNPC);
        gameManager.gameManagerNetwork.SendNpcDoorShorterAndLonger(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Index, doorNameShorter, doorNameLonger);
        powerIsUsed = true;

        gameManager.gameManagerNetwork.SendPowerIsUsed(gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.Index, true);

        HideNormalNPC();
    }

    [PunRPC]
    public void SendDisplayResult(int indexNpcChosen, bool evilIsleft, string doorName)
    {
        if (indexNpcChosen == 0)
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
            if(indexNpcChosen == 1)
            {

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

            
        }
        StartCoroutine(CoroutineHideMessage());
    }
    public void ActivateRoom()
    {

        this.transform.Find("NPCLeft").gameObject.SetActive(false);
        this.transform.Find("NPCRight").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").gameObject.SetActive(false);

        this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("Angel").gameObject.SetActive(false);

        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
        {
            if (gameManager.game.currentRoom.npcChooseIndex == 0)
            {
                this.transform.Find("NPCLeft").gameObject.SetActive(true);
                if (gameManager.game.currentRoom.indexEvilNPC == 0 || gameManager.game.currentRoom.indexEvilNPC_2 == 0)
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
                if (gameManager.game.currentRoom.npcChooseIndex == 1)
                {
                    this.transform.Find("NPCMiddle").gameObject.SetActive(true);
                    if (gameManager.game.currentRoom.indexEvilNPC == 1 || gameManager.game.currentRoom.indexEvilNPC_2 == 1)
                    {
                        this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(true);
                    }
                    else
                    {
                        this.transform.Find("NPCMiddle").Find("Angel").gameObject.SetActive(true);
                    }

                }
                else
                {
                    this.transform.Find("NPCRight").gameObject.SetActive(true);
                    if (gameManager.game.currentRoom.indexEvilNPC == 2 || gameManager.game.currentRoom.indexEvilNPC_2 == 2)
                    {
                        this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                    }
                    else
                    {
                        this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);
                    }
                }

            }
        }
        else
        {
            this.transform.Find("NPCLeft").gameObject.SetActive(true);
            this.transform.Find("NPCRight").gameObject.SetActive(true);
            this.transform.Find("NPCMiddle").gameObject.SetActive(true);

            if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().hasTrueEyes)
            {
                if ((gameManager.game.currentRoom.indexEvilNPC == 0 && gameManager.game.currentRoom.indexEvilNPC_2 == 1 ) 
                    || gameManager.game.currentRoom.indexEvilNPC == 1 && gameManager.game.currentRoom.indexEvilNPC_2 == 0)

                {
/*                    this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Angel").gameObject.SetActive(true);*/

                    this.transform.Find("NPCLeft").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Transparency").Find("Angel").gameObject.SetActive(true);
                }
                else if((gameManager.game.currentRoom.indexEvilNPC == 0 && gameManager.game.currentRoom.indexEvilNPC_2 == 2) 
                    || gameManager.game.currentRoom.indexEvilNPC == 2 && gameManager.game.currentRoom.indexEvilNPC_2 == 0)
                {
/*                    this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("Evil").gameObject.SetActive(true);*/

                    this.transform.Find("NPCRight").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Transparency").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("Transparency").Find("Evil").gameObject.SetActive(true);

                }
                else if (gameManager.game.currentRoom.indexEvilNPC == 1 && gameManager.game.currentRoom.indexEvilNPC_2 == 2 
                    || gameManager.game.currentRoom.indexEvilNPC == 2 && gameManager.game.currentRoom.indexEvilNPC_2 == 1)
                {
/*                    this.transform.Find("NPCLeft").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Evil").gameObject.SetActive(true);*/

                    this.transform.Find("NPCLeft").Find("Transparency").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Transparency").Find("Evil").gameObject.SetActive(true);
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
        this.transform.Find("NPCMiddle").Find("SquareMessage").gameObject.SetActive(false);
    }
    public IEnumerator CoroutineHideMessage()
    {
        yield return new WaitForSeconds(5);
        this.transform.Find("NPCRight").Find("SquareMessage").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("SquareMessage").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("SquareMessage").gameObject.SetActive(false);
    }

    public void HideNormalNPC()
    {
        this.transform.Find("NPCRight").Find("img").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("img").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("img").gameObject.SetActive(false);
    }


}
