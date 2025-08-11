# 神树动态攻击范围系统配置指南

## 概述
神树攻击范围现在可以根据PrefabManager中已实例化预制体的数量动态调整，实现随着游戏进程攻击范围逐渐扩大的效果。

## 系统特性
- **实时调整**: 攻击范围根据已实例化预制体数量实时更新
- **可配置缩放**: 可以调整攻击范围增长的倍数
- **性能优化**: 避免频繁的微小更新，只在有明显变化时更新
- **自动查找**: 如果未手动设置PrefabManager引用，系统会自动查找

## 配置步骤

### 1. 设置PrefabManager引用
1. 在场景中选择包含TreeAttackZone脚本的神树对象
2. 在Inspector面板中找到"预制体管理器引用"部分
3. 将场景中的PrefabManager对象拖拽到"Prefab Manager"字段
4. 如果不设置，系统会在Start时自动查找

### 2. 调整攻击范围参数
- **Attack Radius**: 设置基础攻击范围（建议5-10）
- 系统会自动保存这个值作为基础范围
- 实际攻击范围 = 基础范围 × (1 + 预制体数量 × 0.5)

### 3. 验证配置
运行游戏后检查Console输出：
- 成功配置会显示："TreeAttackZone: 攻击范围已更新为 X.XX，基于 N 个已实例化预制体"
- 配置失败会显示："TreeAttackZone: 未找到PrefabManager，攻击范围将不会动态调整"

## 工作原理

### 攻击范围计算公式
```
新攻击范围 = 基础攻击范围 × (1.0 + 预制体数量 × 0.5)
```

### 示例计算
- 基础攻击范围: 8
- 0个预制体: 8 × (1.0 + 0 × 0.5) = 8
- 1个预制体: 8 × (1.0 + 1 × 0.5) = 12
- 2个预制体: 8 × (1.0 + 2 × 0.5) = 16
- 3个预制体: 8 × (1.0 + 3 × 0.5) = 20

### 更新时机
- 每帧检查PrefabManager中的预制体数量
- 只有当攻击范围变化超过0.01时才更新
- 避免不必要的性能消耗

## 代码架构

### 核心方法
- `UpdateAttackRadius()`: 动态更新攻击范围的核心方法
- `Start()`: 初始化基础攻击范围和PrefabManager引用
- `Update()`: 每帧调用UpdateAttackRadius()

### 新增字段
- `prefabManager`: PrefabManager的引用
- `baseAttackRadius`: 保存的初始攻击范围值

## 自定义配置

### 修改缩放系数
在`UpdateAttackRadius()`方法中修改这一行：
```csharp
// 当前: 每个预制体增加0.5倍基础范围
float radiusMultiplier = 1.0f + (maxIndex * 0.5f);

// 示例: 每个预制体增加0.3倍基础范围（更温和的增长）
float radiusMultiplier = 1.0f + (maxIndex * 0.3f);

// 示例: 每个预制体增加1.0倍基础范围（更激进的增长）
float radiusMultiplier = 1.0f + (maxIndex * 1.0f);
```

### 设置最大攻击范围
```csharp
// 在UpdateAttackRadius()方法中添加最大值限制
float maxAllowedRadius = 50f; // 设置最大攻击范围
float newAttackRadius = Mathf.Min(baseAttackRadius * radiusMultiplier, maxAllowedRadius);
```

## 调试功能

### 可视化调试
- 在Scene视图中选中神树对象
- 红色圆圈显示当前攻击范围
- 黄色圆圈显示玩家检测范围
- 攻击范围会随着预制体数量实时变化

### Console日志
- 攻击范围更新时会输出详细信息
- 包含当前攻击范围值和预制体数量
- 便于调试和验证功能正常工作

## 性能考虑

### 优化措施
- 使用阈值检查避免频繁更新
- 只在攻击范围有明显变化时才更新
- 缓存PrefabManager引用避免重复查找

### 性能监控
- 监控Console输出频率
- 正常情况下只在预制体数量变化时输出日志
- 如果频繁输出可能需要调整阈值

## 扩展功能建议

### 1. 非线性增长
```csharp
// 使用平方根实现递减增长
float radiusMultiplier = 1.0f + (Mathf.Sqrt(maxIndex) * 0.3f);
```

### 2. 分阶段增长
```csharp
// 每3个预制体增加一个阶段
int stage = maxIndex / 3;
float radiusMultiplier = 1.0f + (stage * 0.8f);
```

### 3. 攻击范围可视化增强
- 添加攻击范围变化的动画效果
- 不同阶段使用不同颜色的攻击圆圈
- 添加攻击范围数值的UI显示

## 常见问题

### Q: 攻击范围没有变化？
- 检查PrefabManager引用是否正确设置
- 确认PrefabManager中确实有预制体被添加
- 查看Console是否有错误信息

### Q: 攻击范围增长太快/太慢？
- 调整UpdateAttackRadius()方法中的缩放系数
- 修改0.5f为其他值来控制增长速度

### Q: 性能问题？
- 检查Console输出频率
- 考虑增加更新阈值
- 可以改为事件驱动而非每帧检查

## 版本记录
- v1.0: 初始版本，实现基于预制体数量的动态攻击范围调整
- 支持自动PrefabManager查找
- 包含性能优化和调试功能