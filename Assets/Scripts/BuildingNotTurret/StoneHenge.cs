using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoneHenge : Building
{
     [SerializeField] private Transform pointToTrap;
     [SerializeField] private float pullSpeed = 1f;
     [SerializeField] private int maxEnemyToTrap = 2;
     [SerializeField] List<Enemy> trappedEnemies;
     [SerializeField, Range(0f, 5f)] private float randomOffset;
     public int TrappedEnemiesNumber => trappedEnemies.Count;
     public List<Enemy> TrappedEnemies => trappedEnemies;
     public int MaxEnemyToTrap => maxEnemyToTrap;
     public Action  OnChange;
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
          
          enemy.OnDeath += _ =>
          {
               trappedEnemies.Remove(enemy);
               OnChange?.Invoke();
          };
          
          enemy.GetComponent<EnemyAI>().enabled = false;
          enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
          Collider2D[] collider2Ds = enemy.GetComponentsInChildren<Collider2D>();
          if (collider2Ds[1])
          {
               collider2Ds[1].enabled = false;
          }
 
          if (collider2Ds[0])
          {
               collider2Ds[0].isTrigger = true;
          }
          StartCoroutine(PullEnemy(enemy.transform));
     }

     public void Trap(Enemy enemy, int index)
     {
          trappedEnemies[index] = enemy;
          Trap(enemy);
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
               if(!enemy) continue;
               enemy.GetComponent<EnemyAI>().enabled = true;
               Collider2D[] collider2Ds = enemy.GetComponentsInChildren<Collider2D>();
               if (collider2Ds[1])
               {
                    collider2Ds[1].enabled = true;
               }
               if (collider2Ds.Length > 0)
               {
                    collider2Ds[0].isTrigger = false;
               }
          }
          trappedEnemies.Clear();
     }

     private void OnDestroy()
     {
          RecoverEnemy();
     }
}
