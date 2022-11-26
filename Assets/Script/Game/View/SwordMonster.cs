using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMonster : MonoBehaviour
{
    public SwordRoom MonstersRoom;
    // Start is called before the first frame update
    void Start()
    {
        MonstersRoom = GameObject.Find("MonstersRoom").GetComponent<SwordRoom>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        IsTouchMonster(collision);
    }


    public void IsTouchMonster(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            if (!this.transform.parent.parent.GetComponent<PhotonView>().IsMine)
                return;
            collision.GetComponent<MonsterNPC>().SendDestroy();
        }
    }

}
