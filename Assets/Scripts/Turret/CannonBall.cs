using System;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Transform target { get; set;}
    public float cannonballSpeed;
    public int damage;
    private Rigidbody2D rb2d;
    [SerializeField] private GameObject explodePrefab;
    private Vector3 lastPos;
    private void Start()
    {
        rb2d  = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target)
        {
            lastPos = target.position;
        }
        
        Vector3 direction = (lastPos - transform.position).normalized;
        rb2d.linearVelocity = direction * cannonballSpeed;
        if ((transform.position - lastPos).magnitude < 0.5f)
        {
            Instantiate(explodePrefab, transform.position, Quaternion.identity).GetComponent<CannonBallExplode>().damage = damage;
            Destroy(gameObject);
        }
    }
}
