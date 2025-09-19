using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 레이스 패널 컨트롤러
/// </summary>
public class RacePanelController : MonoBehaviour
{
    [Header("레이스 정보 UI")]
    [SerializeField] private TextMeshProUGUI m_raceTitle;
    [SerializeField] private TextMeshProUGUI m_raceDescription;
    [SerializeField] private Button m_startRaceButton;

    [Header("레이스 패널 버튼")]
    [SerializeField] private Button m_racePanelButton;

    private void Start()
    {
        InitializeRacePanel();
    }

    /// <summary>
    /// 레이스 패널 초기화
    /// </summary>
    private void InitializeRacePanel()
    {
        m_startRaceButton?.onClick.AddListener(OnStartRaceClicked);
        m_racePanelButton?.onClick.AddListener(OnRacePanelButtonClicked);
        UpdateRaceInfo();
    }

    /// <summary>
    /// 레이스 정보 업데이트
    /// </summary>
    private void UpdateRaceInfo()
    {
        if (m_raceTitle != null)
        {
            m_raceTitle.text = "츄르 그랑프리";
        }

        if (m_raceDescription != null)
        {
            m_raceDescription.text =
                                   "- 계절 : 봄\n" +
                                   "- 거리 : 단거리\n" +
                                   "- 날씨 : 비";
        }
    }

    /// <summary>
    /// 레이스 패널 버튼 클릭 처리
    /// </summary>
    private void OnRacePanelButtonClicked()
    {
        Debug.Log("레이스 패널 버튼 클릭됨!");

        // LobbyManager를 통해 레이스 패널 토글
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.ToggleRacePanel();
        }
        else
        {
            Debug.LogError("LobbyManager.Instance가 null입니다!");
        }
    }

    /// <summary>
    /// 레이스 시작 버튼 클릭 처리
    /// </summary>
    private void OnStartRaceClicked()
    {
        Debug.Log("레이스 시작 버튼 클릭됨!");

        // 레이스 패널 비활성화
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.HideRacePanel();
        }

        // LobbyManager를 통해 메인씬 로드
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.LoadMainScene();
        }
        else
        {
            Debug.LogError("LobbyManager.Instance가 null입니다!");
        }
    }
}
