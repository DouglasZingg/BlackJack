using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Decks")]
    public List<Card> deck = new();
    public List<Card> discardDeck = new();

    [Header("References")]
    public Player player;
    public Dealer dealer;
    public GameObject startPile;
    public GameObject discardPile;
    public GameObject scoreChart;

    [Header("Split Settings")]
    public Transform splitCardPosition;

    [Header("Managers")]
    public UIManager ui;

    // --- State Tracking ---
    public bool roundComplete = false;
    private bool playingSplitHand = false;
    private bool dealerAutoWin = false;
    private int firstHandScore = 0;

    // -------------------------------------------------------
    void Update()
    {
        ui.SetDeckCount(deck.Count);

        // Only trigger auto end for standard (non-split) rounds
        if (!player.split && player.endTurn && dealer.endTurn && roundComplete)
        {
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);
            roundComplete = false;
            EndRound();
        }
    }

    // -------------------------------------------------------
    public void PlayGame()
    {
        ui.EnablePlayButton(false);
        StartCoroutine(DealOpeningHands());
    }

    private IEnumerator DealOpeningHands()
    {
        scoreChart.SetActive(true);

        yield return StartCoroutine(dealer.HitWithDelay(0.5f));
        yield return StartCoroutine(player.HitWithDelay(0.5f));
        yield return StartCoroutine(dealer.HitWithDelay(0.5f));
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        ui.EnablePlayButton(false);
        dealer.dealerHand.CalculateValue();
        if (dealer.dealerHand.cards[1].cardValue == 10 || dealer.dealerHand.cards[1].cardValue == 11)
        {
            yield return StartCoroutine(ui.ShowMessage(ui.messageText));
            if (dealer.dealerHand.score == 21)
            {
                dealerAutoWin = true;
                ui.EnablePlayButton(false);
                ui.EnablePlayButtons(false);
                ui.EnableSplitButton(false);
                dealer.hiddenCard.transform.position = startPile.transform.position;
                EndRound();
            }
        }

        if (player.firstHand.cards.Count == 2 &&
            player.firstHand.cards[0].cardValue == player.firstHand.cards[1].cardValue)
        {
            ui.EnableSplitButton(true);
        }

        dealer.canNowPlay = false;
        dealer.endTurn = false;
        player.endTurn = false;

        ui.EnablePlayButtons(true);
        ui.HideAllResults();
    }

    // -------------------------------------------------------
    public void Split()
    {
        if (player.firstHand.cards.Count == 2 &&
            player.firstHand.cards[0].cardValue == player.firstHand.cards[1].cardValue)
        {
            // Move one card to split hand
            player.splitHand.cards.Add(player.firstHand.cards[1]);
            player.firstHand.cards.RemoveAt(1);
            player.availablePlayerCardSlots[1] = true;

            // Move split card visually
            player.splitHand.cards[0].transform.position = splitCardPosition.position;

            player.split = true;

            ui.EnableSplitButton(false);

            StartCoroutine(SplitSetupSequence());
        }
    }

    private IEnumerator SplitSetupSequence()
    {
        // Give a new card to the first hand only
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Start playing first hand
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PlayFirstHand());
    }

    private IEnumerator PlayFirstHand()
    {
        Debug.Log(" Playing First Hand");
        playingSplitHand = false;
        player.split = true;
        player.endTurn = false;

        // Wait for player to stand or bust
        yield return new WaitUntil(() => player.endTurn);

        // Save first-hand score
        firstHandScore = player.firstHand.score;
        Debug.Log(" First hand finished with score: " + firstHandScore);

        // Move to second hand
        yield return new WaitForSeconds(0.5f);
        ui.EnablePlayButtons(true); // allow input for next hand
        StartCoroutine(PlaySecondHand());
    }

    private IEnumerator PlaySecondHand()
    {
        for (int i = 1; i < player.availablePlayerCardSlots.Length; i++)
        {
            player.availablePlayerCardSlots[i] = true;
        }

        Debug.Log(" Playing Second (Split) Hand");
        playingSplitHand = true;
        player.endTurn = false;
        dealer.endTurn = false;

        // Hide first-hand cards visually
        foreach (Card c in player.firstHand.cards)
            c.gameObject.SetActive(false);

        // Move split card to player's main position
        Card splitCard = player.splitHand.cards[0];
        yield return StartCoroutine(splitCard.MoveToPosition(player.playerCardSlots[0].position, 0.5f));

        // Transfer split card to first hand
        player.firstHand.cards.Clear();
        player.firstHand.cards.Add(splitCard);
        player.splitHand.cards.Clear();

        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Wait until player finishes second hand
        yield return new WaitUntil(() => player.endTurn);

        Debug.Log(" Second hand finished with score: " + player.firstHand.score);

        // Dealer plays after both hands are done
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealerAndCompare());
    }

    // -------------------------------------------------------
    private IEnumerator DealerAndCompare()
    {
        Debug.Log(" Dealer's Turn");

        dealer.endTurn = false;
        dealer.canNowPlay = true;
        yield return StartCoroutine(dealer.DealerTurnSequence());

        int dealerScore = dealer.dealerHand.score;
        int secondScore = player.firstHand.score;

        // Compare results for both hands
        yield return StartCoroutine(CheckHandOutcome(firstHandScore, dealerScore, "First Hand"));
        yield return StartCoroutine(CheckHandOutcome(secondScore, dealerScore, "Second Hand"));

        // Reset round
        playingSplitHand = false;
        player.split = false;

        yield return new WaitForSeconds(1f);
        Discard();
    }

    // -------------------------------------------------------
    public void EndRound()
    {
        ui.EnablePlayButton(false);
        ui.EnablePlayButtons(false);
        ui.EnableSplitButton(false);

        int dealerScore = dealer.dealerHand.score;
        int playerScore = player.firstHand.score;

        StartCoroutine(CheckHandOutcome(playerScore, dealerScore, "Player Hand"));
        player.endTurn = false;
        dealer.endTurn = false;

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        Discard();
    }

    private IEnumerator CheckHandOutcome(int playerScore, int dealerScore, string handName)
    {
        scoreChart.SetActive(false);

        if (playerScore > 21)
        {
            Debug.Log($"{handName}: Bust! You Lose.");
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        else if (dealerAutoWin)
        {
            dealerAutoWin = false;
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        else if(dealerAutoWin && player.playerAutoWin)
        {
            dealerAutoWin = false;
            player.playerAutoWin = false;
            Debug.Log($"{handName}: It's a Tie!");
            yield return StartCoroutine(ShowResult(ui.ShowTie()));
        }
        else if (dealerScore > 21)
        {
            Debug.Log($"{handName}: Dealer Busts! You Win!");
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        else if (playerScore > dealerScore)
        {
            Debug.Log($"{handName}: You Win!");
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        else if (dealerScore > playerScore)
        {
            Debug.Log($"{handName}: You Lose!");
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        else
        {
            Debug.Log($"{handName}: It's a Tie!");
            yield return StartCoroutine(ShowResult(ui.ShowTie()));
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ShowResult(IEnumerator resultRoutine)
    {
        yield return StartCoroutine(resultRoutine);
        yield return new WaitForSeconds(0.5f);
    }

    // -------------------------------------------------------
    public void Discard()
    {
        MoveCardsToDiscard(player.firstHand.cards, player.availablePlayerCardSlots);
        MoveCardsToDiscard(dealer.dealerHand.cards, dealer.availableDealerCardSlots);

        player.firstHand.score = 0;
        dealer.dealerHand.score = 0;
        dealer.hiddenCard.transform.position = startPile.transform.position;

        if (deck.Count < 10)
            ResetGame();

        player.split = false;
        player.splitHand.cards.Clear();
        player.splitHand.score = 0;
        playingSplitHand = false;

        ui.EnablePlayButton(true);
        ui.EnablePlayButtons(false);
        ui.EnableSplitButton(false);
    }

    private void MoveCardsToDiscard(List<Card> cards, bool[] slotFlags)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (!card.hasBeenPlayed)
            {
                card.hasBeenPlayed = true;
                if (slotFlags != null && i < slotFlags.Length)
                    slotFlags[i] = true;

                card.transform.position = discardPile.transform.position;
                discardDeck.Add(card);
            }
        }
        cards.Clear();
    }

    public void ResetGame()
    {
        if (discardDeck.Count <= 0) return;

        foreach (Card card in discardDeck)
        {
            deck.Add(card);
            card.transform.position = startPile.transform.position;
        }
        discardDeck.Clear();

        player.firstHand.cards.Clear();
        dealer.dealerHand.cards.Clear();
    }
}
