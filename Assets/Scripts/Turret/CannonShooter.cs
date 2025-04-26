using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonShooter : Building
{
    [SerializeField] private GameObject cannonballPrefab, explodePrefab;
    [SerializeField] private Transform firePoint;
    private float fireCooldown = 1.5f;
    [SerializeField]private float cannonballSpeed = 50f;
    private int damage = 1; 
    
    [SerializeField] private Turret cardData;
    
    public Turret cardToUse
    {
        set
        {
            cardData = value;
            SetUp();
        }
        private get => cardData;
    }

    [SerializeField] private List<Transform> enemiesInRange = new List<Transform>();
    [SerializeField] private float fireTimer;

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        if(cardToUse==null) return;
        fireCooldown = cardToUse.attackCooldown;
        GetComponent<SpriteRenderer>().sprite = cardData.cardSprite;
        damage = cardToUse.damage;
        health = cardData.healthPoints;
        maxHealth = cardData.healthPoints;
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
        GameObject ball = Instantiate(cannonballPrefab, firePoint.position, Quaternion.identity);
        Instantiate(explodePrefab, firePoint.position, Quaternion.identity);
        CannonBall temp =  ball.GetComponent<CannonBall>();
        temp.damage = damage;
        temp.target = target; 
        temp.cannonballSpeed = cannonballSpeed;
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

    public void AddEnemy(Transform target)
    {
        enemiesInRange.Add(target);
    }
    public void RemoveEnemy(Transform target)
    {
        enemiesInRange.Remove(target);
    }
    
}