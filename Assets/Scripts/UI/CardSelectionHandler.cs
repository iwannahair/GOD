using System.Collections.Generic; 
using UnityEngine;
using Random = UnityEngine.Random;

public class CardSelectionHandler : MonoBehaviour
{
    [SerializeField] private Card[] cardData;
    [SerializeField] private Card selectedCard;
    private CardSelectable selectedCardSelectable;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<GameObject> spawnedCards;
    [SerializeField] private Transform[] presetPositions;
    private void OnEnable()
    {
        SpawnRandomCards();
        Time.timeScale = 0;
    }

    private void SpawnRandomCards()
    {
        foreach (var tran in presetPositions)
        {
            GameObject spawnedCard = Instantiate(cardPrefab, tran);
            spawnedCards.Add(spawnedCard.gameObject);
            Card card = cardData[Random.Range(0, cardData.Length)];
            spawnedCard.GetComponent<NewCardHolder>().Setup(card);
            spawnedCard.GetComponent<CardSelectable>().CardData = card;
        }
    }

    private void OnDisable()
    {
        CleanUp();
    }

    public void AddCardToHand()
    {
        if (!selectedCard) return;
        GameManager.instance.AddToHand(selectedCard);
        this.gameObject.SetActive(false);
    }
    private void CleanUp()
    {
        Time.timeScale = 1;
        DeselectCard();
        foreach (var card in spawnedCards)
        {
            Destroy(card);
        }
        spawnedCards.Clear();
    }
    public void SelectCard(Card card,CardSelectable selectedCardSelectable)
    {
        if (this.selectedCardSelectable)
        {
            this.selectedCardSelectable.Deselect();
        }
        this.selectedCardSelectable = selectedCardSelectable;
        selectedCard = card;
    }

    public void DeselectCard()
    {
        selectedCardSelectable = null;
        selectedCard = null;
    }

    public void DeselectCurrentCard(CardSelectable selectedCardSelectable)
    {
        if (this.selectedCardSelectable == selectedCardSelectable)
        {
            this.selectedCardSelectable = null;
            selectedCard = null;
        }
            
    }
}
