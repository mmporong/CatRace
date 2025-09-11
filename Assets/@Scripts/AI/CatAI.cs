using UnityEngine;

/// <summary>
/// 고양이 AI 로직을 관리하는 클래스
/// </summary>
public class CatAI : MonoBehaviour
{
    [Header("AI 설정")]
    [SerializeField] private float updateInterval = 0.1f; // AI 업데이트 간격
    [SerializeField] private float decisionRadius = 2f; // 의사결정 반경
    [SerializeField] private float avoidanceRadius = 1.5f; // 회피 반경

    [Header("참조")]
    [SerializeField] private Cat cat; // 고양이 컴포넌트
    [SerializeField] private CatMovement movement; // 이동 컴포넌트
    [SerializeField] private TrackManager trackManager; // 트랙 매니저

    [Header("AI 상태")]
    [SerializeField] private AIState currentState = AIState.Moving;
    [SerializeField] private Vector3 targetPosition; // 목표 위치
    [SerializeField] private float currentSpeed; // 현재 이동 속도
    [SerializeField] private Vector3 avoidanceDirection; // 회피 방향

    // AI 상태 열거형
    public enum AIState
    {
        Moving,     // 이동 중
        Avoiding,   // 회피 중
        Overtaking, // 추월 중
        Defending   // 방어 중
    }

    // 공개 프로퍼티들
    public AIState CurrentState => currentState;
    public Vector3 TargetPosition => targetPosition;
    public float CurrentSpeed => currentSpeed;

    private float lastUpdateTime;
    private Vector3 lastPosition;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        cat = GetComponent<Cat>();
        movement = GetComponent<CatMovement>();
        trackManager = FindFirstObjectByType<TrackManager>();

        if (cat == null)
        {
            Debug.LogError($"{gameObject.name}: Cat 컴포넌트를 찾을 수 없습니다!");
        }

        if (movement == null)
        {
            Debug.LogError($"{gameObject.name}: CatMovement 컴포넌트를 찾을 수 없습니다!");
        }

        if (trackManager == null)
        {
            Debug.LogError($"{gameObject.name}: TrackManager를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// AI 업데이트
    /// </summary>
    private void Update()
    {
        // 일정 간격으로 AI 의사결정 업데이트
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateAI();
            lastUpdateTime = Time.time;
        }

        // 매 프레임 이동 처리
        if (movement != null)
        {
            movement.UpdateMovement();
        }
    }

    /// <summary>
    /// AI 로직을 업데이트합니다
    /// </summary>
    private void UpdateAI()
    {
        if (cat == null || trackManager == null) return;

        // 1. 현재 위치에서 트랙 진행도 계산
        float progress = trackManager.CalculateProgress(transform.position);
        
        // 2. 스탯 기반 속도 계산
        currentSpeed = CalculateMovementSpeed();
        
        // 3. 목표 위치 결정
        targetPosition = DetermineTargetPosition(progress);
        
        // 4. 장애물 회피 확인
        Vector3 avoidance = CheckAvoidance();
        
        // 5. 최종 이동 방향 결정
        Vector3 finalDirection = CalculateFinalDirection(targetPosition, avoidance);
        
        // 6. 이동 실행
        if (movement != null)
        {
            movement.SetTargetDirection(finalDirection);
            movement.SetSpeed(currentSpeed);
        }
    }

    /// <summary>
    /// 스탯 기반 이동 속도를 계산합니다
    /// </summary>
    /// <returns>계산된 이동 속도</returns>
    private float CalculateMovementSpeed()
    {
        if (cat == null) return 1f;

        // 기본 속도 (속도 스탯 기반)
        float baseSpeed = cat.CurrentSpeed;
        
        // 가속도 스탯으로 속도 변동 추가
        float accelerationFactor = cat.CurrentAcceleration / 10f; // 0.1 ~ 2.0
        float speedVariation = Random.Range(-accelerationFactor, accelerationFactor);
        
        // 지능 스탯으로 속도 안정성 추가
        float intelligenceFactor = cat.CurrentIntelligence / 100f; // 0.01 ~ 1.0
        float stability = 1f - (speedVariation * (1f - intelligenceFactor));
        
        // 최종 속도 계산
        float finalSpeed = baseSpeed * stability;
        
        // 속도 범위 제한 (1 ~ 20)
        return Mathf.Clamp(finalSpeed, 1f, 20f);
    }

    /// <summary>
    /// 목표 위치를 결정합니다
    /// </summary>
    /// <param name="currentProgress">현재 트랙 진행도</param>
    /// <returns>목표 위치</returns>
    private Vector3 DetermineTargetPosition(float currentProgress)
    {
        if (trackManager == null) return transform.position;

        // 다음 목표 진행도 계산 (속도에 따라 조절)
        float speedFactor = currentSpeed / 10f; // 0.1 ~ 2.0
        float nextProgress = currentProgress + (speedFactor * 0.01f); // 진행도 증가량
        
        // 진행도 제한
        nextProgress = Mathf.Clamp01(nextProgress);

        // 원형 범위 내에서 랜덤 오프셋 추가
        float randomOffset = Random.Range(-180f, 180f);
        
        // 목표 위치 계산
        return trackManager.GetPositionAtProgress(nextProgress, randomOffset);
    }

    /// <summary>
    /// 장애물 회피를 확인합니다
    /// </summary>
    /// <returns>회피 방향</returns>
    private Vector3 CheckAvoidance()
    {
        // 주변 고양이들 확인
        Collider2D[] nearbyCats = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        
        Vector3 avoidanceVector = Vector3.zero;
        int avoidanceCount = 0;

        foreach (Collider2D col in nearbyCats)
        {
            if (col.gameObject != gameObject && col.CompareTag("Cat"))
            {
                Vector3 direction = transform.position - col.transform.position;
                float distance = direction.magnitude;
                
                if (distance > 0f)
                {
                    // 거리에 반비례하는 회피 강도
                    float avoidanceStrength = (avoidanceRadius - distance) / avoidanceRadius;
                    avoidanceVector += direction.normalized * avoidanceStrength;
                    avoidanceCount++;
                }
            }
        }

        // 평균 회피 방향 계산
        if (avoidanceCount > 0)
        {
            avoidanceVector /= avoidanceCount;
            currentState = AIState.Avoiding;
        }
        else
        {
            currentState = AIState.Moving;
        }

        return avoidanceVector;
    }

    /// <summary>
    /// 최종 이동 방향을 계산합니다
    /// </summary>
    /// <param name="targetPos">목표 위치</param>
    /// <param name="avoidance">회피 방향</param>
    /// <returns>최종 이동 방향</returns>
    private Vector3 CalculateFinalDirection(Vector3 targetPos, Vector3 avoidance)
    {
        // 목표 방향 계산
        Vector3 targetDirection = (targetPos - transform.position).normalized;
        
        // 회피 방향과 목표 방향을 결합
        Vector3 finalDirection = targetDirection + avoidance * 0.5f;
        
        // 정규화
        return finalDirection.normalized;
    }

    /// <summary>
    /// AI 상태를 설정합니다
    /// </summary>
    /// <param name="newState">새로운 상태</param>
    public void SetAIState(AIState newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// 목표 위치를 설정합니다
    /// </summary>
    /// <param name="position">목표 위치</param>
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    /// <summary>
    /// AI 정보를 디버그 출력합니다
    /// </summary>
    [ContextMenu("AI 정보 출력")]
    public void PrintAIInfo()
    {
        Debug.Log($"=== {gameObject.name} AI 정보 ===");
        Debug.Log($"현재 상태: {currentState}");
        Debug.Log($"목표 위치: {targetPosition}");
        Debug.Log($"현재 속도: {currentSpeed:F2}");
        Debug.Log($"회피 방향: {avoidanceDirection}");
    }
}
