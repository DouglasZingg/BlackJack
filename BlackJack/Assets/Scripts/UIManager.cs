using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI wonText;
    public TextMeshProUGUI lostText;
    public TextMeshProUGUI tieText;
    public TextMeshProUGUI messageText;
    public GameObject betUI;

    [Header("Buttons")]
    public Button hitButton;
    public Button standButton;
    public Button splitButton;
    public Button nextRoundButton;
    public Button doubleButton;
    public Button exitButton;
    public Button resetButton;
    public Button backButton;

    [Header("References")]
    [SerializeField]
    private BettingManager bet;
    public GameManager gameManager;
    public GameObject howToPlayScreen;
    public GameObject creditsScreen;

    // Set the deck count display
    public void SetDeckCount(int count)
    {
        if (deckSizeText != null)
            deckSizeText.text = count.ToString();
    }

    // Enable or disable the main play buttons
    public void EnablePlayButtons(bool enabled)
    {
        hitButton.interactable = enabled;
        standButton.interactable = enabled;
        doubleButton.interactable = enabled;
    }

    // Enable or disable the play button
    public void EnablePlayButton(bool enabled)
    {
        nextRoundButton.interactable = enabled;
    }

    // Enable or disable the split button
    public void EnableSplitButton(bool enabled)
    {
        splitButton.interactable = enabled;
    }

    // Show result messages for a specified duration
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

    public IEnumerator ShowTieResult(TextMeshProUGUI text, float displayTime = 2f)
    {
        text.text = "Push!";
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    // Show a generic message for a specified duration
    public IEnumerator ShowMessage(TextMeshProUGUI text, float displayTime = 2f)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        text.gameObject.SetActive(false);
    }

    // Show specific results based on game outcome
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
        yield return ShowTieResult(tieText);
    }

    // Reset all result texts
    public void HideAllResults()
    {
        wonText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        tieText.gameObject.SetActive(false);
    }

    // Exit the game application
    public void ExitGame()
    {
        Application.Quit();
    }

    // Reset the current game scene
    public void ResetGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Return to the main play screen from other UI screens
    public void ReturnToPlayScreen()
    {
        howToPlayScreen.SetActive(false);
        creditsScreen.SetActive(false);
        gameManager.playScreen.SetActive(true);
        betUI.SetActive(true);
    }

    // Show the How to Play screen
    public void ShowHowToPlay()
    {
        betUI.SetActive(false);
        gameManager.playScreen.SetActive(false);
        howToPlayScreen.SetActive(true);
    }

    // Show the Credits screen
    public void ShowCredits()
    {
        betUI.SetActive(false);
        gameManager.playScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }
}
