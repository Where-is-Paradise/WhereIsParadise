using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersRoom : TrialsRoom
{
    public GameObject listSpawn;
    public bool roomIsLaunch = false;
    public GameManager gameManager;
    public bool canSpawn = true;
    public bool canAttack = true;
    public float timerSpawnMonster = 1;
    public bool isLoose = false;

    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        listSpawn = this.transform.Find("Spawns").gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.speciallyIsLaunch)
            return;
        if (!gameManager.SamePositionAtBoss())
            return;
        if (gameManager.ui_Manager.MainRoomGraphic.transform.Find("Levers").transform.Find("OpenDoor_lever").gameObject.activeSelf)
        {
            gameManager.speciallyIsLaunch = false;
            roomIsLaunch = false;
            DestroyAllMonster();
            return;
        }
            
        if (roomIsLaunch && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss && canSpawn)
            StartCoroutine(SpawnMonsterCouroutine(timerSpawnMonster));

        if (!roomIsLaunch || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchInTrial)
            return;
        CanAttack();
    }

    public void StartMonstersRoom()
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        
        StartCoroutine(LaunchMonsterRoom());
        StartCoroutine(AddDifficulty());
       

        //this.transform.Find("X_zone_displayAnimation").gameObject.SetActive(true);
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
        gameManager.CloseDoorWhenVote(true);
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManagerParent.DisplayTorchBarre(false);
        yield return new WaitForSeconds(2);
        isLoose = false;
        roomIsLaunch = true;
        DisplaySwordAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.speciallyIsLaunch = true;
        canSpawn = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
       
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        
        float randomTimer = Random.Range(25, 80);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            photonView.RPC("SendTimer", RpcTarget.All, randomTimer);
        gameManager.ui_Manager.DisplayKeyAndTorch(false);

       
    }

    public float CalculAnimationSpeed(float timer)
    {
        return timer/ 60;
    }

    [PunRPC]
    public void SendTimer(float randomTimer)
    {
        StartCoroutine(CouroutineEndGame(randomTimer));
        this.transform.Find("X_zone_animation").gameObject.SetActive(true);
        this.transform.Find("X_zone_animation").Find("Timer").GetComponent<Timer>().LaunchTimer(randomTimer, true);
        this.transform.Find("X_zone_animation").GetComponent<Animator>().speed = (this.transform.Find("X_zone_animation").GetComponent<Animator>().speed / (CalculAnimationSpeed(randomTimer)));
    }

    public IEnumerator SpawnMonsterCouroutine( float timer)
    {
        canSpawn = false;
        yield return new WaitForSeconds(timer);
        if (roomIsLaunch)
        {
            SpawnMonster();
            canSpawn = true;
        }
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
            if (player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
                continue;

            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }
    public void DisplaySwordAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!player.GetComponent<PlayerGO>().isSacrifice && !player.GetComponent<PlayerGO>().isInJail)
                player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").gameObject.SetActive(display);
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

    public void DesactivateRoomChild()
    {
        if(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            DestroyAllMonster();
        DisplaySwordAllPlayer(false);
        timerSpawnMonster = 1;
        roomIsLaunch = false;
        this.transform.Find("X_zone_animation").gameObject.SetActive(false);
    }

    public void ResetIsTouchByMonsterAllPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().isTouchInTrial = false;
            if(player.GetComponent<PlayerGO>().hasProtection)
                player.GetComponent<PlayerGO>().lifeTrialRoom = 3;
            else
                player.GetComponent<PlayerGO>().lifeTrialRoom = 2;
        }
    }


    public void DestroyAllMonster()
    {
        GameObject[] monsterList = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsterList)
        {
            PhotonNetwork.Destroy(monster);
        }
    }

    /* Animation sword attack */
    public IEnumerator DisplayInitial(int indexPlayer)
    {
        yield return new WaitForSeconds(0.4f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Initial").gameObject.SetActive(true);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().canMove = true;
        canAttack = true;
    }

    [PunRPC]
    public void DisplayMiddleOne(int indexPlayer)
    {
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Initial").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle1").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleTwo(indexPlayer));
    }
    public IEnumerator DisplayMiddleTwo(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle1").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle2").gameObject.SetActive(true);
        StartCoroutine(DisplayMiddleThree(indexPlayer));
    }
    public IEnumerator DisplayMiddleThree(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle2").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle3").gameObject.SetActive(true);
        StartCoroutine(DisplayFinal(indexPlayer));
    }
    public IEnumerator DisplayFinal(int indexPlayer)
    {
        yield return new WaitForSeconds(0.01f);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("middle3").gameObject.SetActive(false);
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Final").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameManager.GetPlayer(indexPlayer).transform.Find("Skins").GetChild(gameManager.GetPlayer(indexPlayer).GetComponent<PlayerGO>().indexSkin).Find("SwordMonster").Find("Final").gameObject.GetComponent<BoxCollider2D>().enabled = true;
        StartCoroutine(DisplayInitial(indexPlayer));
    }
    public void GiveAwardToPlayer(GameObject lastPlayer)
    {
        photonView.RPC("SetCanLunchExploration", RpcTarget.All, lastPlayer.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SetCanLunchExploration(int indexPlayer)
    {
        if (!gameManager.SamePositionAtBoss())
            return;
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendOnclickToExpedtionN2();
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerNetwork>().SendHasWinFireBallRoom(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().SetCanLaunchExplorationCoroutine(true);
        gameManager.GetPlayer(indexPlayer).gameObject.GetComponent<PlayerGO>().gameManager.ui_Manager.mobileCanvas.transform.Find("Exploration_button").gameObject.SetActive(true);
    }

    public void DisplayLeverToRelauch()
    {
        if (!GameObject.FindGameObjectWithTag("Monster"))
        {
            gameManager.ui_Manager.DisplaySpeciallyLevers(true, 9);
            roomIsLaunch = false;
        }
    }



    public IEnumerator CouroutineEndGame(float seconde)
    {
        //yield return new WaitForSeconds(seconde);

        yield return new WaitForSeconds(seconde);
        if (!isLoose)
        {
            GiveTeamAward();
          
        }
        
        //photonView.RPC("SendTeamAward", RpcTarget.All);
    }
    

    public void GiveTeamAward()
    {
        gameManager.teamHasWinTrialRoom = true;
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;
        
        int randomInt = Random.Range(0, 2);
        photonView.RPC("SendIndexAward", RpcTarget.All, randomInt);
    }
    [PunRPC]
    public void SendIndexAward(int randomInt)
    {
        DisplayGloballyAward(randomInt);
        DesactivateRoom();
        DesactivateRoomChild();
        //gameManager.ui_Manager.DisplayKeyAndTorch(true);
        this.transform.Find("X_zone_animation").GetComponent<Animator>().speed = 1;
        this.transform.Find("X_zone_animation").Find("Timer").GetComponent<Timer>().ResetTimer();
    }
}
