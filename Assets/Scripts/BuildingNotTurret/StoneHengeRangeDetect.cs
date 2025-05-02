using System.Collections;
using UnityEngine;

public class StoneHengeRangeDetect : MonoBehaviour
{
    [SerializeField] private StoneHenge stoneHenge;
    private Collider2D _collider2D;
    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        stoneHenge.OnChange += Refresh;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log(other.name);
            if (other.TryGetComponent(out Enemy enemy))
            {
                if (stoneHenge.TrappedEnemiesNumber<stoneHenge.MaxEnemyToTrap)
                {
                    stoneHenge.TrappedEnemies.Add(enemy);
                    stoneHenge.Trap(enemy);
                }

                for (int i=0;i<stoneHenge.TrappedEnemiesNumber;i++)
                {
                    if (!stoneHenge.TrappedEnemies[i])
                    {
                        stoneHenge.Trap(enemy,i);
                        return;
                    }
                }
            }
        }
    }

    private void Refresh()
    {
        StartCoroutine(RefreshCollider());
    }

    private IEnumerator RefreshCollider()
    {
        _collider2D.enabled = false;
        yield return new WaitForFixedUpdate();
        _collider2D.enabled = true;
    }
}
