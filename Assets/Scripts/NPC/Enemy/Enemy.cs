using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int health = 45;
    private int hps;
    private GameManager gameManager;
    [SerializeField] protected Slider healthSlider;
    public Action<Enemy> OnDeath;
    protected Action OnHit;
    [SerializeField] private GameObject splashPrefab;
    void Start()
    {
        hps = health;
        gameManager = GameManager.instance;
        if (!healthSlider) healthSlider = GetComponentInChildren<Slider>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHit?.Invoke();
        healthSlider.value = (float)health/hps;
        if(health <= 0)
        {
            Die();
        }
    }

    public void TakeDamageByPercentage(int percentage)
    {
        TakeDamage(Mathf.FloorToInt(hps*percentage/100f));
    }

    void Die()
    {
        if(gameManager != null)
        {
            gameManager.OnEnemyKilled();
            OnDeath?.Invoke(this);
        }
        Instantiate(splashPrefab,transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}