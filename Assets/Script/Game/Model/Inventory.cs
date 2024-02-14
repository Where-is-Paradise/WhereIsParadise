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

    public string orderId = "";
    public string linkRequest;

    // Start is called before the first frame update
    void Start()
    {
        lobby = GameObject.Find("Lobby_Manager").GetComponent<Lobby>();
        //lobby.GetPlayerMineGO().GetComponent<PlayerGO>().ManageInventoryTest();
        AddSkinInventory();
        DisplayNextButton();
        UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicrotransactionResponse);

        linkRequest = lobby.setting.linkServerAws;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddSkinInventory()
    {
        if (!lobby.GetPlayerMineGO())
            StartCoroutine(AddSkinInventoryCouroutine());
        int counterElement = 0;
        foreach(int index in lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory)
        {
            for(int i=0; i < parentAllSkin.transform.childCount - 2; i++)
            {
                if (parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().id == index)
                {
                    if(counterElement < maxElementInPanel)
                    {
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + i).gameObject);
                        newPanel.transform.parent = dynamicPanelInventory.transform;
                        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                    else
                    {
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + i).gameObject);
                        newPanel.transform.parent = dynamicPanelInventory2.transform;
                        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                    counterElement++;
                }
                
            }
        }
       
    }

    public IEnumerator AddSkinInventoryCouroutine()
    {
        yield return new WaitForSeconds(3);
        int counterElement = 0;
        foreach (int index in lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory)
        {
            for (int i = 0; i < parentAllSkin.transform.childCount -2; i++)
            {
                if (parentAllSkin.transform.GetChild(i).GetComponent<ProductSkin>().id == index)
                {
                    if (counterElement < maxElementInPanel)
                    {
                        Debug.Log(index);
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + i).gameObject);
                        newPanel.transform.parent = dynamicPanelInventory.transform;
                        newPanel.transform.localScale = new Vector2(0.4f, 0.4f);
                    }
                    else
                    {
                        GameObject newPanel = Instantiate(parentAllSkin.transform.Find("panelSkinReturn" + i).gameObject);
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

    public void UpdateMoneyOnClick()
    {
        StartCoroutine(UpdateValeuPlayerMoneyUI_inServer());
    }

    public IEnumerator UpdateValeuPlayerMoneyUI_inServer()
    {
        string steamId;
        if (lobby.setting.MODE_TEST_SKIN_IP)
            steamId = lobby.setting.ip;
        else
            steamId = SteamUser.GetSteamID().ToString();

        WWWForm form = new WWWForm();
        Debug.LogError(steamId);
        Debug.LogError(lobby.setting.MODE_TEST_SKIN_IP);
        UnityWebRequest www = UnityWebRequest.Post(linkRequest + "/player/find?steamId=" + steamId, form);
        www.certificateHandler = new CertifcateValidator();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.downloadHandler.text);
            RequestSkin skinreturn = JsonUtility.FromJson<RequestSkin>(www.downloadHandler.text);
            lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money = skinreturn.response.money;
            UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        }
    }

    public IEnumerator AddSkinRequest(int idSkin, int price)
    {
        string steamId;
        if (lobby.setting.MODE_TEST_SKIN_IP)
            steamId = lobby.setting.ip;
        else
            steamId = SteamUser.GetSteamID().ToString();
        
        string idSkinString = idSkin + "";
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(linkRequest + "/player/addSkin?steamId=" + steamId + "&idSkin=" + idSkinString, form);
        www.certificateHandler = new CertifcateValidator();
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
        WWWForm form = new WWWForm();
        string steamId = SteamUser.GetSteamID().ToString();
        string pseudoSteam = SteamFriends.GetPersonaName();
        UnityWebRequest requestinit = UnityWebRequest.Post(linkRequest + "/paiement/init?steamId=" + steamId+"&idMoney="+ indexMoney+ "&steamPseudo=" + pseudoSteam, form);
        requestinit.certificateHandler = new CertifcateValidator();
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
        WWWForm form = new WWWForm();
        string steamId = SteamUser.GetSteamID().ToString();
        UnityWebRequest requestFinalise = UnityWebRequest.Post(linkRequest + "/paiement/finalize?steamId=" + steamId + "&orderId=" + this.orderId, form);
        requestFinalise.certificateHandler = new CertifcateValidator();
        yield return requestFinalise.SendWebRequest();

        if (requestFinalise.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(requestFinalise.downloadHandler.text);
        }
        else
        {
            Debug.Log(requestFinalise.downloadHandler.text);
            string blackSoulMoneyReturn = ParserJson.ParseStringToJson(requestFinalise.downloadHandler.text, "valueMoney");
            int blackSoulMoneyInt = int.Parse(blackSoulMoneyReturn);
            lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money = lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money + blackSoulMoneyInt;
            UpdateValeuPlayerMoneyUI(lobby.GetPlayerMineGO().GetComponent<PlayerGO>().blackSoul_money);
        }
    }
}
