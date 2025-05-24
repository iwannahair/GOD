using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // 添加UI命名空间

/// <summary>
/// 人类跟随者类：控制人类跟随玩家的行为
/// 该类实现了队列式跟随系统，使多个人类能够有序地跟随玩家
/// </summary>
public class HumanFollower : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField] private float maxHealth = 100f; // 最大生命值
    [SerializeField] private float currentHealth; // 当前生命值
    [SerializeField] private Slider healthBar; // 血量滑动条
    [SerializeField] private Canvas healthCanvas; // 血量UI的Canvas
    
    private bool isFollowing = false; // 是否正在跟随玩家，由碰撞触发设置
    private Transform playerTransform; // 玩家的Transform，用于获取位置和计算跟随位置
    [SerializeField] private float followSpeed = 3f; // 跟随速度，控制移动的快慢
    [SerializeField] private float followDistance = 0.5f; // 跟随距离，保持0.5像素，控制与前一个对象的间距
    
    private Vector3 previousPlayerPosition; // 记录玩家上一帧的位置，用于计算速度
    private Vector3 playerVelocity; // 玩家的速度向量，用于确定移动方向
    private float playerSpeed; // 玩家的移动速度大小，用于判断是否在移动
    private Vector3 lastPlayerDirection = Vector3.down; // 默认朝向，初始为向下，保存最后有效的移动方向
    private bool isInvincible = false; // 是否处于无敌状态
    
    // 队列跟随相关变量
    private HumanFollower nextInQueue = null; // 队列中的下一个对象，指向后面跟随的人类
    private HumanFollower previousInQueue = null; // 队列中的前一个对象，指向前面的人类或玩家
    private static HumanFollower lastInQueue = null; // 队列中的最后一个对象，静态变量在所有实例间共享
    
    /// <summary>
    /// 初始化方法：确保有碰撞器用于检测玩家
    /// </summary>
    private void Start()
    {
        // 添加碰撞器，如果没有的话，确保能检测到玩家
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
        }
        
        // 初始化生命值
        currentHealth = maxHealth;
        
        // 如果没有指定血量条，尝试在子对象中查找
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<Slider>();
        }
        
        // 初始化血量UI
        UpdateHealthBar();
    }
    
    /// <summary>
    /// 每帧更新：处理跟随逻辑
    /// </summary>
    private void Update()
    {
        // 如果正在跟随玩家且玩家引用有效
        if (isFollowing && playerTransform != null)
        {
            // 计算玩家的速度和速度大小，用于确定移动方向
            if (Time.deltaTime > 0)
            {
                playerVelocity = (playerTransform.position - previousPlayerPosition) / Time.deltaTime;
                playerSpeed = playerVelocity.magnitude;
            }
            
            // 获取玩家的朝向（基于移动方向）
            Vector3 playerDirection;
            if (playerSpeed > 0.01f) // 如果玩家在移动，使用当前移动方向
            {
                playerDirection = playerVelocity.normalized;
                lastPlayerDirection = playerDirection; // 记录最后的有效朝向
            }
            else // 如果玩家几乎不动，使用上一次记录的朝向
            {
                playerDirection = lastPlayerDirection;
            }
            
            Vector3 targetPosition;
            
            if (previousInQueue != null)
            {
                // 如果有前一个对象，跟随前一个对象，形成队列
                targetPosition = previousInQueue.transform.position - playerDirection * followDistance;
            }
            else
            {
                // 如果是队列中的第一个，直接跟随玩家
                // 始终在玩家朝向的反方向上跟随，保持一定距离
                targetPosition = playerTransform.position - playerDirection * followDistance;
            }
            
            // 平滑移动到目标位置，使用MoveTowards实现平滑跟随
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            // 更新玩家上一帧位置，用于下一帧计算速度
            previousPlayerPosition = playerTransform.position;
        }
    }
    
    /// <summary>
    /// 碰撞检测：当玩家碰到人类时触发跟随行为
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的是否是玩家
        if (other.CompareTag("Player") && !isFollowing)
        {
            // 获取玩家的Transform
            playerTransform = other.transform;
            // 设置为跟随状态
            isFollowing = true;
            // 记录玩家当前位置，用于计算速度
            previousPlayerPosition = playerTransform.position;
            
            Debug.Log("人类与玩家碰撞，开始跟随");
            
            // 如果队列中没有最后一个，说明这是第一个跟随的人类
            if (lastInQueue == null)
            {
                // 设置为队列中的第一个，直接跟随玩家
                previousInQueue = null;
            }
            else
            {
                // 将自己添加到队列末尾
                HumanFollower previousLast = lastInQueue;
                previousLast.nextInQueue = this;
                previousInQueue = previousLast;
                nextInQueue = null;
            }

            lastInQueue = this;
            
            // 检查是否有活跃的斧子效果，如果有，为这个新跟随的人类也添加斧子
            GoldenTree goldenTree = FindObjectOfType<GoldenTree>();
            if (goldenTree != null && goldenTree.AxesRotating)
            {
                goldenTree.AddAxeToHuman(this);
                Debug.Log("新跟随的人类继承了斧子旋转效果");
            }
        }
    }
    
    /// <summary>
    /// 销毁时调用：更新队列引用关系
    /// </summary>
    private void OnDestroy()
    {
        // 如果此对象在队列中，更新队列引用关系，确保队列完整性
        if (isFollowing)
        {
            if (previousInQueue != null)
            {
                previousInQueue.nextInQueue = nextInQueue;
            }
            
            if (nextInQueue != null)
            {
                nextInQueue.previousInQueue = previousInQueue;
            }
            
            if (lastInQueue == this)
            {
                lastInQueue = previousInQueue;
            }
        }
    }
    
    /// <summary>
    /// 设置人类的无敌状态
    /// </summary>
    /// <param name="invincible">是否无敌</param>
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
        Debug.Log($"人类{gameObject.name}设置无敌状态: {invincible}");
    }

    /// <summary>
    /// 更新血量滑动条
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // 如果处于无敌状态，不受伤害
        
        currentHealth -= damage;
        Debug.Log($"人类受到{damage}点伤害，剩余生命值：{currentHealth}");
        
        // 更新血量UI
        UpdateHealthBar();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 死亡处理
    /// </summary>
    private void Die()
    {
        Debug.Log("人类死亡");
        
        // 查找StatsForGod组件并减少生命值
        StatsForGod statsForGod = FindObjectOfType<StatsForGod>();
        if (statsForGod != null)
        {
            // 减少神的生命值百分比
            statsForGod.AddHealthPercent(-1f);
            Debug.Log("人类死亡，神的生命值减少1%");
        }
        else
        {
            Debug.LogWarning("未找到StatsForGod组件，无法减少神的生命值");
        }
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 检查人类是否正在跟随玩家
    /// </summary>
    /// <returns>是否正在跟随</returns>
    public bool IsFollowing()
    {
        return isFollowing && playerTransform != null;
    }
}