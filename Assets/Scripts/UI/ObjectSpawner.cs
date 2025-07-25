using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject prefabToSpawn; // 要生成的预制体
    public int spawnCount = 10; // 生成数量
    public float spawnRadius = 10f; // 生成半径
    public float minDistance = 2f; // 物体间最小距离
    
    [Header("生成区域设置")]
    public bool useRectangleArea = false; // 是否使用矩形区域
    public Vector2 rectangleSize = new Vector2(20f, 20f); // 矩形区域大小
    
    [Header("生成选项")]
    public bool spawnOnStart = true; // 开始时自动生成
    public bool showGizmos = true; // 显示生成区域
    public LayerMask obstacleLayer = -1; // 障碍物图层
    
    [Header("调试信息")]
    public int maxAttempts = 100; // 最大尝试次数
    
    [Header("Sprite触发设置")]
    public Sprite triggerSprite; // 当玩家接近时显示的Sprite
    public float detectionRadius = 3f; // 检测半径
    public Vector3 spriteOffset = new Vector3(0, 1, 0); // Sprite相对于prefab的偏移
    public Vector3 spriteScale = Vector3.one; // Sprite缩放
    public Color spriteColor = Color.white; // Sprite颜色
    public string sortingLayerName = "Default"; // 排序层名称
    public int sortingOrder = 10; // 排序顺序
    
    [Header("玩家检测设置")]
    public string playerTag = "Player"; // 玩家标签
    public bool showDetectionRange = true; // 显示检测范围
    public bool showRuntimeDebug = true; // 运行时调试显示
    
    [Header("UI交互设置")]
    public KeyCode interactionKey = KeyCode.F; // 交互按键
    public GameObject interactionPanel; // 交互面板预制体
    public string button1Text = "选项1"; // 按钮1文本
    public string button2Text = "选项2"; // 按钮2文本
    public string button3Text = "选项3"; // 按钮3文本
    
    private List<Vector2> spawnedPositions = new List<Vector2>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<bool> objectsPlayerInRange = new List<bool>(); // 记录每个对象的玩家检测状态
    private List<GameObject> spawnedSprites = new List<GameObject>(); // 记录每个对象生成的Sprite
    private GameObject player; // 玩家对象引用
    private GameObject currentPanel; // 当前显示的面板实例
    private bool isPanelOpen = false; // 面板是否打开
    private int currentInteractionIndex = -1; // 当前可交互的对象索引
    
    void Start()
    {
        // 查找玩家对象
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj;
            Debug.Log($"[ObjectSpawner] 找到玩家对象: {player.name}");
        }
        else
        {
            Debug.LogWarning($"[ObjectSpawner] 未找到标签为 '{playerTag}' 的玩家对象");
        }
        
        // 确保交互面板在开始时是隐藏的
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
        
        // 确保游戏开始时时间流速正常
        Time.timeScale = 1f;
        
        if (spawnOnStart)
        {
            SpawnObjects();
        }
    }
    
    void OnDestroy()
    {
        // 确保在脚本销毁时恢复正常时间流速
        Time.timeScale = 1f;
    }
    
    void OnApplicationQuit()
    {
        // 确保在应用退出时恢复正常时间流速
        Time.timeScale = 1f;
    }
    
    void Update()
    {
        if (player == null) return;
        
        // 检测每个生成对象与玩家的距离
        currentInteractionIndex = -1;
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null) continue;
            
            float distance = Vector2.Distance(player.transform.position, spawnedObjects[i].transform.position);
            bool playerInRange = distance <= detectionRadius;
            
            // 如果状态发生变化
            if (i < objectsPlayerInRange.Count && objectsPlayerInRange[i] != playerInRange)
            {
                objectsPlayerInRange[i] = playerInRange;
                
                if (playerInRange)
                {
                    ShowSprite(i);
                    currentInteractionIndex = i; // 设置当前可交互对象
                }
                else
                {
                    HideSprite(i);
                }
            }
            else if (i >= objectsPlayerInRange.Count)
            {
                // 初始化状态
                objectsPlayerInRange.Add(playerInRange);
                if (playerInRange)
                {
                    ShowSprite(i);
                    currentInteractionIndex = i; // 设置当前可交互对象
                }
            }
            else if (playerInRange)
            {
                currentInteractionIndex = i; // 更新当前可交互对象
            }
        }
        
        // 检测F键输入
        if (Input.GetKeyDown(interactionKey))
        {
            if (currentInteractionIndex >= 0 && !isPanelOpen)
            {
                // 有Sprite显示且面板未打开时，打开面板
                OpenInteractionPanel();
            }
            else if (isPanelOpen)
            {
                // 面板已打开时，关闭面板
                CloseInteractionPanel();
            }
        }
    }
    
    /// <summary>
    /// 生成物体
    /// </summary>
    public void SpawnObjects()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("预制体未设置！");
            return;
        }
        
        ClearSpawnedObjects();
        spawnedPositions.Clear();
        
        int successfulSpawns = 0;
        int attempts = 0;
        
        while (successfulSpawns < spawnCount && attempts < maxAttempts)
        {
            Vector2 randomPosition = GetRandomPosition();
            
            if (IsValidPosition(randomPosition))
            {
                GameObject spawnedObject = Instantiate(prefabToSpawn, (Vector3)randomPosition, Quaternion.identity);
                

                
                spawnedObjects.Add(spawnedObject);
                spawnedPositions.Add(randomPosition);
                successfulSpawns++;
            }
            
            attempts++;
        }
        
        Debug.Log($"成功生成 {successfulSpawns}/{spawnCount} 个物体，尝试次数：{attempts}");
        
        if (triggerSprite == null)
        {
            Debug.LogWarning("警告：未设置Trigger Sprite，Sprite触发功能将不会工作！");
        }
    }
    
    /// <summary>
    /// 获取随机位置
    /// </summary>
    private Vector2 GetRandomPosition()
    {
        Vector2 basePosition = (Vector2)transform.position;
        
        if (useRectangleArea)
        {
            // 矩形区域生成
            float x = Random.Range(-rectangleSize.x / 2f, rectangleSize.x / 2f);
            float y = Random.Range(-rectangleSize.y / 2f, rectangleSize.y / 2f);
            return basePosition + new Vector2(x, y);
        }
        else
        {
            // 圆形区域生成
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return basePosition + randomCircle;
        }
    }
    
    /// <summary>
    /// 检查位置是否有效
    /// </summary>
    private bool IsValidPosition(Vector2 position)
    {
        // 检查与已生成物体的距离
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if (Vector2.Distance(position, existingPos) < minDistance)
            {
                return false;
            }
        }
        
        // 检查是否与障碍物重叠
        Collider2D hit = Physics2D.OverlapCircle(position, minDistance / 2f, obstacleLayer);
        if (hit != null)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 清除已生成的物体
    /// </summary>
    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();
        spawnedPositions.Clear();
        objectsPlayerInRange.Clear();
        
        // 清除所有Sprite
        foreach (GameObject sprite in spawnedSprites)
        {
            if (sprite != null)
            {
                DestroyImmediate(sprite);
            }
        }
        spawnedSprites.Clear();
        
        Debug.Log($"[ObjectSpawner] 已清除所有生成的对象和Sprite");
    }
    
    /// <summary>
    /// 显示指定索引对象的Sprite
    /// </summary>
    private void ShowSprite(int index)
    {
        if (triggerSprite == null || index >= spawnedObjects.Count) return;
        
        // 确保spawnedSprites列表足够大
        while (spawnedSprites.Count <= index)
        {
            spawnedSprites.Add(null);
        }
        
        // 如果Sprite已经存在，直接激活
        if (spawnedSprites[index] != null)
        {
            spawnedSprites[index].SetActive(true);
            return;
        }
        
        // 创建新的Sprite对象
        GameObject spriteObj = new GameObject($"TriggerSprite_{index}");
        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        
        // 设置Sprite属性
        spriteRenderer.sprite = triggerSprite;
        spriteRenderer.color = spriteColor;
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
        
        // 设置位置和缩放
        Vector3 targetPos = spawnedObjects[index].transform.position + spriteOffset;
        spriteObj.transform.position = targetPos;
        spriteObj.transform.localScale = spriteScale;
        
        // 将Sprite设为对象的子物体
        spriteObj.transform.SetParent(spawnedObjects[index].transform);
        
        spawnedSprites[index] = spriteObj;
        
        Debug.Log($"[ObjectSpawner] 为对象 {index} 显示Sprite");
    }
    
    /// <summary>
    /// 隐藏指定索引对象的Sprite
    /// </summary>
    private void HideSprite(int index)
    {
        if (index >= spawnedSprites.Count || spawnedSprites[index] == null) return;
        
        spawnedSprites[index].SetActive(false);
        Debug.Log($"[ObjectSpawner] 为对象 {index} 隐藏Sprite");
     }
     
     /// <summary>
     /// 打开交互面板并暂停游戏
     /// </summary>
     private void OpenInteractionPanel()
     {
         if (interactionPanel == null)
         {
             Debug.LogWarning("[ObjectSpawner] 交互面板预制体未设置！");
             return;
         }
         
         // 直接激活面板
         interactionPanel.SetActive(true);
         currentPanel = interactionPanel;
         
         // 设置按钮事件和文本
         SetupPanelButtons();
         
         // 暂停游戏
         Time.timeScale = 0f;
         
         isPanelOpen = true;
         Debug.Log($"[ObjectSpawner] 打开交互面板，当前交互对象索引: {currentInteractionIndex}，游戏已暂停");
     }
     
     /// <summary>
     /// 设置面板按钮的事件和文本
     /// </summary>
     private void SetupPanelButtons()
     {
         if (currentPanel == null) return;
         
         // 查找按钮
         UnityEngine.UI.Button[] buttons = currentPanel.GetComponentsInChildren<UnityEngine.UI.Button>();
         
         for (int i = 0; i < buttons.Length && i < 3; i++)
         {
             UnityEngine.UI.Button button = buttons[i];
             
             // 清除现有的监听器
             button.onClick.RemoveAllListeners();
             
             // 设置按钮文本
             UnityEngine.UI.Text buttonText = button.GetComponentInChildren<UnityEngine.UI.Text>();
             if (buttonText != null)
             {
                 switch (i)
                 {
                     case 0:
                         buttonText.text = button1Text;
                         button.onClick.AddListener(OnButton1Click);
                         break;
                     case 1:
                         buttonText.text = button2Text;
                         button.onClick.AddListener(OnButton2Click);
                         break;
                     case 2:
                         buttonText.text = button3Text;
                         button.onClick.AddListener(OnButton3Click);
                         break;
                 }
             }
         }
     }
     
     /// <summary>
     /// 关闭交互面板并恢复游戏
     /// </summary>
     private void CloseInteractionPanel()
     {
         if (currentPanel != null)
         {
             currentPanel.SetActive(false);
         }
         
         // 恢复游戏
         Time.timeScale = 1f;
         
         isPanelOpen = false;
         Debug.Log("[ObjectSpawner] 关闭交互面板，游戏已恢复");
     }
     
     /// <summary>
     /// 按钮1点击事件
     /// </summary>
     public void OnButton1Click()
     {
         Debug.Log($"[ObjectSpawner] 按钮1被点击: {button1Text}");
         // 在这里添加按钮1的具体功能
         CloseInteractionPanel();
     }
     
     /// <summary>
     /// 按钮2点击事件
     /// </summary>
     public void OnButton2Click()
     {
         Debug.Log($"[ObjectSpawner] 按钮2被点击: {button2Text}");
         // 在这里添加按钮2的具体功能
         CloseInteractionPanel();
     }
     
     /// <summary>
     /// 按钮3点击事件
     /// </summary>
     public void OnButton3Click()
     {
         Debug.Log($"[ObjectSpawner] 按钮3被点击: {button3Text}");
         // 在这里添加按钮3的具体功能
         CloseInteractionPanel();
     }
    
    /// <summary>
    /// 重新生成物体
    /// </summary>
    public void RespawnObjects()
    {
        SpawnObjects();
    }
    
    /// <summary>
    /// 获取已生成的物体列表
    /// </summary>
    public List<GameObject> GetSpawnedObjects()
    {
        return new List<GameObject>(spawnedObjects);
    }
    
    /// <summary>
    /// 设置生成参数
    /// </summary>
    public void SetSpawnParameters(int count, float radius, float distance)
    {
        spawnCount = count;
        spawnRadius = radius;
        minDistance = distance;
    }
    

    
    // 在Scene视图中显示生成区域
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        
        if (useRectangleArea)
        {
            // 绘制矩形区域
            Gizmos.DrawWireCube(center, new Vector3(rectangleSize.x, rectangleSize.y, 0f));
        }
        else
        {
            // 绘制圆形区域
            Gizmos.DrawWireSphere(center, spawnRadius);
        }
        
        // 绘制已生成物体的位置和间距
        Gizmos.color = Color.green;
        foreach (Vector2 pos in spawnedPositions)
        {
            Gizmos.DrawWireSphere((Vector3)pos, minDistance / 2f);
        }
        
        // 显示检测范围
        if (showDetectionRange && player != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    Vector3 objPos = spawnedObjects[i].transform.position;
                    Gizmos.DrawWireSphere(objPos, detectionRadius);
                    
                    // 如果玩家在范围内，绘制连线
                    if (i < objectsPlayerInRange.Count && objectsPlayerInRange[i])
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(player.transform.position, objPos);
                        Gizmos.color = Color.cyan;
                    }
                }
            }
        }
     }
     
     void OnGUI()
     {
         if (!showRuntimeDebug || player == null) return;
         
         GUILayout.BeginArea(new Rect(10, 10, 300, 200));
         GUILayout.BeginVertical("box");
         
         GUILayout.Label("ObjectSpawner 调试信息");
         GUILayout.Label($"生成对象数量: {spawnedObjects.Count}");
         GUILayout.Label($"玩家位置: {player.transform.position}");
         GUILayout.Label($"检测半径: {detectionRadius}");
         
         GUILayout.Space(10);
         
         int inRangeCount = 0;
         for (int i = 0; i < spawnedObjects.Count; i++)
         {
             if (spawnedObjects[i] != null)
             {
                 float distance = Vector2.Distance(player.transform.position, spawnedObjects[i].transform.position);
                 bool inRange = distance <= detectionRadius;
                 if (inRange) inRangeCount++;
                 
                 if (i < 5) // 只显示前5个对象的详细信息
                 {
                     string status = inRange ? "范围内" : "范围外";
                      GUI.color = inRange ? Color.green : Color.red;
                      GUILayout.Label($"对象{i}: 距离{distance:F1} - {status}");
                      GUI.color = Color.white;
                 }
             }
         }
         
         GUILayout.Space(5);
         GUILayout.Label($"范围内对象: {inRangeCount}/{spawnedObjects.Count}");
         
         GUILayout.EndVertical();
         GUILayout.EndArea();
     }
 }