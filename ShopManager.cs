using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内のショップ機能を管理するクラス
/// アイテムの表示、購入、リロール、および購入した効果の適用を制御する
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// ショップに表示するアイテム数
    /// </summary>
    private const int ShopItemCount = 3;

    /// <summary>
    /// リロール価格の増加量
    /// </summary>
    private const int RerollPriceIncrease = 5;

    /// <summary>
    /// リロール価格の初期値
    /// </summary>
    private const int DefaultRerollPrice = 5;

    /// <summary>
    /// リロール価格の最大値
    /// </summary>
    private const int DefaultMaxRerollPrice = 20;

    /// <summary>
    /// カウントダウンの開始秒数
    /// </summary>
    private const int CountdownStart = 3;

    /// <summary>
    /// カウントダウン終了後の待機時間
    /// </summary>
    private const float CountdownGoDuration = 1f;

    /// <summary>
    /// HP回復アイテム1個あたりの回復量
    /// </summary>
    private const int HealAmountPerItem = 20;

    /// <summary>
    /// 攻撃力アップアイテム1個あたりの上昇量
    /// </summary>
    private const int AttackIncreasePerItem = 2;

    /// <summary>
    /// 移動速度アップアイテム1個あたりの上昇量
    /// </summary>
    private const float SpeedIncreasePerItem = 0.01f;

    /// <summary>
    /// 弾幕アイテム購入時のインターバル短縮量
    /// </summary>
    private const float BulletIntervalReduction = 0.5f;

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static ShopManager instance { get; private set; }

    #region UI

    [Header("UI Objects")]

    /// <summary>
    /// ショップUI
    /// </summary>
    [SerializeField]
    private GameObject shopUi;

    /// <summary>
    /// メインUI
    /// </summary>
    [SerializeField]
    private GameObject mainUi;

    /// <summary>
    /// カウントダウン表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI countdownText;

    /// <summary>
    /// アイテムUIを配置する親Transform
    /// </summary>
    [SerializeField]
    private Transform itemParent;

    /// <summary>
    /// ショップアイテムUIのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject itemPrefab;

    #endregion

    #region Buttons

    /// <summary>
    /// ショップを閉じるボタン
    /// </summary>
    [SerializeField]
    private Button closeButton;

    /// <summary>
    /// アイテムをリロールするボタン
    /// </summary>
    [SerializeField]
    private Button rerollButton;

    /// <summary>
    /// リロール価格を表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI rerollPriceText;

    #endregion

    #region Shop Settings

    /// <summary>
    /// 出現可能な全ショップアイテム
    /// </summary>
    [SerializeField]
    private List<ShopItem> allItems;

    /// <summary>
    /// 現在のリロール価格
    /// </summary>
    [SerializeField]
    private int rerollPrice = DefaultRerollPrice;

    /// <summary>
    /// リロール価格の上限
    /// </summary>
    [SerializeField]
    private int maxRerollPrice = DefaultMaxRerollPrice;

    #endregion

    #region Summon

    /// <summary>
    /// 仲間が追従するプレイヤー
    /// </summary>
    [SerializeField]
    private Transform playerTransform;

    /// <summary>
    /// 召喚するウィザードのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject wizardPrefab;

    /// <summary>
    /// 召喚するウォーリアのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject warriorPrefab;

    /// <summary>
    /// 仲間を召喚する位置
    /// </summary>
    [SerializeField]
    private Transform summonLocation;

    #endregion

    #region Managers

    /// <summary>
    /// ルーントレーサーを管理するクラス
    /// </summary>
    [SerializeField]
    private RunetracerManager runetracerManager;

    /// <summary>
    /// 魔法書を管理するクラス
    /// </summary>
    [SerializeField]
    private MagicBookManager magicBookManager;

    /// <summary>
    /// 弾幕攻撃のインターバルを管理するコンポーネント
    /// </summary>
    [SerializeField]
    private IntervalTime bulletIntervalTime;

    #endregion

    #region Player

    /// <summary>
    /// プレイヤーのHPを管理するコンポーネント
    /// </summary>
    [SerializeField]
    private PlayerHPBar playerHpBar;

    /// <summary>
    /// プレイヤーを管理するコンポーネント
    /// </summary>
    [SerializeField]
    private Player player;

    #endregion

    #region Runtime Data

    /// <summary>
    /// 現在ショップに並んでいるアイテム
    /// </summary>
    private List<ShopItem> currentItems =
        new List<ShopItem>();

    /// <summary>
    /// HP回復アイテムの購入数
    /// </summary>
    private int healItemCount;

    /// <summary>
    /// 攻撃力アップアイテムの購入数
    /// </summary>
    private int attackBoostCount;

    /// <summary>
    /// 移動速度アップアイテムの購入数
    /// </summary>
    private int speedBoostCount;

    /// <summary>
    /// カウントダウンコルーチン
    /// </summary>
    private Coroutine countdownCoroutine;

    /// <summary>
    /// カウントダウン中か
    /// </summary>
    private bool isCountingDown;

    #endregion

    /// <summary>
    /// ショップが開いているか
    /// </summary>
    public bool IsShopOpen { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        rerollPrice =
            Mathf.Max(0, rerollPrice);

        maxRerollPrice =
            Mathf.Max(0, maxRerollPrice);

        rerollPrice =
            Mathf.Min(
                rerollPrice,
                maxRerollPrice);
    }

    private void Start()
    {
        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(
                RerollItems);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                CloseShop);
        }

        if (shopUi != null)
        {
            shopUi.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        UpdateRerollPriceDisplay();
    }

    private void OnDestroy()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// ショップを開く
    /// </summary>
    public void OpenShop()
    {
        if (IsShopOpen || isCountingDown)
        {
            return;
        }

        if (mainUi != null)
        {
            mainUi.SetActive(false);
        }

        if (shopUi != null)
        {
            shopUi.SetActive(true);
        }

        IsShopOpen = true;

        GenerateRandomItems();
        UpdateRerollPriceDisplay();
    }

    /// <summary>
    /// ショップを閉じる
    /// </summary>
    public void CloseShop()
    {
        if (!IsShopOpen || isCountingDown)
        {
            return;
        }

        IsShopOpen = false;

        if (mainUi != null)
        {
            mainUi.SetActive(true);
        }

        if (shopUi != null)
        {
            shopUi.SetActive(false);
        }

        ApplyPurchasedEffects();

        isCountingDown = true;

        countdownCoroutine =
            StartCoroutine(
                StartCountDownAndResumeSpawning());
    }

    /// <summary>
    /// カウントダウン後に戦闘を再開する
    /// </summary>
    private IEnumerator StartCountDownAndResumeSpawning()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            for (
                int i = CountdownStart;
                i > 0;
                i--)
            {
                countdownText.text =
                    i.ToString();

                yield return new WaitForSeconds(1f);
            }

            countdownText.text = "Go!";

            yield return new WaitForSeconds(
                CountdownGoDuration);

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(
                CountdownStart +
                CountdownGoDuration);
        }

        isCountingDown = false;
        countdownCoroutine = null;

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResumeAfterShop();
        }
        else
        {
            Debug.LogWarning(
                "WaveManagerが見つかりません。",
                this);
        }
    }

    /// <summary>
    /// ランダムにショップアイテムを生成する
    /// </summary>
    private void GenerateRandomItems()
    {
        ClearItems();

        currentItems.Clear();

        if (allItems == null ||
            allItems.Count == 0)
        {
            Debug.LogWarning(
                "ショップアイテムが設定されていません。",
                this);

            return;
        }

        if (itemPrefab == null ||
            itemParent == null)
        {
            Debug.LogWarning(
                "ショップUIのPrefabまたはParentが設定されていません。",
                this);

            return;
        }

        List<ShopItem> itemPool =
            new List<ShopItem>();

        foreach (ShopItem item in allItems)
        {
            if (item != null)
            {
                itemPool.Add(item);
            }
        }

        int displayCount =
            Mathf.Min(
                ShopItemCount,
                itemPool.Count);

        for (
            int i = 0;
            i < displayCount;
            i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    itemPool.Count);

            ShopItem item =
                itemPool[randomIndex];

            itemPool.RemoveAt(randomIndex);

            currentItems.Add(item);
        }

        foreach (ShopItem item in currentItems)
        {
            GameObject itemObject =
                Instantiate(
                    itemPrefab,
                    itemParent);

            ShopItemUI uiScript =
                itemObject.GetComponent<ShopItemUI>();

            if (uiScript == null)
            {
                Debug.LogWarning(
                    "ItemPrefabにShopItemUIが設定されていません。",
                    itemObject);

                Destroy(itemObject);
                continue;
            }

            uiScript.Setup(
                item,
                () => BuyItem(
                    item,
                    itemObject));
        }
    }

    /// <summary>
    /// アイテムを購入する
    /// </summary>
    /// <param name="item">購入するアイテム</param>
    /// <param name="itemObject">購入したアイテムのUIオブジェクト</param>
    private void BuyItem(
        ShopItem item,
        GameObject itemObject)
    {
        if (!IsShopOpen ||
            item == null ||
            itemObject == null)
        {
            return;
        }

        if (!currentItems.Contains(item))
        {
            return;
        }

        if (!CanApplyItemEffect(item.effectType))
        {
            Debug.LogWarning(
                "このアイテムの効果を適用できません。",
                this);

            return;
        }

        if (CoinManager.instance == null)
        {
            Debug.LogWarning(
                "CoinManagerが見つかりません。",
                this);

            return;
        }

        if (!CoinManager.instance.SpendCoins(item.price))
        {
            Debug.LogWarning(
                $"コインが不足しています。必要コイン数 : {item.price}",
                this);

            return;
        }

        Debug.Log(
            $"{item.itemName} を購入しました。");

        currentItems.Remove(item);

        Destroy(itemObject);

        switch (item.effectType)
        {
            case ItemEffectType.Heal:
                healItemCount++;
                break;

            case ItemEffectType.Attack:
                attackBoostCount++;
                break;

            case ItemEffectType.Speed:
                speedBoostCount++;
                break;

            case ItemEffectType.Wizard:
                SummonFollow(wizardPrefab);
                break;

            case ItemEffectType.Warrior:
                SummonFollow(warriorPrefab);
                break;

            case ItemEffectType.Runetracer:
                SpawnRunetracer();
                break;

            case ItemEffectType.MagicBook:
                SpawnMagicBook();
                break;

            case ItemEffectType.Bullet:
                SpawnBullet();
                break;

            default:
                Debug.LogWarning(
                    "未対応のアイテムタイプです。",
                    this);
                break;
        }
    }

    /// <summary>
    /// アイテムの効果を適用できるか確認する
    /// </summary>
    private bool CanApplyItemEffect(
        ItemEffectType effectType)
    {
        switch (effectType)
        {
            case ItemEffectType.Heal:
                return playerHpBar != null;

            case ItemEffectType.Attack:
            case ItemEffectType.Speed:
                return player != null;

            case ItemEffectType.Wizard:
                return wizardPrefab != null &&
                       summonLocation != null;

            case ItemEffectType.Warrior:
                return warriorPrefab != null &&
                       summonLocation != null;

            case ItemEffectType.Runetracer:
                return runetracerManager != null;

            case ItemEffectType.MagicBook:
                return magicBookManager != null;

            case ItemEffectType.Bullet:
                return bulletIntervalTime != null;

            default:
                return false;
        }
    }

    /// <summary>
    /// 仲間を召喚する
    /// </summary>
    /// <param name="prefab">召喚する仲間のプレハブ</param>
    private void SummonFollow(GameObject prefab)
    {
        if (prefab == null ||
            summonLocation == null)
        {
            return;
        }

        GameObject fellowObject =
            Instantiate(
                prefab,
                summonLocation.position,
                summonLocation.rotation);

        if (fellowObject.TryGetComponent(
            out Fellow fellow))
        {
            if (playerTransform != null)
            {
                fellow.SetPlayerTransform(
                    playerTransform);
            }
        }
        else if (fellowObject.TryGetComponent(
            out Warrior warrior))
        {
            if (playerTransform != null)
            {
                warrior.SetPlayerTransform(
                    playerTransform);
            }
        }
        else
        {
            Debug.LogWarning(
                "召喚したオブジェクトにFellowまたはWarriorが設定されていません。",
                fellowObject);
        }
    }

    /// <summary>
    /// ルーントレーサーを生成する
    /// </summary>
    private void SpawnRunetracer()
    {
        if (runetracerManager == null)
        {
            Debug.LogWarning(
                "RunetracerManagerが設定されていません。",
                this);

            return;
        }

        runetracerManager.SpawnRunetracer();
    }

    /// <summary>
    /// 魔法書を生成する
    /// </summary>
    private void SpawnMagicBook()
    {
        if (magicBookManager == null)
        {
            Debug.LogWarning(
                "MagicBookManagerが設定されていません。",
                this);

            return;
        }

        magicBookManager.SpawnAndRotate();
    }

    /// <summary>
    /// 弾幕武器を有効化または強化する
    /// </summary>
    private void SpawnBullet()
    {
        if (bulletIntervalTime == null)
        {
            Debug.LogWarning(
                "BulletIntervalTimeが設定されていません。",
                this);

            return;
        }

        if (bulletIntervalTime.enabled)
        {
            bulletIntervalTime.ReduceInterval(
                BulletIntervalReduction);
        }
        else
        {
            bulletIntervalTime.enabled = true;

            Debug.Log(
                "Bulletのスクリプトを有効化しました。");
        }
    }

    /// <summary>
    /// リロール価格の表示を更新する
    /// </summary>
    private void UpdateRerollPriceDisplay()
    {
        if (rerollPriceText != null)
        {
            rerollPriceText.text =
                rerollPrice.ToString();
        }
    }

    /// <summary>
    /// アイテムをリロールする
    /// </summary>
    public void RerollItems()
    {
        if (!IsShopOpen ||
            isCountingDown)
        {
            return;
        }

        if (CoinManager.instance == null)
        {
            Debug.LogWarning(
                "CoinManagerが見つかりません。",
                this);

            return;
        }

        if (!CoinManager.instance.SpendCoins(
            rerollPrice))
        {
            Debug.LogWarning(
                "コインが足りません。",
                this);

            return;
        }

        GenerateRandomItems();

        rerollPrice =
            Mathf.Min(
                rerollPrice +
                RerollPriceIncrease,
                maxRerollPrice);

        UpdateRerollPriceDisplay();
    }

    /// <summary>
    /// 購入した効果を適用する
    /// </summary>
    private void ApplyPurchasedEffects()
    {
        if (healItemCount > 0)
        {
            if (playerHpBar != null)
            {
                int totalHeal =
                    HealAmountPerItem *
                    healItemCount;

                playerHpBar.Heal(totalHeal);
            }

            healItemCount = 0;
        }

        if (attackBoostCount > 0)
        {
            if (player != null)
            {
                int totalAttackIncrease =
                    AttackIncreasePerItem *
                    attackBoostCount;

                player.IncreaseAttack(
                    totalAttackIncrease);
            }

            attackBoostCount = 0;
        }

        if (speedBoostCount > 0)
        {
            if (player != null)
            {
                float totalSpeedIncrease =
                    SpeedIncreasePerItem *
                    speedBoostCount;

                player.IncreaseSpeed(
                    totalSpeedIncrease);
            }

            speedBoostCount = 0;
        }
    }

    /// <summary>
    /// アイテムUIをすべて削除する
    /// </summary>
    private void ClearItems()
    {
        if (itemParent == null)
        {
            return;
        }

        for (
            int i = itemParent.childCount - 1;
            i >= 0;
            i--)
        {
            Destroy(
                itemParent.GetChild(i).gameObject);
        }
    }
}