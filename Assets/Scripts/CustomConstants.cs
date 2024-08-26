using UnityEngine;

public static class CustomConstants
{
    public const string play_again_enabled = "play_again_enabled";
    public const string n_variations_pref = "n_variations";
    public const string rune_count_pref = "rune_count";

    // Start is called before the first frame update
    public static Color HexToColor(string hexColor)
    {
        hexColor = hexColor.Replace("#", ""); // Remove potential '#' prefix

        if (hexColor.Length != 6 && hexColor.Length != 8)
        {
            Debug.LogError("Hex color should be 6 or 8 characters long");
            return Color.white; // Or handle the error as needed
        }

        int red = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

        int green = int.Parse(
            hexColor.Substring(2, 2),
            System.Globalization.NumberStyles.HexNumber
        );
        int blue = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        int alpha =
            hexColor.Length == 8
                ? int.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                : 255;

        return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
    }

    public static readonly Color[] PRIZECOLORS =
    {
        HexToColor("FEDD56"), // Gold
        HexToColor("c0c0c0"), // Silver
        HexToColor("CD7F32"), // Bronze
        HexToColor("898784") // Stone
    };

    public const string title_state_scene = "TitleStateV3";
    public const string game_state_scene = "GameStateV2";
    public const string score_state_scene = "VictoryStateV2";
}
