using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        private int _xDist;
        private int _yDist;
        private int _placementRandomness;

        private int _floors;
        private int _mapWidth;
        private int _paths;

        private float _BattleRoomWeight;
        private float _ShopRoomWeight;
        private float _EventRoomWeight;


        private List<float> _randomRoomTypeWeights = new()
        {
            0.0f, //(Room.Type.NotAssgined
            0.0f, //Room.Type.Battle, 
            0.0f, //Room.Type.Shop,
            0.0f, //Room.Type.Event, 
            0.0f, //Room.Type.Boss, 
        };

        private float _randomRoomTypeTotalWeight = 0.0f;

        private List<List<Node>> _map;


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
                int currentJ = j;
                for (int i = 1; i < _floors - 1; i++)
                {
                    currentJ = SetUpConnection(i, currentJ);
                }
            }

            //4. Room type 지정
            Node startNode = SetUpStartRoom();
            Node bossNode = SetUpBossRoom();
            SetUpRandomRoomWeights();
            SetUpRoomType();


            //5. MapData 생성
            MapData mapData = new MapData(_map, startNode, bossNode);

            return mapData;
        }


        private void InitSetting(MapConfig config)
        {
            _xDist = config.xDist;
            _yDist = config.yDist;
            _placementRandomness = config.placementRandomness;

            _floors = config.floors;
            _mapWidth = config.mapWidth;
            _paths = config.paths;

            _BattleRoomWeight = config.BattleRoomWeight;
            _ShopRoomWeight = config.ShopRoomWeight;
            _EventRoomWeight = config.EventRoomWeight;
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
                    Vector2 offset = new Vector2(Random.Range(0, 1), Random.Range(0, 1)) * _placementRandomness;

                    Vector2 position = new Vector2(j * _xDist, i * _yDist) + offset;

                    Node currentNode = new Node(i, j, position);

                    //start room position.y 값 고정
                    if (i == 0)
                        currentNode.position.y = i * _yDist;
                    //boss room 일 경우 position.y 값을 고정시킨다. (마지막 floor)
                    if (i == _mapWidth - 1)
                        currentNode.position.y = (i + 1) * _yDist;
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
                    currentNode.nextNodes.Add(bossNode);
                }
            }

            bossNode.nodeType = NodeType.Boss;

            return bossNode;
        }

        private void SetUpRandomRoomWeights()
        {
            _randomRoomTypeWeights[(int)NodeType.Battle] = _BattleRoomWeight;
            _randomRoomTypeWeights[(int)NodeType.Shop] = _BattleRoomWeight + _ShopRoomWeight;
            _randomRoomTypeWeights[(int)NodeType.Event] = _BattleRoomWeight + _ShopRoomWeight + _EventRoomWeight;
            _randomRoomTypeTotalWeight = _randomRoomTypeWeights[(int)NodeType.Event];
        }


        private void SetUpRoomType()
        {
            //2. 커스텀 규칙
            //예) 9th floor(floors / 2)는 항상 shop
            foreach (Node room in _map[_floors / 2])
            {
                if (room.HasConnections())
                    room.nodeType = NodeType.Shop;
            }

            //예) 4th floor는 항상 Event 발생
            foreach (Node room in _map[3])
            {
                if (room.HasConnections())
                    room.nodeType = NodeType.Event;
            }


            //3. 나머지 랜덤
            foreach (List<Node> currentFloor in _map)
            {
                foreach (Node room in currentFloor)
                {
                    //사용하는 방인데 타입이 정해지지 않은 경우
                    if (room.HasConnections() && room.nodeType == NodeType.NotAssgined)
                        SetRoomRandomly(room);
                }
            }
        }


        private void SetRoomRandomly(Node nodeToSet)
        {
            //룰 기반으로 한 플래그 설정
            //예) 4층 이내에 event room이 있으면 안된다, 연속으로 rest room 배치 불가능, 연속으로 shop room 배치 불가능, 보스룸 이전에 rest room을 강제로 배치해서 floors-3는 rest room이면 안됨

            bool eventBelow4 = true;
            // bool consecutiveEvent = true;
            bool consecutiveShop = true;
            //bool consecutiveEvent = true;
            bool eventOn13 = true;

            NodeType typeCandidate = NodeType.NotAssgined;
            while (eventBelow4 || consecutiveShop || eventOn13)
            {
                typeCandidate = GetRandomRoomTypeByWeight();

                //--------------flag 체크-------------
                bool isEventRoom = typeCandidate == NodeType.Event;
                bool hasEventRoomParent = HasParentOfType(nodeToSet, NodeType.Event);
                bool isShop = typeCandidate == NodeType.Shop;
                //bool isEvent = typeCandidate
                bool haseShopRoomParent = HasParentOfType(nodeToSet, NodeType.Shop);

                //rule을 어기지 않으면 아래 플래그들은 모두 false로 설정됨
                eventBelow4 = isEventRoom && nodeToSet.row < 3;
                consecutiveShop = isShop && haseShopRoomParent;
                eventOn13 = isEventRoom && nodeToSet.row == 12;
            }

            //rule을 어기지 않는 type candidate 획득
            nodeToSet.nodeType = typeCandidate;
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
            float roll = Random.Range(0f, _randomRoomTypeTotalWeight);

            for (int i = 0; i < _randomRoomTypeWeights.Count; i++)
            {
                float typeWeight = _randomRoomTypeWeights[i];
                if (typeWeight > roll)
                    return (NodeType)i;
            }

            return NodeType.Battle;
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