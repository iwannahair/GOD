using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandLayout : MonoBehaviour
{
    [SerializeField] private float yLevel = -250f;
    [SerializeField] private float xBegin = 150f;
    [SerializeField] private float xEnd = 1100f;
    [SerializeField] private float cardWidth = 308f;
    [SerializeField,Range(0f,40f)] private float margin = 20f;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] private float cardMoveDuration = 0.2f;
    private float middlePoint;
    private float offset;
    private int maxFlatCardsNum = 5;
    [SerializeField] private List<RectTransform> hands = new List<RectTransform>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        middlePoint = (xBegin + xEnd) / 2;
    }

    public void UpdateHandPos()
    {
        if (hands.Count == 0) return;
        if (hands.Count > maxFlatCardsNum)
        {
            float length = xEnd - xBegin;
            float offset = length/hands.Count;
            float tempPosX =  xBegin;
            for (int i = 0; i < hands.Count; i++)
            {
                StartCoroutine(CardMove(hands[i],new Vector2(tempPosX, yLevel)));
                tempPosX+=offset;
            }
            return;
        }
        LayoutCardsUnderFive();
    }
    
    void OnGUI()
    {
            Rect buttonRect = new Rect(10, 10, 120, 40);
            // Draw the button and check if it's clicked
            if (GUI.Button(buttonRect, "Click Me"))
            {
                Debug.Log("Temp Button Clicked!");
                RectTransform newCardTran = Instantiate(cardPrefab, this.transform).GetComponent<RectTransform>();
                if (newCardTran.TryGetComponent(out NewCardHolder cardHolder))
                { 
                    cardHolder.handLayout = this;
                }
                hands.Add(newCardTran);
                UpdateHandPos();
            }
        
    }

    public void AddCardToHand(Card cardData)
    {
        RectTransform newCardTran = Instantiate(cardPrefab, this.transform).GetComponent<RectTransform>();
        if (newCardTran.TryGetComponent(out NewCardHolder cardHolder))
        {
            cardHolder.Setup(cardData);
            cardHolder.handLayout = this;
        }
        hands.Add(newCardTran);
        UpdateHandPos();
    }

    public void RemoveCardFromHand(RectTransform cardTrans)
    {
        hands.Remove(cardTrans);
        UpdateHandPos();
    }
    private IEnumerator CardMove(RectTransform cardTran, Vector2 targetPos)
    {
        float elapsed = 0;
        if (cardTran.TryGetComponent(out CardAnimation cardAnimation))
        {
            cardAnimation.cardPosition = targetPos;
        }
        while (elapsed < cardMoveDuration)
        {
            if (!cardTran) break;
            elapsed += 0.02f;
            cardTran.anchoredPosition = Vector2.Lerp(cardTran.anchoredPosition, targetPos, elapsed/cardMoveDuration);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
    private void LayoutCardsUnderFive()
    {
        if (hands.Count % 2 == 1)
        {
            if (hands.Count == 1)
            {
                StartCoroutine(CardMove(hands[0],new Vector2(middlePoint, yLevel)));
                return;
            }
            float tempMidPoint = middlePoint;
            offset = cardWidth+margin;
            for (int i = hands.Count/2-1; i >= 0; i--)
            {
                if(!hands[i]) return;
                tempMidPoint -= offset;
                StartCoroutine(CardMove(hands[i],new Vector2(tempMidPoint, yLevel)));
            }

            StartCoroutine(CardMove(hands[hands.Count/2],new Vector2(middlePoint, yLevel))); 
            tempMidPoint = middlePoint;
            for (int i = hands.Count/2+1; i < hands.Count; i++)
            {
                if(!hands[i]) return;
                tempMidPoint += offset;
                StartCoroutine(CardMove(hands[i],new Vector2(tempMidPoint, yLevel)));
            }
            return;
        }

        if (hands.Count % 2 == 0)
        {
            offset = cardWidth / 2f + margin/2f;
            float tempMidPoint = middlePoint;
            bool oneTimeBool = false;
            for (int i = hands.Count/2-1; i >= 0; i--)
            {
                if(!hands[i]) return;
                tempMidPoint -= offset;
                StartCoroutine(CardMove(hands[i],new Vector2(tempMidPoint, yLevel)));
                
                if(oneTimeBool) continue;//skip first one
                oneTimeBool = true;
                offset = cardWidth + margin;
            }
            tempMidPoint = middlePoint;
            offset = cardWidth / 2f + margin/2f;
            oneTimeBool = false;
            for (int i = hands.Count/2; i < hands.Count; i++)
            {
                if(!hands[i]) return;
                tempMidPoint += offset;
                StartCoroutine(CardMove(hands[i],new Vector2(tempMidPoint, yLevel)));
                
                if(oneTimeBool) continue;//skip first one
                oneTimeBool = true;
                offset = cardWidth + margin;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
