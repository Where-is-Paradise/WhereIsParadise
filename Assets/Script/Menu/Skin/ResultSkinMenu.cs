using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSkinMenu : MonoBehaviour
{
    public Lobby lobby;
    public int indexSkin = 0;
    public int indexSkinColor = 0;
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
        this.transform.Find("ApplyButton").GetComponent<Button>().interactable = false;
        lobby.setting.INDEX_SKIN = indexSkin;
        lobby.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendindexSkinColor(0,true);
        lobby.setting.INDEX_SKIN_COLOR = 0;

    }

    public void OnClickChangeSkinColor()
    {
        if (!canPress)
            return;
        lobby.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendindexSkinColor(indexSkinColor, false);
        this.transform.Find("ApplyButton").GetComponent<Button>().interactable = false;
        lobby.setting.INDEX_SKIN_COLOR = indexSkinColor;
    }

    public void ChangeCanPress(bool canPress)
    {
        this.canPress = canPress;
        this.transform.Find("ApplyButton").GetComponent<Button>().interactable = canPress;
    }

    public void ChangeCanPressColor(bool canPress)
    {
        this.canPress = canPress;
        this.transform.Find("ApplyButtonColor").GetComponent<Button>().interactable = canPress;
    }


}
