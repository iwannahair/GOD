using System;
using System.Collections;
using UnityEngine;

public class BigEnemy : Enemy
{
    [Header("BigEnemy设置")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject replacementPrefab; // 死亡时替换的预制体
    [SerializeField] private float healthDecayRate = 10f; // 生命值衰减速率（每秒）
    
    private bool isHealthDecaying = false; // 是否正在衰减生命值
    private Coroutine healthDecayCoroutine; // 生命值衰减协程

    private void Start()
    { 
        OnHit += HitTrigger;
        animator = animator ? animator : GetComponent<Animator>();
    }

    private void HitTrigger()
    {
        animator.SetTrigger("Hit");
    }
    
    /// <summary>
    /// 开始生命值衰减
    /// </summary>
    public void StartHealthDecay()
    {
        if (!isHealthDecaying)
        {
            isHealthDecaying = true;
            healthDecayCoroutine = StartCoroutine(HealthDecayCoroutine());
        }
    }
    
    /// <summary>
    /// 生命值衰减协程
    /// </summary>
    private IEnumerator HealthDecayCoroutine()
    {
        while (isHealthDecaying)
        {
            yield return new WaitForSeconds(1f); // 每秒衰减一次
            TakeDamage(Mathf.RoundToInt(healthDecayRate));
        }
    }
    
    /// <summary>
    /// 停止生命值衰减
    /// </summary>
    public void StopHealthDecay()
    {
        if (isHealthDecaying)
        {
            isHealthDecaying = false;
            if (healthDecayCoroutine != null)
            {
                StopCoroutine(healthDecayCoroutine);
                healthDecayCoroutine = null;
            }
        }
    }

    protected override void Die()
    {
        // 停止生命值衰减
        StopHealthDecay();
        
        if(gameManager != null)
        {
            gameManager.OnEnemyKilled();
            OnDeath?.Invoke(this);
        }
        
        // 如果设置了替换预制体，则在当前位置实例化新的预制体
        if (replacementPrefab != null)
        {
            Instantiate(replacementPrefab, transform.position, transform.rotation);
            
            // 当BigEnemy变成另一种预制体时，调用StatsForGod中的方法增加percentText的值
            if (StatsForGod.instance != null)
            {
                StatsForGod.instance.OnBigEnemyTransformed();
            }
        }
        
        gameManager.GetSplash(transform.position).GetComponent<Splash>().ScaleFactor = 1.5f;
        gameManager.ReturnBigEnemy(gameObject);
    }
    
    private void OnDisable()
    {
        // 当对象被禁用时停止生命值衰减
        StopHealthDecay();
    }
}
