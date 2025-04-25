using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "Scriptable Objects/Turret")] 
public class Turret : Card
{
    public float attackRange, attackCooldown, damageRadius;
    public int damage;
}
