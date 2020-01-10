using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; set; }

    public Player Player { get; private set; }
    public Cameracontroller Camera { get; private set; }
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }

    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference) * BonusSecondMultiplier;
        }
    }

    private List<Checkpoint> _checkpoints;
    private int _currentCheckpointIndex;
    private DateTime _started;
    private int _savedPoints;


    public Checkpoint DebugSpawn;
    public int BonusCutoffSeconds;
    public int BonusSecondMultiplier;

    public void Awake()
    {
        _savedPoints = GameManager.Instance.Points;
        Instance = this;
    }

    public void Start()
    {
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        _currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;

        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<Cameracontroller>();

        _started = DateTime.UtcNow;

        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listiner in listeners)
        {
            for (var i = _checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listiner).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;

                _checkpoints[i].AssigmObjectToCheckpoint(listiner);
                break;
            }
        }

#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);

#else
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#endif
    }

    private void Update()
    {
        var isLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
            if (isLastCheckpoint)
            return;

        var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;

        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
        _currentCheckpointIndex++;
        _checkpoints[_currentCheckpointIndex].PlayerHitCheckpoint();

        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;

    }

    [Obsolete]
    public void GoTonextLevelCo(string levelName)
    {
        StartCoroutine(GoToNextLevelCo(levelName));
    }

    private void StartCoroutine(IEnumerable enumerable)
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    private IEnumerable GoToNextLevelCo(string levelName)
    {
        Player.FinishLevel();
        GameManager.Instance.AddPoints(CurrentTimeBonus);

        FloatingText.Show("Level Complete!", "CheckPointText", new CenteredTextPositioner(.2f));
        yield return new WaitForSeconds(1);

        FloatingText.Show(string.Format("{0} points!", GameManager.Instance.Points), "CheckPointText", new CenteredTextPositioner(.1f));
        yield return new WaitForSeconds(5f);

        if (string.IsNullOrEmpty(levelName))
            Application.LoadLevel("StartScreen");
        else
            Application.LoadLevel(levelName);
    }

    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo()
    {
        Player.Kill();
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(2f);

        Camera.IsFollowing = true;

        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);

        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);
    }
}
