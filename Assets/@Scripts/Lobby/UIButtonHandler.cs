using UnityEngine;

/// <summary>
/// UI 버튼 클릭 이벤트 핸들러
/// </summary>
public class UIButtonHandler : MonoBehaviour
{
    /// <summary>
    /// 레이스 정보 패널 클릭 처리
    /// </summary>
    public void OnRaceInfoPanelClicked()
    {
        LobbyManager.Instance?.ToggleRacePanel();
    }

    /// <summary>
    /// 리소스 정보 패널 클릭 처리
    /// </summary>
    public void OnResourceInfoPanelClicked()
    {
        LobbyManager.Instance?.ToggleResourcePanel();
    }
}
