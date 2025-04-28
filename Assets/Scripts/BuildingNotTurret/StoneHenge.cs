using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoneHenge : Building
{
     [SerializeField] private Transform pointToTrap;
     [SerializeField] private float pullSpeed;
     [SerializeField] private int maxEnemyToTrap = 2;
     [SerializeField] List<Enemy> trappedEnemies;
     [SerializeField, Range(0f, 5f)] private float randomOffset;
     public int TrappedEnemiesNumber => trappedEnemies.Count;
     public int MaxEnemyToTrap => maxEnemyToTrap;
     private void Awake()
     {
          maxHealth = 3000;
          health = maxHealth;
          width = 5;
          height = 5;
          humanInside = 10;
     }

     public void Trap(Enemy enemy)
     {
          trappedEnemies.Add(enemy);
          enemy.GetComponent<EnemyAI>().enabled = false;
          enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
          Collider2D[] collider2Ds = enemy.GetComponentsInChildren<Collider2D>();
          foreach (var col in collider2Ds)
          {
               col.enabled = false;
          }
          StartCoroutine(PullEnemy(enemy.transform));
     }

     private IEnumerator PullEnemy(Transform enemyTran)
     {
          float timeToPull = Vector2.Distance(enemyTran.position, pointToTrap.position)/pullSpeed;
          float timeCounter = 0;
          Vector2 endPoint = pointToTrap.position + new Vector3(Random.Range(-randomOffset,randomOffset),Random.Range(-randomOffset,randomOffset));
          Vector2 startPoint = enemyTran.position;
          while (timeToPull <= timeCounter)
          {
               timeCounter += Time.fixedDeltaTime;
               enemyTran.position= Vector2.Lerp(startPoint, endPoint, timeCounter / timeToPull);
               yield return new WaitForFixedUpdate();
          }
          enemyTran.position = endPoint;
     }

     private void RecoverEnemy()
     {
          foreach (var enemy in trappedEnemies)
          {
               enemy.GetComponent<EnemyAI>().enabled = true;
               Collider2D[] collider2Ds = enemy.GetComponentsInChildren<Collider2D>();
               foreach (var col in collider2Ds)
               {
                    col.enabled = true;
               }
          }
     }

     private void OnDestroy()
     {
          RecoverEnemy();
     }
}
