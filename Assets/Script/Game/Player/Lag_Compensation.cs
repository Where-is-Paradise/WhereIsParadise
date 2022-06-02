using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lag_Compensation : MonoBehaviour, IPunObservable
{

    public Vector3 networkPosition;
    public Rigidbody2D rigidbody;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.transform.position);

        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            
            if (rigidbody)
            {
                rigidbody.position += rigidbody.velocity * lag;
            }
               
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            rigidbody.position = Vector3.MoveTowards(rigidbody.position, networkPosition, Time.fixedDeltaTime);
            return;
        }
    }
}
