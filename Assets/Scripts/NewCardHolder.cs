using CardEnum;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;

public class NewCardHolder : MonoBehaviour
{
    [SerializeField] private Card cardData; 
    [SerializeField] Image cardWholeImage; 
    private void Start()
    {
        cardWholeImage ??= GetComponent<Image>();
        if (cardData) Setup(cardData);
    }

    public void Setup(Card cardData)
    {
        this.cardData = cardData; 
        cardWholeImage.sprite = cardData.wholeCardSprite;
    }

}