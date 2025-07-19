using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionButtonManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject optionPanel; // 选项面板
    public Button[] optionButtons = new Button[3]; // 3个选项按钮
    public Text[] optionTexts = new Text[3]; // 按钮文本
    
    [Header("选项内容")]
    public string[] defaultOptionTexts = new string[3] 
    {
        "选项1：生命值+10",
        "选项2：攻击力+5", 
        "选项3：移动速度+2"
    };
    
    [Header("动画设置")]
    public float fadeInDuration = 0.3f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private InteractableObject currentInteractable;
    private CanvasGroup panelCanvasGroup;
    private bool isShowing = false;
    
    void Start()
    {
        InitializeUI();
        HideOptions();
    }
    
    void InitializeUI()
    {
        // 如果没有指定选项面板，尝试查找
        if (optionPanel == null)
        {
            optionPanel = transform.Find("OptionPanel")?.gameObject;
        }
        
        // 确保有CanvasGroup组件用于淡入淡出
        if (optionPanel != null)
        {
            panelCanvasGroup = optionPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = optionPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // 初始化按钮事件
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] != null)
            {
                int index = i; // 闭包变量
                optionButtons[i].onClick.AddListener(() => OnOptionButtonClicked(index));
                
                // 如果没有指定文本组件，尝试从按钮子对象中查找
                if (optionTexts[i] == null)
                {
                    optionTexts[i] = optionButtons[i].GetComponentInChildren<Text>();
                }
            }
        }
        
        // 设置默认文本
        UpdateOptionTexts(defaultOptionTexts);
    }
    
    public void ShowOptions(InteractableObject interactable)
    {
        if (isShowing) return;
        
        currentInteractable = interactable;
        isShowing = true;
        
        // 显示面板
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            
            // 播放淡入动画
            if (panelCanvasGroup != null)
            {
                StartCoroutine(FadeInPanel());
            }
        }
        
        // 暂停游戏（可选）
        Time.timeScale = 0f;
        
        Debug.Log("显示选项按钮");
    }
    
    public void HideOptions()
    {
        if (!isShowing) return;
        
        isShowing = false;
        currentInteractable = null;
        
        // 隐藏面板
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
        
        // 恢复游戏时间
        Time.timeScale = 1f;
        
        Debug.Log("隐藏选项按钮");
    }
    
    void OnOptionButtonClicked(int optionIndex)
    {
        Debug.Log($"点击了选项按钮 {optionIndex + 1}");
        
        // 通知当前交互物体
        if (currentInteractable != null)
        {
            currentInteractable.OnOptionSelected(optionIndex);
        }
        
        // 隐藏选项面板
        HideOptions();
    }
    
    void UpdateOptionTexts(string[] texts)
    {
        for (int i = 0; i < optionTexts.Length && i < texts.Length; i++)
        {
            if (optionTexts[i] != null)
            {
                optionTexts[i].text = texts[i];
            }
        }
    }
    
    // 淡入动画协程
    System.Collections.IEnumerator FadeInPanel()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 使用unscaledDeltaTime因为游戏可能暂停
            float progress = elapsedTime / fadeInDuration;
            float alpha = fadeInCurve.Evaluate(progress);
            
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = alpha;
            }
            
            yield return null;
        }
        
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f;
        }
    }
    
    // 设置自定义选项文本
    public void SetCustomOptions(string[] customTexts)
    {
        if (customTexts != null && customTexts.Length > 0)
        {
            UpdateOptionTexts(customTexts);
        }
    }
    
    // 检查是否正在显示选项
    public bool IsShowingOptions()
    {
        return isShowing;
    }
    
    void Update()
    {
        // ESC键取消选择（可选功能）
        if (isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            HideOptions();
        }
    }
}