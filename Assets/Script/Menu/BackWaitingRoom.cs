using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWaitingRoom : MonoBehaviour
{
    public string codeRoom;
    public bool isBackToWaitingRoom;
    public string playerName;
    public GameManager gameManager;
    public bool isMatchmaking = false;
    public int indexSkin;
    public int indexSkinColor;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
