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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        deckSize.text = deck.Count.ToString();

        if (player.endTurn == true && dealer.turndone == true)
        {
            if (dealer.instantLost == true)
            {
                wonText.gameObject.SetActive(true);
            }
            else if (player.instantLost == true)
            {
                lostText.gameObject.SetActive(true);
            }
            else
            {
                int playerdifference = 21 - player.value;
                int dealerdifference = 21 - dealer.dealersValue;

                if (playerdifference == 0 && dealerdifference == 0)
                {
                    tieText.gameObject.SetActive(true);
                }

                if (playerdifference == 0 || playerdifference < dealerdifference || player.playersHand.Count == 5)
                {
                    wonText.gameObject.SetActive(true);
                }

                if (dealerdifference == 0 || dealerdifference < playerdifference || dealer.dealersHand.Count == 5)
                {
                    lostText.gameObject.SetActive(true);
                }
            }

        }
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
            player.playersHand.Clear();
            dealer.dealersHand.Clear();
        }
    }

    public void Discard()
    {
        lostText.gameObject.SetActive(false);
        wonText.gameObject.SetActive(false);
        tieText.gameObject.SetActive(false);
        player.endTurn = false;
        dealer.turndone = false;
        player.instantLost = false;
        dealer.instantLost = false;

        for (int i = 0; i < player.playersHand.Count; i++)
        {
            if (player.playersHand[i].hasBeenPlayed == false)
            {
                player.playersHand[i].hasBeenPlayed = true;
                player.availablePlayerCardSlots[i] = true;
                player.playersHand[i].transform.position = discardPile.transform.position;
                discardDeck.Add(player.playersHand[i]);
            }
        }

        for (int i = 0; i < dealer.dealersHand.Count; i++)
        {
            if (dealer.dealersHand[i].hasBeenPlayed == false)
            {
                dealer.dealersHand[i].hasBeenPlayed = true;
                dealer.availableDealerCardSlots[i] = true;
                dealer.dealersHand[i].transform.position = discardPile.transform.position;
                discardDeck.Add(dealer.dealersHand[i]);
            }
        }

        player.playersHand.Clear();
        player.value = 0;

        dealer.dealersHand.Clear();
        dealer.dealersValue = 0;

        if (deck.Count < 10)
        {
            ResetGame();
        }
        else
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        dealer.hiddenCard.gameObject.SetActive(true);

        for (int i = 0; i < 2; i++)
        {
            dealer.Hit();
            player.Hit();
        }
    }
}
