using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<IPlayerRespawnListener> _listiners;


    public void Awake()
    {
        _listiners = new List<IPlayerRespawnListener>();
    }

    public void PlayerHitCheckpoint()
    {

    }

    private IEnumerator PlayerHitCheckpoint (int bonus)
    {
        yield break;
    }

    public void PlayerLeftCheckpoint()
    {

    }

    public void SpawnPlayer (Player player)
    {
        player.RespawnAt(transform);

        foreach (var listener in _listiners)
            listener.OnPlayerRespawnInThisCheckPoint(this, player);
    }

    public void AssigmObjectToCheckpoint(IPlayerRespawnListener listener)
    {
        _listiners.Add(listener);
    }

}
