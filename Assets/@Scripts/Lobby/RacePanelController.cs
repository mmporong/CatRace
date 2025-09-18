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
            m_raceDescription.text = "고양이들과 함께하는 레이싱 대회에 참가하세요!";
        }
    }

    /// <summary>
    /// 레이스 시작 버튼 클릭 처리
    /// </summary>
    private void OnStartRaceClicked()
    {
        // 메인씬으로 이동
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
