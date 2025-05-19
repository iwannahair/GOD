using UnityEngine;
using TMPro;

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
}