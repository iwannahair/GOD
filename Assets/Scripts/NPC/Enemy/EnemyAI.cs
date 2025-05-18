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
        player = GameManager.instance.PlayerTran;
        
        if(player == null)
        {
            Debug.LogError("找不到玩家对象！请确保GameManager里赋值了PlayerTran");
        }
    }


    private void FixedUpdate()
    {
        Move();
    }

    protected void Move()
    {
        // 删除对 HumanFollowerTail 的引用，直接使用 PlayerTran
        player = GameManager.instance.PlayerTran;
        if(player){
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // 使用刚体移动更稳定
        }
        else
        {
            Vector2 direction = (GameManager.instance.PlayerTran.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // 使用刚体移动更稳定
        }
        
        // 删除 Building 相关代码
        /*
        if (targetBuilding)
        {
            timer -= Time.fixedDeltaTime;
            if (timer<=0)
            {
                timer = attackCooldown;
                targetBuilding.TakeDamage(damage);
            }
        }
        */
    }
}
