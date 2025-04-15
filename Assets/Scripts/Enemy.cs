using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Range(1,100)]private int health = 30;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(gameManager != null)
        {
            gameManager.OnEnemyKilled();
        }
        Destroy(gameObject);
    }
}