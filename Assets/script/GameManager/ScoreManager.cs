using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    [Header("Scoring")]
    public float pointsPerSecond = 1f;  // 1s = 1 điểm
    public int score;

    int best;

    void Start()
    {
        best = PlayerPrefs.GetInt("Best", 0);
        if (bestText) bestText.text = $"Điểm cao nhất {best:00}";
    }

    void Update()
    {
        score += Mathf.FloorToInt(pointsPerSecond * Time.deltaTime);
        if (scoreText) scoreText.text = $"Điểm {score:00}";
    }

    public void CommitBest()
    {
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("Best", best);
            if (bestText) bestText.text = $"Điểm cao nhất {best:00}";
        }
    }

    public void ResetRun()
    {
        score = 0;
        if (scoreText) scoreText.text = "00";
    }
}
