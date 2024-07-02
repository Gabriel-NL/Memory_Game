using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    private int image_id;
    public bool clicked = false;

    [SerializeField]
    public CardList cardList;
    public Image texture;
    private Color initial_color;
    private Sprite card_back,
        card_front;

    public void Set_id(int id)
    {
        this.image_id = id;
        this.texture=gameObject.GetComponent<Image>();
        card_back = cardList.card_back;
        card_front = cardList.Get_card(image_id);
        initial_color=texture.color;
        texture.sprite = card_back;
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
            texture.sprite = card_back;
            hide = !hide;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = card_front;
            gameObject.GetComponent<Image>().color = initial_color;
            hide = !hide;
        }
    }
}
