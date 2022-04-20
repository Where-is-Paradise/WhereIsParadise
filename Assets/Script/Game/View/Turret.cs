using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviourPun
{
    public int index;
    public float frequency;
    public GameObject fireBall;

    public bool canFire = false;
    // Start is called before the first frame update
    void Start()
    {
        //LaunchTurret(true);
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

    public void LaunchTurret()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        frequency = Random.Range(5, 15);
        SendFrequency(frequency);
    }

    public void ShotFireBall()
    {
        GameObject fireball = PhotonNetwork.Instantiate("FireBall", this.transform.position, Quaternion.identity);
        fireball.GetComponent<FireBall>().direction = this.transform.right;
        fireball.transform.parent = this.gameObject.transform;
        fireball.GetComponent<FireBall>().SendParent(fireball.transform.parent.GetComponent<Turret>().index);
        frequency = Random.Range(5, 15);
        SendFrequency(frequency);
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

    public void DestroyFireBalls()
    {
        for(int i = 0; i < transform.childCount; i++){
            this.transform.GetChild(i).GetComponent<FireBall>().SendDestroy();
        }
    }
}
