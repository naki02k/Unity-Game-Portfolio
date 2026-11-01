using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// プレイヤーに追従し、敵を攻撃するウォーリアの行動を制御するクラス
/// </summary>
public class Warrior : MonoBehaviour
{
    /// <summary>
    /// 追跡するプレイヤー
    /// </summary>
    private Transform player;

    /// <summary>
    /// 投げ斧プレハブ
    /// </summary>
    [SerializeField]
    private AxeController axePrefab;

    /// <summary>
    /// プレイヤー追従距離
    /// </summary>
    [SerializeField]
    private float followDistance = 3f;

    /// <summary>
    /// 敵検知範囲
    /// </summary>
    [SerializeField]
    private float detectRange = 10f;

    /// <summary>
    /// 攻撃範囲
    /// </summary>
    [SerializeField]
    private float attackRange = 2f;

    /// <summary>
    /// 攻撃クールダウン
    /// </summary>
    [SerializeField]
    private float attackCooldown = 1f;

    /// <summary>
    /// ナビゲーション制御
    /// </summary>
    private NavMeshAgent2D agent;

    /// <summary>
    /// 現在のターゲット
    /// </summary>
    private Transform currentTarget;

    /// <summary>
    /// 最後に攻撃した時間
    /// </summary>
    private float lastAttackTime;

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 移動中か
    /// </summary>
    private bool isMoving;

    /// <summary>
    /// プレイヤーを設定する
    /// </summary>
    public void SetPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent2D>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent2D not found.", this);
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        // 敵がいない場合はプレイヤーを追従する
        if (currentTarget == null)
        {
            FollowPlayer();
            DetectEnemies();
            FacePlayer();
            return;
        }

        // 敵がいる場合は敵を追跡・攻撃する
        ChaseAttackEnemy();
        FaceEnemy();
    }

    /// <summary>
    /// プレイヤーを追従する
    /// </summary>
    private void FollowPlayer()
    {
        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > followDistance + 0.5f)
        {
            agent.SetDestination(player.position);

            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool("isFollowing", true);
            }

            return;
        }

        if (distanceToPlayer <= followDistance)
        {
            agent.SetDestination(transform.position);

            if (isMoving)
            {
                isMoving = false;
                animator.SetBool("isFollowing", false);
            }
        }
    }

    /// <summary>
    /// 敵を検知する
    /// </summary>
    private void DetectEnemies()
    {
        Collider2D[] hitColliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                detectRange);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") ||
                hitCollider.CompareTag("Boss"))
            {
                currentTarget = hitCollider.transform;
                break;
            }
        }
    }

    /// <summary>
    /// 敵を追跡・攻撃する
    /// </summary>
    private void ChaseAttackEnemy()
    {
        if (currentTarget == null)
        {
            return;
        }

        float distanceToEnemy =
            Vector3.Distance(
                transform.position,
                currentTarget.position);

        if (distanceToEnemy <= attackRange)
        {
            AttackEnemy();
            return;
        }

        agent.SetDestination(currentTarget.position);
        animator.SetBool("isFollowing", true);
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    private void AttackEnemy()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        agent.SetDestination(transform.position);
        animator.SetBool("isFollowing", false);

        ThrowAxe();
    }

    /// <summary>
    /// 斧を投げる
    /// </summary>
    private void ThrowAxe()
    {
        if (axePrefab == null || currentTarget == null)
        {
            return;
        }

        AxeController axe =
            Instantiate(
                axePrefab,
                transform.position,
                Quaternion.identity);

        axe.Initialize(
            currentTarget.position,
            3f,
            null);
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        if (player.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// 敵の方向を向く
    /// </summary>
    private void FaceEnemy()
    {
        if (currentTarget == null)
        {
            return;
        }

        if (currentTarget.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (currentTarget.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// 検知範囲を表示する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}