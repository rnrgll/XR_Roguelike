using CustomUtility.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI.Extensions;

namespace Map
{
    public class MapView : MonoBehaviour
    {
        [Header("Map Configuration")]
        public MapNode mapNodePrefab;
        
        [Header("Background Settings")] 
        public Sprite background;
        public Color32 backgroundColor = Color.white;
       
        [Header("Colors")] 
        public static Color32 visitedColor =  new Color32(0x23, 0x3B, 0x54, 0xFF);//방문 or 도달 가능한 노드 색상
        public static Color32 lockedColor = Color.gray; // 잠긴 노드 색상
        public static Color32 lineVisitedColor =  new Color32(0x23, 0x3B, 0x54, 0xFF); // 방문 or 도달 가능한 경로 색상
        public static Color32 lineLockedColor = Color.gray; // 잠긴 경로 색상

        
        [Header("Line Settings")] 
        public UILineRenderer uiLinePrefab;
        [Range(3, 10)] public int linePointsCount = 10; //부드러운 라인 그라데이션을 위해선 3개 이상의 point 권장
        public float offsetFromNodes = 0.5f; // 노드에서 라인 시작점 사이의 offest
        

        [Header("UI Map Settings")]
        public ScrollRect scrollRect;
        [SerializeField] private float widthPadding; // 맵 전체의 좌우 여백
        [SerializeField] private float heightPadding; // 맵 전체의 상하 여백
        [SerializeField] private Vector2 backgroundPadding; // 배경 이미지 좌우 여백
        [SerializeField] private float backgroundPPUMultiplier = 1; // 배경 이미지 Pixels Per Unit 배율
        
        [Header("Map UI")] 
        public Button closeButton;
        
        //Map Data
        public MapData Map { get; private set; } = null;
        //map nodes, path list
        public readonly List<MapNode> MapNodes = new ();
        
        
        //============= 내부 상태 필드================//
        //map parent
        private GameObject firstParent;
        private GameObject mapParent;
        
        // 유틸
        private Camera cam;
        private readonly List<LineConnection> lineConnections = new List<LineConnection>();

        //======================================//


        #region Map Generator Main Logic
        
        public void CreateMapView(MapData m)
        {
            if (m == null)
            {
                Debug.LogWarning("Map was null in MapView.ShowMap()");
                return;
            }

            Map = m;

            ClearMap();

            CreateMapParent();

            CreateNodes();

            DrawLines();

            SetAttainableNodes();

            SetLineColors();

            //CreateMapBackground();
        }
        #endregion

        #region Clear Map - 맵 초기화

        /// <summary>
        /// 맵 초기화
        /// </summary>
        private void ClearMap()
        {
            scrollRect.gameObject.SetActive(false);
            foreach (Transform child in scrollRect.content)
            {
                Destroy(child.gameObject);
            }

            MapNodes.Clear();
            lineConnections.Clear();
            // nodeToMapNode.Clear();
        }

        #endregion

        #region Map Container, Background 설정

        /// <summary>
        /// Map Parent 생성
        /// </summary>
        private void CreateMapParent()
        {
            scrollRect.gameObject.SetActive(true);

            //ScrollRect Content 아래 맵을 담을 부모 생성
            
            //1. 
            firstParent = new GameObject("OuterMapParent");
            firstParent.transform.SetParent(scrollRect.content);
            firstParent.transform.localScale = Vector3.one;
            RectTransform fprt = firstParent.AddComponent<RectTransform>();
            UILayout.Stretch(fprt);

            mapParent = new GameObject("MapParentWithAScroll");
            mapParent.transform.SetParent(firstParent.transform);
            mapParent.transform.localScale = Vector3.one;
            RectTransform mprt = mapParent.AddComponent<RectTransform>();
            UILayout.Stretch(mprt);

            
            //맵 전체 길이 계산해서 스크롤 영역(Content) 크기 설정하기
            SetMapLength();
            
            //맵 스트롤 위치 초기화
            StartCoroutine(ScrollReset());
        }
        
        
                
        private void CreateMapBackground()
        {
            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(mapParent.transform);
            backgroundObject.transform.localScale = Vector3.one;
            RectTransform rt = backgroundObject.AddComponent<RectTransform>();
            UILayout.Stretch(rt);
            rt.SetAsFirstSibling();
            rt.sizeDelta = backgroundPadding;
            
            Image image = backgroundObject.AddComponent<Image>();
            image.color = backgroundColor;
            image.type = Image.Type.Sliced;
            image.sprite = background;
            image.pixelsPerUnitMultiplier = backgroundPPUMultiplier;
            
        }


        #endregion

        #region Node, Line 생성

        #region Node
        
        /// <summary>
        /// Map에 들어갈 Node (Map Node instance) 생성
        /// </summary>
        private void CreateNodes()
        {
            foreach (Node node in Map.Nodes)
            {
                MapNode mapNode = CreateMapNode(node);
                MapNodes.Add(mapNode);
                // nodeToMapNode[node] = mapNode;
                
            }
        }
        private MapNode CreateMapNode(Node node)
        {
            MapNode mapNodeObj = Instantiate(mapNodePrefab, mapParent.transform);
            mapNodeObj.transform.rotation = Quaternion.identity;
            NodeTemplate template = GetTemplate(node.nodeType);

            mapNodeObj.SetUp(node, template);
            
            mapNodeObj.transform.localPosition = SetNodePosition(node);

            return mapNodeObj;
        }
        
        private NodeTemplate GetTemplate(NodeType nodeType)
        {
            return Manager.Map.config.NodeTemplates.FirstOrDefault(template => template.nodeType == nodeType);
        }
        
        private Vector2 SetNodePosition(Node node)
        {
            Vector2 mapParentSize = mapParent.GetComponent<RectTransform>().rect.size;
            
            Vector2 offset = new Vector2(Manager.randomManager.RandFloat(0, 1), Manager.randomManager.RandFloat(0, 1)) *
                             Manager.Map.config.placementRandomness;
            
            float x = (mapParentSize.x - widthPadding) * ( (float)node.column / (Manager.Map.config.mapWidth - 1) - 0.5f);
            float y = (Manager.Map.config.yGap) * ((float)node.row - (Manager.Map.config.floors) / 2f + 0.5f);

            Vector2 localPos = new Vector2(x, y) + offset;
            return localPos;
        }

        #endregion

        #region Line

        //path line 그리기
        private void DrawLines()
        {
            foreach (MapNode mapNode in MapNodes)
            {
                if (!mapNode.Node.HasConnections()) break;
                foreach (Node nextNode in mapNode.Node.nextNodes)
                {
                    MapNode nextMapNode = GetMapNode(nextNode.point);
                    AddLine(mapNode, nextMapNode);
                }
            }
        }
        
        private void AddLine(MapNode from, MapNode to)
        {
            if(uiLinePrefab == null) return;
            
            UILineRenderer line = Instantiate(uiLinePrefab);
            line.transform.SetParent(mapParent.transform);
            line.transform.SetAsFirstSibling();
            
            RectTransform fromRect = from.transform as RectTransform;
            RectTransform toRect = to.transform as RectTransform;
            
            //from => to 방향 벡터를 구하고 거기에 offest을 곱해서 살짝 떨어진 위치를 구하기
            Vector2 fromPoint = fromRect.anchoredPosition
                                + (toRect.anchoredPosition - fromRect.anchoredPosition).normalized
                                * offsetFromNodes;
            //마찬가지로 to => from 방향벡터 구하고 offset 곱해서 거리 보정
            Vector2 toPoint = toRect.anchoredPosition
                              + (fromRect.anchoredPosition - toRect.anchoredPosition).normalized
                              * offsetFromNodes;
            
            //line 그리기
            line.transform.position = from.transform.position +
                                      (Vector3)(toRect.anchoredPosition - fromRect.anchoredPosition).normalized *
                                      offsetFromNodes;

            List<Vector2> pointList = new ();
            for (int i = 0; i < linePointsCount; i++)
            {
                pointList.Add(Vector3.Lerp(
                    Vector3.zero,
                    toPoint - fromPoint,
                    (float)i/(linePointsCount-1)
                ));
            }

            line.Points = pointList.ToArray();
            lineConnections.Add(new LineConnection(line, from, to));

        }


        #endregion
        
        #endregion


        #region Node, Line 설정

        public void SetAttainableNodes()
        {
            foreach (MapNode node in MapNodes)
                node.SetState(NodeState.Locked);
            
            //아직 방문한 경로가 없을 때
            if (Manager.Map.CurrentMap.Path.Count == 0)
            {
                GetMapNode(Map.StartNode.point).SetState(NodeState.Attainable);
            }
            else
            {
                foreach (Vector2Int point in Manager.Map.CurrentMap.Path)
                {
                    Node node = Map.GetNodeByPoint(point.x, point.y);
                    if (node == null)
                    {
                        Debug.LogError("Map View : SetAttainable Nodes - Point Error");
                        return;
                    }
                    
                    GetMapNode(node.point).SetState(NodeState.Visited);
                    
                }

                Vector2Int currentPoint = Manager.Map.CurrentMap.Path[Manager.Map.CurrentMap.Path.Count - 1];

                Node currentNode = Manager.Map.CurrentMap.GetNodeByPoint(currentPoint.x, currentPoint.y);

                foreach (Node nextNode in currentNode.nextNodes)
                {
                    GetMapNode(nextNode.point).SetState(NodeState.Attainable);
                }
            }
        }
        
        
        public void SetLineColors()
        {
            // 모든 라인을 회색으로 초기화
            foreach (LineConnection connection in lineConnections)
                connection.SetColor(lineLockedColor);
            
            //아직 방문한 경로가 없다면 return
            if (Manager.Map.CurrentMap.Path.Count == 0)
                return;
            
            // 경로에 포함된 path 색상 변경
            // 마지막 방문 노드 = 현재 노드 이후의 attainable 한 경로 설정
            Vector2Int currentPoint = Manager.Map.CurrentMap.Path[Manager.Map.CurrentMap.Path.Count - 1];
            Node currentNode = Manager.Map.CurrentMap.GetNodeByPoint(currentPoint.x, currentPoint.y);
            
            foreach (Node nextNode in currentNode.nextNodes)
            {
                LineConnection line =
                    lineConnections.FirstOrDefault(l => l.from.Node.point == currentPoint && l.to.Node.point == nextNode.point);
                GetMapNode(nextNode.point).SetState(NodeState.Attainable);
                
                line?.SetColor(lineVisitedColor);
            }
            
            if (Manager.Map.CurrentMap.Path.Count <= 1) return;
            
            //이전 경로 path 색깔 설정
            for (int i = 0; i < Manager.Map.CurrentMap.Path.Count - 1; i++)
            {
                Vector2Int current = Manager.Map.CurrentMap.Path[i];
                Vector2Int next =Manager.Map.CurrentMap.Path[i + 1];
                
                LineConnection line = lineConnections.FirstOrDefault(l => l.from.Node.row == current.x && l.from.Node.column == current.y && l.to.Node.row == next.x && l.to.Node.column == next.y
                );
                line?.SetColor(lineVisitedColor);
            }   
            
        }

        #endregion
     


        #region UI Setting

        private void SetMapLength()
        {
            RectTransform content = scrollRect.content;
            Vector2 sizeDelta = content.sizeDelta;

            float length = heightPadding + Manager.Map.config.yGap * (Manager.Map.config.floors - 1);

            sizeDelta.y = length;

            content.sizeDelta = sizeDelta;
            
        }

        private IEnumerator ScrollReset()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            scrollRect.normalizedPosition = Vector2.zero;
        }


        public void SetShowType(ShowType type)
        {
            closeButton.gameObject.SetActive(type==ShowType.View);
            foreach (MapNode mapNode in MapNodes)
            {
                mapNode.IsLocked = type == ShowType.View;
            }
            
        }

        #endregion

        #region Util
        public MapNode GetMapNode(Vector2Int p)
        {
            return MapNodes.FirstOrDefault(n => n.Node.point.Equals(p));
        }
        #endregion
        
        
    }
}