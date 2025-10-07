using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardDeck = new List<Card>();

    public TextMeshProUGUI deckSize;
    public TextMeshProUGUI wonText;
    public TextMeshProUGUI lostText;
    public TextMeshProUGUI tieText;

    public Player player;

    public Dealer dealer;

    public GameObject startPile;
    public GameObject discardPile;

    public Button hitButton;
    public Button nextRoundButton;
    public Button standButton;
    public Button playButton;
    public Button splitButton;

    public Transform splitCardPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    //private bool hasLost = false;
    //private bool hasWon = false;

    // Update is called once per frame
    void Update()
    {
        deckSize.text = deck.Count.ToString();

        if (player.endTurn == true && dealer.endTurn == true)
        {
            EndRound();
        }
    }

    IEnumerator HandleLoss()
    {
        lostText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // show text for 2 seconds
        lostText.gameObject.SetActive(false);
        Discard();
        //hasLost = false; // allow new rounds to trigger again if needed
    }
    IEnumerator HandleWon()
    {
        wonText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // show text for 2 seconds
        wonText.gameObject.SetActive(false);
        Discard();
        //hasWon = false; // allow new rounds to trigger again if needed
    }
    IEnumerator HandleTie()
    {
        tieText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // show text for 2 seconds
        tieText.gameObject.SetActive(false);
        Discard();
        //hasWon = false; // allow new rounds to trigger again if needed
    }

    public void ResetGame()
    {
        if (discardDeck.Count >= 1)
        {
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

    public void Discard()
    {
        for (int i = 0; i < player.firstHand.cards.Count; i++)
        {
            if (player.firstHand.cards[i].hasBeenPlayed == false)
            {
                player.firstHand.cards[i].hasBeenPlayed = true;
                player.availablePlayerCardSlots[i] = true;
                player.firstHand.cards[i].transform.position = discardPile.transform.position;
                discardDeck.Add(player.firstHand.cards[i]);
            }
        }

        for (int i = 0; i < dealer.dealerHand.cards.Count; i++)
        {
            if (dealer.dealerHand.cards[i].hasBeenPlayed == false)
            {
                dealer.dealerHand.cards[i].hasBeenPlayed = true;
                dealer.availableDealerCardSlots[i] = true;
                dealer.dealerHand.cards[i].transform.position = discardPile.transform.position;
                discardDeck.Add(dealer.dealerHand.cards[i]);
            }
        }

        player.firstHand.cards.Clear();
        player.firstHand.score = 0;
        dealer.dealerHand.cards.Clear();
        dealer.dealerHand.score = 0;

        if (deck.Count < 10)
        {
            ResetGame();
        }

        if (player.split == true)
        {
            player.firstHand.cards.Add(player.splitHand.cards[0]);
            player.splitHand.cards.RemoveAt(0);
            player.availablePlayerCardSlots[0] = false;
            player.firstHand.cards[0].transform.position = player.playerCardSlots[0].position;

            player.split = false;
        }
        else
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        dealer.hiddenCard.gameObject.SetActive(true);

        hitButton.interactable = true;
        standButton.interactable = true;
        nextRoundButton.interactable = false;
        playButton.interactable = false;

        for (int i = 0; i < 2; i++)
        {
            dealer.Hit();
            player.Hit();
        }
        if (player.firstHand.cards[0].cardValue == player.firstHand.cards[1].cardValue)
        {
            splitButton.interactable = true;
        }
    }

    public void Split()
    {
        player.splitHand.cards.Add(player.firstHand.cards[1]);
        player.firstHand.cards.RemoveAt(1);
        player.availablePlayerCardSlots[1] = true;

        player.splitHand.cards[0].transform.position = splitCardPosition.position;

        splitButton.interactable = false;
    }

    public void EndRound()
    {
        int playerScore = player.firstHand.score;
        int dealerScore = dealer.dealerHand.score;

        // Player busts ? Lose
        if (playerScore > 21)
        {
            StartCoroutine(HandleLoss());
        }
        // Dealer busts ? Win
        else if (dealerScore > 21)
        {
            StartCoroutine(HandleWon());
        }
        // Player wins with higher score
        else if (playerScore > dealerScore)
        {
            StartCoroutine(HandleWon());
        }
        // Dealer wins with higher score
        else if (dealerScore > playerScore)
        {
            StartCoroutine(HandleLoss());
        }
        // Tie (same score)
        else
        {
            StartCoroutine(HandleTie());
        }

        player.endTurn = false;
        dealer.endTurn = false;
    }
}
