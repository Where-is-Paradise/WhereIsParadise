using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BackWaitingRoom : MonoBehaviour
{
    public string codeRoom;
    public bool isBackToWaitingRoom;
    public string playerName;
    public GameManager gameManager;
    public bool isMatchmaking = false;
    public int indexSkin;
    public int indexSkinColor;
    public string indexMasterClient = "-1";

    // Start is called before the first frame update
    void Start()
    {
        codeRoom = GameObject.Find("Setting").GetComponent<Setting>().codeRoom;
        isMatchmaking = GameObject.Find("Setting").GetComponent<Setting>().isMatchmaking ;
        isBackToWaitingRoom = true;
        DontDestroyOnLoad(this.gameObject);

        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            playerName = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().playerName;
        }
        indexSkin = GameObject.Find("Setting").GetComponent<Setting>().INDEX_SKIN;
        indexSkinColor = GameObject.Find("Setting").GetComponent<Setting>().INDEX_SKIN_COLOR;
        if (PhotonNetwork.IsMasterClient)
            indexMasterClient = PhotonNetwork.LocalPlayer.UserId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
