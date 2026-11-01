using System.Collections;
using UnityEngine;

/// <summary>
/// プレイヤーキャラクターの動き、攻撃、ステータスを管理するクラス
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// 攻撃時に使用するAudioSourceのインデックス
    /// </summary>
    private const int AttackSoundIndex = 0;

    /// <summary>
    /// ステータス上昇時に使用するAudioSourceのインデックス
    /// </summary>
    private const int StatusUpSoundIndex = 1;

    /// <summary>
    /// 斬撃エフェクトの表示時間
    /// </summary>
    private const float SlashEffectDuration = 1f;

    /// <summary>
    /// ステータス上昇エフェクトの表示時間
    /// </summary>
    private const float StatusEffectDuration = 2f;

    /// <summary>
    /// 攻撃間隔の最小値
    /// </summary>
    private const float MinAttackInterval = 0.01f;

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static Player instance { get; private set; }

    [Header("Settings")]

    /// <summary>
    /// プレイヤーの移動速度
    /// </summary>
    [SerializeField]
    private float moveSpeed = 0.02f;

    [Header("Attack")]

    /// <summary>
    /// プレイヤーの攻撃力
    /// </summary>
    [SerializeField]
    private int attack = 1;

    /// <summary>
    /// 攻撃間隔
    /// </summary>
    [SerializeField]
    private float attackInterval = 2f;

    /// <summary>
    /// 攻撃判定の中心となるTransform
    /// </summary>
    [SerializeField]
    private Transform attackPoint;

    /// <summary>
    /// 攻撃が届く範囲の半径
    /// </summary>
    [SerializeField]
    private float attackRadius;

    /// <summary>
    /// 攻撃対象のレイヤー
    /// </summary>
    [SerializeField]
    private LayerMask enemyLayer;

    [Header("Prefabs")]

    /// <summary>
    /// 斬撃エフェクトのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject slashEffectPrefab;

    /// <summary>
    /// 攻撃力アップ時のエフェクト
    /// </summary>
    [SerializeField]
    private GameObject attackEffectPrefab;

    /// <summary>
    /// 移動速度アップ時のエフェクト
    /// </summary>
    [SerializeField]
    private GameObject speedEffectPrefab;

    [Header("References")]

    /// <summary>
    /// アニメーション制御用
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 効果音再生用
    /// </summary>
    private AudioSource[] audioSources;

    /// <summary>
    /// 自動攻撃コルーチン
    /// </summary>
    private Coroutine autoAttackCoroutine;

    /// <summary>
    /// 現在の攻撃力
    /// </summary>
    public int AttackPower => attack;

    /// <summary>
    /// 現在の移動速度
    /// </summary>
    public float CurrentMoveSpeed => moveSpeed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSources = GetComponents<AudioSource>();

        autoAttackCoroutine =
            StartCoroutine(AutoAttackCoroutine());
    }

    private void Update()
    {
        HandleMovement();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void HandleMovement()
    {
        if (Time.timeScale <= 0f)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 inputDir =
            new Vector2(h, v).normalized;

        if (inputDir.magnitude > 0f)
        {
            transform.position +=
                (Vector3)inputDir *
                moveSpeed *
                Time.deltaTime;

            if (h != 0f)
            {
                transform.localScale =
                    new Vector3(
                        Mathf.Sign(h),
                        1f,
                        1f);
            }

            if (animator != null)
            {
                animator.SetFloat("speed", 1f);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("speed", 0f);
            }
        }
    }

    /// <summary>
    /// 一定間隔で自動攻撃を行う
    /// </summary>
    private IEnumerator AutoAttackCoroutine()
    {
        while (true)
        {
            bool isShopOpen =
                ShopManager.instance != null &&
                ShopManager.instance.IsShopOpen;

            if (!isShopOpen && Time.timeScale != 0f)
            {
                Attack();
            }

            yield return new WaitForSeconds(
                Mathf.Max(
                    MinAttackInterval,
                    attackInterval));
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        if (slashEffectPrefab == null ||
            attackPoint == null)
        {
            return;
        }

        PlaySound(AttackSoundIndex);

        int finalDamage =
            attack + Random.Range(1, 6);

        GameObject slash =
            Instantiate(
                slashEffectPrefab,
                attackPoint.position,
                Quaternion.identity,
                transform);

        if (slash.TryGetComponent(
            out SlashEffect slashEffect))
        {
            slashEffect.Initialize(
                finalDamage,
                enemyLayer,
                attackRadius);
        }

        Destroy(
            slash,
            SlashEffectDuration);
    }

    /// <summary>
    /// 攻撃力を増加させる
    /// </summary>
    public void IncreaseAttack(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        attack += amount;

        SpawnEffect(attackEffectPrefab);
        PlaySound(StatusUpSoundIndex);
    }

    /// <summary>
    /// 移動速度を増加させる
    /// </summary>
    public void IncreaseSpeed(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        moveSpeed += amount;

        SpawnEffect(speedEffectPrefab);
        PlaySound(StatusUpSoundIndex);
    }

    /// <summary>
    /// ステータス上昇エフェクトを生成する
    /// </summary>
    private void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null)
        {
            return;
        }

        GameObject effect =
            Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.identity);

        Destroy(
            effect,
            StatusEffectDuration);
    }

    /// <summary>
    /// 攻撃範囲を可視化する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }

    /// <summary>
    /// 効果音を再生する
    /// </summary>
    /// <param name="index">再生するAudioSourceのインデックス</param>
    private void PlaySound(int index)
    {
        if (audioSources != null &&
            index >= 0 &&
            index < audioSources.Length &&
            audioSources[index] != null)
        {
            audioSources[index].Play();
        }
    }
}