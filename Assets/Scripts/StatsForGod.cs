using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 添加场景管理命名空间

/// <summary>
/// 神的属性类：管理神的各种属性
/// </summary>
public class StatsForGod : MonoBehaviour
{
    // 单例模式，方便全局访问
    public static StatsForGod instance;
    
    [System.Serializable]
    public class Stat
    {
        public float percent;
        public TextMeshProUGUI percentText; // 引用TMP文本组件
    }
    
    public Stat health = new Stat();
    
    // 可配置的敌人类型标签，用于检测碰撞减少生命值
    [Header("敌人设置")]
    [Tooltip("当玩家与这些标签的敌人碰撞时，将减少生命值")]
    public string[] enemyTagsToDecreaseHealth = new string[] { "Enemy", "BigEnemy" };
    
    private void Awake()
    {
        // 设置单例
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 初始化显示
        UpdateHealthDisplay();
    }
    
    /// <summary>
    /// 增加生命值百分比
    /// </summary>
    /// <param name="amount">增加的百分比</param>
    public void AddHealthPercent(float amount)
    {
        health.percent += amount;
        Debug.Log($"神的生命值增加：+{amount}%，当前：{health.percent}%");
        
        // 更新UI显示
        UpdateHealthDisplay();
        
        // 检查生命值是否小于等于0
        CheckGameOver();
    }
    
    /// <summary>
    /// 减少生命值百分比
    /// </summary>
    /// <param name="amount">减少的百分比</param>
    public void DecreaseHealthPercent(float amount)
    {
        health.percent -= amount;
        Debug.Log($"神的生命值减少：-{amount}%，当前：{health.percent}%");
        
        // 更新UI显示
        UpdateHealthDisplay();
        
        // 检查生命值是否小于等于0
        CheckGameOver();
    }
    
    /// <summary>
    /// 当BigEnemy变成另一种预制体时调用，增加percentText的值
    /// </summary>
    public void OnBigEnemyTransformed()
    {
        AddHealthPercent(1);
        Debug.Log("BigEnemy变形，神的生命值+1");
    }
    
    /// <summary>
    /// 当玩家被敌人碰到时调用，减少percentText的值
    /// </summary>
    /// <param name="enemyTag">敌人的标签</param>
    /// <returns>如果是配置的敌人类型则返回true</returns>
    public bool OnPlayerHitByEnemy(string enemyTag)
    {
        // 检查是否是配置的敌人类型
        foreach (string tag in enemyTagsToDecreaseHealth)
        {
            if (tag == enemyTag)
            {
                DecreaseHealthPercent(1);
                Debug.Log($"玩家被{enemyTag}碰到，神的生命值-1");
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 更新生命值显示
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (health.percentText != null)
        {
            health.percentText.text = $"{health.percent}";
            Debug.Log($"更新TMP文本显示：{health.percent}%");
        }
        else
        {
            Debug.LogWarning("TMP文本组件未设置！");
        }
    }
    
    /// <summary>
    /// 检查游戏是否结束
    /// </summary>
    private void CheckGameOver()
    {
        if (health.percent <= 0)
        {
            Debug.LogWarning("神的生命值降至0，游戏结束！");
            // 延迟一帧加载开始场景，以便显示最终状态
            StartCoroutine(GameOverCoroutine());
        }
    }
    
    /// <summary>
    /// 游戏结束协程
    /// </summary>
    private System.Collections.IEnumerator GameOverCoroutine()
    {
        // 等待1秒，让玩家看到生命值为0的状态
        yield return new WaitForSeconds(1f);
        // 加载开始场景
        SceneManager.LoadScene("StartScene");
    }
}