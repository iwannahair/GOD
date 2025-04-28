using System;
using Unity.VisualScripting;
using UnityEngine;

public class StoneHengeRangeDetect : MonoBehaviour
{
    [SerializeField] private StoneHenge stoneHenge;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Enemy enemy)&&stoneHenge.TrappedEnemiesNumber<stoneHenge.MaxEnemyToTrap)
            {
                stoneHenge.Trap(enemy);
            }
        }
    }
}
