# 敌人生成器配置指南
```
## 概述
EnemySpawner脚本会在场上没有敌人时，自动在摄像机显示区域外侧3Unity单位的地方生成10个敌人，并确保它们朝目标物体移动（需要敌人预制体挂载 `EnemyMovement.cs` 脚本）。
```
## 配置步骤

### 1. 创建空对象
1. 在Hierarchy面板中右键 → Create Empty
2. 重命名为"EnemySpawner"
3. 将EnemySpawner.cs脚本拖拽到该对象上

### 2. 设置敌人预制体
1. 在Project面板中找到敌人预制体（如Enemy.prefab、BigEnemy.prefab等）
2. 将预制体拖拽到EnemySpawner组件的"Enemy Prefab"字段

### 3. 配置标签系统
1. 选择所有敌人预制体
2. 在Inspector面板的Tag下拉菜单中选择或创建"Enemy"标签
3. 确保EnemySpawner组件的"Enemy Tag"字段设置为"Enemy"

### 4. 摄像机设置
- 确保场景中有主摄像机（Main Camera）
- 脚本会自动使用Camera.main，无需手动设置

### 5. 可选配置
- **Spawn Count**: 每次生成的敌人数量（默认10个）
- **Spawn Interval**: 生成间隔时间（秒）```
- **Spawn Offset**: 距离摄像机边界的Unity单位距离。此值决定了敌人生成点距离摄像机可见区域的远近。
```- **Detection Radius**: 检测范围（0表示全地图检测）

## 使用说明

### 手动测试
1. 在Inspector面板中右键点击EnemySpawner组件
2. 选择"立即生成敌人"菜单项进行测试

### 运行时行为
- 系统会持续检测场上敌人数量
- 当敌人数量为0时，自动开始生成流程
- 生成10个敌人后停止，等待下次触发

## 调试技巧

### 可视化检测范围
- 在Scene视图中选中EnemySpawner对象
- 如果设置了Detection Radius > 0，会显示红色圆形范围

### 日志监控
- 查看Console面板中的生成日志
- 包含开始生成和完成生成的提示信息

## 扩展功能

### 支持多种敌人类型
可以创建多个EnemySpawner实例，每个使用不同的敌人预制体

### 动态调整生成参数
```csharp
// 在游戏运行时修改参数示例
EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
spawner.spawnCount = 15;  // 改为生成15个敌人
spawner.spawnInterval = 0.5f;  // 改为每0.5秒生成一个
```

### 获取当前敌人数量
```csharp
int enemyCount = spawner.GetCurrentEnemyCount();
```

## 常见问题

### Q: 敌人没有生成？
- 检查敌人预制体是否正确设置
- 确认敌人标签是否为"Enemy"
- 查看Console是否有错误信息
```
### Q: 生成位置不正确？
- 检查 `Spawn Offset` 值是否合适。如果值太小，敌人可能生成在屏幕内；如果值太大，敌人可能生成得太远。
- 确保主摄像机（Main Camera）在场景中正确设置并带有“MainCamera”标签。
```
### Q: 敌人生成后不移动？
- 确保敌人预制体上挂载了 `EnemyMovement.cs` 脚本
- 确保场景中存在标签为 "GoldenTree" 的目标对象

### Q: 想要不同的生成模式？
- 修改GetSpawnPosition()方法来自定义生成位置
- 可以继承EnemySpawner类并重写相关方法