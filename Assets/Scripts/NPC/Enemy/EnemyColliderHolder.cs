using System;
using UnityEngine;

public class EnemyColliderHolder : MonoBehaviour
{
     
    [SerializeField] private Building targetBuilding;
    [SerializeField] private int damage;
    [SerializeField] private EnemyAI enemyAI;

    private void Start()
    {
        if (enemyAI)
        {
            damage = enemyAI.Damage;
            targetBuilding = enemyAI.TargetBuilding;
        }
        else
        {
            Debug.LogError("The enemy AI was not assigned.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out targetBuilding))
        {
            targetBuilding.TakeDamage(damage);
        }
    }
}
