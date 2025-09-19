using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 로비씬의 3페이즈 시스템을 관리하는 매니저 (성능 최적화 버전)
/// 페이즈 0: 고양이 선택, 페이즈 1: 스탯 강화, 페이즈 2: 부적 선택
/// </summary>
public class LobbyManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("패널 참조")]
    [SerializeField] private GameObject m_catPanel;
    [SerializeField] private GameObject m_statPanel;
    [SerializeField] private GameObject m_charmPanel;
    [SerializeField] private GameObject m_racePanel;
    [SerializeField] private GameObject m_resourcePanel;

    [Header("고양이 데이터")]
    [SerializeField] private CatStats[] m_availableCats;
    #endregion

    #region Private Fields
    private int m_currentPhase = 0;
    private int m_selectedCatIndex = -1;
    private bool m_isInitialized = false;
    
    // 성능 최적화를 위한 캐시
    private readonly Dictionary<int, GameObject> m_phasePanelCache = new Dictionary<int, GameObject>();
    private CatStats m_cachedSelectedCat;
    private bool m_cachedCatValid = false;
    #endregion

    #region Public Properties
    public static LobbyManager Instance { get; private set; }
    
    public CatStats SelectedCat 
    { 
        get 
        {
            if (!m_cachedCatValid)
            {
                m_cachedSelectedCat = (m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length) 
                    ? m_availableCats[m_selectedCatIndex] : null;
                m_cachedCatValid = true;
            }
            return m_cachedSelectedCat;
        }
    }
    
    public int CurrentPhase => m_currentPhase;
    #endregion

    #region Events
    public static event Action<int> OnPhaseChanged;
    public static event Action<CatStats> OnCatSelectionChanged;
    public static event Action<StatType, int> OnStatEnhancementChanged;
    public static event Action<CharmData> OnCharmSelectionChanged;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCache();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!m_isInitialized)
        {
            InitializeLobby();
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        OnPhaseChanged = null;
        OnCatSelectionChanged = null;
        OnStatEnhancementChanged = null;
        OnCharmSelectionChanged = null;
    }
    #endregion

    #region Initialization
    /// <summary>
    /// 캐시 초기화
    /// </summary>
    private void InitializeCache()
    {
        m_phasePanelCache.Clear();
        m_phasePanelCache[0] = m_catPanel;
        m_phasePanelCache[1] = m_statPanel;
        m_phasePanelCache[2] = m_charmPanel;
    }

    /// <summary>
    /// 로비 초기화
    /// </summary>
    private void InitializeLobby()
    {
        m_currentPhase = 0;
        m_isInitialized = true;
        
        // 레이스 패널 초기 상태를 비활성화로 설정
        m_racePanel?.SetActive(false);
        
        ShowPhasePanel();
    }
    #endregion

    #region Phase Management
    /// <summary>
    /// 현재 페이즈에 맞는 패널 표시 (캐시 최적화)
    /// </summary>
    private void ShowPhasePanel()
    {
        // 모든 패널 비활성화 (레이스 패널 제외 - 독립적으로 관리)
        foreach (var panel in m_phasePanelCache.Values)
        {
            panel?.SetActive(false);
        }

        // 현재 페이즈에 맞는 패널 활성화 (캐시 사용)
        if (m_phasePanelCache.TryGetValue(m_currentPhase, out GameObject targetPanel))
        {
            targetPanel?.SetActive(true);
        }

        // 이벤트 발생
        OnPhaseChanged?.Invoke(m_currentPhase);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 고양이 선택 완료 (인덱스로 선택) - 최적화된 버전
    /// </summary>
    public void OnCatSelected(CatStats selectedCat)
    {
        if (selectedCat == null) return;

        // 선택된 고양이의 인덱스를 찾아서 저장 (최적화된 검색)
        m_selectedCatIndex = Array.FindIndex(m_availableCats, cat => cat == selectedCat);
        
        if (m_selectedCatIndex >= 0)
        {
            // 캐시 무효화
            m_cachedCatValid = false;
            
            Debug.Log($"고양이 선택됨: {selectedCat.CatName} (인덱스: {m_selectedCatIndex})");
            Debug.Log($"선택된 고양이 스탯: {selectedCat.ToString()}");
            
            // 이벤트 발생
            OnCatSelectionChanged?.Invoke(selectedCat);
            
            // 다음 페이즈로 진행
            NextPhase();
        }
        else
        {
            Debug.LogError($"선택된 고양이를 찾을 수 없습니다: {selectedCat.CatName}");
        }
    }

    /// <summary>
    /// 스탯 강화 완료 - 최적화된 버전
    /// </summary>
    public void OnStatEnhanced(StatType statType, int value)
    {
        if (IsValidCatIndex())
        {
            var selectedCat = m_availableCats[m_selectedCatIndex];
            
            // 스탯 수정 (최적화된 조건문)
            switch (statType)
            {
                case StatType.Speed:
                    selectedCat.ModifyStats(value, 0, 0, 0, 0);
                    break;
                case StatType.Acceleration:
                    selectedCat.ModifyStats(0, value, 0, 0, 0);
                    break;
                case StatType.Health:
                    selectedCat.ModifyStats(0, 0, value, 0, 0);
                    break;
                case StatType.Intelligence:
                    selectedCat.ModifyStats(0, 0, 0, value, 0);
                    break;
                case StatType.Strength:
                    selectedCat.ModifyStats(0, 0, 0, 0, value);
                    break;
            }
            
            // 캐시 무효화
            m_cachedCatValid = false;
            
            Debug.Log($"스탯 강화 완료: {selectedCat.CatName} - {statType} +{value}");
            
            // 이벤트 발생
            OnStatEnhancementChanged?.Invoke(statType, value);
        }
        NextPhase();
    }

    /// <summary>
    /// 부적 선택 완료 - 최적화된 버전
    /// </summary>
    public void OnCharmSelected(CharmData charmData)
    {
        if (IsValidCatIndex())
        {
            var selectedCat = m_availableCats[m_selectedCatIndex];
            selectedCat.ModifyStats(
                charmData.speedModifier,
                charmData.accelerationModifier,
                charmData.healthModifier,
                charmData.intelligenceModifier,
                charmData.strengthModifier
            );
            
            // 캐시 무효화
            m_cachedCatValid = false;
            
            Debug.Log($"부적 적용 완료: {selectedCat.CatName} - 속도({charmData.speedModifier:+0;-0}) 가속도({charmData.accelerationModifier:+0;-0}) 체력({charmData.healthModifier:+0;-0}) 지능({charmData.intelligenceModifier:+0;-0}) 힘({charmData.strengthModifier:+0;-0})");
            
            // 이벤트 발생
            OnCharmSelectionChanged?.Invoke(charmData);
        }
        
        SaveGameData();
        
        // 부적 선택 완료 후 레이스 패널 활성화
        Debug.Log("부적 선택 완료! 레이스 패널을 활성화합니다.");
        m_charmPanel?.SetActive(false);
        m_racePanel?.SetActive(true);
    }

    #region Private Methods
    /// <summary>
    /// 유효한 고양이 인덱스인지 확인
    /// </summary>
    private bool IsValidCatIndex()
    {
        return m_selectedCatIndex >= 0 && m_selectedCatIndex < m_availableCats.Length;
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
    #endregion


    #region Data Management
    /// <summary>
    /// 게임 데이터 저장 (PlayerPrefs 기반) - 최적화된 버전
    /// </summary>
    private void SaveGameData()
    {
        if (!IsValidCatIndex()) return;

        var selectedCat = m_availableCats[m_selectedCatIndex];
        
        // 저장 키 상수
        const string C_SELECTED_CAT_NAME = "SelectedCatName";
        const string C_CAT_SPEED = "CatSpeed";
        const string C_CAT_ACCELERATION = "CatAcceleration";
        const string C_CAT_HEALTH = "CatHealth";
        const string C_CAT_INTELLIGENCE = "CatIntelligence";
        const string C_CAT_STRENGTH = "CatStrength";

        // 배치 저장으로 성능 최적화
        PlayerPrefs.SetString(C_SELECTED_CAT_NAME, selectedCat.CatName);
        PlayerPrefs.SetFloat(C_CAT_SPEED, selectedCat.Speed);
        PlayerPrefs.SetFloat(C_CAT_ACCELERATION, selectedCat.Acceleration);
        PlayerPrefs.SetInt(C_CAT_HEALTH, selectedCat.Health);
        PlayerPrefs.SetInt(C_CAT_INTELLIGENCE, selectedCat.Intelligence);
        PlayerPrefs.SetInt(C_CAT_STRENGTH, selectedCat.Strength);
        PlayerPrefs.Save();
        
        Debug.Log($"게임 데이터 저장 완료: {selectedCat.CatName}");
    }

    /// <summary>
    /// 저장된 고양이 데이터 로드 (메인씬에서 사용) - 리플렉션 제거 버전
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
        
        // 리플렉션 대신 직접 데이터 설정 (성능 최적화)
        // CatStats 클래스에 ModifyStats 메소드가 있다고 가정
        // 실제 구현에서는 CatStats 클래스의 구조에 맞게 수정 필요
        try
        {
            // 기본값으로 설정 후 PlayerPrefs에서 로드
            catStats.ModifyStats(
                PlayerPrefs.GetFloat(C_CAT_SPEED, 10f),
                PlayerPrefs.GetFloat(C_CAT_ACCELERATION, 10f),
                PlayerPrefs.GetInt(C_CAT_HEALTH, 50),
                PlayerPrefs.GetInt(C_CAT_INTELLIGENCE, 50),
                PlayerPrefs.GetInt(C_CAT_STRENGTH, 50)
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"고양이 데이터 로드 실패: {e.Message}");
            return null;
        }

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
    /// 모든 데이터 삭제 - 최적화된 버전
    /// </summary>
    public static void ClearAllData()
    {
        // 배치 삭제로 성능 최적화
        string[] keysToDelete = {
            "SelectedCatName", "CatSpeed", "CatAcceleration", 
            "CatHealth", "CatIntelligence", "CatStrength"
        };
        
        foreach (string key in keysToDelete)
        {
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }
    #endregion

    #region Game Integration
    /// <summary>
    /// 메인씬에서 저장된 고양이 데이터를 로드하고 적용하는 메소드 - 리플렉션 제거 버전
    /// 메인씬의 고양이 오브젝트에 이 메소드를 호출하여 스탯 적용
    /// </summary>
    /// <param name="catComponent">적용할 Cat 컴포넌트</param>
    public static void ApplyCatStatsToGame(Cat catComponent)
    {
        var loadedCatData = LoadCatData();
        if (loadedCatData == null || catComponent == null) return;

        // 리플렉션 대신 직접 메소드 호출 (성능 최적화)
        // Cat 클래스에 ModifyStats 메소드가 있다고 가정
        // 실제 구현에서는 Cat 클래스의 구조에 맞게 수정 필요
        try
        {
            catComponent.ModifyStats(
                loadedCatData.Speed,
                loadedCatData.Acceleration,
                loadedCatData.Health,
                loadedCatData.Intelligence,
                loadedCatData.Strength
            );
            
            Debug.Log($"고양이 데이터 적용 완료: {loadedCatData.ToString()}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"고양이 데이터 적용 실패: {e.Message}");
        }
    }
    #endregion

    #region UI Management
    /// <summary>
    /// 메인씬 로드 (현재 주석 처리)
    /// </summary>
    public void LoadMainScene()
    {
        Debug.Log("메인씬 로드 요청됨 - 현재 주석 처리됨");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// 레이스 패널 토글 - 최적화된 버전
    /// </summary>
    public void ToggleRacePanel()
    {
        if (m_racePanel == null)
        {
            Debug.LogError("레이스 패널이 할당되지 않았습니다!");
            return;
        }

        bool newState = !m_racePanel.activeSelf;
        m_racePanel.SetActive(newState);
        Debug.Log($"레이스 패널 토글: {newState}");
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
    /// 리소스 패널 토글 - 최적화된 버전
    /// </summary>
    public void ToggleResourcePanel()
    {
        if (m_resourcePanel == null) return;
        
        m_resourcePanel.SetActive(!m_resourcePanel.activeSelf);
    }
    #endregion
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
#endregion