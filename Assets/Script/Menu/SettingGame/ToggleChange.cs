using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChange : MonoBehaviour
{
    public int index;
    public GameObject toggle1;
    public GameObject toggle2;
    public bool isOneActive = true;
    public Setting setting;
    public UI_Managment uiManager;
    // Start is called before the first frame update
    void Start()
    {
        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<Setting>();
        UpdateToggle();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateToggle();

    }

    public void OnClickToggle1()
    {
        ActivateCheckMark(toggle1, true);
        ActivateCheckMark(toggle2, false);
        isOneActive = true;
        if(PhotonNetwork.IsMasterClient)
            uiManager.SendSettingDoubleChoice(this.gameObject.name, 0);
    }
    public void OnClickToggle2()
    {
        ActivateCheckMark(toggle1, false);
        ActivateCheckMark(toggle2, true);
        isOneActive = false;
        if (PhotonNetwork.IsMasterClient)
            uiManager.SendSettingDoubleChoice(this.gameObject.name, 1);
    }

    public void ActivateCheckMark(GameObject toogle, bool activate)
    {
        toogle.transform.Find("Background").Find("Checkmark").gameObject.SetActive(activate);
    }

    public void SetNbCorruptedSoul()
    {
        if (isOneActive)
        {
            setting.NB_IMPOSTOR = 2;
            uiManager.SendNBImpostorSetting(setting.NB_IMPOSTOR);
            return;
        }
        setting.NB_IMPOSTOR = 3;
        uiManager.SendNBImpostorSetting(setting.NB_IMPOSTOR);
    }

    public void SetKeyAdditional()
    {
        if (isOneActive)
        {
            setting.KEY_ADDITIONAL = 1;
            uiManager.SendKeyAdditionalSetting(setting.KEY_ADDITIONAL);
            return;
        }
        setting.KEY_ADDITIONAL = 2;
        uiManager.SendKeyAdditionalSetting(setting.KEY_ADDITIONAL);
    }

    public void SetTorchAdditional()
    {
        if (isOneActive)
        {
            setting.TORCH_ADDITIONAL = 0;
            uiManager.SendKeyTorchSetting(setting.TORCH_ADDITIONAL);
            return;
        }
        setting.TORCH_ADDITIONAL = 1;
        uiManager.SendKeyTorchSetting(setting.TORCH_ADDITIONAL);
    }

    public void SetColorDesactivate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.GetComponent<Button>().interactable = false;
            this.transform.Find("Background").GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        }
        else
        {
            this.GetComponent<Button>().interactable = true;
            this.transform.Find("Background").GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        }
            
    }

    public void UpdateToggle()
    {
        SetColorDesactivate();
        switch (index)
        {
            case 0:
                if (setting.NB_IMPOSTOR == 2)
                {
                    ActivateCheckMark(toggle1, true);
                    ActivateCheckMark(toggle2, false);
                    isOneActive = true;
                }
                else
                {
                    ActivateCheckMark(toggle2, true);
                    ActivateCheckMark(toggle1, false);
                    isOneActive = false;
                }
                break;
            case 1:
                if (setting.KEY_ADDITIONAL == 1)
                {
                    ActivateCheckMark(toggle1, true);
                    ActivateCheckMark(toggle2, false);
                    isOneActive = true;
                }
                else
                {
                    ActivateCheckMark(toggle2, true);
                    ActivateCheckMark(toggle1, false);
                    isOneActive = false;
                }
                    
                break;
            case 2:
                if (setting.TORCH_ADDITIONAL == 0)
                {
                    ActivateCheckMark(toggle1, true);
                    ActivateCheckMark(toggle2, false);
                    isOneActive = true;
                }
                else
                {
                    ActivateCheckMark(toggle2, true);
                    ActivateCheckMark(toggle1, false);
                    isOneActive = false;
                }
                break;
        }

    }
}
