using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSkin : MonoBehaviour
{

    public GameObject result;
    public int indexSkin;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickButton(bool eye)
    {
        result.transform.Find("Image").GetComponent<Image>().sprite = this.transform.Find("Image").GetComponent<Image>().sprite;
        //result.transform.Find("Image").Find("eye").gameObject.SetActive(eye);
        if (indexSkin == result.GetComponent<ResultSkinMenu>().lobby.GetPlayerMineGO().GetComponent<PlayerGO>().indexSkin)
        {
            result.GetComponent<ResultSkinMenu>().ChangeCanPress(false);
            return;
        }            
        result.GetComponent<ResultSkinMenu>().ChangeCanPress(true);
        result.GetComponent<ResultSkinMenu>().indexSkin = indexSkin;


    }
    public void OnClickButtonColors(bool eye)
    {
        result.transform.Find("Image").GetComponent<Image>().sprite = this.transform.Find("Image").GetComponent<Image>().sprite;
        //result.transform.Find("Image").Find("eye").gameObject.SetActive(eye);
        if (indexSkin == result.GetComponent<ResultSkinMenu>().lobby.GetPlayerMineGO().GetComponent<PlayerGO>().indexSkinColor)
        {
            result.GetComponent<ResultSkinMenu>().ChangeCanPressColor(false);
            return;
        }
        result.GetComponent<ResultSkinMenu>().ChangeCanPressColor(true);
        result.GetComponent<ResultSkinMenu>().indexSkinColor = indexSkin;

/*        Lobby lobby = GameObject.Find("Lobby_Manager").GetComponent<Lobby>();

        if (indexSkin == 6 && lobby.GetPlayerMineGO().GetComponent<PlayerGO>().playerName == "Homertimes")
            indexSkin = 9;*/
    }



}
