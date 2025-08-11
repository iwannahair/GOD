using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 收集进度管理器 - 管理玩家收集敌人掉落物的进度
/// 当进度满时弹出选择按钮面板
/// </summary>
public class CollectionProgressManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static CollectionProgressManager Instance { get; private set; }
    
    [Header("进度设置")]
    [Tooltip("达到满进度所需的收集数量")]
    [SerializeField] private int maxCollectionCount = 10;
    
    [Tooltip("当前收集进度")]
    [SerializeField] private int currentCollectionCount = 0;
    
    [Header("UI组件")]
    [Tooltip("进度条UI组件")]
    [SerializeField] private Slider progressSlider;
    
    [Tooltip("进度条文本显示")]
    [SerializeField] private Text progressText;
    
    [Tooltip("选择按钮面板")]
    [SerializeField] private GameObject buttonPanel;
    
    [Tooltip("按钮1")]
    [SerializeField] private Button button1;
    
    [Tooltip("按钮2")]
    [SerializeField] private Button button2;
    
    [Tooltip("按钮3")]
    [SerializeField] private Button button3;
    
    [Header("按钮设置")]
    [Tooltip("按钮1文本")]
    [SerializeField] private string button1Text = "增加攻击力";
    
    [Tooltip("按钮2文本")]
    [SerializeField] private string button2Text = "增加移动速度";
    
    [Tooltip("按钮3文本")]
    [SerializeField] private string button3Text = "恢复生命值";
    
    [Header("预制体设置")]
    [Tooltip("PrefabManager引用")]
    [SerializeField] private PrefabManager prefabManager;
    
    [Tooltip("按钮1对应的预制体")]
    [SerializeField] private GameObject button1Prefab;
    
    [Tooltip("按钮2对应的预制体")]
    [SerializeField] private GameObject button2Prefab;
    
    [Tooltip("按钮3对应的预制体")]
    [SerializeField] private GameObject button3Prefab;
    
    /// <summary>
    /// 进度变化事件
    /// </summary>
    public System.Action<int, int> OnProgressChanged;
    
    /// <summary>
    /// 进度满事件
    /// </summary>
    public System.Action OnProgressComplete;
    
    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 订阅资源管理器的事件
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += OnResourceCollected;
            Debug.Log("已成功订阅ResourceManager.OnResourceChanged事件");
        }
        else
        {
            Debug.LogError("ResourceManager.Instance为null，无法订阅事件！");
        }
        
        // 调试信息
        Debug.Log("CollectionProgressManager启动，progressText是否为null: " + (progressText == null));
        Debug.Log("progressSlider是否为null: " + (progressSlider == null));
        
        // 设置按钮点击事件
        SetupButtonEvents();
    }
    
    void OnDestroy()
    {
        // 取消订阅事件
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= OnResourceCollected;
        }
    }
    
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 初始化进度条
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxCollectionCount;
            progressSlider.value = currentCollectionCount;
        }
        
        // 更新进度文本
        UpdateProgressText();
        
        // 隐藏按钮面板
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(false);
        }
        
        // 设置按钮文本
        SetButtonTexts();
    }
    
    /// <summary>
    /// 设置按钮文本
    /// </summary>
    private void SetButtonTexts()
    {
        if (button1 != null && button1.GetComponentInChildren<Text>() != null)
        {
            button1.GetComponentInChildren<Text>().text = button1Text;
        }
        
        if (button2 != null && button2.GetComponentInChildren<Text>() != null)
        {
            button2.GetComponentInChildren<Text>().text = button2Text;
        }
        
        if (button3 != null && button3.GetComponentInChildren<Text>() != null)
        {
            button3.GetComponentInChildren<Text>().text = button3Text;
        }
    }
    
    /// <summary>
    /// 设置按钮点击事件
    /// </summary>
    private void SetupButtonEvents()
    {
        if (button1 != null)
        {
            button1.onClick.AddListener(() => OnButtonClicked(1));
        }
        
        if (button2 != null)
        {
            button2.onClick.AddListener(() => OnButtonClicked(2));
        }
        
        if (button3 != null)
        {
            button3.onClick.AddListener(() => OnButtonClicked(3));
        }
    }
    
    /// <summary>
    /// 当资源被收集时调用
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="newAmount">新的资源数量</param>
    private void OnResourceCollected(ResourceDrop.ResourceType resourceType, int newAmount)
    {
        Debug.Log($"收集到资源：{resourceType}，数量：{newAmount}");
        
        // 每次收集任何类型的资源都增加进度
        AddProgress(1);
    }
    
    /// <summary>
    /// 增加收集进度
    /// </summary>
    /// <param name="amount">增加的数量</param>
    public void AddProgress(int amount)
    {
        Debug.Log($"尝试增加进度：{amount}，当前进度：{currentCollectionCount}/{maxCollectionCount}");
        
        if (amount <= 0)
        {
            Debug.LogWarning("增加的进度数量小于等于0，忽略此次更新");
            return;
        }
        
        currentCollectionCount += amount;
        Debug.Log($"进度增加后：{currentCollectionCount}");
        
        // 限制最大值
        if (currentCollectionCount > maxCollectionCount)
        {
            currentCollectionCount = maxCollectionCount;
            Debug.Log($"进度超过最大值，已限制为：{maxCollectionCount}");
        }
        
        // 更新UI
        Debug.Log("开始更新UI...");
        UpdateProgressUI();
        
        // 触发进度变化事件
        Debug.Log("触发OnProgressChanged事件");
        OnProgressChanged?.Invoke(currentCollectionCount, maxCollectionCount);
        
        // 检查是否达到满进度
        if (currentCollectionCount >= maxCollectionCount)
        {
            Debug.Log("进度已满，调用OnProgressReachedMax");
            OnProgressReachedMax();
        }
        
        Debug.Log($"收集进度更新完成: {currentCollectionCount}/{maxCollectionCount}");
    }
    
    /// <summary>
    /// 更新进度UI
    /// </summary>
    private void UpdateProgressUI()
    {
        // 更新进度条
        if (progressSlider != null)
        {
            float oldValue = progressSlider.value;
            progressSlider.value = currentCollectionCount;
            Debug.Log($"进度条值从 {oldValue} 更新为 {progressSlider.value}，最大值：{progressSlider.maxValue}");
        }
        else
        {
            Debug.LogError("progressSlider为null，无法更新进度条！");
        }
        
        // 更新进度文本
        UpdateProgressText();
    }
    
    /// <summary>
    /// 更新进度文本
    /// </summary>
    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = $"{currentCollectionCount}/{maxCollectionCount}";
            Debug.Log($"更新进度文本：{currentCollectionCount}/{maxCollectionCount}");
        }
        else
        {
            // 如果progressText为null，仍然记录进度变化
            Debug.Log($"进度已更新（但progressText为null）：{currentCollectionCount}/{maxCollectionCount}");
        }
    }
    
    /// <summary>
    /// 当进度达到最大值时调用
    /// </summary>
    private void OnProgressReachedMax()
    {
        Debug.Log("收集进度已满！弹出选择面板");
        
        // 触发进度完成事件
        OnProgressComplete?.Invoke();
        
        // 显示按钮面板
        ShowButtonPanel();
    }
    
    /// <summary>
    /// 显示按钮面板
    /// </summary>
    private void ShowButtonPanel()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
            
            // 暂停游戏时间（可选）
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// 隐藏按钮面板
    /// </summary>
    private void HideButtonPanel()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(false);
            
            // 恢复游戏时间
            Time.timeScale = 1f;
        }
    }
    
    /// <summary>
    /// 按钮点击处理
    /// </summary>
    /// <param name="buttonIndex">按钮索引（1-3）</param>
    private void OnButtonClicked(int buttonIndex)
    {
        Debug.Log($"玩家选择了按钮 {buttonIndex}");
        
        // 根据按钮执行不同的效果
        switch (buttonIndex)
        {
            case 1:
                ApplyButton1Effect();
                break;
            case 2:
                ApplyButton2Effect();
                break;
            case 3:
                ApplyButton3Effect();
                break;
        }
        
        // 重置进度
        ResetProgress();
        
        // 隐藏按钮面板
        HideButtonPanel();
    }
    
    /// <summary>
    /// 应用按钮1效果（增加攻击力）
    /// </summary>
    private void ApplyButton1Effect()
    {
        Debug.Log("应用效果：增加攻击力");
        
        // 将对应的预制体添加到PrefabManager的列表中并立即实例化
        if (prefabManager != null && button1Prefab != null)
        {
            prefabManager.AddPrefabAndSpawn(button1Prefab);
            Debug.Log($"通过按钮1选择，已将预制体 {button1Prefab.name} 添加到PrefabManager并实例化");
        }
        else
        {
            Debug.LogWarning("PrefabManager或按钮1预制体未设置！");
        }
        
        // 这里可以添加具体的攻击力增加逻辑
        // 例如：PlayerStats.Instance.IncreaseAttackPower(10);
    }
    
    /// <summary>
    /// 应用按钮2效果（增加移动速度）
    /// </summary>
    private void ApplyButton2Effect()
    {
        Debug.Log("应用效果：增加移动速度");
        
        // 将对应的预制体添加到PrefabManager的列表中并立即实例化
        if (prefabManager != null && button2Prefab != null)
        {
            prefabManager.AddPrefabAndSpawn(button2Prefab);
            Debug.Log($"通过按钮2选择，已将预制体 {button2Prefab.name} 添加到PrefabManager并实例化");
        }
        else
        {
            Debug.LogWarning("PrefabManager或按钮2预制体未设置！");
        }
        
        // 这里可以添加具体的移动速度增加逻辑
        // 例如：PlayerStats.Instance.IncreaseMovementSpeed(1.5f);
    }
    
    /// <summary>
    /// 应用按钮3效果（恢复生命值）
    /// </summary>
    private void ApplyButton3Effect()
    {
        Debug.Log("应用效果：恢复生命值");
        
        // 将对应的预制体添加到PrefabManager的列表中并立即实例化
        if (prefabManager != null && button3Prefab != null)
        {
            prefabManager.AddPrefabAndSpawn(button3Prefab);
            Debug.Log($"通过按钮3选择，已将预制体 {button3Prefab.name} 添加到PrefabManager并实例化");
        }
        else
        {
            Debug.LogWarning("PrefabManager或按钮3预制体未设置！");
        }
        
        // 这里可以添加具体的生命值恢复逻辑
        // 例如：PlayerHealth.Instance.RestoreHealth(50);
    }
    
    /// <summary>
    /// 重置收集进度
    /// </summary>
    public void ResetProgress()
    {
        currentCollectionCount = 0;
        UpdateProgressUI();
        Debug.Log("收集进度已重置");
    }
    
    /// <summary>
    /// 设置最大收集数量
    /// </summary>
    /// <param name="newMaxCount">新的最大收集数量</param>
    public void SetMaxCollectionCount(int newMaxCount)
    {
        if (newMaxCount > 0)
        {
            maxCollectionCount = newMaxCount;
            
            // 更新进度条最大值
            if (progressSlider != null)
            {
                progressSlider.maxValue = maxCollectionCount;
            }
            
            // 更新UI
            UpdateProgressUI();
        }
    }
    
    /// <summary>
    /// 获取当前进度百分比
    /// </summary>
    /// <returns>进度百分比（0-1）</returns>
    public float GetProgressPercentage()
    {
        return (float)currentCollectionCount / maxCollectionCount;
    }
}