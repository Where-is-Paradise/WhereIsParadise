using Pathfinding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathNPC_2 : MonoBehaviour
{
    public Death_NPC godDeath;
    public float speed;
    public bool isTranparencying = false;
    public bool isInvertTranparencying = false;
    public float tranparency = 255;
    public List<GameObject> listObstacle; 
    // Start is called before the first frame update
    void Start()
    {
        godDeath = GameObject.Find("DeathNpc(Clone)").GetComponent<Death_NPC>() ;
        if (!godDeath.gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            this.gameObject.SetActive(false);
        speed = godDeath.GetComponent<AIPath>().maxSpeed * 0.9f;
        Physics2D.IgnoreCollision(godDeath.transform.GetComponent<CapsuleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), true);
        for(int i =0; i < GameObject.Find("DeathNPCRoom").transform.Find("Obstacles").childCount ; i++)
        {
            listObstacle.Add(GameObject.Find("DeathNPCRoom").transform.Find("Obstacles").GetChild(i).gameObject);
        }
        foreach (GameObject obstacle in listObstacle)
        {
            Physics2D.IgnoreCollision(obstacle.GetComponent<CircleCollider2D>(), this.GetComponent<CapsuleCollider2D>(), true);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        MoveOnTarget();
        speed = godDeath.GetComponent<AIPath>().maxSpeed * 0.9f;
        ChangeScaleForSituation();
        ChangeTransparencyToInvisibilty();
        if (speed < 0.2f)
        {
            this.transform.localScale = godDeath.transform.localScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!godDeath)
            return;

        if (!godDeath.gameManager.deathNPCIsLaunch)
        {
            return;
        }

        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
                return;
            //godDeath.DeathTouchPlayerEvent(collision.gameObject);
        }

    }
    public void ChangeTransparencyToInvisibilty()
    {
        if (godDeath.isInvisible && !isTranparencying)
        {
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency/255);
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency/255);
            tranparency = (tranparency - 2f );
            if (tranparency < 0)
            {
                isTranparencying = true;
                isInvertTranparencying = false;
            }
               
   
        }
        if (!godDeath.isInvisible && !isInvertTranparencying)
        {
            this.transform.Find("body_gfx").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency/255);
            this.transform.Find("Faux").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, tranparency/255);
            tranparency = (tranparency + 2f) ;
            if (tranparency > 255)
            {
                isInvertTranparencying = true;
                isTranparencying = false;
            }
                
        }
    }

    public void ChangeToInvisible()
    {
        if (godDeath.isInvisible)
        {
            //isTranparencying = true;
        }
        else
        {
            //isInvertTranparencying = true;
        }
    }


    public void MoveOnTarget()
    {
        if (!godDeath)
            Destroy(this.gameObject);
        float vectorX = godDeath.transform.position.x - this.transform.position.x;
        float vectorY = godDeath.transform.position.y - this.transform.position.y;

        this.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(new Vector3(vectorX, vectorY)) * speed;
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


}
