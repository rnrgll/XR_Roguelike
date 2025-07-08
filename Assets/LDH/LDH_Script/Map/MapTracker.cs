using DG.Tweening;
using Item;
using Managers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Map
{
    public class MapTracker : MonoBehaviour
    {
        public float enterNodeDelay = 1.5f;
        
        private void OnDestroy()
        {
            DOTween.Kill(this);

        }
        
        
        public void SelectNode(MapNode mapNode)
        {
            if (mapNode.NodeState == NodeState.Locked)
                return;

            if (mapNode.NodeState == NodeState.Attainable)
            {
                  VisitNode(mapNode);
            }
        }

        
        
        public void VisitNode(MapNode mapNode)
        {
            EventSystem eventSystem = EventSystem.current;
            eventSystem.enabled = false;
            Manager.Map.CurrentMap.Path.Add(mapNode.Node.point);
            Manager.Map.View.SetAttainableNodes();
            Manager.Map.View.SetLineColors();
            mapNode.ShowVisitedCircle();
            
            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() =>
            {
                eventSystem.enabled = true;
                mapNode.ResetScale();
                EnterNode(mapNode);
            });
            
        }

        public void EnterNode(MapNode mapNode)
        {
            var pc = TurnManager.Instance.GetPlayerController();
            switch (mapNode.Node.nodeType)
            {
                case NodeType.Battle:
                    //todo : 배틀 씬으로 전환 및 설정
                    SceneManager.LoadScene("BattleScene");
                    ItemManager.Instance.SetInventorySlotState(false);

                    break;
                case NodeType.Shop:
                    //todo : 인게임 상점 씬으로 전환
                    SceneManager.LoadScene("InGameShop");
                    ItemManager.Instance.SetInventorySlotState(true);
                    pc.SetSpriteVisible(false);
                    break;
                case NodeType.Event:
                    //todo : 이벤트 씬으로 전환
                    SceneManager.LoadScene("EventScene");
                    ItemManager.Instance.SetInventorySlotState(true);
                    pc.SetSpriteVisible(false);
                    break;
                case NodeType.Boss:
                    //todo : boss battle 로 전환
                    SceneManager.LoadScene("BossBattleScene");
                    ItemManager.Instance.SetInventorySlotState(false);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Manager.Map.HideMap();
        }
        
        
    }
}