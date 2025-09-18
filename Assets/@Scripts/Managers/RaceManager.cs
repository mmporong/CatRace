using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레이스 라이프사이클을 관리하는 매니저
/// PreRace -> Countdown -> Racing -> Finished -> PostRace
/// </summary>
public class RaceManager : MonoBehaviour
{
    public enum RaceState
    {
        PreRace,
        Countdown,
        Racing,
        Finished,
        PostRace
    }

    [Header("Race Config")]
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private float countdownSeconds = 3f;
    [SerializeField] private Transform[] startGridPoints;

    [Header("References")]
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private List<CatAI> racers = new List<CatAI>();

    [Header("State")]
    [SerializeField] private RaceState currentState = RaceState.PreRace;
    [SerializeField] private float raceClock = 0f;

    // Events
    public event Action OnRacePrepared;
    public event Action<int> OnCountdownTick; // 남은 정수 초
    public event Action OnRaceStarted;
    public event Action OnRaceFinished;

    private Coroutine countdownRoutine;

    private void Awake()
    {
        if (trackManager == null)
        {
            trackManager = FindFirstObjectByType<TrackManager>();
        }
        if (racers.Count == 0)
        {
            racers.AddRange(FindObjectsByType<CatAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        }
    }

    private void Update()
    {
        if (currentState == RaceState.Racing)
        {
            raceClock += Time.deltaTime;
        }
    }

    [ContextMenu("Prepare Race")]
    public void PrepareRace()
    {
        currentState = RaceState.PreRace;
        raceClock = 0f;

        // 배치 및 고정
        for (int i = 0; i < racers.Count; i++)
        {
            var ai = racers[i];
            if (ai == null) continue;
            var t = ai.transform;
            if (startGridPoints != null && i < startGridPoints.Length && startGridPoints[i] != null)
            {
                t.position = startGridPoints[i].position;
                t.rotation = startGridPoints[i].rotation;
            }

            // 이동 정지
            var move = ai.GetComponent<CatMovement>();
            if (move != null)
            {
                move.Stop();
                move.ApplyStats();
            }

            // AI 입력 잠금(Countdown까지)
            ai.enabled = false;
        }

        OnRacePrepared?.Invoke();
    }

    [ContextMenu("Start Countdown")]
    public void StartCountdown()
    {
        if (currentState != RaceState.PreRace && currentState != RaceState.PostRace)
            return;

        currentState = RaceState.Countdown;
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }
        countdownRoutine = StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        float remaining = Mathf.Max(1f, countdownSeconds);
        int lastWhole = Mathf.CeilToInt(remaining);
        while (remaining > 0f)
        {
            int whole = Mathf.CeilToInt(remaining);
            if (whole != lastWhole)
            {
                lastWhole = whole;
                OnCountdownTick?.Invoke(whole);
            }
            remaining -= Time.deltaTime;
            yield return null;
        }

        // GO!
        OnCountdownTick?.Invoke(0);
        BeginRace();
    }

    [ContextMenu("Begin Race")] 
    public void BeginRace()
    {
        currentState = RaceState.Racing;
        raceClock = 0f;

        foreach (var ai in racers)
        {
            if (ai == null) continue;
            ai.enabled = true;
        }

        OnRaceStarted?.Invoke();
    }

    public void FinishRace()
    {
        if (currentState == RaceState.Finished) return;
        currentState = RaceState.Finished;

        // 이동 잠금
        foreach (var ai in racers)
        {
            if (ai == null) continue;
            var move = ai.GetComponent<CatMovement>();
            if (move != null) move.Stop();
            ai.enabled = false;
        }

        OnRaceFinished?.Invoke();
    }

    [ContextMenu("Restart Race")]
    public void RestartRace()
    {
        currentState = RaceState.PostRace;
        PrepareRace();
        StartCountdown();
    }

    // Public getters
    public RaceState CurrentState => currentState;
    public float RaceClock => raceClock;
    public int TotalLaps => totalLaps;
    public IReadOnlyList<CatAI> Racers => racers;
}


