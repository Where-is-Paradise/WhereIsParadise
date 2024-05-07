using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DisplayErrorCouroutine(string objectName)
    {
        this.transform.Find(objectName).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        this.transform.Find(objectName).gameObject.SetActive(false);
    }

    public void DisplayErrorBlueTorchNotAvaible()
    {
        if(!this.transform.Find("BlueTocheNotAvailable").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("BlueTocheNotAvailable"));
    }
    
    public void DisplayHeHasAlreadyTorch()
    {
        if (!this.transform.Find("HeHasAlreadyTorch").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("HeHasAlreadyTorch"));
    }

    public void DisplayDoTrialBeforeUsedBlueTorch()
    {
        if (!this.transform.Find("DoTrialBeforeUsedBlueTorch").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("DoTrialBeforeUsedBlueTorch"));
    }

    public void DisplayOpenDoorBeforeChangeBoss()
    {
        if (!this.transform.Find("OpenDoorBeforeChangeBoss").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("OpenDoorBeforeChangeBoss"));
    }

    public void DisplaySoulMustBeInCircle()
    {
        if (!this.transform.Find("SoulMustBeInCircle").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("SoulMustBeInCircle"));
    }

    public void AllDoorAreAlreadyOpen()
    {
        if (!this.transform.Find("CannotOpenDoor").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("CannotOpenDoor"));
    }

    public void YouHaveToWait()
    {
        if (!this.transform.Find("YouHaveToWait").gameObject.activeSelf)
            StartCoroutine(DisplayErrorCouroutine("YouHaveToWait"));
    }


}
