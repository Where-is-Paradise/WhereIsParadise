using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_zIndex : MonoBehaviour
{

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = (player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Colors")
            .GetChild(player.GetComponent<PlayerGO>().indexSkinColor).GetComponent<SpriteRenderer>().sortingOrder + 1);


    }
}
