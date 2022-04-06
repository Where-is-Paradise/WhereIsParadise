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
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadLanguage());
        setting = GameObject.Find("Setting").GetComponent<Setting>();


            QuickSaveReader.Create(setting.langage)
                    .Read<string>(indexStringInFileLanguage, (r) => { result = r; });

        GetComponent<Text>().text = result;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadLanguage()
    {
        yield return new WaitForSeconds(0.5f);
        QuickSaveReader.Create("fr")
                      .Read<string>(indexStringInFileLanguage, (r) => { result = r; });

        GetComponent<Text>().text = result;
    }

}
