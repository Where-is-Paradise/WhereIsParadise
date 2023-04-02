using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lag_Compensation : MonoBehaviour, IPunObservable
{

    public Vector3 networkPosition;
    public Vector3 velocity;

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            GetComponent<Rigidbody2D>().velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition  += (this.velocity * lag);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        velocity = GetComponent<Rigidbody2D>().velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.tag == "Player")
        {
            if (!GetComponent<PhotonView>().IsMine)
            {
                this.GetComponent<Rigidbody2D>().position = Vector3.MoveTowards(this.GetComponent<Rigidbody2D>().position, networkPosition, Time.fixedDeltaTime);

            }
        }
       if(this.tag == "GodDeath")
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                this.GetComponent<Rigidbody2D>().position = Vector3.MoveTowards(this.GetComponent<Rigidbody2D>().position, networkPosition, Time.fixedDeltaTime);
            }
        }
    }
}
