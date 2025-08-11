using UnityEngine;

/// <summary>
/// 敌人自动移动系统 - 让敌人自动朝着黄金树方向移动
/// 包含寻路、移动速度控制、停止距离等功能
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("敌人移动速度")]
    [SerializeField] private float moveSpeed = 2f;
    
    [Tooltip("距离目标的停止距离")]
    [SerializeField] private float stoppingDistance = 0.5f;
    
    [Tooltip("移动平滑度")]
    [SerializeField] private float smoothTime = 0.3f;

    [Header("目标设置")]
    [Tooltip("黄金树的标签")]
    [SerializeField] private string targetTag = "GoldenTree";
    
    [Tooltip("黄金树的层级")]
    [SerializeField] private LayerMask targetLayer = -1;

    private Transform targetTree;
    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private bool hasTarget = false;

    /// <summary>
    /// 初始化组件和查找目标
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindGoldenTree();
    }

    /// <summary>
    /// 每帧更新移动逻辑
    /// </summary>
    private void Update()
    {
        if (!hasTarget)
        {
            FindGoldenTree();
            return;
        }

        MoveTowardsTarget();
    }

    /// <summary>
    /// 查找场景中的黄金树
    /// </summary>
    private void FindGoldenTree()
    {
        GameObject treeObject = GameObject.FindGameObjectWithTag(targetTag);
        if (treeObject != null)
        {
            targetTree = treeObject.transform;
            hasTarget = true;
            Debug.Log($"找到目标黄金树: {treeObject.name}");
        }
        else
        {
            Debug.LogWarning("未找到黄金树！请确保场景中有标签为\"GoldenTree\"的对象。");
        }
    }

    /// <summary>
    /// 向目标移动
    /// </summary>
    private void MoveTowardsTarget()
    {
        if (targetTree == null) return;

        // 计算到目标的距离
        Vector2 direction = (Vector2)(targetTree.position - transform.position);
        float distance = direction.magnitude;

        // 如果距离大于停止距离，继续移动
        if (distance > stoppingDistance)
        {
            direction.Normalize();
            
            // 使用平滑移动
            Vector2 targetVelocity = direction * moveSpeed;
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, smoothTime);
            
            // 根据移动方向翻转精灵
            FlipSprite(direction.x);
        }
        else
        {
            // 到达目标，停止移动
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 根据移动方向翻转精灵
    /// </summary>
    /// <param name="moveDirection">移动方向</param>
    private void FlipSprite(float moveDirection)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (moveDirection > 0)
            {
                spriteRenderer.flipX = false; // 向右移动
            }
            else if (moveDirection < 0)
            {
                spriteRenderer.flipX = true;  // 向左移动
            }
        }
    }

    /// <summary>
    /// 设置新的移动速度
    /// </summary>
    /// <param name="newSpeed">新的速度值</param>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0f, newSpeed);
    }

    /// <summary>
    /// 设置新的目标
    /// </summary>
    /// <param name="newTarget">新的目标Transform</param>
    public void SetTarget(Transform newTarget)
    {
        targetTree = newTarget;
        hasTarget = (newTarget != null);
    }

    /// <summary>
    /// 获取当前移动速度
    /// </summary>
    /// <returns>当前移动速度</returns>
    public float GetCurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    /// <summary>
    /// 获取到目标的距离
    /// </summary>
    /// <returns>到目标的距离</returns>
    public float GetDistanceToTarget()
    {
        if (targetTree == null) return float.MaxValue;
        return Vector2.Distance(transform.position, targetTree.position);
    }

    /// <summary>
    /// 判断是否到达目标
    /// </summary>
    /// <returns>是否到达目标</returns>
    public bool HasReachedTarget()
    {
        if (targetTree == null) return false;
        return GetDistanceToTarget() <= stoppingDistance;
    }

    
    
}