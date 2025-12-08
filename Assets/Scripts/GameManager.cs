using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI 연결")]
    public GameObject gameOverPanel; // 아까 만든 패널 연결

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    // 게임 오버가 되면 호출할 함수
    public void GameOver()
    {
        // 1. 게임 오버 패널을 켭니다.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // 2. 게임 시간을 멈춥니다. (몬스터도 멈춤)
        Time.timeScale = 0f; 
        
        // (선택 사항) 마우스 커서가 다시 보이게 잠금 해제
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    // '다시 하기' 버튼에 연결할 함수
    public void Retry()
    {
        // 1. 멈춘 시간을 다시 흐르게 합니다. (중요! 안 하면 재시작해도 멈춰있음)
        Time.timeScale = 1f;

        // 2. 현재 씬을 다시 로드합니다. (초기화)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // '나가기' 버튼에 연결할 함수
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 끄기
        #else
            Application.Quit(); // 실제 게임에서 끄기
        #endif
    }
}