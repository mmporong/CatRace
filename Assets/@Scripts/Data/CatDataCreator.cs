using UnityEngine;

/// <summary>
/// 고양이 스탯 데이터를 생성하는 에디터 도구
/// </summary>
public class CatDataCreator : MonoBehaviour
{
    [Header("고양이 데이터 생성")]
    [SerializeField] private bool createCatData = false;

    private void OnValidate()
    {
        if (createCatData)
        {
            CreateAllCatData();
            createCatData = false;
        }
    }

    /// <summary>
    /// 모든 고양이 데이터를 생성합니다
    /// </summary>
    private void CreateAllCatData()
    {
        // 1. 스피드스터 고양이 (속도 특화)
        CreateCatData("스피드스터", "빠른 속도로 유명한 고양이", 18f, 15f, 30, 40, 35);

        // 2. 탱크 고양이 (체력 특화)
        CreateCatData("탱크", "튼튼한 체력을 가진 고양이", 8f, 6f, 85, 30, 70);

        // 3. 지능형 고양이 (지능 특화)
        CreateCatData("지능형", "똑똑한 전략가 고양이", 12f, 10f, 45, 90, 50);

        // 4. 파워 고양이 (힘 특화)
        CreateCatData("파워", "강력한 힘을 가진 고양이", 10f, 8f, 60, 35, 85);

        // 5. 밸런스 고양이 (균형형)
        CreateCatData("밸런스", "모든 능력이 균형잡힌 고양이", 12f, 12f, 60, 60, 60);

        // 6. 스프린터 고양이 (가속도 특화)
        CreateCatData("스프린터", "빠른 가속도로 유명한 고양이", 14f, 18f, 40, 45, 40);

        // 7. 서바이버 고양이 (체력+지능)
        CreateCatData("서바이버", "생존에 특화된 고양이", 9f, 7f, 80, 75, 45);

        // 8. 스위프트 고양이 (속도+가속도)
        CreateCatData("스위프트", "민첩한 움직임의 고양이", 16f, 16f, 35, 50, 30);

        // 9. 스마트 고양이 (지능+힘)
        CreateCatData("스마트", "똑똑하고 강한 고양이", 11f, 9f, 50, 80, 75);

        // 10. 올라운더 고양이 (전체적으로 우수)
        CreateCatData("올라운더", "모든 면에서 뛰어난 고양이", 15f, 14f, 70, 70, 70);

        Debug.Log("모든 고양이 데이터 생성 완료!");
    }

    /// <summary>
    /// 개별 고양이 데이터를 생성합니다
    /// </summary>
    private void CreateCatData(string name, string description, float speed, float acceleration, 
                              int health, int intelligence, int strength)
    {
        // 실제로는 Unity Editor에서 수동으로 생성해야 합니다
        // 이 스크립트는 데이터 구조를 보여주는 용도입니다
        Debug.Log($"고양이 데이터 생성: {name} - 속도({speed}) 가속도({acceleration}) 체력({health}) 지능({intelligence}) 힘({strength})");
    }
}
