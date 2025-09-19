using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 레이스 HUD UI를 관리하는 클래스
/// 카운트다운, 타이머, 랩 표시 등을 담당
/// </summary>
public class RaceHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI raceTimeText;
    [SerializeField] private TextMeshProUGUI totalLapsText;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject countdownPanel;

    [Header("Settings")]
    [SerializeField] private string countdownFormat = "{0}";
    [SerializeField] private string goText = "GO!";
    [SerializeField] private string timeFormat = "mm:ss.ff";
    [SerializeField] private string lapFormat = "Lap {0}/{1}";

    private RaceManager raceManager;
    private bool isInitialized = false;

    private void Awake()
    {
        // RaceManager 찾기
        raceManager = FindFirstObjectByType<RaceManager>();
        if (raceManager == null)
        {
            Debug.LogError("RaceHUD: RaceManager를 찾을 수 없습니다!");
            return;
        }

        // 이벤트 구독
        raceManager.OnRacePrepared += OnRacePrepared;
        raceManager.OnCountdownTick += OnCountdownTick;
        raceManager.OnRaceStarted += OnRaceStarted;
        raceManager.OnRaceFinished += OnRaceFinished;

        isInitialized = true;
    }

    private void OnDestroy()
    {
        if (raceManager != null)
        {
            raceManager.OnRacePrepared -= OnRacePrepared;
            raceManager.OnCountdownTick -= OnCountdownTick;
            raceManager.OnRaceStarted -= OnRaceStarted;
            raceManager.OnRaceFinished -= OnRaceFinished;
        }
    }

    private void Update()
    {
        if (!isInitialized || raceManager == null) return;

        // 레이스 중일 때만 타이머 업데이트
        if (raceManager.CurrentState == RaceManager.RaceState.Racing)
        {
            UpdateRaceTime();
            UpdateLapDisplay();
        }
    }

    private void OnRacePrepared()
    {
        // HUD 초기화
        if (hudPanel != null)
            hudPanel.SetActive(true);
        
        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        if (raceTimeText != null)
            raceTimeText.text = "00:00.00";

        if (totalLapsText != null)
            totalLapsText.text = string.Format(lapFormat, 0, raceManager.TotalLaps);
    }

    private void OnCountdownTick(int remainingSeconds)
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        if (countdownText != null)
        {
            if (remainingSeconds > 0)
            {
                countdownText.text = string.Format(countdownFormat, remainingSeconds);
            }
            else
            {
                countdownText.text = goText;
            }
        }
    }

    private void OnRaceStarted()
    {
        // 카운트다운 패널 숨기기
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
    }

    private void OnRaceFinished()
    {
        // HUD는 유지하되 최종 시간 표시
        Debug.Log("레이스 완료!");
    }

    private void UpdateRaceTime()
    {
        if (raceTimeText != null)
        {
            float time = raceManager.RaceClock;
            int minutes = Mathf.FloorToInt(time / 60f);
            float seconds = time % 60f;
            raceTimeText.text = string.Format("{0:00}:{1:00.00}", minutes, seconds);
        }
    }

    private void UpdateLapDisplay()
    {
        // 첫 번째 고양이의 진행도를 표시 (1위 고양이)
        var rankings = raceManager.GetCurrentRankings();
        if (rankings != null && rankings.Count > 0)
        {
            var leader = rankings[0];
            if (leader != null)
            {
                UpdateLap(leader.currentLap);
            }
        }
    }

    /// <summary>
    /// 현재 랩을 업데이트합니다
    /// </summary>
    /// <param name="currentLap">현재 랩</param>
    public void UpdateLap(int currentLap)
    {
        if (totalLapsText != null)
            totalLapsText.text = string.Format(lapFormat, currentLap, raceManager.TotalLaps);
    }

    /// <summary>
    /// HUD를 숨깁니다
    /// </summary>
    public void HideHUD()
    {
        if (hudPanel != null)
            hudPanel.SetActive(false);
    }

    /// <summary>
    /// HUD를 표시합니다
    /// </summary>
    public void ShowHUD()
    {
        if (hudPanel != null)
            hudPanel.SetActive(true);
    }

    /// <summary>
    /// 카운트다운 텍스트를 설정합니다
    /// </summary>
    /// <param name="text">표시할 텍스트</param>
    public void SetCountdownText(string text)
    {
        if (countdownText != null)
            countdownText.text = text;
    }

    /// <summary>
    /// 레이스 시간을 설정합니다
    /// </summary>
    /// <param name="time">레이스 시간</param>
    public void SetRaceTime(float time)
    {
        if (raceTimeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            float seconds = time % 60f;
            raceTimeText.text = string.Format("{0:00}:{1:00.00}", minutes, seconds);
        }
    }
}
