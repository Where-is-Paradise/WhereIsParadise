using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public GameObject parentAllSkin;
    public Lobby lobby;
    public GameObject dynamicPanelInventory;
    public GameObject dynamicPanelInventory2;
    private int maxElementInPanel = 9;
    public GameObject UI_moneyPlayer;
    public int current_indexItemBuy = -1;

    public GameObject panelNoMoreMoney;
    public GameObject panelAlreadyHave;

    // Start is called before the first frame update
    void Start()
    {
        lobby = GameObject.Find("Lobby_Manager").GetComponent<Lobby>();
        //lobby.GetPlayerMineGO().GetComponent<PlayerGO>().ManageInventoryTest();
        AddSkinInventory();
        DisplayNextButton();
        UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddSkinInventory()
    {
        int counterElement = 0;
        foreach(int index in lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory)
        {
            for(int i=0; i < parentAllSkin.transform.childCount; i++)
            {
                if(i == index)
                {
                    if(counterElement < maxElementInPanel)
                    {
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + index).gameObject);
                        newPanel.transform.parent = dynamicPanelInventory.transform;
                        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                    else
                    {
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + index).gameObject);
                        newPanel.transform.parent = dynamicPanelInventory2.transform;
                        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                    counterElement++;
                }
                
            }
        }
       
    }

    public void AddOneSkinInventory(int indexSkin)
    {
        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + indexSkin).gameObject);

        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory.Count <= maxElementInPanel)
            newPanel.transform.parent = dynamicPanelInventory.transform;
        else
            newPanel.transform.parent = dynamicPanelInventory2.transform;
        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
    }

    public void DisplayNextButton()
    {
        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory.Count > maxElementInPanel)
        {
            dynamicPanelInventory.transform.Find("Right button").gameObject.SetActive(true);
        }
    }

    public void ReserveItem(int index)
    {
        current_indexItemBuy = index;
    }
    public void ResetCurrentIndexItem()
    {
        current_indexItemBuy = -1;
    }

    public void BuyItem()
    {
        if (current_indexItemBuy == -1)
            return;
        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory.Contains(current_indexItemBuy))
        {
            panelAlreadyHave.SetActive(true);
            return;

        }
        // requete pour connaitre le prix
        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money < 250)
        {
            panelNoMoreMoney.SetActive(true);
            return;
        }
       
        lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money -= 250;
        UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        lobby.GetPlayerMineGO().GetComponent<PlayerGO>().AddInInventory(current_indexItemBuy);
        AddOneSkinInventory(current_indexItemBuy);
        DisplayNextButton();
        ResetCurrentIndexItem();
    }

    public void UpdateValeuPlayerMoneyUI(int newValue)
    {
        UI_moneyPlayer.transform.Find("Text").GetComponent<Text>().text = newValue + "";
    }

}
