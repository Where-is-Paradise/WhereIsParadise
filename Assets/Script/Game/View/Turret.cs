using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviourPun
{
    public float frequency;
    public GameObject fireBall;

    private bool canFire = false;
    // Start is called before the first frame update
    void Start()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        frequency = Random.Range(5, 15);

        SendFrequency(frequency);


    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (canFire)
        {
            ShotFireBall();
            canFire = false;
        }
        

    }

    public void ShotFireBall()
    {
        //GameObject fireball = Instantiate(fireBall, this.transform.position , Quaternion.identity);
        GameObject fireball = PhotonNetwork.Instantiate("FireBall", this.transform.position, Quaternion.identity);
        fireball.GetComponent<FireBall>().direction = this.transform.right;

        frequency = Random.Range(5, 15);
        SendFrequency(frequency);
        //StartCoroutine(CoroutineFrequency(frequency));
    }

    public IEnumerator CoroutineFrequency(float frequency)
    {
        yield return new WaitForSeconds(frequency);
        canFire = true;
        
    }

    public void SendFrequency(float frequency)
    {
        photonView.RPC("SetFrequency", RpcTarget.All,frequency);
    }

    [PunRPC]
    public void SetFrequency(float frequency)
    {
        StartCoroutine(CoroutineFrequency(frequency));
    }
}
