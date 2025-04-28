using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int health = 45;
    private int hps;
    private GameManager gameManager;
    [SerializeField] protected Slider healthSlider;
    void Start()
    {
        hps = health;
        gameManager = GameManager.instance;
        if (!healthSlider) healthSlider = GetComponentInChildren<Slider>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = (float)health/hps;
        if(health <= 0)
        {
            Die();
        }
    }

    public void TakeDamageByPercentage(int percentage)
    {
        health -= hps*percentage/100;
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