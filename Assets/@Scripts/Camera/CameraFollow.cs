using DG.Tweening;
using UnityEngine;

/// <summary>
/// 카메라가 고양이를 따라가는 스크립트
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private void Start()
    {
       DOTween.To(() => GetComponent<Camera>().orthographicSize, x => GetComponent<Camera>().orthographicSize = x, 8, 1);
    }
}
