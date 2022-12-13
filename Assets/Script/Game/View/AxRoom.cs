using Luminosity.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxRoom : MonoBehaviourPun
{
    public GameManager gameManager;
    public bool isPreparedToLauch = false;
    public Vector3 currentDirection = new Vector3(0,0,0);
    public bool isLaunch = false;
    public bool canShoot = true;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaunch)
            return;
        if (Input.GetMouseButton(0) && !isPreparedToLauch && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchByAx)
        {
            isPreparedToLauch = true;
            DisplayLineToShot(true);
        }
        if (isPreparedToLauch)
        {
            UpdatePositionAndRotationOfLineByMouse();
            if (Input.GetMouseButtonUp(0))
            {
                isPreparedToLauch = false;
                DisplayLineToShot(false);
                if (!canShoot)
                    return;
                SendShotAxToDirection();
               
                canShoot = false;
                StartCoroutine(CanShootCoroutine());
            }
        }
    }
    public void LaunchAxRoom()
    {
        StartCoroutine(LaunchAxRoomAfterTeleportation());
    }
    public IEnumerator LaunchAxRoomAfterTeleportation()
    {
        yield return new WaitForSeconds(2);
        DiplayAxForAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        isLaunch = true;
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
        gameManager.CloseDoorWhenVote(true);
        
    }

    public IEnumerator CanShootCoroutine()
    {
        yield return new WaitForSeconds(2);
        canShoot = true;
    }

    public void DiplayAxForAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.transform.Find("Perso").Find("Ax").gameObject.SetActive(display);
        }
    }

    public void DisplayHeartsFoAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            player.GetComponent<PlayerGO>().DisiplayHeartInitial(display);
        }
    }

    public void DisplayLineToShot(bool display)
    {
        gameManager.GetPlayerMineGO().transform.Find("Perso").Find("LineForAx").gameObject.SetActive(display);
    }
    public void UpdatePositionAndRotationOfLineByMouse()
    {
        Vector3 wPos = Input.mousePosition;
        wPos.z = gameManager.GetPlayerMineGO().transform.position.z - Camera.main.transform.position.z;
        wPos = Camera.main.ScreenToWorldPoint(wPos);
        Vector3 direction = wPos - gameManager.GetPlayerMineGO().transform.position;
        float radius = 1;
        direction = Vector3.ClampMagnitude(direction, radius);
        gameManager.GetPlayerMineGO().transform.Find("Perso").Find("LineForAx").position = (gameManager.GetPlayerMineGO().transform.position + direction) ;
        currentDirection = direction;
    }

    [PunRPC]
    public void ShotAxToDirection( float positionX, float positionY, float directionX, float directionY , int indexPlayer)
    {
        if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
            return;

        GameObject newAx = PhotonNetwork.Instantiate("Ax", new Vector3(positionX,positionY), Quaternion.identity);
        newAx.GetComponent<Ax>().SendLancher(indexPlayer);
        newAx.GetComponent<Ax>().SendSpeedAndDirection(5, directionX, directionY);
        newAx.GetComponent<Ax>().player = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
    }

    public void SendShotAxToDirection()
    {
        Vector3 positionPlayer = gameManager.GetPlayerMineGO().transform.position;
        int indexPlayer = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
        photonView.RPC("ShotAxToDirection", RpcTarget.All , positionPlayer.x , positionPlayer.y, currentDirection.x, currentDirection.y , indexPlayer);
    }

    public void DesactivateRoom()
    {
        GameObject[] listAx = GameObject.FindGameObjectsWithTag("Ax");
        foreach(GameObject ax  in listAx)
        {
            PhotonNetwork.Destroy(ax);
        }
        photonView.RPC("SendDisplayAxForAllPlayer", RpcTarget.All, false);
        photonView.RPC("SetIsLaunch", RpcTarget.All, false);
        photonView.RPC("SendSpeciallyPowerIsUsed", RpcTarget.All, true);
        photonView.RPC("SendResetSpeciallyIsLauch", RpcTarget.All );
        DisplayLineToShot(false);
    }
    [PunRPC]
    public  void SetIsLaunch(bool isLaunch)
    {
        this.isLaunch = isLaunch;
    }

    [PunRPC]
    public void SendDisplayAxForAllPlayer( bool display)
    {
        DiplayAxForAllPlayer(display);
        DisplayHeartsFoAllPlayer(display);
    }

    [PunRPC]
    public void SendSpeciallyPowerIsUsed(bool speciallyPowerIsUsed)
    {
        gameManager.GetRoomOfBoss().GetComponent<Hexagone>().Room.speciallyPowerIsUsed = speciallyPowerIsUsed;
    }
   

    [PunRPC]
    public void SendResetSpeciallyIsLauch()
    {
        gameManager.speciallyIsLaunch = false;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(true);
        }
    }

}
