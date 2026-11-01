using System.Collections;
using UnityEngine;

/// <summary>
/// ボスのHP、移動、攻撃、死亡時の挙動を管理するクラス
/// </summary>
public class Boss : MonoBehaviour
{
    [Header("移動・攻撃設定")]

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float attackRange = 2f;

    [SerializeField]
    private float rangedAttackCooldown = 5f;

    [SerializeField]
    private float attackInterval = 1f;

    [SerializeField]
    private float rangedAttackDelayTime = 0.5f;

    [SerializeField]
    private float meleeAttackDelayTime = 0.5f;

    [SerializeField]
    private float rangedAttackIntervalTime = 0.3f;

    [SerializeField]
    private float attackHitboxDuration = 0.2f;

    [Header("HP")]

    [SerializeField]
    private int maxHp = 100;

    private int hp;

    [Header("エフェクト")]

    [SerializeField]
    private GameObject rangedAttackEffectPrefab;

    [SerializeField]
    private float rangedEffectLifetime = 2.5f;

    [SerializeField]
    private Collider2D attackHitbox;

    [SerializeField]
    private GameObject childObject;

    [Header("死亡設定")]

    [SerializeField]
    private float deathAnimationDuration = 2f;

    [Header("サウンド")]

    [SerializeField]
    private AudioClip[] attackSounds;

    [Header("ダメージ表示")]

    [SerializeField]
    private GameObject damagePopupPrefab;

    [Header("スコア設定")]

    [SerializeField]
    private int defeatScore = 500;

    private Player player;

    private Animator animator;

    private AudioSource audioSource;

    private BossRotate bossRotate;

    private bool isDead;

    private bool isAttacking;

    private bool isRangedAttacking;

    private float rangedAttackTimer;

    private float attackTimer;

    private float attackRangeSqr;

    private WaitForSeconds rangedAttackDelay;

    private WaitForSeconds meleeAttackDelay;

    private WaitForSeconds rangedAttackInterval;

    private WaitForSeconds attackHitboxDurationWait;

    private Coroutine meleeAttackCoroutine;

    private Coroutine rangedAttackCoroutine;

    private Coroutine deathCoroutine;

    private const int MeleeAttackSoundIndex = 0;

    private const int RangedAttackSoundIndex = 1;

    private const int MinRangedAttackCount = 3;

    private const int MaxRangedAttackCount = 4;

    private void Awake()
    {
        // Animator取得
        if (childObject != null)
        {
            animator = childObject.GetComponent<Animator>();
        }

        // コンポーネント取得
        audioSource = GetComponent<AudioSource>();

        bossRotate = GetComponent<BossRotate>();

        // 待機時間をキャッシュ
        rangedAttackDelay =
            new WaitForSeconds(rangedAttackDelayTime);

        meleeAttackDelay =
            new WaitForSeconds(meleeAttackDelayTime);

        rangedAttackInterval =
            new WaitForSeconds(rangedAttackIntervalTime);

        attackHitboxDurationWait =
            new WaitForSeconds(attackHitboxDuration);

        // 距離比較用
        attackRangeSqr = attackRange * attackRange;

        // 初期状態では攻撃判定を無効化
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        if (animator == null)
        {
            Debug.LogError("Boss: Animatorが取得できていません。", this);
        }
    }

    private void OnEnable()
    {
        // 状態初期化
        isDead = false;

        isAttacking = false;

        isRangedAttacking = false;

        // HPを最大値に戻す
        hp = Mathf.Max(1, maxHp);

        // タイマー初期化
        rangedAttackTimer = 0f;

        attackTimer = 0f;

        // コルーチン参照初期化
        meleeAttackCoroutine = null;

        rangedAttackCoroutine = null;

        deathCoroutine = null;

        // プレイヤー参照更新
        UpdatePlayerReference();

        // 攻撃判定を無効化
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        // アニメーション初期化
        if (animator != null)
        {
            animator.Rebind();

            animator.Update(0f);
        }
    }

    private void OnDisable()
    {
        // 非アクティブ時に攻撃判定を無効化
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        isAttacking = false;

        isRangedAttacking = false;

        meleeAttackCoroutine = null;

        rangedAttackCoroutine = null;

        deathCoroutine = null;
    }

    /// <summary>
    /// プレイヤー参照を更新する
    /// </summary>
    private void UpdatePlayerReference()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        // プレイヤー参照を再取得
        if (player == null)
        {
            UpdatePlayerReference();

            if (player == null)
            {
                return;
            }
        }

        Vector3 playerPos = player.transform.position;

        // XY平面で距離を計算
        Vector2 difference =
            (Vector2)playerPos - (Vector2)transform.position;

        float distanceSqr = difference.sqrMagnitude;

        // プレイヤーの方向を向く
        if (bossRotate != null)
        {
            bossRotate.LookAtPlayer(playerPos);
        }

        // 近距離の場合
        if (distanceSqr <= attackRangeSqr)
        {
            // 遠距離攻撃を停止
            StopRangedAttack();

            rangedAttackTimer = 0f;

            // 近距離攻撃タイマー
            attackTimer += Time.deltaTime;

            if (!isAttacking && attackTimer >= attackInterval)
            {
                attackTimer = 0f;

                meleeAttackCoroutine =
                    StartCoroutine(PerformMeleeAttack());
            }
        }
        else
        {
            // プレイヤーへ移動
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerPos,
                speed * Time.deltaTime
            );

            attackTimer = 0f;

            // 遠距離攻撃
            if (!isRangedAttacking)
            {
                rangedAttackTimer += Time.deltaTime;

                if (rangedAttackTimer >= rangedAttackCooldown)
                {
                    rangedAttackTimer = 0f;

                    rangedAttackCoroutine =
                        StartCoroutine(PerformRangedAttack());
                }
            }
        }
    }

    /// <summary>
    /// 実行中の遠距離攻撃を停止する
    /// </summary>
    private void StopRangedAttack()
    {
        if (rangedAttackCoroutine != null)
        {
            StopCoroutine(rangedAttackCoroutine);

            rangedAttackCoroutine = null;
        }

        isRangedAttacking = false;
    }

    /// <summary>
    /// 遠距離攻撃を行う
    /// </summary>
    private IEnumerator PerformRangedAttack()
    {
        if (isDead || isRangedAttacking)
        {
            yield break;
        }

        isRangedAttacking = true;

        // 遠距離攻撃アニメーション
        if (animator != null)
        {
            animator.SetTrigger("cast");
        }

        yield return rangedAttackDelay;

        if (isDead || player == null || IsPlayerInAttackRange())
        {
            isRangedAttacking = false;

            rangedAttackCoroutine = null;

            yield break;
        }

        PlayAttackSound(RangedAttackSoundIndex);

        if (rangedAttackEffectPrefab == null)
        {
            isRangedAttacking = false;

            rangedAttackCoroutine = null;

            yield break;
        }

        // 3～4回攻撃
        int numberOfAttacks = Random.Range(
            MinRangedAttackCount,
            MaxRangedAttackCount + 1
        );

        for (int i = 0; i < numberOfAttacks; i++)
        {
            if (isDead || player == null || IsPlayerInAttackRange())
            {
                break;
            }

            // 発射時点のプレイヤー位置を取得
            Vector3 targetPosition = player.transform.position;

            GameObject effect = Instantiate(
                rangedAttackEffectPrefab,
                targetPosition,
                Quaternion.identity
            );

            // エフェクトを一定時間後に削除
            Destroy(
                effect,
                Mathf.Max(0.1f, rangedEffectLifetime)
            );

            yield return rangedAttackInterval;
        }

        isRangedAttacking = false;

        rangedAttackCoroutine = null;
    }

    /// <summary>
    /// プレイヤーが近距離攻撃範囲内にいるか
    /// </summary>
    private bool IsPlayerInAttackRange()
    {
        if (player == null)
        {
            return false;
        }

        Vector2 difference =
            (Vector2)player.transform.position -
            (Vector2)transform.position;

        return difference.sqrMagnitude <= attackRangeSqr;
    }

    /// <summary>
    /// 近距離攻撃を行う
    /// </summary>
    private IEnumerator PerformMeleeAttack()
    {
        if (isDead || isAttacking)
        {
            yield break;
        }

        isAttacking = true;

        // 近距離攻撃アニメーション
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }

        // 攻撃開始まで待つ
        yield return meleeAttackDelay;

        if (isDead)
        {
            isAttacking = false;

            meleeAttackCoroutine = null;

            yield break;
        }

        // 攻撃判定ON
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }

        PlayAttackSound(MeleeAttackSoundIndex);

        // 攻撃判定を維持
        yield return attackHitboxDurationWait;

        // 攻撃判定OFF
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        isAttacking = false;

        meleeAttackCoroutine = null;
    }

    /// <summary>
    /// 攻撃音を再生する
    /// </summary>
    private void PlayAttackSound(int index)
    {
        if (audioSource == null ||
            attackSounds == null ||
            index < 0 ||
            index >= attackSounds.Length)
        {
            return;
        }

        if (attackSounds[index] != null)
        {
            audioSource.PlayOneShot(attackSounds[index]);
        }
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead ||
            !gameObject.activeInHierarchy ||
            damage <= 0)
        {
            return;
        }

        hp -= damage;

        hp = Mathf.Max(0, hp);

        Debug.Log(
            "Boss Damage: " + damage + " / HP: " + hp
        );

        // ダメージポップアップ生成
        if (damagePopupPrefab != null)
        {
            Vector3 popupPosition = transform.position + new Vector3(0f, 1f, 0f);

            GameObject popup = Instantiate(
                damagePopupPrefab,
                popupPosition,
                Quaternion.identity
            );

            // ダメージ表示を初期化
            DamagePopup damagePopup =
                popup.GetComponent<DamagePopup>();

            if (damagePopup != null)
            {
                damagePopup.Setup(damage);
            }
            else
            {
                Debug.LogWarning(
                    "Boss: DamagePopupコンポーネントが見つかりません。",
                    popup
                );
            }
        }

        // HPが0以下なら死亡
        if (hp <= 0)
        {
            // 二重死亡防止
            isDead = true;

            // 遠距離攻撃停止
            StopRangedAttack();

            // 近距離攻撃停止
            if (meleeAttackCoroutine != null)
            {
                StopCoroutine(meleeAttackCoroutine);

                meleeAttackCoroutine = null;
            }

            isAttacking = false;

            // スコア加算
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(defeatScore);

                ScoreManager.instance.IncrementBossCount();
            }

            // 死亡処理開始
            deathCoroutine = StartCoroutine(Die());
        }
    }

    /// <summary>
    /// ボスの死亡処理
    /// </summary>
    private IEnumerator Die()
    {
        isAttacking = false;

        isRangedAttacking = false;

        // 攻撃判定を無効化
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        // 死亡アニメーションを優先
        if (animator != null)
        {
            animator.ResetTrigger("attack");

            animator.ResetTrigger("cast");

            animator.ResetTrigger("dead");

            animator.SetTrigger("dead");
        }

        // 死亡アニメーションの再生を待つ
        yield return new WaitForSeconds(
            Mathf.Max(0.1f, deathAnimationDuration)
        );

        // EnemyManagerのプールへ返却
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyDefeated(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

        deathCoroutine = null;
    }
}