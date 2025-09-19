using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 고양이 선택 패널 컨트롤러
/// </summary>
public class CatPanelController : MonoBehaviour
{
    [Header("고양이 버튼들")]
    [SerializeField] private Button[] m_catButtons;
    [SerializeField] private Image[] m_catImages;
    [SerializeField] private TextMeshProUGUI[] m_catNames;

    [Header("고양이 데이터")]
    [SerializeField] private CatStats[] m_availableCats;

    [Header("고양이 이름 생성")]
    [SerializeField] private string[] m_adjectives = { "푹신한", "동그란", "오렌지", "우아한", "장난스러운", "똑똑한", "용감한", "사랑스러운", "신비로운", "활발한" };
    [SerializeField] private string[] m_breeds = { "히말라얀", "턱시도", "노르웨이 숲", "페르시안", "러시안 블루", "메인쿤", "스코티시 폴드", "브리티시 숏헤어", "아메리칸 숏헤어", "뱅갈" };

    private void Start()
    {
        InitializeCatButtons();
    }

    /// <summary>
    /// 고양이 버튼들 초기화
    /// </summary>
    private void InitializeCatButtons()
    {
        if (m_availableCats == null || m_availableCats.Length == 0)
        {
            Debug.LogError("사용 가능한 고양이 데이터가 설정되지 않았습니다!");
            return;
        }

        // 고양이 이름 생성 및 적용
        GenerateCatNames();

        for (int i = 0; i < m_catButtons.Length && i < m_availableCats.Length; i++)
        {
            int catIndex = i; // 클로저를 위한 로컬 변수
            
            // 버튼 이벤트 연결
            m_catButtons[i].onClick.RemoveAllListeners();
            m_catButtons[i].onClick.AddListener(() => OnCatButtonClicked(catIndex));

            // 고양이 정보 설정
            if (m_catImages[i] != null && m_availableCats[i].CatSprite != null)
            {
                m_catImages[i].sprite = m_availableCats[i].CatSprite;
            }

            if (m_catNames[i] != null)
            {
                m_catNames[i].text = m_availableCats[i].CatName;
            }
        }
    }

    /// <summary>
    /// 고양이 버튼 클릭 처리
    /// </summary>
    private void OnCatButtonClicked(int catIndex)
    {
        if (catIndex >= 0 && catIndex < m_availableCats.Length)
        {
            Debug.Log($"고양이 선택됨: {m_availableCats[catIndex].CatName} (인덱스: {catIndex})");
            LobbyManager.Instance?.OnCatSelected(m_availableCats[catIndex]);
        }
    }

    /// <summary>
    /// 고양이 이름들을 랜덤하게 생성합니다
    /// </summary>
    private void GenerateCatNames()
    {
        if (m_availableCats == null) return;

        for (int i = 0; i < m_availableCats.Length; i++)
        {
            string generatedName = GenerateRandomCatName();
            
            // ScriptableObject의 이름을 리플렉션으로 수정
            var nameField = typeof(CatStats).GetField("catName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nameField?.SetValue(m_availableCats[i], generatedName);
            
            Debug.Log($"고양이 {i}번 이름 생성: {generatedName}");
        }
    }

    /// <summary>
    /// 랜덤한 고양이 이름을 생성합니다
    /// </summary>
    private string GenerateRandomCatName()
    {
        string adjective = m_adjectives[Random.Range(0, m_adjectives.Length)];
        string breed = m_breeds[Random.Range(0, m_breeds.Length)];
        
        return $"{adjective} {breed}";
    }
}
