using UnityEngine;

/// <summary>
/// 资源掉落控制器 - 管理敌人死亡后掉落的资源
/// 处理资源拾取、自动收集、视觉效果等
/// </summary>
public class ResourceDrop : MonoBehaviour
{
    [Header("资源设置")]
    [Tooltip("资源价值")]
    [SerializeField] private int resourceValue = 1;
    
    [Tooltip("资源类型")]
    [SerializeField] private ResourceType resourceType = ResourceType.Gold;
    
    [Header("收集设置")]
    [Tooltip("自动收集范围")]
    [SerializeField] private float autoCollectRange = 2f;
    
    [Tooltip("自动收集速度")]
    [SerializeField] private float collectSpeed = 5f;
    
    [Tooltip("资源标签")]
    [SerializeField] private string playerTag = "Player";
    
    [Header("视觉效果")]
    [Tooltip("旋转速度")]
    [SerializeField] private float rotationSpeed = 180f;
    
    [Tooltip("浮动高度")]
    [SerializeField] private float floatHeight = 0.5f;
    
    [Tooltip("浮动速度")]
    [SerializeField] private float floatSpeed = 2f;
    
    private Transform playerTransform;
    private bool isCollected = false;
    private Vector3 startPosition;
    private float floatOffset;
    
    /// <summary>
    /// 资源类型枚举
    /// </summary>
    public enum ResourceType
    {
        Gold,
        Wood,
        Stone,
        Crystal,
        Energy
    }
    
    void Start()
    {
        startPosition = transform.position;
        floatOffset = Random.Range(0f, 2f * Mathf.PI); // 随机浮动偏移
        
        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void Update()
    {
        if (isCollected)
            return;
            
        // 浮动动画
        FloatingAnimation();
        
        // 旋转动画
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        
        // 检查玩家距离，自动收集
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= autoCollectRange)
            {
                MoveToPlayer();
            }
        }
    }
    
    /// <summary>
    /// 浮动动画效果
    /// </summary>
    private void FloatingAnimation()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatHeight;
        transform.position = startPosition + Vector3.up * yOffset;
    }
    
    /// <summary>
    /// 向玩家移动（自动收集）
    /// </summary>
    private void MoveToPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * collectSpeed * Time.deltaTime;
        
        // 如果足够接近玩家，收集资源
        if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            CollectResource();
        }
    }
    
    /// <summary>
    /// 触发器检测（手动收集）
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;
            
        if (other.CompareTag(playerTag))
        {
            CollectResource();
        }
    }
    
    /// <summary>
    /// 收集资源
    /// </summary>
    private void CollectResource()
    {
        if (isCollected)
            return;
            
        isCollected = true;
        
        // 添加到玩家资源
        ResourceManager.Instance.AddResource(resourceType, resourceValue);
        
        // 播放收集特效
        PlayCollectEffect();
        
        // 销毁资源对象
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 播放收集特效
    /// </summary>
    private void PlayCollectEffect()
    {
        // 这里可以添加收集特效，比如粒子效果、音效等
        Debug.Log($"收集了 {resourceValue} 个 {resourceType}");
    }
    
    /// <summary>
    /// 设置资源数据
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="value">资源价值</param>
    public void SetResourceData(ResourceType type, int value)
    {
        resourceType = type;
        resourceValue = value;
    }
}