using UnityEngine;

/// <summary>
/// 고양이 캐릭터를 관리하는 클래스
/// </summary>
public class Cat : MonoBehaviour
{
    [Header("고양이 스탯")]
    [SerializeField] private CatStats catStats;
    [SerializeField] private bool isMyCat = false;
    [SerializeField] private int catIndex = 0; // 고양이 스프라이트 인덱스 (0-19)
    public GameObject[] hands;

    [Header("컴포넌트 참조")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private CatManager catManager;

    [Header("현재 스탯 (런타임)")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentAcceleration;
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentIntelligence;
    [SerializeField] private int currentStrength;

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

        // Model 자식 오브젝트에서 SpriteRenderer와 Animator 찾기
        Transform modelTransform = transform.Find("Model");
        if (modelTransform != null)
        {
            spriteRenderer = modelTransform.GetComponent<SpriteRenderer>();
            animator = modelTransform.GetComponent<Animator>();
        }
        else
        {
            // Model을 찾을 수 없으면 전체 자식에서 찾기
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        // CatManager 찾기
        catManager = FindFirstObjectByType<CatManager>();
        if (catManager == null)
        {
            Debug.LogWarning($"{gameObject.name}: CatManager를 찾을 수 없습니다!");
        }
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

        // 고양이 인덱스 설정
        if (isMyCat)
        {
            // MyCat은 Inspector에서 설정된 catIndex 값 사용 (또는 CatStats의 인덱스)
            if (catIndex == 0 && catStats.CatIndex != 0)
            {
                catIndex = catStats.CatIndex; // CatStats의 인덱스 사용
            }
            Debug.Log($"{catStats.CatName} (MyCat) 인덱스 설정: {catIndex}");
        }
        else
        {
            // AI 고양이는 랜덤 인덱스 사용
            catIndex = Random.Range(0, 20);
            Debug.Log($"{catStats.CatName} (AI) 랜덤 인덱스 설정: {catIndex}");
        }

        // 랜덤 스탯 변동 적용
        ApplyRandomStatVariation();

        // 스프라이트 설정
        SetSprite(0); // 기본 스프라이트
    }

    /// <summary>
    /// 랜덤 스탯 변동을 적용합니다 (-20% ~ +20%)
    /// </summary>
    private void ApplyRandomStatVariation()
    {
        // 각 스탯에 대해 -20% ~ +20% 랜덤 변동 적용
        float speedVariation = Random.Range(-0.2f, 0.2f);
        float accelerationVariation = Random.Range(-0.2f, 0.2f);
        float healthVariation = Random.Range(-0.2f, 0.2f);
        float intelligenceVariation = Random.Range(-0.2f, 0.2f);
        float strengthVariation = Random.Range(-0.2f, 0.2f);

        // 변동된 스탯 적용
        currentSpeed = Mathf.Clamp(currentSpeed * (1f + speedVariation), 1f, 20f);
        currentAcceleration = Mathf.Clamp(currentAcceleration * (1f + accelerationVariation), 1f, 20f);
        currentHealth = Mathf.Clamp(Mathf.RoundToInt(currentHealth * (1f + healthVariation)), 1, 100);
        currentIntelligence = Mathf.Clamp(Mathf.RoundToInt(currentIntelligence * (1f + intelligenceVariation)), 1, 100);
        currentStrength = Mathf.Clamp(Mathf.RoundToInt(currentStrength * (1f + strengthVariation)), 1, 100);
    }

    /// <summary>
    /// 스탯을 수정합니다
    /// </summary>
    public void ModifyStats(float speedModifier = 0f, float accelerationModifier = 0f,
                           int healthModifier = 0, int intelligenceModifier = 0, int strengthModifier = 0)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + speedModifier, 1f, 20f);
        currentAcceleration = Mathf.Clamp(currentAcceleration + accelerationModifier, 1f, 20f);
        currentHealth = Mathf.Clamp(currentHealth + healthModifier, 1, 100);
        currentIntelligence = Mathf.Clamp(currentIntelligence + intelligenceModifier, 1, 100);
        currentStrength = Mathf.Clamp(currentStrength + strengthModifier, 1, 100);
    }

    /// <summary>
    /// 특정 스탯을 설정합니다
    /// </summary>
    public void SetStat(StatType statType, float value)
    {
        int oldHealth = currentHealth;
        
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
                
                // 체력이 회복되었는지 확인
                if (currentHealth > oldHealth)
                {
                    OnHealthRecovered();
                }
                break;
            case StatType.Intelligence:
                currentIntelligence = Mathf.Clamp((int)value, 1, 100);
                break;
            case StatType.Strength:
                currentStrength = Mathf.Clamp((int)value, 1, 100);
                break;
        }
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
    /// 달리기 애니메이션을 시작합니다
    /// </summary>
    public void StartRunning()
    {
        SetAnimation("Run", 1);
    }

    /// <summary>
    /// 잠자기 애니메이션을 시작합니다
    /// </summary>
    public void StartSleeping()
    {
        SetAnimation("Sleep", 2);
    }

    /// <summary>
    /// 기본 상태 애니메이션을 시작합니다
    /// </summary>
    public void StartIdle()
    {
        SetAnimation("Idle", 0);
    }

    /// <summary>
    /// 애니메이션과 스프라이트를 설정합니다
    /// </summary>
    private void SetAnimation(string triggerName, int spriteType)
    {
        if (animator != null && HasAnimatorParameter(triggerName))
        {
            animator.SetTrigger(triggerName);
            SetSprite(spriteType);
            
            // hands 스프라이트 관리
            if (spriteType == 1) // Run 애니메이션일 때만 hands 표시
            {
                SetHandsSprites();
                SetHandsVisibility(true);
            }
            else // 다른 애니메이션일 때는 hands 숨김
            {
                SetHandsVisibility(false);
            }
        }
    }

    /// <summary>
    /// 애니메이터 파라미터 존재 여부를 확인합니다
    /// </summary>
    private bool HasAnimatorParameter(string parameterName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 체력 상태를 확인하고 필요시 잠자기 상태로 전환합니다
    /// </summary>
    public void CheckHealthStatus()
    {
        if (currentHealth <= 0)
        {
            StartSleeping();
        }
    }

    /// <summary>
    /// 체력 회복 시 애니메이션 상태를 업데이트합니다
    /// </summary>
    public void OnHealthRecovered()
    {
        // 체력이 50% 이상 회복되면 Run 애니메이션으로 전환
        float healthPercentage = (float)currentHealth / catStats.Health;
        if (healthPercentage >= 0.5f)
        {
            StartRunning();
            Debug.Log($"{catStats.CatName} 체력 회복으로 Run 애니메이션 실행!");
        }
    }

    /// <summary>
    /// 스프라이트를 설정합니다
    /// </summary>
    private void SetSprite(int spriteType)
    {
        if (catManager == null) return;

        Sprite[] spriteArray = GetSpriteArray(spriteType);
        if (spriteArray != null && catIndex >= 0 && catIndex < spriteArray.Length)
        {
            Sprite sprite = spriteArray[catIndex];
            SetSpriteOnModel(sprite);
            
            string spriteTypeName = spriteType == 0 ? "기본" : spriteType == 1 ? "달리기" : "잠자기";
        }
    
    }

    /// <summary>
    /// 스프라이트 타입에 따른 배열을 반환합니다
    /// </summary>
    private Sprite[] GetSpriteArray(int spriteType)
    {
        switch (spriteType)
        {
            case 0: return catManager.CatSprites;
            case 1: return catManager.CatSprites_Run;
            case 2: return catManager.CatSprites_Sleep;
            default: return catManager.CatSprites;
        }
    }

    /// <summary>
    /// Model 자식 오브젝트에 스프라이트를 설정합니다
    /// </summary>
    private void SetSpriteOnModel(Sprite sprite)
    {
        if (sprite == null) return;

        Transform modelTransform = transform.Find("Model");
        if (modelTransform != null)
        {
            SpriteRenderer modelSpriteRenderer = modelTransform.GetComponent<SpriteRenderer>();
            if (modelSpriteRenderer != null)
            {
                modelSpriteRenderer.sprite = sprite;
            }
        }
    }

    /// <summary>
    /// hands 오브젝트들에 스프라이트를 설정합니다
    /// </summary>
    private void SetHandsSprites()
    {
        if (catManager == null || hands == null || hands.Length == 0) return;

        // CatManager에서 해당 고양이의 hands 스프라이트들을 가져옴
        Sprite[] catHandsSprites = catManager.GetCatHandsSprites(catIndex);
        if (catHandsSprites == null || catHandsSprites.Length == 0) return;

        // hands 오브젝트에 스프라이트 할당
        for (int i = 0; i < hands.Length && i < catHandsSprites.Length; i++)
        {
            if (hands[i] != null)
            {
                SpriteRenderer handSpriteRenderer = hands[i].GetComponent<SpriteRenderer>();
                if (handSpriteRenderer != null)
                {
                    handSpriteRenderer.sprite = catHandsSprites[i];
                }
            }
        }
    }

    /// <summary>
    /// hands 오브젝트들의 가시성을 설정합니다
    /// </summary>
    private void SetHandsVisibility(bool isVisible)
    {
        if (hands == null || hands.Length == 0) return;

        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] != null)
            {
                hands[i].SetActive(isVisible);
            }
        }
    }

    /// <summary>
    /// 고양이 인덱스를 설정하고 스프라이트를 업데이트합니다
    /// </summary>
    public void SetCatIndex(int index)
    {
        int oldIndex = catIndex;
        catIndex = Mathf.Clamp(index, 0, 19);
        SetSprite(0); // 기본 스프라이트로 업데이트
        
        Debug.Log($"{catStats.CatName} 인덱스 변경: {oldIndex} → {catIndex}");
    }

    /// <summary>
    /// 현재 고양이 인덱스를 반환합니다
    /// </summary>
    public int GetCatIndex()
    {
        return catIndex;
    }

    /// <summary>
    /// MyCat 여부를 설정합니다
    /// </summary>
    public void SetIsMyCat(bool isMyCatValue)
    {
        isMyCat = isMyCatValue;
    }

    /// <summary>
    /// MyCat 여부를 반환합니다
    /// </summary>
    public bool IsMyCat()
    {
        return isMyCat;
    }

    /// <summary>
    /// MyCat의 인덱스를 명시적으로 설정합니다
    /// </summary>
    public void SetMyCatIndex(int index)
    {
        if (isMyCat)
        {
            SetCatIndex(index);
            Debug.Log($"{catStats.CatName} MyCat 인덱스를 {index}로 설정했습니다.");
        }
        else
        {
            Debug.LogWarning($"{catStats.CatName}: MyCat이 아닌 고양이의 인덱스를 설정할 수 없습니다!");
        }
    }

    /// <summary>
    /// 고양이 정보를 문자열로 반환합니다
    /// </summary>
    public override string ToString()
    {
        return $"{catStats.CatName}: 현재 속도({currentSpeed:F1}) 가속도({currentAcceleration:F1}) 체력({currentHealth}) 지능({currentIntelligence}) 힘({currentStrength})";
    }
}
