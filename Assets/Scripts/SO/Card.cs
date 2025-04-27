using UnityEngine;
using CardEnum;
[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite cardSprite;
    [TextArea]
    public string cardDescription;
    public GameObject cardBuildingPrefab;
    public TypeEnum.AttributeType attributeType;
    public TypeEnum.CardType cardType;
    public int cardCost;
    public int followersCapacity;
    public int healthPoints;
    public int width,height;
    public Sprite wholeCardSprite;
}
