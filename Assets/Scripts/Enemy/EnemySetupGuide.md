# 敌人设置指南

## 标签设置

为了使敌人能够被正确识别并与玩家交互，请确保：

1. 所有敌人预制体都设置了标签为 "Enemy"
2. 玩家对象设置了标签为 "Player"

## 碰撞器设置

1. 敌人需要有 Collider2D 组件（可以是 BoxCollider2D、CircleCollider2D 等）
2. 如果使用触发器检测，请确保勾选 "Is Trigger" 选项
3. 如果使用物理碰撞检测，请确保不勾选 "Is Trigger" 选项

## 敌人移动脚本设置

在 EnemyMovement 脚本中：

1. 确保 "目标标签" 设置为 "Player"，使敌人朝玩家移动
2. 调整 "移动速度" 参数以控制敌人追逐玩家的速度
3. 调整 "停止距离" 参数以控制敌人与玩家保持的距离

## 玩家生命值脚本设置

在场景中的玩家对象上添加 PlayerHealth 脚本，并确保：

1. 引用场景中的 SceneManagerScript 对象
2. 确认 "敌人标签" 设置为 "Enemy"

## 场景管理器设置

1. 在场景中创建一个空对象，命名为 "SceneManager"
2. 添加 SceneManagerScript 组件
3. 确保场景已添加到 Build Settings 中的场景列表