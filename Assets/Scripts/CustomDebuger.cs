using System;
using System.IO;
using TMPro;
using UnityEngine;

public class CustomDebuger : MonoBehaviour
{
    public TMP_Text tmpText;

    public void ReadTextFile()
    {
        
        string filePath = Path.Combine(Application.persistentDataPath, "DebugTextFile.txt");
        if (File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);
            AddLine(fileContents);
        }
        else
        {
            // Create the file and immediately write to it
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.Write("Initial content"); // Or write whatever initial content you want
            }
            Debug.Log("File created: " + filePath);
        }

    }

    public void AddLine(string line){
        string filePath = Path.Combine(Application.persistentDataPath, "DebugTextFile.txt");
        tmpText=GameObject.FindWithTag("EditEdit").GetComponent<TMP_Text>();
        tmpText.text += $"{Environment.NewLine}>{line}";
        
        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine(" /-/ "+line);
        }
    }

    public void ClearLines(){
        string filePath = Path.Combine(Application.persistentDataPath, "DebugTextFile.txt");
        File.WriteAllText(filePath, "");
    }
}
