using UnityEngine;

public class GoldenTreeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldenTreePrefab; // 黄金树预制体引用
    [SerializeField] private Vector2 spawnOffset = Vector2.zero; // 可选的生成位置偏移量

    private void Start()
    {
        SpawnGoldenTree();
    }

    private void SpawnGoldenTree()
    {
        // 获取屏幕中央的世界坐标
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);
        worldPosition.z = 0; // 确保z坐标为0（2D游戏）
        
        // 应用偏移量
        worldPosition += new Vector3(spawnOffset.x, spawnOffset.y, 0);
        
        // 实例化黄金树预制体
        if (goldenTreePrefab != null)
        {
            Instantiate(goldenTreePrefab, worldPosition, Quaternion.identity);
            Debug.Log("黄金树已生成在屏幕中央");
        }
        else
        {
            Debug.LogError("黄金树预制体未设置，请在Inspector中设置goldenTreePrefab");
        }
    }
}