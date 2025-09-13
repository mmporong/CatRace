using UnityEngine;

/// <summary>
/// 고양이 AI 로직을 관리하는 클래스
/// </summary>
public class CatAI : MonoBehaviour
{
    [Header("AI 설정")]
    [SerializeField] private float updateInterval = 0.05f; // AI 업데이트 간격 (더 자주 업데이트)
    [SerializeField] private float decisionRadius = 2f; // 의사결정 반경
    [SerializeField] private float avoidanceRadius = 1.5f; // 회피 반경
    [SerializeField] private float trackPointReachDistance = 4f; // 트랙 포인트 도달 거리

    [Header("참조")]
    [SerializeField] private Cat cat; // 고양이 컴포넌트
    [SerializeField] private CatMovement movement; // 이동 컴포넌트
    [SerializeField] private TrackManager trackManager; // 트랙 매니저
    [SerializeField] private Collider2D catCollider; // 고양이 콜라이더

    [Header("AI 상태")]
    [SerializeField] private AIState currentState = AIState.Moving;
    [SerializeField] private Vector3 targetPosition; // 목표 위치
    [SerializeField] private float currentSpeed; // 현재 이동 속도
    [SerializeField] private Vector3 avoidanceDirection; // 회피 방향
    [SerializeField] private int currentTrackPointIndex = 0; // 현재 목표 트랙 포인트 인덱스

    // AI 상태 열거형
    public enum AIState
    {
        Moving,     // 이동 중
        Avoiding,   // 회피 중
        Overtaking, // 추월 중
        Defending,  // 방어 중
        Recovery    // 회복 중 (체력이 0이 되어 정지)
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
        catCollider = GetComponent<Collider2D>();

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

        // Recovery 상태 처리
        if (currentState == AIState.Recovery)
        {
            HandleRecoveryState();
            return;
        }

        // 1. 스탯 기반 속도 계산
        currentSpeed = CalculateMovementSpeed();
        
        // 2. 목표 위치 결정 (트랙 포인트 순서대로)
        targetPosition = DetermineTargetPosition(); // progress 파라미터는 사용하지 않음
        
        // 3. 장애물 회피 확인
        Vector3 avoidance = CheckAvoidance();
        
        // 4. 최종 이동 방향 결정
        Vector3 finalDirection = CalculateFinalDirection(targetPosition, avoidance);
        
        // 5. 이동 실행
        if (movement != null)
        {
            movement.SetTargetDirection(finalDirection);
            movement.SetSpeed(currentSpeed);
        }
    }

    /// <summary>
    /// Recovery 상태를 처리합니다
    /// </summary>
    private void HandleRecoveryState()
    {
        if (movement != null)
        {
            // 완전히 정지
            movement.SetTargetDirection(Vector3.zero);
            movement.SetSpeed(0f);
        }

        // 콜라이더 비활성화
        if (catCollider != null)
        {
            catCollider.enabled = false;
        }

        // 체력이 50% 이상 회복되면 Moving 상태로 전환
        float healthPercentage = (float)cat.CurrentHealth / cat.CatStats.Health;
        if (healthPercentage >= 0.5f)
        {
            currentState = AIState.Moving;
            if (movement != null)
            {
                movement.ResetExhaustion();
            }
            
            // 콜라이더 다시 활성화
            if (catCollider != null)
            {
                catCollider.enabled = true;
            }
            
            Debug.Log($"{cat.CatStats.CatName} 체력이 회복되어 다시 이동을 시작합니다!");
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
    /// 목표 위치를 결정합니다 (트랙 포인트 순서대로 방문)
    /// </summary>
    /// <param name="currentProgress">사용하지 않음 (호환성을 위해 유지)</param>
    /// <returns>목표 위치</returns>
    private Vector3 DetermineTargetPosition()
    {
        if (trackManager == null || trackManager.TrackPoints == null || trackManager.TrackPoints.Length == 0)
        {
            return transform.position;
        }

        // 현재 목표 트랙 포인트 가져오기
        TrackPoint targetTrackPoint = trackManager.TrackPoints[currentTrackPointIndex];
        if (targetTrackPoint == null)
        {
            return transform.position;
        }

        // 현재 목표 트랙 포인트에 도달했는지 확인
        float distanceToTarget = Vector3.Distance(transform.position, targetTrackPoint.CenterPosition);
        if (distanceToTarget <= trackPointReachDistance)
        {
            // 다음 트랙 포인트로 이동
            currentTrackPointIndex = (currentTrackPointIndex + 1) % trackManager.TrackPoints.Length;
            targetTrackPoint = trackManager.TrackPoints[currentTrackPointIndex];
            
            Debug.Log($"{gameObject.name}: 트랙 포인트 {currentTrackPointIndex}로 이동");
        }

        // 목표 트랙 포인트의 원형 범위 내에서 랜덤 위치 선택
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0f, targetTrackPoint.TrackWidth / 2f);
        
        Vector3 randomOffset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius,
            0f
        );

        return targetTrackPoint.CenterPosition + randomOffset;
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
        Debug.Log($"현재 트랙 포인트 인덱스: {currentTrackPointIndex}");
        if (trackManager != null && trackManager.TrackPoints != null && currentTrackPointIndex < trackManager.TrackPoints.Length)
        {
            Debug.Log($"목표 트랙 포인트: {trackManager.TrackPoints[currentTrackPointIndex].name}");
        }
    }
}
