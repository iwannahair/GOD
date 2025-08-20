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
    public Transform playerSpawnPoint;  // 保留生成点但不使用
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

    
    
    [Header("敌人设置")] 
    public GameObject enemyPrefab;
    public GameObject bigEnemyPrefab;
    public int initialEnemyCount = 10;
    public int enemiesPerWave = 5;
    public float spawnRadius = 10f;
    public float waveInterval = 5f;
    [SerializeField] private TMP_Text spawnWaveText;
    private int spawnWave;
    [SerializeField] private int enemyIncreasePerWave = 1;
    [SerializeField] private float waveIntervalIncreasePerWave = 0.3f;
    
    // 新增敌人生成控制变量
    [Header("敌人生成控制")]
    [SerializeField] private int maxEnemiesOnField = 5; // 场上最多普通敌人数量
    [SerializeField] private int maxBigEnemiesOnField = 1; // 场上最多大敌人数量
    [SerializeField] private bool allEnemiesDead = false; // 所有敌人是否已死亡
    
    private int SpawnWave
    {
        get => spawnWave;
        set
        {
            spawnWave = value;
            spawnWaveText.text = spawnWave.ToString();
        }
    }
    
    private float nextWaveTime;

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
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        OnFollowerUIChange+= UpdateCurrentFollowerNumberUI;
        OnPlayerHealthChanged += UpdateHealthUI;
        OnPlayerDamageChanged += UpdateAttackDamageUI;
        OnPlayerAttackSpeedChanged+= UpdateAttackSpeedUI;
    }
    
    private void Start()
    {
        // 移除了SpawnPlayer()调用
        playerTran = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnInitialEnemies();
        
        // 初始化小地图（已移除）
        // 小地图相关功能已移除
    }
    
    #region HandleSpawn

    [Header("Pool Settings")] 
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    private Queue<GameObject> bigEnemyPool = new Queue<GameObject>();
    public Queue<GameObject> splashPool = new Queue<GameObject>();
    [SerializeField] protected GameObject splashPrefab;
    public GameObject GetSplash(Vector2 _position)
    {
        if (splashPool.Count == 0)
        {
            GameObject newSplash = Instantiate(splashPrefab,_position, Quaternion.identity);
            newSplash.SetActive(false);
            splashPool.Enqueue(newSplash);
        }

        GameObject splash = splashPool.Dequeue();
        splash.transform.position = _position;
        splash.SetActive(true);
        return splash;
    }
    public void ReturnSplash(GameObject splash)
    {
        splash.SetActive(false);
        splashPool.Enqueue(splash);
    }
    //when spawnEnemy
    private GameObject GetEnemy(Vector2 _position)
    {
        if (enemyPool.Count == 0)
        {
            GameObject newEnemy = Instantiate(enemyPrefab,_position, Quaternion.identity);
            newEnemy.SetActive(false);
            enemyPool.Enqueue(newEnemy);
        }

        GameObject enemy = enemyPool.Dequeue();
        enemy.transform.position = _position;
        enemy.SetActive(true);
        return enemy;
    }
    private GameObject GetBigEnemy(Vector2 _position)
    {
        if (bigEnemyPool.Count == 0)
        {
            GameObject newEnemy = Instantiate(bigEnemyPrefab, _position, Quaternion.identity);
            newEnemy.SetActive(false);
            bigEnemyPool.Enqueue(newEnemy);
        }

        GameObject enemy = bigEnemyPool.Dequeue();
        enemy.transform.position = _position;
        enemy.SetActive(true);
        return enemy;
    }
    //when enemy killed
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
    public void ReturnBigEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        bigEnemyPool.Enqueue(enemy);
    }
    /// <summary>
    /// 生成初始敌人，固定生成5个Enemy和1个BigEnemy
    /// </summary>
    void SpawnInitialEnemies()
    {
        // 生成5个普通敌人
        for(int i = 0; i < maxEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetEnemy(spawnPos);
        }
        
        // 生成1个大敌人
        for(int i = 0; i < maxBigEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetBigEnemy(spawnPos);
        }
        
        allEnemiesDead = false; // 重置死亡标记
    }

    [SerializeField] private float endGameTimeToWait = 3f;
    Coroutine endGameCoroutine;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text endGame_HumanSpawned, endGame_HumanKilled, endGame_TotalEnemyKilled, endGame_BigEnemyKilled, endGame_SmallEnemyKilled, endGame_BuildingBuilt;
    void Update()
    {
        if(!_endGame)
        {
            CheckAndRespawnEnemies(); // 检查并重新生成敌人
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
    /// 检查并重新生成敌人，当所有敌人死亡时重新生成5个Enemy和1个BigEnemy
    /// </summary>
    void CheckAndRespawnEnemies()
    {
        // 检查所有敌人是否已死亡
        if (AreAllEnemiesDead() && !allEnemiesDead)
        {
            allEnemiesDead = true; // 标记所有敌人已死亡
            
            // 等待一小段时间后重新生成敌人
            StartCoroutine(RespawnEnemiesAfterDelay());
        }
    }
    
    /// <summary>
    /// 延迟重新生成敌人的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 等待2秒
        
        // 生成5个普通敌人
        for(int i = 0; i < maxEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetEnemy(spawnPos);
        }
        
        // 生成1个大敌人
        for(int i = 0; i < maxBigEnemiesOnField; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            GetBigEnemy(spawnPos);
        }
        
        allEnemiesDead = false; // 重置死亡标记
        SpawnWave++; // 增加波次计数
    }

    /// <summary>
    /// 获取当前场上敌人数量
    /// </summary>
    /// <returns>当前场上敌人总数</returns>
    private int GetCurrentEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bigEnemies = GameObject.FindGameObjectsWithTag("BigEnemy");
        return enemies.Length + bigEnemies.Length;
    }
    
    /// <summary>
    /// 检测所有敌人是否已死亡
    /// </summary>
    /// <returns>如果所有敌人都死亡返回true，否则返回false</returns>
    private bool AreAllEnemiesDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bigEnemies = GameObject.FindGameObjectsWithTag("BigEnemy");
        
        // 如果场上没有任何敌人，则认为所有敌人都死亡了
        return enemies.Length == 0 && bigEnemies.Length == 0;
    }
    
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        totalEnemiesKilled++;
        if(enemiesKilled >= KILLS_TO_SPAWN_FOLLOWER)
        {
            
            
            enemiesKilled = 0; // 重置计数
        }
    }

   
    #endregion
}