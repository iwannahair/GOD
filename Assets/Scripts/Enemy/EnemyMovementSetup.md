# 敌人自动移动系统配置指南

## 概述
EnemyMovement脚本让敌人能够自动朝着场景中的黄金树方向移动，包含智能寻路、速度控制、精灵翻转等功能。

## 配置步骤

### 1. 添加脚本到敌人预制体
1. 在Project面板中找到敌人预制体（如Enemy.prefab、BigEnemy.prefab）
2. 双击打开预制体编辑模式
3. 将EnemyMovement.cs脚本拖拽到敌人对象上
4. 确保敌人对象有Rigidbody2D组件（脚本会自动添加）

### 2. 设置黄金树标签
1. 在场景中选择黄金树对象
2. 在Inspector面板顶部的Tag下拉菜单中选择"GoldenTree"
3. 如果没有"GoldenTree"标签，点击"Add Tag..."创建新标签

### 3. 调整移动参数

#### 基本参数
- **Move Speed**: 敌人移动速度（建议2-5）
- **Stopping Distance**: 距离目标的停止距离（建议0.5-2）
- **Smooth Time**: 移动平滑度（建议0.1-0.5）

#### 目标设置
- **Target Tag**: 目标对象的标签（默认"GoldenTree"）
- **Target Layer**: 目标对象的层级（默认-1表示所有层级）

### 4. 添加碰撞检测
确保敌人预制体有以下组件：
- **Rigidbody2D**: 设置为Kinematic或Dynamic
- **Collider2D**: 根据精灵形状选择合适的碰撞器
- **EnemyHealth**: 处理生命值和死亡逻辑

## 使用说明

### 运行时行为
- 敌人生成后会自动寻找黄金树
- 持续向黄金树方向移动
- 到达停止距离后停止移动
- 根据移动方向自动翻转精灵

### 动态调整
```csharp
// 在游戏运行时修改移动速度
EnemyMovement movement = GetComponent<EnemyMovement>();
movement.SetMoveSpeed(3.5f);

// 设置新的目标
movement.SetTarget(newTargetTransform);

// 检查是否到达目标
bool reached = movement.HasReachedTarget();
```

## 调试功能

### 可视化调试
- 在Scene视图中选中敌人，会显示：
  - 红色连线：到目标的直线路径
  - 黄色圆圈：停止距离范围

### 运行时信息
- 在Game视图中显示每个敌人的实时距离信息
- 在Console面板查看寻路日志

## 性能优化

### 批量寻路优化
```csharp
// 避免每帧查找目标，可以缓存目标引用
public class GameManager : MonoBehaviour
{
    public static Transform GoldenTreeTransform { get; private set; }
    
    void Start()
    {
        GoldenTreeTransform = GameObject.FindGameObjectWithTag("GoldenTree").transform;
    }
}

// 在EnemyMovement中使用
public void SetTargetFromManager()
{
    SetTarget(GameManager.GoldenTreeTransform);
}
```

### 距离计算优化
- 使用Vector2.SqrMagnitude代替Vector2.Distance避免开方运算
- 设置合理的检测频率，不需要每帧都计算

## 扩展功能

### 1. 路径障碍避免
可以继承EnemyMovement类添加障碍检测：
```csharp
public class SmartEnemyMovement : EnemyMovement
{
    [SerializeField] private float obstacleDetectionDistance = 1f;
    [SerializeField] private LayerMask obstacleLayer = -1;
    
    protected override void MoveTowardsTarget()
    {
        Vector2 direction = AvoidObstacles();
        base.MoveTowardsTarget(direction);
    }
}
```

### 2. 多种移动模式
```csharp
public enum MovementType
{
    Direct,
    Zigzag,
    Spiral
}

[SerializeField] private MovementType movementType = MovementType.Direct;
```

### 3. 群体移动
添加群体行为，避免敌人重叠：
```csharp
[SerializeField] private float separationDistance = 1f;
[SerializeField] private float separationForce = 0.5f;
```

## 常见问题

### Q: 敌人不移动？
- 检查是否有Rigidbody2D组件
- 确认黄金树标签设置正确
- 查看Console是否有错误信息

### Q: 移动速度太慢/太快？
- 调整Move Speed参数
- 检查Rigidbody2D的重力和阻力设置

### Q: 敌人穿过障碍物？
- 添加Collider2D到障碍物
- 设置合适的Layer和碰撞矩阵
- 考虑使用A*寻路系统替代直接移动

### Q: 多个敌人重叠？
- 调整Stopping Distance参数
- 添加分离力（见扩展功能）
- 使用NavMeshAgent进行群体移动

## 测试场景设置

### 快速测试
1. 创建新场景
2. 添加一个黄金树对象，设置标签为"GoldenTree"
3. 添加敌人预制体到场景
4. 运行游戏观察敌人移动

### 压力测试
1. 使用EnemySpawner生成大量敌人
2. 观察性能表现
3. 调整移动参数优化性能

## 版本更新记录
- v1.0: 基础自动移动功能
- v1.1: 添加调试可视化
- v1.2: 支持动态目标切换
- v1.3: 优化性能，减少GC分配