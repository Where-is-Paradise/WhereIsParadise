using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManagement : MonoBehaviour
{

    public UI_Manager uiManager;

    public float currentOrthographicSize = 5.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.map.activeSelf)
        {
            if (!GetComponent<Renderer>().isVisible)
            {
                //Camera.main.orthographicSize = currentOrthographicSize;
                Camera.main.orthographicSize += 0.001f;
                currentOrthographicSize = Camera.main.orthographicSize;
            }
        }


    }
}
