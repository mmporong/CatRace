using UnityEngine;

/// <summary>
/// 개별 고양이의 레이스 진행도를 추적하는 클래스
/// </summary>
[System.Serializable]
public class RaceProgress
{
    [Header("기본 정보")]
    public CatAI catAI;
    public string catName;
    public int catIndex;

    [Header("진행도 정보")]
    public int currentLap = 0;
    public int currentTrackPointIndex = 0;
    public float lapProgress = 0f; // 0.0 ~ 1.0 (현재 랩 내 진행도)
    public float totalProgress = 0f; // 0.0 ~ 1.0 (전체 레이스 진행도)
    public float distanceTraveled = 0f; // 총 이동 거리
    public float lapTime = 0f; // 현재 랩 시간
    public float totalTime = 0f; // 총 레이스 시간

    [Header("상태")]
    public bool hasFinished = false;
    public bool isOnTrack = true;
    public int currentRank = 0;

    [Header("마지막 위치")]
    public Vector3 lastPosition;
    public float lastUpdateTime;

    public RaceProgress(CatAI cat, int index)
    {
        catAI = cat;
        catName = cat != null ? cat.gameObject.name : $"Cat_{index}";
        catIndex = index;
        lastPosition = cat != null ? cat.transform.position : Vector3.zero;
        lastUpdateTime = Time.time;
    }

    /// <summary>
    /// 진행도를 업데이트합니다
    /// </summary>
    /// <param name="trackManager">트랙 매니저</param>
    /// <param name="totalLaps">총 랩 수</param>
    public void UpdateProgress(TrackManager trackManager, int totalLaps)
    {
        if (catAI == null || trackManager == null || hasFinished) return;

        Vector3 currentPos = catAI.transform.position;
        float deltaTime = Time.time - lastUpdateTime;
        
        // 거리 이동량 계산
        float distanceMoved = Vector3.Distance(lastPosition, currentPos);
        distanceTraveled += distanceMoved;
        
        // 시간 업데이트
        lapTime += deltaTime;
        totalTime += deltaTime;
        
        // 트랙 포인트 진행도 계산
        UpdateTrackPointProgress(trackManager);
        
        // 랩 진행도 계산
        UpdateLapProgress(trackManager, totalLaps);
        
        // 위치 업데이트
        lastPosition = currentPos;
        lastUpdateTime = Time.time;
    }

    /// <summary>
    /// 트랙 포인트 진행도를 업데이트합니다
    /// </summary>
    private void UpdateTrackPointProgress(TrackManager trackManager)
    {
        if (trackManager.TrackPoints == null || trackManager.TrackPoints.Length == 0) return;

        // 현재 트랙 포인트 찾기
        int closestPointIndex = FindClosestTrackPoint(trackManager);
        
        if (closestPointIndex != -1)
        {
            // 다음 포인트로 진행했는지 확인
            if (closestPointIndex > currentTrackPointIndex)
            {
                currentTrackPointIndex = closestPointIndex;
                
                // 시작점을 다시 지나면 랩 완료
                if (currentTrackPointIndex == 0 && currentLap > 0)
                {
                    CompleteLap();
                }
            }
        }
    }

    /// <summary>
    /// 가장 가까운 트랙 포인트를 찾습니다
    /// </summary>
    private int FindClosestTrackPoint(TrackManager trackManager)
    {
        if (catAI == null || trackManager.TrackPoints == null) return -1;

        Vector3 catPos = catAI.transform.position;
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < trackManager.TrackPoints.Length; i++)
        {
            float distance = Vector3.Distance(catPos, trackManager.TrackPoints[i].CenterPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    /// <summary>
    /// 랩 진행도를 업데이트합니다
    /// </summary>
    private void UpdateLapProgress(TrackManager trackManager, int totalLaps)
    {
        if (trackManager.TrackPoints == null || trackManager.TrackPoints.Length == 0) return;

        // 현재 랩 내 진행도 계산 (0.0 ~ 1.0)
        float totalTrackPoints = trackManager.TrackPoints.Length;
        lapProgress = currentTrackPointIndex / totalTrackPoints;

        // 전체 레이스 진행도 계산
        if (totalLaps > 0)
        {
            totalProgress = (currentLap + lapProgress) / totalLaps;
        }

        // 레이스 완료 확인
        if (currentLap >= totalLaps)
        {
            hasFinished = true;
            totalProgress = 1.0f;
        }
    }

    /// <summary>
    /// 랩을 완료합니다
    /// </summary>
    private void CompleteLap()
    {
        currentLap++;
        lapTime = 0f; // 랩 시간 리셋
        
        Debug.Log($"{catName} 랩 {currentLap} 완료! (시간: {lapTime:F2}초)");
    }

    /// <summary>
    /// 순위를 설정합니다
    /// </summary>
    public void SetRank(int rank)
    {
        currentRank = rank;
    }

    /// <summary>
    /// 진행도를 리셋합니다
    /// </summary>
    public void ResetProgress()
    {
        currentLap = 0;
        currentTrackPointIndex = 0;
        lapProgress = 0f;
        totalProgress = 0f;
        distanceTraveled = 0f;
        lapTime = 0f;
        totalTime = 0f;
        hasFinished = false;
        isOnTrack = true;
        currentRank = 0;
        lastPosition = catAI != null ? catAI.transform.position : Vector3.zero;
        lastUpdateTime = Time.time;
    }

    /// <summary>
    /// 진행도 정보를 문자열로 반환합니다
    /// </summary>
    public override string ToString()
    {
        return $"{catName}: 랩 {currentLap}, 포인트 {currentTrackPointIndex}, 진행도 {totalProgress:P1}, 순위 {currentRank}";
    }
}
