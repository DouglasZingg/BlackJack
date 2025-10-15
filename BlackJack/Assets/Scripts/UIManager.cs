using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI deckSize;
    public TextMeshProUGUI wonText;
    public TextMeshProUGUI lostText;
    public TextMeshProUGUI tieText;
    public TextMeshProUGUI splitText;
    public TextMeshProUGUI messageText;
    public GameObject betUI;

    [Header("Buttons")]
    public Button hitButton;
    public Button standButton;
    public Button splitButton;
    public Button playButton;
    public Button doubleButton;

    public BettingManager bet;

    // ----------------------------

    public void SetDeckCount(int count)
    {
        if (deckSize != null)
            deckSize.text = count.ToString();
    }

    public void EnablePlayButtons(bool enabled)
    {
        hitButton.interactable = enabled;
        standButton.interactable = enabled;
        doubleButton.interactable = enabled;
    }

    public void EnablePlayButton(bool enabled)
    {
        playButton.interactable = enabled;
    }

    public void EnableSplitButton(bool enabled)
    {
        splitButton.interactable = enabled;
    }

    public IEnumerator ShowWinResult(TextMeshProUGUI text, int bet, float displayTime = 2f)
    {
        text.text = "You win $" + bet + "!";
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    public IEnumerator ShowLoseResult(TextMeshProUGUI text, int bet, float displayTime = 2f)
    {
        text.text = "You lost $" + bet + "!";
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    public IEnumerator ShowTieResult(TextMeshProUGUI text, int bet, float displayTime = 2f)
    {
        text.text = "Push!";
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    public IEnumerator ShowMessage(TextMeshProUGUI text, float displayTime = 2f)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    public IEnumerator ShowWin()
    {
        yield return ShowWinResult(wonText, bet.betAmount);
    }

    public IEnumerator ShowLoss()
    {
        yield return ShowLoseResult(lostText, bet.betAmount);
    }

    public IEnumerator ShowTie()
    {
        yield return ShowTieResult(tieText, bet.betAmount);
    }

    // Reset all result texts
    public void HideAllResults()
    {
        wonText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        tieText.gameObject.SetActive(false);
    }
}
