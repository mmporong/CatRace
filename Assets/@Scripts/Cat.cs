using UnityEngine;

/// <summary>
/// 고양이 캐릭터를 관리하는 클래스
/// </summary>
public class Cat : MonoBehaviour
{
    [Header("고양이 스탯")]
    [SerializeField] private CatStats catStats; // 고양이 스탯 데이터

    [Header("컴포넌트 참조")]
    private Rigidbody2D rb; // 2D 물리 컴포넌트
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    private Animator animator; // 애니메이터

    [Header("현재 스탯 (런타임)")]
    [SerializeField] private float currentSpeed; // 현재 속도
    [SerializeField] private float currentAcceleration; // 현재 가속도
    [SerializeField] private int currentHealth; // 현재 체력
    [SerializeField] private int currentIntelligence; // 현재 지능
    [SerializeField] private int currentStrength; // 현재 힘

    // 공개 프로퍼티들
    public CatStats CatStats => catStats;
    public float CurrentSpeed => currentSpeed;
    public float CurrentAcceleration => currentAcceleration;
    public int CurrentHealth => currentHealth;
    public int CurrentIntelligence => currentIntelligence;
    public int CurrentStrength => currentStrength;

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        // 컴포넌트 참조 캐싱
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 고양이 스탯 초기화
    /// </summary>
    private void Start()
    {
        InitializeStats();
    }

    /// <summary>
    /// 고양이 스탯을 초기화합니다
    /// </summary>
    private void InitializeStats()
    {
        if (catStats == null)
        {
            Debug.LogError($"{gameObject.name}: CatStats가 설정되지 않았습니다!");
            return;
        }

        // 기본 스탯을 현재 스탯으로 복사
        currentSpeed = catStats.Speed;
        currentAcceleration = catStats.Acceleration;
        currentHealth = catStats.Health;
        currentIntelligence = catStats.Intelligence;
        currentStrength = catStats.Strength;

        // 스프라이트 설정
        if (spriteRenderer != null && catStats.CatSprite != null)
        {
            spriteRenderer.sprite = catStats.CatSprite;
        }

        // AI 및 이동 컴포넌트는 별도로 초기화됩니다

        Debug.Log($"{catStats.CatName} 스탯 초기화 완료: {catStats}");
    }

    /// <summary>
    /// 스탯을 수정합니다
    /// </summary>
    /// <param name="speedModifier">속도 수정값</param>
    /// <param name="accelerationModifier">가속도 수정값</param>
    /// <param name="healthModifier">체력 수정값</param>
    /// <param name="intelligenceModifier">지능 수정값</param>
    /// <param name="strengthModifier">힘 수정값</param>
    public void ModifyStats(float speedModifier = 0f, float accelerationModifier = 0f, 
                           int healthModifier = 0, int intelligenceModifier = 0, int strengthModifier = 0)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + speedModifier, 1f, 20f);
        currentAcceleration = Mathf.Clamp(currentAcceleration + accelerationModifier, 1f, 20f);
        currentHealth = Mathf.Clamp(currentHealth + healthModifier, 1, 100);
        currentIntelligence = Mathf.Clamp(currentIntelligence + intelligenceModifier, 1, 100);
        currentStrength = Mathf.Clamp(currentStrength + strengthModifier, 1, 100);

        Debug.Log($"{catStats.CatName} 스탯 수정: 속도({currentSpeed:F1}) 가속도({currentAcceleration:F1}) 체력({currentHealth}) 지능({currentIntelligence}) 힘({currentStrength})");
    }

    /// <summary>
    /// 특정 스탯을 설정합니다
    /// </summary>
    /// <param name="statType">설정할 스탯 타입</param>
    /// <param name="value">설정할 값</param>
    public void SetStat(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.Speed:
                currentSpeed = Mathf.Clamp(value, 1f, 20f);
                break;
            case StatType.Acceleration:
                currentAcceleration = Mathf.Clamp(value, 1f, 20f);
                break;
            case StatType.Health:
                currentHealth = Mathf.Clamp((int)value, 0, 100);
                break;
            case StatType.Intelligence:
                currentIntelligence = Mathf.Clamp((int)value, 1, 100);
                break;
            case StatType.Strength:
                currentStrength = Mathf.Clamp((int)value, 1, 100);
                break;
        }

        Debug.Log($"{catStats.CatName} {statType} 스탯을 {value}로 설정");
    }

    /// <summary>
    /// 현재 스탯의 총합을 반환합니다
    /// </summary>
    /// <returns>총 스탯 점수</returns>
    public float GetTotalCurrentStats()
    {
        return currentSpeed + currentAcceleration + currentHealth + currentIntelligence + currentStrength;
    }

    /// <summary>
    /// 고양이 정보를 문자열로 반환합니다
    /// </summary>
    /// <returns>고양이 정보 문자열</returns>
    public override string ToString()
    {
        return $"{catStats.CatName}: 현재 속도({currentSpeed:F1}) 가속도({currentAcceleration:F1}) 체력({currentHealth}) 지능({currentIntelligence}) 힘({currentStrength})";
    }
}
