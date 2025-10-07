using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dealer : MonoBehaviour
{
    public Hand dealerHand = new Hand();
    public bool[] availableDealerCardSlots;
    public Transform[] dealerCardSlots;
    public GameManager gameManager;
    public Image hiddenCard;

    public bool canNowPlay = false;
    public bool endTurn = false;


    public TextMeshProUGUI deckValue;

    // Start is called before the first frame update
    void Start()
    {
        dealerHand.cards = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canNowPlay == true)
        {
            DealerTurn();
        }
        deckValue.text = dealerHand.score.ToString();
    }

    public void Hit()
    {
        if (gameManager.deck.Count >= 1)
        {
            Card randCard = gameManager.deck[UnityEngine.Random.Range(0, gameManager.deck.Count)];

            for (int i = 0; i < availableDealerCardSlots.Length; i++)
            {
                if (availableDealerCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = dealerCardSlots[i].position;
                    randCard.hasBeenPlayed = false;
                    availableDealerCardSlots[i] = false;
                    dealerHand.cards.Add(randCard);
                    gameManager.deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void DealerTurn()
    {
        dealerHand.CalculateValue();
        hiddenCard.gameObject.SetActive(false);

        if (dealerHand.score < 17)
        {
            Hit();
        }
        else
        {
            dealerHand.CalculateValue();
            canNowPlay = false;
            endTurn = true;
        }

    }
}
