using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform m_Pivot;
    public List<GameObject> listPlayerNoneImpostor;
    public GameManager  gameManager;
    private bool doorHell_IsOpen = false;

    public AudioSource soundDoorHell;

    public GameObject lavaBackgrounl;
    private bool changeSensLava = false;

    void Start()
    {
        m_Pivot = gameManager.GetPlayerMineGO().transform;
    }

    private void FixedUpdate()
    {
        if (gameManager.AllPlayerHaveHell() )
        {
            
            StartCoroutine(OpenDoorHell());
        }

        if(gameManager.AllPlayerHaveHell() && !doorHell_IsOpen)
        {
            soundDoorHell.PlayDelayed(1.5f);
            doorHell_IsOpen = true;
        }
        MoveLavaBackground();

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.GetChild(0).gameObject.SetActive(false);
            collision.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void AttractivePlayer()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isImpostor)
        {
            m_Pivot.GetComponent<Rigidbody2D>().AddForce((transform.position - m_Pivot.transform.position) * 4, ForceMode2D.Force);
        }
    }

    public IEnumerator OpenDoorHell()
    {
        
        yield return new WaitForSeconds(2);
        gameManager.ui_Manager.DisplayKeyAndTorch(false);
        transform.parent.GetChild(1).gameObject.SetActive(false);
        transform.parent.GetChild(3).gameObject.SetActive(false);
        transform.parent.GetChild(7).gameObject.SetActive(false);

        transform.GetChild(0).transform.GetComponent<Animator>().SetBool("OpenHell", true);
        StartCoroutine(WaitForAttraction());

    }


    public IEnumerator WaitForAttraction()
    {
        yield return new WaitForSeconds(2);
        AttractivePlayer();
    }

    public void MoveLavaBackground()
    {
        if(lavaBackgrounl.transform.position.x > 8.5f)
        {
            changeSensLava = true;
        }
        if (lavaBackgrounl.transform.position.x < -7f)
        {
            changeSensLava = false;
        }
        if (changeSensLava)
        {
            lavaBackgrounl.gameObject.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime);
        }
        else
        {
            lavaBackgrounl.gameObject.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime);
        }
        
    }
}
