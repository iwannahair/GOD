# 神树防御系统配置指南（SpriteRenderer版本）

## 主要更新
- ✅ **attackCircleSprite** 从 `Image` 改为 `SpriteRenderer`
- ✅ 更好的2D游戏兼容性
- ✅ 简化的层级控制
- ✅ 优化渲染性能

## 配置步骤

### 1. 创建圆形攻击范围精灵

#### 创建SpriteRenderer对象
1. 在Hierarchy中选中Tree对象
2. 右键 → Create Empty 创建子对象
3. 重命名为"AttackCircle"
4. 添加SpriteRenderer组件

#### 设置SpriteRenderer
```
SpriteRenderer组件设置：
- Sprite: 圆圈精灵（可用Unity内置Knob或自定义圆圈）
- Color: RGBA(255,255,255,128) 半透明
- Sorting Order: 10（确保在树上方）
- Material: Sprites-Default
```

### 2. 圆圈精灵资源

#### 使用Unity内置资源
- Sprite: `Knob`（在Unity内置资源中搜索）
- 调整Color为半透明白色

#### 自定义圆圈图片
1. 创建512x512像素透明圆圈PNG
2. 导入到Assets/Art resource/UI/目录
3. 设置Texture Type为`Sprite (2D and UI)`
4. 拖拽到SpriteRenderer的Sprite字段

### 3. 层级配置

#### 创建专用层级
1. Edit → Project Settings → Tags and Layers
2. 添加：
   - Layer 6: "Enemy"
   - Layer 7: "Player"

#### 设置对象层级
- **Tree对象**: 保持默认层级
- **AttackCircle对象**: 任意层级（不影响功能）
- **敌人预制体**: Layer设为"Enemy"
- **玩家对象**: Layer设为"Player"

### 4. TreeAttackZone配置

#### 脚本字段设置
- **Attack Radius**: 3（攻击范围半径）
- **Attack Damage**: 10（每次攻击伤害）
- **Attack Interval**: 1（攻击间隔秒）
- **Enemy Layer**: 选择"Enemy"
- **Player Layer**: 选择"Player"
- **Attack Circle Sprite**: 拖拽AttackCircle对象

#### CircleCollider2D设置
1. 确保Tree有CircleCollider2D
2. 勾选"Is Trigger"
3. Radius与Attack Radius匹配

## 运行时测试

### 验证步骤
1. 运行游戏
2. 玩家进入树范围 → 圆圈显示绿色
3. 敌人进入范围 → 自动攻击最近敌人
4. 玩家离开 → 圆圈隐藏

### 调试可视化
- Scene视图：红色/绿色圆圈显示攻击范围
- Console日志：显示攻击事件和玩家进出

## 常见问题

### 圆圈不显示
- 检查SpriteRenderer的Sorting Order
- 确认圆圈纹理透明度
- 验证AttackCircle是Tree的子对象

### 攻击不触发
- 确认敌人层级为"Enemy"
- 检查敌人有EnemyHealth组件
- 验证CircleCollider2D设置

### 位置偏移
- 确保AttackCircle本地坐标为(0,0,0)
- 检查Sprite的Pivot设置为中心点

## 代码扩展

### 动态调整攻击范围
```csharp
TreeAttackZone treeZone = GetComponent<TreeAttackZone>();
treeZone.attackRadius = 5f;

// 更新圆圈大小
float diameter = treeZone.attackRadius * 2f;
treeZone.attackCircleSprite.transform.localScale = 
    new Vector3(diameter, diameter, 1f);
```

### 切换圆圈样式
```csharp
// 运行时更换圆圈纹理
Sprite newCircle = Resources.Load<Sprite>("UI/NewCircle");
attackCircleSprite.sprite = newCircle;
```

## 性能优化
- 使用256x512或512x512圆圈纹理
- 禁用Generate Mip Maps
- 合理设置Sorting Order避免过度绘制

## 版本记录
- v2.0: 迁移到SpriteRenderer系统
- v1.0: 原始Image系统