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
    public string current_date;
    public string current_time;
    public int score;
    public float elapsed_time;

    public PlayerScore(string current_date, string current_time, int score,float elapsed_time)
    {
        this.current_date = current_date;
        this.current_time = current_time;
        this.score = score;
        this.elapsed_time=elapsed_time;
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
    [SerializeField]
    private GameObject scores_container,scores_prefab;
    private float internal_margin = 20;


    void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerScores.json");

        List<PlayerScore> scores = new List<PlayerScore>();
        scores = ReadStoredScores(filePath);
        scores.Add(AddLastPlayerPrefsToScores());

        scores = scores.OrderByDescending(s => s.score).ThenBy(s => s.elapsed_time).ToList();

        ShowScoresToUser(scores);

        int limit = 3;
        if (scores.Count > limit)
        {
            scores = scores.Take(limit).ToList();
        }

        WriteScores(scores, filePath);
    }

    private List<PlayerScore> ReadStoredScores(string filePath)
    {
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
            return new List<PlayerScore>();
        }
        else
        {
            return all_score.ToList();
        }
    }

    private PlayerScore AddLastPlayerPrefsToScores()
    {
        string current_date_pref = PlayerPrefs.GetString(CustomConstants.lastScore_day);
        string current_time_pref = PlayerPrefs.GetString(CustomConstants.lastScore_current_time);
        
        int rune_count_pref = PlayerPrefs.GetInt(CustomConstants.rune_count_pref);
        int n_variations_index_pref = PlayerPrefs.GetInt(CustomConstants.n_variations_pref);
        int last_fail_attempts_pref = PlayerPrefs.GetInt(CustomConstants.lastScore_fails);
        
        float last_time_elapsed_pref = PlayerPrefs.GetFloat(CustomConstants.lastScore_time_elapsed);

        int positive = rune_count_pref * (n_variations_index_pref - 1) * 10;
        int negative = last_fail_attempts_pref * 10;
        int score = positive - negative;

        string formatted_time= current_date_pref+" at "+last_time_elapsed_pref;
        
        Debug.Log(
            $"Date: {formatted_time}, Elapsed_time: {last_time_elapsed_pref}, Score: {score} "
        );

        PlayerScore new_score = new PlayerScore(current_date_pref,current_time_pref, score,last_time_elapsed_pref );
        return new_score;
    }

    private void ShowScoresToUser(List<PlayerScore> scores) { 
        
        if (scores!=null && scores.Count>0) 
        {
            Debug.Log("Populating...");

            float container_height= scores_container.GetComponent<RectTransform> ().sizeDelta.y;
            float y_pos= container_height/2-scores_prefab.GetComponent<RectTransform>().sizeDelta.y/2;

            foreach (var stored_entry in scores)
            {
                GameObject new_entry= Instantiate(scores_prefab,scores_container.transform);
                
                TextMeshProUGUI child_text;
                 child_text = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
                 child_text.text=$"Score: {stored_entry.score.ToString()}";

                 child_text = GameObject.Find("Elapsed_time").GetComponent<TextMeshProUGUI>();
                 child_text.text=$"Elapsed_time: {stored_entry.elapsed_time.ToString()}";

                 child_text = GameObject.Find("Date").GetComponent<TextMeshProUGUI>();
                 child_text.text=$"Date & Time: {Environment.NewLine}{stored_entry.current_date.ToString()}, at {stored_entry.current_time.ToString()}";

                new_entry.transform.position = new Vector3(0,y_pos,0);
                y_pos+=new_entry.GetComponent<RectTransform>().sizeDelta.y + internal_margin;

            }


        }

    }

    private void WriteScores(List<PlayerScore> scores, string filePath)
    {
        PlayerScoreWrapper save_changes = new PlayerScoreWrapper(scores.ToArray());

        string json_data = JsonUtility.ToJson(save_changes);
        Debug.Log(json_data);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json_data);
    }

    public void Go_To_Title_Screen()
    {
        SceneManager.LoadScene("TitleStateV3");
    }

    public void Play_Again()
    {
        SceneManager.LoadScene("GameStateV2");
    }
}
