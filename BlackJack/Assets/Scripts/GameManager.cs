using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardDeck = new List<Card>();
    public List<Card> playersHand = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawCard()
    {
        if (deck.Count >= 1)
        {
            Card randCard = deck[UnityEngine.Random.Range(0, deck.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
                    randCard.hasBeenPlayed = false;
                    availableCardSlots[i] = false;
                    playersHand.Add(randCard);
                    deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void Shuffle()
    {
        if (discardDeck.Count >= 1)
        {
            foreach (Card card in discardDeck)
            {
                deck.Add(card);
            }
            discardDeck.Clear();
            playersHand.Clear();
        }
    }

    public void Discard()
    {
        for (int i = 0; i < playersHand.Count; i++)
        {
            if (playersHand[i].hasBeenPlayed == false)
            {
                playersHand[i].hasBeenPlayed = true;
                availableCardSlots[i] = true;
                playersHand[i].gameObject.SetActive(false);
                discardDeck.Add(playersHand[i]);
            }
        }
        playersHand.Clear();
    }
}
