using CustomUtility.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using System.Linq;

namespace Map
{
    public class MapView : MonoBehaviour
    {
        public static MapView Instance; 
        
        [Header("Map Configuration")]
        public MapConfig mapConfig;
        public MapNode mapNodePrefab;
        public float offset; //start node, end node와 화면 가장자리 offset

        [Header("Background Settings")] public Sprite background;
        public Color32 backgroundColor = Color.white;
        public float xSize;
        public float yOffset;

        [Header("Line Settings")] public GameObject linePrefab;

        [Tooltip("Distance from the node till the line starting point")]
        public float offsetFromNodes = 0.5f;

        [Header("Colors")] [Tooltip("Node Visited or Attainable color")]
        public Color32 visitedColor = Color.white;

        [Tooltip("Locked node color")] public Color32 lockedColor = Color.gray;

        [Tooltip("Visited or available path color")]
        public Color32 lineVisitedColor = Color.white;

        [Tooltip("Unavailable path color")] public Color32 lineLockedColor = Color.gray;


        [Header("UI Map Settings")]
        [SerializeField]
        private ScrollRect scrollRectVertical;
        
        [Tooltip(
            "Multiplier to compensate for larger distances in UI pixels on the canvas compared to distances in world units")]
        [SerializeField]
        private float unitsToPixelsMultiplier = 10f;

        [Tooltip("Padding of the first and last rows of nodes from the sides of the scroll rect")] [SerializeField]
        private float padding;

        [Tooltip("Padding of the background from the sides of the scroll rect")] [SerializeField]
        private Vector2 backgroundPadding;

        [Tooltip("Pixels per Unit multiplier for the background image")] [SerializeField]
        private float backgroundPPUMultiplier = 1;

        [Tooltip("Prefab of the UI line between the nodes (uses scripts from Unity UI Extensions)")] [SerializeField]
        private UILineRenderer uiLinePrefab;

        
        //-------------------------------
        //map parent
        private GameObject firstParent;
        private GameObject mapParent;
        
        //map data
        public MapData Map { get; protected set; }
        
        //map nodes, path list
        public readonly List<MapNode> MapNodes = new List<MapNode>();
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

            CreateNodes(m.Nodes);

            DrawLines();

            SetOrientation();

            ResetNodesRotation();

            SetAttainableNodes();

            SetLineColors();

            CreateMapBackground(m);
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
            UILayout.Stretch(mprt);

            
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
                
            }
        }

        //path line 그리기
        private void DrawLines()
        {
            foreach (MapNode mapNode in MapNodes)
            {
                foreach (Node nextNode in mapNode.Node.nextNodes)
                {
                    AddLine(mapNode, nextNode);
                }
            }
        }

        private void SetOrientation();

        private void ResetNodesRotation();

        private void SetAttainableNodes();

        private void SetLineColors();

        private void CreateMapBackground(m);


        #region UI Setting

        private void SetMapLength()
        {
            RectTransform content = scrollRectVertical.content;
            Vector2 sizeDelta = content.sizeDelta;
            
            float length = padding + Map.GetMapLength() * unitsToPixelsMultiplier;

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
            NodeTemplate template = GetTemplate(node.nodeType);
            mapNodeObj.SetUp(node, template);

            return mapNodeObj;
        }
        
        
        private NodeTemplate GetTemplate(NodeType nodeType)
        {
           return Manager.Map.config.NodeTemplates.FirstOrDefault(template => template.nodeType == nodeType);
        }
        
        
        

        #endregion
    }
}