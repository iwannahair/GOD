using UnityEngine;

public class ScrollingMap : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public float mapHeight = 10f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move downward
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Reset when moved past mapHeight
        if (transform.position.y <= startPosition.y - mapHeight)
        {
            transform.position = startPosition;
        }
    }
}