using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HyperLinks : MonoBehaviour
{

    public string link;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(HyperLink);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HyperLink()
    {
        Application.OpenURL(link);
    }
}
