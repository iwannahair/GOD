using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int health = 45;
    private int hps;
    protected GameManager gameManager;
    [SerializeField] protected Slider healthSlider;
    public Action<Enemy> OnDeath;
    protected Action OnHit;

    void Awake()
    {
        hps = health;
        healthSlider.value = (float)health/hps;
        gameManager = GameManager.instance;
        if (!healthSlider) healthSlider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        health = hps;
        healthSlider.value = (float)health/hps;
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

    protected virtual void Die()
    {
        if(gameManager != null)
        {
            gameManager.OnEnemyKilled();
            OnDeath?.Invoke(this);
        }

        gameManager.GetSplash(transform.position);
        gameManager.ReturnEnemy(gameObject);
    }

}