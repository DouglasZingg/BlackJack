using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public List<Card> originalHand = new List<Card>();
    //public List<Card> splitHand = new List<Card>();
    public bool[] availablePlayerCardSlots;
    public Transform[] playerCardSlots;
    public GameManager gameManager;
    public Dealer dealer;
    public TextMeshProUGUI deckValue;
    //public int value;
    public bool endTurn = false;
    public bool split = false;
    //public bool instantLost = false;


    public Hand firstHand = new Hand();
    public Hand splitHand = new Hand();

    // Start is called before the first frame update
    void Start()
    {
        firstHand.cards = new List<Card>();
        splitHand.cards = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        firstHand.CalculateValue();
        deckValue.text = firstHand.score.ToString();
    }

    public void Hit()
    {
        if (gameManager.deck.Count >= 1)
        {
            Card randCard = gameManager.deck[UnityEngine.Random.Range(0, gameManager.deck.Count)];

            for (int i = 0; i < availablePlayerCardSlots.Length; i++)
            {
                if (availablePlayerCardSlots[i] == true)
                {
                    randCard.handIndex = i;
                    randCard.transform.position = playerCardSlots[i].position;
                    randCard.hasBeenPlayed = false;
                    availablePlayerCardSlots[i] = false;
                    firstHand.cards.Add(randCard);
                    gameManager.deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void Stand()
    {
        if (splitHand.cards.Count > 0)
        {
            split = true;
        }
        else
        {
            dealer.canNowPlay = true;
        }

        endTurn = true;
    }
}
