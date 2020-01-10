using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    public string LevelName;

    [System.Obsolete]
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        LevelManager.Instance.GoTonextLevelCo(LevelName);
    }
}
