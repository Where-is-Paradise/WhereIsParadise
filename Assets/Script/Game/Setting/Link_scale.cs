using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link_scale : MonoBehaviour
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
        if(Mathf.Sign(player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).localScale.x) > 0){
            if(this.transform.localScale.x < 0)
                this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y);
        }else if (Mathf.Sign(player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).localScale.x) < 0)
        {
            if (this.transform.localScale.x > 0)
                this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y);
        }

        if(this.transform.localScale.x < 0)
        {
            this.transform.localPosition = new Vector2(0.07f, this.transform.localPosition.y);
        }
        else
        {
            this.transform.localPosition = new Vector2(-0.065f, this.transform.localPosition.y);
        }


    }
}
