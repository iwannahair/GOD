using UnityEngine;

public class ShooterRangeDetect : MonoBehaviour
{
    [SerializeField] Shooter shooter;
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
