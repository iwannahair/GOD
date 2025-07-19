using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("交互设置")]
    public bool canInteract = true; // 是否可以交互
    public bool destroyOnInteract = true; // 交互后是否销毁
    
    [Header("视觉反馈")]
    public Color highlightColor = Color.yellow; // 高亮颜色
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // 确保有Collider2D组件
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true; // 设置为触发器
        }
        
        // 确保有SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            // 创建一个简单的圆形精灵作为默认外观
            spriteRenderer.sprite = CreateCircleSprite();
        }
        
        // 保存原始颜色
        originalColor = spriteRenderer.color;
        
        // 确保有Rigidbody2D（可选，用于物理交互）
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true; // 设置为运动学，不受物理影响
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家
        if (other.CompareTag("Player") && canInteract)
        {
            // 高亮显示
            if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
            }
            
            // 触发选项按钮显示
            TriggerInteraction();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        // 玩家离开时恢复原始颜色
        if (other.CompareTag("Player"))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
    
    void TriggerInteraction()
    {
        if (!canInteract) return;
        
        // 查找并激活选项按钮UI
        OptionButtonManager optionManager = FindObjectOfType<OptionButtonManager>();
        if (optionManager != null)
        {
            optionManager.ShowOptions(this);
        }
        else
        {
            Debug.LogWarning("未找到OptionButtonManager，无法显示选项按钮");
        }
    }
    
    // 处理选项选择
    public void OnOptionSelected(int optionIndex)
    {
        Debug.Log($"玩家选择了选项 {optionIndex + 1}");
        
        // 根据选项执行不同的逻辑
        switch (optionIndex)
        {
            case 0:
                ExecuteOption1();
                break;
            case 1:
                ExecuteOption2();
                break;
            case 2:
                ExecuteOption3();
                break;
            default:
                Debug.LogWarning("无效的选项索引");
                break;
        }
        
        // 交互后处理
        if (destroyOnInteract)
        {
            DestroyObject();
        }
        else
        {
            canInteract = false; // 禁用进一步交互
        }
    }
    
    // 选项1的逻辑
    void ExecuteOption1()
    {
        Debug.Log("执行选项1：获得生命值加成");
        // 这里可以添加具体的游戏逻辑
        // 例如：增加玩家生命值
    }
    
    // 选项2的逻辑
    void ExecuteOption2()
    {
        Debug.Log("执行选项2：获得攻击力加成");
        // 这里可以添加具体的游戏逻辑
        // 例如：增加玩家攻击力
    }
    
    // 选项3的逻辑
    void ExecuteOption3()
    {
        Debug.Log("执行选项3：获得移动速度加成");
        // 这里可以添加具体的游戏逻辑
        // 例如：增加玩家移动速度
    }
    
    // 销毁物体
    void DestroyObject()
    {
        // 添加销毁特效（可选）
        // 例如：粒子效果、音效等
        
        Destroy(gameObject);
    }
    
    // 创建简单的圆形精灵
    Sprite CreateCircleSprite()
    {
        // 创建一个简单的白色圆形纹理
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}