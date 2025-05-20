using UnityEngine;

public class HumanAttacker : MonoBehaviour
{
    [SerializeField] private float damage = 20f; // 敌人造成的伤害
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞的是否是人类
        HumanFollower human = collision.gameObject.GetComponent<HumanFollower>();
        if (human != null)
        {
            // 对人类造成伤害
            human.TakeDamage(damage);
        }
    }
    
    // 如果使用触发器碰撞
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的是否是人类
        HumanFollower human = other.GetComponent<HumanFollower>();
        if (human != null)
        {
            // 对人类造成伤害
            human.TakeDamage(damage);
        }
    }
}