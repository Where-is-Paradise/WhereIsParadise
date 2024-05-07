using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover_clickFront : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update

    public bool isHover = false;

    public void Update()
    {
        if (isHover)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.transform.Find("lghte_click").gameObject.SetActive(true);
                if (this.transform.Find("lghte_click").gameObject.activeSelf)
                    StartCoroutine(CouroutineUnDisplay());
            }
        }
    }

    public IEnumerator CouroutineUnDisplay()
    {
        yield return new WaitForSeconds(0.1f);
        this.transform.Find("lghte_click").gameObject.SetActive(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.Find("lghte_hover").gameObject.SetActive(true);
        isHover = true;

       

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        this.transform.Find("lghte_hover").gameObject.SetActive(false);
    }
}
