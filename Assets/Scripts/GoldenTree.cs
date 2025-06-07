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
    [SerializeField] private float rotationSpeed = 1; // 旋转速度
    [SerializeField] private float rotationRadius = 2f; // 旋转半径
    [SerializeField] private float angleOffsetPerAxe = 30f; // 每把斧子之间的角度偏移量

    private List<GameObject> activeAxes = new List<GameObject>(); // 活跃的斧子列表
    private bool axesRotating = false; // 斧子是否正在旋转
    // 用于记录每个人类当前拥有多少斧子，以便计算角度偏移
    private Dictionary<HumanFollower, int> humanAxeCount = new Dictionary<HumanFollower, int>();

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
        axesRotating = true; 
        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 为当前跟随的人类添加斧子
        AddAxesToFollowingHumans();

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
        axesRotating = true; 
        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 为当前跟随的人类添加斧子
        AddAxesToFollowingHumans();

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
        axesRotating = true; 
        // 重置游戏完成状态和计数
        ResetCompletionState();

        // 为当前跟随的人类添加斧子
        AddAxesToFollowingHumans();

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
    private void ToggleAxesRotation() // 这个方法现在实际上总是添加斧子
    {
        // 移除旧的逻辑，直接调用添加斧子的方法
        StartAxesRotation(); 
    }

    /// <summary>
    /// 为所有当前跟随的人类添加斧子旋转效果
    /// </summary>
    private void AddAxesToFollowingHumans()
    {
        if (axePrefab == null)
        {
            Debug.LogWarning("斧子预制体未设置！");
            return;
        }

        HumanFollower[] humans = FindObjectsOfType<HumanFollower>();
        if (humans.Length == 0)
        {
            Debug.LogWarning("场景中没有人类！");
            return;
        }

        foreach (HumanFollower human in humans)
        {
            if (human != null && human.gameObject.activeSelf && human.IsFollowing())
            {
                AddAxeToSpecificHuman(human);
            }
        }
        axesRotating = activeAxes.Count > 0; // 更新斧子旋转状态
        Debug.Log("为所有跟随的人类添加了斧子");
    }

    /// <summary>
    /// 开始斧子旋转（现在这个方法主要被 AddAxesToFollowingHumans 和 AddAxeToHuman 调用）
    /// </summary>
    private void StartAxesRotation() // 修改：这个方法现在更像一个通用的添加斧子的逻辑
    {
        // 这个方法的功能已经大部分被 AddAxesToFollowingHumans 替代
        // 保留这个方法是为了 AddAxeToHuman 仍然可以工作，或者未来可能有其他地方直接调用
        // 但按钮点击现在应该调用 AddAxesToFollowingHumans
        AddAxesToFollowingHumans(); 
    }

    /// <summary>
    /// 停止斧子旋转并清除所有斧子
    /// </summary>
    private void StopAxesRotation() // 修改：这个方法现在只负责清除斧子
    {
        // 销毁所有活跃的斧子
        foreach (GameObject axe in activeAxes)
        {
            if (axe != null)
            {
                // 在销毁斧子前，获取其父对象（人类）
                HumanFollower human = axe.transform.parent.GetComponent<HumanFollower>();
                if (human != null && humanAxeCount.ContainsKey(human))
                {
                    humanAxeCount[human]--; // 减少该人类的斧子计数
                    if (humanAxeCount[human] <= 0)
                    {
                        humanAxeCount.Remove(human); // 如果计数为0，则移除记录
                    }
                }
                Destroy(axe);
            }
        }

        // 清空列表
        activeAxes.Clear();
        humanAxeCount.Clear(); // 同时清空斧子计数记录，因为所有斧子都没了
        axesRotating = false; // 当所有斧子被清除时，更新状态
        Debug.Log("所有斧子已停止旋转并被销毁");
    }

    /// <summary>
    /// 获取当前进度百分比
    /// </summary>
    /// <returns>进度百分比（0-1之间）</returns>
    public float GetProgressPercentage()
    {
        return (float)absorbedHumanCount / targetHumanCount;
    }

    // 添加公共属性，用于检查斧子是否正在旋转
    public bool AxesRotating => axesRotating;
    
    /// <summary>
    /// 为单个人类添加斧子效果
    /// </summary>
    /// <param name="human">要添加斧子的人类</param>
    public void AddAxeToHuman(HumanFollower human) // 修改：确保这个方法能独立运作或配合新的逻辑
    {
        if (axePrefab == null || human == null || !human.gameObject.activeSelf)
        {
            return;
        }
        
        AddAxeToSpecificHuman(human);
    }

    /// <summary>
    /// 为指定的人类添加斧子（内部辅助方法）
    /// </summary>
    private void AddAxeToSpecificHuman(HumanFollower human)
    {
        if (axePrefab == null || human == null || !human.gameObject.activeSelf) return;

        // 获取当前人类已有的斧子数量，用于计算角度偏移
        int currentAxeNum = 0;
        if (humanAxeCount.ContainsKey(human))
        {
            currentAxeNum = humanAxeCount[human];
        }
        else
        {
            humanAxeCount[human] = 0;
        }

        AxeAttack.InitAngle += angleOffsetPerAxe;
        GameObject axe = Instantiate(axePrefab, human.transform.position, Quaternion.identity);
        axe.GetComponent<AxeAttack>().InitAxe(rotationSpeed,rotationRadius);
        axe.transform.SetParent(human.transform);
 
        activeAxes.Add(axe);
  
        // 更新该人类的斧子数量
        humanAxeCount[human]++;
        
        Debug.Log($"为人类 {human.name} 创建了第 {humanAxeCount[human]} 把斧子，初始偏移角度: {AxeAttack.InitAngle}");
    }
} // This is the closing brace for the GoldenTree class

