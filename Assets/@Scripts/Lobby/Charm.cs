using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 부적 아이템 컨트롤러
/// </summary>
public class Charm : MonoBehaviour
{
    [Header("부적 UI")]
    [SerializeField] private Button m_charmButton;
    [SerializeField] private Image m_charmIcon;
    [SerializeField] private TextMeshProUGUI m_charmName;
    [SerializeField] private TextMeshProUGUI m_charmDescription;

    private CharmData m_charmData;

    private void Start()
    {
        InitializeCharm();
    }

    /// <summary>
    /// 부적 초기화
    /// </summary>
    private void InitializeCharm()
    {
        m_charmButton?.onClick.AddListener(OnCharmClicked);
    }

    /// <summary>
    /// 부적 데이터 설정
    /// </summary>
    public void SetCharmData(CharmData charmData)
    {
        m_charmData = charmData;
        UpdateCharmDisplay();
    }

    /// <summary>
    /// 부적 표시 업데이트
    /// </summary>
    private void UpdateCharmDisplay()
    {
        if (m_charmName != null)
        {
            m_charmName.text = GenerateCharmName();
        }

        if (m_charmDescription != null)
        {
            m_charmDescription.text = GenerateCharmDescription();
        }
    }

    /// <summary>
    /// 부적 이름 생성
    /// </summary>
    private string GenerateCharmName()
    {
        string[] charmNames = { "신비한 부적", "마법의 부적", "고대의 부적", "행운의 부적", "강력한 부적" };
        return charmNames[Random.Range(0, charmNames.Length)];
    }

    /// <summary>
    /// 부적 설명 생성
    /// </summary>
    private string GenerateCharmDescription()
    {
        string description = "";
        
        if (m_charmData.speedModifier != 0)
        {
            description += $"속도 {m_charmData.speedModifier:+0;-0} ";
        }
        if (m_charmData.accelerationModifier != 0)
        {
            description += $"가속도 {m_charmData.accelerationModifier:+0;-0} ";
        }
        if (m_charmData.healthModifier != 0)
        {
            description += $"체력 {m_charmData.healthModifier:+0;-0} ";
        }
        if (m_charmData.intelligenceModifier != 0)
        {
            description += $"지능 {m_charmData.intelligenceModifier:+0;-0} ";
        }
        if (m_charmData.strengthModifier != 0)
        {
            description += $"힘 {m_charmData.strengthModifier:+0;-0} ";
        }

        return description.Trim();
    }

    /// <summary>
    /// 부적 클릭 처리
    /// </summary>
    private void OnCharmClicked()
    {
        LobbyManager.Instance?.OnCharmSelected(m_charmData);
    }
}
