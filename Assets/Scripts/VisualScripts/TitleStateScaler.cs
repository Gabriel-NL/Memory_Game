using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleStateScaler : MonoBehaviour
{

    [SerializeField] public Image outer_bg,inner_bg;
    private float margin=100;


    void Start(){
        ScaleBG();
    }

    private void ScaleBG(){
        float width= outer_bg.rectTransform.rect.width-margin;
        float height= outer_bg.rectTransform.rect.height-margin;
        Debug.Log(width);
        inner_bg.rectTransform.sizeDelta=  new Vector2(width,height);

        
    }

}
