using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float boundaryPadding = 0.5f;
    
    private Vector2 moveDirection;
    private Camera cam;
    private float minX, maxX, minY, maxY;

    void Start()
    {
        cam = Camera.main;
        CalculateScreenBounds();
    }

    void Update()
    {
        // 获取输入
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        
        // 移动角色
        Vector3 newPos = transform.position + (Vector3)(moveDirection * speed * Time.deltaTime);
        
        // 限制在屏幕内
        newPos.x = Mathf.Clamp(newPos.x, minX + boundaryPadding, maxX - boundaryPadding);
        newPos.y = Mathf.Clamp(newPos.y, minY + boundaryPadding, maxY - boundaryPadding);
        
        transform.position = newPos;
    }

    void CalculateScreenBounds()
    {
        if (cam == null) return;
        
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        
        Vector3 camPos = cam.transform.position;
        
        minX = camPos.x - camWidth;
        maxX = camPos.x + camWidth;
        minY = camPos.y - camHeight;
        maxY = camPos.y + camHeight;
    }
}