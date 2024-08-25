using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    [SerializeField]
    private GameObject scores_container,
        scores_prefab;
    private float internal_margin = 20;

    void Start()
    {
        List<PlayerScore> scores = ScoreRegister.ReadStoredScores();

        Debug.Log($"Scores count: {scores.Count}");

        ShowScoresToUser(scores);
    }

    private void ShowScoresToUser(List<PlayerScore> scores)
    {
        if (scores != null && scores.Count > 0)
        {
            Debug.Log($"Scores count: {scores.Count}");
            Debug.Log("Populating...");

            float container_height = scores_container.GetComponent<RectTransform>().sizeDelta.y;
            float y_pos =
                container_height / 2 - scores_prefab.GetComponent<RectTransform>().sizeDelta.y / 2;
            int color_index = 0;

            foreach (var stored_entry in scores)
            {
                GameObject new_entry = Instantiate(scores_prefab, scores_container.transform);

                TextMeshProUGUI child_text;
                child_text = new_entry.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                child_text.text = $"Score: {stored_entry.score}";

                child_text = new_entry
                    .transform.Find("Elapsed_time")
                    .GetComponent<TextMeshProUGUI>();
                child_text.text = $"Elapsed_time: {stored_entry.elapsed_time}";

                child_text = new_entry.transform.Find("Date").GetComponent<TextMeshProUGUI>();
                child_text.text =
                    $"Date & Time: {Environment.NewLine}{stored_entry.current_date}, at {stored_entry.current_time}";

                new_entry.transform.Find("TierColor").GetComponent<Image>().color =
                    CustomConstants.PRIZECOLORS[color_index];
                color_index += 1;

                new_entry.GetComponent<RectTransform>().localPosition = new Vector3(0, y_pos, 0);
                y_pos -= new_entry.GetComponent<RectTransform>().sizeDelta.y + internal_margin;
            }
        }
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
