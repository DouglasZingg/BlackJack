using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Hands")]
    public Hand firstHand = new();
    public Hand splitHand = new();

    [Header("Display Helpers")]
    public bool[] availablePlayerCardSlots;
    public Transform[] playerCardSlots;

    [Header("References")]
    public GameManager gameManager;
    public Dealer dealer;

    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI deckValue;

    [Header("State Tracking")]
    public bool endTurn;
    public bool split;
    public bool firstHandDone;
    public bool secondHandDone;
    public bool playerAutoWin;

    void Start()
    {
        // Initialize hands
        firstHand.cards = new List<Card>();
        splitHand.cards = new List<Card>();
    }

    void Update()
    {
        // Update the displayed value of the current hand
        firstHand.CalculateValue();
        deckValue.text = firstHand.score.ToString();
    }

    public void HitButton()
    {
        //Disables double button
        gameManager.ui.doubleButton.interactable = false;

        //Delayed hit for better UX
        StartCoroutine(HitWithDelay(0.5f));
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
        availablePlayerCardSlots[slotIndex] = false;
        firstHand.cards.Add(card);

        //Update card properties and animate movement
        card.handIndex = slotIndex;
        card.hasBeenPlayed = false;
        card.gameObject.SetActive(true);
        gameManager.deck.Remove(card);
        yield return StartCoroutine(card.MoveToPosition(playerCardSlots[slotIndex].position, 0.5f));

        //Recalculate hand value and check for bust or 5-cards
        firstHand.CalculateValue();
        if (firstHand.score >= 21 || firstHand.cards.Count == 5)
            Stand();

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
        for (int i = 0; i < availablePlayerCardSlots.Length; i++)
        {
            if (availablePlayerCardSlots[i])
                return i;
        }
        return -1;
    }

    public void Stand()
    {
        //Disable buttons
        gameManager.ui.EnablePlayButton(false);
        gameManager.ui.EnablePlayButtons(false);
        gameManager.ui.EnableSplitButton(false);

        if (split)
        {
            // Split hands end separately
            endTurn = true;
        }
        else if (firstHand.score > 21)
        {
            // Player busts, round ends
            endTurn = true;
            gameManager.EndRound();
        }
        else
        {
            // Normal hand and 21 ends round
            gameManager.roundComplete = true;
            endTurn = true;
            dealer.canNowPlay = true;
            dealer.UpdateDeckValueText();
            StartCoroutine(dealer.DealerTurnSequence());
        }
    }
}
