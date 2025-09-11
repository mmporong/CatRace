using UnityEngine;

/// <summary>
/// AI 시스템을 테스트하는 스크립트
/// </summary>
public class AITester : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private bool runTests = false; // 테스트 실행 플래그
    [SerializeField] private Cat[] testCats; // 테스트할 고양이들
    [SerializeField] private TrackManager trackManager; // 트랙 매니저

    [Header("AI 테스트")]
    [SerializeField] private bool testMovement = true; // 이동 테스트
    [SerializeField] private bool testAIBehavior = true; // AI 행동 테스트
    [SerializeField] private bool testStats = true; // 스탯 테스트

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
        Debug.Log("=== AI 시스템 테스트 시작 ===");

        // 1. 기본 설정 확인
        TestBasicSetup();

        // 2. 이동 시스템 테스트
        if (testMovement)
        {
            TestMovementSystem();
        }

        // 3. AI 행동 테스트
        if (testAIBehavior)
        {
            TestAIBehavior();
        }

        // 4. 스탯 시스템 테스트
        if (testStats)
        {
            TestStatsSystem();
        }

        Debug.Log("=== AI 시스템 테스트 완료 ===");
    }

    /// <summary>
    /// 기본 설정을 확인합니다
    /// </summary>
    private void TestBasicSetup()
    {
        Debug.Log("1. 기본 설정 확인");

        // 트랙 매니저 확인
        if (trackManager == null)
        {
            trackManager = FindObjectOfType<TrackManager>();
        }

        if (trackManager != null)
        {
            Debug.Log($"✅ 트랙 매니저 발견: {trackManager.name}");
        }
        else
        {
            Debug.LogError("❌ 트랙 매니저를 찾을 수 없습니다!");
        }

        // 테스트 고양이들 확인
        if (testCats == null || testCats.Length == 0)
        {
            testCats = FindObjectsOfType<Cat>();
        }

        if (testCats != null && testCats.Length > 0)
        {
            Debug.Log($"✅ 테스트 고양이 {testCats.Length}마리 발견");
        }
        else
        {
            Debug.LogWarning("⚠️ 테스트할 고양이를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 이동 시스템을 테스트합니다
    /// </summary>
    private void TestMovementSystem()
    {
        Debug.Log("2. 이동 시스템 테스트");

        if (testCats == null) return;

        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            // CatMovement 컴포넌트 확인
            CatMovement movement = cat.GetComponent<CatMovement>();
            if (movement != null)
            {
                Debug.Log($"✅ {cat.CatStats.CatName}: CatMovement 컴포넌트 확인");
                
                // 이동 속도 테스트
                float originalSpeed = movement.MoveSpeed;
                movement.SetSpeed(10f);
                Debug.Log($"✅ {cat.CatStats.CatName}: 속도 설정 테스트 (원래: {originalSpeed:F2} → 설정: 10.00)");
                
                // 스탯 적용 테스트
                movement.ApplyStats();
                Debug.Log($"✅ {cat.CatStats.CatName}: 스탯 적용 테스트");
            }
            else
            {
                Debug.LogError($"❌ {cat.CatStats.CatName}: CatMovement 컴포넌트가 없습니다!");
            }
        }
    }

    /// <summary>
    /// AI 행동을 테스트합니다
    /// </summary>
    private void TestAIBehavior()
    {
        Debug.Log("3. AI 행동 테스트");

        if (testCats == null) return;

        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            // CatAI 컴포넌트 확인
            CatAI catAI = cat.GetComponent<CatAI>();
            if (catAI != null)
            {
                Debug.Log($"✅ {cat.CatStats.CatName}: CatAI 컴포넌트 확인");
                
                // AI 상태 확인
                Debug.Log($"✅ {cat.CatStats.CatName}: 현재 AI 상태 - {catAI.CurrentState}");
                
                // 목표 위치 설정 테스트
                Vector3 testTarget = transform.position + Vector3.right * 5f;
                catAI.SetTargetPosition(testTarget);
                Debug.Log($"✅ {cat.CatStats.CatName}: 목표 위치 설정 테스트 - {testTarget}");
                
                // AI 상태 변경 테스트
                catAI.SetAIState(CatAI.AIState.Moving);
                Debug.Log($"✅ {cat.CatStats.CatName}: AI 상태 변경 테스트 - {catAI.CurrentState}");
            }
            else
            {
                Debug.LogError($"❌ {cat.CatStats.CatName}: CatAI 컴포넌트가 없습니다!");
            }
        }
    }

    /// <summary>
    /// 스탯 시스템을 테스트합니다
    /// </summary>
    private void TestStatsSystem()
    {
        Debug.Log("4. 스탯 시스템 테스트");

        if (testCats == null) return;

        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            // 스탯 정보 출력
            Debug.Log($"✅ {cat.CatStats.CatName}: 스탯 정보");
            Debug.Log($"  - 속도: {cat.CurrentSpeed:F2}");
            Debug.Log($"  - 가속도: {cat.CurrentAcceleration:F2}");
            Debug.Log($"  - 체력: {cat.CurrentHealth}");
            Debug.Log($"  - 지능: {cat.CurrentIntelligence}");
            Debug.Log($"  - 힘: {cat.CurrentStrength}");

            // 스탯 수정 테스트
            float originalSpeed = cat.CurrentSpeed;
            cat.ModifyStats(speedModifier: 2f);
            Debug.Log($"✅ {cat.CatStats.CatName}: 속도 수정 테스트 (원래: {originalSpeed:F2} → 현재: {cat.CurrentSpeed:F2})");

            // 스탯 초기화
            cat.ModifyStats(speedModifier: -2f);
            Debug.Log($"✅ {cat.CatStats.CatName}: 속도 복원 테스트 (현재: {cat.CurrentSpeed:F2})");
        }
    }

    /// <summary>
    /// 고양이들을 트랙에 배치합니다
    /// </summary>
    [ContextMenu("고양이들을 트랙에 배치")]
    public void PlaceCatsOnTrack()
    {
        if (trackManager == null || testCats == null) return;

        Debug.Log("고양이들을 트랙에 배치합니다...");

        for (int i = 0; i < testCats.Length; i++)
        {
            if (testCats[i] == null) continue;

            // 트랙 시작점 근처에 배치
            float progress = 0f; // 시작점
            float offset = (i - testCats.Length / 2f) * 2f; // 고양이들 간격

            Vector3 position = trackManager.GetPositionAtProgress(progress, offset);
            testCats[i].transform.position = position;

            Debug.Log($"고양이 {i + 1} 배치: {position}");
        }
    }

    /// <summary>
    /// 모든 고양이의 AI를 활성화합니다
    /// </summary>
    [ContextMenu("AI 활성화")]
    public void EnableAllAI()
    {
        if (testCats == null) return;

        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            CatAI catAI = cat.GetComponent<CatAI>();
            if (catAI != null)
            {
                catAI.enabled = true;
                Debug.Log($"{cat.CatStats.CatName} AI 활성화");
            }
        }
    }

    /// <summary>
    /// 모든 고양이의 AI를 비활성화합니다
    /// </summary>
    [ContextMenu("AI 비활성화")]
    public void DisableAllAI()
    {
        if (testCats == null) return;

        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            CatAI catAI = cat.GetComponent<CatAI>();
            if (catAI != null)
            {
                catAI.enabled = false;
                Debug.Log($"{cat.CatStats.CatName} AI 비활성화");
            }
        }
    }

    /// <summary>
    /// AI 정보를 출력합니다
    /// </summary>
    [ContextMenu("AI 정보 출력")]
    public void PrintAIInfo()
    {
        if (testCats == null) return;

        Debug.Log("=== AI 정보 ===");
        foreach (Cat cat in testCats)
        {
            if (cat == null) continue;

            CatAI catAI = cat.GetComponent<CatAI>();
            if (catAI != null)
            {
                catAI.PrintAIInfo();
            }
        }
    }
}
