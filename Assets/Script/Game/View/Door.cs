using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{

    public UI_Manager ui_manager;
    public string doorName;
    public int index;
    public int nbVote = 0;
    public string letter;
    public bool letter_displayed = false;
    public GameObject player;
    public GameObject old_player;
    public int counterPlayerInDoorZone = 0;
    public GameManager gameManager;
    public Timer timer;
    public bool barricade = false;
    public bool isOpen = false;
    public bool iscurrentlyOpen = false;
    public bool isOpenForAll = false;
    public bool IsCloseNotPermantly = false;
    public bool closeForTimerExploration = false;
    public int counterPlayerCollisionSelected = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player)
        {
            if (player.GetComponent<PlayerGO>().isBoss &&  !player.GetComponent<PlayerGO>().hasWinFireBallRoom)
            {
                player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().enabled = false;
                player = null;
                counterPlayerInDoorZone = 0;
                letter_displayed = false;
            }
            if ((gameManager.expeditionHasproposed && timer.timerLaunch) || (gameManager.voteDoorHasProposed && timer.timerLaunch))
            {
                player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().enabled = false;
                this.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
                this.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.25f);
                //this.transform.transform.GetChild(4).GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                old_player = player;
                player = null;
            }

        }

        if (counterPlayerInDoorZone < 0)
        {
            counterPlayerInDoorZone = 0;
        }

        if (isOpenForAll)
        {
            if (IsCloseNotPermantly)
            {
                if (gameManager.expeditionHasproposed)
                {
                    if (!gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().haveToGoToExpedition)
                    {
                        Physics2D.IgnoreCollision(gameManager.GetPlayerMineGO().GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                    }
                    else
                    {
                        if (old_player && gameManager.GetPlayerMineGO().GetComponent<PhotonView>().ViewID != old_player.GetComponent<PhotonView>().ViewID)
                        {
                            Physics2D.IgnoreCollision(gameManager.GetPlayerMineGO().GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                        }
                    }
                }
                else
                {
                    Physics2D.IgnoreCollision(gameManager.GetPlayerMineGO().GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                }
            }
            else
            {
                old_player = null;
                player = null;
            }

        }

        if (nbVote < 0)
        {
            nbVote = 0;
        }


        if (barricade || isOpenForAll || isOpen || iscurrentlyOpen || gameManager.paradiseIsFind || gameManager.hellIsFind)
        {
            this.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().gameObject.SetActive(false);
        }
        else
        {
            this.transform.GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
        }
        //counterPlayerInDoorZone = gameManager.GetNumberOfPlayerSelected(this.index);

        if (isOpenForAll)
        {
            this.transform.Find("CollisionPowerImpostor").GetComponent<CircleCollider2D>().enabled = false;
        }
        else
        {
            this.transform.Find("CollisionPowerImpostor").GetComponent<CircleCollider2D>().enabled = true;
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine && !gameManager.paradiseIsFind && !gameManager.hellIsFind)
        {

            if (gameManager.expeditionHasproposed && old_player)
            {
                if (collision.GetComponent<PhotonView>().ViewID == old_player.GetComponent<PhotonView>().ViewID)
                {
                    if (transform.GetChild(6).GetComponent<Animator>().GetBool("open"))
                    {
                        Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>());
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                    }
                }
            }
            else
            {
                if (isOpenForAll && !collision.GetComponent<PlayerGO>().haveToGoToExpedition && !gameManager.voteDoorHasProposed && !IsCloseNotPermantly)
                {

                    Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>());
                }
                else
                {
                    Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                }
            }
        }
        if (collision.tag == "CollisionPlayer" && collision.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            GameObject player = collision.transform.parent.gameObject;
            if ((!collision.transform.parent.GetComponent<PlayerGO>().isBoss || collision.transform.parent.GetComponent<PlayerGO>().hasWinFireBallRoom) && !player.GetComponent<PlayerGO>().gameManager.expeditionHasproposed && !barricade)
            {
                if (counterPlayerInDoorZone == 0)
                    collision.transform.parent.GetComponent<PlayerNetwork>().SendFirstAtDoorToExploration(true);
                gameManager.gameManagerNetwork.SendCollisionExpeditionLetter(player.GetComponent<PhotonView>().ViewID, this.index, true , counterPlayerInDoorZone);
            }
        }
        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine && !gameManager.paradiseIsFind && !gameManager.hellIsFind)
        {

            if (gameManager.expeditionHasproposed && old_player )
            {
                if (collision.GetComponent<PhotonView>().ViewID == old_player.GetComponent<PhotonView>().ViewID)
                {
                    if (transform.GetChild(6).GetComponent<Animator>().GetBool("open"))
                    {

                        Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>());
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);
                    }
                }
            }
            else
            {
                if (isOpenForAll && !collision.GetComponent<PlayerGO>().haveToGoToExpedition && !gameManager.voteDoorHasProposed && !IsCloseNotPermantly)
                {
                    Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>());
                }
                else
                {
                    Physics2D.IgnoreCollision(collision.GetComponent<CapsuleCollider2D>(), GetComponent<BoxCollider2D>(), false);

                }
            }
        }


        if (collision.tag == "CollisionPlayer" && collision.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            if ((!collision.transform.parent.GetComponent<PlayerGO>().isBoss || collision.transform.parent.GetComponent<PlayerGO>().hasWinFireBallRoom))
            {
                if (!barricade && !isOpenForAll && !gameManager.voteDoorHasProposed)
                {
                    if (counterPlayerInDoorZone <= 1 )
                    {
                        if (collision.transform.parent.GetComponent<PlayerGO>().firstAtDoorToExploration)
                        {
                            gameManager.gameManagerNetwork.SendCollisionExpeditionLetterStay(collision.transform.parent.GetComponent<PhotonView>().ViewID, this.index);
                        }

                        if (counterPlayerInDoorZone == 1)
                        {
                            collision.transform.parent.GetComponent<PlayerGO>().firstAtDoorToExploration = false;
                            gameManager.gameManagerNetwork.SendCollisionExpeditionLetterStay(collision.transform.parent.GetComponent<PhotonView>().ViewID, this.index);
                        }
                       
                    }

                }
            }
        }
        

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "CollisionPlayer" && collision.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            if ((!collision.transform.parent.GetComponent<PlayerGO>().isBoss || collision.transform.parent.GetComponent<PlayerGO>().hasWinFireBallRoom) && !barricade && !gameManager.paradiseIsFind && !gameManager.hellIsFind  )
            {
                collision.transform.parent.GetComponent<PlayerNetwork>().SendFirstAtDoorToExploration(false);
                gameManager.gameManagerNetwork.SendCollisionExpeditionLetter(collision.transform.parent.GetComponent<PhotonView>().ViewID, this.index, false, counterPlayerInDoorZone);
            }

        }
    }

    public Room GetRoomBehind()
    {
        switch (this.index)
        {
            case 0:
                return gameManager.game.currentRoom.left_neighbour;
            case 1:
                return gameManager.game.currentRoom.up_Left_neighbour;
            case 2:
                return gameManager.game.currentRoom.up_Right_neighbour;
            case 3:
                return gameManager.game.currentRoom.right_neighbour;
            case 4:
                return gameManager.game.currentRoom.down_Right_neighbour;
            case 5:
                return gameManager.game.currentRoom.down_Left_neighbour;
        }
        return null; 
    }

    public bool RoomBehindHaslessDistance()
    {
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.left_neighbour.DistancePathFinding && !gameManager.game.currentRoom.left_neighbour.IsObstacle)
            return false;
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.up_Left_neighbour.DistancePathFinding && !gameManager.game.currentRoom.up_Left_neighbour.IsObstacle)
            return false;
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.up_Right_neighbour.DistancePathFinding && !gameManager.game.currentRoom.up_Right_neighbour.IsObstacle)
            return false;
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.right_neighbour.DistancePathFinding && !gameManager.game.currentRoom.right_neighbour.IsObstacle)
            return false;
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.down_Right_neighbour.DistancePathFinding && !gameManager.game.currentRoom.down_Right_neighbour.IsObstacle)
            return false;
        if (GetRoomBehind().DistancePathFinding > gameManager.game.currentRoom.down_Left_neighbour.DistancePathFinding && !gameManager.game.currentRoom.down_Left_neighbour.IsObstacle)
            return false;
        return true;
    }

    public void DisplayColorLightToExploration()
    {
        if (gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isBlind)
        {
            DisplayTransparencyLightExploration();
            return;
        }
        if (gameManager.game.currentRoom.IsFoggy)
        {
            this.transform.Find("LightsExploration").Find("TransparencyLight").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 45f / 255);
            DisplayTransparencyLightExploration();
            return;
        }
        if (GetRoomBehind().IsHell)
        {
            this.transform.Find("LightsExploration").Find("RedLight").gameObject.SetActive(true);
            StartCoroutine(Desactivatelight(false));
            return;
        }

        bool isBlue = false;
        if ((RoomBehindHaslessDistance() && !gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isCursed) ||
            (!RoomBehindHaslessDistance() && gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().isCursed))
        {
            this.transform.Find("LightsExploration").Find("GreenLight").gameObject.SetActive(true);
            isBlue = true;
        }
        else
        {
            this.transform.Find("LightsExploration").Find("RedLight").gameObject.SetActive(true);
        }
        StartCoroutine(Desactivatelight(isBlue));
    }
    public IEnumerator Desactivatelight(bool isBlue)
    {
        yield return new WaitForSeconds(6);
        if(isBlue)
            this.transform.Find("LightsExploration").Find("GreenEndAnimation").gameObject.SetActive(true);
        else
            this.transform.Find("LightsExploration").Find("RedEndAnimation").gameObject.SetActive(true);
        this.transform.Find("LightsExploration").Find("GreenLight").gameObject.SetActive(false);
        this.transform.Find("LightsExploration").Find("BlackLight").gameObject.SetActive(false);
        this.transform.Find("LightsExploration").Find("RedLight").gameObject.SetActive(false);
        //this.transform.Find("LightsExploration").Find("BlindLight").gameObject.SetActive(true);
    }

    public IEnumerator DesactiveAnimationEnd()
    {
        yield return new WaitForSeconds(0.7f);
    }

    public void DisplayTransparencyLightExploration()
    {
        if (gameManager.game.currentRoom.IsFoggy)
        {
            this.transform.Find("LightsExploration").Find("TransparencyLight").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 45f / 255);
        }
        this.transform.Find("LightsExploration").Find("TransparencyLight").gameObject.SetActive(true);
        StartCoroutine(DesactivateTransparencyAnimation());
    }

    public IEnumerator DesactivateTransparencyAnimation()
    {
        yield return new WaitForSeconds(6);
        this.transform.Find("LightsExploration").Find("TransparencyLight").gameObject.SetActive(false);
        this.transform.Find("LightsExploration").Find("TransparencyLightEnd").gameObject.SetActive(true);
        this.transform.Find("LightsExploration").Find("TransparencyLight").GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 156f / 255);
    }


}
