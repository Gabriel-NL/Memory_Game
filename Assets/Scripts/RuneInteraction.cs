using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RuneInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    private int image_id;
    public bool clicked = false;

    [SerializeField]
    public RuneList runeList;
    public Image texture;
    private Color initial_color;
    private Sprite rune_back,
        rune_front;

    public void Set_id(int id)
    {
        this.image_id = id;
        this.texture=gameObject.GetComponent<Image>();
        rune_back = runeList.rune_back;
        rune_front = runeList.Get_rune(image_id);
        initial_color=texture.color;
        texture.sprite = rune_back;
    }
    public void Set_coordinates(int x,int y){

    }
    public int GetImageId(){
        return image_id;
    }

    public void Hide_face(bool hide)
    {
        if (hide)
        {
            if (clicked)
            {
                Color new_color = texture.color*0.5f;
                new_color.a = 1f;
                texture.color = new_color;
            }
            texture.sprite = rune_back;
            hide = !hide;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = rune_front;
            gameObject.GetComponent<Image>().color = initial_color;
            hide = !hide;
        }
    }
}