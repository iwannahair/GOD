using UnityEngine;

/// <summary>
/// 神树投射物控制器 - 管理投射物的移动、碰撞和伤害逻辑
/// </summary>
public class TreeProjectile : MonoBehaviour
{
    public float speed = 10f; // 投射物移动速度
    private float damage = 10f; // 投射物造成的伤害
    public string enemyTag = "Enemy"; // 敌人标签
    public GameObject hitEffectPrefab; // 命中效果预制体（可选）

    private Transform targetEnemy; // 目标敌人
    private bool hasHitTarget = false; // 是否已命中目标，防止重复伤害

    /// <summary>
    /// 设置投射物的目标和伤害
    /// </summary>
    /// <param name="target">目标敌人的Transform</param>
    /// <param name="dmg">造成的伤害值</param>
    /// <param name="projectileSpeed">投射物速度</param>
    public void Initialize(Transform target, float dmg, float projectileSpeed)
    {
        targetEnemy = target;
        damage = dmg;
        speed = projectileSpeed;
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            // 如果目标消失，销毁投射物
            Destroy(gameObject);
            return;
        }

        // 朝目标移动
        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, speed * Time.deltaTime);

        // 如果非常接近目标，直接命中
        if (Vector2.Distance(transform.position, targetEnemy.position) < 0.1f)
        {
            HitTarget(targetEnemy.gameObject);
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否碰撞到带敌人标签的对象
        if (other.CompareTag(enemyTag))
        {
            // 确保只命中一次目标，或者命中其他敌人
            if (other.transform == targetEnemy || targetEnemy == null) // 如果命中目标或目标已消失但命中其他敌人
            {
                HitTarget(other.gameObject);
            }
        }
    }

    /// <summary>
    /// 命中目标并造成伤害
    /// </summary>
    /// <param name="hitObject">被命中的GameObject</param>
    private void HitTarget(GameObject hitObject)
    {
        if (hitObject == null)
            return;

        // 确保只处理一次伤害
        if (hasHitTarget)
            return;

        hasHitTarget = true;

        EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage((int)damage);
            Debug.Log($"投射物命中了 {hitObject.name}，造成 {damage} 点伤害");
        }

        // 播放命中效果
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // 销毁投射物
        Destroy(gameObject);
    }
}