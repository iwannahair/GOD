using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CreateHuman管理器：负责在地图上随机生成多个CreateHuman实体
/// </summary>
public class CreateHumanManager : MonoBehaviour
{
    [SerializeField] private GameObject createHumanSpawnerPrefab; // CreateHumanSpawner预制体
    [SerializeField] private int minCreateHumanCount = 2; // 最小CreateHuman数量
    [SerializeField] private int maxCreateHumanCount = 5; // 最大CreateHuman数量
    [SerializeField] private float mapWidth = 50f; // 地图宽度
    [SerializeField] private float mapHeight = 50f; // 地图高度
    [SerializeField] private float minDistanceBetweenSpawners = 10f; // CreateHuman之间的最小距离
    
    private List<GameObject> createHumanSpawners = new List<GameObject>(); // 所有生成的CreateHumanSpawner
    
    /// <summary>
    /// 初始化：随机生成多个CreateHuman
    /// </summary>
    private void Start()
    {
        // 随机确定要生成的CreateHuman数量
        int createHumanCount = Random.Range(minCreateHumanCount, maxCreateHumanCount + 1);
        Debug.Log($"将在地图上生成{createHumanCount}个CreateHuman");
        
        // 生成指定数量的CreateHuman
        for (int i = 0; i < createHumanCount; i++)
        {
            SpawnCreateHumanSpawner();
        }
    }
    
    /// <summary>
    /// 生成单个CreateHumanSpawner
    /// </summary>
    private void SpawnCreateHumanSpawner()
    {
        if (createHumanSpawnerPrefab == null) return;
        
        // 尝试找到合适的生成位置
        Vector3 spawnPosition = FindValidSpawnPosition();
        
        // 实例化CreateHumanSpawner
        GameObject spawner = Instantiate(createHumanSpawnerPrefab, spawnPosition, Quaternion.identity);
        createHumanSpawners.Add(spawner);
        Debug.Log($"在位置{spawnPosition}生成了CreateHuman，当前总数：{createHumanSpawners.Count}");
    }
    
    /// <summary>
    /// 寻找有效的生成位置，确保与其他CreateHuman保持一定距离
    /// </summary>
    /// <returns>有效的生成位置</returns>
    private Vector3 FindValidSpawnPosition()
    {
        Vector3 position;
        int maxAttempts = 30; // 最大尝试次数
        int attempts = 0;
        
        do
        {
            // 在地图范围内随机选择位置
            float x = Random.Range(-mapWidth/2, mapWidth/2);
            float y = Random.Range(-mapHeight/2, mapHeight/2);
            position = new Vector3(x, y, 0);
            
            // 检查是否与现有的CreateHuman距离过近
            if (IsPositionValid(position))
            {
                return position;
            }
            
            attempts++;
        } while (attempts < maxAttempts);
        
        // 如果尝试多次仍找不到合适位置，返回一个随机位置
        Debug.LogWarning("无法找到理想的CreateHuman生成位置，使用随机位置");
        float fallbackX = Random.Range(-mapWidth/2, mapWidth/2);
        float fallbackY = Random.Range(-mapHeight/2, mapHeight/2);
        return new Vector3(fallbackX, fallbackY, 0);
    }
    
    /// <summary>
    /// 检查位置是否有效（与其他CreateHuman保持足够距离）
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>位置是否有效</returns>
    private bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject spawner in createHumanSpawners)
        {
            if (spawner == null) continue;
            
            float distance = Vector3.Distance(position, spawner.transform.position);
            if (distance < minDistanceBetweenSpawners)
            {
                return false; // 距离太近，位置无效
            }
        }
        
        return true; // 位置有效
    }
}