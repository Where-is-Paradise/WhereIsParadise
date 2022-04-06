using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using UnityEngine.UI;

public class EnterButton : MonoBehaviour
{

    public Text FormNamePlayer; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetButtonDown("Enter"))
        {
            if(FormNamePlayer.text.Length > 0)
            {
                this.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
