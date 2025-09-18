using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 리소스 패널 컨트롤러
/// </summary>
public class ResourcePanelController : MonoBehaviour
{
    [Header("리소스 UI")]
    [SerializeField] private TextMeshProUGUI m_coinText;
    [SerializeField] private TextMeshProUGUI m_gemText;
    [SerializeField] private Button m_buyCoinButton;
    [SerializeField] private Button m_buyGemButton;

    [Header("리소스 데이터")]
    [SerializeField] private int m_currentCoins = 9999;
    [SerializeField] private int m_currentGems = 9999;

    private void Start()
    {
        InitializeResourcePanel();
        UpdateResourceDisplay();
    }

    /// <summary>
    /// 리소스 패널 초기화
    /// </summary>
    private void InitializeResourcePanel()
    {
        m_buyCoinButton?.onClick.AddListener(OnBuyCoinClicked);
        m_buyGemButton?.onClick.AddListener(OnBuyGemClicked);
    }

    /// <summary>
    /// 리소스 표시 업데이트
    /// </summary>
    private void UpdateResourceDisplay()
    {
        if (m_coinText != null)
        {
            m_coinText.text = m_currentCoins.ToString();
        }

        if (m_gemText != null)
        {
            m_gemText.text = m_currentGems.ToString();
        }
    }

    /// <summary>
    /// 코인 구매 버튼 클릭 처리
    /// </summary>
    private void OnBuyCoinClicked()
    {
        // 코인 구매 로직 (실제 구현에서는 결제 시스템 연동)
        m_currentCoins += 1000;
        UpdateResourceDisplay();
    }

    /// <summary>
    /// 젬 구매 버튼 클릭 처리
    /// </summary>
    private void OnBuyGemClicked()
    {
        // 젬 구매 로직 (실제 구현에서는 결제 시스템 연동)
        m_currentGems += 100;
        UpdateResourceDisplay();
    }
}
