using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;  // 添加刚体引用

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if(player == null)
        {
            Debug.LogError("找不到玩家对象！请确保玩家有'Player'标签");
        }
    }

    void FixedUpdate()
    {
        if(player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // 使用刚体移动更稳定
        }
    }
}