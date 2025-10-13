using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BettingManager : MonoBehaviour
{
    public int betAmount = 15;
    public int playerBalance = 500;
    public Button increaseBetButton;
    public Button decreaseBetButton;
    public Button placeBetButton;
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI playerBalanceText;



    // Update is called once per frame
    void Update()
    {
        betAmountText.text = "$" + betAmount.ToString();

        if (betAmount > playerBalance)
        {
            placeBetButton.interactable = false;
        }
        else
        {
            placeBetButton.interactable = true;
        }

        if (betAmount - 15 < 0)
        {
            decreaseBetButton.interactable = false;
        }
        else
        {
            decreaseBetButton.interactable = true;
        }

        if (betAmount + 15 > playerBalance)
        {
            increaseBetButton.interactable = false;
        }
        else
        {
            increaseBetButton.interactable = true;
        }
    }

    public void IncreaseBet()
    {
        betAmount += 15;
    }

    public void DecreaseBet()
    {
        betAmount -= 15;
    }

    public void PlaceBet()
    {
        playerBalance -= betAmount;
        playerBalanceText.text = "$" + playerBalance.ToString();
        betAmountText.text = "$15";
    }

    public void ResetMoney()
    {
        playerBalance = 500;
        playerBalanceText.text = "$" + playerBalance.ToString();
        betAmount = 15;
    }

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
