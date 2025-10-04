using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 베이스 클래스
/// MonoBehaviour를 상속받는 매니저 클래스들이 사용할 수 있는 싱글톤 패턴
/// </summary>
/// <typeparam name="T">싱글톤으로 만들 클래스 타입</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Private Fields
    private static T m_instance;
    private static readonly object m_lock = new object();
    private static bool m_applicationIsQuitting = false;
    #endregion

    #region Public Properties
    /// <summary>
    /// 싱글톤 인스턴스에 접근하는 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            // 애플리케이션이 종료 중인 경우 null 반환
            if (m_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] {typeof(T)} 인스턴스가 이미 파괴되었습니다. 애플리케이션이 종료 중입니다.");
                return null;
            }

            // 스레드 안전성을 위한 lock
            lock (m_lock)
            {
                if (m_instance == null)
                {
                    // 씬에서 기존 인스턴스 찾기
                    m_instance = FindFirstObjectByType<T>();

                    // 씬에 인스턴스가 없는 경우 새로 생성
                    if (m_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        m_instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"[Singleton] {typeof(T).Name}";

                        // DontDestroyOnLoad 설정 (옵션) - 인스턴스 생성 후 호출
                        // 인스턴스가 생성된 후에 ShouldPersistAcrossScenes()를 호출할 수 있도록
                        // Awake에서 처리하도록 변경

                        Debug.Log($"[Singleton] {typeof(T).Name} 인스턴스가 새로 생성되었습니다.");
                    }
                    else
                    {
                        Debug.Log($"[Singleton] {typeof(T).Name} 인스턴스를 씬에서 찾았습니다.");
                    }
                }

                return m_instance;
            }
        }
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// 씬 전환 시에도 유지할지 결정하는 메소드
    /// 오버라이드하여 각 매니저별로 설정 가능
    /// </summary>
    /// <returns>true면 DontDestroyOnLoad 적용, false면 씬과 함께 파괴</returns>
    protected virtual bool ShouldPersistAcrossScenes()
    {
        return true; // 기본값: 씬 전환 시에도 유지
    }

    /// <summary>
    /// 싱글톤 초기화 시 호출되는 메소드
    /// 각 매니저에서 오버라이드하여 초기화 로직 구현
    /// </summary>
    protected virtual void InitializeSingleton()
    {
        Debug.Log($"[Singleton] {typeof(T).Name} 싱글톤이 초기화되었습니다.");
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Awake에서 싱글톤 초기화
    /// </summary>
    protected virtual void Awake()
    {
        // 이미 인스턴스가 있고 현재 오브젝트가 아닌 경우 파괴
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning($"[Singleton] {typeof(T).Name} 인스턴스가 이미 존재합니다. 중복 인스턴스를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        // 현재 오브젝트를 인스턴스로 설정
        m_instance = this as T;

        // DontDestroyOnLoad 설정 (옵션)
        if (ShouldPersistAcrossScenes())
        {
            DontDestroyOnLoad(gameObject);
        }

        // 싱글톤 초기화
        InitializeSingleton();
    }

    /// <summary>
    /// 애플리케이션 종료 시 플래그 설정
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        m_applicationIsQuitting = true;
    }

    /// <summary>
    /// 오브젝트 파괴 시 인스턴스 정리
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (m_instance == this)
        {
            m_instance = null;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 싱글톤 인스턴스가 존재하는지 확인
    /// </summary>
    /// <returns>인스턴스가 존재하면 true</returns>
    public static bool HasInstance()
    {
        return m_instance != null;
    }

    /// <summary>
    /// 싱글톤 인스턴스를 강제로 파괴 (디버깅용)
    /// </summary>
    [ContextMenu("싱글톤 인스턴스 파괴")]
    public static void DestroyInstance()
    {
        if (m_instance != null)
        {
            Debug.Log($"[Singleton] {typeof(T).Name} 인스턴스를 강제로 파괴합니다.");
            Destroy(m_instance.gameObject);
            m_instance = null;
        }
    }
    #endregion
}
