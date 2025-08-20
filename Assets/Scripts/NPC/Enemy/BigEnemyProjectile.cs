using UnityEngine;

/// <summary>
/// BigEnemy发射的投射物，能够自动寻找并攻击最近的敌人
/// </summary>
public class BigEnemyProjectile : MonoBehaviour
{
    [Header("投射物设置")]
    [Tooltip("投射物速度")]
    [SerializeField] private float speed = 10f;
    [Tooltip("投射物伤害")]
    [SerializeField] private int damage = 20;
    [Tooltip("投射物生命周期（秒）")]
    [SerializeField] private float lifetime = 5f;

    // 刚体组件的引用
    private Rigidbody2D rb;
    // 目标敌人的Transform组件
    private Transform targetEnemy;
    // 是否已锁定目标
    private bool hasTarget = false;
    // 发射投射物的对象（用于避免自伤）
    private GameObject owner;

    private void Awake()
    {
        // 获取此游戏对象上的 Rigidbody2D 组件
        rb = GetComponent<Rigidbody2D>();
        
        // 在指定的生命周期时间后销毁此游戏对象
        Destroy(gameObject, lifetime);
    }

    private void Start()
    {
        // 如果没有设置目标，投射物将直线飞行
        if (targetEnemy == null)
        {
            // 设置默认方向（向右飞行）
            if (rb != null)
            {
                rb.linearVelocity = transform.right * speed;
            }
        }
    }

    private void Update()
    {
        // 如果已锁定目标且目标仍然存在，则朝目标移动
        if (hasTarget && targetEnemy != null)
        {
            MoveTowardsTarget();
        }
        // 如果目标丢失，继续直线飞行
        else if (hasTarget && targetEnemy == null)
        {
            // 目标已被销毁，保持当前方向继续飞行
            hasTarget = false;
        }
    }

    /// <summary>
    /// 设置投射物的目标
    /// </summary>
    /// <param name="target">目标Transform</param>
    public void SetTarget(Transform target)
    {
        if (target != null)
        {
            targetEnemy = target;
            hasTarget = true;
            
            // 立即开始朝向目标移动
            MoveTowardsTarget();
        }
    }

    /// <summary>
    /// 设置投射物的发射者（用于避免自伤）
    /// </summary>
    /// <param name="ownerObject">发射投射物的对象</param>
    public void SetOwner(GameObject ownerObject)
    {
        owner = ownerObject;
    }

    /// <summary>
    /// 朝着锁定的目标移动
    /// </summary>
    private void MoveTowardsTarget()
    {
        // 如果目标丢失，则重置目标状态
        if (targetEnemy == null)
        {
            hasTarget = false;
            return;
        }
        
        // 计算朝向目标的方向
        Vector3 direction = (targetEnemy.position - transform.position).normalized;
        
        // 设置刚体的速度，使其朝目标移动
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    /// <summary>
    /// 当此碰撞体进入另一个碰撞体时调用
    /// </summary>
    /// <param name="other">此碰撞体进入的另一个碰撞体</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰撞到的是发射者，则忽略
        if (other.gameObject == owner)
        {
            return;
        }

        // 检查碰撞到的对象是否带有 "Enemy" 或 "BigEnemy" 标签
        if (other.CompareTag("Enemy") || other.CompareTag("BigEnemy"))
        {
            // 尝试从碰撞的对象上获取 Enemy 组件（BigEnemy也继承自Enemy）
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 对敌人造成伤害
                enemy.TakeDamage(damage);
            }
            
            // 销毁投射物
            DestroyProjectile();
        }
    }

    /// <summary>
    /// 销毁投射物游戏对象
    /// </summary>
    private void DestroyProjectile()
    {
        // 销毁此游戏对象
        Destroy(gameObject);
    }

    /// <summary>
    /// 在Scene视图中绘制辅助线框，便于调试
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 如果已锁定目标，则绘制一条指向目标的红线
        if (hasTarget && targetEnemy != null)
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetEnemy.position);
        }
    }
}