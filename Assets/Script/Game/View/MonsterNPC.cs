using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNPC : MonoBehaviourPun
{
    public MonstersRoom monsterRoom;
    public PlayerGO target;
    public float speed = 3;
    // Start is called before the first frame update
    void Start()
    {
        monsterRoom = GameObject.Find("MonstersRoom").GetComponent<MonstersRoom>();
        if (!monsterRoom)
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeScaleForSituation();
        if (!monsterRoom)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CapsuleCollider2D>().enabled = false;
            return;
        }
        else
        {
            if (!monsterRoom.gameManager.SamePositionAtBoss())
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
                this.GetComponent<CapsuleCollider2D>().enabled = false;
                return;
            }
        }
        if (monsterRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            MoveOnTarget();
            if (target)
            {
                if (target.isTouchByMonster || target.isSacrifice)
                    ChangeRandomTarget();
            }
        }

     
    }

    public void ChangeScaleForSituation()
    {
        if (GetComponent<Rigidbody2D>().velocity.x >= 0.01f)
        {
            this.transform.localScale = new Vector2(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
        else if (GetComponent<Rigidbody2D>().velocity.x <= -0.01f)
        {
            this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ( (monsterRoom && !monsterRoom.gameManager.SamePositionAtBoss()))
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
                return;
            IsTouchPlayer(collision);
          
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (monsterRoom && !monsterRoom.gameManager.SamePositionAtBoss())
            return;
        if (collision.tag == "CollisionTrigerPlayer")
        {
            if (!collision.transform.parent.GetComponent<PhotonView>().IsMine)
                return;
            IsTouchPlayer(collision);

        }
    }

    public void MoveOnTarget()
    {
        if (!target)
        {
            ChangeRandomTarget();
            return;
        }

        float vectorX = target.transform.position.x - this.transform.position.x;
        float vectorY = target.transform.position.y - this.transform.position.y;

        this.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(new Vector3(vectorX, vectorY))  * speed ;
    }

    public void ChangeRandomTarget()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listPotentialPlayer = new List<GameObject>();
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByMonster)
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            listPotentialPlayer.Add(player);
        }

        int randomIndexPlayer = Random.Range(0, listPotentialPlayer.Count);
        target = listPotentialPlayer[randomIndexPlayer].GetComponent<PlayerGO>() ;
    }

    public void IsTouchPlayer(Collider2D collision)
    {
        GameObject player = collision.transform.parent.gameObject;
        if (player.GetComponent<PlayerGO>().isInvincible)
            return;
        player.GetComponent<PlayerGO>().lifeTrialRoom--;
        player.GetComponent<PlayerNetwork>()
            .SendLifeTrialRoom(player.GetComponent<PlayerGO>().lifeTrialRoom);

        if (player.GetComponent<PlayerGO>().lifeTrialRoom <= 0)
        {
            SetPlayerColor(player);

            if (TestLastPlayer())
            {
                monsterRoom.GiveAwardToPlayer(GetLastPlayer());
                photonView.RPC("SendDectivateRoom", RpcTarget.All);
            }
        }
    }
    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByMonster || !monsterRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!monsterRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchByMonster)
                return player;
        }
        Debug.Log("return null");
        return null;
    }





    public void SetPlayerColor(GameObject player)
    {
        player.gameObject.GetComponent<PlayerNetwork>().SendIstouchByMonsterNPC(true);
        player.gameObject.GetComponent<PlayerNetwork>().SendChangeColorWhenTouchByDeath();
    }

    public void ChoosePlayerRandomly()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int randomIndex = Random.Range(0, listPlayer.Length);
        target = listPlayer[randomIndex].GetComponent<PlayerGO>();
    }

    public void SendResetColor()
    {
        photonView.RPC("ResetColorAllPlayer", RpcTarget.All);
    }

    public void SendDestroy()
    {
        this.gameObject.SetActive(false);
        photonView.RPC("SetDestroy", RpcTarget.All);
    }

    [PunRPC]
    public void SetDestroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    [PunRPC]
    public void SendDectivateRoom()
    {
       if (!monsterRoom.gameManager.SamePositionAtBoss())
            return;
        this.monsterRoom.DesactivateRoom();
    }
}
