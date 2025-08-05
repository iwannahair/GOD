using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("发射设置")]
    public GameObject projectilePrefab;  // 要发射的预制体
    public Transform firePoint;          // 发射点
    public Vector2 fireOffset = new Vector2(0, 0.5f); // 相对于玩家的偏移
    
    [Header("发射控制")]
    public float fireRate = 1f;         // 发射频率（每秒）
    public KeyCode fireKey = KeyCode.Space; // 发射按键
    
    [Header("自动发射")]
    public bool autoFire = false;       // 是否启用自动发射
    public bool autoFireOnStart = false; // 游戏开始时是否自动开始发射
    
    [Header("自动创建发射点")]
    public bool autoCreateFirePoint = true; // 自动创建发射点
    
    private float nextFireTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        // 自动创建发射点
        if (autoCreateFirePoint && firePoint == null)
        {
            CreateFirePoint();
        }
        
        // 如果启用自动发射，立即设置下次发射时间
        if (autoFireOnStart)
        {
            autoFire = true;
            nextFireTime = Time.time;
        }
    }

    void Update()
    {
        // 实时计算间隔时间
        float interval = 1f / fireRate;

        // 处理手动发射
        if (Input.GetKeyDown(fireKey) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + interval;
        }
        
        // 处理自动发射
        if (autoFire && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + interval;
        }
        
        // 更新发射点位置
        UpdateFirePointPosition();
    }
    
    void CreateFirePoint()
    {
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(transform);
        firePointObj.transform.localPosition = fireOffset;
        firePoint = firePointObj.transform;
        
        // 添加可视化标记（调试用）
        SpriteRenderer sr = firePointObj.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(8, 8);
        Color[] pixels = new Color[8 * 8];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.yellow;
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f));
        sr.sprite = sprite;
        sr.sortingOrder = 10; // 确保可见
    }

    void UpdateFirePointPosition()
    {
        if (firePoint != null)
        {
            firePoint.position = (Vector2)transform.position + fireOffset;
        }
    }

    void Fire()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // 保持预制体的原始旋转
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);
            
            // 设置发射方向 - 向上发射
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 确保向上发射（世界坐标系的上方）
                rb.linearVelocity = Vector2.up * 5f;
            }
            else
            {
                // 如果没有Rigidbody2D，使用Transform移动
                projectile.transform.position += Vector3.up * 5f * Time.deltaTime;
            }
            
            // 设置发射物体的标签
            projectile.tag = "PlayerProjectile";
        }
    }

    // 可视化发射点
    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }

    // 公共方法：设置发射预制体
    public void SetProjectilePrefab(GameObject prefab)
    {
        projectilePrefab = prefab;
    }

    // 公共方法：设置发射频率
    public void SetFireRate(float rate)
    {
        fireRate = Mathf.Max(0.1f, rate); // 防止频率过高
    }
    
    // 控制自动发射
    public void SetAutoFire(bool enabled)
    {
        autoFire = enabled;
        if (enabled)
        {
            nextFireTime = Time.time; // 立即开始发射
        }
    }
    
    // 切换自动发射状态
    public void ToggleAutoFire()
    {
        autoFire = !autoFire;
        if (autoFire)
        {
            nextFireTime = Time.time;
        }
    }
}