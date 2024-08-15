using System;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class FailCounter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI counter;
    private float elapsed_time=0;
    private int fail_count=0;
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
        counter.text = $"Time: {elapsed_time:F1}{Environment.NewLine}Fails: {fail_count}";
    }
    public float GetElapsedTime(){
        return elapsed_time;
    }
    public int GetFailCount(){
        return fail_count;
    }

    void OnDisable()
    {
        CancelInvoke(timerFunctionName);
    }

}



