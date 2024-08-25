using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlayerScore
{
    public string current_date;
    public string current_time;
    public int score;
    public float elapsed_time;

    public PlayerScore(string current_date, string current_time, int score, float elapsed_time)
    {
        this.current_date = current_date;
        this.current_time = current_time;
        this.score = score;
        this.elapsed_time = elapsed_time;
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

public class ScoreRegister : MonoBehaviour
{
    private static string filePath = Path.Combine(
        Application.persistentDataPath,
        "PlayerScores.json"
    );

    public static void RegisterScore(
        string current_date,
        string current_time,
        int runecount,
        int n_variations,
        float elapsed_time,
        int fail_count
    )
    {
        int positive = runecount * (n_variations - 1) * 10;
        int negative = fail_count * 10;
        int score = positive - negative;

        PlayerScore new_score = new PlayerScore(current_date, current_time, score, elapsed_time);

        List<PlayerScore> scores = new List<PlayerScore>();
        scores = ReadStoredScores();
        scores.Add(new_score);
        scores = scores.OrderByDescending(s => s.score).ThenBy(s => s.elapsed_time).ToList();

        int limit = 3;
        if (scores.Count > limit)
        {
            scores = scores.Take(limit).ToList();
        }

        WriteScores(scores, filePath);
    }

    public static List<PlayerScore> ReadStoredScores()
    {
        PlayerScore[] all_score = new PlayerScore[] { };

        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(filePath);

            Debug.Log($"Json: {json}");
            // Deserialize the JSON string back to a PlayerScoreWrapper object
            PlayerScoreWrapper wrapper = JsonUtility.FromJson<PlayerScoreWrapper>(json);
            Debug.Log($"Wrapper: {JsonUtility.ToJson(wrapper)}");
            all_score = wrapper.score_Database ?? new PlayerScore[0];
            //all_score=null;
        }
        if (all_score == null)
        {
            return new List<PlayerScore>();
        }
        else
        {
            return all_score.ToList();
        }
    }

    private static void WriteScores(List<PlayerScore> scores, string filePath)
    {
        PlayerScoreWrapper save_changes = new PlayerScoreWrapper(scores.ToArray());

        string json_data = JsonUtility.ToJson(save_changes);
        Debug.Log(json_data);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json_data);
    }
}
