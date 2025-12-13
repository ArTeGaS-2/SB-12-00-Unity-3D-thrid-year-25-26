// ScoreManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] RawImage blackScreen;
    [SerializeField] TextMeshProUGUI scoreText;     // посилання на UI Text (Canvas)
    [SerializeField] int pointsPerGem = 10;

    int score;
    public void LessScale()
    {
        blackScreen.transform.localScale -= new Vector3(
            0.01f,
            0.01f,
            0.01f);
    }
    void Start()
    {
        UpdateUI();
    }

    public void AddGems(int gemCount)
    {
        score += gemCount * pointsPerGem;
        UpdateUI();
        LessScale();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
