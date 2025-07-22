# Prefab Sprite 系统说明

## 系统概述

这是一个集成的对象生成和Sprite触发系统，当玩家接近生成的对象时会显示Sprite图片，并可以通过按键与对象进行交互。

## 系统组件

### ObjectSpawner.cs
集成的对象生成器和Sprite触发系统，负责：
- 对象的随机生成和管理
- 玩家距离检测
- Sprite的显示和隐藏
- UI交互面板的管理
- 调试信息显示

### SimpleSpriteDisplay.cs（可选）
简单的Sprite显示组件，可用于独立的Sprite显示需求。

### InteractionPanel.prefab
交互面板预制体，包含3个可配置的按钮。

## 使用步骤

1. **设置ObjectSpawner**：
   - 在场景中创建一个空的GameObject
   - 添加ObjectSpawner组件
   - 配置生成参数

2. **配置对象生成设置**：
   - 设置要生成的预制体
   - 配置生成数量、区域和间距

3. **配置Sprite显示设置**：
   - 设置要显示的Sprite图片
   - 配置Sprite的颜色、大小和排序层

4. **配置玩家检测设置**：
   - 设置玩家标签（默认"Player"）
   - 配置检测半径

5. **配置UI交互设置**：
   - 设置交互按键（默认F键）
   - 分配InteractionPanel预制体
   - 配置三个按钮的文本

6. **运行测试**：
   - 点击"Generate Objects"按钮生成对象
   - 移动玩家靠近对象查看Sprite显示
   - 在Sprite显示时按F键打开交互面板

## 参数说明

### 对象生成设置
- **Objects To Spawn**: 要生成的预制体数组
- **Spawn Count**: 生成对象的数量
- **Spawn Area Size**: 生成区域的大小
- **Min Distance Between Objects**: 对象之间的最小距离
- **Max Spawn Attempts**: 最大生成尝试次数

### Sprite显示设置
- **Sprite To Display**: 要显示的Sprite图片
- **Sprite Color**: Sprite的颜色
- **Sprite Scale**: Sprite的缩放比例
- **Sprite Sorting Layer**: Sprite的排序层名称
- **Sprite Order In Layer**: 排序层内的顺序

### 玩家检测设置
- **Player Tag**: 玩家对象的标签
- **Detection Radius**: 检测半径
- **Show Detection Range**: 是否在Scene视图显示检测范围
- **Show Runtime Debug**: 是否在Game视图显示调试信息

### UI交互设置
- **Interaction Key**: 交互按键（默认为F键）
- **Interaction Panel**: 交互面板预制体（使用Assets/Prefabs/InteractionPanel.prefab）
- **Button1 Text**: 按钮1的文本（默认："按钮1"）
- **Button2 Text**: 按钮2的文本（默认："按钮2"）
- **Button3 Text**: 按钮3的文本（默认："按钮3"）

## 交互功能

### F键交互
- 当玩家在检测范围内且Sprite显示时，按下F键会弹出交互面板
- **面板打开时游戏会自动暂停**（Time.timeScale = 0）
- 面板包含3个可配置的按钮
- 点击任意按钮或再次按F键可关闭面板并恢复游戏（Time.timeScale = 1）
- 按钮点击事件可在ObjectSpawner的OnButton1Click、OnButton2Click、OnButton3Click方法中自定义

## 调试功能

### Scene视图调试
- 绿色圆圈：显示每个对象的检测范围
- 红色圆圈：显示玩家周围的检测半径

### Game视图调试
- 左上角显示实时调试信息
- 包括生成对象数量、玩家位置、检测半径
- 显示前5个对象的距离和状态（绿色=在范围内，红色=不在范围内）
- 显示当前范围内的对象统计

### Console日志
- 详细的操作日志，包括对象生成、Sprite显示/隐藏、面板操作等

## 故障排除

### Sprite不显示
1. 检查Sprite To Display是否已分配
2. 确认玩家对象标签是否正确
3. 检查检测半径设置
4. 查看Console是否有错误信息

### 交互面板不弹出
1. 确认Interaction Panel预制体已分配
2. 检查玩家是否在检测范围内
3. 确认Sprite是否正在显示
4. 检查交互按键设置

### 按钮点击无响应
1. 确认场景中有EventSystem
2. 检查按钮的Interactable属性
3. 查看Console中的按钮点击日志

## 注意事项

1. **性能优化**：系统会自动管理Sprite的创建和销毁，避免内存泄漏
2. **玩家标签**：确保玩家对象使用正确的标签
3. **Canvas管理**：系统会自动创建Canvas，无需手动添加
4. **预制体路径**：InteractionPanel预制体应放在Assets/Prefabs/目录下
5. **按钮事件**：可在ObjectSpawner的按钮点击方法中添加自定义逻辑

## 扩展功能

### 自定义按钮行为
在ObjectSpawner.cs中修改以下方法来实现自定义功能：
```csharp
public void OnButton1Click()
{
    // 添加按钮1的自定义逻辑
    Debug.Log($"按钮1被点击: {button1Text}");
    CloseInteractionPanel();
}
```

### 添加更多交互
可以扩展系统支持更多交互方式：
- 不同的按键触发不同功能
- 根据对象类型显示不同的面板
- 添加音效和动画效果

### 数据持久化
可以添加数据保存功能：
- 保存对象生成位置
- 记录玩家交互历史
- 保存自定义配置