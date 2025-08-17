using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 进度条UI管理器：负责显示和更新进度条
/// </summary>
public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider; // 进度条滑块组件
    [SerializeField] private TextMeshProUGUI progressText; // 进度文本组件（可选）
    [SerializeField] private int targetCount = 10; // 目标数量
    
    private int currentCount = 0; // 当前数量

    private void Start()
    {
        // 初始化进度条
        UpdateProgressBar(0, targetCount);
        Debug.Log("进度条已初始化");
    }

    /// <summary>
    /// 更新进度条显示
    /// </summary>
    /// <param name="current">当前数量</param>
    /// <param name="target">目标数量</param>
    public void UpdateProgressBar(int current, int target)
    {
        // 更新滑块值
        if (progressSlider != null)
        {
            progressSlider.value = (float)current / target;
        }

        // 更新文本显示（如果有）
        if (progressText != null)
        {
            progressText.text = $"{current} / {target}";
        }

        Debug.Log($"进度条更新：{current}/{target}");
    }

    /// <summary>
    /// 重置进度条显示
    /// </summary>
    public void ResetProgressBarDisplay()
    {
        Debug.Log("重置进度条显示");
        currentCount = 0;
        UpdateProgressBar(currentCount, targetCount);
    }
    
    /// <summary>
    /// 增加当前进度
    /// </summary>
    public void IncrementProgress()
    {
        currentCount++;
        UpdateProgressBar(currentCount, targetCount);
    }
}