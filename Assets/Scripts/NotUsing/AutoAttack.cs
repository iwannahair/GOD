using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackRange = 5f;    // 攻击范围
    public float attackRate = 1f;    // 每秒攻击次数
    public int damage = 10;         // 每次攻击伤害值
    public GameObject projectilePrefab; // 投射物预制体
    
    private float nextAttackTime = 0f;
    
    void Update()
    {
        // 检查是否到了可以攻击的时间
        if(Time.time >= nextAttackTime)
        {
            // 尝试寻找并攻击最近的敌人
            TryAttackNearestEnemy();
            // 设置下次攻击时间
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }
    
    void TryAttackNearestEnemy()
    {
        // 1. 寻找范围内最近的敌人
        GameObject nearestEnemy = FindNearestEnemy();
        
        // 2. 如果找到敌人且在攻击范围内
        if(nearestEnemy != null && 
           Vector2.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
        {
            // 3. 执行攻击
            Attack(nearestEnemy);
        }
    }
    
    GameObject FindNearestEnemy()
    {
        // 获取所有带有"Enemy"标签的游戏对象
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        
        foreach(GameObject enemy in enemies)
        {
            // 计算与敌人的距离
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            
            // 更新最近敌人
            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        
        return nearestEnemy;
    }
    
    void Attack(GameObject target)
    {
        // 创建投射物
        if(projectilePrefab != null)
        {
            GameObject projectile = Instantiate(
                projectilePrefab, 
                transform.position, 
                Quaternion.identity);
                
            // 设置投射物方向和速度
            Vector2 direction = (target.transform.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;
            
            // 设置伤害值
            Projectile projScript = projectile.GetComponent<Projectile>();
            if(projScript != null)
            {
                projScript.damage = damage;
            }
        }
        else
        {
            // 直接伤害(近战攻击)
            target.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}