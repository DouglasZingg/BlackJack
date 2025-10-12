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

    [Header("Buttons")]
    public Button hitButton;
    public Button standButton;
    public Button splitButton;
    public Button playButton;

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
    }

    public void EnablePlayButton(bool enabled)
    {
        playButton.interactable = enabled;
    }

    public void EnableSplitButton(bool enabled)
    {
        splitButton.interactable = enabled;
    }

    public IEnumerator ShowResult(TextMeshProUGUI text, float displayTime = 2f)
    {
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
        yield return ShowResult(wonText);
    }

    public IEnumerator ShowLoss()
    {
        yield return ShowResult(lostText);
    }

    public IEnumerator ShowTie()
    {
        yield return ShowResult(tieText);
    }
    public IEnumerator ShowDealerAutpWIn()
    {
        yield return ShowResult(messageText);
    }

    public IEnumerator ShowSplit()
    {
        yield return ShowResult(splitText);
    }

    // Reset all result texts
    public void HideAllResults()
    {
        wonText.gameObject.SetActive(false);
        lostText.gameObject.SetActive(false);
        tieText.gameObject.SetActive(false);
    }
}
