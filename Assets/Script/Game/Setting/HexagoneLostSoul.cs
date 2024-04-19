using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagoneLostSoul : MonoBehaviour
{

    public int indexRoom;
    public Room room;
    public GameManager gameManager;
    public bool isLightByOther = false;
    public bool isLighted = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        room = gameManager.game.dungeon.GetRoomByIndex(indexRoom);
        this.GetComponent<BoxCollider>().enabled = true;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnClickToLight();
        }
    }

    public void OnClickToLight()
    {
        if (this.room.IsObstacle)
            return;
        if (this.isLightByOther)
            return;
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lightHexagoneIsOn && !isLighted)
            return;
        if (this.transform.parent.name == "Listhexa")
            return;

        isLighted = !this.transform.Find("Light").gameObject.activeSelf;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lightHexagoneIsOn = isLighted;
        this.transform.Find("Light").gameObject.SetActive(isLighted);
        gameManager.gameManagerNetwork.SendLightHexagoneLostSoul(this.room.Index, isLighted);
    }

    public void SetLight(bool active)
    {
        this.transform.Find("Light").gameObject.SetActive(active);
        isLightByOther = active;
        isLighted = active;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().lightHexagoneIsOn = active;
        gameManager.gameManagerNetwork.SendLightHexagoneLostSoul(this.room.Index, active);
    }
}
