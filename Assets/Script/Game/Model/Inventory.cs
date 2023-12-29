using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

    public int priceMoney = 0;

    public string orderId = "";

    // Start is called before the first frame update
    void Start()
    {
        lobby = GameObject.Find("Lobby_Manager").GetComponent<Lobby>();
        //lobby.GetPlayerMineGO().GetComponent<PlayerGO>().ManageInventoryTest();
        AddSkinInventory();
        DisplayNextButton();
        UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicrotransactionResponse);

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
        int priceSkin = GetPriceOfSkin(current_indexItemBuy);
        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money < priceSkin)
        {
            panelNoMoreMoney.SetActive(true);
            return;
        }
        StartCoroutine(AddSkinRequest(current_indexItemBuy, priceSkin));
       
    }

    public void UpdateValeuPlayerMoneyUI(int newValue)
    {
        UI_moneyPlayer.transform.Find("Text").GetComponent<Text>().text = newValue + "";
    }

    public IEnumerator AddSkinRequest(int idSkin, int price)
    {
        string steamId = SteamUser.GetSteamID().ToString();
        string idSkinString = idSkin + "";
        string nameString = GetNameOfSkin(idSkin);
        string money = price +" ";
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8090/player/addSkin?steamId=" + steamId + "&idSkin=" + idSkinString + "&nameSkin=" + nameString +"&money="+ money, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (ParserJson.ParseStringToJson(www.downloadHandler.text, "code") == "407")
            {
                panelNoMoreMoney.SetActive(true);
            }
            else
            {
                lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money -= price;
                UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
                lobby.GetPlayerMineGO().GetComponent<PlayerGO>().AddInInventory(idSkin);
                AddOneSkinInventory(current_indexItemBuy);
                ResetCurrentIndexItem();
                DisplayNextButton();
            }
        }
    }

    public int GetPriceOfSkin(int indexSkin)
    {
        for(int i =0; i < parentAllSkin.transform.childCount; i++)
        {
            if (parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().id == indexSkin)
                return parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().price;
        }
        return 200;
    }
    public string GetNameOfSkin(int indexSkin)
    {
        for (int i = 0; i < parentAllSkin.transform.childCount; i++)
        {
            if (parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().id == indexSkin)
                return parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().nameProduct;
        }
        return "inconnue..";
    }


    public void BuyMoney(int indexMoney)
    {
        StartCoroutine(TestPurchasing(indexMoney));
    }

    public IEnumerator TestPurchasing(int indexMoney)
    {

                this.priceMoney = 550;
                
        /*WWWForm form = new WWWForm();
                form.AddField("key", "110CECAF8B4523084D352599DD2EFFA2");
                form.AddField("orderid", 1035);
                form.AddField("steamid", "" + SteamUser.GetSteamID().m_SteamID);
                form.AddField("appid", 1746620);
                form.AddField("itemcount", 1);
                form.AddField("language", "en");
                form.AddField("currency", "EUR");
                form.AddField("itemid[0]", 10);
                form.AddField("qty[0]", 1);
                form.AddField("amount[0]", price);
                form.AddField("description[0]", "description");


                UnityWebRequest www = UnityWebRequest.Post("https://partner.steam-api.com/ISteamMicroTxnSandbox/InitTxn/v3", form);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.downloadHandler.text);
                    //Debug.Log(form);
                    //StartCoroutine(GetText());
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);

                    //SteamAPI.RunCallbacks();
                    Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicrotransactionResponse);

                }*/

        WWWForm form = new WWWForm();
        string steamId = SteamUser.GetSteamID().ToString();
        Debug.Log("indexMoney : " + indexMoney);
        UnityWebRequest requestinit = UnityWebRequest.Post("http://127.0.0.1:8090/paiement/init?steamId=" + steamId+"&idMoney="+ indexMoney, form);
        yield return requestinit.SendWebRequest();

        if (requestinit.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(requestinit.downloadHandler.text);
        }
        else
        {
            Debug.Log(requestinit.downloadHandler.text);
            string orderIdReturnJson = ParserJson.ParseStringToJsonWithoutCote(requestinit.downloadHandler.text, "orderId");
            this.orderId = orderIdReturnJson;
        }


    }

    public void OnMicrotransactionResponse(MicroTxnAuthorizationResponse_t pCallback)
    {
        if (pCallback.m_bAuthorized == 1)
        {
            StartCoroutine(FinaliseTransaction());
        }
        else
        {
            Debug.Log("c pas payé sale radin, ou sale pauvre " + pCallback.m_bAuthorized);
        }

    }

    public IEnumerator FinaliseTransaction()
    {
        /*        WWWForm form = new WWWForm();
                form.AddField("key", "110CECAF8B4523084D352599DD2EFFA2");
                form.AddField("orderid", 1035);
                form.AddField("steamid", "" + SteamUser.GetSteamID().m_SteamID);
                form.AddField("appid", 1746620);

                UnityWebRequest www = UnityWebRequest.Post("https://partner.steam-api.com/ISteamMicroTxnSandbox/FinalizeTxn/v2", form);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.downloadHandler.text);
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    string steamId = SteamUser.GetSteamID().ToString();
                    UnityWebRequest requestAddMoney = UnityWebRequest.Post("http://127.0.0.1:8090/player/addMoney?steamId=" + steamId + "&money=" + this.priceMoney, form);
                    yield return requestAddMoney.SendWebRequest();
                    lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money = lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money + this.priceMoney;
                    UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
                }*/

        WWWForm form = new WWWForm();
        string steamId = SteamUser.GetSteamID().ToString();
        UnityWebRequest requestFinalise = UnityWebRequest.Post("http://127.0.0.1:8090/paiement/finalize?steamId=" + steamId + "&orderId=" + this.orderId, form);
        yield return requestFinalise.SendWebRequest();

        if (requestFinalise.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(requestFinalise.downloadHandler.text);
        }
        else
        {
            Debug.Log(requestFinalise.downloadHandler.text);
            /*            UnityWebRequest requestAddMoney = UnityWebRequest.Post("http://127.0.0.1:8090/player/addMoney?steamId=" + steamId + "&money=" + this.priceMoney, form);
                        yield return requestAddMoney.SendWebRequest();
                        lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money = lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money + this.priceMoney;
                        UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);*/


            string blackSoulMoneyReturn = ParserJson.ParseStringToJson(requestFinalise.downloadHandler.text, "valueMoney");
            int blackSoulMoneyInt = int.Parse(blackSoulMoneyReturn);
            lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money = lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money + blackSoulMoneyInt;
            UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        }



    }
}
