// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class Stickman : MonoBehaviour
// {
//     [Header("Sprites Player")]
//     [SerializeField] Sprite ballSprite;
//     [SerializeField] Sprite stopSprite;
//     [SerializeField] Sprite goSprite;
//     [SerializeField] Sprite backSprite;
//     [SerializeField] Sprite winSprite;

//     [Header("Components")]
//     private HingeJoint2D hJoint;
//     private Rigidbody2D rb;
//     private LineRenderer lineRenderer;
//     private SpriteRenderer spriteRenderer;
//     private AudioSource audioSource;

//     [Header("Anchor")]
//     [SerializeField] private GameObject anchor;

//     [Header("Private Variables")]
//     private int lastBestPosJoint;
//     private int lastBestPosSelected;
//     private int touches;
//     private int bestPos;
//     private float bestDistance;
//     private Vector3 actualJointPos;

//     [Header("Public Variables")]
//     [SerializeField] private float gravityRope = 2f;
//     [SerializeField] private float gravityAir = 0.5f;
//     [SerializeField] private float factorX = 1.4f;
//     [SerializeField] private float factorY = 1f;

//     [Header("Game Stats")]
//     public int life = 3;
//     public int coins = 0;
//     public Text lifeText;
//     public Text coinsText;

//     [Header("Game Over UI")]
//     public GameObject gameOverPanel;
//     public Text gameOverText;
//     public Button restartButton;
//     public Button exitButtonGameOver; //  Added Exit button for Game Over panel

//     [Header("Play Panel UI")]
//     public GameObject playPanel;
//     public Button playButton;
//     public Button exitButtonStart; //  Added Exit button for Start panel

//     [Header("Sound UI")]
//     public Button soundButton;
//     public Text soundButtonText;

//     [Header("Win Panel UI")]
//     public GameObject winPanel;
//     public Button nextButton;
//     public Button homeButton;
//     public TextMeshProUGUI winCoinsText;

//     [Header("Resume UI (Universal)")]
//     public Button resumeButton;

//     [Header("Bool")]
//     private bool sticked = false;
//     private bool won = false;

//     [Header("Visual Effects")]
//     public GameObject bombExplosionEffect;
//     public GameObject coinExplosionEffect;

//     [Header("Sound Effects")]
//     public AudioClip bombSound;
//     public AudioClip skullSound;
//     public AudioClip coinSound;

//     private static bool restartedFromGameOver = false;
//     private bool soundOn = true;

//     private void Start()
//     {
//         //  Set the game frame rate to 120 FPS
//         QualitySettings.vSyncCount = 0;
//         Application.targetFrameRate = 120;

//         hJoint = GetComponent<HingeJoint2D>();
//         rb = GetComponent<Rigidbody2D>();
//         lineRenderer = GetComponent<LineRenderer>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         audioSource = GetComponent<AudioSource>();

//         if (audioSource == null)
//             audioSource = gameObject.AddComponent<AudioSource>();

//         //  Ensure line always renders behind player
//         if (spriteRenderer != null && lineRenderer != null)
//         {
//             lineRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
//             lineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

// #if UNITY_ANDROID
//             if (lineRenderer.material == null)
//             {
//                 Material mat = new Material(Shader.Find("Sprites/Default"));
//                 mat.renderQueue = 3000;
//                 lineRenderer.material = mat;
//             }
// #endif
//         }

//         lastBestPosJoint = 0;
//         lastBestPosSelected = 0;
//         touches = 0;

//         if (anchor != null && anchor.transform.childCount > 0)
//             anchor.transform.GetChild(lastBestPosSelected).gameObject.GetComponent<JointAnchor>().Selected();

//         //  Reset coins only when starting from Home screen
//         if (SceneManager.GetActiveScene().buildIndex == 0)
//         {
//             PlayerPrefs.SetInt("TotalCoins", 0);
//             PlayerPrefs.Save();
//         }

//         // Load total coins for gameplay
//         coins = PlayerPrefs.GetInt("TotalCoins", 0);
//         UpdateUI();
//         won = false;

//         if (gameOverPanel != null) gameOverPanel.SetActive(false);
//         if (winPanel != null) winPanel.SetActive(false);

//         if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
//         if (soundButton != null) soundButton.onClick.AddListener(ToggleSound);
//         if (nextButton != null) nextButton.onClick.AddListener(NextLevel);
//         if (homeButton != null) homeButton.onClick.AddListener(ReturnHome);
//         if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);

//         //  Exit buttons
//         if (exitButtonStart != null) exitButtonStart.onClick.AddListener(ExitGame);
//         if (exitButtonGameOver != null) exitButtonGameOver.onClick.AddListener(ExitGame);

//         UpdateSoundUI();

//         if (!restartedFromGameOver)
//         {
//             if (playPanel != null) playPanel.SetActive(true);
//             if (playButton != null) playButton.onClick.AddListener(StartGame);
//             Time.timeScale = 0f;
//         }
//         else
//         {
//             if (playPanel != null) playPanel.SetActive(false);
//             Time.timeScale = 1f;
//             restartedFromGameOver = false;
//         }
//     }

//     private void StartGame()
//     {
//         if (playPanel != null) playPanel.SetActive(false);
//         Time.timeScale = 1f;
//         rb.WakeUp();
//         if (hJoint != null) hJoint.enabled = false;
//     }

//     private void Update()
//     {
//         bestPos = 0;
//         bestDistance = float.MaxValue;

//         for (int i = 0; i < anchor.transform.childCount; i++)
//         {
//             float actualDistance = Vector2.Distance(transform.position, anchor.transform.GetChild(i).position);
//             if (actualDistance < bestDistance)
//             {
//                 bestPos = i;
//                 bestDistance = actualDistance;
//             }
//         }

//         if (!won)
//             CheckInput();

//         if (lastBestPosSelected != bestPos)
//         {
//             anchor.transform.GetChild(lastBestPosSelected).GetComponent<JointAnchor>().Unselected();
//             anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Selected();
//         }

//         lastBestPosSelected = bestPos;
//     }

//     private void FixedUpdate()
//     {
//         if (sticked)
//         {
//             if (hJoint != null && hJoint.connectedBody != null)
//             {
//                 actualJointPos = hJoint.connectedBody.transform.position;
//                 lineRenderer.SetPosition(0, transform.position);
//                 lineRenderer.SetPosition(1, actualJointPos);
//             }
//             ChangeSprite();
//         }
//     }

//     private void CheckInput()
//     {
//         if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ||
//             ((Input.touchCount > 0) && (touches == 0))) && !sticked)
//         {
//             lineRenderer.enabled = true;
//             hJoint.enabled = true;
//             rb.gravityScale = gravityRope;

//             Rigidbody2D anchorBody = anchor.transform.GetChild(bestPos).GetChild(0).GetComponent<Rigidbody2D>();
//             if (anchorBody != null)
//             {
//                 hJoint.connectedBody = anchorBody;
//                 actualJointPos = anchorBody.transform.position;
//                 lineRenderer.SetPosition(0, transform.position);
//                 lineRenderer.SetPosition(1, actualJointPos);
//             }

//             anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().SetSticked();
//             anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Unselected();

//             lastBestPosJoint = bestPos;
//             rb.angularVelocity = 0f;
//             sticked = true;
//         }

//         if ((Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) ||
//             ((Input.touchCount == 0) && (touches > 0))) && sticked)
//         {
//             lineRenderer.enabled = false;
//             hJoint.enabled = false;
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x * factorX, rb.linearVelocity.y + factorY);
//             rb.gravityScale = gravityAir;

//             if (lastBestPosJoint < anchor.transform.childCount)
//                 anchor.transform.GetChild(lastBestPosJoint).GetComponent<JointAnchor>().SetUnsticked();

//             if (bestPos == lastBestPosJoint)
//             {
//                 anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Selected();
//                 anchor.transform.GetChild(lastBestPosSelected).GetComponent<JointAnchor>().Unselected();
//             }

//             spriteRenderer.sprite = ballSprite;
//             rb.AddTorque(-rb.linearVelocity.magnitude);
//             sticked = false;
//         }

//         touches = Input.touchCount;
//     }

//     private void ChangeSprite()
//     {
//         spriteRenderer.flipX = rb.linearVelocity.x <= 0;

//         if (rb.linearVelocity.x < 0.7f && rb.linearVelocity.x > -0.7f && transform.position.y < actualJointPos.y)
//             spriteRenderer.sprite = stopSprite;
//         else
//             spriteRenderer.sprite = rb.linearVelocity.y < 0 ? goSprite : backSprite;

//         transform.eulerAngles = LookAt2d(actualJointPos - transform.position);
//     }

//     public Vector3 LookAt2d(Vector3 vec)
//     {
//         return new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Vector2.SignedAngle(Vector2.up, vec));
//     }

//     public bool getSticked() => sticked;

//     public void reset(Vector3 initPos)
//     {
//         rb.linearVelocity = Vector2.zero;
//         rb.angularVelocity = 0f;
//         transform.position = initPos;
//         transform.rotation = Quaternion.identity;
//     }

//     public void Win(float speedWin)
//     {
//         won = true;
//         spriteRenderer.flipX = false;
//         rb.gravityScale = 0;
//         transform.eulerAngles = LookAt2d(rb.linearVelocity);
//         rb.linearVelocity = rb.linearVelocity.normalized * speedWin;
//         rb.angularVelocity = 0f;
//         spriteRenderer.sprite = winSprite;

//         Time.timeScale = 0f;
//         if (winPanel != null)
//         {
//             winPanel.SetActive(true);

//             PlayerPrefs.SetInt("TotalCoins", coins);
//             PlayerPrefs.Save();

//             if (winCoinsText != null)
//                 winCoinsText.text = " Coins: " + coins;
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Heart"))
//         {
//             life++;
//             UpdateUI();
//             Destroy(collision.gameObject);
//         }
//         else if (collision.CompareTag("Coin"))
//         {
//             coins++;
//             UpdateUI();

//             if (soundOn && coinSound != null)
//                 audioSource.PlayOneShot(coinSound);

//             if (coinExplosionEffect != null)
//             {
//                 GameObject effect = Instantiate(coinExplosionEffect, collision.transform.position, Quaternion.identity);
//                 Destroy(effect, 2f);
//             }

//             Destroy(collision.gameObject);
//         }
//         else if (collision.CompareTag("Diamond"))
//         {
//             coins += 10;
//             UpdateUI();

//             if (soundOn && coinSound != null)
//                 audioSource.PlayOneShot(coinSound);

//             if (coinExplosionEffect != null)
//             {
//                 GameObject effect = Instantiate(coinExplosionEffect, collision.transform.position, Quaternion.identity);
//                 Destroy(effect, 2f);
//             }

//             Destroy(collision.gameObject);
//         }
//         else if (collision.CompareTag("Bomb"))
//         {
//             if (bombExplosionEffect != null)
//             {
//                 GameObject explosion = Instantiate(bombExplosionEffect, collision.transform.position, Quaternion.identity);
//                 Destroy(explosion, 2f);
//             }

//             if (soundOn && bombSound != null)
//                 audioSource.PlayOneShot(bombSound);

//             life--;
//             UpdateUI();
//             Destroy(collision.gameObject);

//             if (life <= 0)
//                 GameOver();
//         }
//         else if (collision.CompareTag("Skull"))
//         {
//             if (soundOn && skullSound != null)
//                 audioSource.PlayOneShot(skullSound);

//             GameOver();
//             Destroy(collision.gameObject);
//         }
//         else if (collision.CompareTag("Finish"))
//         {
//             StartCoroutine(ShowWinPanelAfterDelay(4f));  // ⬅️ Changed line (Win panel will open after 4 seconds)
//         }
//     }

//     // Coroutine to delay the Win panel by specified seconds
//      private IEnumerator ShowWinPanelAfterDelay(float delay)
//      {
//          yield return new WaitForSeconds(delay);  // Wait for 4 seconds before showing the Win panel
//          Win(5f);  // Call Win function after delay
//      }


//     private void UpdateUI()
//     {
//         if (lifeText != null) lifeText.text = " " + life;
//         if (coinsText != null) coinsText.text = " " + coins;
//     }

//     private void GameOver()
//     {
//         Time.timeScale = 0f;
//         if (gameOverPanel != null) gameOverPanel.SetActive(true);
//         if (gameOverText != null) gameOverText.text = "GAME OVER!";
//     }

//     public void RestartGame()
//     {
//         restartedFromGameOver = true;
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }

//     private void NextLevel()
//     {
//         Time.timeScale = 1f;
//         int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
//         if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
//             SceneManager.LoadScene(nextSceneIndex);
//         else
//             SceneManager.LoadScene(0);
//     }

//     private void ReturnHome()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(0);
//     }

//     private void ToggleSound()
//     {
//         soundOn = !soundOn;
//         AudioListener.volume = soundOn ? 1f : 0f;
//         UpdateSoundUI();
//     }

//     private void UpdateSoundUI()
//     {
//         if (soundButtonText != null)
//             soundButtonText.text = soundOn ? " Sound: ON" : " Sound: OFF";
//     }

//     private void ResumeGame()
//     {
//         Time.timeScale = 1f;
//         if (gameOverPanel != null) gameOverPanel.SetActive(false);
//         if (winPanel != null) winPanel.SetActive(false);
//         if (playPanel != null) playPanel.SetActive(false);
//     }

//     //  Exit Game Function
//     private void ExitGame()
//     {
//         #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
//         #else
//         Application.Quit();
//         #endif
//     }
// }

using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stickman : MonoBehaviour
{
    [Header("Sprites Player")]
    [SerializeField] Sprite ballSprite;
    [SerializeField] Sprite stopSprite;
    [SerializeField] Sprite goSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite winSprite;

    [Header("Components")]
    private HingeJoint2D hJoint;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [Header("Anchor")]
    [SerializeField] private GameObject anchor;

    [Header("Private Variables")]
    private int lastBestPosJoint;
    private int lastBestPosSelected;
    private int touches;
    private int bestPos;
    private float bestDistance;
    private Vector3 actualJointPos;

    [Header("Public Variables")]
    [SerializeField] private float gravityRope = 2f;
    [SerializeField] private float gravityAir = 0.5f;
    [SerializeField] private float factorX = 1.4f;
    [SerializeField] private float factorY = 1f;

    [Header("Game Stats")]
    public int life = 3;
    public int coins = 0;
    public Text lifeText;
    public Text coinsText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button restartButton;
    public Button exitButtonGameOver; // Added Exit button for Game Over panel

    [Header("Play Panel UI")]
    public GameObject playPanel;
    public Button playButton;
    public Button exitButtonStart; // Added Exit button for Start panel

    [Header("Sound UI")]
    public Button soundButton;
    public Text soundButtonText;

    [Header("Win Panel UI")]
    public GameObject winPanel;
    public Button nextButton;
    public Button homeButton;
    public TextMeshProUGUI winCoinsText;

    [Header("Resume UI (Universal)")]
    public Button resumeButton;

    [Header("Bool")]
    private bool sticked = false;
    private bool won = false;

    [Header("Visual Effects")]
    public GameObject bombExplosionEffect;
    public GameObject coinExplosionEffect;

    [Header("Sound Effects")]
    public AudioClip bombSound;
    public AudioClip skullSound;
    public AudioClip coinSound;
    public AudioClip winSound; // 🎵 Added optional win sound effect

    private static bool restartedFromGameOver = false;
    private bool soundOn = true;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        hJoint = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (spriteRenderer != null && lineRenderer != null)
        {
            lineRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            lineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

#if UNITY_ANDROID
            if (lineRenderer.material == null)
            {
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.renderQueue = 3000;
                lineRenderer.material = mat;
            }
#endif
        }

        lastBestPosJoint = 0;
        lastBestPosSelected = 0;
        touches = 0;

        if (anchor != null && anchor.transform.childCount > 0)
            anchor.transform.GetChild(lastBestPosSelected).gameObject.GetComponent<JointAnchor>().Selected();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayerPrefs.SetInt("TotalCoins", 0);
            PlayerPrefs.Save();
        }

        coins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateUI();
        won = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (soundButton != null) soundButton.onClick.AddListener(ToggleSound);
        if (nextButton != null) nextButton.onClick.AddListener(NextLevel);
        if (homeButton != null) homeButton.onClick.AddListener(ReturnHome);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);

        if (exitButtonStart != null) exitButtonStart.onClick.AddListener(ExitGame);
        if (exitButtonGameOver != null) exitButtonGameOver.onClick.AddListener(ExitGame);

        UpdateSoundUI();

        if (!restartedFromGameOver)
        {
            if (playPanel != null) playPanel.SetActive(true);
            if (playButton != null) playButton.onClick.AddListener(StartGame);
            Time.timeScale = 0f;
        }
        else
        {
            if (playPanel != null) playPanel.SetActive(false);
            Time.timeScale = 1f;
            restartedFromGameOver = false;
        }
    }

    private void StartGame()
    {
        if (playPanel != null) playPanel.SetActive(false);
        Time.timeScale = 1f;
        rb.WakeUp();
        if (hJoint != null) hJoint.enabled = false;
    }

    private void Update()
    {
        bestPos = 0;
        bestDistance = float.MaxValue;

        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            float actualDistance = Vector2.Distance(transform.position, anchor.transform.GetChild(i).position);
            if (actualDistance < bestDistance)
            {
                bestPos = i;
                bestDistance = actualDistance;
            }
        }

        if (!won)
            CheckInput();

        if (lastBestPosSelected != bestPos)
        {
            anchor.transform.GetChild(lastBestPosSelected).GetComponent<JointAnchor>().Unselected();
            anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Selected();
        }

        lastBestPosSelected = bestPos;
    }

    private void FixedUpdate()
    {
        if (sticked)
        {
            if (hJoint != null && hJoint.connectedBody != null)
            {
                actualJointPos = hJoint.connectedBody.transform.position;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, actualJointPos);
            }
            ChangeSprite();
        }
    }

    private void CheckInput()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ||
            ((Input.touchCount > 0) && (touches == 0))) && !sticked)
        {
            lineRenderer.enabled = true;
            hJoint.enabled = true;
            rb.gravityScale = gravityRope;

            Rigidbody2D anchorBody = anchor.transform.GetChild(bestPos).GetChild(0).GetComponent<Rigidbody2D>();
            if (anchorBody != null)
            {
                hJoint.connectedBody = anchorBody;
                actualJointPos = anchorBody.transform.position;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, actualJointPos);
            }

            anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().SetSticked();
            anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Unselected();

            lastBestPosJoint = bestPos;
            rb.angularVelocity = 0f;
            sticked = true;
        }

        if ((Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) ||
            ((Input.touchCount == 0) && (touches > 0))) && sticked)
        {
            lineRenderer.enabled = false;
            hJoint.enabled = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * factorX, rb.linearVelocity.y + factorY);
            rb.gravityScale = gravityAir;

            if (lastBestPosJoint < anchor.transform.childCount)
                anchor.transform.GetChild(lastBestPosJoint).GetComponent<JointAnchor>().SetUnsticked();

            if (bestPos == lastBestPosJoint)
            {
                anchor.transform.GetChild(bestPos).GetComponent<JointAnchor>().Selected();
                anchor.transform.GetChild(lastBestPosSelected).GetComponent<JointAnchor>().Unselected();
            }

            spriteRenderer.sprite = ballSprite;
            rb.AddTorque(-rb.linearVelocity.magnitude);
            sticked = false;
        }

        touches = Input.touchCount;
    }

    private void ChangeSprite()
    {
        spriteRenderer.flipX = rb.linearVelocity.x <= 0;

        if (rb.linearVelocity.x < 0.7f && rb.linearVelocity.x > -0.7f && transform.position.y < actualJointPos.y)
            spriteRenderer.sprite = stopSprite;
        else
            spriteRenderer.sprite = rb.linearVelocity.y < 0 ? goSprite : backSprite;

        transform.eulerAngles = LookAt2d(actualJointPos - transform.position);
    }

    public Vector3 LookAt2d(Vector3 vec)
    {
        return new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Vector2.SignedAngle(Vector2.up, vec));
    }

    public bool getSticked() => sticked;

    public void reset(Vector3 initPos)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = initPos;
        transform.rotation = Quaternion.identity;
    }

    public void Win(float speedWin)
    {
        won = true;
        spriteRenderer.flipX = false;
        rb.gravityScale = 0;
        transform.eulerAngles = LookAt2d(rb.linearVelocity);
        rb.linearVelocity = rb.linearVelocity.normalized * speedWin;
        rb.angularVelocity = 0f;
        spriteRenderer.sprite = winSprite;

        Time.timeScale = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            PlayerPrefs.SetInt("TotalCoins", coins);
            PlayerPrefs.Save();

            if (winCoinsText != null)
                winCoinsText.text = " Coins: " + coins;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Heart"))
        {
            life++;
            UpdateUI();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Coin"))
        {
            coins++;
            UpdateUI();

            if (soundOn && coinSound != null)
                audioSource.PlayOneShot(coinSound);

            if (coinExplosionEffect != null)
            {
                GameObject effect = Instantiate(coinExplosionEffect, collision.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Diamond"))
        {
            coins += 10;
            UpdateUI();

            if (soundOn && coinSound != null)
                audioSource.PlayOneShot(coinSound);

            if (coinExplosionEffect != null)
            {
                GameObject effect = Instantiate(coinExplosionEffect, collision.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Bomb"))
        {
            if (bombExplosionEffect != null)
            {
                GameObject explosion = Instantiate(bombExplosionEffect, collision.transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            if (soundOn && bombSound != null)
                audioSource.PlayOneShot(bombSound);

            life--;
            UpdateUI();
            Destroy(collision.gameObject);

            if (life <= 0)
                GameOver();
        }
        else if (collision.CompareTag("Skull"))
        {
            if (soundOn && skullSound != null)
                audioSource.PlayOneShot(skullSound);

            GameOver();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Finish"))
        {
            // 🕓 Delay Win panel by 4 seconds after Finish line collision
            StartCoroutine(ShowWinPanelAfterDelay(4f));  // ⬅️ Changed this line
        }
    }

    // 🕓 Coroutine that waits before showing Win Panel
    private IEnumerator ShowWinPanelAfterDelay(float delay)
    {
        if (soundOn && winSound != null)
            audioSource.PlayOneShot(winSound); // 🎵 Play win sound before delay

        Time.timeScale = 0.5f; // 🐢 Optional slow motion effect before win panel
        yield return new WaitForSecondsRealtime(delay); // Wait using real time (not affected by timeScale)
        Time.timeScale = 1f; // Reset speed
        Win(5f); // Show Win panel
    }

    private void UpdateUI()
    {
        if (lifeText != null) lifeText.text = " " + life;
        if (coinsText != null) coinsText.text = " " + coins;
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverText != null) gameOverText.text = "GAME OVER!";
    }

    public void RestartGame()
    {
        restartedFromGameOver = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(0);
    }

    private void ReturnHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void ToggleSound()
    {
        soundOn = !soundOn;
        AudioListener.volume = soundOn ? 1f : 0f;
        UpdateSoundUI();
    }

    private void UpdateSoundUI()
    {
        if (soundButtonText != null)
            soundButtonText.text = soundOn ? " Sound: ON" : " Sound: OFF";
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (playPanel != null) playPanel.SetActive(false);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}







