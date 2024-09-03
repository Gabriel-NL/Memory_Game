using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    [SerializeField]
    private GameObject scores_container,
        scores_prefab;
    private float internal_margin = 20;

    [SerializeField]
    private GameObject play_again_button;

    [SerializeField]
    private GameObject menu_button;

    private List<PlayerScore> scores = new List<PlayerScore>();

    void Start()
    {
        scores = ScoreRegister.ReadStoredScores();

        ShowScoresToUser(scores);

        if (PlayerPrefs.GetInt(CustomConstants.play_again_enabled) == 0)
        {
            play_again_button.SetActive(false);
            RectTransform rect_menu = menu_button.GetComponent<RectTransform>();
            rect_menu.localPosition = new Vector3(0, 0, 0);
            rect_menu.sizeDelta = new Vector2(rect_menu.sizeDelta.x * 2, rect_menu.sizeDelta.y);
        }
    }

    private void ShowScoresToUser(List<PlayerScore> scores)
    {
        if (scores != null && scores.Count > 0)
        {
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
                    $"Date & Time: {System.Environment.NewLine}{stored_entry.score_time.ToString("MM/dd/yyyy")}, at {stored_entry.score_time.ToString("HH:mm")}";

                new_entry.transform.Find("TierColor").GetComponent<Image>().color =
                    CustomConstants.PRIZECOLORS[color_index];
                color_index += 1;

                new_entry.GetComponent<RectTransform>().localPosition = new Vector3(0, y_pos, 0);
                y_pos -= new_entry.GetComponent<RectTransform>().sizeDelta.y + internal_margin;
            }
        }
    }

    private void HighlightRecent()
    {
        if (scores == null || scores.Count == 0)
        {
            return;
        }

        PlayerScore mostRecentScore = scores[0];

        foreach (var score in scores)
        {
            if (score.score_time > mostRecentScore.score_time)
            {
                mostRecentScore = score;
            }
        }
    }

    

    public void Go_To_Title_Screen()
    {
        SceneManager.LoadScene(CustomConstants.title_state_scene);
    }

    public void Play_Again()
    {
        SceneManager.LoadScene(CustomConstants.game_state_scene);
    }
}
