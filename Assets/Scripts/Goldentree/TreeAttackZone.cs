using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 神树攻击区域控制器
/// 控制神树的攻击行为，包括玩家检测、敌人攻击、投射物发射等。
/// </summary>
public class TreeAttackZone : MonoBehaviour
{
    [Header("范围设置")]
    [Tooltip("攻击范围半径(同时也是玩家检测范围)")]
    public float attackRadius = 8f;

    [Header("攻击设置")]
    [Tooltip("每次攻击造成的伤害")]
    public float attackDamage = 10f;
    [Tooltip("攻击间隔时间（秒）")]
    public float attackInterval = 1f;
    private float attackTimer;

    [Header("投射物设置")]
    [Tooltip("神树发射的投射物预制体")]
    public GameObject projectilePrefab;
    [Tooltip("投射物发射点")]
    public Transform projectileSpawnPoint;
    [Tooltip("投射物飞行速度")]
    public float projectileSpeed = 10f;

    [Header("标签设置")]
    [Tooltip("玩家对象的标签")]
    public string playerTag = "Player";
    [Tooltip("敌人对象的标签")]
    public string enemyTag = "Enemy";

    [Header("预制体管理器引用")]
    [Tooltip("PrefabManager的引用，用于获取已实例化预制体信息")]
    public PrefabManager prefabManager;

    private bool isPlayerInRange = false;
    private Collider2D playerCollider;
    private float baseAttackRadius; // 保存初始攻击范围
    private CircleCollider2D circleCollider; // 神树的圆形碰撞器组件



    void Start()
    {
        attackTimer = attackInterval;
        // 保存初始攻击范围
        baseAttackRadius = attackRadius;
        
        // 获取CircleCollider2D组件
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogWarning("TreeAttackZone: 未找到CircleCollider2D组件，将自动添加");
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true; // 设置为触发器
        }
        
        // 初始化碰撞器半径与攻击范围一致
        UpdateColliderRadius();
        
        // 确保投射物发射点已设置
        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform; // 默认为神树自身位置
        }
        
        // 如果没有手动设置PrefabManager引用，尝试自动查找
        if (prefabManager == null)
        {
            prefabManager = FindFirstObjectByType<PrefabManager>();
            if (prefabManager == null)
            {
                Debug.LogWarning("TreeAttackZone: 未找到PrefabManager，攻击范围将不会动态调整");
            }
        }
    }

    void Update()
    {
        // 动态调整攻击范围基于PrefabManager中已实例化预制体的最大索引值
        UpdateAttackRadius();
        
        // 只有当玩家在检测范围内时，才执行攻击逻辑
        if (isPlayerInRange)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Debug.Log("攻击计时器归零，尝试攻击最近的敌人。");
                AttackNearestEnemy();
                attackTimer = attackInterval;
            }
        }
        else
        {
            
        }
    }

    /// <summary>
    /// 根据PrefabManager中已实例化预制体的最大索引值动态更新攻击范围
    /// 攻击范围 = 下标最大的prefab大小
    /// 此范围同时用于玩家检测和敌人攻击
    /// </summary>
    private void UpdateAttackRadius()
    {
        if (prefabManager == null)
            return;

        // 获取当前已实例化预制体的数量（即最大索引值）
        int maxIndex = prefabManager.GetPrefabCount();
        
        if (maxIndex <= 0)
        {
            // 如果没有预制体，使用基础攻击范围
            if (Mathf.Abs(attackRadius - baseAttackRadius) > 0.01f)
            {
                attackRadius = baseAttackRadius;
                // 同步更新碰撞器半径
                UpdateColliderRadius();
                Debug.Log($"TreeAttackZone: 攻击和检测范围已重置为基础值 {attackRadius:F2}");
            }
            return;
        }
        
        // 计算最大prefab的大小：下标+1
        // 在PrefabManager中，prefab大小 = 下标 + 1
        float maxPrefabSize = maxIndex; // 最大下标值就是prefab数量-1，所以这里直接使用maxIndex
        
        // 设置攻击范围等于最大prefab的大小
        float newAttackRadius = maxPrefabSize * baseAttackRadius;
        
        // 更新攻击范围
        if (Mathf.Abs(attackRadius - newAttackRadius) > 0.01f) // 避免频繁的微小更新
        {
            attackRadius = newAttackRadius;
            // 同步更新碰撞器半径
            UpdateColliderRadius();
            Debug.Log($"TreeAttackZone: 攻击和检测范围已更新为 {attackRadius:F2}，与下标最大的prefab大小相同");
        }
    }

    /// <summary>
    /// 攻击最近的敌人
    /// 只有当玩家在检测范围内且有敌人在攻击范围内时才执行。
    /// </summary>
    void AttackNearestEnemy()
    {
        // 再次确认玩家是否在检测范围内
        if (!isPlayerInRange)
        {
            //Debug.Log("玩家不在检测范围内，神树不攻击。");
            return; // 如果玩家不在范围内，则不攻击
        }

        // 查找攻击范围内的所有碰撞体（不限制层级，避免层级设置问题）
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        Debug.Log($"在攻击范围内检测到 {allColliders.Length} 个碰撞体。");

        // 筛选出敌人对象
        List<Collider2D> enemyColliders = new List<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            if (collider.CompareTag(enemyTag))
            {
                enemyColliders.Add(collider);
                Debug.Log($"发现敌人对象: {collider.name}, 标签: {collider.tag}");
            }
            else
            {
                Debug.Log($"非敌人对象: {collider.name}, 标签: {collider.tag}");
            }
        }

        Debug.Log($"筛选后的敌人数量: {enemyColliders.Count}");

        if (enemyColliders.Count > 0)
        {
            // 找到最近的敌人
            GameObject nearestEnemy = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider2D enemyCollider in enemyColliders)
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < minDistance)
                {
                    Debug.Log($"计算敌人距离: {enemyCollider.name}, 距离: {distance}");
                    minDistance = distance;
                    nearestEnemy = enemyCollider.gameObject;
                }
            }

            if (nearestEnemy != null)
            {
                Debug.Log($"神树攻击最近的敌人: {nearestEnemy.name}, 距离: {minDistance}");
                // 发射投射物
                ShootProjectile(nearestEnemy.transform);
            }
            else
            {
                Debug.LogWarning("虽然检测到敌人，但未能确定最近的敌人。");
            }
        }
        else
        {
            Debug.Log("攻击范围内没有标签为 '" + enemyTag + "' 的敌人。");
        }
    }

    /// <summary>
    /// 发射投射物
    /// </summary>
    /// <param name="target">投射物目标Transform</param>
    void ShootProjectile(Transform target)
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            TreeProjectile treeProjectile = projectile.GetComponent<TreeProjectile>();
            if (treeProjectile != null)
            {
                treeProjectile.Initialize(target, attackDamage, projectileSpeed);
            }
            else
            {
                Debug.LogWarning("投射物预制体缺少TreeProjectile组件。");
            }
        }
        else
        {
            Debug.LogWarning("投射物预制体或发射点未设置。");
        }
    }

    /// <summary>
    /// 当有2D碰撞体进入触发器时调用
    /// 用于检测玩家是否进入神树的检测范围。
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            playerCollider = other;
            Debug.Log($"玩家 {other.name} 进入神树检测范围. isPlayerInRange: {isPlayerInRange}");
        }
    }

    /// <summary>
    /// 当有2D碰撞体退出触发器时调用
    /// 用于检测玩家是否离开神树的检测范围。
    /// </summary>
    /// <param name="other">退出触发器的碰撞体</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            playerCollider = null;
            Debug.Log($"玩家 {other.name} 离开神树检测范围. isPlayerInRange: {isPlayerInRange}");
        }
    }

    /// <summary>
    /// 在编辑器中绘制Gizmos，用于可视化攻击和检测范围。
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 绘制攻击和检测范围（现在是同一个范围）
        Gizmos.color = isPlayerInRange ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    /// <summary>
    /// 当脚本在编辑器中加载或值改变时调用。
    /// 用于确保在编辑器中调整参数时，Gizmos能够实时更新。
    /// </summary>
    void OnValidate()
    {
        // 确保半径不会是负值
        if (attackRadius < 0) attackRadius = 0;

        // 确保攻击间隔不会是负值
        if (attackInterval < 0) attackInterval = 0;
        
        // 在编辑器中也同步更新碰撞器半径
        if (Application.isEditor && !Application.isPlaying)
        {
            CircleCollider2D editorCollider = GetComponent<CircleCollider2D>();
            if (editorCollider != null)
            {
                editorCollider.radius = attackRadius;
            }
        }
    }
    
    /// <summary>
    /// 更新碰撞器半径，使其与攻击范围保持一致
    /// </summary>
    private void UpdateColliderRadius()
    {
        if (circleCollider != null)
        {
            circleCollider.radius = attackRadius;
            Debug.Log($"TreeAttackZone: 碰撞器半径已更新为 {attackRadius:F2}");
        }
    }
}