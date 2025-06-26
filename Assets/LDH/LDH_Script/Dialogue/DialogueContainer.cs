using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueContainer : MonoBehaviour
    {
        public Image portrait;
        public TMP_Text nameText;
        public TMP_Text dialogueText;

        private Coroutine typingCoroutine;
        public event Action OnTypingEnd;

        public Coroutine Show(string speakerName, Sprite portraitSprite, string text)
        {
            nameText.text = speakerName;
            portrait.sprite = portraitSprite;
            dialogueText.text = "";
            
            Active();
            
            if(typingCoroutine!=null)
                StopCoroutine(typingCoroutine);
           typingCoroutine =  StartCoroutine(TypeText(text, 0.03f));

           return typingCoroutine;
        }

        
        private IEnumerator TypeText(string text, float typingSpeed)
        {
            dialogueText.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                dialogueText.text = text.Substring(0, i+1);
                yield return new WaitForSeconds(typingSpeed);
            }
            
            OnTypingEnd?.Invoke();

        }

        public void ShowFullText(string fullText)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            dialogueText.text = fullText;
            OnTypingEnd?.Invoke();
        }

        public void InActive()
        {
            portrait.enabled = false;
            nameText.enabled = false;
            dialogueText.enabled = false;
        }

        public void Active()
        {
            portrait.enabled = true;
            nameText.enabled = true;
            dialogueText.enabled = true;
        }
    }
}