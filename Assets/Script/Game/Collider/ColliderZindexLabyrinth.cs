using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderZindexLabyrinth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.name == "CollisionZIndex")
        {
            PlayerGO player = collision.transform.parent.parent.gameObject.GetComponent<PlayerGO>();
            int indexSkin = player.indexSkin;
            int indexSkinColor = player.indexSkinColor;
            SetZIndexByPositionYToLabyritnhRoom(this.transform.parent.GetComponent<ObstacleLabyrinth>().Y_position, indexSkin, indexSkinColor, player.gameObject);
        }

        if (!this.transform.parent.GetComponent<ObstacleLabyrinth>().hasAward)
        {
            return;
        }
        if(!(this.name == "CollisionAward") || !(collision.name == "collisionLeft" || collision.name == "collisionUp" || collision.name == "collisionDown"))
        {
            return;
        }
        if (!collision.transform.parent.parent.gameObject.GetComponent<PhotonView>().IsMine)
            return;
        ObstacleLabyrinth obstacle = this.transform.parent.GetComponent<ObstacleLabyrinth>();
        obstacle.labyrinthRoom.SendObjectAwardFind(obstacle.labyrinthRoom.gameManagerParent.GetPlayerMineGO().GetComponent<PhotonView>().ViewID, obstacle.indexObject, obstacle.indexObjectInList);
        obstacle.DesactivateAward();
        obstacle.labyrinthRoom.SendDesactivateAward(obstacle.X_position, obstacle.Y_position);
    }

    public void SetZIndexByPositionYToLabyritnhRoom(int indexObstacle, int indexSkin, int indexSkinColor, GameObject player)
    {
        //player.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().sortingOrder = indexObstacle - 11;
        player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = indexObstacle - 11;
    }
}
