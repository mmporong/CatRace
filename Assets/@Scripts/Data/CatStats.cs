using UnityEngine;

/// <summary>
/// 고양이의 스탯 데이터를 관리하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CatStats", menuName = "CatRace/Cat Stats")]
public class CatStats : ScriptableObject
{
    [Header("고양이 기본 정보")]
    [SerializeField] private string catName = "기본 고양이"; // 고양이 이름
    [SerializeField] private Sprite catSprite; // 고양이 스프라이트
    [SerializeField] private string description = "고양이 설명"; // 고양이 설명

    [Header("고양이 스탯")]
    [SerializeField] [Range(1f, 20f)] private float speed = 10f; // 속도
    [SerializeField] [Range(1f, 20f)] private float acceleration = 10f; // 가속도
    [SerializeField] [Range(1, 100)] private int health = 50; // 체력
    [SerializeField] [Range(1, 100)] private int intelligence = 50; // 지능
    [SerializeField] [Range(1, 100)] private int strength = 50; // 힘

    // 공개 프로퍼티들 (읽기 전용)
    public string CatName => catName;
    public Sprite CatSprite => catSprite;
    public string Description => description;
    
    // 스탯 프로퍼티들 (읽기 전용)
    public float Speed => speed;
    public float Acceleration => acceleration;
    public int Health => health;
    public int Intelligence => intelligence;
    public int Strength => strength;

    /// <summary>
    /// 스탯을 수정합니다 (런타임에서 사용)
    /// </summary>
    /// <param name="speedModifier">속도 수정값</param>
    /// <param name="accelerationModifier">가속도 수정값</param>
    /// <param name="healthModifier">체력 수정값</param>
    /// <param name="intelligenceModifier">지능 수정값</param>
    /// <param name="strengthModifier">힘 수정값</param>
    public void ModifyStats(float speedModifier = 0f, float accelerationModifier = 0f, 
                           int healthModifier = 0, int intelligenceModifier = 0, int strengthModifier = 0)
    {
        speed = Mathf.Clamp(speed + speedModifier, 1f, 20f);
        acceleration = Mathf.Clamp(acceleration + accelerationModifier, 1f, 20f);
        health = Mathf.Clamp(health + healthModifier, 1, 100);
        intelligence = Mathf.Clamp(intelligence + intelligenceModifier, 1, 100);
        strength = Mathf.Clamp(strength + strengthModifier, 1, 100);
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
                speed = Mathf.Clamp(value, 1f, 20f);
                break;
            case StatType.Acceleration:
                acceleration = Mathf.Clamp(value, 1f, 20f);
                break;
            case StatType.Health:
                health = Mathf.Clamp((int)value, 1, 100);
                break;
            case StatType.Intelligence:
                intelligence = Mathf.Clamp((int)value, 1, 100);
                break;
            case StatType.Strength:
                strength = Mathf.Clamp((int)value, 1, 100);
                break;
        }
    }

    /// <summary>
    /// 모든 스탯의 총합을 반환합니다
    /// </summary>
    /// <returns>총 스탯 점수</returns>
    public float GetTotalStats()
    {
        return speed + acceleration + health + intelligence + strength;
    }

    /// <summary>
    /// 스탯 정보를 문자열로 반환합니다
    /// </summary>
    /// <returns>스탯 정보 문자열</returns>
    public override string ToString()
    {
        return $"{catName}: 속도({speed:F1}) 가속도({acceleration:F1}) 체력({health}) 지능({intelligence}) 힘({strength})";
    }
}

/// <summary>
/// 스탯 타입 열거형
/// </summary>
public enum StatType
{
    Speed,      // 속도
    Acceleration, // 가속도
    Health,     // 체력
    Intelligence, // 지능
    Strength    // 힘
}
