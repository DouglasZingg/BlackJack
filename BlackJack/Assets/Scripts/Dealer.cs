using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    [Header("Hands")]
    public Hand dealerHand = new();

    [Header("Display Helpers")]
    public bool[] availableDealerCardSlots;
    public Transform[] dealerCardSlots;

    [Header("References")]
    public GameManager gameManager;
    public Player player;

    [Header("UI Elements")]
    public Card hiddenCard;
    public TextMeshProUGUI deckValue;

    [Header("State Tracking")]
    public bool canNowPlay = false;
    public bool endTurn = false;

    void Start()
    {
        // Initialize dealer's hand
        dealerHand.cards = new List<Card>();
    }
    void Update()
    {
        // Continuously update the displayed value of the dealer's hand
        UpdateDeckValueText();
    }

    public void UpdateDeckValueText()
    {
        // Before cards are dealt
        if (dealerHand.cards.Count == 0)
        {
            deckValue.text = "? + ?";
            return;
        }

        if (canNowPlay)
        {
            // Reveal full dealer hand value
            deckValue.text = dealerHand.score.ToString();
        }
        else if (dealerHand.cards.Count > 1)
        {
            // Show only the value of the second card
            deckValue.text = "? + " + dealerHand.cards[1].cardValue;
        }
        else if(endTurn)
        {
            // Show full dealer hand value at end of turn
            deckValue.text = dealerHand.score.ToString();
        }
        else
        {
            // All other cases (e.g., only one card)
            deckValue.text = "? + ?";
        }
    }

    public IEnumerator HitWithDelay(float delay)
    {
        //Deck size check
        if (gameManager.deck.Count < 1)
            yield break;

        //Draw a random card and find an available slot
        var card = DrawRandomCard();
        int slotIndex = GetAvailableSlotIndex();

        //If no slots are available, exit
        if (slotIndex == -1)
            yield break;

        //Assign card to hand and update states
        availableDealerCardSlots[slotIndex] = false;
        dealerHand.cards.Add(card);

        //Update card properties and animate movement
        card.handIndex = slotIndex;
        card.hasBeenPlayed = false;
        card.gameObject.SetActive(true);
        gameManager.deck.Remove(card);

        // Animate movement
        if (card.handIndex == 0)
        {
            yield return StartCoroutine(hiddenCard.MoveToPosition(dealerCardSlots[slotIndex].position, 0.5f));
            card.transform.position = dealerCardSlots[slotIndex].position;
        }
        else
        {
            yield return StartCoroutine(card.MoveToPosition(dealerCardSlots[slotIndex].position, 0.5f));
        }

        //  Update dealer's value immediately after adding the new card and displaying it
        dealerHand.CalculateValue();
        UpdateDeckValueText();

        //Delay for better UX
        yield return new WaitForSeconds(delay);
    }

    private Card DrawRandomCard()
    {
        //Draws a random card from the deck
        int index = Random.Range(0, gameManager.deck.Count);
        return gameManager.deck[index];
    }

    private int GetAvailableSlotIndex()
    {
        //Finds the first available slot
        for (int i = 0; i < availableDealerCardSlots.Length; i++)
        {
            if (availableDealerCardSlots[i])
                return i;
        }
        return -1;
    }

    public IEnumerator DealerTurnSequence()
    {
        // Disables player controls during dealer's turn
        gameManager.ui.EnablePlayButtons(false);

        // Reveal hidden card
        hiddenCard.transform.position = gameManager.startPile.transform.position;

        // Wait before starting dealer's actions
        yield return new WaitForSeconds(2f);

        // Dealer hits until reaching at least 17 or hitting 5 cards
        dealerHand.CalculateValue();
        int hitCount = 0;
        while (dealerHand.score < 17 && hitCount < 5)
        {
            yield return StartCoroutine(HitWithDelay(1f));

            dealerHand.CalculateValue();

            hitCount++;

            // Keep it live-updating
            UpdateDeckValueText(); 
        }

        // State tracking updates
        endTurn = true;
        canNowPlay = false;
        gameManager.notDoublingDown = true;
    }
}