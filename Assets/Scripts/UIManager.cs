using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject winPanel;
    public Text winCoinsText;
    public GameObject playPanel;
    public Text lifeText;
    public Text coinsText;
    public Button playButton;
    public Button restartButton;
    public Button nextButton;
    public Button homeButton;
    public Button exitButton;

    [Header("Visual Effects")]
    public GameObject coinEffect;
    public GameObject bombEffect;

    private Stickman player;

    public void InitializeUI(Stickman playerRef)
    {
        player = playerRef;

        if (playPanel != null)
        {
            playPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (playButton != null) playButton.onClick.AddListener(StartGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (nextButton != null) nextButton.onClick.AddListener(NextLevel);
        if (homeButton != null) homeButton.onClick.AddListener(ReturnHome);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);

        UpdateUI(player.life, player.coins);
    }

    public void StartGame()
    {
        if (playPanel != null) playPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateUI(int life, int coins)
    {
        if (lifeText != null) lifeText.text = "❤️ " + life;
        if (coinsText != null) coinsText.text = "💰 " + coins;
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null) gameOverText.text = "GAME OVER!";
        }
    }

    public void ShowWinPanel(int coins)
    {
        Time.timeScale = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winCoinsText != null)
                winCoinsText.text = "💰 Coins: " + coins;
        }
    }

    public void PlayCoinEffect(Vector3 pos)
    {
        if (coinEffect != null)
        {
            GameObject effect = Instantiate(coinEffect, pos, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void PlayBombEffect(Vector3 pos)
    {
        if (bombEffect != null)
        {
            GameObject effect = Instantiate(bombEffect, pos, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(0);
    }

    public void ReturnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
