using UnityEngine;

public class BallistaShooter : Shooter
{
    [SerializeField] private GameObject boltPrefab;
    [SerializeField] private float boltSpeed = 20f;
    void FixedUpdate()
    {
        fireTimer -= Time.fixedDeltaTime;

        if (fireTimer <= 0f && enemiesInRange.Count > 0)
        {
            Transform target = GetClosestEnemy();
            if (target != null)
            {
                FireAt(target);
                fireTimer = fireCooldown;
            }
        }
    }

    private void FireAt(Transform target)
    {
        GameObject bolt = Instantiate(boltPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = transform.position -target.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bolt.transform.rotation = Quaternion.Euler(0, 0, angle);
        BoltArrow boltArrow =  bolt.GetComponent<BoltArrow>();
        boltArrow.damage = damage;
        boltArrow.target = target; 
        boltArrow.boltSpeed = boltSpeed;
    }
    private Transform GetClosestEnemy()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
}
