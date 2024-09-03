using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardCreatorV2 : MonoBehaviour
{
    public RectTransform playableArea;

    [SerializeField]
    private GameObject runeTemplate,
        select_frame;
    private float internalMargin = 20f; // Minimum distance between runes (both X and Y)
    private GameObject[] selected_runes = new GameObject[2];

    [SerializeField]
    private FailCounter fail_counter;

    private Sprite[] runes_front;
    private Sprite rune_backside;

    private int rune_count,
        n_variations;

    // Start is called before the first frame update
    void Start()
    {
        Sprite[] textures_from_runes_folder = Resources.LoadAll<Sprite>("Runes");
        runes_front = InitializeRunes(textures_from_runes_folder);
        int n_runes_rows = 5;
        int n_runes_cols = 8;
        rune_count = n_runes_rows * n_runes_cols;

        n_variations = PlayerPrefs.GetInt(CustomConstants.n_variations_pref);
        Debug.Log($"variations: {n_variations}");

        CreateBoard(n_runes_rows, n_runes_cols, n_variations);
    }

    public Sprite[] InitializeRunes(Sprite[] textures)
    {
        string different_rune = "RuneBack";
        List<Sprite> runes_front = new List<Sprite>();
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i].name == different_rune)
            {
                rune_backside = textures[i];
            }
            else
            {
                runes_front.Add(textures[i]);
            }
        }
        return runes_front.ToArray();
    }

    public void CreateBoard(int n_runes_rows, int n_runes_cols, int variations)
    {
        float playableWidth = playableArea.rect.width;
        float playableHeight = playableArea.rect.height;

        float runeWidth = (playableWidth - internalMargin * (n_runes_rows - 1)) / n_runes_rows;

        float runeHeight = (playableHeight - internalMargin * (n_runes_cols - 1)) / n_runes_cols;

        Vector2 new_size = new Vector2(runeWidth, runeHeight);
        runeTemplate.GetComponent<RectTransform>().sizeDelta = new_size;
        select_frame.GetComponent<RectTransform>().sizeDelta = new_size;

        Debug.Log($"Newsize: {runeWidth}x{runeHeight}");

        int total_runes = n_runes_rows * n_runes_cols;

        Debug.Log($"runes per row: {n_runes_rows}, runes per column: {n_runes_cols}");
        Debug.Log("Total runes that fit: " + total_runes);

        int[] sequence = IDSequence(variations, total_runes);

        float runeArrayWidth = (n_runes_rows - 1) * (runeWidth + internalMargin) / 2;
        float runeArrayHeight = (n_runes_cols - 1) * (runeHeight + internalMargin) / 2;

        for (int i = 0; i < total_runes; i++)
        {
            // Calculate position for current rune (i)
            int row = i % n_runes_rows;
            int col = i / n_runes_rows;

            //float x = col * (runeWidth + margin) + margin;
            float x = -runeArrayWidth + row * (runeWidth + internalMargin);

            //float y = playableHeight - ((row + 1) * (runeHeight + margin)); // Start from top, adjust as needed
            float y = runeArrayHeight - col * (runeHeight + internalMargin);
            // Create a new rune instance from the template (consider object pooling for efficiency)
            GameObject newrune = Instantiate(runeTemplate, playableArea.transform);

            newrune.name = ($"rune Row:{row}, Col:{col}");
            newrune.transform.localPosition = new Vector3(x, y, 0); // Set position based on calculations
            RuneInteraction script = newrune.GetComponent<RuneInteraction>();
            Sprite image = runes_front[sequence[i]];
            script.Set_id(sequence[i], image, rune_backside);
            script.SetCoordinates(row, col);
            AddEventTrigger(
                newrune,
                EventTriggerType.PointerClick,
                (eventData) => OnGameObjectClicked(newrune)
            );
            AddEventTrigger(
                newrune,
                EventTriggerType.PointerEnter,
                (eventData) => OnPointerEnter(newrune)
            );
            AddEventTrigger(
                newrune,
                EventTriggerType.PointerExit,
                (eventData) => OnPointerExit(newrune)
            );
        }
    }

    private void AddEventTrigger(
        GameObject obj,
        EventTriggerType eventType,
        UnityEngine.Events.UnityAction<BaseEventData> action
    )
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private int[] IDSequence(int n_variations_index, int total_runes)
    {
        System.Random random = new System.Random();
        if (n_variations_index > runes_front.Length)
        {
            n_variations_index = runes_front.Length;
        }

        if (n_variations_index == null || n_variations_index == 0)
        {
            Debug.LogError("Player prefs not loaded");
        }
        List<int> all_id_list = Enumerable.Range(0, runes_front.Length).ToList();
        int[] selected_id_array = new int[n_variations_index];

        for (int i = 0; i < selected_id_array.Length; i++)
        {
            int new_id = all_id_list[random.Next(0, all_id_list.Count)];
            selected_id_array[i] = new_id;
            all_id_list.Remove(new_id);
        }
        
        List<int> id_sequence = new List<int>(); //Size should be equal to totalrunes

        int slot = 0;
        for (int i = 0; i < total_runes; i += 2)
        {
            int selected_id = selected_id_array[slot];
            id_sequence.Add(selected_id);
            id_sequence.Add(selected_id);
            if (slot >= (selected_id_array.Length - 1))
            {
                slot = 0;
            }
            else
            {
                slot += 1;
            }
        }
        /*

        for (int i = 0; i < total_runes; i += 2)
        {
            int randomID = random.Next(0, selected_id_array.Length);
            int selected_id = selected_id_array[randomID];
            id_sequence.Add(selected_id);
            id_sequence.Add(selected_id);
        }
        */
        // Shuffle the sequence to avoid fully sequential lists
        id_sequence = id_sequence.OrderBy(x => random.Next()).ToList();
        string sequence_text = "";

        // Print the ID sequence
        foreach (var id in id_sequence)
        {
            sequence_text += id + " ";
        }

        return id_sequence.ToArray();
    }

    void OnGameObjectClicked(GameObject obj)
    {
        if (selected_runes[0] == null)
        {
            selected_runes[0] = obj;

            GameObject frame = Instantiate(select_frame, Vector3.zero, Quaternion.identity);
            frame.transform.position = obj.transform.position;
            frame.transform.SetParent(obj.transform);
            frame.GetComponent<RectTransform>().sizeDelta =
                obj.GetComponent<RectTransform>().sizeDelta;
            frame.transform.localScale = Vector3.one;
        }
        else
        {
            if (selected_runes[0] != obj)
            {
                selected_runes[1] = obj;
                foreach (GameObject item in selected_runes)
                {
                    item.GetComponent<RuneInteraction>().Hide_face(false);
                }
                // Destroy child objects of the first selected rune
                foreach (Transform child in selected_runes[0].transform)
                {
                    Destroy(child.gameObject);
                }
                StartCoroutine(ShowAndHide());
            }
            else
            {
                selected_runes[0] = null;
                foreach (Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void OnPointerEnter(GameObject obj)
    {
        Color new_color = obj.GetComponent<Image>().color;
        new_color.a = 0.5f;
        obj.GetComponent<Image>().color = new_color;
    }

    void OnPointerExit(GameObject obj)
    {
        var script = obj.GetComponent<RuneInteraction>();
        Color new_color = script.texture.color;

        new_color.a = 1f;
        script.texture.color = new_color;
    }

    IEnumerator ShowAndHide()
    {
        Debug.Log("Coroutine started...");
        EventSystem eventSystem = EventSystem.current;
        // Wait for the specified amount of time

        // Code to execute after waiting
        Debug.Log("Coroutine finished waiting.");
        bool deleteBoth = false;

        List<GameObject> analiser = new List<GameObject> { };
        for (int i = 0; i < selected_runes.Length; i++)
        {
            analiser.Add(selected_runes[i]);
            selected_runes[i].GetComponent<EventTrigger>().enabled=false;
            selected_runes[i] = null;
        }
        //eventSystem.enabled = false;
        yield return new WaitForSeconds(1f);
        int id_1 = analiser[0].GetComponent<RuneInteraction>().GetImageId();
        int id_2 = analiser[1].GetComponent<RuneInteraction>().GetImageId();
        


        if (id_1 == id_2)
        {
            deleteBoth = true;
        }
        else
        {
            deleteBoth = false;
        }

        for (int i = 0; i < analiser.Count; i++)
        {
            if (deleteBoth)
            {
                Destroy(analiser[i]);
            }
            else
            {
                if (analiser[i].GetComponent<RuneInteraction>().clicked)
                {
                    fail_counter.AddFail();
                }
                else
                {
                    analiser[i].GetComponent<RuneInteraction>().clicked = true;
                }
                analiser[i].GetComponent<RuneInteraction>().Hide_face(true);
                analiser[i].GetComponent<EventTrigger>().enabled=true;
            }
        }
        //eventSystem.enabled = true;

        yield return new WaitForSeconds(0.1f);
        if (playableArea.transform.childCount == 0)
        {
            SaveData();
            PlayerPrefs.SetInt(CustomConstants.play_again_enabled,1);
            SceneManager.LoadScene(CustomConstants.score_state_scene);
        }
    }

    public void SaveData()
    {
        int fail_count = fail_counter.GetFailCount();
        
        float elapsed_time = fail_counter.GetElapsedTime();
        float time_elapsed_formated = (int)(elapsed_time * 10) / 10f;

        PlayerScore score= new PlayerScore(DateTime.Now,time_elapsed_formated,rune_count,n_variations,fail_count);
        ScoreRegister.RegisterScore(score);

        Debug.Log($"Data saved!");
        Debug.Log($"Date: {score.ShowDay()} at {score.ShowTime()},elapsed_time: {time_elapsed_formated},Fails: {fail_count}");
    }

    public void GoBack()
    {
        SceneManager.LoadScene(CustomConstants.title_state_scene);
    }
}
