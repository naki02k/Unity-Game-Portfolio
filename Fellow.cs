using UnityEngine;

/// <summary>
/// プレイヤーと一定の距離を保ちながら追従する仲間キャラクター
/// </summary>
public class Fellow : MonoBehaviour
{
    /// <summary>
    /// 追従対象のプレイヤー
    /// </summary>
    [SerializeField]
    private Transform player;

    /// <summary>
    /// プレイヤーとの維持距離
    /// </summary>
    [SerializeField]
    private float followDistance = 3f;

    /// <summary>
    /// 移動を制御するナビゲーションコンポーネント
    /// </summary>
    private NavMeshAgent2D agent;

    /// <summary>
    /// 仲間のアニメーションを制御するコンポーネント
    /// </summary>
    private Animator animator;

    /// <summary>
    /// プレイヤーを追従して移動しているか
    /// </summary>
    private bool isMoving;

    /// <summary>
    /// 追従を開始するための余裕距離
    /// </summary>
    private const float FollowMargin = 0.5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 追従対象のプレイヤーを設定する
    /// </summary>
    /// <param name="playerTransform">追従するプレイヤーのTransform</param>
    public void SetPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        FollowPlayer();
        FacePlayer();
    }

    /// <summary>
    /// プレイヤーとの距離に応じて追従する
    /// </summary>
    private void FollowPlayer()
    {
        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position);

        if (distanceToPlayer >
            followDistance + FollowMargin)
        {
            agent.SetDestination(player.position);

            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool("isFollowing", true);
            }
        }
        else if (distanceToPlayer <= followDistance)
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
    /// プレイヤーの方向を向く
    /// </summary>
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation =
                Quaternion.Euler(0f, 180f, 0f);
        }
    }
}