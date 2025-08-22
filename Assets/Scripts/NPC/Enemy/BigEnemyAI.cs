using System.Collections;
using UnityEngine;

public class BigEnemyAI : EnemyAI
{   
    [Header("BigEnemyAI设置")]
    [SerializeField] private Animator animator;
    [Header("投射物设置")]
    [Tooltip("投射物预制体")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("投射物发射点")]
    [SerializeField] private Transform firePoint;
    [Tooltip("发射频率（秒）")]
    [SerializeField] private float ProjectileFireInterval = 2f;
    // 新增：每次发射的投射物数量（可在 Inspector 配置），默认 1
    [Tooltip("每次发射的投射物数量（至少为1）")]
    [SerializeField, Min(1)] private int ProjectilesPerShot = 1;
    // 新增：扇形散射与连发（Burst）配置
    [Tooltip("扇形散射的总角度（度），0 表示不散射（所有子弹同一方向）")]
    [SerializeField, Min(0f)] private float spreadAngleDegrees = 0f;
    
    [Header("连发设置")]
    [Tooltip("每轮连发的发射次数（至少为1）")]
    [SerializeField, Min(1)] private int burstCount = 1;
    [Tooltip("每次连发之间的时间间隔（秒），0 表示瞬时连发")]
    [SerializeField, Min(0f)] private float burstInterval = 0.1f;
    
    private bool isTriggered = false; // 是否已被触发
    private bool canMove = true; // 是否可以移动
    private BigEnemy bigEnemy; // BigEnemy组件引用
    private Coroutine fireCoroutine; // 发射投射物的协程

    private void Awake()
    {
        animator = animator ? animator : GetComponent<Animator>();
        bigEnemy = GetComponent<BigEnemy>();
        // 保护性校验，避免运行时配置被设置为非法值
        if (ProjectilesPerShot < 1) ProjectilesPerShot = 1;
        if (spreadAngleDegrees < 0f) spreadAngleDegrees = 0f;
        if (burstCount < 1) burstCount = 1;
        if (burstInterval < 0f) burstInterval = 0f;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
        animator.SetFloat("VelocityX",  rb.linearVelocity.x);
    }
    
    /// <summary>
    /// 当玩家进入left_0子对象的触发区域时调用
    /// 注意：此方法应该放在left_0子对象上，而不是BigEnemy父对象上
    /// </summary>
    /// <param name="other">碰撞的对象</param>
    public void OnPlayerEnterLeft0Area()
    {
        if (!isTriggered)
        {
            Debug.Log("玩家进入left_0区域，触发BigEnemy行为");
            TriggerBigEnemyBehavior();
        }
    }
    
    /// <summary>
    /// 当玩家离开left_0子对象的触发区域时调用
    /// 注意：此方法应该从left_0子对象调用，而不是BigEnemy父对象
    /// </summary>
    /// <param name="other">离开碰撞的对象</param>
    public void OnPlayerExitLeft0Area()
    {
        if (isTriggered)
        {
            Debug.Log("玩家离开left_0区域，恢复BigEnemy行为");
            RestoreBigEnemyBehavior();
        }
    }
    
    /// <summary>
    /// 触发BigEnemy的特殊行为
    /// </summary>
    private void TriggerBigEnemyBehavior()
    {
        isTriggered = true;
        canMove = false; // 停止移动
        
        // 停止移动动画
        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("VelocityX", 0f);
        
        // 开始生命值衰减和发射投射物
        if (bigEnemy != null)
        {
            bigEnemy.StartHealthDecay();
            fireCoroutine = StartCoroutine(FireProjectilesCoroutine());
        }
    }
    
    /// <summary>
    /// 恢复BigEnemy的正常行为
    /// </summary>
    private void RestoreBigEnemyBehavior()
    {
        isTriggered = false;
        canMove = true; // 恢复移动
        
        // 停止生命值衰减和发射投射物
        if (bigEnemy != null)
        {
            bigEnemy.StopHealthDecay();
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }
        }
    }

    /// <summary>
    /// 定期发射投射物的协程
    /// </summary>
    private IEnumerator FireProjectilesCoroutine()
    {
        while (true)
        {
            // 连发：一轮中按照 burstCount 次发射，每次间隔 burstInterval 秒
            for (int b = 0; b < burstCount; b++)
            {
                FireProjectile();
                if (b < burstCount - 1)
                {
                    yield return new WaitForSeconds(burstInterval);
                }
            }
            // 轮与轮之间的间隔
            yield return new WaitForSeconds(ProjectileFireInterval);
        }
    }

    /// <summary>
    /// 发射投射物（数量可配置 + 扇形散射），目标为距离玩家最近的Enemy
    /// </summary>
    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab未在BigEnemyAI中设置！");
            return;
        }
        if (firePoint == null)
        {
            Debug.LogError("Fire Point未在BigEnemyAI中设置！");
            return;
        }
        
        // 寻找距离玩家最近的Enemy
        Transform nearestEnemyToPlayer = FindNearestEnemyToPlayer();
        if (nearestEnemyToPlayer == null)
        {
            Debug.Log("未找到可攻击的Enemy目标");
            return;
        }
        
        // 计算扇形发射的角度分布：当数量为1时角度偏移为0；大于1时在 [-spread/2, spread/2] 等分
        int count = Mathf.Max(1, ProjectilesPerShot);
        float spread = Mathf.Max(0f, spreadAngleDegrees);
        float startAngle = count > 1 ? -spread * 0.5f : 0f;
        float step = count > 1 ? (spread / (count - 1)) : 0f;
        
        for (int i = 0; i < count; i++)
        {
            float angleOffset = (count == 1) ? 0f : (startAngle + step * i);
            // 2D 下围绕 Z 轴旋转，若为 3D 项目可改为 Vector3.up/Vector3.right 等轴向
            Quaternion shotRotation = Quaternion.AngleAxis(angleOffset, Vector3.forward) * firePoint.rotation;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, shotRotation);
            
            // 设置投射物的所有者和目标，以避免自我碰撞
            BigEnemyProjectile projectileScript = projectile.GetComponent<BigEnemyProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetOwner(gameObject); // 将此 BigEnemy 设置为所有者
                projectileScript.SetTarget(nearestEnemyToPlayer); // 设置目标
            }
        }
        Debug.Log($"成功发射投射物 x{count}（spread={spread}°）");
    }
    
    /// <summary>
    /// 寻找距离玩家最近的Enemy
    /// </summary>
    /// <returns>距离玩家最近的Enemy的Transform，如果没有找到则返回null</returns>
    private Transform FindNearestEnemyToPlayer()
    {
        // 寻找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("未找到玩家对象");
            return null;
        }
        
        // 寻找所有标签为Enemy的对象
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.Log("场景中没有Enemy对象");
            return null;
        }
        
        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        foreach (GameObject enemy in enemies)
        {
            // 排除已死亡的敌人
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null && enemyComponent.CurrentHealth <= 0)
            {
                continue;
            }
            
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        
        return nearestEnemy;
    }
    
    private void OnDisable()
    {
        if (GameManager.instance) GameManager.instance.BigMonsterKilledAmount++;
    }
}
