using System.Collections;
using System.Collections.Generic;
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
    public GameObject gameOverScreen;

    [Header("Split Settings")]
    public Transform splitCardPosition;

    [Header("Managers")]
    public UIManager ui;
    public BettingManager bet;

    [Header("State Tracking")]
    public bool roundComplete = false;
    private bool dealerAutoWin = false;
    private int firstHandScore = 0;
    public bool dealerIsChecking = false;
    public bool notDoublingDown = true;
    public bool notSplit = true;

    void Update()
    {
        // Continuously update deck count in UI
        ui.SetDeckCount(deck.Count);

        // Only trigger auto end for standard (non-split an double) rounds
        if (!player.split && player.endTurn && dealer.endTurn && notDoublingDown && notSplit && roundComplete)
        {
            // Disable buttons to prevent further input
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);

            // Reset flag to avoid multiple triggers
            roundComplete = false;

            EndRound();
        }
    }

    public void PlayGame()
    {
        // Start a new round
        ui.EnablePlayButton(false);

        // Hides bet UI
        ui.betUI.SetActive(false);

        // Deals Cards and checks auto win conditions
        StartCoroutine(DealOpeningHands());
    }

    private IEnumerator DealOpeningHands()
    {
        // Show score chart at start of round
        scoreChart.SetActive(true);

        // Initial deal sequence with delays for better UX
        yield return StartCoroutine(dealer.HitWithDelay(0.5f));
        yield return StartCoroutine(player.HitWithDelay(0.5f));
        yield return StartCoroutine(dealer.HitWithDelay(0.5f));
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Dealer checks for blackjack
        dealerIsChecking = true;

        dealer.dealerHand.CalculateValue();
        if (dealer.dealerHand.cards[1].cardValue == 10 || dealer.dealerHand.cards[1].cardValue == 11)
        {
            // Display's checking message
            yield return StartCoroutine(ui.ShowMessage(ui.messageText));

            if (dealer.dealerHand.score == 21)
            {
                // Dealer has blackjack
                dealerAutoWin = true;

                ui.EnablePlayButton(false);
                ui.EnablePlayButtons(false);
                ui.EnableSplitButton(false);

                // Reveal dealer's hidden card
                dealer.hiddenCard.transform.position = startPile.transform.position;

                // End round immediately
                player.endTurn = true;
                dealer.endTurn = true;

                // Check outcome
                yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score));

                // Restart after a short delay
                StartCoroutine(RestartAfterDelay());

                // Stops further play
                yield break;
            }

        }

        dealerIsChecking = false;

        // Check if player has blackjack
        if (player.firstHand.score == 21)
        {
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);

            //Player has blackjack
            player.playerAutoWin = true;

            // End round immediately
            player.endTurn = true;
            dealer.endTurn = true;

            // Check outcome
            yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score));

            // Restart after a short delay
            StartCoroutine(RestartAfterDelay());

            // Stops further play
            yield break;
        }

        // Enable split button if player has a pair
        if (player.firstHand.cards.Count == 2 && player.firstHand.cards[0].cardValue == player.firstHand.cards[1].cardValue)
            ui.EnableSplitButton(true);

        // Normal play can continue
        dealer.canNowPlay = false;
        dealer.endTurn = false;
        player.endTurn = false;
        ui.EnablePlayButtons(true);

        // Disable double button if player can't afford to double
        if (bet.playerBalance - bet.betAmount < bet.betAmount)
            ui.doubleButton.interactable = false;
        else
            ui.doubleButton.interactable = true;

        ui.HideAllResults();
    }

    public void Split()
    {
        // Player chooses to split their hand
        player.split = true;

        // Cannot Double and Split
        ui.doubleButton.interactable = false;

        // Move second card to split hand
        player.splitHand.cards.Add(player.firstHand.cards[1]);
        player.firstHand.cards.RemoveAt(1);
        player.availablePlayerCardSlots[1] = true;

        // Move split card to designated position
        player.splitHand.cards[0].transform.position = splitCardPosition.position;

        // Disable split button to prevent multiple splits
        ui.EnableSplitButton(false);

        // Start split play sequence
        StartCoroutine(SplitSetupSequence());
    }

    private IEnumerator SplitSetupSequence()
    {
        // Give a new card to the first hand 
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Start playing first hand after a short delay
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayFirstHand());
    }

    private IEnumerator PlayFirstHand()
    {
        // State tracking for first split hand
        player.firstHandDone = false;

        // Wait for player to stand or bust
        yield return new WaitUntil(() => player.endTurn);

        // Mark first hand as done
        player.firstHandDone = true;

        // Store first hand score for later comparison
        firstHandScore = player.firstHand.score;

        // Move to second hand
        yield return new WaitForSeconds(0.5f);
        ui.EnablePlayButtons(true);
        ui.doubleButton.interactable = false;
        StartCoroutine(PlaySecondHand());
    }

    private IEnumerator PlaySecondHand()
    {
        // Resets available slots for second hand
        for (int i = 1; i < player.availablePlayerCardSlots.Length; i++)
            player.availablePlayerCardSlots[i] = true;

        // State tracking for second split hand
        player.endTurn = false;
        player.secondHandDone = false;

        // Hide first-hand cards
        foreach (Card c in player.firstHand.cards)
            c.gameObject.SetActive(false);

        // Move and transfer split card
        Card splitCard = player.splitHand.cards[0];
        yield return StartCoroutine(splitCard.MoveToPosition(player.playerCardSlots[0].position, 0.5f));
        player.firstHand.cards.Clear();
        player.firstHand.cards.Add(splitCard);
        player.splitHand.cards.Clear();

        // Give a new card to the second hand
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Wait for player to stand or bust on second hand
        yield return new WaitUntil(() => player.endTurn);

        // Mark second hand as done
        player.secondHandDone = true;

        // Both hands are done, proceed to dealer's turn
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealerAndCompare());
    }

    private IEnumerator DealerAndCompare()
    {
        // State tracking for dealer's turn
        dealer.endTurn = false;
        dealer.canNowPlay = true;

        // Start dealer's turn sequence
        yield return StartCoroutine(dealer.DealerTurnSequence());

        // Get scores for comparison
        int dealerScore = dealer.dealerHand.score;
        int secondScore = player.firstHand.score;

        // Compare results for both hands
        yield return StartCoroutine(CheckHandOutcome(firstHandScore, dealerScore));
        yield return StartCoroutine(CheckHandOutcome(secondScore, dealerScore));

        // Reset round
        player.split = false;

        // Restart after a short delay
        yield return new WaitForSeconds(1f);
        Discard();
    }

    public void Double()
    {
        // State tracking for doubling down
        notDoublingDown = false;

        // Can only double once
        ui.doubleButton.interactable = false;

        // Start double down sequence
        StartCoroutine(DoubleDownSequence());
    }

    private IEnumerator DoubleDownSequence()
    {
        // Double the bet amount
        bet.betAmount *= 2;

        // Give one final card to player
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Automatically stand after the hits
        player.Stand();
    }

    public void EndRound()
    {
        // Disable buttons to prevent further input
        ui.EnablePlayButton(false);
        ui.EnablePlayButtons(false);
        ui.EnableSplitButton(false);

        // Store scores for comparison
        int dealerScore = dealer.dealerHand.score;
        int playerScore = player.firstHand.score;

        // Start outcome check sequence
        StartCoroutine(CheckHandOutcome(playerScore, dealerScore));

        // Reset turn flags
        player.endTurn = false;
        dealer.endTurn = false;

        // Restart after a short delay
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        // Wait before discarding and resetting for next round
        yield return new WaitForSeconds(2f);
        Discard();
    }

    private IEnumerator CheckHandOutcome(int playerScore, int dealerScore)
    {
        scoreChart.SetActive(false);

        // Player Busts
        if (playerScore > 21)
        {
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        // Player and Dealer both have Auto Win conditions
        else if (dealerAutoWin && player.playerAutoWin)
        {
            dealerAutoWin = false;
            player.playerAutoWin = false;
            yield return StartCoroutine(ShowResult(ui.ShowTie()));
        }
        // Dealer has Auto Win condition
        else if (dealerAutoWin)
        {
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
            dealerAutoWin = false;
        }
        // Dealer Busts
        else if (dealerScore > 21)
        {
            bet.AddWinnings(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        // Player has a better score then Dealer
        else if (playerScore > dealerScore)
        {
            bet.AddWinnings(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        // Dealer has a better score then Player
        else if (dealerScore > playerScore)
        {
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        // Player and Dealer have the same score
        else
        {
            yield return StartCoroutine(ShowResult(ui.ShowTie()));
        }

        yield return new WaitForSeconds(0.5f);
    }

    // Helper to show result with a slight delay
    private IEnumerator ShowResult(IEnumerator resultRoutine)
    {
        yield return StartCoroutine(resultRoutine);
        yield return new WaitForSeconds(0.5f);
    }

    public void Discard()
    {
        // Move all cards from both player and dealer to discard
        MoveCardsToDiscard(player.firstHand.cards, player.availablePlayerCardSlots);
        MoveCardsToDiscard(player.splitHand.cards, player.availablePlayerCardSlots);
        MoveCardsToDiscard(dealer.dealerHand.cards, dealer.availableDealerCardSlots);

        // Reset scores and visuals
        player.firstHand.score = 0;
        player.splitHand.score = 0;
        dealer.dealerHand.score = 0;
        dealer.hiddenCard.transform.position = startPile.transform.position;

        // Refill deck if running low
        if (deck.Count < 10)
            ResetDeck();

        // Reset split/double/round flags
        player.split = false;
        roundComplete = false;
        notDoublingDown = false;
        dealerAutoWin = false;
        player.playerAutoWin = false;

        // Lose condition check
        if (bet.playerBalance < 15)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            // Reset UI
            bet.betAmount = 15;
            ui.EnablePlayButton(true);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);
            ui.betUI.SetActive(true);
        }
    }

    public void ResetDeck()
    {
        // Discard deck check
        if (discardDeck.Count <= 0)
            return;

        // Move all cards from discard back to main deck
        foreach (Card card in discardDeck)
        {
            if (!deck.Contains(card))
            {
                deck.Add(card);
                card.transform.position = startPile.transform.position;
                card.hasBeenPlayed = false;
            }
        }

        // Clear discard deck
        discardDeck.Clear();

        // Safety clear any lingering references
        player.firstHand.cards.Clear();
        player.splitHand.cards.Clear();
        dealer.dealerHand.cards.Clear();

    }

    private void MoveCardsToDiscard(List<Card> cards, bool[] slotFlags)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];

            // Reset basic flags
            card.hasBeenPlayed = true;

            // Free the slot for future deals
            if (slotFlags != null && i < slotFlags.Length)
                slotFlags[i] = true;

            // Move card to discard location
            card.transform.position = discardPile.transform.position;

            // Avoid duplicates
            if (!discardDeck.Contains(card))
                discardDeck.Add(card);
        }

        cards.Clear();
    }

    public void ResetGame()
    {
        // Resets player money and game state for a new game
        bet.ResetMoney();
        ResetDeck();
        Discard();
        gameOverScreen.SetActive(false);
    }
}

