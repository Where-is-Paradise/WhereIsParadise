using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallRoom : TrialsRoom
{
    public GameManager gameManager;
    public float frequencyRang = 0.1f;
    public float frequency = 4;
    public bool roomIsLaunch = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLeverToRelauch()
    {
        if (!GameObject.FindGameObjectWithTag("FireBall") && gameManager.fireBallIsLaunch)
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 2);
            gameManager.fireBallIsLaunch = false;
        }
    }

    public void LanchFireBallRoom()
    {
        roomIsLaunch = true;
        StartCoroutine(ShotFireBallFrequency());
        StartCoroutine(ReduceFrequencyCouroutine());
    }

    public void ChooseRandomTurret()
    {
        int randomInt = Random.Range(0, 4);
        GameObject turret = this.transform.GetChild(randomInt).gameObject;
        turret.GetComponent<Turret>().ShotFireBall();
    }

    public IEnumerator ShotFireBallFrequency()
    {
        if (roomIsLaunch)
        {
            yield return new WaitForSeconds(frequency);
            ChooseRandomTurret();
            StartCoroutine(ShotFireBallFrequency());
        }
    }

    public IEnumerator ReduceFrequencyCouroutine()
    {
        yield return new WaitForSeconds(3);
        if(frequency > 0.25f)
            frequency -= 0.15f;

        StartCoroutine(ReduceFrequencyCouroutine());
    }

    public void DesactivateFireBallRoom()
    {
        DestroyAllFireball();
    }

    public void DestroyAllFireball()
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        for(int i=0; i < transform.childCount;i++)
        {
            transform.GetChild(i).GetComponent<Turret>().DestroyFireBalls();
        }
    }

}
