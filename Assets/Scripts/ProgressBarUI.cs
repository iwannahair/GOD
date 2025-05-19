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
    [SerializeField] private GoldenTree goldenTree; // 黄金树引用
    
    private void Start()
    {
        // 尝试在场景中查找GoldenTree
        if (goldenTree == null)
        {
            goldenTree = FindObjectOfType<GoldenTree>();
        }
        
        if (goldenTree == null)
        {
            Debug.LogError("场景中没有找到GoldenTree！请确保GoldenTree已实例化到场景中。");
            return;
        }
        
        // 初始化进度条 - 使用公共属性TargetHumanCount
        UpdateProgressBar(0, goldenTree.TargetHumanCount);
        
        // 订阅黄金树的人类吸收事件
        goldenTree.onHumanAbsorbed.AddListener(UpdateProgressBar);
        
        Debug.Log("进度条已成功连接到GoldenTree");
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
}