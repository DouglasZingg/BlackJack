using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardDealerHelper
{
    public static IEnumerator DealCardWithDelay(List<Card> deck, List<Card> hand, bool[] availableSlots, Transform[] slotPositions, MonoBehaviour context, float moveDuration = 0.5f, float delayAfter = 0.5f, Card hiddenCard = null, bool useHiddenForFirstCard = false)
    {
        if (deck == null || deck.Count == 0)
            yield break;

        // Find available slot
        int slotIndex = GetAvailableSlotIndex(availableSlots);
        if (slotIndex == -1)
            yield break;

        // Pick random card
        Card card = deck[Random.Range(0, deck.Count)];
        deck.Remove(card);
        availableSlots[slotIndex] = false;

        card.handIndex = slotIndex;
        card.hasBeenPlayed = false;
        card.gameObject.SetActive(true);
        hand.Add(card);

        // Move animation
        Transform target = slotPositions[slotIndex];
        if (useHiddenForFirstCard && slotIndex == 0 && hiddenCard != null)
            yield return context.StartCoroutine(hiddenCard.MoveToPosition(target.position, moveDuration));
        else
            yield return context.StartCoroutine(card.MoveToPosition(target.position, moveDuration));

        yield return new WaitForSeconds(delayAfter);
    }

    private static int GetAvailableSlotIndex(bool[] availableSlots)
    {
        for (int i = 0; i < availableSlots.Length; i++)
        {
            if (availableSlots[i])
                return i;
        }
        return -1;
    }
}
