using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_hell_circie : MonoBehaviour
{

    public int speed = 50;

    public bool canStart = false;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LaunchAnimationRotation());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.eulerAngles.y);
        if (!canStart)
            return;

        if (this.transform.eulerAngles.y > 90)
            return;

   

        transform.eulerAngles += new Vector3(0, speed * Time.deltaTime, 0);
    }

    public IEnumerator LaunchAnimationRotation()
    {
        yield return new WaitForSeconds(2);
        canStart = true;
    }

}
