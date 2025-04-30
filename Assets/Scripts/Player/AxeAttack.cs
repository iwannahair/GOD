using System;
using System.Collections;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Range(0f, 3f)] private float attackCooldown = 1f;
    [SerializeField, Range(1, 100)]private int attackDamage = 45;

    [SerializeField, Range(1f, 45f)] private float rotateSpeed = 15;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Collider2D col;
    [SerializeField] private float timer ;
    [SerializeField,Range(30,100)]private int tickNumber = 50;

    private float _rotateFactor = 1f;

    private int percentage = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        timer = 360f/rotateSpeed/tickNumber;
        col = GetComponent<Collider2D>();
        spriteRenderer = spriteRenderer!=null ? spriteRenderer :GetComponentInChildren<SpriteRenderer>();
        _audioSource = _audioSource!=null ? _audioSource: GetComponentInChildren<AudioSource>();
        StartCoroutine(Spin());
        if (GameManager.instance)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged += UpdateRotateFactor;
            GameManager.instance.OnPlayerDamageChanged += UpdateDamagePercentage;
        }
    }

    private void UpdateRotateFactor()
    {
        _rotateFactor = GameManager.instance.PlayerAttackSpeed / 100f;
    }

    private void UpdateDamagePercentage()
    {
        percentage = GameManager.instance.PlayerDamage;
    }
    private IEnumerator Spin()
    {
        while (true)
        {
            
            while (timer>0)
            {
                timer -= Time.fixedDeltaTime;
                transform.Rotate(Vector3.forward, rotateSpeed*_rotateFactor);
                yield return new WaitForFixedUpdate();
            }
            timer = 360f/(rotateSpeed*_rotateFactor)/tickNumber;
            transform.rotation = Quaternion.identity;
            DisableGFXandCollider();
            yield return new WaitForSeconds(attackCooldown);
            EnableGFXandCollider();
        }
        
    }

    private void OnDisable()
    {
        StopCoroutine(Spin());
    }

    private void DisableGFXandCollider()
    {
        col.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void EnableGFXandCollider()
    {
        col.enabled = true;
        spriteRenderer.enabled = true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                _audioSource.Play();
                enemy.TakeDamageByPercentage(percentage);
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged -= UpdateRotateFactor;
            GameManager.instance.OnPlayerDamageChanged -= UpdateDamagePercentage;
        }
    }
}
