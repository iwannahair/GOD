using System;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Building
{
    [SerializeField] protected float fireCooldown = 1.5f;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected Turret cardData;
    [SerializeField] protected List<Transform> enemiesInRange = new List<Transform>();
    [SerializeField] protected float fireTimer;
    public Turret cardToUse
    {
        set
        {
            cardData = value;
            SetUp();
        }
        private get => cardData;
    }
    
    private void Start()
    {
        if (cardData == null) return;
        SetUp();
    }

    protected void SetUp()
    {
        if(cardToUse==null) return;
        fireCooldown = cardToUse.attackCooldown;
        GetComponent<SpriteRenderer>().sprite = cardData.cardSprite;
        damage = cardToUse.damage;
        health = cardData.healthPoints;
        maxHealth = cardData.healthPoints;
        humanInside = cardToUse.followersCapacity;
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
