# 神树防御系统配置指南

## 1. 攻击范围圆圈显示设置

### 配置攻击范围圆圈精灵
1. 在神树GameObject下创建一个子对象，并添加 `SpriteRenderer` 组件。
2. 将圆形图案的Sprite拖拽到 `SpriteRenderer` 的 `Sprite` 字段。
3. 调整颜色为半透明红色（RGBA: 255, 0, 0, 128）或其他您喜欢的颜色。
4. 将此子对象拖拽到 `TreeAttackZone` 组件的 `Attack Circle Sprite` 字段。

### 脚本配置
- `TreeAttackZone` 脚本会自动调整 `Attack Circle Sprite` 的大小以匹配 `Attack Radius`，并在玩家进入/离开范围时启用/禁用其显示。
当玩家进入神树的防御区域后，神树会开始自动向范围内最近的敌人发射投射物。攻击逻辑会每隔 `Attack Interval` 秒执行一次，实例化 `Projectile Prefab` 并将其发射向最近的敌人。投射物命中敌人后会造成 `Attack Damage` 指定的伤害并消失。

### 脚本参数
- **攻击伤害 (Attack Damage)**: 投射物造成的伤害值。
- **攻击间隔 (Attack Interval)**: 神树发射投射物的时间间隔（秒）。
- **投射物预制体 (Projectile Prefab)**: 神树攻击时发射的投射物预制体。你需要创建一个包含 `TreeProjectile.cs` 脚本的预制体，并将其拖拽到此字段。

## 2. 层级设置

### 创建层级
1. 打开Edit > Project Settings > Tags and Layers
2. 添加新层级：
   - "Player"（玩家层）
   - "Enemy"（敌人层）

### 分配层级
- 玩家对象：Player层
- 敌人对象：Enemy层
- 神树对象：Default层

## 3. 碰撞器配置

### 神树设置
1. 在Tree预制体上添加Circle Collider 2D
2. 设置Radius为攻击范围
3. 勾选Is Trigger
4. 添加Rigidbody 2D，设置Body Type为Kinematic

### 玩家检测
确保玩家对象有：
- Collider 2D（任意形状）
- Rigidbody 2D
- 标签设置为"Player"

## 4. 敌人配置

### 敌人预制体要求
1. 添加EnemyHealth脚本
2. 设置标签为"Enemy"
3. 添加Collider 2D和Rigidbody 2D
4. 层级设置为"Enemy"

## 5. 调试技巧

### 场景视图调试
- 选中神树对象时，场景视图会显示攻击范围圆圈
- 绿色圆圈：玩家在范围内
- 红色圆圈：玩家不在范围内

### 控制台输出
- 玩家进入/离开防御区域时会输出日志
- 每次攻击会显示攻击目标和伤害值

## 6. 性能优化

### 建议设置
- 攻击间隔：0.5-2秒（避免频繁检测）
- 攻击半径：3-8米（平衡性能与范围）
- 使用对象池管理特效

## 7. 扩展功能

### 可添加功能
- 攻击音效
- 攻击特效
- 生命值显示
- 升级系统
- 范围变化动画