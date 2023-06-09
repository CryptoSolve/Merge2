using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action<int> OnScoreChanged;
    public int Score {
        get { return score; } private set
        {
            score = value;
            OnScoreChanged?.Invoke(value);
        }
    }
    [SerializeField] private MergeablesSpawner mergeablesSpawner;
    [SerializeField] private ScorePresenter scorePresenter;
    private int score;



    private void Awake()
    {
        Debug.Log("Height " + Screen.height);

        if (mergeablesSpawner != null)
            mergeablesSpawner.Init(value => Score += value);

        if (scorePresenter != null)
            OnScoreChanged += scorePresenter.UpdateScore;
    }

    public void ToMainMenu()
    {
        LoadScene(Scenes.MainMenu);
    }

    public void ToMeta()
    {
        LoadScene(Scenes.Meta);
    }

    private void LoadScene(Scenes scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}