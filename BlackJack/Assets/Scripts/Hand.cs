using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<Card> cards;
    public int score;

    public void CalculateValue()
    {
        score = 0;
        int aceCount = 0;

        foreach (var card in cards)
        {
            if (card.isAce)
                aceCount++;

            score += card.cardValue;
        }

        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }
    }
}
