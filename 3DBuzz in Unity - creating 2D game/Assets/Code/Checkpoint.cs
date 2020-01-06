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
        StartCoroutine(PlayerHitCheckpointCo(LevelManager.Instance.CurrentTimeBonus));
    }

    private IEnumerator PlayerHitCheckpointCo (int bonus)
    {
        FloatingText.Show("CheckPoint!", "CheckPointText", new CenteredTextPositioner(.5f));
        yield return new WaitForSeconds(.5f);
        FloatingText.Show(string.Format("+{0} time bonus!",bonus), "CheckPointText", new CenteredTextPositioner(.5f));
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
