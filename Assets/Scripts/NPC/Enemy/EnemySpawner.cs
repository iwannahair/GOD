using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人生成管理器，负责敌人的对象池管理、生成逻辑和重生机制
/// 从GameManager中拆分出来，实现更好的代码模块化
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("敌人预制体设置")]
    [SerializeField] private GameObject enemyPrefab; // 普通敌人预制体
    [SerializeField] private GameObject bigEnemyPrefab; // 大敌人预制体
    [SerializeField] private GameObject splashPrefab; // 溅射效果预制体
    
    [Header("敌人生成控制")]
    [SerializeField] private int maxEnemiesOnField = 5; // 场上最多普通敌人数量
    [SerializeField] private int maxBigEnemiesOnField = 1; // 场上最多大敌人数量
    [SerializeField] private float spawnRadius = 10f; // 生成半径
    [SerializeField] private bool allEnemiesDead = false; // 所有敌人是否已死亡
    
    [Header("对象池设置")]
    private Queue<GameObject> enemyPool = new Queue<GameObject>(); // 普通敌人对象池
    private Queue<GameObject> bigEnemyPool = new Queue<GameObject>(); // 大敌人对象池
    private Queue<GameObject> splashPool = new Queue<GameObject>(); // 溅射效果对象池
    
    // 事件系统
    public static event Action OnWaveCompleted; // 波次完成事件
    public static event Action<int> OnWaveChanged; // 波次变化事件
    
    private int currentWave = 0; // 当前波次
    public Transform playerTransform; // 玩家Transform引用
    private bool isGameActive = true; // 游戏是否激活状态
    
    #region 单例模式
    public static EnemySpawner Instance { get; private set; }
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 监听场景加载事件，用于在场景重新加载时刷新玩家引用
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// 场景加载完成时的回调，用于刷新玩家引用
    /// </summary>
    /// <param name="scene">加载的场景</param>
    /// <param name="mode">加载模式</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 延迟一帧执行，确保场景中的对象已完全初始化
        StartCoroutine(RefreshPlayerReferenceDelayed());
    }
    
    /// <summary>
    /// 延迟刷新玩家引用的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshPlayerReferenceDelayed()
    {
        yield return null; // 等待一帧
        RefreshPlayerReference();
    }
    #endregion
    
    #region 初始化
    /// <summary>
    /// 初始化敌人生成器
    /// </summary>
    /// <param name="playerTran">玩家Transform引用</param>
    public void Initialize(Transform playerTran)
    {
        playerTransform = playerTran;
        isGameActive = true;
        
        // 生成初始敌人
        SpawnInitialEnemies();
    }
    
    /// <summary>
    /// 重新查找玩家对象，用于场景重新加载后更新玩家引用
    /// 同时重新生成初始敌人，确保场景重新加载后敌人正常生成
    /// </summary>
    public void RefreshPlayerReference()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            Debug.Log("EnemySpawner: 已重新找到玩家对象并更新引用");
            
            // 场景重新加载后，重新生成初始敌人
            SpawnInitialEnemies();
            Debug.Log("EnemySpawner: 场景重新加载后已重新生成初始敌人");
        }
        else
        {
            Debug.LogWarning("EnemySpawner: 场景重新加载后未找到玩家对象");
        }
    }
    
    /// <summary>
    /// 设置游戏状态
    /// </summary>
    /// <param name="active">是否激活</param>
    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }
    
    /// <summary>
    /// 每帧检查敌人重生逻辑
    /// </summary>
    private void Update()
    {
        if (isGameActive)
        {
            CheckAndRespawnEnemies();
        }
    }
    #endregion
    
    #region 对象池管理
    /// <summary>
    /// 从对象池获取溅射效果
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <returns>溅射效果GameObject</returns>
    public GameObject GetSplash(Vector2 position)
    {
        // 清理对象池中已被销毁的对象
        while (splashPool.Count > 0)
        {
            GameObject pooledSplash = splashPool.Peek();
            if (pooledSplash == null)
            {
                splashPool.Dequeue(); // 移除空引用
            }
            else
            {
                break; // 找到有效对象，退出循环
            }
        }
        
        // 如果对象池为空或没有有效对象，创建新对象
        if (splashPool.Count == 0)
        {
            GameObject newSplash = Instantiate(splashPrefab, position, Quaternion.identity);
            newSplash.SetActive(false);
            splashPool.Enqueue(newSplash);
        }

        GameObject splash = splashPool.Dequeue();
        // 再次检查对象是否有效
        if (splash == null)
        {
            // 如果对象无效，创建新对象
            splash = Instantiate(splashPrefab, position, Quaternion.identity);
        }
        
        splash.transform.position = position;
        splash.SetActive(true);
        return splash;
    }
    
    /// <summary>
    /// 将溅射效果返回对象池
    /// </summary>
    /// <param name="splash">溅射效果GameObject</param>
    public void ReturnSplash(GameObject splash)
    {
        // 检查对象是否有效
        if (splash != null)
        {
            splash.SetActive(false);
            splashPool.Enqueue(splash);
        }
    }
    
    /// <summary>
    /// 从对象池获取普通敌人
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <returns>敌人GameObject</returns>
    private GameObject GetEnemy(Vector2 position)
    {
        // 清理对象池中已被销毁的对象
        while (enemyPool.Count > 0)
        {
            GameObject pooledEnemy = enemyPool.Peek();
            if (pooledEnemy == null)
            {
                enemyPool.Dequeue(); // 移除空引用
            }
            else
            {
                break; // 找到有效对象，退出循环
            }
        }
        
        // 如果对象池为空或没有有效对象，创建新对象
        if (enemyPool.Count == 0)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            newEnemy.SetActive(false);
            enemyPool.Enqueue(newEnemy);
        }

        GameObject enemy = enemyPool.Dequeue();
        // 再次检查对象是否有效
        if (enemy == null)
        {
            // 如果对象无效，创建新对象
            enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        }
        
        enemy.transform.position = position;
        enemy.SetActive(true);
        return enemy;
    }
    
    /// <summary>
    /// 从对象池获取大敌人
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <returns>大敌人GameObject</returns>
    private GameObject GetBigEnemy(Vector2 position)
    {
        // 清理对象池中已被销毁的对象
        while (bigEnemyPool.Count > 0)
        {
            GameObject pooledEnemy = bigEnemyPool.Peek();
            if (pooledEnemy == null)
            {
                bigEnemyPool.Dequeue(); // 移除空引用
            }
            else
            {
                break; // 找到有效对象，退出循环
            }
        }
        
        // 如果对象池为空或没有有效对象，创建新对象
        if (bigEnemyPool.Count == 0)
        {
            GameObject newEnemy = Instantiate(bigEnemyPrefab, position, Quaternion.identity);
            newEnemy.SetActive(false);
            bigEnemyPool.Enqueue(newEnemy);
        }

        GameObject enemy = bigEnemyPool.Dequeue();
        // 再次检查对象是否有效
        if (enemy == null)
        {
            // 如果对象无效，创建新对象
            enemy = Instantiate(bigEnemyPrefab, position, Quaternion.identity);
        }
        
        enemy.transform.position = position;
        enemy.SetActive(true);
        return enemy;
    }
    
    /// <summary>
    /// 将普通敌人返回对象池
    /// </summary>
    /// <param name="enemy">敌人GameObject</param>
    public void ReturnEnemy(GameObject enemy)
    {
        // 检查对象是否有效
        if (enemy != null)
        {
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }
    
    /// <summary>
    /// 将大敌人返回对象池
    /// </summary>
    /// <param name="enemy">大敌人GameObject</param>
    public void ReturnBigEnemy(GameObject enemy)
    {
        // 检查对象是否有效
        if (enemy != null)
        {
            enemy.SetActive(false);
            bigEnemyPool.Enqueue(enemy);
        }
    }
    #endregion
    
    #region 敌人生成逻辑
    /// <summary>
    /// 生成初始敌人，固定生成指定数量的Enemy和BigEnemy
    /// 基于玩家当前位置生成敌人
    /// </summary>
    public void SpawnInitialEnemies()
    {
        // 检查玩家对象是否存在
        if (playerTransform == null)
        {
            Debug.LogWarning("玩家对象不存在，无法生成初始敌人");
            return;
        }
        
        // 生成普通敌人
        for(int i = 0; i < maxEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerTransform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetEnemy(spawnPos);
        }
        
        // 生成大敌人
        for(int i = 0; i < maxBigEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerTransform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetBigEnemy(spawnPos);
        }
        
        allEnemiesDead = false; // 重置死亡标记
        currentWave = 1; // 设置初始波次
        OnWaveChanged?.Invoke(currentWave); // 触发波次变化事件
    }
    
    /// <summary>
    /// 检查并重新生成敌人，当所有敌人死亡时重新生成
    /// </summary>
    public void CheckAndRespawnEnemies()
    {
        if (!isGameActive) return;
        
        // 检查所有敌人是否已死亡
        if (AreAllEnemiesDead() && !allEnemiesDead)
        {
            allEnemiesDead = true; // 标记所有敌人已死亡
            OnWaveCompleted?.Invoke(); // 触发波次完成事件
            
            // 等待一小段时间后重新生成敌人
            StartCoroutine(RespawnEnemiesAfterDelay());
        }
    }
    
    /// <summary>
    /// 延迟重新生成敌人的协程
    /// 基于玩家当前位置重新生成敌人
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 等待2秒
        
        // 检查玩家对象是否仍然存在
        if (playerTransform == null)
        {
            Debug.LogWarning("玩家对象已被销毁，无法重新生成敌人");
            yield break; // 提前退出协程
        }
        
        // 生成普通敌人
        for(int i = 0; i < maxEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerTransform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetEnemy(spawnPos);
        }
        
        // 生成大敌人
        for(int i = 0; i < maxBigEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerTransform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetBigEnemy(spawnPos);
        }
        
        allEnemiesDead = false; // 重置死亡标记
        currentWave++; // 增加波次计数
        OnWaveChanged?.Invoke(currentWave); // 触发波次变化事件
    }
    #endregion
    
    #region 敌人状态检测
    /// <summary>
    /// 获取当前场上敌人数量
    /// </summary>
    /// <returns>当前场上敌人总数</returns>
    public int GetCurrentEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bigEnemies = GameObject.FindGameObjectsWithTag("BigEnemy");
        return enemies.Length + bigEnemies.Length;
    }
    
    /// <summary>
    /// 检测所有敌人是否已死亡
    /// </summary>
    /// <returns>如果所有敌人都死亡返回true，否则返回false</returns>
    public bool AreAllEnemiesDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bigEnemies = GameObject.FindGameObjectsWithTag("BigEnemy");
        
        // 如果场上没有任何敌人，则认为所有敌人都死亡了
        return enemies.Length == 0 && bigEnemies.Length == 0;
    }
    #endregion
    
    #region 敌人死亡处理
    /// <summary>
    /// 敌人死亡时调用的方法
    /// 处理敌人死亡相关的逻辑
    /// </summary>
    public void OnEnemyKilled()
    {
        // 这里可以添加敌人死亡时的特殊逻辑
        // 比如统计击杀数、触发特殊事件等
        // 目前主要的重生逻辑在CheckAndRespawnEnemies中处理
    }
    #endregion
    
    #region 属性访问器
    /// <summary>
    /// 获取当前波次
    /// </summary>
    public int CurrentWave => currentWave;
    
    /// <summary>
    /// 获取最大普通敌人数量
    /// </summary>
    public int MaxEnemiesOnField => maxEnemiesOnField;
    
    /// <summary>
    /// 获取最大大敌人数量
    /// </summary>
    public int MaxBigEnemiesOnField => maxBigEnemiesOnField;
    
    /// <summary>
    /// 设置最大普通敌人数量
    /// </summary>
    /// <param name="count">数量</param>
    public void SetMaxEnemiesOnField(int count)
    {
        maxEnemiesOnField = Mathf.Max(0, count);
    }
    
    /// <summary>
    /// 设置最大大敌人数量
    /// </summary>
    /// <param name="count">数量</param>
    public void SetMaxBigEnemiesOnField(int count)
    {
        maxBigEnemiesOnField = Mathf.Max(0, count);
    }
    #endregion
    
    #region Unity生命周期
    private void OnDestroy()
    {
        // 清理事件订阅，防止内存泄漏
        OnWaveCompleted = null;
        OnWaveChanged = null;
        
        // 移除场景加载事件监听
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
}