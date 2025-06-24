using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Map
{

    
    public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image image;
        public Image visitedImage;
        
        public Node Node { get; private set; }
        public NodeTemplate Template { get; private set; }

        private float initialScale;
        private const float HOVERSCALEFACTOR = 1.2f;
        private float mouseDownTime;

        private const float MAXCLICKDURATION = 0.5f;

        public void SetUp(Node node, NodeTemplate template)
        {
            Node = node;
            Template = template;
            
            if (image != null) image.sprite = template.sprite;
            if (node.nodeType == NodeType.Boss) transform.localScale *= 1.5f;
            if (image != null) initialScale = image.transform.localScale.x;

            if (visitedImage != null)
            {
                //todo: color
                //visitedImage.color = MapView.Instance.visitedColor;
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
                        //image.color = MapView.Instance.lockedColor;
                    }

                    break;
                case NodeState.Visited:
                    if (image != null)
                    {
                        //image.color = MapView.Instance.visitedColor;
                    }
                    
                    if (visitedImage != null) visitedImage.gameObject.SetActive(true);
                    break;
                case NodeState.Attainable:
                    // start pulsating from visited to locked color:
                    if (image != null)
                    {
                        // image.color = MapView.Instance.lockedColor;
                        // image.DOKill();
                        // image.DOColor(MapView.Instance.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
           
            if (image != null)
            {
                //image.transform.DOKill();
                //image.transform.DOScale(initialScale * HoverScaleFactor, 0.3f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // if (sr != null)
            // {
            //     sr.transform.DOKill();
            //     sr.transform.DOScale(initialScale, 0.3f);
            // }
            //
            // if (image != null)
            // {
            //     image.transform.DOKill();
            //     image.transform.DOScale(initialScale, 0.3f);
            // }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            mouseDownTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // if (Time.time - mouseDownTime < MaxClickDuration)
            // {
            //     // user clicked on this node:
            //     MapPlayerTracker.Instance.SelectNode(this);
            // }
        }
        
        private void OnDestroy()
        {
            if (image != null)
            {
                // image.transform.DOKill();
                // image.DOKill();
            }

        }

    }
}