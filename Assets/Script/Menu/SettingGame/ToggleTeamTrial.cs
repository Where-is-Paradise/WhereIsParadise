using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTeamTrial : MonoBehaviour
{
    private Setting setting;
    public int index;
    private UI_Managment uiManagement;
    // Start is called before the first frame update
    void Start()
    {
        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<Setting>();
        uiManagement = GameObject.Find("UI_Management").GetComponent<UI_Managment>();
        UpdateToggle();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateToggle();

    }

    public void OnClickToogle()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.GetComponent<Toggle>().interactable = false;
            return;
        }
        setting.listTeamTrialRoom[index] = this.GetComponent<Toggle>().isOn;
        uiManagement.SendOnChangeGameSettingTeamTrialsRoom(setting.listTeamTrialRoom[index], index);
        uiManagement.SendSetting(this.gameObject.name, this.GetComponent<Toggle>().isOn);
    }

    public void UpdateToggle()
    {
        this.GetComponent<Toggle>().isOn = setting.listTeamTrialRoom[index];
        this.GetComponent<Toggle>().interactable = true;
        if (!PhotonNetwork.IsMasterClient)
        {
            this.GetComponent<Toggle>().interactable = false;

        }
    }
}
