using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class PlayerScore
{
    public string date;
    public int score;
    public float time;

    public PlayerScore(string date, float time, int score)
    {
        this.date = date;
        this.time = time;
        this.score = score;
    }
}

[Serializable]
public class PlayerScoreWrapper
{
    public PlayerScore[] score_Database;

    public PlayerScoreWrapper(PlayerScore[] scores)
    {
        this.score_Database = scores;
    }
}

public class Victory : MonoBehaviour
{
    private float margin = 10;
    public RectTransform background;
    public TextMeshProUGUI show_best_scores,
        title_label;

    [SerializeField]
    private Transform navbar_box;

    void Start()
    {
        ResizeUI();
        string filePath = Path.Combine(Application.dataPath, "PlayerScores.json");

        CheckAndStorePlayerScores(filePath);
    }

    private void ResizeUI()
    {
        float height_fragments = background.rect.height / 16;
        float base_height = height_fragments * 1.5f;

        RectTransform title_rect = title_label.gameObject.GetComponent<RectTransform>();
        title_rect.sizeDelta = new Vector2(background.rect.width, base_height);
        title_rect.localPosition = new Vector2(0, (background.rect.height / 2) - base_height / 2);

        RectTransform navbar_box_rect = navbar_box.gameObject.GetComponent<RectTransform>();
        navbar_box_rect.sizeDelta = new Vector2(background.rect.width, base_height);
        navbar_box_rect.localPosition = new Vector2(
            0,
            (-background.rect.height / 2) + base_height / 2
        );

        bool alternator = false;
        for (int i = 0; i < navbar_box.childCount; i++)
        {
            // Check if the child has a Button component
            Transform childTransform = navbar_box.GetChild(i);
            Button button = childTransform.GetComponent<Button>();
            if (button != null)
            {
                RectTransform buttonRect = button.gameObject.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(
                    navbar_box_rect.sizeDelta.x / 2,
                    navbar_box_rect.sizeDelta.y
                );

                if (alternator)
                {
                    buttonRect.localPosition = new Vector2(-buttonRect.sizeDelta.x / 2, 0);
                    alternator = !alternator;
                }
                else
                {
                    buttonRect.localPosition = new Vector2(buttonRect.sizeDelta.x / 2, 0);
                    alternator = !alternator;
                }
            }
        }

        RectTransform show_best_scores_rect =
            show_best_scores.gameObject.GetComponent<RectTransform>();

        float total_height =
            background.rect.height - title_rect.rect.height - navbar_box_rect.rect.height;
        show_best_scores_rect.sizeDelta = new Vector2(
            background.rect.width - 2 * margin,
            total_height - 2 * margin
        );
        show_best_scores_rect.localPosition = new Vector2(0, 0);
    }

    public void CheckAndStorePlayerScores(string filePath)
    {
        List<PlayerScore> all_score_list;
        PlayerScore[] all_score = new PlayerScore[] { };

        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(filePath);
            // Deserialize the JSON string back to a PlayerScoreWrapper object
            PlayerScoreWrapper wrapper = JsonUtility.FromJson<PlayerScoreWrapper>(json);
            all_score = wrapper.score_Database;
        }
        if (all_score == null)
        {
            all_score_list = new List<PlayerScore>();
        }
        else
        {
            all_score_list = all_score.ToList();
        }

        string current_date_pref = PlayerPrefs.GetString(CustomConstants.lastScore_currentTime);
        float last_time_elapsed_pref = PlayerPrefs.GetFloat(CustomConstants.lastScore_timeElapsed);

        int rune_count_pref = PlayerPrefs.GetInt(CustomConstants.rune_count_pref);
        int n_variations_index_pref = PlayerPrefs.GetInt(CustomConstants.n_variations_index_pref);
        int last_fail_attempts_pref = PlayerPrefs.GetInt(CustomConstants.lastScore_failAttempts);

        int positive = rune_count_pref * (n_variations_index_pref - 1) * 10;
        int negative = last_fail_attempts_pref * 10;
        int score = positive - negative;

        Debug.Log(
            $"Date: {current_date_pref}, Elapsed_time: {last_time_elapsed_pref}, Score: {score} "
        );
        PlayerScore new_score = new PlayerScore(current_date_pref, last_time_elapsed_pref, score);

        all_score_list.Add(new_score);

        // Sort the list by score in descending order
        all_score_list = all_score_list
            .OrderByDescending(s => s.score)
            .ThenBy(s => s.time)
            .ToList();

        int limit = 5;
        if (all_score_list.Count > limit)
        {
            all_score_list = all_score_list.Take(limit).ToList();
        }

        string divisor = $"<---------------------------------------->\n";
        string scoreText = divisor;
        foreach (PlayerScore entry in all_score_list)
        {
            scoreText += $"Date: {entry.date}, Time: {entry.time}, Score: {entry.score}\n";
            scoreText += divisor;
        }

        // Set the text of the TextMeshProUGUI component
        show_best_scores.text = scoreText;

        all_score = all_score_list.ToArray();

        PlayerScoreWrapper save_changes = new PlayerScoreWrapper(all_score);

        string json_data = JsonUtility.ToJson(save_changes);
        Debug.Log(json_data);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json_data);

        /*
        PlayerScoreWrapper list = new PlayerScoreWrapper(
            new PlayerScore[]
            {
                new PlayerScore("Date", 10, 100),
                new PlayerScore("Date2", 11, 1000),
                new PlayerScore("Date3", 12, 10000)
            }
        );
        */
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
