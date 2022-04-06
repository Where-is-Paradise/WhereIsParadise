using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public bool  ActivatelimiteFPS;
    public int target = 30;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }

    void Update()
    {
        if (ActivatelimiteFPS && Application.targetFrameRate != target)
            Application.targetFrameRate = target;
    }
}
