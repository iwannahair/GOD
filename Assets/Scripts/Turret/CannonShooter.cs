using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CannonShooter : Shooter
{
    [SerializeField] private GameObject cannonballPrefab, explodePrefab;
    [SerializeField]private float cannonballSpeed = 50f;
 
    private void Start()
    {
        SetUp();
    }


    void FixedUpdate()
    {
        fireTimer -= Time.fixedDeltaTime;

        if (fireTimer <= 0f && enemiesInRange.Count > 0)
        {
            Transform target = GetFurthestEnemy();
            if (target != null)
            {
                FireAt(target);
                fireTimer = fireCooldown;
            }
        }
    }

    private void FireAt(Transform target)
    {
        if(GameManager.instance) 
            switch (Random.Range(0, 2))
            {
                case 0:
                    GameManager.instance.AudioPlayerManager.PlayCannon1Sound();
                    break;
                case 1:
                    GameManager.instance.AudioPlayerManager.PlayCannon2Sound();
                    break;
                default:
                    Debug.LogError("CannonShooter: Invalid value");
                    break;
            }
        GameObject ball = Instantiate(cannonballPrefab, firePoint.position, Quaternion.identity);
        Instantiate(explodePrefab, firePoint.position, Quaternion.identity);
        CannonBall cannonBall =  ball.GetComponent<CannonBall>();
        cannonBall.damage = damage;
        cannonBall.target = target; 
        cannonBall.cannonballSpeed = cannonballSpeed;
    }

    private Transform GetFurthestEnemy()
    {
        Transform furthest = null;
        float maxDistance = 0f;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthest = enemy;
            }
        }

        return furthest;
    }
}