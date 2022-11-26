using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersRoom : MonoBehaviourPun
{
    public GameObject listSpawn;
    public bool roomIsLaunch = false;
    public GameManager gameManager;
    public bool canSpawn = true;
    public bool canAttack = true;
    public float timerSpawnMonster = 1;
    // Start is called before the first frame update
    void Start()
    {
        listSpawn = this.transform.Find("Spawns").gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(roomIsLaunch && PhotonNetwork.IsMasterClient && canSpawn)
            StartCoroutine(SpawnMonsterCouroutine(timerSpawnMonster));

        if (!roomIsLaunch || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchByMonster)
            return;
        CanAttack();
    }

    public void StartMonstersRoom()
    {
        StartCoroutine(LaunchMonsterRoom());
        StartCoroutine(AddDifficulty());
    }
    public IEnumerator AddDifficulty()
    {
        yield return new WaitForSeconds(1);
        if (timerSpawnMonster > 0.2f)
            timerSpawnMonster = timerSpawnMonster - 0.01f;
        StartCoroutine(AddDifficulty());
    }

    public IEnumerator LaunchMonsterRoom()
    {
        yield return new WaitForSeconds(2);
        roomIsLaunch = true;
        DisplaySwordAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(true);

    }

    public IEnumerator SpawnMonsterCouroutine( float timer)
    {
        canSpawn = false;
        yield return new WaitForSeconds(timer);
        SpawnMonster();
        canSpawn = true;
    }

    public void SpawnMonster()
    {
        int indexSpawn = Random.Range(0, listSpawn.transform.childCount);
        GameObject Spawn = listSpawn.transform.GetChild(indexSpawn).gameObject;
        GameObject monster = PhotonNetwork.Instantiate("Monster", Spawn.transform.position, Quaternion.identity);
        monster.GetComponent<MonsterNPC>().ChoosePlayerRandomly();
    }

    public void DisplayHeartsFoAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }
    public void ResetHeartForAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().ResetHeart();
        }
    }


    public void DisplaySwordAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PlayerGO>().isSacrifice)
                player.transform.Find("Perso").Find("SwordMonster").gameObject.SetActive(display);
        }
    }
    public void CanAttack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            DisplaySwordAttack();
        }
    }

    public void DisplaySwordAttack()
    {
        LaunchAnimationAttack();
        canAttack = false;
        gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().canMove = false;
    }

    public void LaunchAnimationAttack()
    {
        int indexPlayer = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
        photonView.RPC("DisplayMiddleOne", RpcTarget.Others, indexPlayer);
        DisplayMiddleOne(indexPlayer);
    }

    public void DesactivateRoom()
    {
        DestroyAllMonster();
        DisplaySwordAllPlayer(false);
        ResetHeartForAllPlayer();
        ResetIsTouchByMonsterAllPlayer();
        ResetColorAllPlayer();
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = true;
        gameManager.speciallyIsLaunch = false;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.CloseDoorWhenVote(false);
        timerSpawnMonster = 5;
       
        this.gameObject.SetActive(false);
    }
    public void ResetIsTouchByMonsterAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().isTouchByMonster = false;
            player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
        }
    }
    public void ResetColorAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                int indexSkin = player.gameObject.GetComponent<PlayerGO>().indexSkin;
                player.transform.GetChild(1).GetChild(1).GetChild(indexSkin).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
            }
            else
            {
                if (gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            player.GetComponent<PlayerGO>().ResetHeart();
            player.GetComponent<PlayerGO>().isTouchByMonster = false;
        }
    }

    public void DestroyAllMonster()
    {
        GameObject[] monsterList = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsterList)
        {
            Destroy(monster);
        }
    }

    /* Animation sword attack */
    public IEnumerator DisplayInitial(int indexPlayer)
    {
        yield return new WaitForSeconds(0.4f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Initial").gameObject.SetActive(true);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().canMove = true;
        canAttack = true;
    }

    [PunRPC]
    public void DisplayMiddleOne(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Initial").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle1").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleTwo(indexPlayer));
    }
    public IEnumerator DisplayMiddleTwo(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle1").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle2").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleThree(indexPlayer));
    }
    public IEnumerator DisplayMiddleThree(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle2").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle3").gameObject.SetActive(true);
        StartCoroutine(DisplayFinal(indexPlayer));
    }
    public IEnumerator DisplayFinal(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("middle3").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameManager.GetPlayer(indexPlayer).transform.Find("Perso").Find("SwordMonster").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = true;
        StartCoroutine(DisplayInitial(indexPlayer));
    }



}
