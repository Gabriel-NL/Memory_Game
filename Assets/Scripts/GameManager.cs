using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private int n_cards_index,
        n_variations_index = 0;
    private (int, int[])[] difficulty_Options;

    [SerializeField]private RectTransform background,title,start;
    [SerializeField] private Transform n_cards_box,
        n_variations_box;
    [SerializeField]private TextMeshProUGUI n_cards,
        n_variations;
    public UnityEvent ElementCount;

    void Start()
    {
        ResizeUI();
        Fill_options();

        n_cards_index = 0;
        n_variations_index = 0;
        n_cards.text = difficulty_Options[n_cards_index].Item1.ToString();
        n_variations.text = difficulty_Options[n_cards_index].Item2[n_variations_index].ToString();
    }

    private void ResizeUI()
    {
        
        title.sizeDelta = new Vector2(background.rect.width, background.rect.height/9.6f);
        title.localPosition = new Vector2(0,background.rect.height/2- title.sizeDelta.y/2);
        title.offsetMin = new Vector2(30, title.offsetMin.y);  
        title.offsetMax = new Vector2(-30, title.offsetMax.y); 

        RectTransform n_cards_rect=ResizeBoxContents(n_cards_box);
        n_cards_rect.localPosition = new Vector3(0, n_cards_rect.rect.height/2,0);
        RectTransform n_variations_rect=ResizeBoxContents(n_variations_box);
        n_variations_rect.localPosition=new Vector3(0,-n_variations_rect.rect.height/2,0);

        start.sizeDelta = new Vector2(background.rect.width, background.rect.height/9.6f);
        start.localPosition = new Vector2(0,-background.rect.height/2+ start.sizeDelta.y/2);
        start.offsetMin = new Vector2(0, start.offsetMin.y);
        start.offsetMax = new Vector2(0, start.offsetMax.y);
    }

    private RectTransform ResizeBoxContents(Transform target_parent) { 
        // List to store TextMeshProUGUI elements
        RectTransform target_rect=target_parent.gameObject.GetComponent<RectTransform>();
        float new_parent_width = background.rect.width/1.2f;
        float new_parent_height = background.rect.height/6.0f;

        target_rect.sizeDelta = new Vector2(new_parent_width,new_parent_height);
        bool alternator=true;
        // Iterate through all children
        for (int i = 0; i < target_parent.childCount; i++)
        {
            Transform childTransform = target_parent.GetChild(i);

            // Check if the child has a TextMeshProUGUI component
            TextMeshProUGUI textElement = childTransform.GetComponent<TextMeshProUGUI>();
            if (textElement != null)
            {
                RectTransform textRect= textElement.rectTransform;

                textRect.sizeDelta= new Vector2(new_parent_width,new_parent_height/2);
                textRect.offsetMin = new Vector2(0, textRect.offsetMin.y);  // Adjust the left offset
                textRect.offsetMax = new Vector2(0, textRect.offsetMax.y); // Adjust the right offset (note the negative value)


                // Set left and right offsets
                if (alternator)
                {
                    textRect.localPosition=new Vector2(0,textRect.rect.height/2);
                    alternator= !alternator;
                }else
                {
                    textRect.localPosition=new Vector2(0,-textRect.rect.height/2);
                    alternator= !alternator;
                }
                //Debug.Log("Found TextMeshProUGUI element: " + textElement.name);
            }

            // Check if the child has a Button component
            Button button = childTransform.GetComponent<Button>();
            if (button != null)
            {
                RectTransform buttonRect= button.gameObject.GetComponent<RectTransform>();
                buttonRect.sizeDelta=new Vector2(new_parent_width/9,new_parent_height/3);

                if (alternator)
                {
                    buttonRect.localPosition=new Vector2(-new_parent_width/6,-new_parent_height/4);
                    alternator= !alternator;
                }else
                {
                    buttonRect.localPosition=new Vector2(new_parent_width/6,-new_parent_height/4);
                    alternator= !alternator;
                }

            }

        }
        return target_rect;
    }



    private void Fill_options()
    {
        difficulty_Options = new (int, int[])[]
        {
            (8*8, Get_Valid_Divisors(64, 32)), // From 2 to 21
            (8*7, Get_Valid_Divisors(56, 28)), // From 2 to 18
            (8*6, Get_Valid_Divisors(48, 24)), // From 2 to 10
            (8*5, Get_Valid_Divisors(40, 20)), // From 2 to 9
            (8*4, Get_Valid_Divisors(32, 16)), // From 2 to 8
            (8*3, Get_Valid_Divisors(24, 12)), // From 2 to 7
            (8*2, Get_Valid_Divisors(16, 08)) // From 2 to 3
        };
        System.Array.Sort(difficulty_Options, (a, b) => a.Item1.CompareTo(b.Item1));
    }

    private int[] Get_Valid_Divisors(int numCards, int maxDiffCards)
    {
        string[] files = Directory.GetFiles("Assets/Runes");
        int maxX =(files.Length/2);
        return Enumerable
        .Range(2, maxDiffCards - 2 + 1)
        .Where(divisor => numCards % divisor == 0 && divisor < maxX)
        .ToArray();
    }

    private void Update_Values()
    {
        n_cards.text = difficulty_Options[n_cards_index].Item1.ToString();
        n_variations.text = difficulty_Options[n_cards_index].Item2[n_variations_index].ToString();
    }

    public void Increase_cards_index(bool increase)
    {
        if (increase && n_cards_index + 1 < difficulty_Options.Length)
        {
            n_cards_index += 1;
        }
        if (!increase && n_cards_index > 0)
        {
            n_cards_index -= 1;
        }
        n_variations_index = 0;
        Update_Values();
    }

    public void Increase_variations_index(bool increase)
    {
        if (increase && n_variations_index + 1 < difficulty_Options[n_cards_index].Item2.Length)
        {
            n_variations_index += 1;
        }
        if (!increase && n_variations_index > 0)
        {
            n_variations_index -= 1;
        }
        Update_Values();
    }

    public void Start_game_with_selected_values()
    {
        int item1=difficulty_Options[n_cards_index].Item1/8;
        int item2= difficulty_Options[n_cards_index].Item2[n_variations_index];
        PlayerPrefs.SetInt(CustomConstants.n_cards_index_pref, item1);
        PlayerPrefs.SetInt(CustomConstants.n_variations_index_pref,item2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameState");
        Debug.Log($"n_cards_index_pref: {item1}, n_variations_index:{item2}");
    }

    // Update is called once per frame
}
