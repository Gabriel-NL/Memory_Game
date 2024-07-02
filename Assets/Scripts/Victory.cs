using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update
    public void Go_To_Title_Screen()
    {
        SceneManager.LoadScene("TitleState");
    }

    public void Play_Again()
    {
        SceneManager.LoadScene("GameState");
    }
}
