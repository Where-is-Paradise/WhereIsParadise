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
        if(!(this.name == "CollisionAward"))
        {
            return;
        }

    }

    public void SetZIndexByPositionYToLabyritnhRoom(int indexObstacle, int indexSkin, int indexSkinColor, GameObject player)
    {
        player.transform.Find("Skins").GetChild(indexSkin).GetComponent<SpriteRenderer>().sortingOrder = indexObstacle - 11;
        player.transform.Find("Skins").GetChild(indexSkin).Find("Colors").GetChild(indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder = indexObstacle - 11;
    }
}
