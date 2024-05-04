using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display_Tranparency : MonoBehaviour
{

    public float timer = 0;
    public SpriteRenderer sprite;
    public bool display;
    public float speed;
    public float max  = 1; 

    public bool launchTransition = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (launchTransition)
        {
            SetColorImage(timer);
            if (timer < max)
            {
                timer += (Time.deltaTime / speed);
            }
        }
      
    }

    public void SetColorImage(float t)
    {
        float r = sprite.GetComponent<SpriteRenderer>().color.r;
        float g = sprite.GetComponent<SpriteRenderer>().color.g;
        float b = sprite.GetComponent<SpriteRenderer>().color.b;

        sprite.GetComponent<SpriteRenderer>().color = new Color(r, g, b, t);
    }

    public void LaunchTransitionUnDisplayToDisplay(float speed)
    {


        timer = 0;
        display = true;
        this.speed = speed;
        launchTransition = true;
        max = 1;
    }

    public void LaunchTransitionUnDisplayToDisplay(float speed, float max)
    {
        timer = 0;
        display = true;
        this.speed = speed;
        launchTransition = true;
        this.max = max;

    }

    public void DesactivateTransition()
    {
        timer = 0;
        SetColorImage(0);
        launchTransition = false;
    }
    public void DisplayWithoutTransition()
    {
        SetColorImage(1);
    }
}
