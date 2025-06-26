using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    /// ====== 맵 규칙 ======
    /// 1. path에서 전투는 2번만 들어가도록 한다
    /// 2. boss 이전 floor는 무조건 상점만
    /// 3. floor2는 무조건 상점
    /// 4. 연속 상점 불가능, 이벤트는 가능
    /// ======================
    
    public class MapGenerator : MonoBehaviour
    {
        private int _xDist;
        private int _yGap;
        private int _placementRandomness;

        private int _floors;
        private int _mapWidth;
        private int _paths;

        private float _BattleNodeWeight;
        private float _ShopNodeWeight;
        private float _EventNodeWeight;


        private List<float> _randomWeightList = new()
        {
            0.0f, //(Room.Type.NotAssgined
            0.0f, //Room.Type.Battle, 
            0.0f, //Room.Type.Shop,
            0.0f, //Room.Type.Event, 
            0.0f, //Room.Type.Boss, 
        };

        private float _totalWeight = 0.0f;

        private List<List<Node>> _map;
        private List<List<Node>> _allPath;

        //맵 생성
        public MapData GenerateMap(MapConfig config)
        {
            // setting 초기화
            InitSetting(config);
            
            //1. Grid 생성
            _map = GenerateInitialGrid();

            //2. floor0는 1개의 노드에서 시작 & floor1 부터 시작 - startPoint 랜덤으로 뽑기
            List<int> startingPoints = GetRandomStartingPoints();

            //3. Path 생성
            for (int j = 0; j < startingPoints.Count; j++)
            {
                int currentJ = startingPoints[j];
                for (int i = 1; i < _floors - 1; i++)
                {
                    currentJ = SetUpConnection(i, currentJ);
                }
            }

            //4. start Room, boss room setting
            Node startNode = SetUpStartRoom();
            Node bossNode = SetUpBossRoom();
            
            //5. 모든 path 탐색
            _allPath = GetAllPaths(startNode, bossNode);
            
            //6. RoomType 지정
            SetUpRandomNodeWeights();
            SetUpNodeType();
            
            //5. MapData 생성
            MapData mapData = new MapData(_map, _allPath, startNode, bossNode);

            return mapData;
        }


        private void InitSetting(MapConfig config)
        {
            _xDist = config.xDist;
            _yGap = config.yGap;
            _placementRandomness = config.placementRandomness;
        
            _floors = config.floors;
            _mapWidth = config.mapWidth;
            _paths = config.paths;
        
            _BattleNodeWeight = config.BattleNodeWeight;
            _ShopNodeWeight = config.ShopNodeWeight;
            _EventNodeWeight = config.EventNodeWeight;
        }

        //Grid 생성
        private List<List<Node>> GenerateInitialGrid()
        {
            List<List<Node>> grid = new(_floors);
            
            for (int i = 0; i < _floors; i++)
            {
                List<Node> floorRooms = new List<Node>(_mapWidth);
                for (int j = 0; j < _mapWidth; j++)
                {
                    Node currentNode = new Node(i, j);
                    
                    currentNode.nextNodes.Clear();
                    floorRooms.Add(currentNode);
                }
            
                grid.Add(floorRooms);
            }
            
            return grid;
        }
        

        //시작 포인트 랜덤으로 지정하기
        //최소 2개 이상의 unique 포인트를 지정해야한다.
        private List<int> GetRandomStartingPoints()
        {
            List<int> yCoordinates = new();
            int uniquePoints = 0;

            while (uniquePoints < 2)
            {
                uniquePoints = 0;
                yCoordinates.Clear();

                for (int i = 0; i < _paths; i++)
                {
                    int startingPoint = Random.Range(0, _mapWidth);
                    if (!yCoordinates.Contains(startingPoint))
                        uniquePoints += 1;
                    yCoordinates.Add(startingPoint);
                }
            }

            return yCoordinates;
        }


        private int SetUpConnection(int i, int j)
        {
            Node nextNode = null;
            Node currentNode = _map[i][j];

            //교차 가능성 확인, next room이 null이 아닐 때까지 다음 방 후보 찾기 
            while (nextNode == null || IsCrossingPath(i, j, nextNode))
            {
                int randomJ = Mathf.Clamp(Random.Range(j - 1, j + 2), 0, _mapWidth - 1);
                nextNode = _map[i + 1][randomJ];
            }

            currentNode.nextNodes.Add(nextNode);

            return nextNode.column;
        }

        private bool IsCrossingPath(int i, int j, Node node)
        {
            Node leftNeighbor = null;
            Node rightNeighbor = null;

            //j = 0 이면, left neighbor 없음
            if (j > 0)
                leftNeighbor = _map[i][j - 1];
            //j = mapWidth -1 이면, right neighbor 없음
            if (j < _mapWidth - 1)
                rightNeighbor = _map[i][j + 1];

            //cross 여부 판단
            //1) 오른쪽 이웃이 왼쪽으로 진행하는 path가 있으면 현재 room에서 오른쪽 방향으로 진행 불가
            if (rightNeighbor != null && node.column > j)
            {
                foreach (var nextRoom in rightNeighbor.nextNodes)
                {
                    if (nextRoom.column < node.column)
                        return true;
                }
            }

            //2) 왼쪽 이웃이 오른쪽으로 진행하는 path가 있으면 현재 room에서 왼쪽 방향으로 진행 불가
            if (leftNeighbor != null && node.column < j)
            {
                foreach (Node nextRoom in leftNeighbor.nextNodes)
                {
                    if (nextRoom.column > node.column)
                        return true;
                }
            }

            return false;
        }


        // - 시작 노드와 그 다음 floor와의 connection 설정
        // - room type 설정
        private Node SetUpStartRoom()
        {
            int middle = Mathf.FloorToInt(_mapWidth * 0.5f);
            Node startNode = _map[0][middle];

            for (int j = 0; j < _mapWidth; j++)
            {
                Node currentNode = _map[1][j];
                if (currentNode.HasConnections())
                {
                    startNode.nextNodes.Add(currentNode);
                }
            }

            startNode.nodeType = NodeType.Battle;
            return startNode;
        }

        //- 보스룸 이전 floor와 보스 룸 connection 설정
        //- room type 설정
        private Node SetUpBossRoom()
        {
            int middle = Mathf.FloorToInt(_mapWidth * 0.5F);
            Node bossNode = _map[_floors - 1][middle];

            for (int j = 0; j < _mapWidth; j++)
            {
                Node currentNode = _map[_floors - 2][j];
                if (currentNode.HasConnections())
                {
                    currentNode.nextNodes.Clear();
                    currentNode.nextNodes.Add(bossNode);
                }
            }

            bossNode.nodeType = NodeType.Boss;

            return bossNode;
        }
        
        private void SetUpRandomNodeWeights()
        {
            _randomWeightList[(int)NodeType.Battle] = _BattleNodeWeight;
            _randomWeightList[(int)NodeType.Shop] = _BattleNodeWeight + _ShopNodeWeight;
            _randomWeightList[(int)NodeType.Event] = _BattleNodeWeight + _ShopNodeWeight + _EventNodeWeight;
            _totalWeight = _randomWeightList[(int)NodeType.Event];
        }


        private void SetUpNodeType()
        {
            //===== 커스텀 규칙 ====
            //규칙 2. 2층은 상점
            foreach (Node room in _map[1])
            {
                if (room.HasConnections())
                    room.nodeType = NodeType.Shop;
            }

            //규칙 3. 보스룸 이전 방은 항상 상점
            foreach (Node room in _map[_floors-2])
            {
                if (room.HasConnections())
                    room.nodeType = NodeType.Shop;
            }

            // 나머지 랜덤
            foreach (List<Node> path in _allPath)
            {
                int battleCount = path.Count(n => n.nodeType == NodeType.Battle);
                
                // 후보 노드: 아직 타입이 지정되지 않은 노드
                List<Node> candidates = path
                    .Where(n => n.nodeType == NodeType.NotAssgined)
                    .ToList();
                
                List<Node> shuffled = candidates.OrderBy(_ => Random.value).ToList(); //랜덤 섞기
                
                foreach (Node node in shuffled)
                {
                    if (battleCount < 3) //시작 battle 포함해서 3개까지
                    {
                        node.nodeType = NodeType.Battle;
                        battleCount++;
                    }
                    else
                    {
                        if (node.row == _floors - 3)
                            node.nodeType = NodeType.Event;
                        else
                        {
                            NodeType candidateType = NodeType.NotAssgined;
                            // 상점은 부모가 상점이면 안 됨 (연속 상점 방지)
                            bool isConsecutiveShop = true;
                            while (isConsecutiveShop)
                            {
                                candidateType = GetRandomRoomTypeByWeight();
                                bool isShop = candidateType == NodeType.Shop;
                                isConsecutiveShop = isShop & HasParentOfType(node, NodeType.Shop);
                            }

                            node.nodeType = candidateType;
                        }
                       
                    }
                }
            }
        }

        private List<List<Node>> GetAllPaths(Node start, Node end)
        {
            List<List<Node>> allPaths = new();
            List<Node> currentPath = new();
            DFS(start, end, currentPath, allPaths);
            return allPaths;
        }

        private void DFS(Node current, Node end, List<Node> path, List<List<Node>> allPaths)
        {
            path.Add(current);
            if(current == end)
                allPaths.Add(new List<Node>(path));
            else
            {
                foreach (Node nextNode in current.nextNodes)
                {
                    DFS(nextNode, end, path, allPaths);
                }
            }
            path.RemoveAt(path.Count-1);
        }
        
        

        private bool HasParentOfType(Node node, NodeType roomType)
        {
            List<Node> parent = new();
            //left parent
            if (node.column > 0 && node.row > 0)
            {
                Node parentCandidate = _map[node.row - 1][node.column - 1];
                if (parentCandidate.nextNodes.Contains(node))
                    parent.Add(parentCandidate);
            }

            //below parent
            if (node.column > 0)
            {
                Node parentCandidate = _map[node.row - 1][node.column];
                if (parentCandidate.nextNodes.Contains(node))
                    parent.Add(parentCandidate);
            }

            //right parent
            if (node.column < _mapWidth - 1 && node.row > 0)
            {
                Node parentCandidate = _map[node.row - 1][node.column + 1];
                if (parentCandidate.nextNodes.Contains(node))
                    parent.Add(parentCandidate);
            }

            foreach (Node pRoom in parent)
            {
                if (pRoom.nodeType == roomType)
                    return true;
            }

            return false;
        }

        private NodeType GetRandomRoomTypeByWeight()
        {
            float roll = Random.Range(0f, _totalWeight);

            for (int i = 0; i < _randomWeightList.Count; i++)
            {
                float typeWeight = _randomWeightList[i];
                if (typeWeight > roll)
                    return (NodeType)i;
            }

            return NodeType.Event;
        }


        private void PrintMap()
        {
            List<string> printStr = new List<string>();
            for (int i = 0; i < _floors; i++)
            {
                printStr.Add($"floor {i} : ");
                for (int j = 0; j < _map[i].Count; j++)
                {
                    printStr.Add(_map[i][j].ToString());
                    if (j != _map.Count - 1)
                        printStr.Add(", ");
                }

                printStr.Add("\n");
            }

            string result = string.Join("", printStr);
            Debug.Log(result);
        }
    }
}