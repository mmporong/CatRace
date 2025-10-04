using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 부적 패널 컨트롤러
/// </summary>
public class CharmPanelController : MonoBehaviour
{
    [Header("부적 버튼들")]
    [SerializeField] private Button[] m_charmButtons;
    [SerializeField] private Image[] m_charmImages;
    [SerializeField] private TextMeshProUGUI[] m_charmNames;
    [SerializeField] private TextMeshProUGUI[] m_charmDescriptions;

    [Header("부적 스프라이트")]
    [SerializeField] private Sprite[] m_charmSprites;

    private CharmData[] m_availableCharms = new CharmData[3];

    private void Start()
    {
        InitializeCharmButtons();
    }

    /// <summary>
    /// 부적 버튼들 초기화
    /// </summary>
    private void InitializeCharmButtons()
    {
        Debug.Log("부적 버튼 초기화 시작");
        
        // 랜덤 부적 데이터 생성
        GenerateRandomCharms();

        if (m_charmButtons == null || m_charmButtons.Length == 0)
        {
            Debug.LogError("부적 버튼이 설정되지 않았습니다!");
            return;
        }

        for (int i = 0; i < m_charmButtons.Length && i < m_availableCharms.Length; i++)
        {
            int charmIndex = i; // 클로저를 위한 로컬 변수
            
            Debug.Log($"부적 {i}번 버튼 설정 중...");
            
            // 버튼 이벤트 연결
            m_charmButtons[i].onClick.RemoveAllListeners();
            m_charmButtons[i].onClick.AddListener(() => OnCharmButtonClicked(charmIndex));

            // 부적 정보 설정
            if (m_charmImages[i] != null)
            {
                m_charmImages[i].sprite = GenerateCharmSprite();
            }

            if (m_charmNames[i] != null)
            {
                m_charmNames[i].text = GenerateCharmName();
            }

            if (m_charmDescriptions[i] != null)
            {
                m_charmDescriptions[i].text = GenerateCharmDescription(m_availableCharms[i]);
            }
        }
        
        Debug.Log("부적 버튼 초기화 완료");
    }

    /// <summary>
    /// 부적 버튼 클릭 처리
    /// </summary>
    private void OnCharmButtonClicked(int charmIndex)
    {
        Debug.Log($"부적 버튼 클릭됨: {charmIndex}");
        
        if (charmIndex >= 0 && charmIndex < m_availableCharms.Length)
        {
            Debug.Log($"부적 데이터 전송: {m_availableCharms[charmIndex]}");
            LobbyManager.Instance?.OnCharmSelected(m_availableCharms[charmIndex]);
        }
        else
        {
            Debug.LogError($"잘못된 부적 인덱스: {charmIndex}");
        }
    }

    /// <summary>
    /// 랜덤 부적들 생성
    /// </summary>
    private void GenerateRandomCharms()
    {
        for (int i = 0; i < m_availableCharms.Length; i++)
        {
            m_availableCharms[i] = CreateRandomCharmData();
        }
    }

    /// <summary>
    /// 랜덤 부적 데이터 생성
    /// </summary>
    private CharmData CreateRandomCharmData()
    {
        var charmData = new CharmData();
        
        // 2개의 스탯을 랜덤하게 선택
        var statTypes = new List<StatType> { StatType.Speed, StatType.Acceleration, StatType.Health, StatType.Intelligence, StatType.Strength };
        var selectedStats = new List<StatType>();
        
        // 2개 스탯 랜덤 선택
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, statTypes.Count);
            selectedStats.Add(statTypes[randomIndex]);
            statTypes.RemoveAt(randomIndex);
        }

        // 각 스탯에 -4~4 랜덤 값 할당
        foreach (var statType in selectedStats)
        {
            int value = Random.Range(-4, 5);
            switch (statType)
            {
                case StatType.Speed:
                    charmData.speedModifier = value;
                    break;
                case StatType.Acceleration:
                    charmData.accelerationModifier = value;
                    break;
                case StatType.Health:
                    charmData.healthModifier = value;
                    break;
                case StatType.Intelligence:
                    charmData.intelligenceModifier = value;
                    break;
                case StatType.Strength:
                    charmData.strengthModifier = value;
                    break;
            }
        }

        return charmData;
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
    private string GenerateCharmDescription(CharmData charmData)
    {
        string description = "";
        
        if (charmData.speedModifier != 0)
        {
            description += $"속도 {charmData.speedModifier:+0;-0} ";
        }
        if (charmData.accelerationModifier != 0)
        {
            description += $"가속도 {charmData.accelerationModifier:+0;-0} ";
        }
        if (charmData.healthModifier != 0)
        {
            description += $"체력 {charmData.healthModifier:+0;-0} ";
        }
        if (charmData.intelligenceModifier != 0)
        {
            description += $"지능 {charmData.intelligenceModifier:+0;-0} ";
        }
        if (charmData.strengthModifier != 0)
        {
            description += $"힘 {charmData.strengthModifier:+0;-0} ";
        }

        return description.Trim();
    }

    /// <summary>
    /// 부적 스프라이트 생성
    /// </summary>
    private Sprite GenerateCharmSprite()
    {
        if (m_charmSprites == null || m_charmSprites.Length == 0)
        {
            return null;
        }
        
        return m_charmSprites[Random.Range(0, m_charmSprites.Length)];
    }
}
