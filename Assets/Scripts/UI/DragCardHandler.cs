using System.Collections;
using CardEnum;
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
    [SerializeField]private AudioSource cardSound;

    void Awake()
    {
        cardSound = cardSound?cardSound : GetComponent<AudioSource>();
        cardAnimation = cardAnimation?cardAnimation: GetComponent<CardAnimation>();
        rectTransform = GetComponent<RectTransform>();
        newCardHolder = newCardHolder? newCardHolder: GetComponent<NewCardHolder>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TypeEnum.AttributeType type = newCardHolder.CardData.attributeType;
        switch (type)
            {
                case TypeEnum.AttributeType.Attack:
                    if (GameManager.instance.PlayerDamage <= 0) return;
                    break;
                case TypeEnum.AttributeType.Health:
                    if (GameManager.instance.PlayerHealth <= 0) return;
                    break;
                case TypeEnum.AttributeType.AttackSpeed:
                    if (GameManager.instance.PlayerAttackSpeed <= 0) return;
                    break;
                default:
                    Debug.LogError("Wrong Type");
                    break;
            }
        dragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var pos
        );
        cardSound?.Play();
        cardAnimation?.ScaleDown(dragScale,pos+new Vector2(0, yOffset));
        originalAnchoredPosition = cardAnimation.cardPosition;
        cardAnimation.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        if (Input.GetMouseButtonDown(1))
        {
            dragging = false;
            StartCoroutine(ReturnToOriginalPosition());
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out var pos
        );
        rectTransform.anchoredPosition = pos+new Vector2(0, yOffset);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Transform cardIndicator = GameManager.instance.CardBuildingIndicator;
        cardIndicator.position = new Vector3(worldPos.x, worldPos.y, 0);
        cardIndicator.localScale = new Vector2(newCardHolder.CardData.width, newCardHolder.CardData.height);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false; 
        GameManager.instance.CardBuildingIndicator.gameObject.SetActive(false);
        // Check if released over world map
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         
        int width = newCardHolder.CardData.width;
        int height = newCardHolder.CardData.height;
        Collider2D hit = Physics2D.OverlapBox(mouseWorldPos, new Vector2(width, height), 0f, hittingLayer); 
        if (CheckAvailable(hit)) return;
        if (hit  != null)
        {
            int hitLayer = hit .gameObject.layer;
            if (hitLayer == LayerMask.NameToLayer("Building"))
            {
                
                StartCoroutine(ReturnToOriginalPosition());
                return;
            }
            if ((dropTargetLayer.value & (1 << hitLayer)) != 0 )
            {
                if (Instantiate(cardPrefab, mouseWorldPos, Quaternion.identity).TryGetComponent(out CardInWorld cardInWorld))
                {
                    cardInWorld.CardData = newCardHolder.CardData;
                }
                cardSound?.Play();
                newCardHolder.RemoveThisCard();
                Destroy(gameObject);  
                return;
            }
            
        }
         
            // Return to hand
        StartCoroutine(ReturnToOriginalPosition());
        
        
    }

    private bool CheckAvailable(Collider2D collider)
    {
        if (collider != null && collider.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            StartCoroutine(ReturnToOriginalPosition());
            return true;
        }

        return false;
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
            t += 0.02f; 
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPosition, t / returnDuration);
            yield return new WaitForSecondsRealtime(0.02f);
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}
