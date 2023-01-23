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


    string informationOne = "is not a lost soul";
    string informationTwo = "is a lost soul";
    string informationThree = "is not a corrupted soul";
    string informationFour = "is a corrupted soul";
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
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        RandomTrue();
        GenerateAndSetMessage();
    }

    public void RandomTrue()
    {
        int randomInt = Random.Range(0, 10);
        if (randomInt >= 5)
            isTrue = false;
        else
            isTrue = true;
    }

    public void ActiveMessage()
    {
        this.transform.Find("NPC").Find("SquareMessage").gameObject.SetActive(true);
    }
    public IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(5);
        this.transform.Find("NPC").Find("SquareMessage").gameObject.SetActive(false);
        gameManager.NPCIsUsed = true;
        gameManager.CloseDoorWhenVote(false);
        
    }

    public void GenerateAndSetMessage()
    {
        string randomNamePlayer = ChooseRandomPlayer();
        string randomInformation = ChooseInformation();
        string message = "";
        if(randomNamePlayer.Length > 17 )
            message = randomNamePlayer.Substring(0,17) + ".. " + randomInformation;
        else
            message = randomNamePlayer + " " + randomInformation;
        photonView.RPC("SendMessageInformation", RpcTarget.All, message);
    }
    
    [PunRPC]
    public void SendMessageInformation(string message)
    {
        this.transform.Find("NPC").Find("SquareMessage").Find("Canvas").Find("Text").GetComponent<Text>().text = message;
        ActiveMessage();
        StartCoroutine(HideMessage());
    }

    public string ChooseRandomPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int indexPlayer = Random.Range(0, players.Length);
        playerChooseIsImpostor = players[indexPlayer].GetComponent<PlayerGO>().isImpostor;
        return players[indexPlayer].GetComponent<PlayerGO>().playerName;
    }

    public string ChooseInformation()
    {

        Traduction();
        List<string> potentialString = new List<string>();

        if (isTrue && playerChooseIsImpostor) 
        {
            potentialString.Add(informationFour);
            potentialString.Add(informationOne);
        }
        if (isTrue && !playerChooseIsImpostor)
        {
            potentialString.Add(informationTwo);
            potentialString.Add(informationThree);
        }
        if (!isTrue && playerChooseIsImpostor)
        {
            potentialString.Add(informationTwo);
            potentialString.Add(informationThree);
        }
        if (!isTrue && !playerChooseIsImpostor)
        {
            potentialString.Add(informationOne);
            potentialString.Add(informationFour);
        }

        int randomIndex = Random.Range(0, potentialString.Count);

        return potentialString[randomIndex];
    }


    public void Traduction()
    {
        Debug.LogError(gameManager.setting.langage);
        if(gameManager.setting.langage == "fr")
        {
            informationOne = "n'est pas une âme égarée";
            informationTwo = "est une âme égarée";
            informationThree = "n'est pas une âme corrompue";
            informationFour = "est une âme corrompue";
        }
    }
}
