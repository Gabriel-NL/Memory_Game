using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateCards : MonoBehaviour
{
    private RectTransform rectTransform;
    public CardList cardList;

    [SerializeField]
    private GameObject cardTemplate,
        select_frame;
    private float margin = 15f; // Minimum distance between cards (both X and Y)
    private List<GameObject> selected_cards = new List<GameObject>();

    [SerializeField]
    private FailCounter failCounter;

    void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        if (rectTransform != null)
        {
            int n_cards_index = PlayerPrefs.GetInt("n_cards_index");
            int total_cards;
            if (n_cards_index != null)
            {
                total_cards = Set_Card_Arrangement(n_cards_index);
            }
            else
            {
                total_cards = Set_Card_Arrangement();
            }
            int n_variations_index = PlayerPrefs.GetInt("n_variations_index");
            if (n_variations_index == null || n_variations_index == 0)
            {
                n_variations_index = 20;
                n_variations_index = Check_number_eligibility(total_cards, n_variations_index);
            }
            Assign_cards(n_variations_index);
        }
        else
        {
            Debug.LogError("RectTransform component not found on this GameObject.");
        }
    }

    private int Set_Card_Arrangement()
    {
        float playableWidth = rectTransform.rect.width;
        float playableHeight = rectTransform.rect.height;

        Debug.Log($"This UI object's width: {playableWidth}, height: {playableHeight}");

        /*
        //Increase size temporalilly
        */
        float original_width = 143.8f;
        float original_height = 200f;
        float size_mod = 2.2f;
        Vector2 new_size = new Vector2(original_width * size_mod, original_height * size_mod);
        cardTemplate.GetComponent<RectTransform>().sizeDelta = new_size;
        float cardWidth = cardTemplate.GetComponent<RectTransform>().rect.width;
        float cardHeight = cardTemplate.GetComponent<RectTransform>().rect.height;

        // Calculate usable width for cards (excluding margins)
        float usableWidth = playableWidth - (2 * margin);

        // Calculate the maximum number of cards that fit in a row
        int max_cards_per_row = Mathf.FloorToInt(usableWidth / (cardWidth + margin));

        // Calculate usable height for cards (excluding margins)
        float usableHeight = playableHeight - (2 * margin);

        // Calculate the maximum number of cards that fit in a column
        int max_cards_per_column = Mathf.FloorToInt(usableHeight / (cardHeight + margin));

        // Calculate the total number of cards that fit
        int max_total_cards = max_cards_per_row * max_cards_per_column;

        Debug.Log("Cards per row: " + max_cards_per_row);
        Debug.Log("Cards per column: " + max_cards_per_column);
        Debug.Log("Total cards that fit: " + max_total_cards);

        bool isEven = max_total_cards % 2 == 0;
        if (!isEven)
        {
            max_total_cards = -1;
        }

        float cardArrayWidth = (max_cards_per_row - 1) * (cardWidth + margin) / 2;
        float cardArrayHeight = (max_cards_per_column - 1) * (cardHeight + margin) / 2;

        // Instantiate (or activate) card prefabs based on totalCards
        for (int i = 0; i < max_total_cards; i++)
        {
            // Calculate position for current card (i)
            int row = i % max_cards_per_row;
            int col = i / max_cards_per_row;

            //float x = col * (cardWidth + margin) + margin;
            float x = -cardArrayWidth + row * (cardWidth + margin);

            //float y = playableHeight - ((row + 1) * (cardHeight + margin)); // Start from top, adjust as needed
            float y = cardArrayHeight - col * (cardHeight + margin);

            // Create a new card instance from the template (consider object pooling for efficiency)
            GameObject newCard = Instantiate(cardTemplate, transform);
            newCard.name = ($"Card Row:{row}, Col:{col}");
            newCard.transform.localPosition = new Vector3(x, y, 0); // Set position based on calculations
            newCard.GetComponent<CardInteraction>().cardList = cardList;
        }
        return max_total_cards;
    }

    public int Set_Card_Arrangement(int n_cards_index)
    {
        float playableWidth = rectTransform.rect.width;
        float playableHeight = rectTransform.rect.height;
        float original_width = 143.8f;
        float original_height = 200f;

        float size_mod = 1.0f;

        switch (n_cards_index)
        {
            case 42:
                size_mod = 1.0f;
                break;
            case 36:
                size_mod = 1.1f;
                break;
            case 20:
                size_mod = 1.4f;
                break;
            case 18:
                size_mod = 1.5f;
                break;
            case 16:
                size_mod = 1.7f;
                break;
            case 14:
                size_mod = 1.9f;
                break;
            case 6:
                size_mod = 2.2f;
                break;

            default:
                break;
        }

        Vector2 new_size = new Vector2(original_width * size_mod, original_height * size_mod);
        cardTemplate.GetComponent<RectTransform>().sizeDelta = new_size;
        float cardWidth = cardTemplate.GetComponent<RectTransform>().rect.width;
        float cardHeight = cardTemplate.GetComponent<RectTransform>().rect.height;

        // Calculate usable width for cards (excluding margins)
        float usableWidth = playableWidth - (2 * margin);

        // Calculate the maximum number of cards that fit in a row
        int max_cards_per_row = Mathf.FloorToInt(usableWidth / (cardWidth + margin));

        // Calculate usable height for cards (excluding margins)
        float usableHeight = playableHeight - (2 * margin);

        // Calculate the maximum number of cards that fit in a column
        int max_cards_per_column = Mathf.FloorToInt(usableHeight / (cardHeight + margin));

        // Calculate the total number of cards that fit
        int max_total_cards = max_cards_per_row * max_cards_per_column;

        float cardArrayWidth = (max_cards_per_row - 1) * (cardWidth + margin) / 2;
        float cardArrayHeight = (max_cards_per_column - 1) * (cardHeight + margin) / 2;

        // Instantiate (or activate) card prefabs based on totalCards
        for (int i = 0; i < max_total_cards; i++)
        {
            // Calculate position for current card (i)
            int row = i % max_cards_per_row;
            int col = i / max_cards_per_row;

            //float x = col * (cardWidth + margin) + margin;
            float x = -cardArrayWidth + row * (cardWidth + margin);

            //float y = playableHeight - ((row + 1) * (cardHeight + margin)); // Start from top, adjust as needed
            float y = cardArrayHeight - col * (cardHeight + margin);

            // Create a new card instance from the template (consider object pooling for efficiency)
            GameObject newCard = Instantiate(cardTemplate, transform);
            newCard.name = ($"Card Row:{row}, Col:{col}");
            newCard.transform.localPosition = new Vector3(x, y, 0); // Set position based on calculations
            newCard.GetComponent<CardInteraction>().cardList = cardList;
        }
        return max_total_cards;
    }

    private void Assign_cards(int n_variations_index)
    {
        System.Random random = new System.Random();

        int[] random_id_array = new int[n_variations_index];

        for (int i = 0; i < random_id_array.Length; i++)
        {
            random_id_array[i] = random.Next(0, cardList.cards.Count);
        }
        Debug.Log("" + n_variations_index);
        Debug.Log("" + random_id_array.Length);

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
            random_child.GetComponent<CardInteraction>().Set_id(last_id);
            childObjects.Remove(random_child);
        }
    }

    private int Check_number_eligibility(int max_total_cards, int different_cards_count)
    {
        bool analiser = max_total_cards % different_cards_count == 0;

        while (analiser == false && different_cards_count > 1)
        {
            different_cards_count--;
            analiser = max_total_cards % different_cards_count == 0;
        }
        Debug.Log($"Max different cards: {different_cards_count}");
        return different_cards_count;
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

    void OnGameObjectClicked(GameObject obj)
    {
        if (selected_cards.Count == 0)
        {
            selected_cards.Add(obj);

            GameObject frame = Instantiate(select_frame, Vector3.zero, Quaternion.identity);
            frame.transform.position = obj.transform.position;
            frame.transform.SetParent(obj.transform);
            frame.GetComponent<RectTransform>().sizeDelta=obj.GetComponent<RectTransform>().sizeDelta;
            frame.transform.localScale = Vector3.one;
        }

        if (selected_cards.Count > 0 && selected_cards[0] != obj)
        {
            Debug.Log("Revealing...");
            selected_cards.Add(obj);
            foreach (GameObject item in selected_cards)
            {
                item.GetComponent<CardInteraction>().Hide_face(false);
            }
            // Destroy child objects of the first selected card
            foreach (Transform child in selected_cards[0].transform)
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
        var script = obj.GetComponent<CardInteraction>();
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
        int id_1 = selected_cards[0].GetComponent<CardInteraction>().GetImageId();
        int id_2 = selected_cards[1].GetComponent<CardInteraction>().GetImageId();
        if (id_1 == id_2)
        {
            deleteBoth = true;
        }

        // Hide faces of selected cards
        foreach (GameObject item in selected_cards)
        {
            if (deleteBoth)
            {
                Destroy(item);
                
            }
            else
            {
                if (item.GetComponent<CardInteraction>().clicked)
                {
                    failCounter.AddFail();
                }
                else
                {
                    item.GetComponent<CardInteraction>().clicked = true;
                }
                item.GetComponent<CardInteraction>().Hide_face(true);
            }
        }
        selected_cards.Clear();
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
