using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public GameObject parentAllSkin;
    public Lobby lobby;
    public GameObject dynamicPanelInventory;
    public GameObject dynamicPanelInventory2;
    private int maxElementInPanel = 9;

    // Start is called before the first frame update
    void Start()
    {
        lobby = GameObject.Find("Lobby_Manager").GetComponent<Lobby>();
        lobby.GetPlayerMineGO().GetComponent<PlayerGO>().ManageInventoryTest();
        AddSkinInventory();
        DisplayNextButton();
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
                        parentAllSkin.transform.GetChild(i).parent = dynamicPanelInventory.transform;
                    else
                        parentAllSkin.transform.GetChild(i).parent = dynamicPanelInventory2.transform;
                    counterElement++;
                }
                
            }
        }
       
    }

    public void DisplayNextButton()
    {
        if (lobby.GetPlayerMineGO().GetComponent<PlayerGO>().Inventory.Count > maxElementInPanel)
        {
            dynamicPanelInventory.transform.Find("Right button").gameObject.SetActive(true);
        }
    }
}
