using UnityEngine;

public class ProjectileBoundaryCheck : MonoBehaviour
{
    public Transform checkPoint; // 拖入DestroyPoint
    public float margin = 0.1f; // 边界留白

    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(checkPoint.position);
        
        if (viewportPos.x < -margin || viewportPos.x > 1 + margin ||
            viewportPos.y < -margin || viewportPos.y > 1 + margin)
        {
            Destroy(gameObject);
        }
    }
}