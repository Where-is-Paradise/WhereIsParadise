using Luminosity.IO;
using CI.QuickSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LanguageManagement : MonoBehaviour
{

    public string indexStringInFileLanguage = "";
    private string result;
    private Setting setting;
    public bool isUpdated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadLanguage());
        setting = GameObject.Find("Setting").GetComponent<Setting>();

            QuickSaveReader.Create(setting.langage)
                    .Read<string>(indexStringInFileLanguage, (r) => { result = r; });

        GetComponent<Text>().text = result;

        GetComponent<Text>().color = new Color(255,0,0);


    }

    // Update is called once per frame
    void Update()
    {
        //UpdateLanguage();
    }

    public IEnumerator LoadLanguage()
    {
        yield return new WaitForSeconds(0.5f);
        QuickSaveReader.Create("fr")
                      .Read<string>(indexStringInFileLanguage, (r) => { result = r; });

        GetComponent<Text>().text = result;
    }

    public void UpdateLanguage()
    {
        if (!setting.canUpdate)
            return;
        setting = GameObject.Find("Setting").GetComponent<Setting>();
        try
        {
            QuickSaveReader.Create(setting.langage)
                .Read<string>(indexStringInFileLanguage, (r) => { result = r; });
        }catch(Exception e)
        {
            Debug.LogError(e);
        }
        GetComponent<Text>().text = result;
    }



}
