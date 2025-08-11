using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score = 0;       // Chỉ lưu số nguyên
    private float timer = 0f;
    private int pointRate = 1;  // Điểm/giây ban đầu

    void Update()
    {
        // Cập nhật tốc độ cộng điểm theo thời gian chơi
        float t = Time.timeSinceLevelLoad;

        if (t >= 30f) pointRate = 5;
        else if (t >= 20f) pointRate = 3;
        else if (t >= 10f) pointRate = 2;
        else pointRate = 1;

        // Đếm thời gian để cộng điểm nguyên
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            score += pointRate;
            timer -= 1f; // reset lại 1 giây
        }

        // Hiển thị điểm
        if (scoreText != null)
        {
            scoreText.text = "Điểm: " + score;
        }
    }
}
