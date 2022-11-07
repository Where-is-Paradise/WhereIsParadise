using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviourPun
{
    public int index;
    public float frequency;
    public GameObject fireBall;
    public GameManager gameManager;
    public bool canFire = false;
    public int categorie = 0;
    public bool therenotMasterClient = false;

    // Start is called before the first frame update
    void Start()
    {
        //LaunchTurret(true);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            return;
        }
        if (canFire && gameManager.fireBallIsLaunch)
        {
            ShotFireBall();
            canFire = false;
        }
    }

    public void LaunchTurret()
    {
        switch (categorie)
        {
            case 0:
                frequency = 6 + index;
                SendFrequency(frequency);
                break;
            case 1:
                frequency = Random.Range(0.25f + index, 1.5f + index);
                SendFrequency(frequency);
                break;
            case 2:
                frequency = 2.5f + index;
                SendFrequency(frequency);
                break;
        }
/*        frequency = Random.Range(0.25f + index, 1.5f + index);
        SendFrequency(frequency);*/

    }

    public void ShotFireBall()
    {
        GameObject fireball = PhotonNetwork.Instantiate("FireBall", this.transform.Find("SpawnFireball").position, Quaternion.identity);
        fireball.GetComponent<FireBall>().direction = -this.transform.up;
        fireball.transform.parent = this.gameObject.transform;
        fireball.GetComponent<FireBall>().SendParent(fireball.transform.parent.GetComponent<Turret>().index);
        RandomSpeedCategoriFireball(fireball);
        switch (categorie)
        {
            case 0:
                frequency = 6 * index;
                SendFrequency(frequency);
                break;
            case 1:
                frequency = 2.5f * index;
                SendFrequency(frequency);
                break;
            case 2:
                frequency = Random.Range(0.25f + index, 1.5f + index);
                SendFrequency(frequency);
                break;
        }

    }

    public void RandomSpeedCategoriFireball(GameObject fireball)
    {


        switch (categorie)
        {
            case 0:
                fireball.GetComponent<FireBall>().speed = 5.5f;
                break;
            case 1:
                fireball.GetComponent<FireBall>().speed = 3.5f;
                break;
            case 2:
                fireball.GetComponent<FireBall>().speed = 1.25f;
                break;
        }

        //fireball.GetComponent<FireBall>().speed = 1.5f;
        /*        frequency = 2 * index;
                SendFrequency(frequency);*/
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
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().SamePositionAtBoss())
        {
            return;
        }
        StartCoroutine(CoroutineFrequency(frequency));
    }

    public void DestroyFireBalls()
    {
        for(int i = 0; i < transform.childCount; i++){
            if(this.transform.GetChild(i).GetComponent<FireBall>())
                this.transform.GetChild(i).GetComponent<FireBall>().SendDestroy();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "FireBall")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        }
    }
}
