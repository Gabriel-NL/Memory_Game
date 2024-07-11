using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public delegate void MyFunctionDelegate(string message); // Example delegate

    public MyFunctionDelegate OnLevelCompleted;

    [SerializeField]
    private FailCounter failCounter;

    void Start()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float height_fragments = rectTransform.rect.height / 16;
        int n_runes_rows_index = 6;
        int n_runes_cols_index = 8;
        int variations = 3;
        AdjustPlayableArea(rectTransform, height_fragments, n_runes_rows_index);
        AdjustNavbar(rectTransform, height_fragments);
        int runeCount = CreateBoard(n_runes_rows_index, n_runes_cols_index);
        Assign_runes(variations, playableArea);
    }

    public void AdjustPlayableArea(
        RectTransform rectTransform,
        float height_fragments,
        float n_rows
    )
    {
        float max_height = height_fragments * 14 - externalMargin;
        float max_width = rectTransform.rect.width - (externalMargin * 2);
        Debug.Log("Max width: " + max_width);
        float current_height = playableArea.rect.height;
        float width_modifier = 8f;
        float current_width = current_height / (width_modifier / n_rows);

        float width_with_max_height = (max_height * current_width) / current_height;
        float height_with_max_width = (max_width * current_height) / current_width;

        if (width_with_max_height == height_with_max_width)
        {
            playableArea.sizeDelta = new Vector2(max_width, max_height);
        }
        else
        {
            if (width_with_max_height < height_with_max_width)
            {
                playableArea.sizeDelta = new Vector2(width_with_max_height, max_height);
            }
            else
            {
                playableArea.sizeDelta = new Vector2(max_width, height_with_max_width);
            }
        }
        float y_pos =
            (rectTransform.rect.height / 2) - externalMargin - (playableArea.sizeDelta.y / 2);
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

    public int CreateBoard(int n_runes_rows_index, int n_runes_cols_index)
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

        // Calculate the maximum number of runes that fit in a row
        //int max_runes_per_row = Mathf.FloorToInt(playableWidth / runeWidth );
        // Calculate the maximum number of runes that fit in a column
        //int max_runes_per_column = Mathf.FloorToInt(playableHeight / runeHeight);
        // Calculate the total number of runes that fit
        //total_runes = max_runes_per_row * max_runes_per_column;
        total_runes = n_runes_rows_index * n_runes_cols_index;
        Debug.Log($"runes per row: {n_runes_rows_index}, runes per column: {n_runes_cols_index}");
        Debug.Log("Total runes that fit: " + total_runes);

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
        /*
        This UI object's width: 1020, height: 2080
        I want to adjust the width to fit them
        
        x + 20 + x + 20 + x + 20 + x
        novotamanho
        4x+20*3=1020
        4x+60=1020
        4x=960
        x=960/4
        x=240
        100*x= 1020

        Em formato de formula, seria:
        (4*x)+margin*(n_cards_index-1)=playableWidth
        (x)=(playableWidth-margin*(n_cards_index-1)/4)
        */
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

    private void Assign_runes(int n_variations_index, Transform area)
    {
        System.Random random = new System.Random();
        if (n_variations_index > runeList.runes.Count)
        {
            n_variations_index = runeList.runes.Count;
        }

        int[] random_id_array = new int[n_variations_index];
        List<int> all_id_list = Enumerable.Range(0, runeList.runes.Count).ToList();

        for (int i = 0; i < random_id_array.Length; i++)
        {
            int new_id = all_id_list[random.Next(0, all_id_list.Count)];
            random_id_array[i] = new_id;
            all_id_list.Remove(new_id);
            Debug.Log(new_id );
        }

        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in area)
        {
            childObjects.Add(child.gameObject);
        }

        bool alternator = true;
        int last_id = 0;
        while (childObjects.Count > 0)
        {
            if (alternator)
            {
                last_id = random_id_array[random.Next(0, random_id_array.Length)];
                alternator = !alternator;
            }
            else
            {
                alternator = !alternator;
            }
            GameObject random_child = childObjects[random.Next(0, childObjects.Count)];
            random_child.GetComponent<RuneInteraction>().Set_id(last_id);
            childObjects.Remove(random_child);
        }
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
            Debug.Log("Revealing...");
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
        eventSystem.enabled = false;
        // Wait for the specified amount of time
        yield return new WaitForSeconds(1f);
        eventSystem.enabled = true;

        // Code to execute after waiting
        Debug.Log("Coroutine finished waiting.");

        bool deleteBoth = false;
        int id_1 = selected_runes[0].GetComponent<RuneInteraction>().GetImageId();
        int id_2 = selected_runes[1].GetComponent<RuneInteraction>().GetImageId();
        if (id_1 == id_2)
        {
            deleteBoth = true;
        }

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
                    failCounter.AddFail();
                }
                else
                {
                    item.GetComponent<RuneInteraction>().clicked = true;
                }
                item.GetComponent<RuneInteraction>().Hide_face(true);
            }
        }
        selected_runes.Clear();
        yield return new WaitForSeconds(0.5f);
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
