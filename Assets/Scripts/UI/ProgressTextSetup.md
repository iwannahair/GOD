# 进度条文本设置指南

## 问题描述

收集掉落物后进度条没有增加的原因是：CollectionProgressManager组件中的progressText引用为null，导致进度更新时无法正确显示。

## 解决方案

### 1. 创建进度文本对象

1. 在Unity编辑器中，打开GameScene场景
2. 在Hierarchy面板中找到Canvas → ProgressSlider
3. 右键点击ProgressSlider → UI → Text
4. 重命名为"ProgressText"
5. 设置Text组件属性：
   - 文本内容："0/10"
   - 字体大小：16
   - 颜色：白色
   - 对齐方式：居中
6. 调整位置：
   - 将ProgressText放置在进度条旁边或上方
   - 建议位置：X=0, Y=20（相对于ProgressSlider）

### 2. 关联到CollectionProgressManager

1. 在Hierarchy面板中选择CollectionProgressManager对象
2. 在Inspector面板中找到CollectionProgressManager组件
3. 将新创建的ProgressText对象拖拽到progressText字段

### 3. 测试进度条

1. 进入游戏场景
2. 收集敌人掉落物
3. 观察进度条和进度文本是否正确更新

## 其他可能的问题

如果创建并关联ProgressText后仍然无法正常工作，请检查：

1. ResourceManager是否正确设置并存在于场景中
2. ResourceManager和CollectionProgressManager之间的事件订阅是否正常工作
3. 敌人是否正确掉落资源，ResourceDrop脚本是否正常工作
4. 控制台是否有错误信息

## 调试技巧

在CollectionProgressManager.cs脚本中添加以下调试代码：

```csharp
// 在Start方法中添加
Debug.Log("CollectionProgressManager启动，progressText是否为null: " + (progressText == null));
Debug.Log("ResourceManager是否为null: " + (ResourceManager.Instance == null));

// 在OnResourceCollected方法中添加
Debug.Log($"收集到资源：{resourceType}，数量：{newAmount}");
```

这些日志将帮助确定问题的具体原因。