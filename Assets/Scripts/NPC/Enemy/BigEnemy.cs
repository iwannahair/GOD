using System;
using UnityEngine;

public class BigEnemy : Enemy
{
    [SerializeField] private Animator animator;

    private void Start()
    { 
        OnHit += HitTrigger;
        animator = animator ? animator : GetComponent<Animator>();
    }

    private void HitTrigger()
    {
        animator.SetTrigger("Hit");
    }
    

    protected override void Die()
    {
        if(gameManager != null)
        {
            gameManager.OnEnemyKilled();
            OnDeath?.Invoke(this);
        }
        gameManager.GetSplash(transform.position).GetComponent<Splash>().ScaleFactor = 1.5f;
        gameManager.ReturnBigEnemy(gameObject);
    }
}
