using CustomUtility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class CSVDownloader
    {
        public static IEnumerator Start(string url, Action<CsvTable> onCompleted)
        {
            using (UnityWebRequest wrq = UnityWebRequest.Get(url))
            {
                yield return wrq.SendWebRequest();

                if (wrq.result == UnityWebRequest.Result.Success)
                {
                    string csvText = wrq.downloadHandler.text;
                    CsvTable csvTable = new CsvTable();
                    csvTable.Table = Parse(csvText);
                    onCompleted?.Invoke(csvTable);
                }
                else
                {
                    Debug.Log($"다운로드 실패 : {wrq.error}");
                }
            }
        }

        public static string[,] Parse(string csvText, char delimiter = ',')
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var field = new StringBuilder();

            bool inQuotes = false;
            for (int i = 0; i < csvText.Length; i++)
            {
                char c = csvText[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                        {
                            // "" → "
                            field.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        field.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == delimiter && !inQuotes)
                    {
                        row.Add(field.ToString());
                        field.Clear();
                    }
                    else if (c == '\r')
                    {
                        // ignore
                    }
                    else if (c == '\n')
                    {
                        if (!inQuotes)
                        {
                            row.Add(field.ToString());
                            field.Clear();
                            rows.Add(row);
                            row = new List<string>();
                        }
                        else
                        {
                            field.Append('\n'); // 셀 안 줄바꿈
                        }
                    }
                    else
                    {
                        field.Append(c);
                    }
                }
            }

            // 마지막 셀
            if (field.Length > 0 || inQuotes == false)
                row.Add(field.ToString());
            if (row.Count > 0)
                rows.Add(row);

            // 최대 열 수 계산
            int maxCols = rows.Max(r => r.Count);

            // 2차원 배열 생성
            string[,] result = new string[rows.Count, maxCols];
            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Count; c++)
                {
                    result[r, c] = rows[r][c];
                }
            }

            return result;
        }
    }
}