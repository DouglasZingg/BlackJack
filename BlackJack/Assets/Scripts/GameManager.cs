using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

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

    // --- State Tracking ---
    public bool roundComplete = false;
    private bool playingSplitHand = false;
    private bool dealerAutoWin = false;
    private int firstHandScore = 0;
    public bool dealerIsChecking = true;
    public bool doublingDown = true;

    // -------------------------------------------------------
    void Update()
    {
        ui.SetDeckCount(deck.Count);

        // Only trigger auto end for standard (non-split) rounds
        if (!player.split && player.endTurn && dealer.endTurn && roundComplete && doublingDown)
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
        ui.betUI.SetActive(false);
        bet.PlaceBet();
        StartCoroutine(DealOpeningHands());
    }

    private IEnumerator DealOpeningHands()
    {
        scoreChart.SetActive(true);
        dealerIsChecking = true;

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
                player.endTurn = true;
                dealer.endTurn = true;
                // Wait for loss text to play before continuing
                yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score, "Player Hand"));

                // Stop the rest of the setup — round is already over
                yield break;
            }

        }
        dealerIsChecking = false;

        if (player.firstHand.score == 21)
        {
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);
            player.playerAutoWin = true;
            player.endTurn = true;
            dealer.endTurn = true;
            yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score, "Player Hand"));
            yield break;
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

        if (bet.playerBalance - bet.betAmount < bet.betAmount)
        {
            ui.doubleButton.interactable = false;
        }
        else
        {
            ui.doubleButton.interactable = true;
        }

        ui.HideAllResults();
    }

    // -------------------------------------------------------
    public void Split()
    {
        if (player.firstHand.cards.Count == 2 &&
            player.firstHand.cards[0].cardValue == player.firstHand.cards[1].cardValue)
        {
            ui.doubleButton.interactable = false;
            // Move one card to split hand
            player.splitHand.cards.Add(player.firstHand.cards[1]);
            player.firstHand.cards.RemoveAt(1);
            player.availablePlayerCardSlots[1] = true;

            // Move split card visually
            player.splitHand.cards[0].transform.position = splitCardPosition.position;

            player.split = true;

            ui.EnableSplitButton(false);

            //bet.betAmount *= 2;

            StartCoroutine(SplitSetupSequence());
        }
    }

    public void Double()
    {
        doublingDown = false;
        ui.doubleButton.interactable = false;
        StartCoroutine(DoubleDownSequence());

    }

    private IEnumerator DoubleDownSequence()
    {
        bet.betAmount *= 2;

        yield return StartCoroutine(player.HitWithDelay(0.5f));

        player.firstHand.CalculateValue();

        if (player.firstHand.score > 21)
        {
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);
            player.endTurn = true;
            dealer.endTurn = true;
            yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score, "Player Hand"));
        }
        else if (player.firstHand.score == 21)
        {
            ui.EnablePlayButton(false);
            ui.EnablePlayButtons(false);
            ui.EnableSplitButton(false);
            player.playerAutoWin = true;
            player.endTurn = true;
            dealer.endTurn = true;
            yield return StartCoroutine(CheckHandOutcome(player.firstHand.score, dealer.dealerHand.score, "Player Hand"));
        }
        else
        {
            player.Stand();
        }
    }

    private IEnumerator SplitSetupSequence()
    {

        // Give a new card to the first hand only
        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Start playing first hand
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayFirstHand());
    }

    private IEnumerator PlayFirstHand()
    {
        playingSplitHand = false;
        player.split = true;
        player.endTurn = false;
        player.firstHandDone = false;

        // Wait for player to stand or bust
        if (player.firstHand.score == 21)
            player.endTurn = true;
        else
            yield return new WaitUntil(() => player.endTurn);

        player.firstHandDone = true;
        firstHandScore = player.firstHand.score;

        // Move to second hand
        yield return new WaitForSeconds(0.5f);
        ui.EnablePlayButtons(true);
        ui.doubleButton.interactable = false;
        StartCoroutine(PlaySecondHand());
    }

    private IEnumerator PlaySecondHand()
    {
        for (int i = 1; i < player.availablePlayerCardSlots.Length; i++)
            player.availablePlayerCardSlots[i] = true;

        playingSplitHand = true;
        player.endTurn = false;
        player.secondHandDone = false;
        dealer.endTurn = false;

        // Hide first-hand cards
        foreach (Card c in player.firstHand.cards)
            c.gameObject.SetActive(false);

        // Move and transfer split card
        Card splitCard = player.splitHand.cards[0];
        yield return StartCoroutine(splitCard.MoveToPosition(player.playerCardSlots[0].position, 0.5f));
        player.firstHand.cards.Clear();
        player.firstHand.cards.Add(splitCard);
        player.splitHand.cards.Clear();

        yield return StartCoroutine(player.HitWithDelay(0.5f));

        // Wait for player to stand or bust on second hand
        yield return new WaitUntil(() => player.endTurn);
        player.secondHandDone = true;

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DealerAndCompare());
    }


    // -------------------------------------------------------
    private IEnumerator DealerAndCompare()
    {

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
        if (dealerAutoWin || player.playerAutoWin)
            return;

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
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        else if (dealerAutoWin && player.playerAutoWin)
        {
            dealerAutoWin = false;
            player.playerAutoWin = false;
            bet.AddWinnings(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowTie()));
        }
        else if (dealerAutoWin)
        {
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
            dealerAutoWin = false;
        }
        else if (dealerScore > 21)
        {
            bet.AddWinnings(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        else if (playerScore > dealerScore)
        {
            bet.AddWinnings(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowWin()));
        }
        else if (dealerScore > playerScore)
        {
            bet.SubtractBet(bet.betAmount);
            yield return StartCoroutine(ShowResult(ui.ShowLoss()));
        }
        else
        {
            bet.AddWinnings(bet.betAmount);
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
    // -------------------------------------------------------
    // Safely discard all cards and reset between rounds
    // -------------------------------------------------------
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
        playingSplitHand = false;
        roundComplete = false;
        doublingDown = true;
        dealerAutoWin = false;
        player.playerAutoWin = false;

        // Reset UI
        bet.betAmount = 15;
        ui.EnablePlayButton(bet.playerBalance >= 15);
        ui.EnablePlayButtons(false);
        ui.EnableSplitButton(false);
        ui.betUI.SetActive(bet.playerBalance >= 15);

        // Game over check
        if (bet.playerBalance < 15)
            gameOverScreen.SetActive(true);
    }

    // -------------------------------------------------------
    // Safely rebuild deck from discarded cards
    // -------------------------------------------------------
    public void ResetDeck()
    {
        if (discardDeck.Count <= 0) return;

        foreach (Card card in discardDeck)
        {
            if (!deck.Contains(card))
            {
                deck.Add(card);
                card.transform.position = startPile.transform.position;
                card.hasBeenPlayed = false;
            }
        }

        discardDeck.Clear();

        // Safety clear any lingering references
        player.firstHand.cards.Clear();
        player.splitHand.cards.Clear();
        dealer.dealerHand.cards.Clear();

    }

    // -------------------------------------------------------
    // Safely moves every card to discard pile
    // -------------------------------------------------------
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
        bet.ResetMoney();
        ResetDeck();
        Discard();
        gameOverScreen.SetActive(false);
    }
}

