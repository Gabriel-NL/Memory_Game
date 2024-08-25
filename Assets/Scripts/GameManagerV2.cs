using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerV2 : MonoBehaviour
{
    // Start is called before the first frame update
    private int rune_count = 6 * 8,
        variations_index = 0;

    private int[] variations_options;

    [SerializeField]
    private TextMeshProUGUI variations_label;

    void Start()
    {
        variations_options = InitializeRuneCounter();
        variations_label.text = $"{variations_options[variations_index]}";
        Debug.Log(ScoreRegister.filePath);
    }

    // Update is called once per frame




    private int[] InitializeRuneCounter()
    {
        // Load all Texture2D assets in the "Runes" folder within the Resources folder
        Texture2D[] runeTextures = Resources.LoadAll<Texture2D>("Runes");

        // Count the number of textures
        int rune_variants = runeTextures.Length;

        // Use the count as a variable
        string value = "Number of runes: " + rune_variants;

        List<int> possibleVariationsList = new List<int>();

        Debug.Log(value);

        for (int i = 2; i < rune_variants; i++)
        {
            possibleVariationsList.Add(i);
        }
        return possibleVariationsList.ToArray();
    }

    public void ControlVariationsIndex(bool increase)
    {
        if (increase && variations_index + 1 < variations_options.Length)
        {
            variations_index += 1;
        }
        if (!increase && variations_index > 0)
        {
            variations_index -= 1;
        }
        variations_label.text = $"{variations_options[variations_index]}";
    }

        public void StartGameLoadingValues()
    {
        int variations=variations_options[variations_index];
        PlayerPrefs.SetInt(CustomConstants.n_variations_pref,variations);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameStateV2");
    }
}
