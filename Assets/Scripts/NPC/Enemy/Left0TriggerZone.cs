using UnityEngine;

/// <summary>
/// Left0触发区域脚本
/// 此脚本应该附加到left_0子对象上，用于检测玩家进入和离开触发区域
/// </summary>
public class Left0TriggerZone : MonoBehaviour
{
    [Header("触发区域设置")]
    [Tooltip("BigEnemyAI组件的引用，通常是父对象上的组件")]
    [SerializeField] private BigEnemyAI bigEnemyAI;
    
    private void Awake()
    {
        // 如果没有手动设置BigEnemyAI引用，尝试从父对象获取
        if (bigEnemyAI == null)
        {
            bigEnemyAI = GetComponentInParent<BigEnemyAI>();
            if (bigEnemyAI == null)
            {
                Debug.LogError($"Left0TriggerZone: 无法找到BigEnemyAI组件！请确保此脚本附加到BigEnemy的子对象上，或手动设置BigEnemyAI引用。");
            }
        }
    }
    
    /// <summary>
    /// 当玩家进入left_0触发区域时调用
    /// </summary>
    /// <param name="other">进入触发区域的碰撞体</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家进入了left_0区域
        if (other.CompareTag("Player") && bigEnemyAI != null)
        {
            Debug.Log("Left0TriggerZone: 检测到玩家进入left_0区域");
            bigEnemyAI.OnPlayerEnterLeft0Area();
        }
    }
    
    /// <summary>
    /// 当玩家离开left_0触发区域时调用
    /// </summary>
    /// <param name="other">离开触发区域的碰撞体</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查是否是玩家离开了left_0区域
        if (other.CompareTag("Player") && bigEnemyAI != null)
        {
            Debug.Log("Left0TriggerZone: 检测到玩家离开left_0区域");
            bigEnemyAI.OnPlayerExitLeft0Area();
        }
    }
}