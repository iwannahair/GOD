using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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
    
    private List<HumanFollower> humansToAbsorb = new List<HumanFollower>(); // 需要被吸收的人类列表
    private bool isAbsorbing = false; // 是否正在吸收人类
    
    private void Awake()
    {
        // 初始化事件
        if (onHumanAbsorbed == null)
            onHumanAbsorbed = new UnityEvent<int, int>();
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
        }
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