using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoggyRooml : MonoBehaviour
{
    public GameObject foggyAnimation;
    public bool isRight = false;
    // Start is called before the first frame update
    void Start()
    {
        foggyAnimation = this.transform.Find("FoggyAnimation").gameObject;
        isRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(foggyAnimation.transform.position.x > 50 && isRight)
        {
            isRight = false;
        }
        if (foggyAnimation.transform.position.x < -30 && !isRight)
        {
            isRight = true;
        }

        if (isRight)
        {
            foggyAnimation.transform.position += new Vector3(1 * 2f, 0, 0) * Time.deltaTime;
        }
        else
        {
            foggyAnimation.transform.position += new Vector3(-1 * 2f, 0, 0) * Time.deltaTime;
        }
            


        
    }
}
