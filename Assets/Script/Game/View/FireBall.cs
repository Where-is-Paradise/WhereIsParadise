using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviourPun
{
    public int speed = 2;
    public Vector2 direction = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GetComponent<Rigidbody2D>().velocity = direction * speed;
       
    }

    public void ChangeDirection(string nameWall)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (nameWall == "Left" || nameWall == "Right")
        {
            if (direction.y < 0)
            {
                direction = new Vector2(-direction.x, -1 * Random.Range(0.5f, 1f));
            }
            else
            {
                direction = new Vector2(-direction.x, Random.Range(0.5f, 1f));
            }

        }
        if (nameWall == "Top" || nameWall == "Bottom")
        {
            if (direction.x < 0)
            {
                direction = new Vector2(-1 * Random.Range(0.5f, 1f), -direction.y);
            }
            else
            {
                direction = new Vector2(Random.Range(0.5f, 1f), -direction.y);
            }

        }
        /*        if (nameWall == "Left" || nameWall == "Right")
                {
                     direction = new Vector2(-direction.x, direction.y);
                }
                if (nameWall == "Top" || nameWall == "Bottom")
                {
                    direction = new Vector2(direction.x, -direction.y);
                }*/
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                return;
            }
            if (collision.gameObject.GetComponent<PlayerGO>().isTouchByFireBall)
            {
                return;
            }
            collision.gameObject.GetComponent<PlayerGO>().isTouchByFireBall = true;
            collision.gameObject.GetComponent<PlayerGO>().DisplayCharacter(false);

            
            // dont work 
            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(this.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            if (collision == null)
            {
                return;
            }
            ChangeDirection(collision.gameObject.name);
        }
    }




}
