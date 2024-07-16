using System;
using TMPro;
using UnityEngine;

public class FailCounter : MonoBehaviour
{
    // Start is called before the first frame update
    private float elapsed_time=0;
    private int fail_count=0;
    private TextMeshProUGUI textMeshProUGUI;
    private const string timerFunctionName="UpdateTimer";

    public float GetTime(){
        return elapsed_time;
    }
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
        textMeshProUGUI.text = $"Time: {elapsed_time:F1}  Fails: {fail_count}";
    }

    void OnDisable()
    {
        DateTime now = DateTime.Now;
        string formatted_string = now.ToString("MM/dd/yyyy HH:mm:ss");
        float time_elapsed_formated=(int)(elapsed_time * 10) / 10f ;
        PlayerPrefs.SetString(CustomConstants.lastScore_currentTime, formatted_string);
        PlayerPrefs.SetInt(CustomConstants.lastScore_failAttempts, fail_count);
        PlayerPrefs.SetFloat(CustomConstants.lastScore_timeElapsed, time_elapsed_formated);
        Debug.Log($"Data saved!");


        
        Debug.Log($"Date: {formatted_string},elapsed_time: {elapsed_time},Fails: {fail_count} ");
        PlayerPrefs.Save();
        CancelInvoke(timerFunctionName);
    }

}



