using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Card Info")]
    public int cardValue;
    public int handIndex;
    public bool isAce;
    public bool hasBeenPlayed;

    // Smoothly move the card to a target position over a specified duration
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        // Start position and elapsed time
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        // Lerp the position over time
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the card reaches the exact target position
        transform.position = targetPosition;
    }
}
