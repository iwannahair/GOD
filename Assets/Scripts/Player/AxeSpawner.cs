using UnityEngine;
using System.Collections;

/// <summary>
/// 斧子生成器 - 负责在游戏过程中动态生成和管理斧子
/// 可以根据游戏进度、玩家等级或特定事件来增加斧子数量
/// </summary>
public class AxeSpawner : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private AxeManager axeManager; // 斧子管理器引用
    [SerializeField] private bool autoSpawn = true; // 是否自动生成斧子
    [SerializeField] private float spawnInterval = 10f; // 自动生成间隔（秒）
    [SerializeField] private int maxAutoSpawnCount = 5; // 自动生成的最大数量
    
    [Header("升级条件")]
    [SerializeField] private int enemiesKilledPerAxe = 10; // 每杀死多少敌人增加一个斧子
    [SerializeField] private bool spawnOnLevelUp = true; // 是否在等级提升时生成斧子
    
    [Header("调试信息")]
    [SerializeField] private int currentEnemiesKilled = 0; // 当前杀死的敌人数量
    [SerializeField] private int totalAxesSpawned = 0; // 总共生成的斧子数量
    
    // 私有变量
    private float nextSpawnTime; // 下次自动生成时间
    private bool isInitialized = false; // 是否已初始化
    
    /// <summary>
    /// 初始化斧子生成器
    /// </summary>
    private void Start()
    {
        InitializeSpawner();
    }
    
    /// <summary>
    /// 初始化生成器设置
    /// </summary>
    private void InitializeSpawner()
    {
        // 如果没有设置斧子管理器，尝试自动查找
        if (axeManager == null)
        {
            axeManager = FindObjectOfType<AxeManager>();
            if (axeManager == null)
            {
                Debug.LogError("AxeSpawner: 未找到AxeManager组件！请确保场景中存在AxeManager。");
                enabled = false;
                return;
            }
        }
        
        // 设置下次生成时间
        nextSpawnTime = Time.time + spawnInterval;
        
        // 订阅游戏事件
        SubscribeToGameEvents();
        
        isInitialized = true;
        Debug.Log("AxeSpawner 初始化完成");
    }
    
    /// <summary>
    /// 订阅游戏相关事件
    /// </summary>
    private void SubscribeToGameEvents()
    {
        // 这里可以订阅敌人死亡事件、等级提升事件等
        // 由于当前项目结构，我们使用Update来检查条件
    }
    
    /// <summary>
    /// 每帧更新检查生成条件
    /// </summary>
    private void Update()
    {
        if (!isInitialized || axeManager == null) return;
        
        // 检查自动生成条件
        CheckAutoSpawn();
        
        // 检查基于敌人击杀数的生成条件
        CheckEnemyKillSpawn();
    }
    
    /// <summary>
    /// 检查自动生成条件
    /// </summary>
    private void CheckAutoSpawn()
    {
        if (!autoSpawn) return;
        
        // 检查是否到达生成时间且未超过最大数量
        if (Time.time >= nextSpawnTime && axeManager.GetAxeCount() < maxAutoSpawnCount)
        {
            SpawnAxe("自动生成");
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    /// <summary>
    /// 检查基于敌人击杀数的生成条件
    /// </summary>
    private void CheckEnemyKillSpawn()
    {
        // 从GameManager获取敌人击杀数（如果可用）
        if (GameManager.instance != null)
        {
            // 这里需要根据实际的GameManager实现来获取击杀数
            // 由于当前GameManager没有公开的击杀数属性，我们使用一个简化的实现
            
            // 检查是否达到生成新斧子的击杀数要求
            int requiredKills = (totalAxesSpawned + 1) * enemiesKilledPerAxe;
            if (currentEnemiesKilled >= requiredKills)
            {
                SpawnAxe($"击杀{requiredKills}个敌人奖励");
            }
        }
    }
    
    /// <summary>
    /// 生成一个新的斧子
    /// </summary>
    /// <param name="reason">生成原因（用于调试）</param>
    public void SpawnAxe(string reason = "手动生成")
    {
        if (axeManager == null)
        {
            Debug.LogError("AxeSpawner: AxeManager为空，无法生成斧子！");
            return;
        }
        
        // 尝试创建新斧子
        GameObject newAxe = axeManager.CreateAxe();
        
        if (newAxe != null)
        {
            totalAxesSpawned++;
            Debug.Log($"斧子生成成功！原因: {reason}，当前斧子数量: {axeManager.GetAxeCount()}");
            
            // 可以在这里添加生成特效、音效等
            PlaySpawnEffect(newAxe.transform.position);
        }
        else
        {
            Debug.LogWarning($"斧子生成失败！原因: {reason}");
        }
    }
    
    /// <summary>
    /// 移除一个斧子
    /// </summary>
    public void RemoveAxe()
    {
        if (axeManager != null)
        {
            axeManager.RemoveLastAxe();
            Debug.Log($"移除斧子，当前斧子数量: {axeManager.GetAxeCount()}");
        }
    }
    
    /// <summary>
    /// 设置斧子数量到指定值
    /// </summary>
    /// <param name="count">目标斧子数量</param>
    public void SetAxeCount(int count)
    {
        if (axeManager != null)
        {
            axeManager.SetAxeCount(count);
            Debug.Log($"设置斧子数量为: {count}");
        }
    }
    
    /// <summary>
    /// 播放生成特效
    /// </summary>
    /// <param name="position">特效位置</param>
    private void PlaySpawnEffect(Vector3 position)
    {
        // 这里可以添加粒子特效、音效等
        // 例如：从GameManager的对象池中获取特效对象
        if (GameManager.instance != null)
        {
            // 使用GameManager的splash效果作为生成特效
            GameObject effect = GameManager.instance.GetSplash(position);
            if (effect != null)
            {
                // 设置特效持续时间
                StartCoroutine(ReturnEffectAfterDelay(effect, 1f));
            }
        }
    }
    
    /// <summary>
    /// 延迟归还特效对象到对象池
    /// </summary>
    /// <param name="effect">特效对象</param>
    /// <param name="delay">延迟时间</param>
    /// <returns></returns>
    private IEnumerator ReturnEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.instance != null && effect != null)
        {
            GameManager.instance.ReturnSplash(effect);
        }
    }
    
    /// <summary>
    /// 手动触发敌人击杀事件（供外部调用）
    /// </summary>
    public void OnEnemyKilled()
    {
        currentEnemiesKilled++;
        Debug.Log($"敌人击杀数更新: {currentEnemiesKilled}");
    }
    
    /// <summary>
    /// 重置击杀计数
    /// </summary>
    public void ResetKillCount()
    {
        currentEnemiesKilled = 0;
        Debug.Log("击杀计数已重置");
    }
    
    /// <summary>
    /// 获取当前斧子数量
    /// </summary>
    /// <returns>当前斧子数量</returns>
    public int GetCurrentAxeCount()
    {
        return axeManager != null ? axeManager.GetAxeCount() : 0;
    }
    
    /// <summary>
    /// 获取总生成斧子数量
    /// </summary>
    /// <returns>总生成斧子数量</returns>
    public int GetTotalAxesSpawned()
    {
        return totalAxesSpawned;
    }
    
    /// <summary>
    /// 设置自动生成开关
    /// </summary>
    /// <param name="enabled">是否启用自动生成</param>
    public void SetAutoSpawn(bool enabled)
    {
        autoSpawn = enabled;
        if (enabled)
        {
            nextSpawnTime = Time.time + spawnInterval;
        }
        Debug.Log($"自动生成斧子: {(enabled ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 设置生成间隔
    /// </summary>
    /// <param name="interval">新的生成间隔（秒）</param>
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(1f, interval); // 最小间隔1秒
        Debug.Log($"生成间隔设置为: {spawnInterval}秒");
    }
    
    /// <summary>
    /// 清理资源
    /// </summary>
    private void OnDestroy()
    {
        // 取消订阅事件
        // 这里可以添加事件取消订阅的代码
    }
    
    /// <summary>
    /// 在编辑器中显示调试信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (axeManager != null)
        {
            // 在场景视图中显示斧子管理器的位置
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(axeManager.transform.position, 0.5f);
        }
    }
}