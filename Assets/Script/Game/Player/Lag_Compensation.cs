using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lag_Compensation : MonoBehaviour, IPunObservable
{ 
    private Vector2 lastReceivedPosition;
    private float lastReceivedTime;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
/*        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(GetComponent<Rigidbody2D>().velocity);
        }
        else
        {
            lastReceivedPosition = (Vector2)transform.position;
            lastReceivedTime = (float)info.SentServerTime;
        }*/
    }

    private void FixedUpdate()
    {
/*        if (!GetComponent<PhotonView>().IsMine)
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - lastReceivedTime));
            Vector2 extrapolatedPosition = lastReceivedPosition + GetComponent<Rigidbody2D>().velocity * lag;
            transform.position = Vector2.Lerp(transform.position, extrapolatedPosition, Time.fixedDeltaTime * 10f);
        }*/
    }
}
