using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Managers;

namespace Map
{
    public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image image;
        public Image visitedImage;
        
        public Node Node { get; private set; }
        public NodeTemplate Template { get; private set; }
        public NodeState NodeState { get; private set; }
        
        
        //lock 여부
        public bool IsLocked { get; set; }
        
        
        //애니메이션 효과
        private float initialScale;
        private const float hoverScaleFactor = 1.5f; // hover시 scale 증가값
        private const float maxClickDuration = 0.5f; //
        private const float scaleChangeDuration = 0.3f;
        private const float circleFillDuration = 0.8f;
        private float mouseDownTime;

        private float bossNodeFactor = 2.5f; //boss node의 scale 증가값

        
        private void OnDestroy()
        {
            if (image != null)
            {
                image.transform.DOKill();
                image.DOKill();
            }

        }
        
        public void SetUp(Node node, NodeTemplate template)
        {
            Node = node;
            Template = template;
            NodeState = NodeState.Locked;

            IsLocked = false;
            
            if (image != null) image.sprite = template.sprite;
            if (node.nodeType == NodeType.Boss) transform.localScale *= bossNodeFactor;
            if (image != null) initialScale = image.transform.localScale.x;

            if (visitedImage != null)
            {
                visitedImage.color = MapView.visitedColor;
                visitedImage.gameObject.SetActive(false);    
            }
            
            SetState(NodeState.Locked);
            
        }

        public void SetState(NodeState state)
        {
            if (visitedImage!= null) visitedImage.gameObject.SetActive(false);
            
            switch (state)
            {
                case NodeState.Locked:
                    if (image != null)
                    {
                        image.DOKill();
                        image.color = MapView.lockedColor;
                    }
                    break;
                case NodeState.Visited:
                    if (image != null)
                    {
                        image.DOKill();
                        image.color = MapView.visitedColor;
                    }
                    if (visitedImage != null) visitedImage.gameObject.SetActive(true);
                    break;
                case NodeState.Attainable:
                    if (image != null)
                    {
                        image.color = MapView.lockedColor;
                        image.DOKill();
                        image.DOColor(MapView.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            NodeState = state;
        }


        #region Event / Animation

        public void OnPointerEnter(PointerEventData eventData)
        {
           
            if (image != null)
            {
                image.transform.DOKill();
                image.transform.DOScale(initialScale * hoverScaleFactor, scaleChangeDuration);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (image != null)
            {
                image.transform.DOKill();
                image.transform.DOScale(initialScale, scaleChangeDuration);
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsLocked) return;
            mouseDownTime = Time.time; //마우스 long click, drag를 무시하기 위해 time 측정
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsLocked) return;
            
            if (Time.time - mouseDownTime < maxClickDuration)
            {
                Manager.Map.Tracker.SelectNode(this);
            }
        }

        public void ShowVisitedCircle()
        {
            if (visitedImage == null) return;
            visitedImage.fillAmount = 0f;
            visitedImage.DOFillAmount(1f, circleFillDuration);
        }
    


        #endregion
       
    }
}