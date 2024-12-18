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

    public int dealersValue = 0;

    public TextMeshProUGUI deckValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (canNowPlay == true)
        {
            DealerTurn();
        }
        deckValue.text = dealersValue.ToString();
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
                    gameManager.deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void DealerTurn()
    {
        CalculateValue();


        hiddenCard.gameObject.SetActive(false);

        if (dealersValue < 17)
        {
            Hit();
        }
        else
        {
            dealersValue = 0;
            CalculateValue();
            canNowPlay = false;
            turndone = true;
        }

    }

    public void CalculateValue()
    {
        //for (int i = 0; i < dealersHand.Count; i++)
        //{
        //    if (dealersHand[i].isAce == true)
        //    {
        //        if (dealersValue + 11 > 21)
        //        {
        //            dealersValue += dealersHand[i].cardValue;
        //        }
        //        else
        //        {
        //            dealersValue += dealersHand[i].cardValue;
        //            dealersValue += 10;
        //        }
        //    }
        //    else
        //    {
        //        dealersValue += dealersHand[i].cardValue;
        //    }
        //}

        int aceCount = 0;
        for (int i = 0; i < dealersHand.Count; i++)
        {
            //if(playersHand[i].isAce == true)
            //{
            //    if(value + 11 > 21)
            //    {
            //        value += playersHand[i].cardValue;
            //    }
            //    else
            //    {
            //        value += playersHand[i].cardValue;
            //        value += 10;
            //    }
            //}
            //else
            //{
            //    value += playersHand[i].cardValue;
            //}

            if (dealersHand[i].isAce == true)
            {
                aceCount++;
            }

            dealersValue += dealersHand[i].cardValue;
        }

        if (dealersValue > 21 && aceCount > 0)
        {
            aceCount--;
            dealersValue -= 10;
        }
    }
}
