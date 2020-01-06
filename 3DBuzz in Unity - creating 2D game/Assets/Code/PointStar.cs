﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStar : MonoBehaviour, IPlayerRespawnListener
{
    public GameObject Effect;
    public int PointsToAdd = 10;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        GameManager.Instance.AddPoints(PointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);

        gameObject.SetActive(false);

        FloatingText.Show(string.Format("+{0}!", PointsToAdd), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
    }

   public void OnPlayerRespawnInThisCheckPoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}
