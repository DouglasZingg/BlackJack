using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Card> playersHand = new List<Card>();
    public bool[] availablePlayerCardSlots;
    public Transform[] playerCardSlots;
    public GameManager gameManager;
    public Dealer dealer;
    public TextMeshProUGUI deckValue;
    public int value;
    public bool endTurn = false;
    public bool instantLost = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deckValue.text = value.ToString();
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
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = playerCardSlots[i].position;
                    randCard.hasBeenPlayed = false;
                    availablePlayerCardSlots[i] = false;
                    playersHand.Add(randCard);
                    value += randCard.cardValue;
                    if (value > 21)
                    {
                        instantLost = true;
                        Stand();
                        break;
                    }
                    gameManager.deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void Stand()
    {
        dealer.canNowPlay = true;
        endTurn = true;
    }
}
