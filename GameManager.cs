using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体の進行（シーン遷移や状態管理）を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static GameManager instance { get; private set; }

    /// <summary>
    /// ゲームオーバー後の待機時間
    /// </summary>
    [SerializeField]
    private float gameOverDelay = 2f;

    /// <summary>
    /// 遷移先のリザルトシーン名
    /// </summary>
    [SerializeField]
    private string resultSceneName = "ResultScene";

    /// <summary>
    /// ポーズ確認UI
    /// </summary>
    [SerializeField]
    private GameObject pausePanel;

    /// <summary>
    /// 遷移先のタイトルシーン名
    /// </summary>
    [SerializeField]
    private string titleSceneName = "TitleScene";

    /// <summary>
    /// ポーズ中かどうか
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// ゲームオーバー中かどうか
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// Coin表示UI
    /// </summary>
    [SerializeField]
    private GameObject coinUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    /// <summary>
    /// ポーズ確認UIを表示する
    /// </summary>
    private void OpenPauseMenu()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("GameManager: PausePanelが設定されていません。");
            return;
        }

        isPaused = true;
        pausePanel.SetActive(true);

        if(coinUI!=null)
        {
            coinUI.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// ゲームを再開する
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if(coinUI!=null)
        {
            coinUI.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    /// <summary>
    /// タイトル画面へ戻る
    /// </summary>
    public void ReturnToTitle()
    {
        isPaused = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// ゲームオーバー処理を開始する
    /// </summary>
    public void HandleGameOver()
    {
        isGameOver = true;

        StopAllCoroutines();
        StartCoroutine(TransitionToResultRoutine());
    }

    /// <summary>
    /// リザルト画面へ遷移する
    /// </summary>
    private IEnumerator TransitionToResultRoutine()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveFinalScore();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: ScoreManagerが見つかりません。");
        }

        SceneManager.LoadScene(resultSceneName);
    }
}