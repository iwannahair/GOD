using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BoltArrow : MonoBehaviour
{
    [SerializeField]public int damage = 11;
    public Transform target { get; set;}
    public float boltSpeed;
    public float disappearTime = 0.5f;
    private bool isDestroying;
    private Rigidbody2D rb2d;
    private Vector3 lastPos;
    private void Start()
    {
        rb2d  = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDestroying) return;
        if (target)
        {
            lastPos = target.position;
        }
        
        Vector3 direction = (lastPos - transform.position).normalized;
        rb2d.linearVelocity = direction * boltSpeed;
        if ((transform.position - lastPos).magnitude < 0.5f)
        {
            if (!target) {
                StartCoroutine(LateDestroy());
                isDestroying = true;
            } 
        }
    }

    IEnumerator LateDestroy()
    {
        yield return new WaitForSeconds(disappearTime);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage); // 现在这个方法存在了
            }
            StopAllCoroutines();
            transform.SetParent(collision.transform);
            if (TryGetComponent(out Rigidbody2D rb2d))
            {
                Destroy(rb2d);
            }
            Destroy(this);
        }
    }
}
