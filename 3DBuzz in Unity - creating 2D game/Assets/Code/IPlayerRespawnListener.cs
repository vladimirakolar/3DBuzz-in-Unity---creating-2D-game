using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerRespawnListener
{
    void OnPlayerRespawnInThisCheckPoint(Checkpoint checkpoint, Player player);
}
