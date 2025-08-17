using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 斧子管理器 - 负责管理围绕玩家旋转的斧子系统
/// 功能包括：斧子生成、旋转控制、数量管理、半径调整
/// </summary>
public class AxeManager : MonoBehaviour
{
    [Header("斧子配置")]
    [SerializeField] private GameObject axePrefab; // 斧子预制体
    [SerializeField] private int initialAxeCount = 1; // 初始斧子数量
    [SerializeField] private int maxAxeCount = 8; // 最大斧子数量
    
    [Header("旋转设置")]
    [SerializeField] private float rotationSpeed = 90f; // 旋转速度（度/秒）
    [SerializeField] private float orbitRadius = 3f; // 轨道半径
    [SerializeField] private bool clockwise = true; // 是否顺时针旋转
    
    [Header("动态调整")]
    [SerializeField] private float radiusChangeSpeed = 2f; // 半径变化速度
    [SerializeField] private float minRadius = 2f; // 最小半径
    [SerializeField] private float maxRadius = 6f; // 最大半径
    
    // 私有变量
    private List<GameObject> axes = new List<GameObject>(); // 斧子列表
    private Transform playerTransform; // 玩家Transform引用
    private float currentRotationAngle = 0f; // 当前旋转角度
    private float targetRadius; // 目标半径
    private float currentRadius; // 当前半径
    
    /// <summary>
    /// 初始化斧子管理器
    /// </summary>
    private void Start()
    {
        // 获取玩家Transform引用
        playerTransform = transform;
        
        // 初始化半径
        targetRadius = orbitRadius;
        currentRadius = orbitRadius;
        
        // 生成初始斧子
        for (int i = 0; i < initialAxeCount; i++)
        {
            CreateAxe();
        }
        
        // 订阅游戏管理器事件（如果需要根据玩家属性调整）
        if (GameManager.instance != null)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged += UpdateRotationSpeed;
        }
    }
    
    /// <summary>
    /// 每帧更新斧子位置和旋转
    /// </summary>
    private void Update()
    {
        if (playerTransform == null || axes.Count == 0) return;
        
        // 更新旋转角度
        float rotationDelta = rotationSpeed * Time.deltaTime;
        if (!clockwise) rotationDelta = -rotationDelta;
        currentRotationAngle += rotationDelta;
        
        // 保持角度在0-360度范围内
        if (currentRotationAngle >= 360f) currentRotationAngle -= 360f;
        if (currentRotationAngle < 0f) currentRotationAngle += 360f;
        
        // 平滑调整半径
        if (Mathf.Abs(currentRadius - targetRadius) > 0.01f)
        {
            currentRadius = Mathf.Lerp(currentRadius, targetRadius, radiusChangeSpeed * Time.deltaTime);
        }
        
        // 更新所有斧子的位置
        UpdateAxesPositions();
    }
    
    /// <summary>
    /// 更新所有斧子的位置
    /// </summary>
    private void UpdateAxesPositions()
    {
        for (int i = 0; i < axes.Count; i++)
        {
            if (axes[i] == null) continue;
            
            // 计算每个斧子的角度偏移
            float angleOffset = (360f / axes.Count) * i;
            float totalAngle = currentRotationAngle + angleOffset;
            
            // 转换为弧度
            float radianAngle = totalAngle * Mathf.Deg2Rad;
            
            // 计算位置
            Vector3 offset = new Vector3(
                Mathf.Cos(radianAngle) * currentRadius,
                Mathf.Sin(radianAngle) * currentRadius,
                0f
            );
            
            // 设置斧子位置（相对于玩家）
            axes[i].transform.position = playerTransform.position + offset;
            
            // 可选：让斧子朝向运动方向
            axes[i].transform.rotation = Quaternion.Euler(0, 0, totalAngle);
        }
    }
    
    /// <summary>
    /// 创建新的斧子
    /// </summary>
    /// <returns>创建的斧子GameObject，如果创建失败返回null</returns>
    public GameObject CreateAxe()
    {
        if (axes.Count >= maxAxeCount)
        {
            Debug.LogWarning("已达到最大斧子数量限制: " + maxAxeCount);
            return null;
        }
        
        if (axePrefab == null)
        {
            Debug.LogError("斧子预制体未设置！");
            return null;
        }
        
        // 实例化斧子
        GameObject newAxe = Instantiate(axePrefab, playerTransform.position, Quaternion.identity);
        
        // 设置斧子的父对象（可选，用于组织层级）
        newAxe.transform.SetParent(transform);
        
        // 添加到列表
        axes.Add(newAxe);
        
        // 如果斧子有AxeAttack组件，进行初始化
        if (newAxe.TryGetComponent<AxeAttack>(out AxeAttack axeAttack))
        {
            // 设置初始角度，避免所有斧子重叠
            float initialAngle = (360f / (axes.Count)) * (axes.Count - 1);
            AxeAttack.InitAngle = initialAngle;
            axeAttack.InitAxe(1f, currentRadius);
        }
        
        Debug.Log($"创建斧子成功，当前斧子数量: {axes.Count}");
        return newAxe;
    }
    
    /// <summary>
    /// 移除指定的斧子
    /// </summary>
    /// <param name="axe">要移除的斧子GameObject</param>
    public void RemoveAxe(GameObject axe)
    {
        if (axes.Contains(axe))
        {
            axes.Remove(axe);
            Destroy(axe);
            Debug.Log($"移除斧子成功，当前斧子数量: {axes.Count}");
        }
    }
    
    /// <summary>
    /// 移除最后一个斧子
    /// </summary>
    public void RemoveLastAxe()
    {
        if (axes.Count > 0)
        {
            GameObject lastAxe = axes[axes.Count - 1];
            RemoveAxe(lastAxe);
        }
    }
    
    /// <summary>
    /// 设置斧子数量
    /// </summary>
    /// <param name="count">目标斧子数量</param>
    public void SetAxeCount(int count)
    {
        count = Mathf.Clamp(count, 0, maxAxeCount);
        
        while (axes.Count < count)
        {
            CreateAxe();
        }
        
        while (axes.Count > count)
        {
            RemoveLastAxe();
        }
    }
    
    /// <summary>
    /// 设置旋转速度
    /// </summary>
    /// <param name="speed">新的旋转速度</param>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Max(0f, speed);
    }
    
    /// <summary>
    /// 设置轨道半径
    /// </summary>
    /// <param name="radius">新的轨道半径</param>
    public void SetOrbitRadius(float radius)
    {
        targetRadius = Mathf.Clamp(radius, minRadius, maxRadius);
    }
    
    /// <summary>
    /// 切换旋转方向
    /// </summary>
    public void ToggleRotationDirection()
    {
        clockwise = !clockwise;
    }
    
    /// <summary>
    /// 根据游戏管理器的攻击速度更新旋转速度
    /// </summary>
    private void UpdateRotationSpeed()
    {
        if (GameManager.instance != null)
        {
            // 根据玩家攻击速度调整旋转速度
            float speedMultiplier = GameManager.instance.PlayerAttackSpeed / 100f;
            SetRotationSpeed(90f * speedMultiplier); // 基础速度90度/秒
        }
    }
    
    /// <summary>
    /// 获取当前斧子数量
    /// </summary>
    /// <returns>当前斧子数量</returns>
    public int GetAxeCount()
    {
        return axes.Count;
    }
    
    /// <summary>
    /// 获取当前旋转速度
    /// </summary>
    /// <returns>当前旋转速度</returns>
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }
    
    /// <summary>
    /// 获取所有斧子的引用
    /// </summary>
    /// <returns>斧子GameObject列表的只读副本</returns>
    public List<GameObject> GetAllAxes()
    {
        return new List<GameObject>(axes);
    }
    
    /// <summary>
    /// 清理资源
    /// </summary>
    private void OnDestroy()
    {
        // 取消订阅事件
        if (GameManager.instance != null)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged -= UpdateRotationSpeed;
        }
        
        // 清理所有斧子
        foreach (GameObject axe in axes)
        {
            if (axe != null)
            {
                Destroy(axe);
            }
        }
        axes.Clear();
    }
    
    /// <summary>
    /// 在编辑器中绘制轨道半径的可视化辅助线
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) playerTransform = transform;
        
        // 绘制轨道圆圈
        // 绘制当前轨道半径
        Gizmos.color = Color.yellow;
        DrawWireCircle(playerTransform.position, currentRadius);
        
        // 绘制最小和最大半径
        Gizmos.color = Color.red;
        DrawWireCircle(playerTransform.position, minRadius);
        Gizmos.color = Color.green;
        DrawWireCircle(playerTransform.position, maxRadius);
    }
    
    /// <summary>
    /// 绘制线框圆圈的辅助方法
    /// </summary>
    /// <param name="center">圆心位置</param>
    /// <param name="radius">半径</param>
    private void DrawWireCircle(Vector3 center, float radius)
    {
        const int segments = 32; // 圆圈分段数
        float angleStep = 360f / segments;
        
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 currentPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
            
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }
}