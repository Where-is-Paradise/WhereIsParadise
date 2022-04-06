using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CallBackNetwork : MonoBehaviourPunCallbacks
{

    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        Room room = PhotonNetwork.CurrentRoom;
        room.IsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    public override void OnDisconnected(DisconnectCause cause)
    {
        ui_management.DisplayErrorPanel(message);
    }*/

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GameObject player = gameManager.GetPlayerUserId(otherPlayer.UserId);
        SettingGameAfterPlayerDisconnect(player);
    }
    
    public void SettingGameAfterPlayerDisconnect(GameObject player)
    {
        if (!player)
        {
            return;
        }
        player.SetActive(false);
        gameManager.UpdateListPlayerGO();
        gameManager.SetTABToList(gameManager.listPlayerTab, gameManager.listPlayer);
        //gameManager.game.NumberExpeditionAvailable(gameManager.setting.LIMITED_TORCH, 0);
        if (player.GetComponent<PlayerGO>().isBoss)
        {
            gameManager.ChangeBoss();
        }
    }

    public void QuitLobby()
    {
        
        PhotonNetwork.LeaveRoom();
        //SceneManager.LoadScene("Menu");
    }


    public override void OnLeftRoom()
    {
        foreach (GameObject objectDonDesroy in this.gameObject.scene.GetRootGameObjects())
        {
            Destroy(objectDonDesroy);
        }
        Destroy(GameObject.Find("Setting"));
        Destroy(GameObject.Find("Input Manager"));

        SceneManager.LoadScene("Menu");

        base.OnLeftRoom();
    }




    public override void OnJoinedRoom()
    {
        PhotonNetwork.LeaveRoom();
    }





}
