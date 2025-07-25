# Prefab Sprite 触发系统使用说明

这个系统允许当玩家进入生成的prefab检测范围时，在prefab位置显示一个Sprite，当玩家离开时Sprite消失。

## 系统组件

### ObjectSpawner.cs
集成的对象生成器和Sprite触发系统：
- 生成prefab对象
- 自动检测玩家与每个对象的距离
- 管理Sprite的显示和隐藏
- 包含完整的调试功能

### SimpleSpriteDisplay.cs
可选的Sprite显示组件：
- 提供淡入淡出和缩放动画效果
- 可自定义精灵、颜色、缩放
- 支持动画曲线控制

## 使用步骤

### 步骤1：设置ObjectSpawner
1. 在场景中创建一个空GameObject
2. 添加ObjectSpawner组件
3. 配置以下参数：
   - **Prefab To Spawn**: 要生成的预制体
   - **Spawn Count**: 生成数量
   - **Spawn Radius**: 生成半径
   - **Trigger Sprite**: 当玩家接近时显示的Sprite
   - **Detection Radius**: 检测半径
   - **Sprite Offset**: Sprite相对于prefab的偏移位置

### 步骤2：设置Sprite
1. 准备一个Sprite资源（图片文件，Texture Type设置为Sprite (2D and UI)）
2. 将Sprite拖拽到ObjectSpawner的Trigger Sprite字段
3. 配置Sprite的颜色、缩放、偏移等参数

### 步骤3：设置玩家
确保玩家对象的Tag设置为"Player"（或在ObjectSpawner中修改Player Tag字段）

### 步骤4：运行测试
1. 运行游戏
2. ObjectSpawner会自动生成prefab对象
3. 当玩家接近任何生成的对象时，会在该对象位置显示Sprite
4. 当玩家离开时，Sprite会消失

## 参数说明

### 生成设置
- **Prefab To Spawn**: 要生成的预制体
- **Spawn Count**: 生成数量
- **Spawn Radius**: 生成半径
- **Min Distance**: 物体间最小距离

### 生成区域设置
- **Use Rectangle Area**: 是否使用矩形区域
- **Rectangle Size**: 矩形区域大小

### 生成选项
- **Spawn On Start**: 开始时自动生成
- **Show Gizmos**: 显示生成区域
- **Obstacle Layer**: 障碍物图层

### Sprite触发设置
- **Trigger Sprite**: 要显示的Sprite
- **Detection Radius**: 玩家检测半径（单位：Unity单位）
- **Sprite Offset**: Sprite相对于prefab的偏移位置
- **Sprite Scale**: Sprite缩放大小
- **Sprite Color**: Sprite颜色
- **Sorting Layer Name**: 排序层名称
- **Sorting Order**: 排序顺序

### 玩家检测设置
- **Player Tag**: 玩家标签（默认"Player"）
- **Show Detection Range**: 是否在Scene视图显示检测范围
- **Show Runtime Debug**: 运行时调试显示

## 调试功能

### Scene视图调试
在Scene视图中可以看到以下调试信息：
- **黄色圆圈**: 生成区域范围
- **青色圆圈**: 每个对象的检测范围
- **红色连线**: 玩家与范围内对象的连接线
- **绿色小圆**: 已生成对象的位置

### Game视图调试
运行时在Game视图中会显示：
- **生成对象数量**: 显示当前生成的对象总数
- **玩家位置**: 显示玩家当前位置
- **检测半径**: 显示当前检测半径设置
- **对象距离**: 显示前5个对象与玩家的距离和状态
- **范围内对象**: 显示有多少对象在检测范围内

### Console日志
系统会在Console中输出以下信息：
- 玩家对象查找结果
- Sprite显示/隐藏操作
- 成功生成的对象数量
- 清除对象操作确认

## 故障排除

### 检测范围不可见
1. **Scene视图**: 确保`Show Detection Range`已勾选
2. **Game视图**: 确保`Show Runtime Debug`已勾选，运行时会显示调试信息
3. **Gizmos设置**: 在Scene视图右上角确保Gizmos已开启
4. **玩家对象**: 确保场景中存在标签为"Player"的对象

### Sprite不显示
1. **检查Trigger Sprite**: 确保ObjectSpawner中的`Trigger Sprite`字段已设置
2. **检查排序层**: 调整`Sorting Layer Name`和`Sorting Order`确保Sprite显示在其他对象之上
3. **检查摄像机**: 确保摄像机能看到Sprite的显示位置（prefab位置 + spriteOffset）
4. **检查玩家标签**: 确保玩家对象的Tag设置正确
5. **检查检测半径**: 确保`Detection Radius`足够大，玩家能够进入检测范围

### 系统不工作
1. **检查Console**: 查看是否有错误信息
2. **检查玩家对象**: 确保找到了玩家对象（Console会显示查找结果）
3. **检查生成**: 确保prefab对象已成功生成
4. **检查距离**: 使用调试信息查看玩家与对象的实际距离

## 注意事项

1. **集成设计**: 所有功能都集成在ObjectSpawner中，无需额外组件
2. **Sprite设置**: 系统直接使用SpriteRenderer在世界空间显示，无需Canvas
3. **性能优化**: 检测使用距离计算，性能较好
4. **单玩家支持**: 目前支持单玩家，如需多玩家可修改Player Tag设置
5. **Sprite层级**: Sprite会显示在世界空间中，通过sortingOrder控制层级
6. **资源要求**: 需要准备Sprite资源，确保Texture Type设置为Sprite (2D and UI)
7. **调试模式**: 开发时建议开启所有调试选项，发布时可关闭以提高性能
8. **内存管理**: 系统会自动管理Sprite的创建和销毁，避免内存泄漏

## 扩展功能

### 多种Sprite类型
可以为不同类型的prefab设置不同的Sprite，通过修改ObjectSpawner代码实现：
- 根据prefab类型选择不同的Sprite
- 实现多样化的显示效果
- 添加条件判断逻辑

### 自定义动画效果
可以通过修改ShowSprite和HideSprite方法来：
- 添加淡入淡出效果
- 实现缩放动画
- 添加旋转或移动动画
- 集成粒子效果

### 交互功能
可以扩展系统以支持：
- 点击Sprite触发事件
- 显示UI信息面板
- 播放音效
- 触发游戏逻辑

## API参考

### 公共方法
- `SpawnObjects()`: 手动生成对象
- `ClearSpawnedObjects()`: 清除所有生成的对象
- `RespawnObjects()`: 重新生成对象
- `GetSpawnedObjects()`: 获取已生成的对象列表

### 私有方法
- `ShowSprite(int index)`: 显示指定索引对象的Sprite
- `HideSprite(int index)`: 隐藏指定索引对象的Sprite
- `IsValidPosition(Vector2 position)`: 检查位置是否有效
- `GetRandomPosition()`: 获取随机生成位置