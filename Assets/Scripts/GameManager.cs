using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TextMeshProUGUI n_cards,
        n_variations;
    private int n_cards_index,
        n_variations_index = 0;
    private (int, int[])[] difficulty_Options;

    void Start()
    {
        Fill_options();
        Show_All_Values(difficulty_Options);
        Debug.Log($"A: {difficulty_Options.Length}");
        n_cards_index = 3;
        n_variations_index = 3;
        n_cards.text = difficulty_Options[n_cards_index].Item1.ToString();
        n_variations.text = difficulty_Options[n_cards_index].Item2[n_variations_index].ToString();
    }

    public void Show_All_Values((int, int[])[] options)
    {
        foreach (var option in options)
        {
            Debug.Log($"{option.Item1}: {{ {string.Join(", ", option.Item2)} }}");
        }
    }

    private void Fill_options()
    {
        difficulty_Options = new (int, int[])[]
        {
            (42, Get_Valid_Divisors(42, 21)), // From 2 to 21
            (36, Get_Valid_Divisors(36, 18)), // From 2 to 18
            (20, Get_Valid_Divisors(20, 10)), // From 2 to 10
            (18, Get_Valid_Divisors(18, 9)), // From 2 to 9
            (16, Get_Valid_Divisors(16, 8)), // From 2 to 8
            (14, Get_Valid_Divisors(14, 7)), // From 2 to 7
            (6, Get_Valid_Divisors(6, 3)) // From 2 to 3
        };
    }

    private int[] Get_Valid_Divisors(int numCards, int maxDiffCards)
    {
        return Enumerable
            .Range(2, maxDiffCards - 2 + 1)
            .Where(divisor => numCards % divisor == 0)
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
        PlayerPrefs.SetInt("n_cards_index", n_cards_index);
        PlayerPrefs.SetInt("n_variations_index", n_variations_index);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameState");
    }

    // Update is called once per frame
}
