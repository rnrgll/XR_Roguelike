using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueContainer leftContainer;
        [SerializeField] private DialogueContainer middleContainer;
        [SerializeField] private DialogueContainer rightContainer;
        [SerializeField] private GameObject _rootUI;
        
        
        public event Action OnEndDialogue;
        
        private List<DialogueLine> currentLines;
        private int index;
        private bool isTyping = false;
        private Coroutine typingCoroutine = null;
        private DialogueContainer currentContainer;

        private void Start()
        {
           PlayDialogue(Manager.Dialogue.DialogueData.lines);
        }

        public void PlayDialogue(List<DialogueLine> lines)
        {
            Clear();

            currentLines = lines;
            
            _rootUI.SetActive(true);
            Speak();
        }
        
        private void Speak()
        {
            DialogueLine line = currentLines[index];
            Speaker speaker = line.GetSpeaker();
            Sprite sprite = speaker.GetSprite(line.portraitKey);
            
            //모든 말풍선 끄기
            HideAllSpeaker();
            
            //Container 선택
            (DialogueContainer from, DialogueContainer to) = SelectContainer(line.position);
           
            // 이전 이벤트 연결 제거
            if (currentContainer != null)
            {
                currentContainer.OnTypingEnd -= HandleTypingEnd;
            }
            
            //Container 교체
            currentContainer = from;
            
            // 새로운 이벤트 연결
            currentContainer.OnTypingEnd += HandleTypingEnd;
            
            isTyping = true;
            
            // 이동 여부에 따라 target position 전달
            Vector2? moveTo = from == to ? null : to.portraitOriginAnchorPos;

            typingCoroutine = from.Show(
                speaker,
                sprite,
                line.dialogueText,
                moveTo
            );
            
        }

        
        public void Next()
        {
            if (currentLines == null || currentContainer == null) return;
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
        
        private void SkipTyping()
        {
            if (typingCoroutine != null && currentContainer != null)
            {
                StopCoroutine(typingCoroutine);
                currentContainer.ShowFullText(currentLines[index].dialogueText); // 전체 텍스트 즉시 출력
                isTyping = false;
                typingCoroutine = null;
                //모든 라인을 출력해야하는데... currentspeaker를 캐싱해서 해야하나..? 어쩌지
            }
        }


      
       private  (DialogueContainer, DialogueContainer)  SelectContainer(string position)
        {
            string[] pos = position.ToLower().Split("to");
            string from = pos[0];
            string to = pos.Length > 1 ? pos[1] : pos[0];

            return (GetContainer(from), GetContainer(to));
        }

        DialogueContainer GetContainer(string pos) => pos switch
        {
            "left" => leftContainer,
            "middle" => middleContainer,
            "right" => rightContainer,
            _ => middleContainer
        };
      
        private void HandleTypingEnd()
        {
            isTyping = false;
        }
        
        private void EndDialogue()
        {
            HideAllSpeaker();
            Clear();
            Manager.Dialogue.ClearDialogueData();
            _rootUI.SetActive(false);
            SceneManager.UnloadSceneAsync("Dialogue");

            OnEndDialogue?.Invoke();
        }

     

        private void HideAllSpeaker()
        {
            //모든 UI 끄기
            leftContainer.DeactiveUI();
            rightContainer.DeactiveUI();
            middleContainer.DeactiveUI();
        }
        private void Clear()
        {
            index = 0;
            currentLines = null;
            currentContainer = null;
            typingCoroutine = null;
            isTyping = false;
        }
      
    }
}