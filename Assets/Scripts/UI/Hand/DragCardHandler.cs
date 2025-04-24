using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCardHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]private float dragScale = 0.9f;
    [SerializeField]private float returnDuration = 0.2f;
    [SerializeField]private float yOffset = 100f;
    
    [SerializeField]private LayerMask dropTargetLayer; // World map layer
    private Canvas canvas;
    private bool dragging = false;
    
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector2 originalAnchoredPosition;
    private CardAnimation cardAnimation;
    
    void Awake()
    {
        cardAnimation = GetComponent<CardAnimation>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var pos
        );
        cardAnimation.ScaleDown(dragScale,pos+new Vector2(0, yOffset));
        originalAnchoredPosition = cardAnimation.cardPosition;
        cardAnimation.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var pos
        );
        rectTransform.anchoredPosition = pos+new Vector2(0, yOffset);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        GameManager.instance.CardBuildingIndicator.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        rectTransform.localScale = originalScale;

        // Check if released over world map
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, dropTargetLayer))
        {
            Debug.Log("Card dropped on world map at " + hit.point);
            // You can instantiate a world object or handle placement here
            Destroy(gameObject); // Optional: remove card from hand
        }
        else
        {
            // Return to hand
            StartCoroutine(ReturnToOriginalPosition());
            cardAnimation.enabled = true;
        }
        
    }

    IEnumerator ReturnToOriginalPosition()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float t = 0f;

        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float progress = t / returnDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPosition, progress);
            yield return null;
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}
