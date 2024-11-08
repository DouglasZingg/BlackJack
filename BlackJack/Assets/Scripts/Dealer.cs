using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dealer : MonoBehaviour
{
    public List<Card> dealersHand = new List<Card>();
    public bool[] availableDealerCardSlots;
    public Transform[] dealerCardSlots;
    public GameManager gameManager;
    public Image hiddenCard;

    public bool canNowPlay = false;
    public bool turndone = false;
    public bool instantLost = false;

    public int dealersValue = 0;

    public TextMeshProUGUI deckValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        deckValue.text = dealersValue.ToString();
        DealerTurn();
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
                    dealersHand.Add(randCard);
                    dealersValue += randCard.cardValue;
                    gameManager.deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void DealerTurn()
    {
        if (canNowPlay == true)
        {
            hiddenCard.gameObject.SetActive(false);
            
            if (dealersValue < 15)
            {
                Hit();
                if(dealersValue > 21)
                {
                    instantLost = true;
                }
            }
            else
            {
                canNowPlay = false;
                turndone = true;
            }
        }
    }
}
