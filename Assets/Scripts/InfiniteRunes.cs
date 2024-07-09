using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class InfiniteRunes : MonoBehaviour
{
    public RuneList runeList;

    [SerializeField]
    private GameObject runeTemplate,
        select_frame;
    private float margin = 20f; // Minimum distance between runes (both X and Y)
    private List<GameObject> selected_runes = new List<GameObject>();

    [SerializeField]
    private FailCounter failCounter;

    public void Start()
    {
        int runeCount= CreateBoard();
        int variations= 2;
        Assign_runes(variations);


    }

    public int CreateBoard()
    {
        int n_runes_index = 4;
        int total_runes;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float playableWidth = rectTransform.rect.width;
        float playableHeight = rectTransform.rect.height;

        Debug.Log($"This UI object's width: {playableWidth}, height: {playableHeight}");

        float runeWidth = (playableWidth - margin * (n_runes_index - 1)) / 4;
        float runeHeight = runeWidth;
        Vector2 new_size = new Vector2(runeWidth, runeHeight);
        runeTemplate.GetComponent<RectTransform>().sizeDelta = new_size;
        select_frame.GetComponent<RectTransform>().sizeDelta = new_size;
        Debug.Log($"Newsize: {runeWidth}x{runeHeight}");

        // Calculate the maximum number of runes that fit in a row
        int max_runes_per_row = Mathf.FloorToInt(playableWidth / runeWidth );
        // Calculate the maximum number of runes that fit in a column
        int max_runes_per_column = Mathf.FloorToInt(playableHeight / runeHeight);
        // Calculate the total number of runes that fit
        total_runes = max_runes_per_row * max_runes_per_column;
        Debug.Log($"runes per row: {max_runes_per_row}, runes per column: {max_runes_per_column}");
        Debug.Log("Total runes that fit: " + total_runes);

        float runeArrayWidth = (max_runes_per_row - 1) * (runeWidth + margin) / 2;
        float runeArrayHeight = (max_runes_per_column - 1) * (runeHeight + margin) / 2;
        for (int i = 0; i < total_runes; i++)
        {
            // Calculate position for current rune (i)
            int row = i % max_runes_per_row;
            int col = i / max_runes_per_row;

            //float x = col * (runeWidth + margin) + margin;
            float x = -runeArrayWidth + row * (runeWidth + margin);

            //float y = playableHeight - ((row + 1) * (runeHeight + margin)); // Start from top, adjust as needed
            float y = runeArrayHeight - col * (runeHeight + margin);

            // Create a new rune instance from the template (consider object pooling for efficiency)
            GameObject newrune = Instantiate(runeTemplate, transform);
            newrune.name = ($"rune Row:{row}, Col:{col}");
            newrune.transform.localPosition = new Vector3(x, y, 0); // Set position based on calculations
            newrune.GetComponent<RuneInteraction>().runeList = runeList;
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

 private void Assign_runes(int n_variations_index)
    {
        System.Random random = new System.Random();

        int[] random_id_array = new int[n_variations_index];

        int identation=0;
        while (identation<random_id_array.Length)
        {
             int new_id=random.Next(0, runeList.runes.Count);
             if (!random_id_array.Contains(new_id))
             {
                random_id_array[identation] = new_id;
                identation+=1;
             }
        }

        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            AddEventTrigger(
                child.gameObject,
                EventTriggerType.PointerClick,
                (eventData) => OnGameObjectClicked(child.gameObject)
            );
            AddEventTrigger(
                child.gameObject,
                EventTriggerType.PointerEnter,
                (eventData) => OnPointerEnter(child.gameObject)
            );
            AddEventTrigger(
                child.gameObject,
                EventTriggerType.PointerExit,
                (eventData) => OnPointerExit(child.gameObject)
            );
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
            frame.GetComponent<RectTransform>().sizeDelta=obj.GetComponent<RectTransform>().sizeDelta;
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
        yield return new WaitForSeconds(2);
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
        Debug.Log($"Child count: {gameObject.transform.childCount}");
        if (gameObject.transform.childCount == 0)
                {
                    
                    SceneManager.LoadScene("VictoryState");
                }
        
    }

    public void GoBack()
    {
        SceneManager.LoadScene("TitleState");
    }

}
