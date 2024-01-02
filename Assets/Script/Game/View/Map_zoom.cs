using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_zoom : MonoBehaviour
{

    private Vector3 touchStart;
    private float zoomOutMin = 2f;
    private float zoomOutMax = 5.5f;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            //Zoom(difference * 0.005f);

        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized - (touchStart.normalized - Vector3.zero);
            CanContinueToTransform(direction * 7);
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
/*        if (!(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell))
        {
            Zoom(InputManager.GetAxis("Mouse ScrollWheel") * 0.2f);
        }*/
        
    }

    public void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }


    public void CanContinueToTransform(Vector3 direction)
    {
        if (transform.position.x < -30)
        {
            if(direction.x < 0)
            {
                return;
            }
            
        }
        if(transform.position.x > 15)
        {
            if (direction.x > 0)
            {
                return;
            }
        }

        if (transform.position.y < -10)
        {
            if (direction.y < 0)
            {
                return;
            }

        }
        if (transform.position.y > 35)
        {
            if (direction.y > 0)
            {
                return;
            }
        }



        transform.position = new Vector3(transform.position.x + direction.x  ,
                                        transform.position.y + direction.y , -3);

    }
}
