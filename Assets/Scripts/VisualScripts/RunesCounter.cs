using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunesCounter : MonoBehaviour
{

    public TextMeshProUGUI debug;
    void Start()
    {
        // Load all Texture2D assets in the "Runes" folder within the Resources folder
        Texture2D[] runeTextures = Resources.LoadAll<Texture2D>("Runes");

        // Count the number of textures
        int runeCount = runeTextures.Length;

        // Use the count as a variable
        string value= "Number of runes: " + runeCount;

        Debug.Log(value);
        if (runeCount!=null)
        {
            
        debug.text = value;
        }
    }

    

}
