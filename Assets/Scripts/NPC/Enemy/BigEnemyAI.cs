using System;
using UnityEngine;

public class BigEnemyAI : EnemyAI
{   // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = animator ? animator : GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
         animator.SetFloat("VelocityX",  rb.linearVelocity.x);
    }
}
