using System;
using UnityEngine;

/// <summary>
/// ショップで販売されるアイテムのデータを保持するクラスです。
/// シリアライズ可能で、Unityエディタ上で簡単に設定できます。
/// </summary>
[Serializable]
public class ShopItem
{
    /// <summary>
    /// アイテム名
    /// </summary>
    public string itemName;

    /// <summary>
    /// 購入価格
    /// </summary>
    public int price;

    /// <summary>
    /// ショップUIに表示するアイコン
    /// </summary>
    public Sprite itemIcon;

    /// <summary>
    /// アイテム効果
    /// </summary>
    public ItemEffectType effectType;
}

/// <summary>
/// アイテム効果の種類
/// </summary>
public enum ItemEffectType
{
    /// <summary>
    /// 体力回復
    /// </summary>
    Heal,

    /// <summary>
    /// 移動速度アップ
    /// </summary>
    Speed,

    /// <summary>
    /// 攻撃力アップ
    /// </summary>
    Attack,

    /// <summary>
    /// ウィザード関連
    /// </summary>
    Wizard,

    /// <summary>
    /// ウォーリア関連
    /// </summary>
    Warrior,

    /// <summary>
    /// ルーントレーサー関連
    /// </summary>
    Runetracer,

    /// <summary>
    /// 魔法書関連
    /// </summary>
    MagicBook,

    /// <summary>
    /// 弾幕関連
    /// </summary>
    Bullet
}