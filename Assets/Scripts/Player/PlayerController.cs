using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    
    [Header("生命值设置")]
    public int maxHealth = 100;
    private int currentHealth;  // 添加当前生命值变量
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer _renderer;
    [SerializeField]private AudioSource _audio;
    [SerializeField] private GameObject axe;

    void Start()
    {
        _audio = _audio ? _audio : gameObject.GetComponent<AudioSource>();
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // 初始化生命值
        if (GameManager.instance) GameManager.instance.OnPlayerHealthChanged += UpdateHealth;
    }

    public void UpdateHealth()
    {
        currentHealth = GameManager.instance.PlayerHealth;
        TakeDamage(0);
    }
    void Update()
    {
        // WASD输入处理
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 移动角色
        rb.MovePosition(rb.position + movement.normalized * (moveSpeed * Time.fixedDeltaTime));
        
        // 可选：使角色朝向移动方向
        if(movement != Vector2.zero)
        {
            if ((movement.x < 0 || movement.y > 0) &&!_renderer.flipX )
            {
                _renderer.flipX = true;
            }

            if ((movement.x > 0 || movement.y < 0)&& _renderer.flipX )
            {
                _renderer.flipX = false;
            }
        }
    }

    private void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // 游戏结束逻辑
        Debug.Log("玩家死亡"); 
        StartCoroutine(TurnToStone()); 
    }

    private IEnumerator TurnToStone()
    {
        _audio.Play();
        axe.SetActive(false);
        Color temp = _renderer.color;
        Color targetColor = new Color(85 / 255f, 85 / 255f, 85 / 255f);
        float timer = 0f;
        while (timer<1f)
        {
            timer+=Time.fixedDeltaTime;
            _renderer.color = Color.Lerp(temp, targetColor, timer / 1f);
            yield return new WaitForFixedUpdate();
        }
        enabled = false;
    }

    private void OnDestroy()
    {
        if(GameManager.instance) GameManager.instance.OnPlayerHealthChanged -= UpdateHealth;
    }
}