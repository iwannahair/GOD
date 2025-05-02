using System;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    protected int health = 300;
    protected int maxHealth = 300;
    protected int width = 1,height = 1;
    [SerializeField] protected Slider healthSlider;
    protected int humanInside;

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = (float)health/maxHealth;
        if (health <= 0)
        {
            if (humanInside>0)
            {
                GameManager.instance.CurrentFollowerNumber -= humanInside;
            }
            Destroy(gameObject);
        }
    }
}
