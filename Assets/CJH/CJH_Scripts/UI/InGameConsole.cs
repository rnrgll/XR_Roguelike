using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameConsole : MonoBehaviour
{
    public static InGameConsole Instance;

    [Header("로그를 출력할 TextMeshProUGUI")]
    public TextMeshProUGUI consoleText;

    [Header("화면에 보관할 최대 로그 개수")]
    public int maxLines = 10;

    private readonly Queue<string> logQueue = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // 에디터 로그도 잡아들이려면
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Warning || type == LogType.Error)
        {
            Enqueue(logString);
        }
    }

    private void Enqueue(string message)
    {
        logQueue.Enqueue(message);
        if (logQueue.Count > maxLines)
            logQueue.Dequeue();

        consoleText.text = string.Join("\n", logQueue);
    }

    /// <summary>
    /// 코드에서 직접 띄우고 싶다면 이 메서드를 호출.
    /// InGameConsole.Instance.Log("여기에 텍스트");
    /// </summary>
    public void Log(string message)
    {
        Enqueue(message);
    }
}