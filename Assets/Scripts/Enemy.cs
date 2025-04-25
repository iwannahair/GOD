using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int health = 45;
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