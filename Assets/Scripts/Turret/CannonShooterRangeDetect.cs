using UnityEngine;

public class CannonShooterRangeDetect : MonoBehaviour
{
    [SerializeField] CannonShooter shooter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            shooter.AddEnemy(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            shooter.RemoveEnemy(other.transform);
        }
    }
}
