using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    // 将 health 字段的访问修饰符从 private 改为 protected
    // 这使得所有继承自 Enemy 的子类（如 BigEnemy）都可以在 Unity Inspector 中看到并独立配置自己的 health 值
    // 同时，这个字段仍然受到保护，不会被不相关的外部类随意访问，维持了良好的封装性
    // 兼容性：通过 FormerlySerializedAs 将旧序列化名 "health" 平滑迁移到新字段名 "EnemyHP"，避免原有 Prefab/场景数据丢失
    [FormerlySerializedAs("health")]
    [SerializeField] protected int EnemyHP = 45;
    // 用于在 UI 上显示生命值进度的 Slider（从子物体中自动获取）
    [SerializeField] protected Slider healthSlider;

    
    private int hps;
    protected GameManager gameManager;
    public Action<Enemy> OnDeath;
    protected Action OnHit;
    
    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public int CurrentHealth => EnemyHP;

    void Awake()
    {
        hps = EnemyHP;
        healthSlider.value = (float)EnemyHP/hps;
        gameManager = GameManager.instance;
        if (!healthSlider) healthSlider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        EnemyHP = hps;
        healthSlider.value = (float)EnemyHP/hps;
    }

    public void TakeDamage(int damage)
    {
        EnemyHP -= damage;
        OnHit?.Invoke();
        healthSlider.value = (float)EnemyHP/hps;
        if(EnemyHP <= 0)
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