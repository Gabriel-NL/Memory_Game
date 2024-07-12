using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    public RectTransform background,
        play_again,
        menu;

    void Start()
    {
        ResizeUI();
    }

    /*
    Lets see...
    Base width= 250
    base height=150

    screen width=720
    screen height=1280
    */

    private void ResizeUI()
    {
        float width_modifier = 3;
        float height_modifier = 8.6f;
        float width,
            height;
        width = background.rect.width / width_modifier;
        height = background.rect.height / height_modifier;
        play_again.sizeDelta = new Vector2(width, height);
        play_again.localPosition = new Vector2(background.rect.width / 5, 0);
        menu.sizeDelta = new Vector2(width, height);
        play_again.localPosition = new Vector2(-background.rect.width / 5, 0);
    }

    public void AddToJson()
    {
        List<PlayerScore> player_scores_container = new List<PlayerScore>();
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerPersistantData.json");
        if (File.Exists(filePath))
        {
            string loadedJsonData = File.ReadAllText(filePath);
            player_scores_container = JsonUtility.FromJson<List<PlayerScore>>(loadedJsonData);
            int n_variations_pref = PlayerPrefs.GetInt(CustomConstants.n_variations_index_pref);
            int fails_pref = PlayerPrefs.GetInt(CustomConstants.last_time_elapsed_pref, 0);

            int calculated_score = (n_variations_pref * 1000000) / fails_pref;
            player_scores_container.Add(
                new PlayerScore()
                {
                    date = PlayerPrefs.GetString(CustomConstants.last_date_and_time_pref, null),
                    time = Mathf.RoundToInt(
                        PlayerPrefs.GetFloat(CustomConstants.last_fail_attempts_pref, 0)
                    ),
                    fails = fails_pref,
                    score = calculated_score
                }
            );
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
        if (player_scores_container.Count > 0)
        {
            foreach (var playerScore in player_scores_container)
            {
                Debug.Log(
                    $"Date: {playerScore.date}, Fails: {playerScore.fails}, Time: {playerScore.time}, Score: {playerScore.score}"
                );
            }
        }
    }

    public void Go_To_Title_Screen()
    {
        SceneManager.LoadScene("TitleState");
    }

    public void Play_Again()
    {
        SceneManager.LoadScene("GameState");
    }
}

public struct PlayerScore
{
    public string date;
    public int fails,
        score;
    public float time;

    public PlayerScore(string date, int fails, float time, int score)
    {
        this.date = date;
        this.fails = fails;
        this.time = time;
        this.score = score;
    }
}
