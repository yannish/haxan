using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LogEntry
{
    public string Message;
    public string StackTrace;
    public LogType Type;

    public LogEntry() { }

    public LogEntry(string message, string stackTrace, LogType type)
    {
        Message = message;
        StackTrace = stackTrace;
        Type = type;
    }
}

public class LogTracker : MonoBehaviour
{
    public List<LogEntry> Logs = new List<LogEntry>();

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        Logs.Add(new LogEntry(logString, stackTrace, type));
    }

    private void OnEnable()
    {
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public static void LogCallback(string stackTrace)
    {
        int startingPoint = stackTrace.IndexOf("Assets/", StringComparison.Ordinal);
        int middlePoint = stackTrace.IndexOf(":", startingPoint, StringComparison.Ordinal);
        int finalPoint = stackTrace.IndexOf(")", middlePoint, StringComparison.Ordinal);

        string adders = stackTrace.Substring(startingPoint, middlePoint - startingPoint);

        int lineNumber = Int32.Parse(stackTrace.Substring(middlePoint + 1, finalPoint - middlePoint - 1));

        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(adders), lineNumber);
    }
}

