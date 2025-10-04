using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 트랙을 관리하는 메인 시스템
/// 싱글톤 패턴으로 구현
/// </summary>
public class TrackManager : Singleton<TrackManager>
{
    [Header("트랙 설정")]
    [SerializeField] private float trackWidth = 10f; // 트랙 너비

    [Header("트랙 포인트")]
    [SerializeField] private TrackPoint[] trackPoints; // 트랙 포인트 배열 (Inspector에서 직접 설정)

    [Header("트랙 데이터")]
    [SerializeField] private float totalTrackDistance; // 총 트랙 거리

    // 공개 프로퍼티들
    public float TrackWidth => trackWidth;
    public TrackPoint[] TrackPoints => trackPoints;
    public float TotalTrackDistance => totalTrackDistance;
    public int NumberOfPoints => trackPoints != null ? trackPoints.Length : 0;

    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    protected override void InitializeSingleton()
    {
        base.InitializeSingleton();
        InitializeTrack();
    }

    /// <summary>
    /// 트랙을 초기화합니다
    /// </summary>
    private void InitializeTrack()
    {
        if (trackPoints == null || trackPoints.Length == 0)
        {
            Debug.LogError("트랙 포인트가 설정되지 않았습니다! Inspector에서 Track Points 배열을 설정해주세요.");
            return;
        }

        // 포인트들 간 연결 관계 설정
        SetupTrackConnections();

        // 거리 계산
        CalculateTotalDistance();

        Debug.Log($"트랙 초기화 완료 - 트랙 너비: {trackWidth}, 포인트: {NumberOfPoints}, 총 거리: {totalTrackDistance:F2}");
    }

    /// <summary>
    /// 포인트들 간 연결 관계를 설정합니다
    /// </summary>
    private void SetupTrackConnections()
    {
        for (int i = 0; i < trackPoints.Length; i++)
        {
            if (trackPoints[i] == null)
            {
                Debug.LogError($"트랙 포인트 {i}가 null입니다!");
                continue;
            }

            // 포인트 타입 설정
            TrackPointType pointType = TrackPointType.Middle;
            if (i == 0) pointType = TrackPointType.Start;
            else if (i == trackPoints.Length - 1) pointType = TrackPointType.End;

            trackPoints[i].SetPointType(pointType);

            // 트랙 너비 설정
            trackPoints[i].SetTrackWidth(trackWidth);

            // 다음 포인트 설정
            if (i < trackPoints.Length - 1)
            {
                trackPoints[i].SetNextPoint(trackPoints[i + 1]);
            }
        }
    }


    /// <summary>
    /// 총 트랙 거리를 계산합니다
    /// </summary>
    private void CalculateTotalDistance()
    {
        totalTrackDistance = 0f;

        for (int i = 0; i < trackPoints.Length - 1; i++)
        {
            TrackPoint currentPoint = trackPoints[i];
            TrackPoint nextPoint = trackPoints[i + 1];

            if (currentPoint != null && nextPoint != null)
            {
                currentPoint.CalculateDistanceToNext();
                totalTrackDistance += currentPoint.DistanceToNext;
            }
        }
    }

    /// <summary>
    /// 현재 위치에서의 진행도를 계산합니다
    /// </summary>
    /// <param name="currentPosition">현재 위치</param>
    /// <returns>진행도 (0.0 ~ 1.0)</returns>
    public float CalculateProgress(Vector3 currentPosition)
    {
        if (totalTrackDistance <= 0f)
        {
            return 0f;
        }

        float traveledDistance = 0f;

        // 현재 위치에서 가장 가까운 포인트 찾기
        int closestPointIndex = FindClosestPointIndex(currentPosition);
        
        if (closestPointIndex < 0 || closestPointIndex >= trackPoints.Length - 1)
        {
            return 1f; // 끝에 도달
        }

        // 현재 포인트까지의 거리 계산
        for (int i = 0; i < closestPointIndex; i++)
        {
            traveledDistance += trackPoints[i].DistanceToNext;
        }

        // 현재 포인트에서 다음 포인트까지의 부분 거리 계산
        TrackPoint currentPoint = trackPoints[closestPointIndex];
        TrackPoint nextPoint = trackPoints[closestPointIndex + 1];
        
        if (currentPoint != null && nextPoint != null)
        {
            Vector3 currentPointPos = currentPoint.CenterPosition;
            Vector3 nextPointPos = nextPoint.CenterPosition;
            
            float segmentDistance = Vector3.Distance(currentPointPos, nextPointPos);
            float currentSegmentDistance = Vector3.Distance(currentPointPos, currentPosition);
            
            if (segmentDistance > 0f)
            {
                traveledDistance += currentSegmentDistance;
            }
        }

        return Mathf.Clamp01(traveledDistance / totalTrackDistance);
    }

    /// <summary>
    /// 가장 가까운 포인트 인덱스를 찾습니다
    /// </summary>
    /// <param name="position">현재 위치</param>
    /// <returns>가장 가까운 포인트 인덱스</returns>
    private int FindClosestPointIndex(Vector3 position)
    {
        if (trackPoints == null || trackPoints.Length == 0)
        {
            return -1;
        }

        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < trackPoints.Length; i++)
        {
            if (trackPoints[i] != null)
            {
                float distance = Vector3.Distance(position, trackPoints[i].CenterPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
        }

        return closestIndex;
    }

    /// <summary>
    /// 진행도에 따른 중심선 위치를 반환합니다
    /// </summary>
    /// <param name="progress">진행도 (0.0 ~ 1.0)</param>
    /// <param name="offset">중심선에서의 오프셋 (좌우)</param>
    /// <returns>계산된 위치</returns>
    public Vector3 GetPositionAtProgress(float progress, float offset = 0f)
    {
        progress = Mathf.Clamp01(progress);
        offset = Mathf.Clamp(offset, -trackWidth / 2f, trackWidth / 2f);

        if (trackPoints == null || trackPoints.Length == 0)
        {
            return Vector3.zero;
        }

        if (progress >= 1f)
        {
            // 끝점 반환
            if (trackPoints != null && trackPoints.Length > 0)
            {
                TrackPoint lastPoint = trackPoints[trackPoints.Length - 1];
                return lastPoint != null ? lastPoint.GetPositionAtCircularOffset(offset) : Vector3.zero;
            }
            return Vector3.zero;
        }

        // 진행도에 따른 포인트 찾기
        float targetDistance = progress * totalTrackDistance;
        float currentDistance = 0f;

        for (int i = 0; i < trackPoints.Length - 1; i++)
        {
            TrackPoint currentPoint = trackPoints[i];
            TrackPoint nextPoint = trackPoints[i + 1];

            if (currentPoint != null && nextPoint != null)
            {
                float segmentDistance = currentPoint.DistanceToNext;
                
                if (currentDistance + segmentDistance >= targetDistance)
                {
                    // 이 세그먼트 내에서 보간
                    float segmentProgress = (targetDistance - currentDistance) / segmentDistance;
                    return currentPoint.GetInterpolatedPosition(segmentProgress, offset);
                }
                
                currentDistance += segmentDistance;
            }
        }

        // 마지막 포인트 반환
        if (trackPoints != null && trackPoints.Length > 0)
        {
            TrackPoint lastPoint = trackPoints[trackPoints.Length - 1];
            return lastPoint != null ? lastPoint.GetPositionAtCircularOffset(offset) : Vector3.zero;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 총 트랙 거리를 반환합니다
    /// </summary>
    /// <returns>총 거리</returns>
    public float GetTotalDistance()
    {
        return totalTrackDistance;
    }

    /// <summary>
    /// 위치가 트랙 경계 내에 있는지 확인합니다
    /// </summary>
    /// <param name="position">확인할 위치</param>
    /// <returns>트랙 경계 내에 있으면 true</returns>
    public bool IsPositionInTrack(Vector3 position)
    {
        // 가장 가까운 포인트 찾기
        int closestIndex = FindClosestPointIndex(position);
        
        if (closestIndex >= 0 && closestIndex < trackPoints.Length)
        {
            return trackPoints[closestIndex].IsPositionInTrack(position);
        }
        
        return false;
    }

    /// <summary>
    /// 위치를 트랙 경계 내로 제한합니다
    /// </summary>
    /// <param name="position">제한할 위치</param>
    /// <returns>제한된 위치</returns>
    public Vector3 ClampPositionToTrack(Vector3 position)
    {
        // 가장 가까운 포인트 찾기
        int closestIndex = FindClosestPointIndex(position);
        
        if (closestIndex >= 0 && closestIndex < trackPoints.Length)
        {
            return trackPoints[closestIndex].ClampPositionToTrack(position);
        }
        
        return position;
    }


    /// <summary>
    /// 트랙을 다시 설정합니다
    /// </summary>
    [ContextMenu("트랙 다시 설정")]
    public void RegenerateTrack()
    {
        InitializeTrack();
        Debug.Log("트랙이 다시 설정되었습니다.");
    }

}
