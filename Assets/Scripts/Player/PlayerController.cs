using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("碰撞设置")]
    
    [Header("光照设置")]
    public Light playerLight; // 玩家光照组件
    public float lightIntensity = 1.0f; // 光照强度
    public Color lightColor = Color.white; // 光照颜色

    [Header("父对象控制设置")]
    public GameObject parentObject; // 需要控制移动的父对象
    
    private bool isInLeft0Area = false; // 是否在left_0区域内
    private bool parentCanMove = true; // 父对象是否可以移动
    private Rigidbody2D parentRb; // 父对象的Rigidbody2D组件
    private Vector2 parentOriginalVelocity; // 父对象的原始速度

    [Header("生命值设置")]
    public int maxHealth = 100;
    private int currentHealth;  // 添加当前生命值变量
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer _renderer;

 

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // 初始化生命值
        if (GameManager.instance) GameManager.instance.OnPlayerHealthChanged += UpdateHealth;
        
        // 初始化父对象的Rigidbody2D组件引用
        if (parentObject != null)
        {
            parentRb = parentObject.GetComponent<Rigidbody2D>();
            if (parentRb == null)
            {
                Debug.LogWarning("父对象没有Rigidbody2D组件！");
            }
        }
        else
        {
            Debug.LogWarning("未分配父对象！请在Inspector中分配需要控制的父对象。");
        }
    }

    public void UpdateHealth()
    {
        currentHealth = GameManager.instance.PlayerHealth;
        TakeDamage(0);
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
       /* if(movement != Vector2.zero)
        {
            if ((movement.x < 0 || movement.y > 0) &&!_renderer.flipX )
            {
                _renderer.flipX = true;
            }

            if ((movement.x > 0 || movement.y < 0)&& _renderer.flipX )
            {
                _renderer.flipX = false;
            }
        }*/
    }

    private void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // 游戏结束逻辑
        Debug.Log("玩家死亡"); 
        StartCoroutine(TurnToStone()); 
    }

    private IEnumerator TurnToStone()
    {
        Color temp = _renderer.color;
        Color targetColor = new Color(85 / 255f, 85 / 255f, 85 / 255f);
        float timer = 0f;
        while (timer<1f)
        {
            timer+=Time.fixedDeltaTime;
            _renderer.color = Color.Lerp(temp, targetColor, timer / 1f);
            yield return new WaitForFixedUpdate();
        }
        enabled = false;
    }

    private void OnDestroy()
    {
        if(GameManager.instance) GameManager.instance.OnPlayerHealthChanged -= UpdateHealth;
    }

    /// <summary>
    /// 当该碰撞体进入另一个触发器时调用（仅2D物理）。
    /// </summary>
    /// <param name="other">进入该触发器的另一个碰撞体。</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 精确检查是否进入left_0区域（只有直接碰撞到left_0子对象才触发）
        if (other.name == "left_0" && !isInLeft0Area)
        {
            isInLeft0Area = true;
            // 启用光照效果
            EnableLight();
            // 停止父对象移动
            StopParentMovement();
            Debug.Log("玩家碰撞到left_0子对象，触发特殊效果");
            return; // 直接返回，避免同时触发敌人碰撞逻辑
        }
        
        // 检查是否与敌人本体碰撞（排除left_0子对象的情况）
        if (other.name != "left_0") // 确保不是left_0子对象
        {
            bool isEnemyCollision = false;
            string enemyTag = "";
            
            // 检查直接碰撞到敌人本体
            if (other.CompareTag("Enemy") || other.CompareTag("BigEnemy"))
            {
                isEnemyCollision = true;
                enemyTag = other.tag;
                Debug.Log($"玩家直接碰撞到{enemyTag}本体");
            }
            // 检查碰撞到敌人的其他子对象（但不是left_0）
            else if (other.transform.parent != null && 
                     (other.transform.parent.CompareTag("Enemy") || other.transform.parent.CompareTag("BigEnemy")))
            {
                isEnemyCollision = true;
                enemyTag = other.transform.parent.tag;
                Debug.Log($"玩家碰撞到{enemyTag}的子对象: {other.name}");
            }
            
            // 如果确认是敌人碰撞，则调用相应的处理方法
            if (isEnemyCollision && StatsForGod.instance != null)
            {
                StatsForGod.instance.OnPlayerHitByEnemy(enemyTag);
            }
        }
    }
    
    /// <summary>
    /// 当该碰撞体离开另一个触发器时调用（仅2D物理）。
    /// </summary>
    /// <param name="other">离开该触发器的另一个碰撞体。</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        // 精确检查是否离开left_0区域（只有离开left_0子对象才触发）
        if (other.name == "left_0" && isInLeft0Area)
        {
            isInLeft0Area = false;
            // 禁用光照效果
            DisableLight();
            // 恢复父对象移动
            ResumeParentMovement();
            Debug.Log("玩家离开left_0子对象，恢复正常状态");
        }
    }

    /// <summary>
    /// 启用玩家光照效果
    /// </summary>
    private void EnableLight()
    {
        if (playerLight != null)
        {
            playerLight.enabled = true;
            playerLight.intensity = lightIntensity;
            playerLight.color = lightColor;
            Debug.Log("玩家进入left_0区域，启用光照效果");
        }
        else
        {
            Debug.LogWarning("PlayerLight组件未分配！请在Inspector中分配Light组件。");
        }
    }

    /// <summary>
    /// 禁用玩家光照效果
    /// </summary>
    private void DisableLight()
    {
        if (playerLight != null)
        {
            playerLight.enabled = false;
            Debug.Log("玩家离开left_0区域，禁用光照效果");
        }
    }

    /// <summary>
    /// 停止父对象移动
    /// </summary>
    private void StopParentMovement()
    {
        if (parentRb != null && parentCanMove)
        {
            // 保存父对象当前的速度
            parentOriginalVelocity = parentRb.linearVelocity;
            // 停止父对象移动
            parentRb.linearVelocity = Vector2.zero;
            parentCanMove = false;
            Debug.Log("玩家进入left_0区域，父对象停止移动");
        }
    }

    /// <summary>
    /// 恢复父对象移动
    /// </summary>
    private void ResumeParentMovement()
    {
        if (parentRb != null && !parentCanMove)
        {
            // 恢复父对象的移动速度
            parentRb.linearVelocity = parentOriginalVelocity;
            parentCanMove = true;
            Debug.Log("玩家离开left_0区域，父对象恢复移动");
        }
    }
}