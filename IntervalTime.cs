using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 一定時間ごとにUnityEventを実行するクラス
/// </summary>
public class IntervalTime : MonoBehaviour
{
    /// <summary>
    /// 最小インターバル時間
    /// </summary>
    private const float MinInterval = 0.1f;

    /// <summary>
    /// タイマー処理が有効かどうか
    /// </summary>
    private bool loopActive;

    /// <summary>
    /// イベント実行間隔
    /// </summary>
    [SerializeField]
    private float interval = 1f;

    /// <summary>
    /// 指定時間ごとに実行されるイベント
    /// </summary>
    [SerializeField]
    private UnityEvent doSomething;

    /// <summary>
    /// 次回実行までの残り時間
    /// </summary>
    private float intervalCount;

    /// <summary>
    /// コンポーネント有効時の初期化
    /// </summary>
    private void OnEnable()
    {
        interval =
            Mathf.Max(
                interval,
                MinInterval);

        intervalCount = interval;
        loopActive = true;
    }

    /// <summary>
    /// コンポーネント無効時の後処理
    /// </summary>
    private void OnDisable()
    {
        loopActive = false;
    }

    private void Update()
    {
        if (ShopManager.instance != null &&
            ShopManager.instance.IsShopOpen)
        {
            return;
        }

        if (Time.timeScale <= 0f)
        {
            return;
        }

        if (!loopActive)
        {
            return;
        }

        intervalCount -= Time.deltaTime;

        if (intervalCount > 0f)
        {
            return;
        }

        doSomething?.Invoke();

        intervalCount = interval;
    }

    /// <summary>
    /// インターバル時間を短縮する
    /// </summary>
    /// <param name="reductionAmount">短縮量</param>
    public void ReduceInterval(float reductionAmount)
    {
        interval =
            Mathf.Max(
                interval - reductionAmount,
                MinInterval);

        intervalCount = interval;
    }
}