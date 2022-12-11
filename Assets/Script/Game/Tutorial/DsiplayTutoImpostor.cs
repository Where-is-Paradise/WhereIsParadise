using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DsiplayTutoImpostor : MonoBehaviour
{


    public GameManager gameManager;
    public int indexTuto = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButton()
    {
        if (!gameManager)
            return;
        DisplayTutorial(indexTuto);
    }


    public void DisplayTutorial(int indexPanel)
    {
        if (gameManager.setting.tutorialImpostor)
        {
            if (!gameManager.ui_Manager.listTutorialBool[indexPanel])
            {
                gameManager.ui_Manager.tutorial_parent.transform.parent.gameObject.SetActive(true);
                gameManager.ui_Manager.tutorial_parent.SetActive(true);
                gameManager.ui_Manager.tutorial[indexPanel].SetActive(true);
                gameManager.ui_Manager.listTutorialBool[indexPanel] = true;
            }
        }
    }
}
