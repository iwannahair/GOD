using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage); // 现在这个方法存在了
            }
            Destroy(gameObject);
        }
    }
}