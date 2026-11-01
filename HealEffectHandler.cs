using UnityEngine;

/// <summary>
/// 回復エフェクトを生成するクラス
/// </summary>
public class HealEffectHandler : MonoBehaviour
{
    /// <summary>
    /// 回復エフェクトのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject healEffectPrefab;

    /// <summary>
    /// 回復エフェクトを再生する
    /// </summary>
    public void TriggerHealEffect()
    {
        if (healEffectPrefab == null)
        {
            return;
        }

        GameObject healEffect =
            Instantiate(
                healEffectPrefab,
                transform.position,
                Quaternion.identity);

        Destroy(healEffect, HealEffectDuration);
    }

    /// <summary>
    /// 回復エフェクトの表示時間
    /// </summary>
    private const float HealEffectDuration = 2f;
}