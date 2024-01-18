using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class informationEndRoom : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchInformationEndRoom()
    {
        photonView.RPC("DisplayMessage", RpcTarget.All);
    }

    [PunRPC]
    public void DisplayMessage()
    {
        if (this.transform.Find("NPC").Find("SquareMessage").gameObject.activeSelf)
            return;
        this.transform.Find("NPC").Find("SquareMessage").gameObject.SetActive(true);
        StartCoroutine(HideMessage());
    }

    public IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(3);
        this.transform.Find("NPC").Find("SquareMessage").gameObject.SetActive(false);
    }
}
