using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour
{
    [Header("Money Values")]
    public int betAmount = 15;
    public int playerBalance = 500;

    [Header("UI Elements")]
    public Button increaseBetButton;
    public Button increaseBetMoreButton;
    public Button decreaseBetButton;
    public Button decreaseBetMoreButton;
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI playerBalanceText;

    void Update()
    {
        // Update UI elements
        betAmountText.text = "$" + betAmount.ToString();
        playerBalanceText.text = "$" + playerBalance.ToString();

        // Decrease bet button should be disabled if betAmount is less than or equal to 15
        if (betAmount - 1 < 15)
            decreaseBetButton.interactable = false;
        else
            decreaseBetButton.interactable = true;
        if (betAmount - 10 < 15)
            decreaseBetMoreButton.interactable = false;
        else
            decreaseBetMoreButton.interactable = true;


        // Increase bet button should be disabled if increasing bet exceeds player balance
        if (betAmount + 1 > playerBalance || betAmount + 1 > 500)
            increaseBetButton.interactable = false;
        else
            increaseBetButton.interactable = true;
        if (betAmount + 10 > playerBalance || betAmount + 1 > 500)
            increaseBetMoreButton.interactable = false;
        else
            increaseBetMoreButton.interactable = true;

    }

    // Methods to adjust bet amount
    public void IncreaseBet()
    {
        betAmount++;
    }

    public void DecreaseBet()
    {
        betAmount--;
    }

    public void IncreaseBetMore()
    {
        betAmount += 10;
    }

    public void DecreaseBetMore()
    {
        betAmount -= 10;
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
