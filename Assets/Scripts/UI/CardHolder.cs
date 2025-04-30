using CardEnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private Card cardData; 
    [SerializeField] Image cardMainImage,cardCostTypeImage;
    [SerializeField] TMP_Text cardNameUI,cardDescriptionUI,cardCostUI;
    [SerializeField] Sprite attackSprite,attackSpeedSprite, healthSprite;
    private void Start()
    {
        if (cardData) Setup(cardData);
    }

    public void Setup(Card cardData)
    {
        this.cardData = cardData;
        cardMainImage.sprite = cardData.cardSprite;
        cardNameUI.text = cardData.cardName;
        cardDescriptionUI.text = cardData.cardDescription;
        cardCostUI.text = cardData.cardCost.ToString();
        switch (cardData.attributeType)
        {
            case TypeEnum.AttributeType.Attack:
                cardCostTypeImage.sprite = attackSprite;
                break;
            case TypeEnum.AttributeType.Health:
                cardCostTypeImage.sprite = healthSprite;
                break;
            default:
                cardCostTypeImage.sprite = attackSpeedSprite;
                break;
        }
    }

}
