using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapUI : MonoBehaviour
{
    [Header("小地图设置")]
    [SerializeField] private RectTransform minimapContainer; // 小地图容器
    [SerializeField] private Image minimapBackground; // 小地图背景
    [SerializeField] private GameObject playerIconPrefab; // 玩家图标预制体
    [SerializeField] private GameObject spawnerIconPrefab; // 生成点图标预制体
    [SerializeField] private GameObject goldenTreeIconPrefab; // 黄金树图标预制体
    
    [Header("地图范围设置")]
    [SerializeField] private float viewRadius = 50f; // 小地图显示半径（以玩家为中心）
    [SerializeField] private Vector2 minimapSize = new Vector2(200, 200); // 小地图UI尺寸
    
    [Header("更新设置")]
    [SerializeField] private float updateInterval = 0.1f; // 更新间隔（秒）
    [SerializeField] private bool enablePlayerCenter = true; // 启用以玩家为中心
    
    private Transform playerTransform; // 玩家Transform引用
    private GameObject playerIcon; // 玩家图标实例
    private List<GameObject> spawnerIcons = new List<GameObject>(); // 生成点图标列表
    private GameObject goldenTreeIcon; // 黄金树图标实例
    private Transform goldenTreeTransform; // 黄金树Transform引用
    private CreateHumanManager createHumanManager; // 人类生成管理器引用
    
    private float lastUpdateTime = 0f;
    
    // 用于调试的变量
    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = false;
    
    private void Start()
    {
        InitializeMinimap();
        FindPlayerAndSpawners();
        FindGoldenTree();
        CreateIcons();
    }
    
    private void Update()
    {
        // 实时更新小地图内容
        if (Time.time - lastUpdateTime > updateInterval)
        {
            UpdatePlayerIcon();
            UpdateSpawnerIcons();
            UpdateGoldenTreeIcon();
            lastUpdateTime = Time.time;
        }
    }
    
    /// <summary>
    /// 初始化小地图UI
    /// </summary>
    private void InitializeMinimap()
    {
        // 设置小地图容器大小和位置（右上角）
        if (minimapContainer != null)
        {
            minimapContainer.sizeDelta = minimapSize;
            minimapContainer.anchorMin = new Vector2(1, 1);
            minimapContainer.anchorMax = new Vector2(1, 1);
            minimapContainer.anchoredPosition = new Vector2(-minimapSize.x/2 - 20, -minimapSize.y/2 - 20);
        }
    }
    
    /// <summary>
    /// 查找玩家和生成点管理器
    /// </summary>
    private void FindPlayerAndSpawners()
    {
        // 查找玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // 查找CreateHumanManager
        createHumanManager = FindObjectOfType<CreateHumanManager>();
    }
    
    /// <summary>
    /// 查找黄金树
    /// </summary>
    private void FindGoldenTree()
    {
        // 查找黄金树
        GameObject goldenTreeObject = GameObject.FindWithTag("GoldenTree");
        if (goldenTreeObject != null)
        {
            goldenTreeTransform = goldenTreeObject.transform;
        }
        else
        {
            // 如果通过标签找不到，尝试通过组件查找
            GoldenTree goldenTree = FindObjectOfType<GoldenTree>();
            if (goldenTree != null)
            {
                goldenTreeTransform = goldenTree.transform;
            }
        }
    }
    
    /// <summary>
    /// 创建图标
    /// </summary>
    private void CreateIcons()
    {
        // 创建玩家图标
        if (playerIconPrefab != null && playerTransform != null)
        {
            playerIcon = Instantiate(playerIconPrefab, minimapContainer);
        }
        
        // 创建黄金树图标
        CreateGoldenTreeIcon();
        
        CreateSpawnerIcons();
    }
    
    /// <summary>
    /// 创建黄金树图标
    /// </summary>
    private void CreateGoldenTreeIcon()
    {
        if (goldenTreeIconPrefab != null && goldenTreeTransform != null)
        {
            goldenTreeIcon = Instantiate(goldenTreeIconPrefab, minimapContainer);
            
            // 设置黄金树图标的初始位置
            UpdateGoldenTreeIcon();
        }
    }
    
    /// <summary>
    /// 创建生成点图标
    /// </summary>
    private void CreateSpawnerIcons()
    {
        if (spawnerIconPrefab == null) return;
        
        // 查找所有CreateHumanSpawner
        CreateHumanSpawner[] spawners = FindObjectsOfType<CreateHumanSpawner>();
        
        foreach (CreateHumanSpawner spawner in spawners)
        {
            GameObject icon = Instantiate(spawnerIconPrefab, minimapContainer);
            spawnerIcons.Add(icon);
            
            // 优先使用CreateHuman的实际位置，如果没有则使用Spawner位置
            Vector3 actualPosition = spawner.transform.position;
            Transform createHumanTransform = spawner.GetCreateHumanTransform();
            if (createHumanTransform != null)
            {
                actualPosition = createHumanTransform.position;
            }
            
            // 设置生成点图标位置
            Vector2 minimapPos = WorldToMinimapPosition(actualPosition);
            icon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
        }
    }
    
    /// <summary>
    /// 更新玩家图标位置（始终在小地图中心）
    /// </summary>
    private void UpdatePlayerIcon()
    {
        if (playerIcon != null && playerTransform != null)
        {
            // 玩家图标始终在小地图中心
            playerIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 更新生成点图标
    /// </summary>
    private void UpdateSpawnerIcons()
    {
        // 检查是否有新的生成点需要添加
        CreateHumanSpawner[] currentSpawners = FindObjectsOfType<CreateHumanSpawner>();
        
        // 如果生成点数量发生变化，重新创建图标
        if (currentSpawners.Length != spawnerIcons.Count)
        {
            // 清除旧图标
            foreach (GameObject icon in spawnerIcons)
            {
                if (icon != null) Destroy(icon);
            }
            spawnerIcons.Clear();
            
            // 重新创建图标
            CreateSpawnerIcons();
        }
        else
        {
            // 更新现有图标位置
            for (int i = 0; i < currentSpawners.Length && i < spawnerIcons.Count; i++)
            {
                if (currentSpawners[i] != null && spawnerIcons[i] != null)
                {
                    // 优先使用CreateHuman的实际位置
                    Vector3 actualPosition = currentSpawners[i].transform.position;
                    Transform createHumanTransform = currentSpawners[i].GetCreateHumanTransform();
                    if (createHumanTransform != null)
                    {
                        actualPosition = createHumanTransform.position;
                    }
                    
                    Vector2 minimapPos = WorldToMinimapPosition(actualPosition);
                    
                    // 只显示在小地图范围内的生成点
                    RectTransform iconRect = spawnerIcons[i].GetComponent<RectTransform>();
                    if (IsPositionInMinimap(minimapPos))
                    {
                        iconRect.anchoredPosition = minimapPos;
                        spawnerIcons[i].SetActive(true);
                    }
                    else
                    {
                        spawnerIcons[i].SetActive(false);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 将世界坐标转换为小地图坐标（以玩家为中心）
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>小地图坐标</returns>
    private Vector2 WorldToMinimapPosition(Vector3 worldPosition)
    {
        if (playerTransform == null) return Vector2.zero;
        
        // 计算相对于玩家的位置
        Vector2 relativePosition = new Vector2(
            worldPosition.x - playerTransform.position.x,
            worldPosition.y - playerTransform.position.y
        );
        
        // 转换为小地图坐标
        Vector2 minimapPosition = new Vector2(
            (relativePosition.x / viewRadius) * (minimapSize.x * 0.5f),
            (relativePosition.y / viewRadius) * (minimapSize.y * 0.5f)
        );
        
        if (showDebugInfo)
        {
            Debug.Log($"世界坐标: {worldPosition}, 相对位置: {relativePosition}, 小地图坐标: {minimapPosition}");
        }
        
        return minimapPosition;
    }
    
    /// <summary>
    /// 检查位置是否在小地图显示范围内
    /// </summary>
    /// <param name="minimapPos">小地图坐标</param>
    /// <returns>是否在范围内</returns>
    private bool IsPositionInMinimap(Vector2 minimapPos)
    {
        float halfWidth = minimapSize.x * 0.5f;
        float halfHeight = minimapSize.y * 0.5f;
        
        return Mathf.Abs(minimapPos.x) <= halfWidth && Mathf.Abs(minimapPos.y) <= halfHeight;
    }
    
    /// <summary>
    /// 更新黄金树图标位置
    /// </summary>
    private void UpdateGoldenTreeIcon()
    {
        if (goldenTreeIcon != null && goldenTreeTransform != null && playerTransform != null)
        {
            Vector2 minimapPos = WorldToMinimapPosition(goldenTreeTransform.position);
            
            // 检查黄金树是否在小地图显示范围内
            if (IsPositionInMinimap(minimapPos))
            {
                // 在范围内，显示实际位置
                goldenTreeIcon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
                goldenTreeIcon.SetActive(true);
            }
            else
            {
                // 超出范围，显示在边界上
                Vector2 clampedPos = ClampPositionToBoundary(minimapPos);
                goldenTreeIcon.GetComponent<RectTransform>().anchoredPosition = clampedPos;
                goldenTreeIcon.SetActive(true);
                
                if (showDebugInfo)
                {
                    Debug.Log($"黄金树超出范围，显示在边界: {clampedPos}");
                }
            }
        }
    }
    
    /// <summary>
    /// 将超出范围的位置限制到小地图边界
    /// </summary>
    /// <param name="position">原始位置</param>
    /// <returns>限制后的位置</returns>
    private Vector2 ClampPositionToBoundary(Vector2 position)
    {
        float halfWidth = minimapSize.x * 0.5f;
        float halfHeight = minimapSize.y * 0.5f;
        
        // 计算从中心到目标位置的方向
        Vector2 direction = position.normalized;
        
        // 计算与边界的交点
        float scaleX = halfWidth / Mathf.Abs(direction.x);
        float scaleY = halfHeight / Mathf.Abs(direction.y);
        
        // 选择较小的缩放比例，确保点在边界上
        float scale = Mathf.Min(scaleX, scaleY);
        
        // 稍微向内偏移，确保图标完全在边界内
        scale *= 0.9f;
        
        return direction * scale;
    }
    
    /// <summary>
    /// 设置小地图显示半径
    /// </summary>
    /// <param name="radius">显示半径</param>
    public void SetViewRadius(float radius)
    {
        viewRadius = radius;
    }
    
    /// <summary>
    /// 获取当前显示半径
    /// </summary>
    /// <returns>显示半径</returns>
    public float GetViewRadius()
    {
        return viewRadius;
    }
    
    /// <summary>
    /// 设置地图范围（向后兼容方法）
    /// </summary>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    public void SetMapBounds(float width, float height)
    {
        // 在新的以玩家为中心的系统中，可以将这些值转换为显示半径
        float radius = Mathf.Max(width, height) * 0.5f;
        SetViewRadius(radius);
    }
}