using UnityEngine;

/// <summary>
/// 고양이 이동을 처리하는 클래스
/// </summary>
public class CatMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 1f; // 기본 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도
    [SerializeField] private float smoothTime = 0.1f; // 부드러운 이동을 위한 시간

    [Header("체력 시스템")]
    [SerializeField] private float healthDrainRate = 5f; // 달리는 동안 체력 감소 속도 (초당)
    [SerializeField] private float healthRecoveryRate = 10f; // 정지 상태에서 체력 회복 속도 (초당)
    [SerializeField] private float originalMoveSpeed; // 원래 이동 속도 저장
    [SerializeField] private bool isExhausted = false; // 체력이 0이 되어 속도가 절반으로 줄어든 상태

    // 체력 시스템을 위한 누적 변수들
    private float healthDrainAccumulator = 0f; // 체력 감소 누적값
    private float healthRecoveryAccumulator = 0f; // 체력 회복 누적값

    [Header("물리 설정")]
    [SerializeField] private float maxVelocity = 15f; // 최대 속도
    [SerializeField] private float drag = 5f; // 저항력

    [Header("참조")]
    [SerializeField] private Rigidbody2D rb; // 2D 물리 컴포넌트
    [SerializeField] private Cat cat; // 고양이 컴포넌트
    [SerializeField] private CatAI catAI; // 고양이 AI 컴포넌트

    [Header("이동 상태")]
    [SerializeField] private Vector3 targetDirection; // 목표 방향
    [SerializeField] private Vector3 currentVelocity; // 현재 속도
    [SerializeField] private bool isMoving = false; // 이동 중인지 여부
    [SerializeField] private bool hasTriggeredRunAnimation = false; // Run 애니메이션 트리거 여부

    // 부드러운 이동을 위한 변수들
    private Vector3 velocitySmoothing;
    private Vector3 lastPosition;

    // 공개 프로퍼티들
    public bool IsMoving => isMoving;
    public Vector3 CurrentVelocity => currentVelocity;
    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        rb = GetComponent<Rigidbody2D>();
        cat = GetComponent<Cat>();
        catAI = GetComponent<CatAI>();

        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}: Rigidbody2D 컴포넌트를 찾을 수 없습니다!");
        }

        if (cat == null)
        {
            Debug.LogError($"{gameObject.name}: Cat 컴포넌트를 찾을 수 없습니다!");
        }

        // 물리 설정
        if (rb != null)
        {
            rb.linearDamping = 0f; // 직접 속도 제어하므로 저항 제거
            rb.angularDamping = 5f; // 회전 저항만 유지
        }

        // 원래 이동 속도 저장
        ApplyStats();
    }

    /// <summary>
    /// 이동을 업데이트합니다
    /// </summary>
    public void UpdateMovement()
    {
        if (rb == null) return;

        // 목표 방향이 설정되어 있으면 목표 속도로 직접 설정
        if (targetDirection.magnitude > 0.1f)
        {
            Vector3 targetVelocity = targetDirection * moveSpeed;

            // 부드러운 속도 전환
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 5f);

            // 속도 제한 적용
            if (newVelocity.magnitude > maxVelocity)
            {
                newVelocity = newVelocity.normalized * maxVelocity;
            }

            rb.linearVelocity = newVelocity;
        }
        else
        {
            // 목표 방향이 없으면 점진적으로 정지
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * 3f);
            rb.linearVelocity = newVelocity;
        }

        // 현재 속도 계산
        currentVelocity = rb.linearVelocity;

        // 이동 중인지 확인
        bool wasMoving = isMoving;
        isMoving = currentVelocity.magnitude > 0.1f;

        // 이동 시작 시 Run 애니메이션 트리거 (한 번만)
        if (!wasMoving && isMoving && cat != null && !hasTriggeredRunAnimation)
        {
            cat.StartRunning();
            hasTriggeredRunAnimation = true;
        }
        
        // 정지 시 트리거 리셋
        if (wasMoving && !isMoving)
        {
            hasTriggeredRunAnimation = false;
        }

        // 고양이 방향 회전 및 플립
        if (currentVelocity.magnitude > 0.1f)
        {
            FlipTowardsDirection(currentVelocity);
        }

        // 체력 관리
        UpdateHealthSystem();
    }

    /// <summary>
    /// 체력 시스템을 업데이트합니다
    /// </summary>
    private void UpdateHealthSystem()
    {
        if (cat == null) return;

        // 달리는 동안 체력 감소
        if (isMoving)
        {
            healthDrainAccumulator += healthDrainRate * Time.deltaTime;
            healthRecoveryAccumulator = 0f; // 이동 중이면 회복 누적값 리셋

            // 누적값이 1 이상이 되면 체력 감소
            if (healthDrainAccumulator >= 1f)
            {
                int healthLoss = Mathf.FloorToInt(healthDrainAccumulator);
                int newHealth = Mathf.Max(0, cat.CurrentHealth - healthLoss);
                cat.SetStat(StatType.Health, newHealth);
                healthDrainAccumulator -= healthLoss; // 사용한 만큼 누적값에서 빼기

                // 체력이 0이 되면 Recovery 상태로 전환
                if (newHealth <= 0 && !isExhausted)
                {
                    isExhausted = true;
                    if (catAI != null)
                    {
                        catAI.SetAIState(CatAI.AIState.Recovery);
                    }
                    Debug.Log($"{cat.CatStats.CatName} 체력이 고갈되어 회복 상태로 전환됩니다!");
                }
            }
        }
        else
        {
            // 정지 상태에서는 체력 회복
            if (cat.CurrentHealth < cat.CatStats.Health)
            {
                healthRecoveryAccumulator += healthRecoveryRate * Time.deltaTime;
                healthDrainAccumulator = 0f; // 정지 중이면 감소 누적값 리셋

                // 누적값이 1 이상이 되면 체력 회복
                if (healthRecoveryAccumulator >= 1f)
                {
                    int healthGain = Mathf.FloorToInt(healthRecoveryAccumulator);
                    int newHealth = Mathf.Min(cat.CatStats.Health, cat.CurrentHealth + healthGain);
                    cat.SetStat(StatType.Health, newHealth);
                    healthRecoveryAccumulator -= healthGain; // 사용한 만큼 누적값에서 빼기

                    // 체력이 최대치가 되면 원래 속도로 복구
                    if (newHealth >= cat.CatStats.Health && isExhausted)
                    {
                        cat.SetStat(StatType.Speed, originalMoveSpeed);
                        isExhausted = false;
                        
                        // 체력이 완전히 회복되면 Run 애니메이션 실행
                        cat.StartRunning();
                        
                        Debug.Log($"{cat.CatStats.CatName} 체력이 회복되어 원래 속도로 돌아갑니다!");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 목표 방향을 설정합니다
    /// </summary>
    /// <param name="direction">목표 방향</param>
    public void SetTargetDirection(Vector3 direction)
    {
        targetDirection = direction.normalized;
    }

    /// <summary>
    /// 이동 속도를 설정합니다
    /// </summary>
    /// <param name="speed">이동 속도</param>
    public void SetSpeed(float speed)
    {
        moveSpeed = Mathf.Clamp(speed, 1f, 20f);
    }

    /// <summary>
    /// 목표 위치로 이동합니다
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    public void MoveTowardsTarget(Vector3 targetPosition)
    {
        if (rb == null) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        SetTargetDirection(direction);

        // 물리 기반 이동
        Vector3 force = direction * moveSpeed;
        rb.AddForce(force, ForceMode2D.Force);
    }

    /// <summary>
    /// 부드러운 이동을 적용합니다
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    public void SmoothMoveTowards(Vector3 targetPosition)
    {
        if (rb == null) return;

        Vector3 direction = (targetPosition - transform.position).normalized;

        // 부드러운 속도 계산
        Vector3 targetVelocity = direction * moveSpeed;
        Vector3 smoothVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocitySmoothing, smoothTime);

        // 물리 적용
        rb.linearVelocity = smoothVelocity;
    }

    /// <summary>
    /// 이동 방향에 따라 고양이를 좌우로 회전합니다 (Y축 180도 회전)
    /// </summary>
    /// <param name="direction">이동 방향</param>
    private void FlipTowardsDirection(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;

        // Model 자식 오브젝트 찾기
        Transform modelTransform = transform.Find("Model");
        if (modelTransform == null) return;

        // X 방향에 따라 Y축 회전으로 좌우 전환
        if (direction.x > 0.1f)
        {
            // 오른쪽으로 이동 - 정상 방향 (Y = 0도)
            modelTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction.x < -0.1f)
        {
            // 왼쪽으로 이동 - 180도 회전 (Y = 180도)
            modelTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// 즉시 정지합니다
    /// </summary>
    public void Stop()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isMoving = false;
        targetDirection = Vector3.zero;
        hasTriggeredRunAnimation = false; // 트리거 리셋
    }

    /// <summary>
    /// 스탯을 적용합니다
    /// </summary>
    public void ApplyStats()
    {
        if (cat == null) return;

        // 속도 스탯에 따른 이동 속도 조절
        float speedMultiplier = cat.CurrentSpeed / 10f; // 0.1 ~ 2.0
        float baseSpeed = 5f * speedMultiplier; // 기본 속도 5에서 스탯에 따라 조절

        // 힘 스탯에 따른 이동 속도 보너스 (최대 +50%)
        float strengthMultiplier = cat.CurrentStrength / 100f; // 0.01 ~ 1.0
        float strengthBonus = 1f + strengthMultiplier * 0.5f; // 1.0 ~ 1.5
        float modifiedSpeed = baseSpeed * strengthBonus;

        // 체력이 고갈되지 않은 상태에서만 원래 속도 업데이트
        if (!isExhausted)
        {
            moveSpeed = modifiedSpeed;
            originalMoveSpeed = modifiedSpeed;
        }

        // 가속도 스탯에 따른 회전 속도 조절
        float accelerationMultiplier = cat.CurrentAcceleration / 10f; // 0.1 ~ 2.0
        rotationSpeed = 10f * accelerationMultiplier;

        // 체력 스탯에 따른 최대 속도 조절 (현재 체력이 아닌 최대 체력 기준)
        float healthMultiplier = cat.CatStats.Health / 100f; // 0.01 ~ 1.0
        maxVelocity = 15f * healthMultiplier;

        Debug.Log($"{cat.CatStats.CatName} 스탯 적용: 이동속도({moveSpeed:F2}, 힘보너스 {strengthBonus:F2}) 회전({rotationSpeed:F2}) 최대속도({maxVelocity:F2})");
    }

    /// <summary>
    /// 특정 방향으로 힘을 가합니다
    /// </summary>
    /// <param name="direction">힘을 가할 방향</param>
    /// <param name="force">힘의 크기</param>
    public void AddForce(Vector3 direction, float force)
    {
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// 특정 방향으로 힘을 가합니다 (ForceMode2D 선택)
    /// </summary>
    /// <param name="direction">힘을 가할 방향</param>
    /// <param name="force">힘의 크기</param>
    /// <param name="mode">ForceMode2D</param>
    public void AddForce(Vector3 direction, float force, ForceMode2D mode)
    {
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, mode);
        }
    }

    /// <summary>
    /// 현재 위치에서 특정 거리만큼 이동합니다
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="distance">이동 거리</param>
    public void MoveByDistance(Vector3 direction, float distance)
    {
        Vector3 targetPosition = transform.position + direction.normalized * distance;
        MoveTowardsTarget(targetPosition);
    }

    /// <summary>
    /// 피로 상태를 리셋합니다
    /// </summary>
    public void ResetExhaustion()
    {
        isExhausted = false;
    }

    /// <summary>
    /// 이동 정보를 디버그 출력합니다
    /// </summary>
    [ContextMenu("이동 정보 출력")]
    public void PrintMovementInfo()
    {
        Debug.Log($"=== {gameObject.name} 이동 정보 ===");
        Debug.Log($"이동 중: {isMoving}");
        Debug.Log($"현재 속도: {currentVelocity.magnitude:F2}");
        Debug.Log($"목표 방향: {targetDirection}");
        Debug.Log($"이동 속도: {moveSpeed:F2}");
        Debug.Log($"최대 속도: {maxVelocity:F2}");
    }
}
