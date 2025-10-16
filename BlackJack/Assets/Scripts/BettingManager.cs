using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour
{
    [Header("aMoney Values")]
    public int betAmount = 15;
    public int playerBalance = 500;

    [Header("UI Elements")]
    public Button increaseBetButton;
    public Button decreaseBetButton;
    public Button placeBetButton;
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI playerBalanceText;

    void Update()
    {
        // Update UI elements
        betAmountText.text = "$" + betAmount.ToString();
        playerBalanceText.text = "$" + playerBalance.ToString();

        // Enable/disable buttons based on current bet and balance
        if (betAmount > playerBalance)
            placeBetButton.interactable = false;
        else
            placeBetButton.interactable = true;

        // Decrease bet button should be disabled if betAmount is less than or equal to 15
        if (betAmount - 15 < 0)
            decreaseBetButton.interactable = false;
        else
            decreaseBetButton.interactable = true;

        // Increase bet button should be disabled if increasing bet exceeds player balance
        if (betAmount + 15 > playerBalance)
            increaseBetButton.interactable = false;
        else
            increaseBetButton.interactable = true;
    }

    // Methods to adjust bet amount
    public void IncreaseBet()
    {
        betAmount += 15;
    }

    public void DecreaseBet()
    {
        betAmount -= 15;
    }

    // Reset player balance and bet amount to default values
    public void ResetMoney()
    {
        playerBalance = 500;
        playerBalanceText.text = "$" + playerBalance.ToString();
        betAmount = 15;
    }

    // Methods to adjust player balance
    public void AddWinnings(int amount)
    {
        playerBalance += amount;
        playerBalanceText.text = "$" + playerBalance.ToString();
    }

    public void SubtractBet(int amount)
    {
        playerBalance -= amount;
        playerBalanceText.text = "$" + playerBalance.ToString();
    }
}
