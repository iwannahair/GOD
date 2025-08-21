using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // 将 health 字段的访问修饰符从 private 改为 protected
    // 这使得所有继承自 Enemy 的子类（如 BigEnemy）都可以在 Unity Inspector 中看到并独立配置自己的 health 值
    // 同时，这个字段仍然受到保护，不会被不相关的外部类随意访问，维持了良好的封装性
    [SerializeField] protected int health = 45;
    [SerializeField] public Slider healthSlider;

    
    private int hps;
    protected GameManager gameManager;
    public Action<Enemy> OnDeath;
    protected Action OnHit;
    
    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public int CurrentHealth => health;

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