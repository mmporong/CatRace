using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스탯 강화 패널 컨트롤러
/// </summary>
public class StatPanelController : MonoBehaviour
{
    [Header("스탯 버튼들")]
    [SerializeField] private Button m_speedButton;
    [SerializeField] private Button m_accelerationButton;
    [SerializeField] private Button m_healthButton;
    [SerializeField] private Button m_intelligenceButton;
    [SerializeField] private Button m_strengthButton;

    [Header("스탯 표시 UI")]
    [SerializeField] private TextMeshProUGUI m_speedText;
    [SerializeField] private TextMeshProUGUI m_accelerationText;
    [SerializeField] private TextMeshProUGUI m_healthText;
    [SerializeField] private TextMeshProUGUI m_intelligenceText;
    [SerializeField] private TextMeshProUGUI m_strengthText;

    [Header("강화 설정")]
    [SerializeField] private int m_enhancementValue = 5; // 스탯 강화 값

    private void Start()
    {
        InitializeStatButtons();
        UpdateStatDisplay();
    }

    /// <summary>
    /// 스탯 버튼들 초기화
    /// </summary>
    private void InitializeStatButtons()
    {
        m_speedButton?.onClick.AddListener(() => OnStatButtonClicked(StatType.Speed));
        m_accelerationButton?.onClick.AddListener(() => OnStatButtonClicked(StatType.Acceleration));
        m_healthButton?.onClick.AddListener(() => OnStatButtonClicked(StatType.Health));
        m_intelligenceButton?.onClick.AddListener(() => OnStatButtonClicked(StatType.Intelligence));
        m_strengthButton?.onClick.AddListener(() => OnStatButtonClicked(StatType.Strength));
    }

    /// <summary>
    /// 스탯 버튼 클릭 처리
    /// </summary>
    private void OnStatButtonClicked(StatType statType)
    {
        LobbyManager.Instance?.OnStatEnhanced(statType, m_enhancementValue);
    }

    /// <summary>
    /// 스탯 표시 업데이트
    /// </summary>
    private void UpdateStatDisplay()
    {
        var selectedCat = LobbyManager.Instance?.SelectedCat;
        if (selectedCat == null) return;

        m_speedText?.SetText($"속도: {selectedCat.Speed:F1}");
        m_accelerationText?.SetText($"가속도: {selectedCat.Acceleration:F1}");
        m_healthText?.SetText($"체력: {selectedCat.Health}");
        m_intelligenceText?.SetText($"지능: {selectedCat.Intelligence}");
        m_strengthText?.SetText($"힘: {selectedCat.Strength}");
    }
}
