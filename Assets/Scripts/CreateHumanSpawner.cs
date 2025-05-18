using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CreateHuman生成器类：负责管理CreateHuman实体及其生成的人类
/// 该类控制CreateHuman的生成、人类的注册和跟踪，以及基于影响范围的生命周期管理
/// </summary>
public class CreateHumanSpawner : MonoBehaviour
{
    [SerializeField] private GameObject createHumanPrefab; // createhuman预制体，用于实例化
    [SerializeField] private float spawnRadius = 20f; // 生成半径，决定createhuman的生成范围
    [SerializeField] private float influenceRadius = 10f; // createhuman的影响半径，超出此范围的人类将被移除跟踪
    [SerializeField] private int maxHumansToSpawn = 5; // 最大生成人类数量，设为5，限制单个createhuman可生成的最大人类数
    [SerializeField] private int minHumansToSpawn = 2; // 最小生成人类数量，设为2，确保至少生成的人类数
    private int targetHumansToSpawn; // 本次实际要生成的human数量，在minHumansToSpawn和maxHumansToSpawn之间随机
    
    private GameObject createHumanInstance; // 当前生成的createhuman实例引用
    private List<GameObject> spawnedHumans = new List<GameObject>(); // 跟踪生成的human实例列表
    private bool hasGeneratedHumans = false; // 标记是否已经生成过human，用于控制逻辑流程
    private int humansSpawnedCount = 0; // 已生成的人类总数量计数
    private bool canGenerateMore = true; // 是否还能生成更多人类，达到目标数量后设为false
    private int createHumanSpawnCount = 0; // createhuman的生成次数计数器，限制只生成一次

    /// <summary>
    /// 初始化方法：设置目标生成数量并生成CreateHuman
    /// </summary>
    private void Start()
    {
        // 随机确定本次要生成的human数量，在最小和最大值之间随机
        targetHumansToSpawn = Random.Range(minHumansToSpawn, maxHumansToSpawn + 1);
        Debug.Log($"本次将生成{targetHumansToSpawn}个human");
        
        // 生成CreateHuman实体
        SpawnCreateHuman();
    }

    /// <summary>
    /// 每帧更新：管理已生成人类的跟踪和CreateHuman的生命周期
    /// </summary>
    private void Update()
    {
        if (createHumanInstance == null) return; // 如果没有createhuman实例则不执行后续逻辑

        // 只有在已经生成过人类的情况下才执行跟踪逻辑
        if (hasGeneratedHumans)
        {
            bool anyHumanInRange = false; // 标记是否有任何人类在影响范围内
            List<GameObject> humansToRemove = new List<GameObject>(); // 存储需要从跟踪列表中移除的人类

            // 检查所有human实例是否在影响范围内
            foreach (GameObject human in spawnedHumans)
            {
                // 如果human已被销毁，添加到移除列表
                if (human == null)
                {
                    humansToRemove.Add(human);
                    continue;
                }

                // 计算human与createhuman之间的距离
                float distance = Vector3.Distance(human.transform.position, createHumanInstance.transform.position);
                Debug.Log("Human距离: " + distance + ", 影响半径: " + influenceRadius);

                // 如果距离超出影响半径，添加到移除列表
                if (distance > influenceRadius)
                {
                    humansToRemove.Add(human);
                }
                else
                {
                    anyHumanInRange = true; // 至少有一个human在范围内
                }
            }

            // 从跟踪列表中移除离开范围的human
            foreach (GameObject human in humansToRemove)
            {
                spawnedHumans.Remove(human);
                Debug.Log("Human离开范围，从列表中移除");
            }
            
            // 如果没有human在范围内，销毁createhuman实例
            if (!anyHumanInRange && spawnedHumans.Count == 0 && createHumanInstance != null)
            {
                Debug.Log("销毁createhuman - 所有human已离开范围");
                Destroy(createHumanInstance);
                createHumanInstance = null;
            }
        }
    }

    /// <summary>
    /// 生成CreateHuman：在指定范围内随机位置生成CreateHuman实体
    /// </summary>
    private void SpawnCreateHuman()
    {
        // 检查是否已经生成过一次createhuman，确保只生成一次
        if (createHumanPrefab == null || !canGenerateMore || createHumanSpawnCount >= 1) return;

        // 在圆形区域内随机生成位置
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, randomCircle.y, 0);

        // 实例化createhuman预制体
        createHumanInstance = Instantiate(createHumanPrefab, spawnPos, Quaternion.identity);
        createHumanSpawnCount++; // 增加createhuman生成次数
        Debug.Log($"生成createhuman成功，当前是第{createHumanSpawnCount}次生成");
    }

    /// <summary>
    /// 获取CreateHuman的Transform：供外部类获取生成位置
    /// </summary>
    /// <returns>CreateHuman的Transform组件，如果不存在则返回null</returns>
    public Transform GetCreateHumanTransform()
    {
        return createHumanInstance != null ? createHumanInstance.transform : null;
    }

    /// <summary>
    /// 注册人类：将新生成的人类添加到跟踪列表中
    /// </summary>
    /// <param name="human">要注册的人类GameObject</param>
    public void RegisterHuman(GameObject human)
    {
        // 检查是否可以注册更多人类（未达到目标数量且允许生成）
        if (human != null && canGenerateMore && humansSpawnedCount < targetHumansToSpawn)
        {
            spawnedHumans.Add(human); // 添加到跟踪列表
            humansSpawnedCount++; // 增加已生成计数
            hasGeneratedHumans = true; // 标记已生成过人类
            Debug.Log($"注册human成功，当前human数量: {spawnedHumans.Count}，总共生成: {humansSpawnedCount}/{targetHumansToSpawn}");

            // 如果已达到目标生成数量，禁止再生成
            if (humansSpawnedCount >= targetHumansToSpawn)
            {
                canGenerateMore = false;
                Debug.Log($"已生成{humansSpawnedCount}个人类，达到目标数量{targetHumansToSpawn}，停止生成更多human");
            }
        }
        else
        {
            // 如果不能注册更多人类，销毁多余的human
            if (human != null)
            {
                Debug.Log("无法生成更多human，销毁多余的human");
                Destroy(human);
            }
        }
    }
}