using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("角色设置")]
    public GameObject playerPrefab;  // 保留Prefab引用但不使用
    public Transform playerSpawnPoint;  // 保留生成点但不使用
    
    [Header("敌人设置")] 
    public GameObject enemyPrefab;
    public int initialEnemyCount = 5;
    public int enemiesPerWave = 5;
    public float spawnRadius = 10f;
    public float waveInterval = 5f;
    
    private float nextWaveTime;

    [Header("跟随设置")]
    public GameObject humanFollowerPrefab;
    private int enemiesKilled = 0;
    private const int KILLS_TO_SPAWN_FOLLOWER = 3;

    void Start()
    {
        // 移除了SpawnPlayer()调用
        SpawnInitialEnemies();
    }


    void SpawnInitialEnemies()
    {
        for(int i = 0; i < initialEnemyCount; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    void Update()
    {
        if(Time.time >= nextWaveTime)
        {
            SpawnEnemyWave();
            nextWaveTime = Time.time + waveInterval;
        }
    }

    void SpawnEnemyWave()
    {
        for(int i = 0; i < enemiesPerWave; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        
        if(enemiesKilled >= KILLS_TO_SPAWN_FOLLOWER)
        {
            SpawnFollower();
            enemiesKilled = 0; // 重置计数
        }
    }

    void SpawnFollower()
    {
        if(humanFollowerPrefab != null)
        {
            // 查找现有的跟随者
            FollowerAI[] existingFollowers = FindObjectsOfType<FollowerAI>();
            
            GameObject follower;
            if(existingFollowers.Length == 0)
            {
                // 第一个跟随者，跟随玩家
                follower = Instantiate(humanFollowerPrefab);
                follower.GetComponent<FollowerAI>().isFirstFollower = true;
            }
            else
            {
                // 后续跟随者，跟随最后一个跟随者
                Transform lastFollower = existingFollowers[existingFollowers.Length - 1].transform;
                Vector2 spawnPos = (Vector2)lastFollower.position - (Vector2)(lastFollower.up * 0.5f);
                follower = Instantiate(humanFollowerPrefab, spawnPos, lastFollower.rotation);
                follower.GetComponent<FollowerAI>().isFirstFollower = false;
                follower.GetComponent<FollowerAI>().target = lastFollower;
            }
        }
    }
}