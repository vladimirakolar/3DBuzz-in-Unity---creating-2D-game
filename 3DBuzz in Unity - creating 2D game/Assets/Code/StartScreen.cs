using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public string FirstLevel;

    [System.Obsolete]
    public void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        GameManager.Instance.Reset();
        Application.LoadLevel(FirstLevel);
    }
}
