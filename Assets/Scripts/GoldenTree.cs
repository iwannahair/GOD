using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI; // 添加UI命名空间引用
using UnityEngine.SceneManagement; // 添加场景管理命名空间引用

/// <summary>
/// 黄金树类：处理玩家与黄金树的交互
/// 当玩家碰撞到黄金树时，通知所有跟随的人类进入树中
/// </summary>
public class GoldenTree : MonoBehaviour
{
    [SerializeField] private float absorptionSpeed = 5f; // 吸收人类的速度
    [SerializeField] private StatsForGod statsForGod; // 引用神的属性类
    
    // 进度相关变量
    [SerializeField] private int targetHumanCount = 10; // 目标人类数量，达到这个数量视为完成
    private int absorbedHumanCount = 0; // 已吸收的人类数量
    
    // 添加公共属性来访问目标人类数量
    public int TargetHumanCount => targetHumanCount;
    
    // 事件系统，当人类被吸收时触发
    public UnityEvent<int, int> onHumanAbsorbed; // 参数：当前数量，目标数量
    
    // 添加新的事件，当完成面板的按钮被点击时触发
    public UnityEvent onCompletionPanelButtonClicked;

    private List<HumanFollower> humansToAbsorb = new List<HumanFollower>(); // 需要被吸收的人类列表
    private bool isAbsorbing = false; // 是否正在吸收人类
    
    [Header("游戏完成UI")]
    [SerializeField] private GameObject completionPanel; // 游戏完成面板
    [SerializeField] private Button continueButton; // 继续按钮
    [SerializeField] private Button restartButton; // 重新开始按钮
    [SerializeField] private Button quitButton; // 退出按钮
    
    private bool gameCompleted = false; // 游戏是否已完成
    
    private void Awake()
    {
        // 初始化事件
        if (onHumanAbsorbed == null)
            onHumanAbsorbed = new UnityEvent<int, int>();

        // 初始化新的事件
        if (onCompletionPanelButtonClicked == null)
            onCompletionPanelButtonClicked = new UnityEvent();

        // 确保完成面板初始隐藏
        if (completionPanel != null)
            completionPanel.SetActive(false);

        // 添加按钮事件监听
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    
    /// <summary>
    /// 碰撞触发器：检测玩家碰撞
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的是否是玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家碰撞到黄金树");
            
            // 查找所有跟随玩家的人类
            HumanFollower[] followers = FindObjectsOfType<HumanFollower>();
            foreach (HumanFollower follower in followers)
            {
                if (follower.IsFollowing())
                {
                    // 将人类添加到吸收列表
                    humansToAbsorb.Add(follower);
                    // 设置人类为无敌状态
                    follower.SetInvincible(true);
                }
            }
            
            // 开始吸收过程
            if (humansToAbsorb.Count > 0)
            {
                isAbsorbing = true;
                Debug.Log($"开始吸收{humansToAbsorb.Count}个人类");
            }
        }
    }
    
    /// <summary>
    /// 每帧更新：处理人类吸收逻辑
    /// </summary>
    private void Update()
    {
        if (!isAbsorbing || humansToAbsorb.Count == 0) return;
        
        List<HumanFollower> humansToRemove = new List<HumanFollower>();
        
        foreach (HumanFollower human in humansToAbsorb)
        {
            if (human == null)
            {
                humansToRemove.Add(human);
                continue;
            }
            
            // 计算人类到黄金树的方向
            Vector3 direction = (transform.position - human.transform.position).normalized;
            
            // 快速移动人类到黄金树
            human.transform.position += direction * absorptionSpeed * Time.deltaTime;
            
            // 检查人类是否已经足够接近黄金树
            float distance = Vector3.Distance(human.transform.position, transform.position);
            if (distance < 2f)
            {
                // 人类已到达黄金树，销毁人类
                Debug.Log("人类进入黄金树，消失");
                
                // 增加已吸收人类计数
                absorbedHumanCount++;
                
                // 增加神的生命值
                if (statsForGod != null)
                {
                    statsForGod.AddHealthPercent(1f);
                }
                
                // 触发人类被吸收事件，通知UI更新
                onHumanAbsorbed.Invoke(absorbedHumanCount, targetHumanCount);
                
                Debug.Log($"已吸收人类：{absorbedHumanCount}/{targetHumanCount}");
                
                Destroy(human.gameObject);
                humansToRemove.Add(human);

                // 检查是否达到目标数量，如果达到则立即触发完成逻辑
                CheckCompletion();
            }
        }
        
        // 从列表中移除已处理的人类
        foreach (HumanFollower human in humansToRemove)
        {
            humansToAbsorb.Remove(human);
        }
        
        // 如果所有人类都已处理完毕，结束吸收过程
        if (humansToAbsorb.Count == 0)
        {
            isAbsorbing = false;
            Debug.Log("所有人类已被黄金树吸收");
            
            // 检查是否达到目标数量
            CheckCompletion();
        }
    }
    
    /// <summary>
    /// 检查是否达到目标数量
    /// </summary>
    private void CheckCompletion()
    {
        if (!gameCompleted && absorbedHumanCount >= targetHumanCount)
        {
            gameCompleted = true;
            Debug.Log("达到目标人类数量，游戏完成！");
            
            // 暂停游戏
            Time.timeScale = 0f;
            
            // 显示完成面板
            if (completionPanel != null)
            {
                completionPanel.SetActive(true);
                Debug.Log("显示完成面板");
            }
            else
            {
                Debug.LogWarning("完成面板未设置！");
            }
        }
    }
    
    [Header("斧子旋转设置")]
    [SerializeField] private GameObject axePrefab; // 斧子预制体
    [SerializeField] private float rotationSpeed = 200f; // 旋转速度
    [SerializeField] private float rotationRadius = 2f; // 旋转半径
    
    private List<GameObject> activeAxes = new List<GameObject>(); // 活跃的斧子列表
    private bool axesRotating = false; // 斧子是否正在旋转
    
    /// <summary>
    /// 继续按钮点击事件
    /// </summary>
    private void OnContinueButtonClicked()
    {
        Debug.Log("点击继续按钮");
        // 隐藏面板
        completionPanel.SetActive(false);
        // 恢复游戏
        Time.timeScale = 1f;

        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 切换斧子旋转状态
        ToggleAxesRotation();

        // 触发完成面板按钮点击事件（用于UI更新）
        onCompletionPanelButtonClicked.Invoke();
    }

    /// <summary>
    /// 重新开始按钮点击事件
    /// </summary>
    private void OnRestartButtonClicked()
    {
        Debug.Log("点击重新开始按钮");
        // 隐藏面板
        completionPanel.SetActive(false); // 隐藏面板
        // 恢复时间缩放
        Time.timeScale = 1f;

        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 切换斧子旋转状态
        ToggleAxesRotation();

        // 触发完成面板按钮点击事件（用于UI更新）
        onCompletionPanelButtonClicked.Invoke();
    }

    /// <summary>
    /// 退出按钮点击事件
    /// </summary>
    private void OnQuitButtonClicked()
    {
        Debug.Log("点击退出按钮");
        // 隐藏面板
        completionPanel.SetActive(false); // 隐藏面板
        // 恢复时间缩放
        Time.timeScale = 1f;

        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 切换斧子旋转状态
        ToggleAxesRotation();

        // 触发完成面板按钮点击事件（用于UI更新）
        onCompletionPanelButtonClicked.Invoke();
    }

    /// <summary>
    /// 重置游戏完成状态和已吸收人类计数
    /// </summary>
    private void ResetCompletionState()
    {
        gameCompleted = false;
        absorbedHumanCount = 0;
        Debug.Log("游戏完成状态和计数已重置");
    }

    /// <summary>
    /// 切换斧子旋转状态
    /// </summary>
    private void ToggleAxesRotation()
    {
        if (axesRotating)
        {
            // 如果斧子正在旋转，停止旋转并销毁斧子
            StopAxesRotation();
        }
        else
        {
            // 如果斧子没有旋转，开始旋转
            StartAxesRotation();
        }
    }

    /// <summary>
    /// 开始斧子旋转
    /// </summary>
    private void StartAxesRotation()
    {
        if (axePrefab == null)
        {
            Debug.LogWarning("斧子预制体未设置！");
            return;
        }

        // 查找所有人类
        HumanFollower[] humans = FindObjectsOfType<HumanFollower>();
        if (humans.Length == 0)
        {
            Debug.LogWarning("场景中没有人类！");
            return;
        }

        // 为每个人类创建一个斧子
        foreach (HumanFollower human in humans)
        {
            if (human != null && human.gameObject.activeSelf)
            {
                // 创建斧子
                GameObject axe = Instantiate(axePrefab, human.transform.position, Quaternion.identity);
                // 设置斧子的父对象为人类，这样斧子会跟随人类移动
                axe.transform.SetParent(human.transform);
                // 设置初始位置（在人类右侧）
                axe.transform.localPosition = new Vector3(rotationRadius, 0, 0);
                // 添加到活跃斧子列表
                activeAxes.Add(axe);

                // 添加旋转组件
                AxeRotator rotator = axe.AddComponent<AxeRotator>();
                rotator.Initialize(rotationSpeed, rotationRadius);
            }
        }

        axesRotating = true;
        Debug.Log("斧子开始围绕人类旋转");
    }

    /// <summary>
    /// 停止斧子旋转
    /// </summary>
    private void StopAxesRotation()
    {
        // 销毁所有活跃的斧子
        foreach (GameObject axe in activeAxes)
        {
            if (axe != null)
            {
                Destroy(axe);
            }
        }

        // 清空列表
        activeAxes.Clear();
        axesRotating = false;
        Debug.Log("斧子停止旋转");
    }

    /// <summary>
    /// 获取当前进度百分比
    /// </summary>
    /// <returns>进度百分比（0-1之间）</returns>
    public float GetProgressPercentage()
    {
        return (float)absorbedHumanCount / targetHumanCount;
    }
}

/// <summary>
/// 斧子旋转器：控制斧子围绕中心点旋转
/// </summary>
public class AxeRotator : MonoBehaviour
{
    private float rotationSpeed; // 旋转速度
    private float radius; // 旋转半径
    private float currentAngle = 0f; // 当前角度

    /// <summary>
    /// 初始化旋转器
    /// </summary>
    /// <param name="speed">旋转速度</param>
    /// <param name="rotationRadius">旋转半径</param>
    public void Initialize(float speed, float rotationRadius)
    {
        rotationSpeed = speed;
        radius = rotationRadius;
    }

    /// <summary>
    /// 每帧更新：处理旋转逻辑
    /// </summary>
    private void Update()
    {
        // 更新角度
        currentAngle += rotationSpeed * Time.deltaTime;

        // 计算新位置
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;

        // 更新位置
        transform.localPosition = new Vector3(x, y, 0);

        // 更新旋转（斧子始终朝向旋转方向）
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}