using TMPro;
using UnityEngine;

/// <summary>
/// Displays the player's score using a TextMeshProUGUI element.
/// </summary>
public class ScoreDisplay : MonoBehaviour // use MonoBehaviour unless UIelement is confirmed working
{
    [Header("References")]
    [Tooltip("The TMP Text element that displays the score.")]
    [SerializeField] public TMP_Text displayText;

    private void Awake()
    {
        if (displayText == null)
        {
            displayText = GetComponent<TMP_Text>();
        }

        if (displayText == null)
        {
            Debug.LogError("ScoreDisplay: No TMP_Text assigned or found on the GameObject!");
        }
    }

    /// <summary>
    /// Updates the TMP Text element with the current score.
    /// </summary>
    public void DisplayScore()
    {
        if (displayText != null)
        {
            displayText.text = $"Score: {GameManager.score}";
        }
    }

    // Call this from another script, like GameManager, when score changes.
    public void UpdateUI()
    {
        DisplayScore();
    }
}
