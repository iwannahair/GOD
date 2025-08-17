using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 斧子攻击组件 - 负责单个斧子的攻击逻辑和自身旋转
/// 配合AxeManager使用，处理敌人碰撞检测和伤害计算
/// </summary>
public class AxeAttack : MonoBehaviour
{
    [Header("音效设置")]
    [SerializeField] private AudioSource _audioSource; // 斧子攻击音效源
    
    [Header("旋转设置")]
    [SerializeField, Range(1f, 45f)] private float rotateSpeed = 15f; // 斧子自身旋转速度
    [SerializeField] private SpriteRenderer spriteRenderer; // 斧子精灵渲染器
    
    [Header("攻击设置")]
    private Collider2D col; // 斧子碰撞器
    [SerializeField] private float timer; // 计时器（暂未使用）
    [SerializeField, Range(30, 100)] private int tickNumber = 50; // 刻度数量（暂未使用）

    // 私有变量
    private float _rotateFactor = 1f; // 旋转速度因子
    private int percentage = 100; // 伤害百分比

    /// <summary>
    /// 静态初始角度，用于设置斧子的起始旋转角度
    /// 由AxeManager在创建斧子时设置，避免多个斧子重叠
    /// </summary>
    public static float InitAngle = 0;
    /// <summary>
    /// 初始化斧子组件
    /// 设置组件引用，订阅游戏管理器事件
    /// </summary>
    private void Start()
    {    
        // 计算计时器值（基于旋转速度和刻度数量）
        timer = 360f / rotateSpeed / tickNumber;
        
        // 获取组件引用
        col = GetComponent<Collider2D>();
        spriteRenderer = spriteRenderer != null ? spriteRenderer : GetComponentInChildren<SpriteRenderer>();
        _audioSource = _audioSource != null ? _audioSource : GetComponentInChildren<AudioSource>();
        
        // 订阅游戏管理器事件，用于动态调整斧子属性
        if (GameManager.instance)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged += UpdateRotateFactor;
            GameManager.instance.OnPlayerDamageChanged += UpdateDamagePercentage;
        }
    }

    /// <summary>
    /// 初始化斧子的旋转因子和位置
    /// 由AxeManager调用，设置斧子的初始状态
    /// </summary>
    /// <param name="rotateFactor">旋转速度因子</param>
    /// <param name="radius">轨道半径</param>
    public void InitAxe(float rotateFactor, float radius)
    {
        // 确保初始角度在0-360度范围内
        if (InitAngle > 360f) InitAngle %= 360f;
        
        // 设置斧子的初始旋转角度
        transform.Rotate(Vector3.forward, InitAngle);
        
        // 设置旋转速度因子
        _rotateFactor = rotateFactor;
        
        // 设置斧子精灵的相对位置（基于半径）
        spriteRenderer.transform.position = new Vector3(
            spriteRenderer.transform.position.x + radius,
            spriteRenderer.transform.position.y,
            spriteRenderer.transform.position.z
        );
    }
    
    /// <summary>
    /// 更新旋转速度因子
    /// 响应游戏管理器的攻击速度变化事件
    /// </summary>
    private void UpdateRotateFactor()
    {
        _rotateFactor = GameManager.instance.PlayerAttackSpeed / 100f;
    }

    /// <summary>
    /// 更新伤害百分比
    /// 响应游戏管理器的伤害变化事件
    /// </summary>
    private void UpdateDamagePercentage()
    {
        percentage = GameManager.instance.PlayerDamage;
    }
    

    /// <summary>
    /// 固定更新 - 处理斧子的自身旋转
    /// 注意：这里的旋转是斧子自身的旋转，不是围绕玩家的轨道运动
    /// 轨道运动由AxeManager统一管理
    /// </summary>
    private void FixedUpdate()
    {
        // 让斧子绕自身Z轴旋转，产生旋转攻击的视觉效果
        transform.Rotate(Vector3.forward, rotateSpeed * _rotateFactor);
    }
/*
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

    }*/
    /*
    private void OnDisable()
    {
        //StopCoroutine(Spin());
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
    */
    /// <summary>
    /// 触发器碰撞检测 - 处理斧子与敌人的碰撞
    /// 当斧子碰撞到敌人时，播放音效并造成伤害
    /// </summary>
    /// <param name="collision">碰撞的其他碰撞器</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为敌人
        if(collision.CompareTag("Enemy"))
        { 
            // 尝试获取敌人组件并造成伤害
            if(collision.TryGetComponent(out Enemy enemy))
            {
                // 播放斧头命中音效
                _audioSource.Play();
                
                // 根据当前伤害百分比对敌人造成伤害
                enemy.TakeDamageByPercentage(percentage);
            }
        }
    }

    /// <summary>
    /// 销毁时清理资源
    /// 取消订阅游戏管理器事件，防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnPlayerAttackSpeedChanged -= UpdateRotateFactor;
            GameManager.instance.OnPlayerDamageChanged -= UpdateDamagePercentage;
        }
    }
}
