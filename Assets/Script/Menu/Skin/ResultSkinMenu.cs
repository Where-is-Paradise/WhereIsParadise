using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSkinMenu : MonoBehaviour
{
    public Lobby lobby;
    public int indexSkin = 0;
    public bool canPress = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickChangeSkin()
    {
        if (!canPress)
            return;
        lobby.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendindexSkin(indexSkin);  
    }

    public void ChangeCanPress(bool canPress)
    {
        this.canPress = canPress;
        this.transform.Find("ApplyButton").GetComponent<Button>().interactable = false;
    }


}
