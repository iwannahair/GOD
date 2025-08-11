using UnityEngine;

/// <summary>
/// 玩家生命值系统 - 处理玩家被敌人碰到的逻辑
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("场景管理器引用")]
    [SerializeField] private SceneManagerScript sceneManager;

    [Header("设置")]
    [Tooltip("敌人标签")]
    [SerializeField] private string enemyTag = "Enemy";

    private void Start()
    {
        // 如果没有指定场景管理器，尝试在场景中查找
        if (sceneManager == null)
        {
            sceneManager = FindObjectOfType<SceneManagerScript>();
            if (sceneManager == null)
            {
                Debug.LogError("PlayerHealth: 未找到SceneManagerScript！请确保场景中有SceneManager对象。");
            }
        }
    }

    /// <summary>
    /// 当玩家与敌人碰撞时触发
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnemyContact(collision.gameObject);
    }

    /// <summary>
    /// 当玩家进入敌人触发器时触发
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyContact(other.gameObject);
    }

    /// <summary>
    /// 处理与敌人接触的逻辑
    /// </summary>
    private void HandleEnemyContact(GameObject otherObject)
    {
        // 检查是否是敌人
        if (otherObject.CompareTag(enemyTag))
        {
            Debug.Log("玩家被敌人击中！重启游戏...");
            
            // 重启游戏
            if (sceneManager != null)
            {
                sceneManager.RestartGame();
            }
            else
            {
                // 如果没有找到场景管理器，直接重新加载当前场景
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}