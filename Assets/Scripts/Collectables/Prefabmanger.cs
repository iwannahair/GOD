using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理和实例化预制体的管理器
/// </summary>
public class PrefabManager : MonoBehaviour
{
    // 在Inspector面板中可见的预制体列表
    [Tooltip("要实例化的预制体列表")]
    public List<GameObject> prefabList = new List<GameObject>();

    // 触发实例化的按键
    [Tooltip("触发实例化的按键")]
    public KeyCode spawnKey = KeyCode.Space;

    // 预制体生成的位置
    [Tooltip("预制体生成的位置")]
    public Transform spawnPoint;

    // 当前要生成的预制体在列表中的索引
    private int currentIndex = 0;

    /// <summary>
    /// 每帧调用一次，用于检测玩家输入
    /// </summary>
    void Update()
    {
        // 检测玩家是否按下了指定的按键
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnPrefab();
        }
    }

    /// <summary>
    /// 添加预制体到列表末尾
    /// </summary>
    /// <param name="prefab">要添加的预制体</param>
    public void AddPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            // 添加到列表末尾，保持与AddPrefabAndSpawn方法的一致性
            prefabList.Add(prefab);
            Debug.Log($"已将预制体 {prefab.name} 添加到列表末尾，当前列表数量: {prefabList.Count}");
        }
        else
        {
            Debug.LogWarning("尝试添加空的预制体到列表中！");
        }
    }
    
    /// <summary>
    /// 添加预制体到列表并立即实例化
    /// 第一个prefab大小为1，后续prefab大小根据下标递增
    /// 后生成的prefab渲染层级始终在前一个prefab的下面
    /// </summary>
    /// <param name="prefab">要添加并实例化的预制体</param>
    public void AddPrefabAndSpawn(GameObject prefab)
    {
        if (prefab != null)
        {
            // 添加到列表末尾
            prefabList.Add(prefab);
            
            // 立即实例化
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            
            // 计算大小：第一个prefab大小为1，后续prefab大小根据下标递增
            int prefabIndex = prefabList.Count - 1; // 获取当前添加的prefab在列表中的索引
            float scale = (float)(prefabIndex + 1); // 下标0 -> 大小1, 下标1 -> 大小2, 下标2 -> 大小3...
            instance.transform.localScale = new Vector3(scale, scale, scale);
            
            // 设置渲染层级，确保后生成的prefab在前一个prefab的下面
            SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // 如果是第一个实例，使用默认排序顺序
                // 如果不是第一个实例，则将排序顺序设置为比上一个实例低1，确保在下面渲染
                if (lastSpawnedInstance != null)
                {
                    SpriteRenderer lastRenderer = lastSpawnedInstance.GetComponent<SpriteRenderer>();
                    if (lastRenderer != null)
                    {
                        renderer.sortingOrder = lastRenderer.sortingOrder - 1;
                    }
                }
                Debug.Log($"已设置预制体渲染层级: {renderer.sortingOrder}");
            }
            
            // 更新上一个生成的实例引用
            lastSpawnedInstance = instance;
            
            Debug.Log($"已将预制体 {prefab.name} 添加到列表并实例化，下标: {prefabIndex}，大小: {scale}，当前列表数量: {prefabList.Count}");
        }
        else
        {
            Debug.LogWarning("尝试添加并实例化空的预制体！");
        }
    }
    
    /// <summary>
    /// 移除指定的预制体
    /// </summary>
    /// <param name="prefab">要移除的预制体</param>
    /// <returns>是否成功移除</returns>
    public bool RemovePrefab(GameObject prefab)
    {
        if (prefab != null && prefabList.Contains(prefab))
        {
            prefabList.Remove(prefab);
            Debug.Log($"已从列表中移除预制体 {prefab.name}，当前列表数量: {prefabList.Count}");
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 清空预制体列表
    /// </summary>
    public void ClearPrefabList()
    {
        prefabList.Clear();
        currentIndex = 0;
        Debug.Log("预制体列表已清空");
    }
    
    /// <summary>
    /// 获取当前预制体列表数量
    /// </summary>
    /// <returns>列表中预制体的数量</returns>
    public int GetPrefabCount()
    {
        return prefabList.Count;
    }

    // 记录上一个生成的预制体实例，用于设置渲染层级关系
    private GameObject lastSpawnedInstance;
    
    /// <summary>
    /// 实例化预制体
    /// 从下标0开始实例化，下标0的prefab大小为1，下标每增加1，prefab大小加1
    /// 后生成的prefab渲染层级始终在前一个prefab的下面
    /// </summary>
    public void SpawnPrefab()
    {
        // 检查预制体列表是否为空
        if (prefabList.Count == 0)
        {
            Debug.LogWarning("预制体列表为空，无法生成预制体。");
            return;
        }

        // 强制从下标0开始生成，解决第一个prefab下标为6的问题
        currentIndex = 0;
        
        // 获取当前要生成的预制体（从下标0开始）
        GameObject prefabToSpawn = prefabList[currentIndex];

        if (prefabToSpawn != null)
        {
            // 确定生成位置
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

            // 实例化预制体
            GameObject instance = Instantiate(prefabToSpawn, position, Quaternion.identity);

            // 根据下标计算大小：下标0对应大小1，下标1对应大小2，以此类推
            float scale = (float)(currentIndex + 1); // 下标0 -> 大小1, 下标1 -> 大小2, 下标2 -> 大小3...
            instance.transform.localScale = new Vector3(scale, scale, scale);
            
            // 设置渲染层级，确保后生成的prefab在前一个prefab的下面
            SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // 如果是第一个实例，使用默认排序顺序
                // 如果不是第一个实例，则将排序顺序设置为比上一个实例低1，确保在下面渲染
                if (lastSpawnedInstance != null)
                {
                    SpriteRenderer lastRenderer = lastSpawnedInstance.GetComponent<SpriteRenderer>();
                    if (lastRenderer != null)
                    {
                        renderer.sortingOrder = lastRenderer.sortingOrder - 1;
                    }
                }
                Debug.Log($"已设置预制体渲染层级: {renderer.sortingOrder}");
            }
            
            // 更新上一个生成的实例引用
            lastSpawnedInstance = instance;

            Debug.Log($"已生成预制体: {prefabToSpawn.name}，下标: {currentIndex}，大小: {scale}");

            // 更新索引，准备生成下一个预制体
            currentIndex++;
        }
        else
        {
            Debug.LogWarning($"索引 {currentIndex} 处的预制体为空。");
            // 跳过空的预制体，继续下一个
            currentIndex++;
        }
    }
}