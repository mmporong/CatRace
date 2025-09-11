using UnityEngine;

/// <summary>
/// 고양이 이동을 처리하는 클래스
/// </summary>
public class CatMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f; // 기본 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도
    [SerializeField] private float smoothTime = 0.1f; // 부드러운 이동을 위한 시간

    [Header("물리 설정")]
    [SerializeField] private float maxVelocity = 15f; // 최대 속도
    [SerializeField] private float drag = 5f; // 저항력

    [Header("참조")]
    [SerializeField] private Rigidbody2D rb; // 2D 물리 컴포넌트
    [SerializeField] private Cat cat; // 고양이 컴포넌트

    [Header("이동 상태")]
    [SerializeField] private Vector3 targetDirection; // 목표 방향
    [SerializeField] private Vector3 currentVelocity; // 현재 속도
    [SerializeField] private bool isMoving = false; // 이동 중인지 여부

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
            rb.linearDamping = drag;
            rb.angularDamping = 10f;
        }
    }

    /// <summary>
    /// 이동을 업데이트합니다
    /// </summary>
    public void UpdateMovement()
    {
        if (rb == null) return;

        // 현재 속도 계산
        currentVelocity = rb.linearVelocity;
        
        // 이동 중인지 확인
        isMoving = currentVelocity.magnitude > 0.1f;

        // 속도 제한
        if (currentVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = currentVelocity.normalized * maxVelocity;
        }

        // 고양이 방향 회전
        if (currentVelocity.magnitude > 0.1f)
        {
            RotateTowardsDirection(currentVelocity);
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
    /// 방향으로 회전합니다
    /// </summary>
    /// <param name="direction">회전할 방향</param>
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;

        // 목표 각도 계산
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 부드러운 회전
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
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
    }

    /// <summary>
    /// 스탯을 적용합니다
    /// </summary>
    public void ApplyStats()
    {
        if (cat == null) return;

        // 속도 스탯에 따른 이동 속도 조절
        float speedMultiplier = cat.CurrentSpeed / 10f; // 0.1 ~ 2.0
        moveSpeed = 5f * speedMultiplier; // 기본 속도 5에서 스탯에 따라 조절

        // 가속도 스탯에 따른 회전 속도 조절
        float accelerationMultiplier = cat.CurrentAcceleration / 10f; // 0.1 ~ 2.0
        rotationSpeed = 10f * accelerationMultiplier;

        // 체력 스탯에 따른 최대 속도 조절
        float healthMultiplier = cat.CurrentHealth / 100f; // 0.01 ~ 1.0
        maxVelocity = 15f * healthMultiplier;

        Debug.Log($"{cat.CatStats.CatName} 스탯 적용: 속도({moveSpeed:F2}) 회전({rotationSpeed:F2}) 최대속도({maxVelocity:F2})");
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
