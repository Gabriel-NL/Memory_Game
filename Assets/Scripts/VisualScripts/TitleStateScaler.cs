using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleStateScaler : MonoBehaviour
{
    [Header("BG Section")]
    [SerializeField]
    private Image outer_bg;

    [SerializeField]
    private Image inner_bg;
    private float margin = 100;

    [Space(10)]
    [Header("Title Section")]
    [SerializeField]
    private RectTransform title_img;

    [SerializeField]
    private RectTransform title_label;

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
        ScaleTitle(width_fragments * 6.9f, height_fragments * 1.7f);
    }

    private void ScaleBG()
    {
        float width = outer_bg.rectTransform.rect.width - margin;
        float height = outer_bg.rectTransform.rect.height - margin;
        Debug.Log(width);
        inner_bg.rectTransform.sizeDelta = new Vector2(width, height);
    }

    private void ScaleTitle(float width, float height)
    {
        title_img.sizeDelta = new Vector2(width, height);
        
    }
}
