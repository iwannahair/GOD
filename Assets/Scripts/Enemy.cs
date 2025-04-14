using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 30;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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