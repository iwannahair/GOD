using System;
using UnityEngine;

public class EnemyColliderHolder : MonoBehaviour
{
     
    // 删除 Building 相关变量
    // [SerializeField] private Building targetBuilding;
    [SerializeField] private int damage;
    [SerializeField] private EnemyAI enemyAI;

    private void Start()
    {
        if (enemyAI)
        {
            damage = enemyAI.Damage;
            // 删除 Building 相关代码
            // targetBuilding = enemyAI.TargetBuilding;
        }
        else
        {
            Debug.LogError("The enemy AI was not assigned.");
        }
    }

 
}
