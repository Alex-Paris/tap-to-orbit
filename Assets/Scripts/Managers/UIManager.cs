using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("StartGame")]
    [SerializeField] private GameObject startRoot;
    [SerializeField] private Button startButton;
    
    [Header("HUD")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        hudRoot.SetActive(false);
        gameOverRoot.SetActive(false);
        startButton?.onClick.AddListener(() => GameManager.Instance.StartRun());
        retryButton?.onClick.AddListener(() => GameManager.Instance.Retry());
    }

    private void OnDestroy()
    {
        startButton?.onClick.RemoveListener(() => GameManager.Instance.StartRun());
        retryButton?.onClick.RemoveListener(() => GameManager.Instance.Retry());
    }

    public void ShowGameHUD(bool show)
    {
        if (startRoot) startRoot.SetActive(!show);
        if (hudRoot) hudRoot.SetActive(show);
        if (gameOverRoot) gameOverRoot.SetActive(!show);
    }

    public void SetScore(int s)
    {
        if (scoreText) scoreText.text = s.ToString();
    }

    public void ShowGameOver(int score, int best)
    {
        if (hudRoot) hudRoot.SetActive(false);
        if (gameOverRoot) gameOverRoot.SetActive(true);
        if (finalScoreText) finalScoreText.text = score.ToString();
        if (bestScoreText) bestScoreText.text = best.ToString();
    }
}
