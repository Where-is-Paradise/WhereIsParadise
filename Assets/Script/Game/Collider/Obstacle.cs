using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchRandomDesactive()
    {
        int randomInt = Random.Range(0, 10);
        if (randomInt < 5)
        {
            this.gameObject.SetActive(false);
            photonView.RPC("SendDesactivate", RpcTarget.All, false);
        }
        else
        {
            this.gameObject.SetActive(true);
            photonView.RPC("SendDesactivate", RpcTarget.All, true);
        }
    }
    [PunRPC]
    public void SendDesactivate(bool active)
    {
        this.gameObject.SetActive(active);
    }
}
