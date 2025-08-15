using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameState { Playing, Paused, GameOver }

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager I { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;

    public static event Action<GameState> OnStateChanged;

    [Header("Keys")]
    [SerializeField] private KeyCode pauseKey1 = KeyCode.Escape;
    [SerializeField] private KeyCode pauseKey2 = KeyCode.P;

    [Header("UI Panels")]
    [SerializeField] private GameObject menuScreen;     // kéo MenuScreen vào đây
    [SerializeField] private GameObject gameOverScreen; // kéo GameOverScreen vào đây

    [Header("Main Menu (optional)")]
    [SerializeField] private string mainMenuSceneName = ""; // khi nào có loadscene thì thêm vào

    void Awake()
    {
        if (I != this && I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        ApplyTimeScale();
        if (menuScreen) menuScreen.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        AudioListener.pause = false;
    }

    void Update()
    {
        if (State == GameState.GameOver) return;

        if (Input.GetKeyDown(pauseKey1) || Input.GetKeyDown(pauseKey2))
            TogglePause();
    }

    // ==== P API ====
    public void TogglePause()
    {
        if (State == GameState.Paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        State = GameState.Paused;
        ApplyTimeScale();
        if (menuScreen) menuScreen.SetActive(true);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        AudioListener.pause = true;
        OnStateChanged?.Invoke(State);
    }

    public void Resume()
    {
        State = GameState.Playing;
        ApplyTimeScale();
        if (menuScreen) menuScreen.SetActive(false);
        AudioListener.pause = false;
        OnStateChanged?.Invoke(State);
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        ApplyTimeScale();
        if (menuScreen) menuScreen.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(true);
        AudioListener.pause = true;
        OnStateChanged?.Invoke(State);
    }

    public void GoToMainMenu()
    {
        // Gán hàm này cho MainMenu
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            Time.timeScale = 1f;        // đảm bảo unpause trước khi load
            AudioListener.pause = false;
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Resume();
        }
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        var idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("ban da thoat game");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ==== Helpers ====
    void ApplyTimeScale()
    {
        Time.timeScale = (State == GameState.Paused || State == GameState.GameOver) ? 0f : 1f;
    }
}
