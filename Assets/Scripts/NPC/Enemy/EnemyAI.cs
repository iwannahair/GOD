using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 8;
    private Transform player;
    protected Rigidbody2D rb;  // 添加刚体引用
    
    // 删除 Building 相关变量
    // [SerializeField] private Building targetBuilding;
    // public Building TargetBuilding=>targetBuilding;
    public int Damage => damage;
    private float timer;
    void Start()
    {
        timer = attackCooldown;
        rb = GetComponent<Rigidbody2D>();
        
        // 从EnemySpawner获取玩家引用，减少对GameManager的依赖
        if (EnemySpawner.Instance != null)
        {
            player = EnemySpawner.Instance.playerTransform;
        }
        
        if(player == null)
        {
            Debug.LogError("找不到玩家对象！请确保EnemySpawner中的playerTransform已正确设置");
        }
    }


    private void FixedUpdate()
    {
        Move();
    }

    protected void Move()
    {
        // 从EnemySpawner获取最新的玩家引用，确保引用始终有效
        if (EnemySpawner.Instance != null && EnemySpawner.Instance.playerTransform != null)
        {
            player = EnemySpawner.Instance.playerTransform;
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // 使用刚体移动更稳定
        }
        else
        {
            // 如果无法获取玩家引用，停止移动
            rb.linearVelocity = Vector2.zero;
            Debug.LogWarning("EnemyAI: 无法获取玩家引用，敌人停止移动");
        }
    }
}
