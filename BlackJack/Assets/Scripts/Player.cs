using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool[] availablePlayerCardSlots;
    public Transform[] playerCardSlots;
    public GameManager gameManager;
    public Dealer dealer;
    public TextMeshProUGUI deckValue;

    public bool endTurn;
    public bool split;
    public bool playerAutoWin;
    public bool firstHandDone;
    public bool secondHandDone;

    public Hand firstHand = new();
    public Hand splitHand = new();

    void Start()
    {
        firstHand.cards = new List<Card>();
        splitHand.cards = new List<Card>();
    }

    void Update()
    {
        firstHand.CalculateValue();
        deckValue.text = firstHand.score.ToString();
    }

    // -------------------------------------
    public void HitButton()
    {
        gameManager.ui.doubleButton.interactable = false;
        StartCoroutine(HitWithDelay(0.5f));
    }

    public IEnumerator HitWithDelay(float delay)
    {
        if (gameManager.deck.Count < 1)
            yield break;

        var card = DrawRandomCard();
        int slotIndex = GetAvailableSlotIndex();

        if (slotIndex == -1)
            yield break;

        availablePlayerCardSlots[slotIndex] = false;
        firstHand.cards.Add(card);

        card.handIndex = slotIndex;
        card.hasBeenPlayed = false;
        card.gameObject.SetActive(true);
        gameManager.deck.Remove(card);

        yield return StartCoroutine(card.MoveToPosition(playerCardSlots[slotIndex].position, 0.5f));

        firstHand.CalculateValue();

        // -------------------------------------
        //  MAIN SCORE CHECKS
        // -------------------------------------

        // Player bust (no split)
        if (firstHand.score > 21 && split == false)
        {
            gameManager.ui.EnablePlayButton(false);
            gameManager.ui.EnablePlayButtons(false);
            gameManager.ui.EnableSplitButton(false);
            endTurn = true;

            gameManager.EndRound();
        }
        // Player hits 21 (auto win, no split)
        else if (firstHand.score == 21 && split == false)
        {
            playerAutoWin = true;

            gameManager.ui.EnablePlayButton(false);
            gameManager.ui.EnablePlayButtons(false);
            gameManager.ui.EnableSplitButton(false);
            endTurn = true;

            gameManager.EndRound();
        }
        // Split hand bust
        else if (firstHand.score > 21 && split == true)
        {
            gameManager.ui.EnablePlayButton(false);
            gameManager.ui.EnablePlayButtons(false);
            gameManager.ui.EnableSplitButton(false);
            endTurn = true;
        }
        // Split hand hits 21
        else if (firstHand.score == 21 && split == true)
        {
            gameManager.ui.EnablePlayButton(false);
            gameManager.ui.EnablePlayButtons(false);
            gameManager.ui.EnableSplitButton(false);
            endTurn = true;
        }
        // 5-card Charlie auto-stand
        else if (firstHand.cards.Count == 5)
        {
            Stand();
        }

        yield return new WaitForSeconds(delay);
    }

    private Card DrawRandomCard()
    {
        int index = Random.Range(0, gameManager.deck.Count);
        return gameManager.deck[index];
    }

    private int GetAvailableSlotIndex()
    {
        for (int i = 0; i < availablePlayerCardSlots.Length; i++)
        {
            if (availablePlayerCardSlots[i])
                return i;
        }
        return -1;
    }

    // -------------------------------------
    public void Stand()
    {
        gameManager.ui.EnablePlayButton(false);
        gameManager.ui.EnablePlayButtons(false);
        gameManager.ui.EnableSplitButton(false);

        if (split)
        {
            // Split hands end separately
            endTurn = true;
        }
        else
        {
            // Normal hand ends round
            gameManager.roundComplete = true;
            endTurn = true;
            dealer.canNowPlay = true;
            dealer.UpdateDeckValueText();
            StartCoroutine(dealer.DealerTurnSequence());
        }
    }
}
