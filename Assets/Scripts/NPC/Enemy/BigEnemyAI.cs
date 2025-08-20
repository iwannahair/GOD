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
    [SerializeField] private float fireRate = 2f;
    
    private bool isTriggered = false; // 是否已被触发
    private bool canMove = true; // 是否可以移动
    private BigEnemy bigEnemy; // BigEnemy组件引用
    private Coroutine fireCoroutine; // 发射投射物的协程

    private void Awake()
    {
        animator = animator ? animator : GetComponent<Animator>();
        bigEnemy = GetComponent<BigEnemy>();
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
            FireProjectile();
            yield return new WaitForSeconds(fireRate);
        }
    }

    /// <summary>
    /// 发射单个投射物，目标为距离玩家最近的Enemy
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
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // 设置投射物的所有者和目标，以避免自我碰撞
        BigEnemyProjectile projectileScript = projectile.GetComponent<BigEnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetOwner(gameObject); // 将此 BigEnemy 设置为所有者
            projectileScript.SetTarget(nearestEnemyToPlayer); // 设置目标
        }
        Debug.Log($"成功发射投射物，目标：{nearestEnemyToPlayer.name}！");
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
