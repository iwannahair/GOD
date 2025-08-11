using UnityEngine;

/// <summary>
/// 资源管理器 - 单例模式管理玩家资源
/// 处理资源的添加、消耗、查询等操作
/// </summary>
public class ResourceManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static ResourceManager Instance { get; private set; }
    
    [Header("初始资源")]
    [Tooltip("初始金币数量")]
    [SerializeField] private int initialGold = 0;
    
    [Tooltip("初始木材数量")]
    [SerializeField] private int initialWood = 0;
    
    [Tooltip("初始石头数量")]
    [SerializeField] private int initialStone = 0;
    
    [Tooltip("初始水晶数量")]
    [SerializeField] private int initialCrystal = 0;
    
    [Tooltip("初始能量数量")]
    [SerializeField] private int initialEnergy = 0;
    
    // 资源数量字典
    private System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int> resources;
    
    /// <summary>
    /// 资源变化事件
    /// </summary>
    public System.Action<ResourceDrop.ResourceType, int> OnResourceChanged;
    
    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化资源
    /// </summary>
    private void InitializeResources()
    {
        resources = new System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int>();
        
        // 设置初始资源
        resources[ResourceDrop.ResourceType.Gold] = initialGold;
        resources[ResourceDrop.ResourceType.Wood] = initialWood;
        resources[ResourceDrop.ResourceType.Stone] = initialStone;
        resources[ResourceDrop.ResourceType.Crystal] = initialCrystal;
        resources[ResourceDrop.ResourceType.Energy] = initialEnergy;
    }
    
    /// <summary>
    /// 添加资源
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="amount">数量</param>
    public void AddResource(ResourceDrop.ResourceType type, int amount)
    {
        if (amount <= 0) return;
        
        if (resources.ContainsKey(type))
        {
            resources[type] += amount;
        }
        else
        {
            resources[type] = amount;
        }
        
        OnResourceChanged?.Invoke(type, resources[type]);
        Debug.Log($"获得 {amount} 个 {type}，当前总数: {resources[type]}");
    }
    
    /// <summary>
    /// 消耗资源
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="amount">数量</param>
    /// <returns>是否消耗成功</returns>
    public bool ConsumeResource(ResourceDrop.ResourceType type, int amount)
    {
        if (amount <= 0) return false;
        
        if (resources.ContainsKey(type) && resources[type] >= amount)
        {
            resources[type] -= amount;
            OnResourceChanged?.Invoke(type, resources[type]);
            Debug.Log($"消耗 {amount} 个 {type}，剩余: {resources[type]}");
            return true;
        }
        
        Debug.LogWarning($"资源不足！需要 {amount} 个 {type}，但当前只有 {GetResourceCount(type)}");
        return false;
    }
    
    /// <summary>
    /// 获取资源数量
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <returns>资源数量</returns>
    public int GetResourceCount(ResourceDrop.ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : 0;
    }
    
    /// <summary>
    /// 检查是否有足够资源
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="amount">需要的数量</param>
    /// <returns>是否足够</returns>
    public bool HasEnoughResource(ResourceDrop.ResourceType type, int amount)
    {
        return resources.ContainsKey(type) && resources[type] >= amount;
    }
    
    /// <summary>
    /// 获取所有资源数据（用于保存/加载）
    /// </summary>
    public System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int> GetAllResources()
    {
        return new System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int>(resources);
    }
    
    /// <summary>
    /// 设置所有资源数据（用于保存/加载）
    /// </summary>
    public void SetAllResources(System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int> newResources)
    {
        resources = new System.Collections.Generic.Dictionary<ResourceDrop.ResourceType, int>(newResources);
        
        // 通知所有资源已更新
        foreach (var resource in resources)
        {
            OnResourceChanged?.Invoke(resource.Key, resource.Value);
        }
    }
}