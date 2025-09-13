using UnityEngine;

/// <summary>
/// 카메라가 고양이를 따라가는 스크립트
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 대상")]
    [SerializeField] private Transform target; // 따라갈 대상 (고양이)
    [SerializeField] private bool autoFindTarget = true; // 자동으로 고양이 찾기

    [Header("카메라 설정")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // 카메라 오프셋
    [SerializeField] private float followSpeed = 5f; // 따라가는 속도
    [SerializeField] private float rotationSpeed = 2f; // 회전 속도

    [Header("부드러운 이동")]
    [SerializeField] private bool useSmoothFollow = true; // 부드러운 이동 사용
    [SerializeField] private float smoothTime = 0.3f; // 부드러운 이동 시간

    [Header("경계 제한")]
    [SerializeField] private bool useBounds = false; // 경계 제한 사용
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f); // 최소 경계
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f); // 최대 경계

    [Header("줌 설정")]
    [SerializeField] private bool enableZoom = true; // 줌 기능 활성화
    [SerializeField] private float minZoom = 3f; // 최소 줌
    [SerializeField] private float maxZoom = 10f; // 최대 줌
    [SerializeField] private float zoomSpeed = 2f; // 줌 속도
    [SerializeField] private float targetZoom = 5f; // 목표 줌

    // 내부 변수들
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private float currentZoom;

    // 공개 프로퍼티들
    public Transform Target => target;
    public bool IsFollowing => target != null;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError($"{gameObject.name}: Camera 컴포넌트를 찾을 수 없습니다!");
        }

        currentZoom = cam.orthographicSize;
    }

    /// <summary>
    /// 시작
    /// </summary>
    private void Start()
    {
        if (autoFindTarget)
        {
            FindTarget();
        }
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        // 카메라 위치 업데이트
        UpdateCameraPosition();

        // 줌 업데이트
        if (enableZoom)
        {
            UpdateZoom();
        }
    }

    /// <summary>
    /// 카메라 위치를 업데이트합니다
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = target.position + offset;

        // 경계 제한 적용
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // 부드러운 이동 또는 즉시 이동
        if (useSmoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 줌을 업데이트합니다
    /// </summary>
    private void UpdateZoom()
    {
        if (cam == null) return;

        // 목표 줌으로 부드럽게 변경
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    /// <summary>
    /// 따라갈 대상을 찾습니다
    /// </summary>
    public void FindTarget()
    {
        // "Cat" 태그를 가진 오브젝트 찾기
        GameObject catObject = GameObject.FindGameObjectWithTag("Cat");
        if (catObject != null)
        {
            SetTarget(catObject.transform);
            Debug.Log($"카메라가 {catObject.name}을 따라가기 시작합니다.");
        }
        else
        {
            Debug.LogWarning("따라갈 고양이를 찾을 수 없습니다! 'Cat' 태그가 있는 오브젝트가 있는지 확인해주세요.");
        }
    }

    /// <summary>
    /// 따라갈 대상을 설정합니다
    /// </summary>
    /// <param name="newTarget">새로운 대상</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log($"카메라가 {target.name}을 따라가기 시작합니다.");
        }
    }

    /// <summary>
    /// 따라가기를 중지합니다
    /// </summary>
    public void StopFollowing()
    {
        target = null;
        Debug.Log("카메라 따라가기를 중지했습니다.");
    }

    /// <summary>
    /// 줌을 설정합니다
    /// </summary>
    /// <param name="zoom">줌 값</param>
    public void SetZoom(float zoom)
    {
        targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    /// <summary>
    /// 오프셋을 설정합니다
    /// </summary>
    /// <param name="newOffset">새로운 오프셋</param>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    /// <summary>
    /// 경계를 설정합니다
    /// </summary>
    /// <param name="min">최소 경계</param>
    /// <param name="max">최대 경계</param>
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }

    /// <summary>
    /// 경계 제한을 비활성화합니다
    /// </summary>
    public void DisableBounds()
    {
        useBounds = false;
    }

    /// <summary>
    /// 카메라 정보를 디버그 출력합니다
    /// </summary>
    [ContextMenu("카메라 정보 출력")]
    public void PrintCameraInfo()
    {
        Debug.Log($"=== {gameObject.name} 카메라 정보 ===");
        Debug.Log($"따라가는 대상: {(target != null ? target.name : "없음")}");
        Debug.Log($"현재 위치: {transform.position}");
        Debug.Log($"오프셋: {offset}");
        Debug.Log($"따라가는 속도: {followSpeed}");
        Debug.Log($"부드러운 이동: {useSmoothFollow}");
        Debug.Log($"현재 줌: {currentZoom:F2}");
        Debug.Log($"목표 줌: {targetZoom:F2}");
        Debug.Log($"경계 제한: {useBounds}");
        if (useBounds)
        {
            Debug.Log($"경계 범위: {minBounds} ~ {maxBounds}");
        }
    }

    /// <summary>
    /// Scene 뷰에서 Gizmo를 그립니다
    /// </summary>
    private void OnDrawGizmos()
    {
        if (target == null) return;

        // 대상으로의 선 그리기
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);

        // 경계 그리기
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, 0f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
