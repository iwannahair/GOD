using System;
using System.Collections;
using UnityEngine;

public class CannonBallExplode : MonoBehaviour
{
    public int damage = 8;
    [SerializeField] private float existTime = 0.3f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);  
            }
            
        }
    }

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(existTime);
        Destroy(gameObject);
    }
}
