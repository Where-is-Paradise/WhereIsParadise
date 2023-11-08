using Luminosity.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lag_Compensation : MonoBehaviourPun
{ 
    private Vector2 lastReceivedPosition;
    private float lastReceivedTime;


    private void FixedUpdate()
    {
        /*        if (!GetComponent<PhotonView>().IsMine)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - lastReceivedTime));
                    Vector2 extrapolatedPosition = lastReceivedPosition + GetComponent<Rigidbody2D>().velocity * lag;
                    transform.position = Vector2.Lerp(transform.position, extrapolatedPosition, Time.fixedDeltaTime * 10f);
                }*/

        if (GetComponent<PhotonView>().IsMine)
        {
            float horizontal = InputManager.GetAxis("Horizontal");
            float vertical = InputManager.GetAxis("Vertical");


            if (horizontal != 0 || vertical != 0)
            {
                photonView.RPC("SendHorizontalAndVertical", RpcTarget.Others, vertical, horizontal, this.GetComponent<PlayerGO>().movementlControlSpeed);
            }
        }
    }


    [PunRPC]
    public void SendHorizontalAndVertical(float vertical, float horizontal, float movementlControlSpeed)
    {
        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 1.1f)
        {

            horizontal *= 0.9f;
            vertical *= 0.9f;
        }
        this.transform.Translate(
        new Vector3(
            horizontal,
            vertical,
            0
        ) * movementlControlSpeed * Time.deltaTime
    );
    }

    public void SetHorizontalAndVertical(int vertical, int horizontal)
    {

    }
}
