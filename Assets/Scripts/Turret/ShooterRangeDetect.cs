using System.Collections; 
using UnityEngine;

public class ShooterRangeDetect : MonoBehaviour
{
    [SerializeField] private Shooter shooter;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float range;
    [SerializeField] private Collider2D[] _collider2Ds;
    private void Start()
    {
        StartCoroutine(Detect());
    }

    private IEnumerator Detect()
    {
        while (true)
        {
            _collider2Ds = Physics2D.OverlapCircleAll(transform.position,range, enemyMask);
            shooter.ResetList();
            foreach (var collider2D in _collider2Ds)
            {
                shooter.AddEnemy(collider2D.transform);
            }
            yield return new WaitForSeconds(shooter.Cooldown);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
