using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardCreator : MonoBehaviour
{
    public RectTransform navBar,
        counterDiv,
        buttonDiv,
        playableArea;

    private float externalMargin = 10f;
    public RuneList runeList;

    [SerializeField]
    private GameObject runeTemplate,
        select_frame;
    private float internalMargin = 20f; // Minimum distance between runes (both X and Y)
    private List<GameObject> selected_runes = new List<GameObject>();
    public UnityEvent trigger_fail;
    private int rune_count;

    void Start()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float height_fragments = rectTransform.rect.height / 16;
        int n_runes_rows_index = PlayerPrefs.GetInt(CustomConstants.n_cards_index_pref);
        Debug.Log($"n_runes_rows_index: {n_runes_rows_index}");
        //int n_runes_rows_index = 16;
        int variations =  PlayerPrefs.GetInt(CustomConstants.n_variations_index_pref);
        Debug.Log($"variations: {variations}");
        //int variations = 2;

        int n_runes_cols_index = 8;
        AdjustPlayableArea(rectTransform, height_fragments, n_runes_rows_index);
        AdjustNavbar(rectTransform, height_fragments);
        rune_count = CreateBoard(n_runes_rows_index, n_runes_cols_index, variations);
        PlayerPrefs.SetInt(CustomConstants.rune_count_pref, rune_count);
    }

    public void AdjustPlayableArea(
        RectTransform target_transform,
        float height_fragments,
        float n_rows
    )
    {
        float max_height = height_fragments * 14 - externalMargin;
        float max_width = target_transform.rect.width - (externalMargin * 2);

        playableArea.sizeDelta = new Vector2(
            playableArea.rect.width / 100,
            playableArea.rect.height / 100
        );

        float current_height = playableArea.rect.height;
        float width_modifier = 8f;
        float current_width = current_height / (width_modifier / n_rows);
        Debug.Log($"Max width: {max_width},Max height: {max_height}");


        float width_with_max_height = (max_height * current_width) / current_height;
        float height_with_max_width = (max_width * current_height) / current_width;
        Debug.Log($"width_with_max_height: {width_with_max_height}, height_with_max_width: {height_with_max_width}");

        if (width_with_max_height == height_with_max_width)
        {
            playableArea.sizeDelta = new Vector2(max_width, max_height);
        }
        else
        {
            if (max_height > height_with_max_width)
            {
                playableArea.sizeDelta = new Vector2(max_width, height_with_max_width);
                Debug.Log(playableArea.rect.width);
            }
            else
            {
                playableArea.sizeDelta = new Vector2(width_with_max_height, max_height);
            }
        }
        float y_pos =
            (target_transform.rect.height / 2) - externalMargin - (playableArea.sizeDelta.y / 2);
        // Set the anchored position (center)
        playableArea.anchoredPosition = new Vector2(0, y_pos);
    }

    public void AdjustNavbar(RectTransform rectTransform, float height_fragments)
    {
        float navbar_height = height_fragments * 1.5f;
        float navBar_width = rectTransform.rect.width;
        float x_pos;
        float y_pos;

        navBar.sizeDelta = new Vector2(navBar_width, navbar_height);
        y_pos = -(rectTransform.rect.height / 2) + navBar.sizeDelta.y / 2;
        navBar.localPosition = new Vector2(0, y_pos);

        float width_fragments = navBar_width / 5;

        buttonDiv.sizeDelta = new Vector2(width_fragments * 2, navbar_height);
        x_pos = -(navBar_width / 2) + buttonDiv.sizeDelta.x / 2;
        buttonDiv.localPosition = new Vector2(x_pos, 0);

        counterDiv.sizeDelta = new Vector2(width_fragments * 3, navbar_height);
        x_pos = (navBar_width / 2) - (counterDiv.sizeDelta.x / 2);
        counterDiv.localPosition = new Vector2(x_pos, 0);
    }

    public int CreateBoard(int n_runes_rows_index, int n_runes_cols_index, int variations)
    {
        int total_runes;

        float playableWidth = playableArea.rect.width;
        float playableHeight = playableArea.rect.height;

        Debug.Log($"This UI object's width: {playableWidth}, height: {playableHeight}");

        float runeWidth =
            (playableWidth - internalMargin * (n_runes_rows_index - 1)) / n_runes_rows_index;

        float runeHeight =
            (playableHeight - internalMargin * (n_runes_cols_index - 1)) / n_runes_cols_index;
        Vector2 new_size = new Vector2(runeWidth, runeHeight);
        runeTemplate.GetComponent<RectTransform>().sizeDelta = new_size;
        select_frame.GetComponent<RectTransform>().sizeDelta = new_size;
        Debug.Log($"Newsize: {runeWidth}x{runeHeight}");

        total_runes = n_runes_rows_index * n_runes_cols_index;
        Debug.Log($"runes per row: {n_runes_rows_index}, runes per column: {n_runes_cols_index}");
        Debug.Log("Total runes that fit: " + total_runes);
        int[] sequence = IDSequence(variations, total_runes);

        float runeArrayWidth = (n_runes_rows_index - 1) * (runeWidth + internalMargin) / 2;
        float runeArrayHeight = (n_runes_cols_index - 1) * (runeHeight + internalMargin) / 2;

        for (int i = 0; i < total_runes; i++)
        {
            // Calculate position for current rune (i)
            int row = i % n_runes_rows_index;
            int col = i / n_runes_rows_index;

            //float x = col * (runeWidth + margin) + margin;
            float x = -runeArrayWidth + row * (runeWidth + internalMargin);

            //float y = playableHeight - ((row + 1) * (runeHeight + margin)); // Start from top, adjust as needed
            float y = runeArrayHeight - col * (runeHeight + internalMargin);
            // Create a new rune instance from the template (consider object pooling for efficiency)
            GameObject newrune = Instantiate(runeTemplate, playableArea.transform);

            newrune.name = ($"rune Row:{row}, Col:{col}");
            newrune.transform.localPosition = new Vector3(x, y, 0); // Set position based on calculations
            RuneInteraction script = newrune.GetComponent<RuneInteraction>();
            script.runeList = runeList;
            script.Set_id(sequence[i]);
            script.Set_coordinates(row, col);
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
        return total_runes;
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
        if (n_variations_index > runeList.runes.Count)
        {
            n_variations_index = runeList.runes.Count;
        }

        if (n_variations_index==null || n_variations_index==0)
        {
            Debug.LogError("Player prefs not loaded");
        }
        List<int> all_id_list = Enumerable.Range(0, runeList.runes.Count).ToList();
        int[] selected_id_array = new int[n_variations_index];

        for (int i = 0; i < selected_id_array.Length; i++)
        {
            int new_id = all_id_list[random.Next(0, all_id_list.Count)];
            selected_id_array[i] = new_id;
            all_id_list.Remove(new_id);
        }
        List<int> id_sequence = new List<int>(); //Size should be equal to totalrunes
        for (int i = 0; i < total_runes; i += 2)
        {
            int randomID = random.Next(0, selected_id_array.Length);
            int selected_id = selected_id_array[randomID];
            id_sequence.Add(selected_id);
            id_sequence.Add(selected_id);
        }

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
        if (selected_runes.Count == 0)
        {
            selected_runes.Add(obj);

            GameObject frame = Instantiate(select_frame, Vector3.zero, Quaternion.identity);
            frame.transform.position = obj.transform.position;
            frame.transform.SetParent(obj.transform);
            frame.GetComponent<RectTransform>().sizeDelta =
                obj.GetComponent<RectTransform>().sizeDelta;
            frame.transform.localScale = Vector3.one;
        }

        if (selected_runes.Count > 0 && selected_runes[0] != obj)
        {
            selected_runes.Add(obj);
            foreach (GameObject item in selected_runes)
            {
                item.GetComponent<RuneInteraction>().Hide_face(false);
            }
            // Destroy child objects of the first selected rune
            foreach (Transform child in selected_runes[0].transform)
            {
                Destroy(child.gameObject);
            }

            StartCoroutine(ShowAndHide(obj));
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

    IEnumerator ShowAndHide(GameObject obj)
    {
        Debug.Log("Coroutine started...");
        EventSystem eventSystem = EventSystem.current;
        // Wait for the specified amount of time

        // Code to execute after waiting
        Debug.Log("Coroutine finished waiting.");
        List<GameObject> deletable = new List<GameObject>();
        bool deleteBoth = false;
        int id_1 = selected_runes[0].GetComponent<RuneInteraction>().GetImageId();
        int id_2 = selected_runes[1].GetComponent<RuneInteraction>().GetImageId();
        if (id_1 == id_2)
        {
            deleteBoth = true;

            foreach (var item in selected_runes)
            {
                deletable.Add(item);
            }
        }

        eventSystem.enabled = false;
        yield return new WaitForSeconds(1f);

        // Hide faces of selected runes
        foreach (GameObject item in selected_runes)
        {
            if (deleteBoth)
            {
                Destroy(item);
            }
            else
            {
                if (item.GetComponent<RuneInteraction>().clicked)
                {
                    trigger_fail.Invoke();
                }
                else
                {
                    item.GetComponent<RuneInteraction>().clicked = true;
                }
                item.GetComponent<RuneInteraction>().Hide_face(true);
            }
        }
        selected_runes.Clear();
        eventSystem.enabled = true;

        yield return new WaitForSeconds(0.1f);
        Debug.Log($"Child count: {playableArea.transform.childCount}");
        if (playableArea.transform.childCount == 0)
        {
            SceneManager.LoadScene("VictoryState");
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("TitleState");
    }
}
