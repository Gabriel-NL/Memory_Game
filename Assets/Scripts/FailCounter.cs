using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailCounter : MonoBehaviour
{
    // Start is called before the first frame update
    private float elapsed_time=0;
    private int fail_count=0;
    private TextMeshProUGUI textMeshProUGUI;
    private const string timerFunctionName="UpdateTimer";

    public void AddFail()
    {
        fail_count += 1;
        ChangeText();
    }

    void Start()
    {
        textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
        ChangeText();
        InvokeRepeating(timerFunctionName, 0.1f, 0.1f);
    }

    void UpdateTimer()
    {
        // Code to be executed every second
        elapsed_time+=0.1f;
        ChangeText();
    }
    private void ChangeText(){
        textMeshProUGUI.text = $"Time: {elapsed_time}  Fails: {fail_count}";
    }

    void OnDisable()
    {
        DateTime now = DateTime.Now;
        string formatted_string = now.ToString("MM/dd/yyyy HH:mm:ss");
        PlayerPrefs.SetString(CustomConstants.last_date_and_time_pref, formatted_string);
        PlayerPrefs.SetInt(CustomConstants.last_fail_attempts_pref, fail_count);
        PlayerPrefs.SetFloat(CustomConstants.last_time_elapsed_pref, elapsed_time);
        PlayerPrefs.Save();
        CancelInvoke(timerFunctionName);
    }

}



