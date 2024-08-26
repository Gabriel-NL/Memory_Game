using UnityEngine;
using UnityEngine.UI;

public class RuneInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private int image_id;
    public bool clicked = false;
    public Image texture;
    private Color initial_color;
    private Sprite rune_back,
        rune_front;

    private (int,int) coordinates;

    public void Set_id(int id,Sprite front,Sprite back){
        this.image_id = id;
        this.texture=gameObject.GetComponent<Image>();

        this.rune_front = front;
        this.rune_back=back;

        initial_color=texture.color;
        texture.sprite = rune_back;
    }
    public void SetCoordinates(int x,int y){
        coordinates=(x,y);
    }
    public (int,int) GetCoordinates(){
        return (coordinates);
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
