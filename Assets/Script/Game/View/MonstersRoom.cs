using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersRoom : MonoBehaviour
{
    public GameObject listSpawn;
    public bool roomIsLaunch = false;
    public GameManager gameManager;
    public bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        listSpawn = this.transform.Find("Spawns").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(roomIsLaunch && PhotonNetwork.IsMasterClient && canSpawn)
            StartCoroutine(SpawnMonsterCouroutine());
    }

    public void StartMonstersRoom()
    {
        StartCoroutine(LaunchMonsterRoom());
    }

    public IEnumerator LaunchMonsterRoom()
    {
        yield return new WaitForSeconds(2);
        roomIsLaunch = true;
        DisplayHeartsFoAllPlayer(true);
    }

    public IEnumerator SpawnMonsterCouroutine()
    {
        canSpawn = false;
        yield return new WaitForSeconds(3);
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

    public void DesactivateRoom()
    {
        DestroyAllMonster();
        this.gameObject.SetActive(false);
    }

    public void DestroyAllMonster()
    {
        GameObject[] monsterList = GameObject.FindGameObjectsWithTag("Monster");
        foreach(GameObject monster in monsterList)
        {
            PhotonNetwork.Destroy(monster);
        }
    }

}
