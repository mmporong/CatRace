using UnityEngine;

/// <summary>
/// 고양이 스탯 시스템을 테스트하는 스크립트
/// </summary>
public class CatStatsTester : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private Cat cat; // 테스트할 고양이
    [SerializeField] private bool runTests = false;

    private void OnValidate()
    {
        if (runTests)
        {
            RunAllTests();
            runTests = false;
        }
    }

    /// <summary>
    /// 모든 테스트를 실행합니다
    /// </summary>
    private void RunAllTests()
    {
        if (cat == null)
        {
            Debug.LogError("테스트할 고양이가 설정되지 않았습니다!");
            return;
        }

        Debug.Log("=== 고양이 스탯 시스템 테스트 시작 ===");

        // 1. 기본 스탯 출력 테스트
        TestBasicStats();

        // 2. 스탯 수정 테스트
        TestStatModification();

        // 3. 스탯 설정 테스트
        TestStatSetting();

        // 4. 총 스탯 계산 테스트
        TestTotalStats();

        Debug.Log("=== 고양이 스탯 시스템 테스트 완료 ===");
    }

    /// <summary>
    /// 기본 스탯 출력 테스트
    /// </summary>
    private void TestBasicStats()
    {
        Debug.Log("1. 기본 스탯 출력 테스트");
        Debug.Log($"고양이 이름: {cat.CatStats?.CatName}");
        Debug.Log($"현재 속도: {cat.CurrentSpeed}");
        Debug.Log($"현재 가속도: {cat.CurrentAcceleration}");
        Debug.Log($"현재 체력: {cat.CurrentHealth}");
        Debug.Log($"현재 지능: {cat.CurrentIntelligence}");
        Debug.Log($"현재 힘: {cat.CurrentStrength}");
        Debug.Log($"고양이 정보: {cat}");
    }

    /// <summary>
    /// 스탯 수정 테스트
    /// </summary>
    private void TestStatModification()
    {
        Debug.Log("2. 스탯 수정 테스트");
        
        // 속도 +2, 가속도 +1, 체력 +10, 지능 +5, 힘 +3
        cat.ModifyStats(2f, 1f, 10, 5, 3);
        
        Debug.Log("스탯 수정 후:");
        Debug.Log($"속도: {cat.CurrentSpeed} (예상: {cat.CatStats.Speed + 2})");
        Debug.Log($"가속도: {cat.CurrentAcceleration} (예상: {cat.CatStats.Acceleration + 1})");
        Debug.Log($"체력: {cat.CurrentHealth} (예상: {cat.CatStats.Health + 10})");
        Debug.Log($"지능: {cat.CurrentIntelligence} (예상: {cat.CatStats.Intelligence + 5})");
        Debug.Log($"힘: {cat.CurrentStrength} (예상: {cat.CatStats.Strength + 3})");
    }

    /// <summary>
    /// 스탯 설정 테스트
    /// </summary>
    private void TestStatSetting()
    {
        Debug.Log("3. 스탯 설정 테스트");
        
        // 속도를 15로 설정
        cat.SetStat(StatType.Speed, 15f);
        Debug.Log($"속도 설정 후: {cat.CurrentSpeed} (예상: 15)");
        
        // 체력을 80으로 설정
        cat.SetStat(StatType.Health, 80f);
        Debug.Log($"체력 설정 후: {cat.CurrentHealth} (예상: 80)");
        
        // 지능을 95로 설정 (최대값 테스트)
        cat.SetStat(StatType.Intelligence, 95f);
        Debug.Log($"지능 설정 후: {cat.CurrentIntelligence} (예상: 95)");
    }

    /// <summary>
    /// 총 스탯 계산 테스트
    /// </summary>
    private void TestTotalStats()
    {
        Debug.Log("4. 총 스탯 계산 테스트");
        
        float totalStats = cat.GetTotalCurrentStats();
        float expectedTotal = cat.CurrentSpeed + cat.CurrentAcceleration + 
                             cat.CurrentHealth + cat.CurrentIntelligence + cat.CurrentStrength;
        
        Debug.Log($"총 스탯: {totalStats} (예상: {expectedTotal})");
        Debug.Log($"계산이 올바른가? {Mathf.Approximately(totalStats, expectedTotal)}");
    }

    /// <summary>
    /// 스탯 한계값 테스트
    /// </summary>
    [ContextMenu("스탯 한계값 테스트")]
    public void TestStatLimits()
    {
        if (cat == null) return;

        Debug.Log("=== 스탯 한계값 테스트 ===");
        
        // 최대값 초과 테스트
        cat.SetStat(StatType.Speed, 25f); // 최대 20
        Debug.Log($"속도 최대값 테스트: {cat.CurrentSpeed} (예상: 20)");
        
        cat.SetStat(StatType.Health, 150f); // 최대 100
        Debug.Log($"체력 최대값 테스트: {cat.CurrentHealth} (예상: 100)");
        
        // 최소값 미만 테스트
        cat.SetStat(StatType.Speed, -5f); // 최소 1
        Debug.Log($"속도 최소값 테스트: {cat.CurrentSpeed} (예상: 1)");
        
        cat.SetStat(StatType.Health, -10f); // 최소 1
        Debug.Log($"체력 최소값 테스트: {cat.CurrentHealth} (예상: 1)");
    }
}
