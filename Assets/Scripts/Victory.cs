using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

            GameObject obj_to_highlight=null;
            DateTime latest_score= new DateTime();
            
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
                    $"Date & Time: {stored_entry.ShowDay()}, at {stored_entry.ShowTime()}";

                new_entry.transform.Find("TierColor").GetComponent<Image>().color =
                    CustomConstants.PRIZECOLORS[color_index];
                color_index += 1;

                new_entry.GetComponent<RectTransform>().localPosition = new Vector3(0, y_pos, 0);
                y_pos -= new_entry.GetComponent<RectTransform>().sizeDelta.y + internal_margin;

                if (stored_entry.GetRawTime() > latest_score)
                {
                    latest_score= stored_entry.GetRawTime();
                    obj_to_highlight=new_entry;
                    Debug.Log($"New better entry {latest_score}");
                }
            }
            if (obj_to_highlight!=null)
            {
                StartCoroutine(Highlight(obj_to_highlight));
            }else
            {
                Debug.Log("Highlight null!");
            }

        }
    }

    public IEnumerator Highlight(GameObject entry)
    {
        Image tierColor = entry.transform.Find("TierColor").GetComponent<Image>();
        Color originalColor = tierColor.color;
        float blinkDuration = 2.0f; // Duration for one full blink cycle (fade out + fade in)
        float minAlpha = 0.3f; // 20% transparency

        Debug.Log("Start Coroutine");
        
        while (true) // Repeat until the scene is changed
        {
            // Fade out
            for (float t = 0.0f; t < blinkDuration / 2; t += Time.deltaTime)
            {
                float normalizedTime = t / (blinkDuration / 2);
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(originalColor.a, minAlpha, normalizedTime);
                tierColor.color = newColor;
                yield return 1;
            }

            // Fade in
            for (float t = 0.0f; t < blinkDuration / 2; t += Time.deltaTime)
            {
                float normalizedTime = t / (blinkDuration / 2);
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(minAlpha, originalColor.a, normalizedTime);
                tierColor.color = newColor;
                yield return 1;
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
