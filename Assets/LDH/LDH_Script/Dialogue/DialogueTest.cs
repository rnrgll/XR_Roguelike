using Dialogue;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CustomUtility.UI
{
    public class DialogueTest : MonoBehaviour
    {
        [SerializeField] private DialogueSystem system;
        public string fileName = "dialogueTestCSV.csv";
        public void Play()
        {
            Manager.Dialogue.LoadDialogueData(fileName);
            system.PlayDialogue(Manager.Dialogue.DialogueData.lines);
        }
    }
}