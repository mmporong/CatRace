using UnityEngine;

/// <summary>
/// 고양이 관련 스프라이트와 데이터를 관리하는 매니저
/// 싱글톤 패턴으로 구현
/// </summary>
public class CatManager : Singleton<CatManager>
{
    #region Serialized Fields
    [Header("고양이 스프라이트")]
    [SerializeField] private Sprite[] m_catSprites;
    [SerializeField] private Sprite[] m_catSprites_Run;
    [SerializeField] private Sprite[] m_catSprites_Sleep;
    [SerializeField] private Sprite[] m_catSprites_Hands;
    #endregion

    #region Public Properties
    /// <summary>
    /// 기본 고양이 스프라이트 배열
    /// </summary>
    public Sprite[] CatSprites => m_catSprites;
    
    /// <summary>
    /// 달리는 고양이 스프라이트 배열
    /// </summary>
    public Sprite[] CatSprites_Run => m_catSprites_Run;
    
    /// <summary>
    /// 잠자는 고양이 스프라이트 배열
    /// </summary>
    public Sprite[] CatSprites_Sleep => m_catSprites_Sleep;
    
    /// <summary>
    /// 손을 든 고양이 스프라이트 배열
    /// </summary>
    public Sprite[] CatSprites_Hands => m_catSprites_Hands;
    #endregion

    #region Unity Lifecycle
    protected override void InitializeSingleton()
    {
        base.InitializeSingleton();
        LoadCatSprites();
    }

    private void Start()
    {
        // Resources에서 스프라이트 로드 (주석 처리된 코드를 활성화할 수 있음)
        // LoadCatSpritesFromResources();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 특정 인덱스의 고양이 스프라이트를 가져옵니다
    /// </summary>
    /// <param name="index">스프라이트 인덱스</param>
    /// <returns>해당 인덱스의 스프라이트, 없으면 null</returns>
    public Sprite GetCatSprite(int index)
    {
        if (m_catSprites != null && index >= 0 && index < m_catSprites.Length)
        {
            return m_catSprites[index];
        }
        return null;
    }

    /// <summary>
    /// 달리는 고양이 스프라이트를 가져옵니다
    /// </summary>
    /// <param name="index">스프라이트 인덱스</param>
    /// <returns>해당 인덱스의 달리는 스프라이트, 없으면 null</returns>
    public Sprite GetCatRunSprite(int index)
    {
        if (m_catSprites_Run != null && index >= 0 && index < m_catSprites_Run.Length)
        {
            return m_catSprites_Run[index];
        }
        return null;
    }

    /// <summary>
    /// 잠자는 고양이 스프라이트를 가져옵니다
    /// </summary>
    /// <param name="index">스프라이트 인덱스</param>
    /// <returns>해당 인덱스의 잠자는 스프라이트, 없으면 null</returns>
    public Sprite GetCatSleepSprite(int index)
    {
        if (m_catSprites_Sleep != null && index >= 0 && index < m_catSprites_Sleep.Length)
        {
            return m_catSprites_Sleep[index];
        }
        return null;
    }

    /// <summary>
    /// 손을 든 고양이 스프라이트를 가져옵니다
    /// </summary>
    /// <param name="index">스프라이트 인덱스</param>
    /// <returns>해당 인덱스의 손을 든 스프라이트, 없으면 null</returns>
    public Sprite GetCatHandsSprite(int index)
    {
        if (m_catSprites_Hands != null && index >= 0 && index < m_catSprites_Hands.Length)
        {
            return m_catSprites_Hands[index];
        }
        return null;
    }

    /// <summary>
    /// 특정 고양이의 hands 스프라이트들을 가져옵니다
    /// </summary>
    /// <param name="catIndex">고양이 인덱스</param>
    /// <returns>해당 고양이의 hands 스프라이트 배열 (왼손, 오른손)</returns>
    public Sprite[] GetCatHandsSprites(int catIndex)
    {
        if (m_catSprites_Hands == null || m_catSprites_Hands.Length == 0) return null;

        int baseIndex = catIndex * 2; // 각 고양이마다 2개의 hands 스프라이트
        if (baseIndex + 1 >= m_catSprites_Hands.Length) return null;

        return new Sprite[] 
        { 
            m_catSprites_Hands[baseIndex],     // 왼손
            m_catSprites_Hands[baseIndex + 1]  // 오른손
        };
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 고양이 스프라이트들을 로드합니다
    /// </summary>
    private void LoadCatSprites()
    {
        // Inspector에서 설정된 스프라이트가 없으면 Resources에서 로드 시도
        if (m_catSprites == null || m_catSprites.Length == 0)
        {
            LoadCatSpritesFromResources();
        }
    }

    /// <summary>
    /// Resources 폴더에서 고양이 스프라이트들을 로드합니다
    /// </summary>
    private void LoadCatSpritesFromResources()
    {
        try
        {
            m_catSprites = Resources.LoadAll<Sprite>("@Sprites/Cat/1. Cat");
            m_catSprites_Run = Resources.LoadAll<Sprite>("@Sprites/Cat/7. RunningCat");
            m_catSprites_Sleep = Resources.LoadAll<Sprite>("@Sprites/Cat/2. SleepingCat");
            
            Debug.Log($"[CatManager] Resources에서 고양이 스프라이트 로드 완료 - 기본: {m_catSprites?.Length}, 달리기: {m_catSprites_Run?.Length}, 잠자기: {m_catSprites_Sleep?.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[CatManager] Resources에서 스프라이트 로드 실패: {e.Message}");
        }
    }
    #endregion
}
