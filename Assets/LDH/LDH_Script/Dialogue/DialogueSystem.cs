using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueContainer leftSpeaker;
        [SerializeField] private DialogueContainer middleSpeaker;
        [SerializeField] private DialogueContainer rightSpeaker;
        [SerializeField] private GameObject _rootUI;
        
        
        public event Action OnEndDialogue;
        
        private List<DialogueLine> currentLines;
        private int index;
        private bool isTyping = false;
        private Coroutine typingCoroutine = null;
        private DialogueContainer currentSpeaker;

      
        
        
        public void PlayDialogue(List<DialogueLine> lines)
        {
            Clear();

            currentLines = lines;
            
            _rootUI.SetActive(true);
            Speak();
        }
        
        public void Next()
        {
            if (currentLines == null || currentSpeaker == null) return;
            if (isTyping)
            {
                SkipTyping();
            }
            else if (index >= currentLines.Count - 1)
            {
                EndDialogue();
            }
            else
            {
                index++;
                Speak();
            }
        }

        private void Speak()
        {
            DialogueLine line = currentLines[index];
            Speaker speaker = line.GetSpeaker();
            Sprite sprite = speaker.GetSprite(line.portraitKey);
            
            //모든 말풍선 끄기
            HideAllSpeaker();
            
            //위치에 해당하는 말풍선 활성화
           currentSpeaker = line.position switch
            {
                "left" => leftSpeaker,
                "middle" => middleSpeaker,
                "right" => rightSpeaker,
                _ => middleSpeaker
            };
           
            // 이전 이벤트 연결 제거
            currentSpeaker.OnTypingEnd -= HandlieTypingEnd;

            // 새로운 이벤트 연결
            currentSpeaker.OnTypingEnd += HandlieTypingEnd;
            
            isTyping = true;
            typingCoroutine = currentSpeaker.Show(speaker.speakerName, sprite, line.dialogueText);
        }

        private void SkipTyping()
        {
            if (typingCoroutine != null && currentSpeaker != null)
            {
                StopCoroutine(typingCoroutine);
                currentSpeaker.ShowFullText(currentLines[index].dialogueText); // 전체 텍스트 즉시 출력
                isTyping = false;
                typingCoroutine = null;
                //모든 라인을 출력해야하는데... currentspeaker를 캐싱해서 해야하나..? 어쩌지
            }
        }

        private void EndDialogue()
        {
            HideAllSpeaker();
            Clear();
            Manager.Dialogue.ClearDialogueData();
            _rootUI.SetActive(false);
            OnEndDialogue?.Invoke();
        }

        private void Clear()
        {
            index = 0;
            currentLines = null;
            currentSpeaker = null;
            typingCoroutine = null;
            isTyping = false;
        }

        private void HideAllSpeaker()
        {
            //모든 UI 끄기
            leftSpeaker.InActive();
            rightSpeaker.InActive();
            middleSpeaker.InActive();
        }

        private void HandlieTypingEnd()
        {
            isTyping = false;
        }
    }
}