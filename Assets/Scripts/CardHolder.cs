using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private Card cardData; 
    [SerializeField] Image image;
    [SerializeField] TMP_Text cardNameUI;
    [SerializeField] TMP_Text cardDescriptionUI;

    private void Start()
    {
        if (cardData) Setup(cardData);
    }

    public void Setup(Card cardData)
    {
        this.cardData = cardData;
        cardNameUI.text = cardData.name;
        cardDescriptionUI.text = cardData.cardDescription;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
