using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 添加场景管理命名空间

/// <summary>
/// 神的属性类：管理神的各种属性
/// </summary>
public class StatsForGod : MonoBehaviour
{
    [System.Serializable]
    public class Stat
    {
        public float percent;
        public TextMeshProUGUI percentText; // 引用TMP文本组件
    }
    
    public Stat health = new Stat();
    
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