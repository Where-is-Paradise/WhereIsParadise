using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCourb : MonoBehaviour
{
    public Vector2 playerTest = new Vector2(0, 0);
    public GameObject O1;
    public GameObject O2;

    public GameObject Square4;
    public GameObject Square5;
    public GameObject Square6;

    public float final_T = 0.5f;

    public GameObject deathGod;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangePostionPlayerInBezier());
       
    }

    // Update is called once per frame
    void Update()
    {
        playerTest = GameObject.Find("Player_GO_1").transform.position;
    }

    public IEnumerator ChangePostionPlayerInBezier()
    {
        yield return new WaitForSeconds(7);

        O1.transform.position = RandomPositionVector();
        O2.transform.position = GetSymetrie(O1.transform.position, this.transform.parent.position); ;

        Square4.transform.position = RandomPositionVector();
        Square5.transform.position = GetSymetrie(Square4.transform.position, O1.transform.position);
        Square6.transform.position  = GetSymetrie(Square5.transform.position, O2.transform.position);


        float tO1 = InverseLerp(Square4.transform.position, Square5.transform.position, O1.transform.position);
        float tO2 = InverseLerp(Square5.transform.position, Square6.transform.position, O2.transform.position);


        final_T = tO1;

        Vector2 lerpAB = Vector2.Lerp(Square4.transform.position, Square5.transform.position, tO1);
        Vector2 lerpBC = Vector2.Lerp(Square5.transform.position, Square6.transform.position, tO2);

        
        StartCoroutine(IncreaseT(lerpAB, lerpBC, final_T));

        StartCoroutine(ChangePostionPlayerInBezier());
    }

    public Vector2 RandomPositionVector()
    {
        return new Vector2(Random.Range(-7, 7), Random.Range(-4,4));
    }

    public Vector2 GetSymetrie(Vector2 reference, Vector2 middle)
    {
        return new Vector2(middle.x + (middle.x - reference.x), middle.y + (middle.y - reference.y));
    }

    public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
    {
        Vector2 AB = b - a;
        Vector2 AV = value - a;
        return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
    }


    public IEnumerator IncreaseT(Vector2 lerpAB, Vector2 lerpBC, float tO1)
    {
        yield return new WaitForSeconds(0.1f);
        deathGod = GameObject.Find("DeathNpc(Clone)");
        final_T += 0.01f;
        Vector2 lerpFinal = Vector2.Lerp(lerpAB, lerpBC, final_T);
        deathGod.GetComponent<Death_NPC>().transform.position = lerpFinal;
        StartCoroutine(IncreaseT(lerpAB, lerpBC, final_T));
    }
}
