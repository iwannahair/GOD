using UnityEngine;
using System.Collections.Generic;

public class RandomObjectSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject objectPrefab; // 要生成的物体预制体
    public int minObjectCount = 3; // 最少生成数量
    public int maxObjectCount = 10; // 最多生成数量
    
    [Header("生成区域")]
    public Vector2 spawnAreaMin = new Vector2(-10f, -10f); // 生成区域最小坐标
    public Vector2 spawnAreaMax = new Vector2(10f, 10f); // 生成区域最大坐标
    
    [Header("生成间隔")]
    public float minDistance = 2f; // 物体之间的最小距离
    
    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    void Start()
    {
        SpawnRandomObjects();
    }
    
    void SpawnRandomObjects()
    {
        // 随机决定生成数量
        int objectCount = Random.Range(minObjectCount, maxObjectCount + 1);
        
        for (int i = 0; i < objectCount; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition();
            
            if (spawnPosition != Vector2.zero)
            {
                GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
                
                // 确保物体有InteractableObject组件
                if (spawnedObject.GetComponent<InteractableObject>() == null)
                {
                    spawnedObject.AddComponent<InteractableObject>();
                }
                
                spawnedObjects.Add(spawnedObject);
            }
        }
        
        Debug.Log($"成功生成了 {spawnedObjects.Count} 个可交互物体");
    }
    
    Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 50; // 最大尝试次数，防止无限循环
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 在指定区域内随机生成位置
            Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            
            // 检查是否与已生成的物体距离足够
            bool validPosition = true;
            foreach (GameObject obj in spawnedObjects)
            {
                if (Vector2.Distance(randomPosition, obj.transform.position) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
            
            if (validPosition)
            {
                return randomPosition;
            }
        }
        
        Debug.LogWarning("无法找到有效的生成位置");
        return Vector2.zero;
    }
    
    // 清除所有生成的物体
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
    }
    
    // 重新生成物体
    public void RespawnObjects()
    {
        ClearSpawnedObjects();
        SpawnRandomObjects();
    }
    
    // 在Scene视图中显示生成区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2f;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);
    }
}