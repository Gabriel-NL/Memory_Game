using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FailCounter : MonoBehaviour
{
    // Start is called before the first frame update
    private int fail_count;
    private TextMeshProUGUI textMeshProUGUI;
    public void AddFail(){
        fail_count+=1;
        textMeshProUGUI.text=$"Fails: {fail_count}";
    }
    void Start()
    {
        textMeshProUGUI=gameObject.GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text=$"Fails: {fail_count}";
    }



}
