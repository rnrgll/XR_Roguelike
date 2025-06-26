using DG.Tweening;
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
        [Header("UI Elements")] public Image portrait;
        public TMP_Text nameText;
        public TMP_Text dialogueText;

        [Header("Animation Control")] [Range(0f, 2f)] [SerializeField]
        private float moveDuration = 0.5f;

        [Range(0f, 1f)] [SerializeField] private float moveDelay = 0.1f;
        [Range(0f, 0.5f)] [SerializeField] private float typingSpeed = 0.03f;

        public Vector2 portraitOriginAnchorPos;

        public event Action OnTypingEnd;
        private Coroutine typingCoroutine;

        #region Unity Methods

        private void Start()
        {
            portraitOriginAnchorPos = portrait.rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            OnTypingEnd += StopDOTween;
        }

        private void OnDisable()
        {
            OnTypingEnd -= StopDOTween;
        }

        #endregion

        #region 대사 출력

        public Coroutine Show(string speakerName, Sprite portraitSprite, string text, Vector2? moveTo = null)
        {
            UpdateData(speakerName, portraitSprite);
            ActiveUI();

            // 이동이 필요한 경우만 애니메이션
            if (moveTo.HasValue)
                PlayPositionMoving(portraitOriginAnchorPos, moveTo.Value);

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(text, typingSpeed));
            return typingCoroutine;
        }


        private void UpdateData(string speakerName, Sprite portraitSprite)
        {
            nameText.text = speakerName;
            portrait.sprite = portraitSprite;
            dialogueText.text = "";
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


        private IEnumerator TypeText(string text, float typingSpeed)
        {
            Debug.Log(text);
            dialogueText.text = "";
            string current = "";
            bool insideTag = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '<')
                    insideTag = true;
                current += c;

                if (c == '>')
                {
                    insideTag = false;
                    continue;
                }

                if (!insideTag)
                {
                    dialogueText.text = current;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            OnTypingEnd?.Invoke();
        }

        #endregion


        #region UI 활성 / 비활성

        public void ActiveUI()
        {
            portrait.enabled = true;
            nameText.enabled = true;
            dialogueText.enabled = true;
        }

        public void DeactiveUI()
        {
            portrait.enabled = false;
            nameText.enabled = false;
            dialogueText.enabled = false;
            ResetPortraitPos();
        }

        #endregion


        #region Animation / Effect

        public void PlayPositionMoving(Vector2 from, Vector2 to)
        {
            portrait.rectTransform.DOAnchorPos(to, moveDuration).SetEase(Ease.InOutCubic)
                .SetDelay(moveDelay);
        }

        public void StopDOTween()
        {
            DOTween.Kill(this);
        }

        public void ResetPortraitPos()
        {
            portrait.rectTransform.anchoredPosition = portraitOriginAnchorPos;
        }

        #endregion
    }
}