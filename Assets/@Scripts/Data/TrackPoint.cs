using UnityEngine;

/// <summary>
/// 트랙의 개별 포인트를 나타내는 클래스
/// </summary>
[System.Serializable]
public class TrackPoint : MonoBehaviour
{
    [Header("포인트 기본 정보")]
    [SerializeField] private int pointIndex; // 포인트 인덱스
    [SerializeField] private TrackPointType pointType; // 포인트 타입
    [SerializeField] private TrackPoint nextPoint; // 다음 포인트 참조

    [Header("트랙 중심선")]
    [SerializeField] private Vector3 centerPosition; // 트랙 중심선 위치
    [SerializeField] private float trackWidth = 10f; // 트랙 너비

    [Header("포인트 간 거리")]
    [SerializeField] private float distanceToNext; // 다음 포인트까지의 거리

    // 공개 프로퍼티들
    public int PointIndex => pointIndex;
    public TrackPointType PointType => pointType;
    public TrackPoint NextPoint => nextPoint;
    public Vector3 CenterPosition => centerPosition;
    public float TrackWidth => trackWidth;
    public float DistanceToNext => distanceToNext;

    private void Awake()
    {
        centerPosition = transform.position;
    }

    /// <summary>
    /// Scene 뷰에서 Gizmo를 그립니다
    /// </summary>
    private void OnDrawGizmos()
    {
        // 중심점 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPosition, 0.2f);

        // 원형 트랙 범위 그리기
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPosition, trackWidth / 2f);

        // 다음 포인트로의 연결선 그리기
        if (nextPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(centerPosition, nextPoint.centerPosition);
        }

        // 포인트 타입에 따른 색상 변경
        switch (pointType)
        {
            case TrackPointType.Start:
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(centerPosition, Vector3.one * 0.3f);
                break;
            case TrackPointType.End:
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(centerPosition, Vector3.one * 0.3f);
                break;
            case TrackPointType.Middle:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(centerPosition, Vector3.one * 0.2f);
                break;
        }
    }

    /// <summary>
    /// 선택된 상태에서 Gizmo를 그립니다
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 선택된 포인트는 더 밝게 표시
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(centerPosition, 0.15f);

        // 원형 트랙 범위를 더 두껍게 그리기
        Gizmos.color = Color.cyan;
        for (int i = 0; i < 32; i++)
        {
            float angle1 = i * 360f / 32f * Mathf.Deg2Rad;
            float angle2 = (i + 1) * 360f / 32f * Mathf.Deg2Rad;
            
            Vector3 pos1 = centerPosition + new Vector3(
                Mathf.Cos(angle1) * trackWidth / 2f,
                Mathf.Sin(angle1) * trackWidth / 2f,
                0f
            );
            Vector3 pos2 = centerPosition + new Vector3(
                Mathf.Cos(angle2) * trackWidth / 2f,
                Mathf.Sin(angle2) * trackWidth / 2f,
                0f
            );
            
            Gizmos.DrawLine(pos1, pos2);
        }
    }

    /// <summary>
    /// 트랙 너비를 설정합니다
    /// </summary>
    /// <param name="width">트랙 너비</param>
    public void SetTrackWidth(float width)
    {
        trackWidth = Mathf.Max(0f, width);
    }

    /// <summary>
    /// 원형 범위 내에서 오프셋을 적용한 위치를 반환합니다
    /// </summary>
    /// <param name="offset">중심선에서의 오프셋 (원형 범위 내)</param>
    /// <returns>계산된 위치</returns>
    public Vector3 GetPositionAtCircularOffset(float offset)
    {
        // 오프셋을 원형 범위로 변환 (0~360도)
        float angle = (offset + 180f) % 360f; // -180~180을 0~360으로 변환
        float radius = trackWidth / 2f;
        
        return GetPositionAtAngle(angle, radius);
    }

    /// <summary>
    /// 원형 범위 내에서 랜덤한 위치를 반환합니다
    /// </summary>
    /// <returns>원형 범위 내의 랜덤 위치</returns>
    public Vector3 GetRandomPositionInCircle()
    {
        // 원형 범위 내에서 랜덤한 각도와 거리 생성
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomRadius = Random.Range(0f, trackWidth / 2f);
        
        // 각도와 거리를 이용해 오프셋 계산
        float offsetX = Mathf.Cos(randomAngle) * randomRadius;
        float offsetY = Mathf.Sin(randomAngle) * randomRadius;
        
        return centerPosition + new Vector3(offsetX, offsetY, 0f);
    }

    /// <summary>
    /// 특정 각도에서의 위치를 반환합니다
    /// </summary>
    /// <param name="angle">각도 (도 단위)</param>
    /// <param name="radius">반지름 (0~trackWidth/2)</param>
    /// <returns>계산된 위치</returns>
    public Vector3 GetPositionAtAngle(float angle, float radius = -1f)
    {
        // radius가 -1이면 최대 반지름 사용
        if (radius < 0f) radius = trackWidth / 2f;
        
        // 반지름을 트랙 너비 범위로 제한
        radius = Mathf.Clamp(radius, 0f, trackWidth / 2f);
        
        // 각도를 라디안으로 변환
        float radian = angle * Mathf.Deg2Rad;
        
        // 각도와 반지름을 이용해 오프셋 계산
        float offsetX = Mathf.Cos(radian) * radius;
        float offsetY = Mathf.Sin(radian) * radius;
        
        return centerPosition + new Vector3(offsetX, offsetY, 0f);
    }

    /// <summary>
    /// 다음 포인트까지의 거리를 설정합니다
    /// </summary>
    /// <param name="distance">거리</param>
    public void SetDistanceToNext(float distance)
    {
        distanceToNext = Mathf.Max(0f, distance);
    }

    /// <summary>
    /// 포인트 타입을 설정합니다
    /// </summary>
    /// <param name="type">포인트 타입</param>
    public void SetPointType(TrackPointType type)
    {
        pointType = type;
    }

    /// <summary>
    /// 다음 포인트를 설정합니다
    /// </summary>
    /// <param name="next">다음 포인트</param>
    public void SetNextPoint(TrackPoint next)
    {
        nextPoint = next;
    }

    /// <summary>
    /// 다음 포인트까지의 거리를 자동으로 계산합니다
    /// </summary>
    public void CalculateDistanceToNext()
    {
        if (nextPoint == null)
        {
            distanceToNext = 0f;
            return;
        }

        distanceToNext = Vector3.Distance(centerPosition, nextPoint.centerPosition);
    }

    /// <summary>
    /// 중심선에서의 위치를 보간하여 반환합니다
    /// </summary>
    /// <param name="t">보간 값 (0.0 ~ 1.0)</param>
    /// <param name="offset">중심선에서의 오프셋 (좌우)</param>
    /// <returns>보간된 위치</returns>
    public Vector3 GetInterpolatedPosition(float t, float offset = 0f)
    {
        if (nextPoint == null)
        {
            return GetPositionAtCircularOffset(offset);
        }

        Vector3 currentPos = centerPosition;
        Vector3 nextPos = nextPoint.centerPosition;
        Vector3 interpolatedCenter = Vector3.Lerp(currentPos, nextPos, t);

        // 원형 범위에서 오프셋 적용
        float angle = (offset + 180f) % 360f;
        float radius = trackWidth / 2f;
        
        float offsetX = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float offsetY = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        
        return interpolatedCenter + new Vector3(offsetX, offsetY, 0f);
    }

    /// <summary>
    /// 트랙 경계 내에 있는지 확인합니다 (원형 범위)
    /// </summary>
    /// <param name="position">확인할 위치</param>
    /// <returns>트랙 경계 내에 있으면 true</returns>
    public bool IsPositionInTrack(Vector3 position)
    {
        Vector3 offset = position - centerPosition;
        float distanceFromCenter = Mathf.Sqrt(offset.x * offset.x + offset.y * offset.y); // 원형 거리
        return distanceFromCenter <= trackWidth / 2f;
    }

    /// <summary>
    /// 위치를 트랙 경계 내로 제한합니다 (원형 범위)
    /// </summary>
    /// <param name="position">제한할 위치</param>
    /// <returns>제한된 위치</returns>
    public Vector3 ClampPositionToTrack(Vector3 position)
    {
        Vector3 offset = position - centerPosition;
        float distanceFromCenter = Mathf.Sqrt(offset.x * offset.x + offset.y * offset.y);
        
        if (distanceFromCenter <= trackWidth / 2f)
        {
            return position; // 이미 트랙 내부에 있음
        }
        
        // 원형 경계로 제한
        float maxRadius = trackWidth / 2f;
        float clampedDistance = Mathf.Clamp(distanceFromCenter, 0f, maxRadius);
        float ratio = clampedDistance / distanceFromCenter;
        
        return centerPosition + new Vector3(offset.x * ratio, offset.y * ratio, offset.z);
    }

}

/// <summary>
/// 트랙 포인트 타입 열거형
/// </summary>
public enum TrackPointType
{
    Start,  // 시작점
    Middle, // 중간점
    End     // 끝점
}
