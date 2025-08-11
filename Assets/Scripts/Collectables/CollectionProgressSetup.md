# 收集进度条系统配置指南

## 概述
收集进度条系统会跟踪玩家收集敌人掉落物的进度，当达到指定数量时弹出3个选择按钮，让玩家选择不同的增益效果。

## 配置步骤

### 1. 创建UI Canvas
1. 在Hierarchy面板中右键 → UI → Canvas
2. 重命名为"CollectionProgressUI"
3. 设置Canvas的Render Mode为"Screen Space - Overlay"

### 2. 创建进度条UI
1. 在Canvas下右键 → UI → Slider
2. 重命名为"ProgressSlider"
3. 设置位置：
   - Anchor: Top-Left
   - Position: X=200, Y=-50
   - Width=300, Height=20

#### 进度条样式设置
1. 选择ProgressSlider下的Background
   - 设置颜色为深灰色 (R:50, G:50, B:50, A:255)
2. 选择ProgressSlider下的Fill Area → Fill
   - 设置颜色为绿色 (R:0, G:255, B:0, A:255)
3. 删除Handle Slide Area（不需要拖拽功能）

### 3. 创建进度文本
1. 在Canvas下右键 → UI → Text
2. 重命名为"ProgressText"
3. 设置位置：
   - Anchor: Top-Left
   - Position: X=350, Y=-50
4. 设置文本属性：
   - Text: "0/10"
   - Font Size: 16
   - Color: 白色
   - Alignment: Middle Center

### 4. 创建按钮面板
1. 在Canvas下右键 → UI → Panel
2. 重命名为"ButtonPanel"
3. 设置为全屏覆盖：
   - Anchor: Stretch
   - Left=0, Top=0, Right=0, Bottom=0
4. 设置背景颜色为半透明黑色 (R:0, G:0, B:0, A:128)

### 5. 创建选择按钮
在ButtonPanel下创建3个按钮：

#### 按钮1 - 增加攻击力
1. 右键ButtonPanel → UI → Button
2. 重命名为"Button1"
3. 设置位置：
   - Anchor: Middle Center
   - Position: X=-200, Y=0
   - Width=150, Height=50
4. 设置按钮文本为"增加攻击力"

#### 按钮2 - 增加移动速度
1. 右键ButtonPanel → UI → Button
2. 重命名为"Button2"
3. 设置位置：
   - Anchor: Middle Center
   - Position: X=0, Y=0
   - Width=150, Height=50
4. 设置按钮文本为"增加移动速度"

#### 按钮3 - 恢复生命值
1. 右键ButtonPanel → UI → Button
2. 重命名为"Button3"
3. 设置位置：
   - Anchor: Middle Center
   - Position: X=200, Y=0
   - Width=150, Height=50
4. 设置按钮文本为"恢复生命值"

### 6. 添加管理器脚本
1. 在Hierarchy中创建空对象，重命名为"CollectionProgressManager"
2. 将CollectionProgressManager.cs脚本拖拽到该对象上
3. 在Inspector中配置脚本参数：
   - Max Collection Count: 10（达到满进度所需收集数量）
   - Progress Slider: 拖拽ProgressSlider对象
   - Progress Text: 拖拽ProgressText对象
   - Button Panel: 拖拽ButtonPanel对象
   - Button1: 拖拽Button1对象
   - Button2: 拖拽Button2对象
   - Button3: 拖拽Button3对象
   - Button1 Text: "增加攻击力"
   - Button2 Text: "增加移动速度"
   - Button3 Text: "恢复生命值"

### 7. 确保ResourceManager存在
1. 检查场景中是否有ResourceManager对象
2. 如果没有，创建空对象并挂载ResourceManager.cs脚本
3. 确保ResourceManager设置为DontDestroyOnLoad

## 使用说明

### 系统工作流程
1. 玩家收集敌人掉落的资源
2. 每收集一个资源，进度条增加
3. 当收集数量达到设定值（默认10个）时：
   - 游戏暂停（Time.timeScale = 0）
   - 弹出选择按钮面板
   - 玩家选择其中一个按钮
   - 应用对应效果
   - 重置进度条
   - 恢复游戏

### 自定义按钮效果
在CollectionProgressManager.cs中修改以下方法来实现具体效果：

```csharp
// 增加攻击力效果
private void ApplyButton1Effect()
{
    // 添加你的攻击力增加逻辑
    // 例如：PlayerStats.Instance.IncreaseAttackPower(10);
}

// 增加移动速度效果
private void ApplyButton2Effect()
{
    // 添加你的移动速度增加逻辑
    // 例如：PlayerStats.Instance.IncreaseMovementSpeed(1.5f);
}

// 恢复生命值效果
private void ApplyButton3Effect()
{
    // 添加你的生命值恢复逻辑
    // 例如：PlayerHealth.Instance.RestoreHealth(50);
}
```

## 调试和测试

### 手动测试进度
可以在CollectionProgressManager脚本中添加测试方法：

```csharp
[ContextMenu("测试增加进度")]
public void TestAddProgress()
{
    AddProgress(1);
}

[ContextMenu("测试重置进度")]
public void TestResetProgress()
{
    ResetProgress();
}
```

### 常见问题

**Q: 进度条不更新？**
- 检查ResourceManager是否正确设置
- 确认CollectionProgressManager的UI组件引用是否正确
- 查看Console是否有错误信息

**Q: 按钮面板不显示？**
- 检查ButtonPanel对象是否正确引用
- 确认Canvas的Render Mode设置
- 检查按钮面板的Active状态

**Q: 游戏暂停后无法恢复？**
- 确保按钮点击事件正确设置
- 检查Time.timeScale是否正确恢复为1

## 扩展功能

### 添加音效
在按钮点击和进度满时添加音效：

```csharp
[Header("音效")]
[SerializeField] private AudioClip progressCompleteSound;
[SerializeField] private AudioClip buttonClickSound;
```

### 添加动画效果
为进度条和按钮面板添加动画：
- 进度条填充动画
- 按钮面板淡入淡出
- 按钮悬停效果

### 保存进度
将收集进度保存到PlayerPrefs：

```csharp
public void SaveProgress()
{
    PlayerPrefs.SetInt("CollectionProgress", currentCollectionCount);
}

public void LoadProgress()
{
    currentCollectionCount = PlayerPrefs.GetInt("CollectionProgress", 0);
    UpdateProgressUI();
}
```