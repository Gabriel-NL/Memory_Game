using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlayerScore
{
    public DateTime score_time;
    public int score;
    public float elapsed_time;

    public PlayerScore(DateTime score_time, float elapsed_time,int rune_count,int n_variations,int fail_count)
    {
        this.score_time = score_time;

        int positive = rune_count * (n_variations - 1) * 10;
        int negative = fail_count * 10;
        this.score =  positive - negative;

        this.elapsed_time = elapsed_time;
    }

    public string ShowDay(){
        return score_time.ToString("MM/dd/yyyy");
    }
    public string ShowTime(){
        return score_time.ToString("HH:mm");
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
    public static string filePath = Path.Combine(
        Application.persistentDataPath,
        "PlayerScores.json"
    );

    public static void RegisterScore(PlayerScore new_score){
        List<PlayerScore> scores = new List<PlayerScore>();
        scores = ReadStoredScores();

        if (scores.Count()<4)
        {
          scores.Add(new_score);  
        }else
        {
            scores[3]=new_score;
        }
        scores = scores.OrderByDescending(s => s.score).ThenBy(s => s.elapsed_time).ToList();

        WriteScores(scores, filePath);
    }

    public static List<PlayerScore> ReadStoredScores()
    {
        PlayerScore[] all_score = new PlayerScore[] { };

        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(filePath);

            //Debug.Log($"Json: {json}");
            // Deserialize the JSON string back to a PlayerScoreWrapper object
            PlayerScoreWrapper wrapper = JsonUtility.FromJson<PlayerScoreWrapper>(json);
            //Debug.Log($"Wrapper: {JsonUtility.ToJson(wrapper)}");
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
        //Debug.Log(json_data);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json_data);
    }

}
