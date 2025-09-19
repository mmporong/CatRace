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

    [Header("Race Progress")]
    [SerializeField] private List<RaceProgress> raceProgresses = new List<RaceProgress>();
    [SerializeField] private float progressUpdateInterval = 0.1f; // 진행도 업데이트 간격

    [Header("State")]
    [SerializeField] private RaceState currentState = RaceState.PreRace;
    [SerializeField] private float raceClock = 0f;

    // Events
    public event Action OnRacePrepared;
    public event Action<int> OnCountdownTick; // 남은 정수 초
    public event Action OnRaceStarted;
    public event Action OnRaceFinished;
    public event Action<RaceProgress> OnLapCompleted; // 랩 완료 이벤트
    public event Action<List<RaceProgress>> OnRankingUpdated; // 순위 업데이트 이벤트

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
    private void Start()
    {
        PrepareRace();
        StartCountdown();
    }
    private void Update()
    {
        if (currentState == RaceState.Racing)
        {
            raceClock += Time.deltaTime;
            UpdateRaceProgress();
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

        // 진행도 추적 초기화
        InitializeRaceProgress();

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
        // 3, 2, 1, GO! 순서로 표시
        for (int i = Mathf.FloorToInt(countdownSeconds); i > 0; i--)
        {
            OnCountdownTick?.Invoke(i);
            yield return new WaitForSeconds(1f);
        }
        
        // GO!
        OnCountdownTick?.Invoke(0);
        yield return new WaitForSeconds(0.5f); // GO! 표시 시간
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

    /// <summary>
    /// 레이스 진행도를 업데이트합니다
    /// </summary>
    private void UpdateRaceProgress()
    {
        if (trackManager == null) return;

        // 모든 고양이의 진행도 업데이트
        foreach (var progress in raceProgresses)
        {
            if (progress != null && !progress.hasFinished)
            {
                progress.UpdateProgress(trackManager, totalLaps);
            }
        }

        // 순위 업데이트
        UpdateRankings();

        // 레이스 완료 확인
        CheckRaceCompletion();
    }

    /// <summary>
    /// 순위를 업데이트합니다
    /// </summary>
    private void UpdateRankings()
    {
        // 진행도에 따라 정렬 (높은 순)
        var sortedProgresses = new List<RaceProgress>(raceProgresses);
        sortedProgresses.Sort((a, b) => 
        {
            // 먼저 완주 여부로 정렬
            if (a.hasFinished && !b.hasFinished) return -1;
            if (!a.hasFinished && b.hasFinished) return 1;
            
            // 완주하지 않은 경우 진행도로 정렬
            if (!a.hasFinished && !b.hasFinished)
            {
                return b.totalProgress.CompareTo(a.totalProgress);
            }
            
            // 둘 다 완주한 경우 완주 시간으로 정렬
            return a.totalTime.CompareTo(b.totalTime);
        });

        // 순위 설정
        for (int i = 0; i < sortedProgresses.Count; i++)
        {
            sortedProgresses[i].SetRank(i + 1);
        }

        // 순위 업데이트 이벤트 발생
        OnRankingUpdated?.Invoke(sortedProgresses);
    }

    /// <summary>
    /// 레이스 완료를 확인합니다
    /// </summary>
    private void CheckRaceCompletion()
    {
        bool allFinished = true;
        foreach (var progress in raceProgresses)
        {
            if (progress != null && !progress.hasFinished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished && currentState == RaceState.Racing)
        {
            FinishRace();
        }
    }

    /// <summary>
    /// 진행도 추적을 초기화합니다
    /// </summary>
    private void InitializeRaceProgress()
    {
        raceProgresses.Clear();
        
        for (int i = 0; i < racers.Count; i++)
        {
            if (racers[i] != null)
            {
                var progress = new RaceProgress(racers[i], i);
                raceProgresses.Add(progress);
            }
        }
    }

    /// <summary>
    /// 특정 고양이의 진행도를 가져옵니다
    /// </summary>
    public RaceProgress GetRaceProgress(CatAI catAI)
    {
        foreach (var progress in raceProgresses)
        {
            if (progress.catAI == catAI)
            {
                return progress;
            }
        }
        return null;
    }

    /// <summary>
    /// 현재 순위를 가져옵니다 (진행도 순)
    /// </summary>
    public List<RaceProgress> GetCurrentRankings()
    {
        var sortedProgresses = new List<RaceProgress>(raceProgresses);
        sortedProgresses.Sort((a, b) => 
        {
            if (a.hasFinished && !b.hasFinished) return -1;
            if (!a.hasFinished && b.hasFinished) return 1;
            if (!a.hasFinished && !b.hasFinished)
            {
                return b.totalProgress.CompareTo(a.totalProgress);
            }
            return a.totalTime.CompareTo(b.totalTime);
        });
        return sortedProgresses;
    }

    // Public getters
    public RaceState CurrentState => currentState;
    public float RaceClock => raceClock;
    public int TotalLaps => totalLaps;
    public IReadOnlyList<CatAI> Racers => racers;
    public IReadOnlyList<RaceProgress> RaceProgresses => raceProgresses;
}


