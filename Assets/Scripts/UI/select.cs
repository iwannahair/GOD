using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject pauseMenuPanel; // 暂停菜单面板
    public Button button1; // 按钮1
    public Button button2; // 按钮2
    public Button button3; // 按钮3
    
    [Header("按钮文本")]
    public string button1Text = "选项1";
    public string button2Text = "选项2";
    public string button3Text = "选项3";
    
    [Header("设置")]
    public bool pauseTimeScale = true; // 是否暂停时间缩放
    public KeyCode resumeKey = KeyCode.Escape; // 恢复游戏的快捷键
    
    private bool isPaused = false;
    
    void Start()
    {
        // 初始化UI
        InitializeUI();
        
        // 确保游戏开始时面板是隐藏的
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // 检查恢复游戏的快捷键
        if (isPaused && Input.GetKeyDown(resumeKey))
        {
            ResumeGame();
        }
    }
    
    private void InitializeUI()
    {
        // 设置按钮文本
        if (button1 != null)
        {
            Text button1TextComponent = button1.GetComponentInChildren<Text>();
            if (button1TextComponent != null)
                button1TextComponent.text = button1Text;
            
            button1.onClick.AddListener(() => OnButtonClicked(1));
        }
        
        if (button2 != null)
        {
            Text button2TextComponent = button2.GetComponentInChildren<Text>();
            if (button2TextComponent != null)
                button2TextComponent.text = button2Text;
            
            button2.onClick.AddListener(() => OnButtonClicked(2));
        }
        
        if (button3 != null)
        {
            Text button3TextComponent = button3.GetComponentInChildren<Text>();
            if (button3TextComponent != null)
                button3TextComponent.text = button3Text;
            
            button3.onClick.AddListener(() => OnButtonClicked(3));
        }
    }
    
    /// <summary>
    /// 显示暂停菜单
    /// </summary>
    public void ShowPauseMenu()
    {
        if (isPaused) return;
        
        isPaused = true;
        
        // 暂停游戏
        if (pauseTimeScale)
        {
            Time.timeScale = 0f;
        }
        
        // 显示UI面板
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        Debug.Log("游戏已暂停，显示选择菜单");
    }
    
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        
        // 恢复游戏时间
        if (pauseTimeScale)
        {
            Time.timeScale = 1f;
        }
        
        // 隐藏UI面板
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        Debug.Log("游戏已恢复");
    }
    
    /// <summary>
    /// 按钮点击事件处理
    /// </summary>
    private void OnButtonClicked(int buttonNumber)
    {
        Debug.Log($"玩家点击了按钮 {buttonNumber}");
        
        // 在这里添加具体的按钮功能
        switch (buttonNumber)
        {
            case 1:
                HandleButton1Action();
                break;
            case 2:
                HandleButton2Action();
                break;
            case 3:
                HandleButton3Action();
                break;
        }
        
        // 点击任何按钮后恢复游戏
        ResumeGame();
    }
    
    /// <summary>
    /// 按钮1的具体功能
    /// </summary>
    private void HandleButton1Action()
    {
        Debug.Log("执行按钮1的功能");
        // 在这里添加按钮1的具体逻辑
        // 例如：增加玩家生命值、获得道具等
    }
    
    /// <summary>
    /// 按钮2的具体功能
    /// </summary>
    private void HandleButton2Action()
    {
        Debug.Log("执行按钮2的功能");
        // 在这里添加按钮2的具体逻辑
        // 例如：增加移动速度、获得技能等
    }
    
    /// <summary>
    /// 按钮3的具体功能
    /// </summary>
    private void HandleButton3Action()
    {
        Debug.Log("执行按钮3的功能");
        // 在这里添加按钮3的具体逻辑
        // 例如：获得金币、解锁新区域等
    }
    
    /// <summary>
    /// 设置按钮文本
    /// </summary>
    public void SetButtonTexts(string text1, string text2, string text3)
    {
        button1Text = text1;
        button2Text = text2;
        button3Text = text3;
        InitializeUI();
    }
    
    /// <summary>
    /// 检查游戏是否暂停
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
    
    /// <summary>
    /// 强制恢复游戏（用于其他脚本调用）
    /// </summary>
    public void ForceResume()
    {
        ResumeGame();
    }
}