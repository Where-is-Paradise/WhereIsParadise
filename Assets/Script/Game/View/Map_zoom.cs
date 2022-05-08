using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_zoom : MonoBehaviour
{

    private Vector3 touchStart;
    private float zoomOutMin = 2.5f;
    private float zoomOutMax = 7f;
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
        if (Input.GetMouseButtonDown(0))
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

            Zoom(difference * 0.01f);

        }
        else if (Input.GetMouseButton(0))
        {

            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - (touchStart - Vector3.zero) ;
            //transform.position += direction * 0.1f;

            float x = Mathf.Clamp(transform.position.x + direction.x * 0.025f, -14,-5f);
            float y = Mathf.Clamp(transform.position.y + direction.y * 0.025f, 1.20f, 8);
            //transform.position = new Vector3(x, y, -3);
/*            transform.position = new Vector3(transform.position.x + direction.x * 0.05f,
                transform.position.y + direction.y * 0.05f, -3);*/
            CanContinueToTransform(direction);

        }
        if (!(gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionParadise || gameManager.GetPlayerMineGO().GetComponent<PlayerGO>().collisionHell))
        {
            Zoom(InputManager.GetAxis("Mouse ScrollWheel") * 0.4f);
        }
        
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



        transform.position = new Vector3(transform.position.x + direction.x * 0.05f,
        transform.position.y + direction.y * 0.05f, -3);

    }
}
