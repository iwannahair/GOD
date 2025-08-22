using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("角色设置")]
    public GameObject playerPrefab;  // 保留Prefab引用但不使用

    private int playerHealth=100, playerDamage=100, playerAttackSpeed=100, spawnBigChanceInver = 90;
    [SerializeField] private TMP_Text playerDamageText, playerHealthText,  playerAttackSpeedText, popText;
    [SerializeField] private GameObject popTextGameObject;
    public event Action OnPlayerHealthChanged, OnPlayerDamageChanged, OnPlayerAttackSpeedChanged;
    private bool _endGame = false;
    
    #region PlayerThreeAtributesSetter/Getter

    [SerializeField] private float popOffset = 10f;

    
    
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            
            OnPlayerHealthChanged?.Invoke();
            CheckEndGame(playerHealth);
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                playerDamage = 0;
                playerAttackSpeed = 0;
            }
        }
        
    }

    

    public int PlayerDamage
    {
        get => playerDamage;
        set
        {
            playerDamage = value;
            OnPlayerDamageChanged?.Invoke();
            CheckEndGame(playerDamage);
            if (playerDamage <= 0)
            {
                playerDamage = 0;
                playerHealth = 0;
                playerAttackSpeed = 0;
            }
        }
    }

    public int PlayerAttackSpeed
    {
        get => playerAttackSpeed;
        set
        {
            if (playerAttackSpeed <= 0) return;
            playerAttackSpeed = value;
            if (playerAttackSpeed <= 0)
            {
                playerAttackSpeed = 0;
                playerHealth = 0;
                playerDamage = 0;
            }
            OnPlayerAttackSpeedChanged?.Invoke();
            CheckEndGame(playerAttackSpeed);
            
        }
    }
    private void CheckEndGame(int playerAttribute)
    {
        if (playerAttribute > 0) return;
        if (playerTran.TryGetComponent(out PlayerController playerController))
        {
            playerController.Die();
            _endGame = true;
        }
    }
    #endregion

    
    
    [Header("敌人生成器引用")]
    [SerializeField] private EnemySpawner enemySpawner; // 敌人生成器引用
    [SerializeField] private TMP_Text spawnWaveText; // 波次显示文本
    
    /// <summary>
    /// 当前波次属性，通过EnemySpawner获取并更新UI
    /// </summary>
    public int CurrentWave
    {
        get => enemySpawner != null ? enemySpawner.CurrentWave : 0;
    }

    [Header("人类设置")]
    public GameObject humanFollowerPrefab;
    private Transform humanFollowerTail;
    private Transform playerTran;
    private int enemiesKilled = 0;
    private const int KILLS_TO_SPAWN_FOLLOWER = 3;
    [SerializeField] private int currentFollowerNumber;
    [SerializeField] private int targetFollowerNumber = 10;
    public int humanSpawnSpeedUp {get; set; }
    private int speedUpContainer;
    private const int SPEED_UP_CONTAINER_MAX = 100;
    private int humanSpawnAmount, totalEnemiesKilled; //humanDied = humanSpawnAmount -  currentFollowerNumber; smallMonsterKilledAmount = enemyKilled - bigMonsterKilledAmount;
    public int BigMonsterKilledAmount { get; set; }
    public int BuildingBuilt { get; set; }

    [Header("UI设置")] 
    [SerializeField] private Slider currentFolNumSlider;  // 删除所有Card相关的变量和方法
 
    
    [Header("场景管理")] 
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winText,loseText;
    
 

    private Action OnFollowerUIChange;

    #region FollowerSliderUI
 
    public int CurrentFollowerNumber { get=>currentFollowerNumber;
        set
        {
             
            currentFollowerNumber = value;
            OnFollowerUIChange.Invoke();
        }
    }
    
    private void UpdateCurrentFollowerNumberUI()
    {
        float showingSliderValue = currentFollowerNumber%targetFollowerNumber; 
        if (currentFollowerNumber > 0&&showingSliderValue==0f) showingSliderValue = 10f;//if it's full, show full, maybe handle the reset in pop menu.
        currentFolNumSlider.value = showingSliderValue/targetFollowerNumber;
    }
    #endregion
    
    

    #region PlayerAtributeUI

    private void UpdateAttackDamageUI()
    {
        if (!playerDamageText) return;
        playerDamageText.text = playerDamage.ToString();
    }

    private void UpdateHealthUI()
    {
        if (!playerHealthText) return;
        playerHealthText.text = playerHealth.ToString();
    }

    private void UpdateAttackSpeedUI()
    {
        if (!playerAttackSpeedText) return;
        playerAttackSpeedText.text = playerAttackSpeed.ToString();
    }

    #endregion
    public Transform PlayerTran => playerTran;
    public void SetPlayerTran(Transform playerTran) => this.playerTran = playerTran;
    public Transform HumanFollowerTail{get=>humanFollowerTail;set=>humanFollowerTail=value;}
    /// <summary>
    /// 初始化单例模式，确保GameManager在场景切换时不被销毁
    /// </summary>
    private void Awake()
    {
        // 单例模式实现
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        }
        else
        {
            Destroy(gameObject); // 销毁重复的GameManager GameObject
            return;
        }
        
        // 注册UI更新事件
        OnFollowerUIChange+= UpdateCurrentFollowerNumberUI;
        OnPlayerHealthChanged += UpdateHealthUI;
        OnPlayerDamageChanged += UpdateAttackDamageUI;
        OnPlayerAttackSpeedChanged+= UpdateAttackSpeedUI;
    }
    
    private void Start()
    {
        // 移除了SpawnPlayer()调用
        // 查找玩家对象并获取其Transform组件
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTran = playerObject.transform;
            InitializeEnemySpawner(); // 初始化敌人生成器
        }
        else
        {
            Debug.LogError("未找到带有'Player'标签的游戏对象！请确保玩家对象已正确设置标签。");
        }
        
        // 订阅敌人生成器事件
        SubscribeToEnemySpawnerEvents();
        
        // 初始化小地图（已移除）
        // 小地图相关功能已移除
    }
    
    /// <summary>
    /// 订阅敌人生成器的事件
    /// </summary>
    private void SubscribeToEnemySpawnerEvents()
    {
        EnemySpawner.OnWaveChanged += UpdateWaveUI;
    }
    
    /// <summary>
    /// 取消订阅敌人生成器的事件，防止内存泄漏
    /// </summary>
    private void UnsubscribeFromEnemySpawnerEvents()
    {
        EnemySpawner.OnWaveChanged -= UpdateWaveUI;
    }
    
    /// <summary>
    /// 更新波次UI显示
    /// </summary>
    /// <param name="wave">当前波次</param>
    private void UpdateWaveUI(int wave)
    {
        if (spawnWaveText != null)
        {
            spawnWaveText.text = wave.ToString();
        }
    }
    
    /// <summary>
    /// 对象销毁时取消事件订阅
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeFromEnemySpawnerEvents();
    }
    
    #region EnemySpawner集成
    
    /// <summary>
    /// 获取溅射效果，委托给EnemySpawner处理
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <returns>溅射效果GameObject</returns>
    public GameObject GetSplash(Vector2 position)
    {
        if (enemySpawner != null)
        {
            return enemySpawner.GetSplash(position);
        }
        Debug.LogWarning("EnemySpawner引用为空，无法获取溅射效果");
        return null;
    }
    
    /// <summary>
    /// 返回溅射效果到对象池，委托给EnemySpawner处理
    /// </summary>
    /// <param name="splash">溅射效果GameObject</param>
    public void ReturnSplash(GameObject splash)
    {
        if (enemySpawner != null)
        {
            enemySpawner.ReturnSplash(splash);
        }
        else
        {
            Debug.LogWarning("EnemySpawner引用为空，无法返回溅射效果");
        }
    }
    
    /// <summary>
    /// 返回普通敌人到对象池，委托给EnemySpawner处理
    /// </summary>
    /// <param name="enemy">敌人GameObject</param>
    public void ReturnEnemy(GameObject enemy)
    {
        if (enemySpawner != null)
        {
            enemySpawner.ReturnEnemy(enemy);
        }
        else
        {
            Debug.LogWarning("EnemySpawner引用为空，无法返回敌人");
        }
    }
    
    /// <summary>
    /// 返回大敌人到对象池，委托给EnemySpawner处理
    /// </summary>
    /// <param name="enemy">大敌人GameObject</param>
    public void ReturnBigEnemy(GameObject enemy)
    {
        if (enemySpawner != null)
        {
            enemySpawner.ReturnBigEnemy(enemy);
        }
        else
        {
            Debug.LogWarning("EnemySpawner引用为空，无法返回大敌人");
        }
    }
    /// <summary>
    /// 初始化敌人生成器并生成初始敌人
    /// </summary>
    private void InitializeEnemySpawner()
    {
        // 检查玩家对象是否存在
        if (playerTran == null)
        {
            Debug.LogWarning("玩家对象不存在，无法初始化敌人生成器");
            return;
        }
        
        // 如果没有手动分配EnemySpawner，尝试查找或创建
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner == null)
            {
                // 创建新的EnemySpawner GameObject
                GameObject spawnerObject = new GameObject("EnemySpawner");
                enemySpawner = spawnerObject.AddComponent<EnemySpawner>();
                Debug.Log("自动创建了EnemySpawner组件");
                // 初始化新创建的敌人生成器
                enemySpawner.Initialize(playerTran);
            }
            else
            {
                // 找到已存在的EnemySpawner，更新玩家引用
                Debug.Log("找到已存在的EnemySpawner，正在更新玩家引用");
                enemySpawner.RefreshPlayerReference();
            }
        }
        else
        {
            // 手动分配的EnemySpawner，直接初始化
            enemySpawner.Initialize(playerTran);
        }
    }

    [SerializeField] private float endGameTimeToWait = 3f;
    Coroutine endGameCoroutine;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text endGame_HumanSpawned, endGame_HumanKilled, endGame_TotalEnemyKilled, endGame_BigEnemyKilled, endGame_SmallEnemyKilled, endGame_BuildingBuilt;
    void Update()
    {
        // 设置敌人生成器的游戏状态
        if (enemySpawner != null)
        {
            enemySpawner.SetGameActive(!_endGame);
        }
        
        InputPauseGame();
        if (!_endGame) return;
        if (endGameCoroutine==null)
        {
            endGameCoroutine = StartCoroutine(CheckIfThereIsEnemies());
        }
    }

    private IEnumerator CheckIfThereIsEnemies()
    { 
        while (true)
        {
            if (!GameObject.FindWithTag("Enemy"))
            {
                EndGameUI(true);
                break;
            }

            if (!GameObject.FindWithTag("Human")&&!GameObject.FindWithTag("Human"))
            {
                EndGameUI(false);
                break;
            }
            yield return new WaitForSeconds(endGameTimeToWait);
        }
    }

    private void EndGameUI(bool Win)
    {
        endGame_HumanSpawned.text = humanSpawnAmount.ToString();
        endGame_HumanKilled.text = (humanSpawnAmount - currentFollowerNumber).ToString();
        endGame_TotalEnemyKilled.text =  totalEnemiesKilled.ToString();
        endGame_BigEnemyKilled.text = BigMonsterKilledAmount.ToString();
        endGame_SmallEnemyKilled.text = (totalEnemiesKilled - BigMonsterKilledAmount).ToString();
        endGame_BuildingBuilt.text = BuildingBuilt.ToString();
        if (Win)
        {
            if (endGamePanel.TryGetComponent(out Image endGamePanelImage))
            {
                endGamePanelImage.color = new Color(0, 183/256f, 195/256f, 100/256f);
                winText.SetActive(true);
                loseText.SetActive(false);
            }
        }
        else
        {
            if (endGamePanel.TryGetComponent(out Image endGamePanelImage))
            {
                endGamePanelImage.color = new Color(195f/256f, 0, 0, 100/256f);
                winText.SetActive(false);
                loseText.SetActive(true);
            }
        }
        endGamePanel.SetActive(true);
         
    }

    private void InputPauseGame()
    {
        if (Input.GetButton("Cancel"))
        {
            pausePanel.SetActive(true);
        }
    }

    

    
    /// <summary>
    /// 获取当前场上敌人数量，委托给EnemySpawner处理
    /// </summary>
    /// <returns>当前场上敌人总数</returns>
    public int GetCurrentEnemyCount()
    {
        if (enemySpawner != null)
        {
            return enemySpawner.GetCurrentEnemyCount();
        }
        return 0;
    }
    
    /// <summary>
    /// 检测所有敌人是否已死亡，委托给EnemySpawner处理
    /// </summary>
    /// <returns>如果所有敌人都死亡返回true，否则返回false</returns>
    public bool AreAllEnemiesDead()
    {
        if (enemySpawner != null)
        {
            return enemySpawner.AreAllEnemiesDead();
        }
        return true;
    }
    
    public void OnEnemyKilled()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnEnemyKilled();
        }
        
        enemiesKilled++;
        totalEnemiesKilled++;
        if(enemiesKilled >= KILLS_TO_SPAWN_FOLLOWER)
        {
            
            
            enemiesKilled = 0; // 重置计数
        }
    }

   
    #endregion
}