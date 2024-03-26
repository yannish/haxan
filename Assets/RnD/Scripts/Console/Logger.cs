using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public string logMsg;


    public EditorButton logWarningBtn = new EditorButton("LogWarning");
    public void LogWarning()
    {
        Debug.LogWarning(logMsg);
    }

    public EditorButton logBtn = new EditorButton("Log");
    public void Log()
    {
        Debug.Log(logMsg);
    }
}
