using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReScalerAuto : MonoBehaviour
{
    
    public RectTransform counterDiv,buttonDiv; 
    
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        float center = rectTransform.rect.width/2;


        counterDiv.offsetMin = new Vector2(center, 0f);  // Left and bottom
        counterDiv.offsetMax = new Vector2(-0, -0);// Right and top (negative values)
        buttonDiv.offsetMin = new Vector2(0, 0f);  // Left and bottom
        buttonDiv.offsetMax = new Vector2(-center, 0f);  // Right and top (negative values)
    }


}
