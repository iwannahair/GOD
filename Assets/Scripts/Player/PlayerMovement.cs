using UnityEngine;

/// <summary>
/// 玩家移动控制脚本
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("玩家的移动速度")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerMovement: Rigidbody2D component not found on this GameObject.");
        }
    }

    void Update()
    {
        // 获取WASD输入
        movementInput.x = Input.GetAxisRaw("Horizontal"); // A/D键或左右箭头
        movementInput.y = Input.GetAxisRaw("Vertical");   // W/S键或上下箭头

        // 归一化移动向量，防止斜向移动过快
        movementInput.Normalize();
    }

    void FixedUpdate()
    {
        // 在FixedUpdate中应用物理移动
        if (rb != null)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 设置玩家移动速度
    /// </summary>
    /// <param name="newSpeed">新的移动速度</param>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}