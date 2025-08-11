using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人生成器 - 当场上没有敌人时在摄像机外侧生成敌人
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("敌人预制体")]
    [Tooltip("要生成的敌人预制体")]
    public GameObject enemyPrefab;
    
    [Header("生成设置")]
    [Tooltip("每次生成的敌人数量")]
    public int spawnCount = 10;
    [Tooltip("生成间隔时间(秒)")]
    public float spawnInterval = 1f;
    [Tooltip("距离摄像机边界的Unity单位距离")]
    public float spawnOffset = 3f;
    
    [Header("敌人检测")]
    [Tooltip("敌人的标签")]
    public string enemyTag = "Enemy";
    [Tooltip("检测敌人的范围半径(0表示全地图)")]
    public float detectionRadius = 0f;
    
    private Camera mainCamera;
    private float lastSpawnTime;
    private bool isSpawning = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("未找到主摄像机！");
        }
    }
    
    void Update()
    {
        // 检查是否需要生成敌人
        if (!isSpawning && ShouldSpawnEnemies())
        {
            StartCoroutine(SpawnEnemiesCoroutine());
        }
    }
    
    /// <summary>
    /// 检查是否应该生成敌人
    /// </summary>
    /// <returns>如果场上没有敌人则返回true</returns>
    private bool ShouldSpawnEnemies()
    {
        // 使用检测半径
        if (detectionRadius > 0)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (Collider2D col in enemies)
            {
                if (col.CompareTag(enemyTag))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            // 全局检测
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            return enemies.Length == 0;
        }
    }
    
    /// <summary>
    /// 协程：分批生成敌人
    /// </summary>
    private System.Collections.IEnumerator SpawnEnemiesCoroutine()
    {
        isSpawning = true;
        Debug.Log("开始生成敌人...");
        
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
        
        isSpawning = false;
        Debug.Log($"已生成 {spawnCount} 个敌人");
    }
    
    /// <summary>
    /// 在摄像机外侧生成一个敌人
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("敌人预制体未设置！");
            return;
        }
        
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // 可选：为新敌人设置父对象
        newEnemy.transform.SetParent(transform);

        // 提示：确保敌人预制体上挂载了EnemyMovement脚本，以便敌人自动朝目标移动
        if (newEnemy.GetComponent<EnemyMovement>() == null)
        {
            Debug.LogWarning($"生成的敌人 '{newEnemy.name}' 未挂载 EnemyMovement 脚本。请确保敌人预制体上包含此脚本，以便敌人自动朝目标移动。");
        }
    }
    
    /// <summary>
    /// 计算敌人的生成位置，使其在摄像机显示区域外侧。
    /// </summary>
    /// <returns>世界坐标系中的生成位置</returns>
    private Vector3 GetSpawnPosition()
    {
        if (mainCamera == null)
        {
            Debug.LogError("主摄像机未设置，无法计算生成位置！");
            return transform.position; // 返回默认位置以避免错误
        }

        // 获取摄像机视口的边界（世界坐标）
        Vector2 screenBottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 screenTopRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float cameraWidth = screenTopRight.x - screenBottomLeft.x;
        float cameraHeight = screenTopRight.y - screenBottomLeft.y;

        // 随机选择一个方向（上、下、左、右）
        int side = Random.Range(0, 4); // 0:上, 1:下, 2:左, 3:右

        Vector3 spawnPosition = Vector3.zero;

        switch (side)
        {
            case 0: // 上方
                spawnPosition.x = Random.Range(screenBottomLeft.x, screenTopRight.x);
                spawnPosition.y = screenTopRight.y + spawnOffset;
                break;
            case 1: // 下方
                spawnPosition.x = Random.Range(screenBottomLeft.x, screenTopRight.x);
                spawnPosition.y = screenBottomLeft.y - spawnOffset;
                break;
            case 2: // 左方
                spawnPosition.x = screenBottomLeft.x - spawnOffset;
                spawnPosition.y = Random.Range(screenBottomLeft.y, screenTopRight.y);
                break;
            case 3: // 右方
                spawnPosition.x = screenTopRight.x + spawnOffset;
                spawnPosition.y = Random.Range(screenBottomLeft.y, screenTopRight.y);
                break;
        }

        return spawnPosition;
    }
    
    /// <summary>
    /// 立即生成敌人（供外部调用）
    /// </summary>
    [ContextMenu("立即生成敌人")]
    public void ForceSpawnEnemies()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnEnemiesCoroutine());
        }
    }
    
    /// <summary>
    /// 设置新的敌人预制体
    /// </summary>
    /// <param name="newPrefab">新的敌人预制体</param>
    public void SetEnemyPrefab(GameObject newPrefab)
    {
        enemyPrefab = newPrefab;
    }
    
    /// <summary>
    /// 调试可视化
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (detectionRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    
    /// <summary>
    /// 获取当前敌人数量
    /// </summary>
    /// <returns>当前场景中的敌人数量</returns>
    public int GetCurrentEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        return enemies.Length;
    }
}