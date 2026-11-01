using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム中のUI表示を管理するクラス
/// </summary>
public class GameUIManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static GameUIManager instance { get; private set; }

    /// <summary>
    /// ゲームクリアUI
    /// </summary>
    [SerializeField]
    private GameObject gameClearUI;

    /// <summary>
    /// Wave表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI waveText;

    /// <summary>
    /// Wave完了表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI waveCompleteText;

    /// <summary>
    /// 残り時間表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI timerText;

    /// <summary>
    /// ボス出現警告テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI bossSpawnText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (gameClearUI != null)
        {
            gameClearUI.SetActive(false);
        }

        if (waveCompleteText != null)
        {
            waveCompleteText.gameObject.SetActive(false);
        }

        if (bossSpawnText != null)
        {
            bossSpawnText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Wave表示を更新する
    /// </summary>
    /// <param name="currentWave">現在のWave</param>
    /// <param name="maxWave">最大Wave数</param>
    public void UpdateWaveText(int currentWave, int maxWave)
    {
        if (waveText == null)
        {
            return;
        }

        waveText.text =
            $"Wave : {currentWave}/{maxWave}";
    }

    /// <summary>
    /// 残り時間表示を更新する
    /// </summary>
    /// <param name="remainingTime">残り時間</param>
    public void UpdateTimerText(float remainingTime)
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text =
            Mathf.CeilToInt(remainingTime).ToString();
    }

    /// <summary>
    /// ゲームクリアUIの表示を切り替える
    /// </summary>
    /// <param name="show">表示する場合はtrue</param>
    public void ShowGameClearUI(bool show)
    {
        if (gameClearUI == null)
        {
            return;
        }

        gameClearUI.SetActive(show);
    }

    /// <summary>
    /// Wave完了メッセージを表示する
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    /// <param name="duration">表示時間</param>
    public IEnumerator ShowWaveCompleteText(
        string message,
        float duration)
    {
        if (waveCompleteText == null)
        {
            yield break;
        }

        waveCompleteText.text = message;
        waveCompleteText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        waveCompleteText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ボス出現警告を表示する
    /// </summary>
    /// <param name="duration">表示時間</param>
    public IEnumerator ShowBossWarning(float duration)
    {
        if (bossSpawnText == null)
        {
            yield break;
        }

        bossSpawnText.gameObject.SetActive(true);
        bossSpawnText.text = "WARNING";
        bossSpawnText.color = Color.red;

        yield return new WaitForSeconds(duration);

        bossSpawnText.color = Color.white;
        bossSpawnText.gameObject.SetActive(false);
    }
}