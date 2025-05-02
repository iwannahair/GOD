using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField,Range(1f, 2f)] private float scaleUp;
    [SerializeField, Range(0.1f, 2f)] private float scaleTime;
    [SerializeField] private AudioSource cardSound;
    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    private bool IsSelected { get; set; }
    public Card CardData { get; set; }

    private void Start()
    {
        originalScale = transform.localScale;
        cardSound = cardSound?cardSound : GetComponent<AudioSource>();
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
        cardSound?.Play(); 
        GameManager.instance.GetCardSelectionHandler.SelectCard(CardData, this);
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimationUp());
    }

    public void Deselect()
    {
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
             timer += 0.02f;
             transform.localScale = Vector3.Lerp(startScale, originalScale*scaleUp, timer / scaleTime);
             yield return new WaitForSecondsRealtime(0.02f);
         }
    }
    private IEnumerator AnimationDown()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        while (timer < scaleTime)
        {
            timer += 0.02f;
            transform.localScale = Vector3.Lerp(startScale, originalScale, timer / scaleTime);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
}
