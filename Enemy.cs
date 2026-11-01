using System.Collections;
using UnityEngine;

/// <summary>
/// 敵キャラクターの振る舞い（移動、HP、ダメージ、死亡）を制御するクラス
/// </summary>
public class Enemy : MonoBehaviour
{
    private const float ColliderDisableDuration = 0.5f;
    private const float DeathAnimationDuration = 0.5f;

    [Header("Movement Settings")]

    /// <summary>
    /// プレイヤーへ移動する速度
    /// </summary>
    [SerializeField]
    private float speed = 1f;

    /// <summary>
    /// 追跡するプレイヤー
    /// </summary>
    private Player player;

    /// <summary>
    /// プレイヤーの現在位置
    /// </summary>
    private Vector3 playerPos;

    [Header("HP Settings")]

    /// <summary>
    /// ベースの最大HP
    /// </summary>
    [SerializeField]
    private int baseHp = 10;

    /// <summary>
    /// 敵の現在のHP
    /// </summary>
    private int hp;

    /// <summary>
    /// 1ウェーブごとに上昇するHPの割合
    /// </summary>
    [SerializeField]
    private float hpGrowthPerWave = 0.2f;

    [Header("Enemy Type")]

    /// <summary>
    /// ボスかどうか
    /// </summary>
    [SerializeField]
    private bool isBoss;

    [Header("Drop Settings")]

    /// <summary>
    /// 死亡時にコインをドロップするコンポーネント
    /// </summary>
    [SerializeField]
    private CoinDropper coinDropper;

    [Header("Effect Settings")]

    /// <summary>
    /// ダメージポップアップのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject damagePopupPrefab;

    [Header("Component References")]

    /// <summary>
    /// 敵のアニメーションを制御するコンポーネント
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 敵のコライダー
    /// </summary>
    private Collider2D enemyCollider;

    /// <summary>
    /// 敵が死亡しているか
    /// </summary>
    private bool isDead;

    /// <summary>
    /// コライダー無効化コルーチン
    /// </summary>
    private Coroutine colliderDisableCoroutine;

    /// <summary>
    /// 死亡後のプール返却コルーチン
    /// </summary>
    private Coroutine deathCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        UpdatePlayerReference();

        // IntervalTimeが存在する場合は有効化
        IntervalTime timer = GetComponent<IntervalTime>();

        if (timer != null)
        {
            timer.enabled = true;
        }
    }

    private void OnEnable()
    {
        // 前回の状態をリセット
        isDead = false;

        // コライダーを初期状態に戻す
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }

        // アニメーションを初期状態に戻す
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // コルーチン管理情報をリセット
        colliderDisableCoroutine = null;
        deathCoroutine = null;

        // プレイヤー参照を更新
        UpdatePlayerReference();

        // ウェーブに応じてHPを設定
        AdjustHpByWave();
    }

    /// <summary>
    /// プレイヤー参照を取得する
    /// </summary>
    private void UpdatePlayerReference()
    {
        player = Player.instance;
    }

    /// <summary>
    /// ウェーブ数に応じてHPを調整する
    /// </summary>
    private void AdjustHpByWave()
    {
        if (WaveManager.Instance == null)
        {
            hp = Mathf.Max(1, baseHp);
            return;
        }

        int currentWave = Mathf.Max(
            1,
            WaveManager.Instance.CurrentWaveCount
        );

        float multiplier =
            1f + (currentWave - 1) * hpGrowthPerWave;

        hp = Mathf.Max(
            1,
            Mathf.RoundToInt(baseHp * multiplier)
        );

        Debug.Log(
            $"<color=green>[スポーン] {gameObject.name} " +
            $"(Wave {currentWave}): 現在のHPは 【{hp}】 です " +
            $"(ベースの {multiplier}倍)</color>"
        );
    }

    private void Update()
    {
        if (player == null || isDead)
        {
            return;
        }

        playerPos = player.transform.position;

        // プレイヤーを追跡
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerPos,
            speed * Time.deltaTime
        );

        // プレイヤーの方向に応じて向きを変更
        float diffX =
            playerPos.x - transform.position.x;

        if (diffX > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (diffX < 0f)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
    }

    /// <summary>
    /// 攻撃オブジェクトとの接触判定
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !gameObject.activeInHierarchy)
        {
            return;
        }

        if (other.CompareTag("Attack") ||
            other.CompareTag("FellowAttack") ||
            other.CompareTag("Runetracer"))
        {
            int randomDamage = Random.Range(1, 6);

            OnDamage(randomDamage);
        }
    }

    /// <summary>
    /// 外部からダメージを与える
    /// </summary>
    public void OnDamage(int damage)
    {
        if (isDead ||
            !gameObject.activeInHierarchy ||
            damage <= 0)
        {
            return;
        }

        TakeDamage(damage);

        if (!isDead && gameObject.activeInHierarchy)
        {
            StartColliderDisableCoroutine();
        }
    }

    /// <summary>
    /// コライダー無効化コルーチンを開始する
    /// </summary>
    private void StartColliderDisableCoroutine()
    {
        if (enemyCollider == null)
        {
            return;
        }

        if (colliderDisableCoroutine != null)
        {
            StopCoroutine(colliderDisableCoroutine);
        }

        colliderDisableCoroutine = StartCoroutine(
            DisableColliderTemporarily(ColliderDisableDuration)
        );
    }

    /// <summary>
    /// ダメージ処理を行う
    /// </summary>
    private void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("hit");
        }

        hp -= damage;

        ShowDamagePopup(damage);

        // HPが0以下になった場合は死亡処理
        if (hp <= 0)
        {
            isDead = true;

            // コライダーを無効化
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }

            // スコアと撃破数を更新
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(100);

                if (isBoss)
                {
                    ScoreManager.instance.IncrementBossCount();
                }
                else
                {
                    ScoreManager.instance.IncrementEnemyCount();
                }
            }

            // 死亡アニメーション
            if (animator != null)
            {
                animator.SetTrigger("death");
            }

            // コインをドロップ
            if (coinDropper != null)
            {
                coinDropper.DropCoins(transform.position);
            }

            // 死亡アニメーション再生後にプールへ返却
            deathCoroutine = StartCoroutine(
                ReturnToPoolAfterDeath()
            );
        }
    }

    /// <summary>
    /// 死亡アニメーション再生後に敵をプールへ返す
    /// </summary>
    private IEnumerator ReturnToPoolAfterDeath()
    {
        yield return new WaitForSeconds(
            DeathAnimationDuration
        );

        deathCoroutine = null;

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyDefeated(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ダメージポップアップを表示する
    /// </summary>
    private void ShowDamagePopup(int damage)
    {
        if (damagePopupPrefab == null)
        {
            return;
        }

        Vector3 popupPosition =
            transform.position + new Vector3(0f, 0.5f, 0f);

        GameObject popup = Instantiate(
            damagePopupPrefab,
            popupPosition,
            Quaternion.identity
        );

        DamagePopup damagePopup =
            popup.GetComponent<DamagePopup>();

        if (damagePopup != null)
        {
            damagePopup.Setup(damage);
        }
    }

    /// <summary>
    /// 一定時間コライダーを無効にする
    /// </summary>
    private IEnumerator DisableColliderTemporarily(float duration)
    {
        if (enemyCollider == null)
        {
            colliderDisableCoroutine = null;
            yield break;
        }

        enemyCollider.enabled = false;

        yield return new WaitForSeconds(duration);

        if (!isDead && gameObject.activeInHierarchy)
        {
            enemyCollider.enabled = true;
        }

        colliderDisableCoroutine = null;
    }
}