using System;
using System.Collections;
using CardEnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("角色设置")]
    public GameObject playerPrefab;  // 保留Prefab引用但不使用
    public Transform playerSpawnPoint;  // 保留生成点但不使用
    private int playerHealth=100, playerDamage=100, playerAttackSpeed=100;
    [SerializeField] private TMP_Text playerDamageText, playerHealthText,  playerAttackSpeedText, popText;
    [SerializeField] private GameObject popTextGameObject;
    public event Action OnPlayerHealthChanged, OnPlayerDamageChanged, OnPlayerAttackSpeedChanged;
    
    #region PlayerThreeAtributesSetter/Getter

    [SerializeField] private float popOffset = 10f;
    public void Pop(TypeEnum.AttributeType attributeType, int value)
    {
        switch (attributeType)
        {
            case TypeEnum.AttributeType.Health:
                popTextGameObject.transform.position = playerHealthText.transform.position+Vector3.right;
                break;
            case TypeEnum.AttributeType.Attack:
                popTextGameObject.transform.position = playerDamageText.transform.position+Vector3.right;
                break; 
            case TypeEnum.AttributeType.AttackSpeed:
                popTextGameObject.transform.position = playerAttackSpeedText.transform.position+Vector3.right;
                break;
            default:
                Debug.LogError("Unknown attribute type");
                break;
        }
        popText.text = value.ToString();
        popTextGameObject.SetActive(true);
    }
    
    
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            OnPlayerHealthChanged?.Invoke();
        }
        
    }
    
    public int PlayerDamage
    {
        get => playerDamage;
        set
        {
            playerDamage = value;
            OnPlayerDamageChanged?.Invoke();
            
        }
    }

    public int PlayerAttackSpeed
    {
        get => playerAttackSpeed;
        set
        {
            playerAttackSpeed = value;
            OnPlayerAttackSpeedChanged?.Invoke();
        }
    }
    #endregion

    
    
    [Header("敌人设置")] 
    public GameObject enemyPrefab;
    public GameObject bigEnemyPrefab;
    public int initialEnemyCount = 5;
    public int enemiesPerWave = 5;
    public float spawnRadius = 10f;
    public float waveInterval = 5f;
    
    private float nextWaveTime;

    [Header("人类设置")]
    public GameObject humanFollowerPrefab;
    private Transform humanFollowerTail;
    private Transform playerTran;
    private int enemiesKilled = 0;
    private const int KILLS_TO_SPAWN_FOLLOWER = 3;
    [SerializeField] private int currentFollowerNumber;
    [SerializeField] private int targetFollowerNumber = 10;
    [SerializeField] public int humanSpawnSpeedUp {get; set; }
    private int speedUpContainer;
    private const int SPEED_UP_CONTAINER_MAX = 100;

    [Header("UI设置")] 
    [SerializeField] private Slider curentFolNumSlider;
    [SerializeField] private Transform cardBuildingIndicator;
    [SerializeField] private float timeWaitToShowCards = 0.5f;
    [SerializeField] private GameObject cardSelectPanel;
    [SerializeField] private CardSelectionHandler cardSelectionHandler;
    [SerializeField] private HandLayout handLayout;
    public CardSelectionHandler GetCardSelectionHandler => cardSelectionHandler;
    
    [Header("场景管理")] 
    [SerializeField] private GameObject pausePanel;
    public Transform CardBuildingIndicator
    {
        get
        {
            cardBuildingIndicator.gameObject.SetActive(true);
            return cardBuildingIndicator;
        }
    }

    private Action OnFollowerUIChange;

    #region FollowerSliderUI
 
    public int CurrentFollowerNumber { get=>currentFollowerNumber;
        set
        {
            if (currentFollowerNumber < value&&value%targetFollowerNumber==0)
            {
                PopCardsSelect();//pop if adding and reach 10
            }
            currentFollowerNumber = value;
            OnFollowerUIChange.Invoke();
        }
    }
    
    private void UpdateCurrentFollowerNumberUI()
    {
        float showingSliderValue = currentFollowerNumber%targetFollowerNumber; 
        if (currentFollowerNumber > 0&&showingSliderValue==0f) showingSliderValue = 10f;//if it's full, show full, maybe handle the reset in pop menu.
        curentFolNumSlider.value = showingSliderValue/targetFollowerNumber;
    }
    #endregion
    
    #region CardUI
    public void PopCardsSelect()
    {
        //maybe sound go off first, then 0.5 sec -> pop UI 
        StartCoroutine(ShowCardsUI()); 
    }
    
    private IEnumerator ShowCardsUI()
    {
        yield return new WaitForSeconds(timeWaitToShowCards);
        cardSelectPanel.SetActive(true);
    }

    public void AddToHand(Card card)
    {
        if(!handLayout) return;
        handLayout.AddCardToHand(card);
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
    void Start()
    {
        // 移除了SpawnPlayer()调用
        playerTran = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnInitialEnemies();
    }
    
    #region HandleSpawn

    void SpawnInitialEnemies()
    {
        for(int i = 0; i < initialEnemyCount; i++)
        {
            Vector2 spawnPos = (Vector2)playerSpawnPoint.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            Instantiate(bigEnemyPrefab, spawnPos, Quaternion.identity);
            if (Random.Range(0,100)>90)
            {
                Instantiate(bigEnemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    void Update()
    {
        if(Time.time >= nextWaveTime)
        {
            SpawnEnemyWave();
            nextWaveTime = Time.time + waveInterval;
        }

        InputPauseGame();
    }

    private void InputPauseGame()
    {
        if (Input.GetButton("Cancel"))
        {
            pausePanel.SetActive(true);
        }
    }

    

    
    void SpawnEnemyWave()
    {
        for(int i = 0; i < enemiesPerWave; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + 
                              Random.insideUnitCircle.normalized * spawnRadius;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            if (Random.Range(0,100)>90)
            {
                Instantiate(bigEnemyPrefab, spawnPos, Quaternion.identity);
            }
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
        speedUpContainer += humanSpawnSpeedUp;
        if (speedUpContainer >= SPEED_UP_CONTAINER_MAX)
        {
            speedUpContainer-= SPEED_UP_CONTAINER_MAX + humanSpawnSpeedUp;
            SpawnFollower();
        }
            
        if(humanFollowerPrefab != null)
        {
            // 查找现有的跟随者
            
            
            GameObject follower;
            if(humanFollowerTail==null)
            {
                // 第一个跟随者，跟随玩家
                follower = Instantiate(humanFollowerPrefab, playerTran.position, playerTran.rotation);
                follower.GetComponent<FollowerAI>().target = playerTran;
                humanFollowerTail=follower.transform;
            }
            else
            {
                // 后续跟随者，跟随最后一个跟随者

                follower = Instantiate(humanFollowerPrefab, humanFollowerTail.position, humanFollowerTail.rotation);
                follower.GetComponent<FollowerAI>().target = humanFollowerTail;
                humanFollowerTail.GetComponent<FollowerAI>().nextFollower = follower.transform;
                humanFollowerTail = follower.transform;
            }

            CurrentFollowerNumber++;
        }
    }
    #endregion
}