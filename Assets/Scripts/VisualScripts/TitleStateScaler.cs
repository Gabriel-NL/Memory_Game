using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleStateScaler : MonoBehaviour
{
    [Header("BG Section")]
    [SerializeField]
    private Image outer_bg;

    [SerializeField]
    private Image inner_bg;
    private float BGmargin = 100;

    [Space(10)]
    [Header("Title Section")]
    [SerializeField]
    private GameObject title_object;

    [SerializeField]
    private GameObject n_runes_object;

    [SerializeField]
    private GameObject n_variations_object;

    [SerializeField]
    private GameObject start_game_object;

    [SerializeField]
    private GameObject element_container;
    private float title_margin = 30, element_margin=20;

    /*
    2160 x 4680

    divide 2160 em 10 partes de largura e 468 de altura

    o titulo ta em 6.9 partes de largura e 1.7 de altura
    */

    void Start()
    {
        ScaleBG();

        float height_fragments = inner_bg.rectTransform.rect.height / 10;
        float width_fragments = inner_bg.rectTransform.rect.width / 10;
        
        float title_width=width_fragments * 6.9f;
        float title_height=height_fragments * 1.7f;
        ScaleTitle(title_width,title_height, title_object);
        
        float element_width=width_fragments * 3.45f;
        float element_height=height_fragments * 1.33f;
        ScaleElement(element_width,element_height , n_runes_object);
        ScaleElement(element_width, element_height, n_variations_object);

        float button_width=width_fragments * 3.45f;
        float button_height=height_fragments * 1.4f;
        ScaleTitle(button_width, button_height, start_game_object);

        float container_height= 
            element_height*2+
            element_margin*2+
            button_height;

        element_container.GetComponent<RectTransform>().sizeDelta=new Vector3(element_width, container_height);

        n_runes_object.GetComponent<RectTransform>().localPosition= new Vector3(
            0,
            container_height/2-element_height/2,
            0
        );
        

        //UniteElements();

    }

    private void ScaleBG()
    {
        float width = outer_bg.rectTransform.rect.width - BGmargin;
        float height = outer_bg.rectTransform.rect.height - BGmargin;
        Debug.Log(width);
        inner_bg.rectTransform.sizeDelta = new Vector2(width, height);
    }

    private void ScaleTitle(float width, float height, GameObject obj)
    {
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        RectTransform child = obj.transform.GetChild(0).GetComponent<RectTransform>();
        //child.sizeDelta = new Vector2(width - title_margin, height - title_margin);
        child.localPosition = new Vector2(0, 0);
    }

    private void ScaleElement(float width, float height, GameObject obj)
    {
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        GameObject[] child_array = GetChilds(obj).ToArray();
        Debug.Log($"child_array Length: {child_array.Length}");
        foreach (var child in child_array)
        {
            RectTransform child_rect = child.GetComponent<RectTransform>();

            bool has_text = child.GetComponent<TextMeshProUGUI>() != null;
            float parent_height = child
                .transform.parent.GetComponentInParent<RectTransform>()
                .sizeDelta.y;
            Debug.Log($"Child: {child.gameObject.name} height: {parent_height}");
            if (has_text)
            {
                child_rect.sizeDelta = new Vector2(
                    width - title_margin,
                    parent_height - title_margin
                );
                child_rect.localPosition = new Vector2(0, 0);
            }
            else
            {
                child_rect.sizeDelta = new Vector2(width, parent_height / 2);
                if (child.name == "LowerHalf")
                {
                    child_rect.localPosition = new Vector2(0, -parent_height / 4);
                }
                if (child.name == "UpperHalf")
                {
                    child_rect.localPosition = new Vector2(0, parent_height / 4);
                }
            }
        }
    }

    private List<GameObject> GetChilds(GameObject parent)
    {
        List<GameObject> child_list = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            child_list.Add(child);
            if (child.transform.childCount > 0)
            {
                child_list.AddRange(GetChilds(child));
            }
        }
        return child_list;
    }

    private void UniteElements(){
        
        float interactible_container_height =n_runes_object.GetComponent<RectTransform>().sizeDelta.y + n_variations_object.GetComponent<RectTransform>().sizeDelta.y+title_object.GetComponent<RectTransform>().sizeDelta.y+element_margin*2;

        float interactible_container_width =title_object.GetComponent<RectTransform>().sizeDelta.x;

    }
}
