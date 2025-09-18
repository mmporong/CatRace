using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부적 패널 컨트롤러
/// </summary>
public class CharmPanelController : MonoBehaviour
{
    [Header("부적 컨테이너")]
    [SerializeField] private Transform m_charmContainer;
    [SerializeField] private GameObject m_charmPrefab;

    private void Start()
    {
        GenerateCharmButtons();
    }

    /// <summary>
    /// 부적 버튼들 생성
    /// </summary>
    private void GenerateCharmButtons()
    {
        if (m_charmContainer == null || m_charmPrefab == null) return;

        // 기존 부적들 제거
        foreach (Transform child in m_charmContainer)
        {
            Destroy(child.gameObject);
        }

        // LobbyManager에서 부적 데이터 가져오기
        var lobbyManager = LobbyManager.Instance;
        if (lobbyManager == null) return;

        // 3개의 부적 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject charmObj = Instantiate(m_charmPrefab, m_charmContainer);
            Charm charmComponent = charmObj.GetComponent<Charm>();
            
            if (charmComponent != null)
            {
                // 랜덤 부적 데이터 생성
                var charmData = CreateRandomCharmData();
                charmComponent.SetCharmData(charmData);
            }
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
}
