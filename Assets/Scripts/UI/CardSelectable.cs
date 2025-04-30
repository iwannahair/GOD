using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField,Range(1f, 2f)] private float scaleUp;
    [SerializeField, Range(0.1f, 2f)] private float scaleTime;
    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    private bool IsSelected { get; set; }
    public Card CardData { get; set; }

    private void Start()
    {
        originalScale = transform.localScale;
    }

    // Called when this object is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    private void Select()
    {
        IsSelected = true;
        Debug.Log("select");
        GameManager.instance.GetCardSelectionHandler.SelectCard(CardData, this);
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimationUp());
    }

    public void Deselect()
    {
        Debug.Log("deselect");
        IsSelected = false;
        GameManager.instance.GetCardSelectionHandler.DeselectCurrentCard(this);
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimationDown());
    }

    private IEnumerator AnimationUp()
    {
         
         float timer = 0f;
         Vector3 startScale = transform.localScale;
         while (timer < scaleTime)
         {
             timer += Time.fixedDeltaTime;
             transform.localScale = Vector3.Lerp(startScale, originalScale*scaleUp, timer / scaleTime);
             yield return new WaitForFixedUpdate();
         }
    }
    private IEnumerator AnimationDown()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        while (timer < scaleTime)
        {
            timer += Time.fixedDeltaTime;
            transform.localScale = Vector3.Lerp(startScale, originalScale, timer / scaleTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
