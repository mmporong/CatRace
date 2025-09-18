using UnityEngine;

/// <summary>
/// 부모 오브젝트의 이동 방향에 따라 자식 오브젝트를 좌우로 flip하는 스크립트
/// </summary>
public class FlipWithParent : MonoBehaviour
{
    [Header("Flip 설정")]
    [SerializeField] private bool flipOnX = true; // X축 기준으로 flip
    [SerializeField] private bool flipOnY = false; // Y축 기준으로 flip
    [SerializeField] private float flipThreshold = 0.1f; // flip을 위한 최소 이동량
    [SerializeField] private bool useSmoothFlip = true; // 부드러운 flip 사용
    [SerializeField] private float flipSpeed = 10f; // flip 속도

    [Header("참조")]
    [SerializeField] private Transform parentTransform; // 부모 Transform (자동으로 찾음)
    [SerializeField] private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 (자동으로 찾음)

    [Header("상태")]
    [SerializeField] private Vector3 lastParentPosition; // 부모의 이전 위치
    [SerializeField] private bool isFlipped = false; // 현재 flip 상태
    [SerializeField] private Vector3 targetScale; // 목표 스케일

    // 공개 프로퍼티들
    public bool IsFlipped => isFlipped;
    public Vector3 TargetScale => targetScale;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        // 부모 Transform 찾기
        if (parentTransform == null)
        {
            parentTransform = transform.parent;
        }

        // SpriteRenderer 찾기
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 초기 위치 저장
        if (parentTransform != null)
        {
            lastParentPosition = parentTransform.position;
        }

        // 초기 스케일 설정
        targetScale = transform.localScale;
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        if (parentTransform == null) return;

        // 부모의 이동 방향 확인
        Vector3 parentMovement = parentTransform.position - lastParentPosition;
        
        // 이동량이 임계값보다 크면 flip 처리
        if (parentMovement.magnitude > flipThreshold)
        {
            CheckAndFlip(parentMovement);
        }

        // 부드러운 flip 적용
        if (useSmoothFlip)
        {
            ApplySmoothFlip();
        }

        // 현재 위치를 다음 프레임을 위해 저장
        lastParentPosition = parentTransform.position;
    }

    /// <summary>
    /// 이동 방향을 확인하고 flip을 결정합니다
    /// </summary>
    /// <param name="movement">부모의 이동 벡터</param>
    private void CheckAndFlip(Vector3 movement)
    {
        bool shouldFlip = false;

        // X축 flip 확인
        if (flipOnX && Mathf.Abs(movement.x) > flipThreshold)
        {
            bool movingRight = movement.x > 0;
            bool currentlyFlipped = targetScale.x < 0;
            
            // 이동 방향과 현재 flip 상태가 다르면 flip
            if (movingRight && currentlyFlipped)
            {
                shouldFlip = true;
            }
            else if (!movingRight && !currentlyFlipped)
            {
                shouldFlip = true;
            }
        }

        // Y축 flip 확인
        if (flipOnY && Mathf.Abs(movement.y) > flipThreshold)
        {
            bool movingUp = movement.y > 0;
            bool currentlyFlipped = targetScale.y < 0;
            
            // 이동 방향과 현재 flip 상태가 다르면 flip
            if (movingUp && currentlyFlipped)
            {
                shouldFlip = true;
            }
            else if (!movingUp && !currentlyFlipped)
            {
                shouldFlip = true;
            }
        }

        // flip 적용
        if (shouldFlip)
        {
            Flip();
        }
    }

    /// <summary>
    /// 오브젝트를 flip합니다
    /// </summary>
    private void Flip()
    {
        Vector3 currentScale = targetScale;

        if (flipOnX)
        {
            currentScale.x = -currentScale.x;
        }

        if (flipOnY)
        {
            currentScale.y = -currentScale.y;
        }

        targetScale = currentScale;
        isFlipped = !isFlipped;

        // 부드러운 flip을 사용하지 않으면 즉시 적용
        if (!useSmoothFlip)
        {
            transform.localScale = targetScale;
        }
    }

    /// <summary>
    /// 부드러운 flip을 적용합니다
    /// </summary>
    private void ApplySmoothFlip()
    {
        if (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, flipSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 즉시 flip합니다
    /// </summary>
    /// <param name="flipX">X축 flip 여부</param>
    /// <param name="flipY">Y축 flip 여부</param>
    public void ForceFlip(bool flipX = true, bool flipY = false)
    {
        Vector3 currentScale = transform.localScale;

        if (flipX)
        {
            currentScale.x = -currentScale.x;
        }

        if (flipY)
        {
            currentScale.y = -currentScale.y;
        }

        transform.localScale = currentScale;
        targetScale = currentScale;
        isFlipped = !isFlipped;
    }

    /// <summary>
    /// flip 상태를 리셋합니다
    /// </summary>
    public void ResetFlip()
    {
        Vector3 resetScale = Vector3.one;
        transform.localScale = resetScale;
        targetScale = resetScale;
        isFlipped = false;
    }

    /// <summary>
    /// 부모 Transform을 설정합니다
    /// </summary>
    /// <param name="parent">새로운 부모 Transform</param>
    public void SetParent(Transform parent)
    {
        parentTransform = parent;
        if (parent != null)
        {
            lastParentPosition = parent.position;
        }
    }

    /// <summary>
    /// flip 설정을 변경합니다
    /// </summary>
    /// <param name="flipX">X축 flip 여부</param>
    /// <param name="flipY">Y축 flip 여부</param>
    public void SetFlipAxes(bool flipX, bool flipY)
    {
        flipOnX = flipX;
        flipOnY = flipY;
    }

    /// <summary>
    /// flip 정보를 디버그 출력합니다
    /// </summary>
    [ContextMenu("Flip 정보 출력")]
    public void PrintFlipInfo()
    {
        Debug.Log($"=== {gameObject.name} Flip 정보 ===");
        Debug.Log($"현재 flip 상태: {isFlipped}");
        Debug.Log($"목표 스케일: {targetScale}");
        Debug.Log($"현재 스케일: {transform.localScale}");
        Debug.Log($"X축 flip: {flipOnX}, Y축 flip: {flipOnY}");
        Debug.Log($"부드러운 flip: {useSmoothFlip}");
        Debug.Log($"Flip 속도: {flipSpeed}");
    }
}
