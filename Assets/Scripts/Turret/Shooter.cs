using System;
using System.Collections.Generic;
using UnityEngine;

// 修改继承关系，不再继承自 Building
public class Shooter : MonoBehaviour
{
    [SerializeField] protected float fireCooldown = 1.5f;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected List<Transform> enemiesInRange = new List<Transform>();
    [SerializeField] protected float fireTimer;
    [SerializeField] protected Sprite shooterSprite;
    // 添加原来在 Building 中的属性
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int humanInside;
    [SerializeField] protected int humanCapacity = 1;
    [SerializeField] protected UnityEngine.UI.Slider healthSlider;

    private void Start()
    {
        SetUp();
    }

    protected void SetUp()
    {
        if(shooterSprite != null)
            GetComponent<SpriteRenderer>().sprite = shooterSprite;
        health = maxHealth;
        humanInside = humanCapacity;
    }
    
    public void AddEnemy(Transform target)
    {
        enemiesInRange.Add(target);
    }
    
    public void RemoveEnemy(Transform target)
    {
        enemiesInRange.Remove(target);
    }
    
    // 添加原来在 Building 中的方法
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if(healthSlider != null)
            healthSlider.value = (float)health/maxHealth;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
