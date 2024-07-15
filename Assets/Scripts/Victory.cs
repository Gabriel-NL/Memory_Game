using System;
using System.Collections.Generic;
using System.IO;
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
    private List<PlayerScore> player_scores_container = new List<PlayerScore>();
    public RectTransform background;
    public TextMeshProUGUI show_best_scores;

    [SerializeField]
    private Transform navbar_box;

    void Start()
    {
        ResizeUI();
         string filePath = Path.Combine(Application.dataPath, "PlayerScores.json");

        CheckAndStorePlayerScores(filePath);
        ReadAndPrintPlayerScores(filePath);


    }

    private void ResizeUI()
    {
        RectTransform navbar_box_rect = navbar_box.gameObject.GetComponent<RectTransform>();
        float height_fragments = background.rect.height / 16;
        float navbar_height = height_fragments * 1.5f;
        navbar_box_rect.sizeDelta = new Vector2(background.rect.width, navbar_height);
        navbar_box_rect.localPosition = new Vector2(
            0,
            (-background.rect.height / 2) + navbar_height / 2
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
    }

    public void CheckAndStorePlayerScores(string filePath)
    {

        PlayerScoreWrapper list = new PlayerScoreWrapper(
            new PlayerScore[]
            {
                new PlayerScore("Date", 10, 100),
                new PlayerScore("Date2", 11, 1000),
                new PlayerScore("Date3", 12, 10000)
            }
        );

        string json_data = JsonUtility.ToJson(list);
        Debug.Log(json_data);

        // Write the JSON string to the file
        File.WriteAllText(filePath, json_data);

        // Read existing JSON data from file
        string loadedJsonData = File.ReadAllText(filePath);
        Debug.Log($"loadedJsonData: {loadedJsonData}");

    }

    void ReadAndPrintPlayerScores(string filePath)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read the JSON string from the file
            string json_data = File.ReadAllText(filePath);

            // Deserialize the JSON string back to a PlayerScoreWrapper object
            PlayerScoreWrapper loadedList = JsonUtility.FromJson<PlayerScoreWrapper>(json_data);

            Debug.Log(loadedList.score_Database.Length);
            // Print the values
            foreach (PlayerScore score in loadedList.score_Database)
            {
                Debug.Log($"Date: {score.date}, Time: {score.time}, Score: {score.score}");
            }
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
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
