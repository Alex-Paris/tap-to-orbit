using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private static bool _isGameOver;
    
    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver
    }
    
    [SerializeField] private Player _player;

    private State _state;
    private UIManager _ui;
    private int _score;
    private int _bestScore;

    public DifficultyManager Difficulty { get; private set; }
    public bool IsWaitingToStart => _state == State.WaitingToStart;
    public bool IsRunning => _state == State.GamePlaying;
    public bool IsGameOver => _state == State.GameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Difficulty = GetComponent<DifficultyManager>();
        _ui = GetComponent<UIManager>();
        _state = State.WaitingToStart;
        _bestScore = PlayerPrefs.GetInt("BEST", 0);
        
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_isGameOver)
            StartRun();
    }
    

    public void StartRun()
    {
        _score = 0;
        _state = State.GamePlaying;
        _ui.SetScore(_score);
        _ui.ShowGameHUD(true);
        Difficulty.ResetDifficulty();
        Time.timeScale = 1f;
        _player.Launch();
    }

    public void AddScore(int amount)
    {
        if (!IsRunning) return;
        _score += amount;
        _ui.SetScore(_score);
    }
    
    public void GameOver()
    {
        if (!IsRunning) return;
        _state = State.GameOver;
        _isGameOver = true;
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt("BEST", _bestScore);
        }
        _ui.ShowGameOver(_score, _bestScore);
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
