using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSelectForm : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<InputField>().Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
