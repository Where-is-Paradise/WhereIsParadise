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
    public string baseText = "";
    public bool powerIsUsed = false;
    public string door = "";

    public int counterNPCRight = 0;
    public int counterNPCMiddle = 0;
    public int counterNPCLeft= 0;

    public bool hasVoted = false;

    // Start is called before the first frame update
    void Start()
    {
        baseText = this.transform.Find("Canvas").Find("Text").GetComponent<Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Find("NPCRight").Find("Canvas").Find("nbVote").GetComponent<Text>().text = gameManager.game.currentRoom.nbvoteNPCRight + "";
        this.transform.Find("NPCMiddle").Find("Canvas").Find("nbVote").GetComponent<Text>().text = gameManager.game.currentRoom.nbvoteNPCMiddle + "";
        this.transform.Find("NPCLeft").Find("Canvas").Find("nbVote").GetComponent<Text>().text = gameManager.game.currentRoom.nbvoteNPCLeft + "";
        
    }

    public void SendRandomNpc()
    {
        if (gameManager.game.currentRoom.speciallyPowerIsUsed)
            return;

        int random = Random.Range(0, 3);
        indexEvilNPC = random;
 
        gameManager.gameManagerNetwork.SendEvilInNPCRoom(gameManager.game.currentRoom.Index, indexEvilNPC, 2);
    }


    public void SendDisplayDistanceByNpc(int indexNPC, int indexPlayer, bool justLocal, int indexRoom)
    {
        // faire en sorte qu'un seul joueur effectue ce code
        if (indexPlayer != gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID)
            return;

        if (!gameManager.game.dungeon.GetRoomByIndex(indexRoom).npcPowerIsUsed)
        {
            //SendRandomNpc();
            float randomInt = Random.Range(0, 100);
            gameManager.game.currentRoom.randomIntEvil = randomInt;
            string doorNameLonger = gameManager.GetRandomDoorLonger().doorName;
            gameManager.game.currentRoom.doorNameLongerNPC = doorNameLonger;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameLongerNPC = doorNameLonger;
            string doorNameShorter = gameManager.GetDoorShorter().doorName;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameShorterNPC = doorNameShorter;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).npcPowerIsUsed = true;
         
        } 
        else
        {
            door = gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorInNpc;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameLongerNPC = door;
            gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameShorterNPC = door;
        }
        indexEvilNPC = gameManager.game.dungeon.GetRoomByIndex(indexRoom).indexEvilNPC;
        indexEvilNPC_2 = gameManager.game.dungeon.GetRoomByIndex(indexRoom).indexEvilNPC_2;
       

        if (!justLocal)
            photonView.RPC("DisplayDistanceByNpc", RpcTarget.All, indexNPC, indexEvilNPC, indexEvilNPC_2, gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameShorterNPC, gameManager.game.dungeon.GetRoomByIndex(indexRoom).doorNameLongerNPC, gameManager.game.dungeon.GetRoomByIndex(indexRoom).randomIntEvil, indexRoom);
        else
            DisplayDistanceByNpc(indexNPC, indexEvilNPC, indexEvilNPC_2, gameManager.game.currentRoom.doorNameShorterNPC, gameManager.game.currentRoom.doorNameLongerNPC, gameManager.game.currentRoom.randomIntEvil, indexRoom);

    }

    [PunRPC]
    public void DisplayDistanceByNpc(int indexNPC, int indexEvilNpc, int indexEvilNpc2, string doorNameShorter, string doorNameLonger, float randomInt, int indexRoom)
    {
        if (gameManager.game.currentRoom.Index != indexRoom)
            return;

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
        StartCoroutine(CoroutineHideMessage());
        gameManager.gameManagerNetwork.SendDoorInNPCRoom(gameManager.game.dungeon.GetRoomByIndex(indexRoom).Index, door);
        gameManager.gameManagerNetwork.SendNpcChooseLeft(gameManager.game.dungeon.GetRoomByIndex(indexRoom).Index, indexNPC);
        gameManager.gameManagerNetwork.SendNpcDoorShorterAndLonger(gameManager.game.dungeon.GetRoomByIndex(indexRoom).Index, doorNameShorter, doorNameLonger);
       
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

        this.transform.Find("NPCLeft").Find("Transparency").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCLeft").Find("Transparency").Find("Angel").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Transparency").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCRight").Find("Transparency").Find("Angel").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("Transparency").Find("Evil").gameObject.SetActive(false);
        this.transform.Find("NPCMiddle").Find("Transparency").Find("Angel").gameObject.SetActive(false);

        if (gameManager.game.currentRoom.npcPowerIsUsed)
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
                    this.transform.Find("NPCLeft").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCRight").Find("Transparency").Find("Angel").gameObject.SetActive(true);
                }
                else if((gameManager.game.currentRoom.indexEvilNPC == 0 && gameManager.game.currentRoom.indexEvilNPC_2 == 2) 
                    || gameManager.game.currentRoom.indexEvilNPC == 2 && gameManager.game.currentRoom.indexEvilNPC_2 == 0)
                {

                    this.transform.Find("NPCRight").Find("Transparency").Find("Evil").gameObject.SetActive(true);
                    this.transform.Find("NPCMiddle").Find("Transparency").Find("Angel").gameObject.SetActive(true);
                    this.transform.Find("NPCLeft").Find("Transparency").Find("Evil").gameObject.SetActive(true);

                }
                else if (gameManager.game.currentRoom.indexEvilNPC == 1 && gameManager.game.currentRoom.indexEvilNPC_2 == 2 
                    || gameManager.game.currentRoom.indexEvilNPC == 2 && gameManager.game.currentRoom.indexEvilNPC_2 == 1)
                {
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

    public void DisplayNormalNPC()
    {
        this.transform.Find("NPCRight").Find("img").gameObject.SetActive(true);
        this.transform.Find("NPCLeft").Find("img").gameObject.SetActive(true);
        this.transform.Find("NPCMiddle").Find("img").gameObject.SetActive(true);
    }

    public void DisplayEvilAngelNpc()
    {
        this.transform.Find("NPCRight").gameObject.SetActive(true);
        this.transform.Find("NPCLeft").gameObject.SetActive(true);
        this.transform.Find("NPCMiddle").gameObject.SetActive(true);
    }

    public void VerifyNbVoteNPC()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int indexPlayerMine = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;

        if (gameManager.game.currentRoom.npcPowerIsUsed)
        {
           
            if (gameManager.game.currentRoom.nbvoteNPCRight >= (players.Length / 2f))
            {
                DisplayEvilAngelNpc();
                gameManager.game.currentRoom.npcPowerIsUsed = true;
                SendDisplayDistanceByNpc(2, indexPlayerMine, true, gameManager.game.currentRoom.Index);
            }
            if (gameManager.game.currentRoom.nbvoteNPCMiddle >= (players.Length / 2f))
            {
                DisplayEvilAngelNpc();
                gameManager.game.currentRoom.npcPowerIsUsed = true;
                SendDisplayDistanceByNpc(1, indexPlayerMine, true, gameManager.game.currentRoom.Index);
            }
            if (gameManager.game.currentRoom.nbvoteNPCLeft >= (players.Length / 2f))
            {
                DisplayEvilAngelNpc();
                gameManager.game.currentRoom.npcPowerIsUsed = true;
                SendDisplayDistanceByNpc(0, indexPlayerMine, true, gameManager.game.currentRoom.Index);
            }
            DisplayNbVoteAllNpc(false);
        }
        else
        {
            DisplayNbVoteAllNpc(true);
            DisplayNormalNPC();
        }
       

    }

    public void DisplayNbVoteAllNpc(bool display)
    {
        this.transform.Find("NPCRight").Find("Canvas").gameObject.SetActive(display);
        this.transform.Find("NPCMiddle").Find("Canvas").gameObject.SetActive(display);
        this.transform.Find("NPCLeft").Find("Canvas").gameObject.SetActive(display);
    }



}
