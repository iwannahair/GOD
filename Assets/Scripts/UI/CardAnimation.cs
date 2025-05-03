using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleMultiplier = 2f;
    public float duration  = 0.2f;
    public float moveUpAmount = 600f;
    
    private RectTransform rectTransform;
    private Vector3 originalScale; 
    private Vector2 originalPosition;
    private Coroutine animationCoroutine; 
    public Vector2 cardPosition { get=>originalPosition; set=>originalPosition = value; }
    private int originalSiblingIndex;
    private bool firstIndexChange = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale; 
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        Vector3 targetScale = originalScale * scaleMultiplier;
        Vector2 targetPosition = originalPosition + new Vector2(0, moveUpAmount);
        if (firstIndexChange)
        {
            originalSiblingIndex = transform.GetSiblingIndex();
            firstIndexChange = false;
        }
        transform.SetAsLastSibling();
        StartAnimation(targetScale, targetPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.SetSiblingIndex(originalSiblingIndex);
        StartAnimation(originalScale, originalPosition);
    }
    
    public void ScaleDown(float dragScale,Vector2 targetPos)
    {
         StartAnimation(originalScale*dragScale, targetPos);
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }
    void StartAnimation(Vector3 targetScale, Vector2 targetPosition)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateTo(targetScale, targetPosition));
    }

    IEnumerator AnimateTo(Vector3 targetScale, Vector2 targetPos)
    {
        Vector3 startScale = rectTransform.localScale;
        Vector2 startPos = rectTransform.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += 0.02f;
            float t = time / duration;

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return new WaitForSecondsRealtime(0.02f);
        }

        rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPos;
        animationCoroutine = null;
    }
}
