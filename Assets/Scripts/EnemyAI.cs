using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;  // 添加刚体引用

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameManager.instance.PlayerTran;
        
        if(player == null)
        {
            Debug.LogError("找不到玩家对象！请确保GameManager里赋值了PlayerTran");
        }
    }

    void FixedUpdate()
    {
        player = GameManager.instance.HumanFollowerTail is not null ? GameManager.instance.HumanFollowerTail : GameManager.instance.PlayerTran;
        if(player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // 使用刚体移动更稳定
            
        }
    }
    
}