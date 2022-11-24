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
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
            MoveOnTarget();
        ChangeScaleForSituation();
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
        if (monsterRoom.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;

        if(collision.tag == "CollisionTrigerPlayer")
        {
            IsTouchPlayer(collision);
        }
    }
    public void MoveOnTarget()
    {
        float vectorX = target.transform.position.x - this.transform.position.x;
        float vectorY = target.transform.position.y - this.transform.position.y;

        this.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(new Vector3(vectorX, vectorY))  * speed ;
    }


    public void IsTouchPlayer(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerGO>().isInvincible)
            return;
        collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom--;
        collision.gameObject.GetComponent<PlayerNetwork>()
            .SendLifeTrialRoom(collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom);

        if (collision.gameObject.GetComponent<PlayerGO>().lifeTrialRoom == 0)
        {
            SetPlayerColor(collision.gameObject);

            if (TestLastPlayer())
            {
                GiveAwardToPlayer(GetLastPlayer());
                SendResetColor();
                DesactivateMonsterRoom();
            }
        }
    }
    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchByAx || !monsterRoom.gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
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
            if (!player.GetComponent<PlayerGO>().isTouchByAx)
                return player;
        }
        Debug.Log("return null");
        return null;
    }

    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
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

    public void DesactivateMonsterRoom()
    {
        this.monsterRoom.DesactivateRoom();
        PhotonNetwork.Destroy(this.gameObject);
    }
}
