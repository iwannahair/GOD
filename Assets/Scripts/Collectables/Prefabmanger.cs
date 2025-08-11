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
    /// 根据预制体在列表中的位置设置大小：第一个位置大小为1，第二个位置大小为2，以此类推
    /// </summary>
    /// <param name="prefab">要添加并实例化的预制体</param>
    public void AddPrefabAndSpawn(GameObject prefab)
    {
        if (prefab != null)
        {
            // 添加到列表末尾，这样可以根据位置正确设置递增大小
            prefabList.Add(prefab);
            
            // 立即实例化
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            
            // 根据预制体在列表中的位置计算大小
            // 获取刚添加的预制体在列表中的索引位置
            int prefabIndex = prefabList.Count - 1; // 最后一个元素的索引
            float scale = (float)(prefabIndex + 1); // 索引0对应大小1，索引1对应大小2，以此类推
            instance.transform.localScale = new Vector3(scale, scale, scale);
            
            Debug.Log($"已将预制体 {prefab.name} 添加到列表位置 {prefabIndex} 并实例化，大小: {scale}，当前列表数量: {prefabList.Count}");
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

    /// <summary>
    /// 实例化预制体
    /// </summary>
    public void SpawnPrefab()
    {
        // 检查预制体列表是否为空以及索引是否有效
        if (prefabList.Count == 0)
        {            Debug.LogWarning("预制体列表为空，无法生成预制体。");
            return;
        }

        // 确保索引在列表范围内
        if (currentIndex >= prefabList.Count)
        {
            Debug.Log("已生成所有预制体，重置索引。");
            currentIndex = 0; // 重置索引或停止生成
        }

        // 获取当前要生成的预制体
        GameObject prefabToSpawn = prefabList[currentIndex];

        if (prefabToSpawn != null)
        {
            // 确定生成位置
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

            // 实例化预制体
            GameObject instance = Instantiate(prefabToSpawn, position, Quaternion.identity);

            // 根据索引计算大小，第0个为2，后续递增1
            float scale = 1.0f + currentIndex;
            instance.transform.localScale = new Vector3(scale, scale, scale);

            Debug.Log($"已生成预制体: {prefabToSpawn.name}，大小调整为: {scale}");

            // 更新索引，准备生成下一个预制体
            currentIndex++;
        }
        else
        {
            Debug.LogWarning($"索引 {currentIndex} 处的预制体为空。");
        }
    }
}