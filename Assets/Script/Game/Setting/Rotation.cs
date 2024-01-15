using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float speed = 150;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.tag == "FireBall")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 newRotation2 = new Vector3(0, 0, speed * Time.deltaTime);
                transform.eulerAngles = newRotation2;
            }
            return;

        }
        if (this.gameObject.tag == "Ax")
        {
            Vector3 newRotation = new Vector3(0, 0, speed * Time.deltaTime);
            if (GetComponent<Ax>().direction.x < 0)
            {
                transform.eulerAngles += newRotation;
            }
            else
            {
                transform.eulerAngles -= newRotation;
            }
            return;
        }
        Vector3 newRotation3 = new Vector3(0, 0, speed * Time.deltaTime);
        transform.eulerAngles += newRotation3;
    }
}
