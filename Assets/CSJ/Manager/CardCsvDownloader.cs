using CustomUtility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class CardCsvDownLoader
{

    public static IEnumerator Start(string url, Action<CsvTable> onCompleted)
    {
        using (UnityWebRequest wrq = UnityWebRequest.Get(url))
        {
            yield return wrq.SendWebRequest();

            if (wrq.result == UnityWebRequest.Result.Success)
            {
                string csv = wrq.downloadHandler.text;

                CsvTable csvFile = new CsvTable(csv, ',');

                onCompleted?.Invoke(csvFile);
            }
            else
            {
                Debug.Log($"다운로드 실패 : {wrq.error}");
            }
        }
    }
}
