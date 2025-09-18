using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 고양이 선택 패널 컨트롤러
/// </summary>
public class CatPanelController : MonoBehaviour
{
    [Header("고양이 버튼들")]
    [SerializeField] private Button[] m_catButtons;
    [SerializeField] private Image[] m_catImages;
    [SerializeField] private TextMeshProUGUI[] m_catNames;

    [Header("고양이 데이터")]
    [SerializeField] private CatStats[] m_availableCats;

    private void Start()
    {
        InitializeCatButtons();
    }

    /// <summary>
    /// 고양이 버튼들 초기화
    /// </summary>
    private void InitializeCatButtons()
    {
        if (m_availableCats == null || m_availableCats.Length == 0)
        {
            Debug.LogError("사용 가능한 고양이 데이터가 설정되지 않았습니다!");
            return;
        }

        for (int i = 0; i < m_catButtons.Length && i < m_availableCats.Length; i++)
        {
            int catIndex = i; // 클로저를 위한 로컬 변수
            
            // 버튼 이벤트 연결
            m_catButtons[i].onClick.RemoveAllListeners();
            m_catButtons[i].onClick.AddListener(() => OnCatButtonClicked(catIndex));

            // 고양이 정보 설정
            if (m_catImages[i] != null && m_availableCats[i].CatSprite != null)
            {
                m_catImages[i].sprite = m_availableCats[i].CatSprite;
            }

            if (m_catNames[i] != null)
            {
                m_catNames[i].text = m_availableCats[i].CatName;
            }
        }
    }

    /// <summary>
    /// 고양이 버튼 클릭 처리
    /// </summary>
    private void OnCatButtonClicked(int catIndex)
    {
        if (catIndex >= 0 && catIndex < m_availableCats.Length)
        {
            LobbyManager.Instance?.OnCatSelected(m_availableCats[catIndex]);
        }
    }
}
