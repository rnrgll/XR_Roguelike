using CustomUtility.IO;
using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Event.Dialogue;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private void Awake() => SingletonInit();
        public Action OnLoadDialogue;
        public DialougeData DialogueData { get; private set; } = new();


        private void Start()
        {
            OnLoadDialogue += LoadDialogueScene;
        }

        /// <summary>
        /// 확장자 포함한 파일 이름 (ex. IntroDialogue.csv)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="splitSymol"></param>
        /// <returns></returns>
        private bool LoadDialogueData(string fileName)
        {
            //1. CSV 테이블 생성
            CsvTable table = new CsvTable($"Data/Dialogue/{fileName}");
            
            //2. Reader로 파일 읽기
            CsvReader.Read(table);
            
            //3. 다이얼로그 파싱
            DialogueData.dialogueID = fileName;
            DialogueData.lines = ParseDialogueLine(table);
            
            Debug.Log($"{DialogueData.dialogueID} file 읽어오기 성공");
            
            OnLoadDialogue?.Invoke();
            //파싱 완료
            return true;
        }

        public List<DialogueLine> ParseDialogueLine(CsvTable table)
        {
            List<DialogueLine> lines = new List<DialogueLine>();

            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);

            Dictionary<string, int> columnMap = new();
            for (int c = 0; c < columnCnt; c++)
                columnMap[table.Table[0, c]] = c;

            for (int r = 1; r < rowCnt; r++)
            {
                DialogueLine line = new DialogueLine
                {
                    index = int.Parse(table.Table[r, columnMap["index"]]),
                    speakerName = table.Table[r, columnMap["speakerName"]],
                    position =  table.Table[r, columnMap["position"]],
                    portraitKey = table.Table[r, columnMap["portraitKey"]],
                    dialogueText = table.Table[r, columnMap["dialogueText"]].Replace("\\n", "\n"),
                };
                lines.Add(line);
            }

            return lines;
        }


        public void ClearDialogueData()
        {
            DialogueData.dialogueID = string.Empty;
            DialogueData.lines?.Clear(); // 리스트 클리어
            DialogueData.lines = null;   // 참조 해제
        }



        public void PlayDialogue(DialogueType dialogueType)
        {
            string fileName = dialogueType switch
            {
                DialogueType.Prologue => "Prologue.csv",
                DialogueType.FirstInGameShop => "FirstInGameShop.csv",
                DialogueType.InGameShop => "InGameShop.csv",
                DialogueType.BeforeBoss => "BeforeBoss.csv",
                DialogueType.AfterBoss => "AfterBoss.csv",
                DialogueType.Ending => "Ending.csv",
            };
            
            LoadDialogueData(fileName);
            
           
        }

        private void LoadDialogueScene()
        {
            SceneManager.LoadScene("Dialogue", LoadSceneMode.Additive);
        }
    }
}