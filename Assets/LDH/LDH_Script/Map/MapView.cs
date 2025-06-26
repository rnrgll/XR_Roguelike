using CustomUtility.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.UI.Extensions;

namespace Map
{
    public class MapView : MonoBehaviour
    {
        public static MapView Instance; 
        
        [Header("Map Configuration")]
        public MapNode mapNodePrefab;
        
        [Header("Background Settings")] public Sprite background;
        public Color32 backgroundColor = Color.white;
       
        [Header("Line Settings")] public UILineRenderer uiLinePrefab;
        [Tooltip("Line point count should be > 2 to get smooth color gradients")]
        [Range(3, 10)]
        public int linePointsCount = 10;
        [Tooltip("Distance from the node till the line starting point")]
        public float offsetFromNodes = 0.5f;

        [Header("Colors")] 
        [Tooltip("Node Visited or Attainable color")] public Color32 visitedColor = Color.white;
        [Tooltip("Locked node color")] public Color32 lockedColor = Color.gray;
        [Tooltip("Visited or available path color")] public Color32 lineVisitedColor = Color.white;
        [Tooltip("Unavailable path color")] public Color32 lineLockedColor = Color.gray;


        [Header("UI Map Settings")]
        [SerializeField] private ScrollRect scrollRectVertical;
       
        [Tooltip("Padding of the first and last rows of nodes from the sides of the scroll rect")] 
        [SerializeField]
        private float padding;

        [Tooltip("Padding of the background from the sides of the scroll rect")] 
        [SerializeField]
        private Vector2 backgroundPadding;

        [Tooltip("Pixels per Unit multiplier for the background image")] [SerializeField]
        private float backgroundPPUMultiplier = 1;
        
        //-------------------------------
        //map parent
        private GameObject firstParent;
        private GameObject mapParent;
        
        //map data
        public MapData Map { get; private set; }
        
        //map nodes, path list
        public readonly List<MapNode> MapNodes = new ();
        
        private List<List<Vector2Int>> paths;

        private Camera cam;
        protected readonly List<LineConnection> lineConnections = new List<LineConnection>();

        
        
        public virtual void ShowMap(MapData m)
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

            CreateMapBackground();
        }


        /// <summary>
        /// 맵 초기화
        /// </summary>
        private void ClearMap()
        {
            scrollRectVertical.gameObject.SetActive(false);
            foreach (Transform child in scrollRectVertical.content)
            {
                Destroy(child.gameObject);
            }

            MapNodes.Clear();
            lineConnections.Clear();
            // nodeToMapNode.Clear();
        }

        /// <summary>
        /// Map Parent 생성
        /// </summary>
        private void CreateMapParent()
        {
            scrollRectVertical.gameObject.SetActive(true);

            //ScrollRect Content 아래 맵을 담을 부모 생성
            
            //1. 
            firstParent = new GameObject("OuterMapParent");
            firstParent.transform.SetParent(scrollRectVertical.content);
            firstParent.transform.localScale = Vector3.one;
            RectTransform fprt = firstParent.AddComponent<RectTransform>();
            UILayout.Stretch(fprt);

            mapParent = new GameObject("MapParentWithAScroll");
            mapParent.transform.SetParent(firstParent.transform);
            mapParent.transform.localScale = Vector3.one;
            RectTransform mprt = mapParent.AddComponent<RectTransform>();
            UILayout.Stretch(mprt, new Vector4(200,0,200,0));

            
            //맵 전체 길이 계산해서 스크롤 영역(Content) 크기 설정하기
            SetMapLength();
            
            //맵 스트롤 위치 초기화
            ScrollReset();
        }
        
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

        private void SetAttainableNodes()
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

        private void SetLineColors()
        {
            // 모든 라인을 회색으로 초기화
            foreach (LineConnection connection in lineConnections)
                connection.SetColor(lineLockedColor);
            
            //아직 방문한 경로가 없다면 return
            if (Manager.Map.CurrentMap.Path.Count == 0)
                return;
            
            // 경로에 포함된 path 색상 변경
            // 마지막 방문 노드
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
            
            for (int i = 0; i < Manager.Map.CurrentMap.Path.Count - 1; i++)
            {
                Vector2Int current = Manager.Map.CurrentMap.Path[i];
                Vector2Int next =Manager.Map.CurrentMap.Path[i + 1];
                
                LineConnection line = lineConnections.FirstOrDefault(l => l.from.Node.row == current.x && l.from.Node.column == current.y && l.to.Node.row == current.x && l.to.Node.column == current.y
                    );
                line?.SetColor(lineVisitedColor);
            }
            
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


        #region UI Setting

        private void SetMapLength()
        {
            RectTransform content = scrollRectVertical.content;
            Vector2 sizeDelta = content.sizeDelta;

            float length = padding + Manager.Map.config.yGap * (Manager.Map.config.floors - 1);

            sizeDelta.y = length;

            content.sizeDelta = sizeDelta;
            
        }

        private void ScrollReset()
        {
            scrollRectVertical.normalizedPosition = Vector2.zero;
        }

        #endregion

        #region Create Node / Line

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
            
            float x = (mapParentSize.x - padding) * ( (float)node.column / (Manager.Map.config.mapWidth - 1) - 0.5f);
            float y = (Manager.Map.config.yGap) * ((float)node.row - (Manager.Map.config.floors) / 2f + 0.5f);

            Vector2 localPos = new Vector2(x, y) + offset;
            return localPos;
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
        
        protected MapNode GetMapNode(Vector2Int p)
        {
            return MapNodes.FirstOrDefault(n => n.Node.point.Equals(p));
        }


        #endregion
        
        
    }
}