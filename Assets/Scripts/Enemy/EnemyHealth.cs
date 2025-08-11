using UnityEngine;

/// <summary>
/// 敌人生命值管理系统
/// 处理伤害接收、生命值变化和死亡逻辑
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    /// <summary>
    /// 最大生命值
    /// </summary>
    [SerializeField] private int maxHealth = 100;
    
    /// <summary>
    /// 当前生命值
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 死亡特效预制体
    /// </summary>
    [SerializeField] private GameObject deathEffect;
    
    /// <summary>
    /// 资源掉落预制体
    /// </summary>
    [SerializeField] private GameObject resourceDropPrefab;
    
    /// <summary>
    /// 资源掉落概率（0-1）
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField] private float resourceDropChance = 0.7f;
    
    /// <summary>
    /// 掉落资源数量范围
    /// </summary>
    [SerializeField] private int minResourceCount = 1;
    [SerializeField] private int maxResourceCount = 3;

    /// <summary>
    /// 初始化生命值
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 接收伤害并处理死亡逻辑
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // 播放受击特效（可选）
        // PlayHitEffect();
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 执行敌人死亡逻辑
    /// </summary>
    private void Die()
    {
        // 生成死亡特效
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // 生成资源掉落
        SpawnResourceDrops();
        
        // 销毁敌人对象
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 生成资源掉落
    /// </summary>
    private void SpawnResourceDrops()
    {
        if (resourceDropPrefab == null)
            return;
            
        // 根据概率决定是否掉落资源
        if (Random.Range(0f, 1f) <= resourceDropChance)
        {
            // 随机生成1-3个资源
            int dropCount = Random.Range(minResourceCount, maxResourceCount + 1);
            
            for (int i = 0; i < dropCount; i++)
            {
                // 在敌人位置附近随机位置生成资源
                Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
                Instantiate(resourceDropPrefab, spawnPosition, Quaternion.identity);
            }
            
            Debug.Log($"敌人死亡后掉落了 {dropCount} 个资源");
        }
    }

    /// <summary>
    /// 获取当前生命值百分比
    /// </summary>
    /// <returns>生命值百分比（0-1）</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}