## 概述
`PlayerMovement.cs` 脚本负责处理玩家的移动输入（WASD键）并控制玩家角色的移动。它使用 `Rigidbody2D` 组件进行物理移动，确保平滑和准确的运动。

## 配置步骤

### 1. 创建玩家GameObject
1. 在Unity中创建一个新的GameObject作为玩家角色（例如，一个Sprite或一个空GameObject）。
2. 确保该GameObject上挂载了 `SpriteRenderer` (如果玩家是2D精灵) 和 `Rigidbody2D` 组件。
   - 对于 `Rigidbody2D`，建议将 `Body Type` 设置为 `Dynamic`，并勾选 `Freeze Rotation Z` 以防止玩家旋转。

### 2. 挂载 `PlayerMovement.cs` 脚本
1. 将 `PlayerMovement.cs` 脚本拖拽到您的玩家GameObject上。

### 3. 调整移动参数
在Inspector面板中，您可以调整以下参数：
- **Move Speed**：玩家的移动速度。根据您的游戏需求调整此值。

## 使用说明

### 1. 输入控制
- 脚本会自动监听WASD键（或方向键）的输入：
  - `W` 或 `上箭头`：向上移动
  - `S` 或 `下箭头`：向下移动
  - `A` 或 `左箭头`：向左移动
  - `D` 或 `右箭头`：向右移动

### 2. 运行时行为
- 玩家将根据WASD输入平滑移动。
- 移动速度由 `Move Speed` 参数控制。

## 调试技巧

### 1. 检查 `Rigidbody2D`
- 确保玩家GameObject上正确挂载了 `Rigidbody2D` 组件，并且其设置（如 `Body Type` 和 `Constraints`）符合预期。
- 如果没有 `Rigidbody2D`，脚本会在控制台输出错误信息。

### 2. 调整 `Move Speed`
- 如果玩家移动过快或过慢，请在Inspector中调整 `Move Speed` 参数。

### 3. 检查输入
- 确保您的输入设备正常工作。
- 在Unity的 `Edit -> Project Settings -> Input Manager` 中，可以查看和修改 `Horizontal` 和 `Vertical` 轴的配置。

## 扩展功能

### 1. 动态修改速度
您可以在其他脚本中获取 `PlayerMovement` 组件并动态修改其速度：
```csharp
// 示例：在另一个脚本中修改玩家速度
PlayerMovement player = FindObjectOfType<PlayerMovement>();
if (player != null)
{
    player.SetMoveSpeed(10f); // 将玩家速度设置为10
}
```

### 2. 添加动画
- 您可以根据 `movementInput` 的方向来触发玩家的移动动画。

### 3. 限制移动范围
- 如果需要限制玩家的移动范围，可以在 `FixedUpdate` 中添加边界检测逻辑。