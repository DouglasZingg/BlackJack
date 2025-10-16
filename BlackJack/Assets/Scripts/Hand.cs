using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    [Header("Hand Info")]
    public List<Card> cards;
    public int score;

    // Calculate the total value of the hand, accounting for Aces
    public void CalculateValue()
    {
        // Reset score and count Aces
        score = 0;
        int aceCount = 0;

        // Sum card values and count Aces
        foreach (var card in cards)
        {
            if (card.isAce)
                aceCount++;

            score += card.cardValue;
        }

        // Adjust for Aces if score exceeds 21
        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }
    }
}
