
using UnityEngine; 
using UnityEngine.UI;

public class NewCardHolder : MonoBehaviour
{
    [SerializeField] private Card cardData; 
    [SerializeField] Image cardWholeImage;
    public Card CardData => cardData;

    public HandLayout handLayout
    {
        private get;
        set;
    }
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

    private void OnDestroy()
    {
        handLayout?.RemoveCardFromHand(GetComponent<RectTransform>());
    }
}