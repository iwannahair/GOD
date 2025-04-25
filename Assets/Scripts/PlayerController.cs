using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    
    [Header("生命值设置")]
    public int maxHealth = 100;
    private int currentHealth;  // 添加当前生命值变量
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer renderer;
    

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // 初始化生命值
    }

    void Update()
    {
        // WASD输入处理
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 移动角色
        rb.MovePosition(rb.position + movement.normalized * (moveSpeed * Time.fixedDeltaTime));
        
        // 可选：使角色朝向移动方向
        if(movement != Vector2.zero)
        {
            if ((movement.x < 0 || movement.y > 0) &&!renderer.flipX )
            {
                renderer.flipX = true;
            }

            if ((movement.x > 0 || movement.y < 0)&& renderer.flipX )
            {
                renderer.flipX = false;
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 游戏结束逻辑
        Debug.Log("玩家死亡");
        Destroy(gameObject);
    }
}