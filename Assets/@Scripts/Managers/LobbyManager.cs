using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 로비씬의 3페이즈 시스템을 관리하는 매니저
/// 페이즈 0: 고양이 선택, 페이즈 1: 스탯 강화, 페이즈 2: 부적 선택
/// </summary>
public class LobbyManager : MonoBehaviour
{
    [Header("패널 참조")]
    [SerializeField] private GameObject m_catPanel;
    [SerializeField] private GameObject m_statPanel;
    [SerializeField] private GameObject m_charmPanel;
    [SerializeField] private GameObject m_racePanel;
    [SerializeField] private GameObject m_resourcePanel;

    [Header("고양이 데이터")]
    [SerializeField] private CatStats[] m_availableCats;
    [SerializeField] private CatStats m_selectedCat;

    private int m_currentPhase = 0;
    private int m_selectedCatIndex = -1; // 선택된 고양이의 인덱스

    public static LobbyManager Instance { get; private set; }
    public CatStats SelectedCat => m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length ? m_availableCats[m_selectedCatIndex] : null;
    public int CurrentPhase => m_currentPhase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeLobby();
    }

    /// <summary>
    /// 로비 초기화
    /// </summary>
    private void InitializeLobby()
    {
        m_currentPhase = 0;
        
        // 레이스 패널 초기 상태를 비활성화로 설정
        m_racePanel?.SetActive(false);
        
        ShowPhasePanel();
    }

    /// <summary>
    /// 현재 페이즈에 맞는 패널 표시
    /// </summary>
    private void ShowPhasePanel()
    {
        // 모든 패널 비활성화 (레이스 패널 제외 - 독립적으로 관리)
        m_catPanel?.SetActive(false);
        m_statPanel?.SetActive(false);
        m_charmPanel?.SetActive(false);

        // 현재 페이즈에 맞는 패널 활성화
        switch (m_currentPhase)
        {
            case 0:
                m_catPanel?.SetActive(true);
                break;
            case 1:
                m_statPanel?.SetActive(true);
                break;
            case 2:
                m_charmPanel?.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 고양이 선택 완료 (인덱스로 선택)
    /// </summary>
    public void OnCatSelected(CatStats selectedCat)
    {
        // 선택된 고양이의 인덱스를 찾아서 저장
        for (int i = 0; i < m_availableCats.Length; i++)
        {
            if (m_availableCats[i] == selectedCat)
            {
                m_selectedCatIndex = i;
                break;
            }
        }
        
        Debug.Log($"고양이 선택됨: {selectedCat.CatName} (인덱스: {m_selectedCatIndex})");
        Debug.Log($"선택된 고양이 스탯: {selectedCat.ToString()}");
        
        // 다음 페이즈로 진행
        NextPhase();
    }

    /// <summary>
    /// 스탯 강화 완료
    /// </summary>
    public void OnStatEnhanced(StatType statType, int value)
    {
        if (m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length)
        {
            var selectedCat = m_availableCats[m_selectedCatIndex];
            selectedCat.ModifyStats(
                statType == StatType.Speed ? value : 0,
                statType == StatType.Acceleration ? value : 0,
                statType == StatType.Health ? value : 0,
                statType == StatType.Intelligence ? value : 0,
                statType == StatType.Strength ? value : 0
            );
            Debug.Log($"스탯 강화 완료: {selectedCat.CatName} - {statType} +{value}");
        }
        NextPhase();
    }

    /// <summary>
    /// 부적 선택 완료
    /// </summary>
    public void OnCharmSelected(CharmData charmData)
    {
        if (m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length)
        {
            var selectedCat = m_availableCats[m_selectedCatIndex];
            selectedCat.ModifyStats(
                charmData.speedModifier,
                charmData.accelerationModifier,
                charmData.healthModifier,
                charmData.intelligenceModifier,
                charmData.strengthModifier
            );
            Debug.Log($"부적 적용 완료: {selectedCat.CatName} - 속도({charmData.speedModifier:+0;-0}) 가속도({charmData.accelerationModifier:+0;-0}) 체력({charmData.healthModifier:+0;-0}) 지능({charmData.intelligenceModifier:+0;-0}) 힘({charmData.strengthModifier:+0;-0})");
        }
        SaveGameData();
        
        // 부적 선택 완료 후 레이스 패널 활성화
        Debug.Log("부적 선택 완료! 레이스 패널을 활성화합니다.");
        m_charmPanel?.SetActive(false);
        m_racePanel?.SetActive(true);
    }

    /// <summary>
    /// 다음 페이즈로 진행
    /// </summary>
    private void NextPhase()
    {
        m_currentPhase++;
        if (m_currentPhase > 2)
        {
            // 모든 페이즈 완료
            LoadMainScene();
        }
        else
        {
            ShowPhasePanel();
        }
    }


    /// <summary>
    /// 게임 데이터 저장 (PlayerPrefs 기반)
    /// </summary>
    private void SaveGameData()
    {
        if (m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length)
        {
            var selectedCat = m_availableCats[m_selectedCatIndex];
            
            // 저장 키 상수
            const string C_SELECTED_CAT_NAME = "SelectedCatName";
            const string C_CAT_SPEED = "CatSpeed";
            const string C_CAT_ACCELERATION = "CatAcceleration";
            const string C_CAT_HEALTH = "CatHealth";
            const string C_CAT_INTELLIGENCE = "CatIntelligence";
            const string C_CAT_STRENGTH = "CatStrength";

            PlayerPrefs.SetString(C_SELECTED_CAT_NAME, selectedCat.CatName);
            PlayerPrefs.SetFloat(C_CAT_SPEED, selectedCat.Speed);
            PlayerPrefs.SetFloat(C_CAT_ACCELERATION, selectedCat.Acceleration);
            PlayerPrefs.SetInt(C_CAT_HEALTH, selectedCat.Health);
            PlayerPrefs.SetInt(C_CAT_INTELLIGENCE, selectedCat.Intelligence);
            PlayerPrefs.SetInt(C_CAT_STRENGTH, selectedCat.Strength);
            PlayerPrefs.Save();
            
            Debug.Log($"게임 데이터 저장 완료: {selectedCat.CatName}");
        }
    }

    /// <summary>
    /// 저장된 고양이 데이터 로드 (메인씬에서 사용)
    /// </summary>
    public static CatStats LoadCatData()
    {
        const string C_SELECTED_CAT_NAME = "SelectedCatName";
        const string C_CAT_SPEED = "CatSpeed";
        const string C_CAT_ACCELERATION = "CatAcceleration";
        const string C_CAT_HEALTH = "CatHealth";
        const string C_CAT_INTELLIGENCE = "CatIntelligence";
        const string C_CAT_STRENGTH = "CatStrength";

        if (!PlayerPrefs.HasKey(C_SELECTED_CAT_NAME)) return null;

        // ScriptableObject 생성 (런타임에서)
        var catStats = ScriptableObject.CreateInstance<CatStats>();
        
        // 데이터 설정 (리플렉션 사용)
        var speedField = typeof(CatStats).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var accelerationField = typeof(CatStats).GetField("acceleration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var healthField = typeof(CatStats).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var intelligenceField = typeof(CatStats).GetField("intelligence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var strengthField = typeof(CatStats).GetField("strength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        speedField?.SetValue(catStats, PlayerPrefs.GetFloat(C_CAT_SPEED, 10f));
        accelerationField?.SetValue(catStats, PlayerPrefs.GetFloat(C_CAT_ACCELERATION, 10f));
        healthField?.SetValue(catStats, PlayerPrefs.GetInt(C_CAT_HEALTH, 50));
        intelligenceField?.SetValue(catStats, PlayerPrefs.GetInt(C_CAT_INTELLIGENCE, 50));
        strengthField?.SetValue(catStats, PlayerPrefs.GetInt(C_CAT_STRENGTH, 50));

        return catStats;
    }

    /// <summary>
    /// 저장된 데이터가 있는지 확인
    /// </summary>
    public static bool HasSavedData()
    {
        return PlayerPrefs.HasKey("SelectedCatName");
    }

    /// <summary>
    /// 모든 데이터 삭제
    /// </summary>
    public static void ClearAllData()
    {
        PlayerPrefs.DeleteKey("SelectedCatName");
        PlayerPrefs.DeleteKey("CatSpeed");
        PlayerPrefs.DeleteKey("CatAcceleration");
        PlayerPrefs.DeleteKey("CatHealth");
        PlayerPrefs.DeleteKey("CatIntelligence");
        PlayerPrefs.DeleteKey("CatStrength");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 메인씬에서 저장된 고양이 데이터를 로드하고 적용하는 메소드
    /// 메인씬의 고양이 오브젝트에 이 메소드를 호출하여 스탯 적용
    /// </summary>
    /// <param name="catComponent">적용할 Cat 컴포넌트</param>
    public static void ApplyCatStatsToGame(Cat catComponent)
    {
        var loadedCatData = LoadCatData();
        if (loadedCatData == null || catComponent == null) return;

        // Cat 컴포넌트의 스탯 필드에 적용
        // 실제 Cat 클래스의 구조에 따라 수정 필요
        var speedField = typeof(Cat).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var accelerationField = typeof(Cat).GetField("acceleration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var healthField = typeof(Cat).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var intelligenceField = typeof(Cat).GetField("intelligence", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var strengthField = typeof(Cat).GetField("strength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        speedField?.SetValue(catComponent, loadedCatData.Speed);
        accelerationField?.SetValue(catComponent, loadedCatData.Acceleration);
        healthField?.SetValue(catComponent, loadedCatData.Health);
        intelligenceField?.SetValue(catComponent, loadedCatData.Intelligence);
        strengthField?.SetValue(catComponent, loadedCatData.Strength);

        Debug.Log($"고양이 데이터 적용 완료: {loadedCatData.ToString()}");
    }

    /// <summary>
    /// 메인씬 로드 (현재 주석 처리)
    /// </summary>
    public void LoadMainScene()
    {
        Debug.Log("메인씬 로드 요청됨 - 현재 주석 처리됨");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// 레이스 패널 토글
    /// </summary>
    public void ToggleRacePanel()
    {
        if (m_racePanel != null)
        {
            bool newState = !m_racePanel.activeSelf;
            m_racePanel.SetActive(newState);
            Debug.Log($"레이스 패널 토글: {newState}");
        }
        else
        {
            Debug.LogError("레이스 패널이 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 레이스 패널 강제 비활성화
    /// </summary>
    public void HideRacePanel()
    {
        if (m_racePanel != null)
        {
            m_racePanel.SetActive(false);
            Debug.Log("레이스 패널 비활성화됨");
        }
    }

    /// <summary>
    /// 리소스 패널 토글
    /// </summary>
    public void ToggleResourcePanel()
    {
        if (m_resourcePanel != null)
        {
            m_resourcePanel.SetActive(!m_resourcePanel.activeSelf);
        }
    }
}

/// <summary>
/// 부적 데이터 구조체
/// </summary>
[System.Serializable]
public struct CharmData
{
    public int speedModifier;
    public int accelerationModifier;
    public int healthModifier;
    public int intelligenceModifier;
    public int strengthModifier;
}