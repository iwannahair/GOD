using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    
    [Header("组件引用")]
    private Rigidbody2D rb;
    private Vector2 movement;
    
    void Start()
    {
        // 获取Rigidbody2D组件
        rb = GetComponent<Rigidbody2D>();
        
        // 如果没有Rigidbody2D组件，添加一个
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // 2D俯视角游戏通常不需要重力
        }
    }
    
    void Update()
    {
        // 获取输入
        GetInput();
    }
    
    void FixedUpdate()
    {
        // 移动玩家
        MovePlayer();
    }
    
    void GetInput()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D键或左右箭头
        float vertical = Input.GetAxisRaw("Vertical");     // W/S键或上下箭头
        
        // 创建移动向量
        movement = new Vector2(horizontal, vertical);
        
        // 标准化向量，防止对角线移动过快
        if (movement.magnitude > 1f)
        {
            movement = movement.normalized;
        }
    }
    
    void MovePlayer()
    {
        // 使用Rigidbody2D移动，保持物理交互
        Vector2 targetVelocity = movement * moveSpeed;
        rb.linearVelocity = targetVelocity;
    }
    
    // 可选：获取当前移动方向（供其他脚本使用）
    public Vector2 GetMovementDirection()
    {
        return movement;
    }
    
    // 可选：获取当前移动速度（供其他脚本使用）
    public float GetCurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }
    
    // 可选：设置移动速度
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}