using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Dealer : MonoBehaviour
{
    public Hand dealerHand = new();
    public bool[] availableDealerCardSlots;
    public Transform[] dealerCardSlots;
    public GameManager gameManager;
    public Card hiddenCard;
    public bool canNowPlay = false;
    public bool endTurn = false;
    public TextMeshProUGUI deckValue;
    void Start()
    {
        dealerHand.cards = new List<Card>();
    }
    void Update()
    {
        UpdateDeckValueText();
    }

    public void UpdateDeckValueText()
    {
        if (dealerHand.cards.Count == 0)
        {
            deckValue.text = "? + ?";
            return;
        }

        if (canNowPlay)
        {
            deckValue.text = dealerHand.score.ToString();
        }
        else if (dealerHand.cards.Count > 1)
        {
            deckValue.text = "? + " + dealerHand.cards[1].cardValue;
        }
        else if(endTurn)
        {
            deckValue.text = dealerHand.score.ToString();
        }
        else
        {
            deckValue.text = "? + ?";
        }
    }

    public IEnumerator HitWithDelay(float delay)
    {
        if (gameManager.deck.Count >= 1)
        {
            Card randCard = gameManager.deck[UnityEngine.Random.Range(0, gameManager.deck.Count)];

            for (int i = 0; i < availableDealerCardSlots.Length; i++)
            {
                if (availableDealerCardSlots[i])
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.hasBeenPlayed = false;

                    availableDealerCardSlots[i] = false;
                    dealerHand.cards.Add(randCard);
                    gameManager.deck.Remove(randCard);

                    // Animate movement
                    if (randCard.handIndex == 0)
                    {
                        yield return StartCoroutine(hiddenCard.MoveToPosition(dealerCardSlots[i].position, 0.5f));
                        randCard.transform.position = dealerCardSlots[i].position;
                    }
                    else
                    {
                        yield return StartCoroutine(randCard.MoveToPosition(dealerCardSlots[i].position, 0.5f));
                    }

                    //  Update dealer's value immediately after adding the new card
                    dealerHand.CalculateValue();
                    UpdateDeckValueText();

                    yield return new WaitForSeconds(delay);
                    yield break;
                }
            }
        }
    }
    public IEnumerator DealerHitOnceSequence()
    {
        while (dealerHand.cards.Count != 2)
        {
            yield return StartCoroutine(HitWithDelay(1f));
            dealerHand.CalculateValue();
        }
        endTurn = true;
    }
    public IEnumerator DealerTurnSequence()
    {
        hiddenCard.transform.position = gameManager.startPile.transform.position;
        yield return new WaitForSeconds(2f);
        dealerHand.CalculateValue();
        while (dealerHand.score < 17 || dealerHand.cards.Count == 5)
        {
            yield return StartCoroutine(HitWithDelay(1f));
            dealerHand.CalculateValue();
            UpdateDeckValueText(); //  keep it live-updating
        }
        endTurn = true;
        canNowPlay = false;
    }
}