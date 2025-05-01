using System.Collections; 
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCardHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]private float dragScale = 0.9f;
    [SerializeField]private float returnDuration = 0.2f;
    [SerializeField]private float yOffset = 100f;
    
    [SerializeField]private LayerMask dropTargetLayer;// World map layer
    [SerializeField] private LayerMask hittingLayer;
    private Canvas canvas;
    private bool dragging;
    
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector2 originalAnchoredPosition;
    [SerializeField]private CardAnimation cardAnimation;
    [SerializeField]private NewCardHolder newCardHolder;
    [SerializeField]private GameObject cardPrefab;
    void Awake()
    {
        cardAnimation = cardAnimation?cardAnimation: GetComponent<CardAnimation>();
        rectTransform = GetComponent<RectTransform>();
        newCardHolder = newCardHolder? newCardHolder: GetComponent<NewCardHolder>();
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
        GameManager.instance.CardBuildingIndicator.gameObject.SetActive(false);
        // Check if released over world map
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, hittingLayer);
        if (hit.collider != null)
        {
            int hitLayer = hit.collider.gameObject.layer;
            if (hitLayer == LayerMask.NameToLayer("Building"))
            {
                
                StartCoroutine(ReturnToOriginalPosition());
                return;
            }
            if ((dropTargetLayer.value & (1 << hitLayer)) != 0 )
            {
                if (Instantiate(cardPrefab, hit.point, Quaternion.identity).TryGetComponent(out CardInWorld cardInWorld))
                {
                    cardInWorld.CardData = newCardHolder.CardData;
                }
                Destroy(gameObject);  
                return;
            }
            
        }
         
            // Return to hand
        StartCoroutine(ReturnToOriginalPosition());
        
        
    }

    IEnumerator ReturnToOriginalPosition()
    {
        cardAnimation.StopAnimation();
        cardAnimation.enabled = true;
        rectTransform.localScale = originalScale;
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
