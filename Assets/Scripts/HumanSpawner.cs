using UnityEngine;

/// <summary>
/// 人类生成器类：负责在游戏中生成人类实体
/// 该类管理人类的初始生成和定时生成功能
/// </summary>
public class HumanSpawner : MonoBehaviour
{
    [SerializeField] private GameObject humanPrefab; // 人类预制体，用于实例化生成
    [SerializeField] private int initialHumanCount = 10; // 初始人类数量，游戏开始时生成
    [SerializeField] private CreateHumanSpawner createHumanSpawner; // CreateHumanSpawner脚本引用，用于获取生成位置和注册人类
    [SerializeField] private float spawnInterval = 10f; // 生成间隔时间，控制人类生成的频率
    
    private float nextSpawnTime; // 下一次生成人类的时间点
    
    /// <summary>
    /// 初始化方法：检查依赖组件并设置初始状态
    /// </summary>
    private void Start()
    {
        if (createHumanSpawner == null)
        {
            Debug.LogError("请设置CreateHumanSpawner以获取生成位置！");
            return;
        }
        
        // 初始生成人类，游戏开始时立即生成指定数量的人类
        SpawnInitialHumans();
        
        // 设置下一次生成时间，基于当前时间和生成间隔
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    /// <summary>
    /// 每帧更新：检查是否到达生成时间点
    /// </summary>
    private void Update()
    {
        // 定时生成新的人类，当达到预设时间点时触发
        if (Time.time >= nextSpawnTime)
        {
            SpawnHuman();
            nextSpawnTime = Time.time + spawnInterval; // 更新下一次生成时间
        }
    }
    
    /// <summary>
    /// 初始生成人类：在游戏开始时批量生成人类
    /// </summary>
    private void SpawnInitialHumans()
    {
        for (int i = 0; i < initialHumanCount; i++)
        {
            SpawnHuman();
        }
    }
    
    /// <summary>
    /// 生成单个人类：在指定位置实例化人类并注册到CreateHumanSpawner
    /// </summary>
    private void SpawnHuman()
    {
        if (humanPrefab == null || createHumanSpawner == null) return; // 安全检查
        
        // 使用createHumanTransform的位置生成，获取生成点位置
        Transform createHumanTransform = createHumanSpawner.GetCreateHumanTransform();
        if (createHumanTransform == null) return; // 如果没有有效的生成点则退出
        
        Vector3 spawnPos = createHumanTransform.position; // 获取生成位置
        
        // 生成人类并注册到CreateHumanSpawner，实例化并交由CreateHumanSpawner管理
        GameObject newHuman = Instantiate(humanPrefab, spawnPos, Quaternion.identity);
        createHumanSpawner.RegisterHuman(newHuman);
    }
}