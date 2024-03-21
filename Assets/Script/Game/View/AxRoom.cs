using Luminosity.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxRoom : TrialsRoom
{
    public GameManager gameManager;
    public bool isPreparedToLauch = false;
    public Vector3 currentDirection = new Vector3(0,0,0);
    public bool isLaunch = false;
    public bool canShoot = true;

    public bool beforeLastDisconnect = false;

    public GameObject prefabAx;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.speciallyIsLaunch)
        {
            DisplayLineToShot(false);
            return;
        }
            
        if (!isLaunch)
            return;
        if (Input.GetMouseButton(0) && !isPreparedToLauch && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isTouchInTrial)
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
            }
        }

        if(gameManager.GetPlayerMineGO().transform.Find("LineForAx").localPosition.x > 0)
        {
            gameManager.GetPlayerMineGO().transform.Find("LineForAx").localScale = new Vector3(-0.9f, 0.77f);
        }
        else
        {
            gameManager.GetPlayerMineGO().transform.Find("LineForAx").localScale = new Vector3(0.9f, 0.77f);
        }
       
    }
    public void LaunchAxRoom()
    {
        StartCoroutine(LaunchAxRoomAfterTeleportation());
    }
    public IEnumerator LaunchAxRoomAfterTeleportation()
    {
        gameManager.ActivateCollisionTPOfAllDoor(false);
        gameManager.CloseDoorWhenVote(true);
        gameManager.ui_Manager.DisplayTrapPowerButtonDesactivate(true);
        gameManager.ui_Manager.DisplayObjectPowerButtonDesactivate(true);
        gameManagerParent.DisplayTorchBarre(false);
        gameManagerParent.ui_Manager.DisplayInteractionObject(false);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendWantToChangeBossFalse();
        gameManager.PauseTimerFroce(true);
        yield return new WaitForSeconds(2);
        StartCoroutine(TimerEndNotWinner(75));
        gameManager.ui_Manager.LaunchFightMusic();
        DiplayAxForAllPlayer(true);
        DisplayHeartsFoAllPlayer(true);
        gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(false);
        isLaunch = true;
        gameManager.speciallyIsLaunch = true;
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(false);
       
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            SendObstalceGroup();
        }
        
    }

    /*    public IEnumerator CanShootCoroutine()
        {
            yield return new WaitForSeconds(2);
            canShoot = true;
        }*/



    public void DiplayAxForAllPlayer(bool display)
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in listPlayer)
        {
            player.transform.Find("Skins").GetChild(player.GetComponent<PlayerGO>().indexSkin).Find("Ax").gameObject.SetActive(display);
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
        gameManager.GetPlayerMineGO().transform.Find("LineForAx").gameObject.SetActive(display);
    }
    public void UpdatePositionAndRotationOfLineByMouse()
    {
        Vector3 wPos = Input.mousePosition;
        wPos.z = gameManager.GetPlayerMineGO().transform.position.z - Camera.main.transform.position.z;
        wPos = Camera.main.ScreenToWorldPoint(wPos);
        Vector3 direction = wPos - gameManager.GetPlayerMineGO().transform.position;
        float radius = 1;
        direction = Vector3.ClampMagnitude(direction, radius);
        gameManager.GetPlayerMineGO().transform.Find("LineForAx").position = (gameManager.GetPlayerMineGO().transform.position + direction) ;
        currentDirection = direction;
    }

    [PunRPC]
    public void ShotAxToDirection( float positionX, float positionY, float directionX, float directionY , int indexPlayer)
    {
        GameObject newAx = GameObject.Instantiate(prefabAx, new Vector3(positionX, positionY), Quaternion.identity);
        newAx.GetComponent<Ax>().SetLancher(indexPlayer);
        newAx.GetComponent<Ax>().SetSpeedAndDirection(5, directionX, directionY);
        newAx.GetComponent<Ax>().player = gameManager.GetPlayerMineGO().GetComponent<PlayerGO>();
        if(gameManager.GetPlayerMineGO().transform.Find("LineForAx").position.x > 0)
            newAx.transform.localScale = new Vector3(Mathf.Sign(gameManager.GetPlayerMineGO().transform.Find("LineForAx").position.x ) * newAx.transform.localScale.x, newAx.transform.localScale.y);
        gameManager.ui_Manager.axeLaunch.Play();
        if (newAx.GetComponent<Ax>().GetNumberLastPlayer() == 2)
        {
            newAx.GetComponent<Ax>().SetBounds(6);
        }
    }

    public void SendShotAxToDirection()
    {
        Vector3 positionPlayer = gameManager.GetPlayerMineGO().transform.position;
        int indexPlayer = gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID;
        photonView.RPC("ShotAxToDirection", RpcTarget.All , positionPlayer.x , positionPlayer.y, currentDirection.x, currentDirection.y , indexPlayer);
    }

    public void DesactivateRoomChild()
    {
        DestoyAxs();
        photonView.RPC("SendDisplayAxForAllPlayer", RpcTarget.All, false);
        photonView.RPC("SetIsLaunch", RpcTarget.All, false);
        photonView.RPC("SendStopTimerAndSoundChrono", RpcTarget.All);
        DisplayLineToShot(false);
        DisplayTimerAllPlayer(false);
        gameManager.ui_Manager.soundChrono2.Stop();

    }
    [PunRPC]
    public void SendStopTimerAndSoundChrono()
    {
        DisplayTimerAllPlayer(false);
        gameManager.ui_Manager.soundChrono2.Stop();
    }

    public void DestoyAxs()
    {
        GameObject[] listAx = GameObject.FindGameObjectsWithTag("Ax");
        foreach (GameObject ax in listAx)
        {
            if (ax.GetComponent<PhotonView>().IsMine)
                PhotonNetwork.Destroy(ax);
        }
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
        StartCoroutine(ResetLineToShotCoroutine());
        SendResetObstacle();
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
        gameManager.ActivateCollisionTPOfAllDoor(true);
        gameManager.gameManagerNetwork.DisplayLightAllAvailableDoorN2(true);
        gameManager.CloseDoorWhenVote(false);
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBoss)
        {
            gameManager.GetPlayerMineGO().GetComponent<PlayerNetwork>().SendDisplayCrown(true);
        }
    }

    public IEnumerator ResetLineToShotCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.GetPlayerMineGO().transform.Find("LineForAx").gameObject.SetActive(false);
    }

    public IEnumerator DesactivateRoomCoroutine()
    {
        yield return new WaitForSeconds(2);
        DesactivateRoomChild();
    }

    public void Victory()
    {
        if (LastPlayerDoesNotExist())
        {
            gameManager.RandomWinFireball("AxeRoom");
            DesactivateRoomChild();
            gameManager.ui_Manager.soundChrono2.Stop();
        }
        if (TestLastPlayer())
        {
            GetAward(GetLastPlayer().GetComponent<PhotonView>().ViewID);
            DesactivateRoom();
            DesactivateRoomChild();
            DisplayTimerAllPlayer(false);
           gameManager.ui_Manager.soundChrono2.Stop();
        }

    }

    public bool LastPlayerDoesNotExist()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice)
            {
                counter++;
            }
        }
        if (counter == listPlayer.Length)
            return true;
        return false;
    }

    public bool TestLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in listPlayer)
        {
            if (player.GetComponent<PlayerGO>().isTouchInTrial || !gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID)
                    || player.GetComponent<PlayerGO>().isSacrifice || player.GetComponent<PlayerGO>().isInJail)
            {
                counter++;
            }
        }
        if (counter == (listPlayer.Length - 1))
            return true;
        return false;
    }

    public GameObject GetLastPlayer()
    {
        GameObject[] listPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in listPlayer)
        {
            if (!gameManager.SamePositionAtBossWithIndex(player.GetComponent<PhotonView>().ViewID))
                continue;
            if (player.GetComponent<PlayerGO>().isSacrifice)
                continue;
            if (player.GetComponent<PlayerGO>().isInJail)
                continue;
            if (!player.GetComponent<PlayerGO>().isTouchInTrial)
                return player;
        }
        Debug.Log("return null");
        return null;
    }
    public IEnumerator TimerEndNotWinner(int secondes)
    {
        StartCoroutine(DisplayTimerEnd(secondes - 15));
        yield return new WaitForSeconds(secondes);
        gameManager.ui_Manager.soundChrono2.Stop();
        if (isLaunch)
        {
            DestoyAxs();
            SendDisplayAxForAllPlayer(false);
            SetIsLaunch(false);
            DesactivateRoom();
            ReactivateCurrentRoom();
            gameManager.ui_Manager.DisplayLeverVoteDoor(true);
            gameManager.speciallyIsLaunch = false;
            DisplayTimerAllPlayer(false);
            gameManager.ui_Manager.soundChrono2.Stop();
        }
    }


    public IEnumerator DisplayTimerEnd(int seconde)
    {
        yield return new WaitForSeconds(seconde);
        if (isLaunch)
        {
            DisplayTimerAllPlayer(true);
            gameManager.ui_Manager.soundChrono2.Play();
            gameManager.ui_Manager.musicFight.Stop();
        }

    }

    public void DisplayTimerAllPlayer(bool display)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ((player.GetComponent<PlayerGO>().isTouchInTrial || player.GetComponent<PlayerGO>().isSacrifice) && display)
                continue;
            player.transform.Find("Timer").gameObject.SetActive(display);
            if (display)
                player.transform.Find("Timer").Find("CanvasTimer").Find("Timer").GetComponent<TimerDisplay>().timeLeft = 15;
            player.transform.Find("Timer").Find("CanvasTimer").Find("Timer").GetComponent<TimerDisplay>().timerLaunch = display;
        }
    }
}
