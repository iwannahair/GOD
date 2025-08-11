# 收集进度条与预制体管理器集成指南

## 概述

此功能实现了当收集进度条满后，玩家点击相应按钮时，会将对应的预制体添加到PrefabManager的列表中。这样玩家可以通过收集资源来解锁新的预制体，增强游戏的策略性和可玩性。

## 系统架构

### 核心组件

1. **CollectionProgressManager** - 收集进度管理器
   - 监听资源收集事件
   - 管理进度条UI
   - 处理按钮点击事件
   - 与PrefabManager交互

2. **PrefabManager** - 预制体管理器
   - 管理预制体列表
   - 提供添加/移除预制体的方法
   - 处理预制体实例化

### 工作流程

```
玩家收集资源 → 进度条增加 → 进度满时弹出选择面板 → 玩家选择按钮 → 对应预制体添加到PrefabManager → 玩家可以生成新预制体
```

## Unity编辑器配置

### 1. 设置CollectionProgressManager

1. 找到场景中的CollectionProgressManager对象
2. 在Inspector面板中配置以下新增字段：

#### 预制体设置区域
- **PrefabManager**: 拖拽场景中的PrefabManager对象
- **Button1 Prefab**: 拖拽按钮1对应的预制体（如武器预制体）
- **Button2 Prefab**: 拖拽按钮2对应的预制体（如防具预制体）
- **Button3 Prefab**: 拖拽按钮3对应的预制体（如道具预制体）

### 2. 确保PrefabManager存在

1. 检查场景中是否有PrefabManager对象
2. 如果没有，创建空对象并挂载PrefabManager脚本
3. 配置PrefabManager的基本设置：
   - **Spawn Key**: 设置生成预制体的按键（默认Space）
   - **Spawn Point**: 设置预制体生成位置

### 3. 预制体准备

确保你有以下预制体准备好：
- 攻击力相关预制体（武器、炮塔等）
- 移动速度相关预制体（载具、坐骑等）
- 生命值相关预制体（治疗道具、防护装备等）

## 代码功能说明

### PrefabManager新增方法

```csharp
/// <summary>
/// 添加预制体到列表中
/// </summary>
/// <param name="prefab">要添加的预制体</param>
public void AddPrefab(GameObject prefab)

/// <summary>
/// 移除指定的预制体
/// </summary>
/// <param name="prefab">要移除的预制体</param>
/// <returns>是否成功移除</returns>
public bool RemovePrefab(GameObject prefab)

/// <summary>
/// 清空预制体列表
/// </summary>
public void ClearPrefabList()

/// <summary>
/// 获取当前预制体列表数量
/// </summary>
/// <returns>列表中预制体的数量</returns>
public int GetPrefabCount()
```

### CollectionProgressManager修改

- 添加了PrefabManager引用
- 添加了三个按钮对应的预制体字段
- 修改了按钮点击效果，现在会调用PrefabManager.AddPrefab()方法

## 使用示例

### 游戏流程示例

1. **初始状态**: PrefabManager列表为空或只有基础预制体
2. **收集阶段**: 玩家击败敌人，收集掉落的资源
3. **进度满**: 收集到10个资源后，进度条满，弹出选择面板
4. **选择奖励**: 玩家点击"增加攻击力"按钮
5. **获得预制体**: 武器预制体被添加到PrefabManager列表
6. **使用预制体**: 玩家按Space键可以生成新的武器预制体

### 预制体配置建议

#### 按钮1 - 增加攻击力
- 武器预制体（剑、弓、法杖等）
- 炮塔预制体
- 攻击型道具

#### 按钮2 - 增加移动速度
- 载具预制体（马车、飞行器等）
- 移动平台
- 速度增强道具

#### 按钮3 - 恢复生命值
- 治疗道具预制体
- 防护装备
- 生命恢复站

## 扩展功能建议

### 1. 预制体解锁系统

```csharp
[System.Serializable]
public class PrefabUnlock
{
    public GameObject prefab;           // 预制体
    public string unlockName;          // 解锁名称
    public string description;         // 描述
    public Sprite icon;               // 图标
    public int requiredLevel;         // 需要的等级
    public bool isUnlocked;           // 是否已解锁
}
```

### 2. 预制体分类管理

```csharp
public enum PrefabCategory
{
    Weapon,      // 武器
    Defense,     // 防御
    Utility,     // 工具
    Special      // 特殊
}
```

### 3. 随机奖励系统

```csharp
/// <summary>
/// 从指定分类中随机选择预制体
/// </summary>
public GameObject GetRandomPrefab(PrefabCategory category)
{
    // 实现随机选择逻辑
}
```

### 4. 预制体升级系统

```csharp
/// <summary>
/// 升级现有预制体
/// </summary>
public void UpgradePrefab(GameObject basePrefab, GameObject upgradedPrefab)
{
    // 替换列表中的预制体
}
```

## 调试和测试

### 测试步骤

1. **基础功能测试**:
   - 运行游戏
   - 手动触发收集事件（可以在ResourceManager中添加测试方法）
   - 观察进度条是否正确更新
   - 进度满时是否弹出选择面板

2. **预制体添加测试**:
   - 点击不同按钮
   - 检查Console日志确认预制体是否成功添加
   - 按Space键测试是否能生成新添加的预制体

3. **边界情况测试**:
   - 测试空预制体引用
   - 测试PrefabManager未设置的情况
   - 测试重复添加相同预制体

### 常见问题解决

**Q: 点击按钮后没有添加预制体？**
- 检查PrefabManager引用是否正确设置
- 检查对应的预制体字段是否已赋值
- 查看Console是否有警告信息

**Q: 添加的预制体无法生成？**
- 确认预制体本身没有错误
- 检查PrefabManager的SpawnPoint是否设置
- 确认预制体在Project面板中是否为Prefab类型

**Q: 进度条不更新？**
- 检查ResourceManager是否正确触发OnResourceChanged事件
- 确认CollectionProgressManager正确订阅了事件

## 性能优化建议

1. **对象池**: 对于频繁生成的预制体，考虑使用对象池模式
2. **延迟加载**: 大型预制体可以考虑延迟加载
3. **内存管理**: 定期清理不需要的预制体引用
4. **批量操作**: 如果需要添加多个预制体，考虑提供批量添加方法

## 总结

这个集成系统提供了一个灵活的预制体解锁机制，玩家通过收集资源可以获得新的游戏内容。系统设计遵循了良好的架构原则，具有高内聚、低耦合的特点，便于后续扩展和维护。

通过合理配置预制体和调整收集目标，可以创造出丰富的游戏体验和策略选择。