using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerRegion_display : MonoBehaviour
{

    public Setting setting;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = AdapteRegionToPhotonComprehsion(setting.region);

    }

    public string AdapteRegionToPhotonComprehsion(string textRegion)
    {
        string textReturn = "Europe";
        switch (textRegion)
        {
            case "eu":
                textReturn = "Europe";
                break;
            case "cae":
                textReturn = "Canada, East";
                break;
            case "kr":
                textReturn = "South Korea";
                break;
            case "jp":
                textReturn = "Japan";
                break;
            case "us":
                textReturn = "USA, East";
                break;
            case "usw":
                textReturn = "USA, West";
                break;
            case "ussc":
                textReturn = "USA,S Central";
                break;
            case "au":
                textReturn = "Australia";
                break;
            case "sa":
                textReturn = "South America";
                break;
            case "asia":
                textReturn = "Asia";
                break;
            case "uae":
                textReturn = "United Arab Emirates";
                break;
            case "cn":
                textReturn = "Chinese Mainland";
                break;
            case "hk":
                textReturn = "Hong Kong";
                break;
            case "in":
                textReturn = "India";
                break;
            case "za":
                textReturn = "South Africa";
                break;
            case "tr":
                textReturn = "Turkey";
                break;
        }

        return textReturn;
    }
}
