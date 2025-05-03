using System;
using UnityEngine;

public class BigEnemy : Enemy
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        OnHit += HitTrigger;
        animator = animator ? animator : GetComponent<Animator>();
    }

    private void HitTrigger()
    {
        animator.SetTrigger("Hit");
    }

    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.BigMonsterKilledAmount++;
        }
    }
}
