using UnityEngine;

public class PlayerGroupController : MonoBehaviour
{
    [Header("跟随物体设置")]
    public GameObject followObject;      // 跟随玩家的物体
    public Vector2 followOffset = new Vector2(0, -1f); // 相对于玩家的偏移
    
    [Header("边界设置")]
    public float padding = 0.5f;       // 边界内边距
    
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        mainCamera = Camera.main;
        
        // 创建跟随物体（如果没有指定）
        if (followObject == null)
        {
            followObject = new GameObject("FollowObject");
            followObject.transform.SetParent(transform);
            
            // 添加SpriteRenderer组件
            SpriteRenderer sr = followObject.AddComponent<SpriteRenderer>();
            // 创建一个简单的精灵作为占位符
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.red;
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            sr.sprite = sprite;
            sr.sortingOrder = -1; // 确保在玩家下方
        }
        
        // 计算边界
        CalculateBounds();
    }

    void Update()
    {
        // 更新跟随物体位置
        if (followObject != null)
        {
            followObject.transform.position = (Vector2)transform.position + followOffset;
        }
        
        // 限制整个组合在屏幕范围内
        ClampToScreen();
    }

    void CalculateBounds()
    {
        // 获取屏幕边界（世界坐标）
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        // 计算物体尺寸（使用Collider或Renderer）
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            objectWidth = sr.bounds.size.x;
            objectHeight = sr.bounds.size.y;
        }
        else
        {
            objectWidth = 1f;
            objectHeight = 1f;
        }
    }

    void ClampToScreen()
    {
        Vector3 pos = transform.position;
        
        // 计算限制边界
        float leftBound = -screenBounds.x + objectWidth/2 + padding;
        float rightBound = screenBounds.x - objectWidth/2 - padding;
        float bottomBound = -screenBounds.y + objectHeight/2 + padding;
        float topBound = screenBounds.y - objectHeight/2 - padding;
        
        // 限制位置
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        
        transform.position = pos;
    }

    // 可视化边界（调试用）
    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            Vector3 size = new Vector3(
                screenBounds.x * 2 - padding * 2,
                screenBounds.y * 2 - padding * 2,
                0.1f
            );
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(camPos, size);
        }
    }

    // 设置跟随物体
    public void SetFollowObject(GameObject obj)
    {
        followObject = obj;
        if (obj != null)
        {
            obj.transform.SetParent(transform);
        }
    }

    // 设置偏移
    public void SetFollowOffset(Vector2 offset)
    {
        followOffset = offset;
    }
}