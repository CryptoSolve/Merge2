using System;
using TMPro;
using UnityEngine;

public class ScorePresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnValidate()
    {
        if (scoreText != null) return;
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScore(int value)
    {
        scoreText.text = value.ToString();
    }
}
